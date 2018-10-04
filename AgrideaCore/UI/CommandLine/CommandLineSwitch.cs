using Agridea.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Agridea.UI.CommandLine
{
    #region CommandLineSwitch

    public abstract class CommandLineSwitch
    {
        #region Constants

        private static readonly Regex EndMarkerRegex = new Regex(@"^\s*[-/]{2,}\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex PrefixRegex = new Regex(@"^\s*[-/]\s*(?<switch>[^-/].*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex NameValueRegex = new Regex(@"^\s*(?<name>\w+)((\s*[=:]\s*|\s+)(?<value>.*)|(?<value>\W*))$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex SepValueRegex = new Regex(@"^\s*([=:]\s*)?(?<value>.*)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        #endregion Constants

        #region Factory

        internal static IEnumerable<CommandLineSwitch> CreateSwitchesForType(object objectWithSwitchAttributes)
        {
            Type type = objectWithSwitchAttributes.GetType();
            MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty);
            for (int i = 0; i < members.Length; ++i)
            {
                object[] attributes = members[i].GetCustomAttributes(false);
                if (attributes.Length > 0)
                {
                    CommandLineSwitch clSwitch = null;
                    foreach (Attribute attribute in attributes)
                    {
                        if (attribute is CommandLineSwitchAttribute)
                        {
                            CommandLineSwitchAttribute switchAttribute = attribute as CommandLineSwitchAttribute;
                            // Get the property information.  We're only handling properties at the moment!
                            if (members[i] is PropertyInfo)
                            {
                                PropertyInfo pi = (PropertyInfo)members[i];
                                string switchName = switchAttribute.Name;
                                if (switchName == null || switchName.Length == 0)
                                    switchName = pi.Name;
                                clSwitch = CommandLineSwitch.Create(switchName, switchAttribute.Description, pi.PropertyType);
                                // Map in the Get/Set methods.
                                clSwitch.SetMethod = pi.GetSetMethod(true);
                                clSwitch.GetMethod = pi.GetGetMethod(true);
                                clSwitch.Target = objectWithSwitchAttributes;
                                // Can only handle a single switch for each property
                                // (otherwise the parsing of aliases gets silly...)
                                break;
                            }
                        }
                    }
                    // See if any aliases are required.  We can only do this after
                    // a switch has been registered and the framework doesn't make
                    // any guarantees about the order of attributes, so we have to
                    // walk the collection a second time.
                    if (clSwitch != null)
                    {
                        foreach (Attribute attribute in attributes)
                        {
                            if (attribute is CommandLineSwitchAliasAttribute)
                            {
                                CommandLineSwitchAliasAttribute aliasAttribute = attribute as CommandLineSwitchAliasAttribute;
                                clSwitch.AddAlias(aliasAttribute.Alias);
                            }
                        }
                    }
                    // Assuming we have an switch (that may or may not have aliases), add it to the collection of switches.
                    if (clSwitch != null)
                        yield return clSwitch;
                }
            }
        }

        public static CommandLineSwitch Create(string name, string description, Type valueType)
        {
            if (valueType == typeof(bool))
                return new CommandLineBooleanSwitch(name, description);
            else if (valueType == typeof(int))
                return new CommandLineIntSwitch(name, description);
            else if (valueType == typeof(double))
                return new CommandLineDoubleSwitch(name, description);
            else if (valueType.IsEnum)
                return new CommandLineEnumSwitch(name, description, valueType);
            else if (valueType == typeof(DateTime))
                return new CommandLineDateTimeSwitch(name, description);
            else if (valueType == typeof(string))
                return new CommandLineStringSwitch(name, description);
            else
                throw new ArgumentException("Currently only bool, int, double, Enum and string types are supported");
        }

        #endregion Factory

        #region Members

        private List<string> _aliases;

        #endregion Members

        #region Initialization

        protected CommandLineSwitch(string name, string description)
        {
            Name = name;
            Description = description;
        }

        #endregion Initialization

        #region Object

        public override string ToString()
        {
            string name = MatchedName != null && MatchedName.Length > 0 ? MatchedName : Name;
            if (Value == null)
                return string.Format("-{0}", name);
            else
                return string.Format("-{0}={1}", name, Value);
        }

        #endregion Object

        #region Queries

        public string Name { get; private set; }

        public ReadOnlyCollection<string> Aliases { get { return _aliases != null ? _aliases.AsReadOnly() : new ReadOnlyCollection<string>(new List<string>()); } }

        public string Description { get; private set; }

        public bool NameMatchExactly { get; private set; }

        public string MatchedName { get; private set; }

        public string ParsedName { get; private set; }

        public int ParsedLength { get; private set; }

        public int ParsedArgsCount { get; private set; }

        public bool EndMarkerSwitch { get; private set; }

        public string RemainingArg { get; private set; }

        public object Value { get; protected set; }

        public virtual object DefaultValue { get { return null; } }

        public abstract Type ValueType { get; }

        public string[] Enumerations
        {
            get
            {
                if (ValueType.IsEnum)
                    return Enum.GetNames(ValueType);
                else
                    return null;
            }
        }

        #endregion Queries

        #region Properties

        public object GetTargetValue()
        {
            if (Target != null && GetMethod != null)
                return GetMethod.Invoke(Target, null);
            return null;
        }

        public void SetTargetValue(object value)
        {
            if (Target != null && SetMethod != null)
                SetMethod.Invoke(Target, new object[] { value });
        }

        #endregion Properties

        #region Commands

        public void AddAlias(string alias)
        {
            if (_aliases == null)
                _aliases = new List<string>();
            _aliases.Add(alias);
        }

        public void Parse(string[] args, int startIndex)
        {
            Reset();
            // match prefix
            Match match = PrefixRegex.Match(args[startIndex]);
            if (match.Success)
            {
                string switchNameValuePair = match.Groups["switch"].Value;    // name + value (named group)
                // match separator
                match = NameValueRegex.Match(switchNameValuePair);
                if (match.Success)
                {
                    // 1. Parse switch name
                    string switchName = match.Groups["name"].Value;   // switch name (named group)
                    int parsedLength = ParseName(switchName);
                    if (parsedLength > 0)
                    {
                        ParsedLength += parsedLength;
                        // 2. Parse switch value
                        string switchValue = match.Groups["value"].Value; // try switch value (all text before match)
                        if (switchValue.Length == 0)
                        {
                            // switch value not on first arg -> arg[startIndex + 1]
                            ParsedArgsCount += 1;
                            if (startIndex + 1 < args.Length)
                            {
                                match = SepValueRegex.Match(args[startIndex + 1]);
                                if (match.Success)
                                {
                                    switchValue = match.Groups["value"].Value;
                                    if (switchValue.Length == 0)
                                    {
                                        // switch value not on first arg -> arg[startIndex + 2]
                                        ParsedArgsCount += 1;
                                        if (startIndex + 2 < args.Length)
                                            switchValue = args[startIndex + 2];
                                    }
                                }
                            }
                        }
                        if (switchValue.Length > 0)
                        {
                            parsedLength = ParseValue(switchValue);
                            if (parsedLength > 0)
                            {
                                ParsedLength += parsedLength;
                                ParsedArgsCount += 1;
                            }
                            else
                            {
                                match = EndMarkerRegex.Match(switchValue);
                                if (match.Success)
                                {
                                    EndMarkerSwitch = true;
                                    string arg = match.Result("$'");
                                    if (arg.Length > 0)
                                        RemainingArg = arg;
                                    ParsedArgsCount += 1;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                match = EndMarkerRegex.Match(args[startIndex]);
                if (match.Success)
                {
                    EndMarkerSwitch = true;
                    string arg = match.Result("$'");
                    if (arg.Length > 0)
                        RemainingArg = arg;
                    ParsedArgsCount += 1;
                }
            }
        }

        internal int Commit()
        {
            if (Value == null)
            {
                if (!EndMarkerSwitch)
                    throw new InvalidOperationException("Value should not be null (switch is not parsed)");
            }
            else
                SetTargetValue(Value);
            return ParsedArgsCount;
        }

        protected abstract int ParseValue(string switchValue);

        #endregion Commands

        #region Helpers

        private MethodInfo SetMethod { get; set; }

        private MethodInfo GetMethod { get; set; }

        private object Target { get; set; }

        private IEnumerable<string> Names
        {
            get
            {
                yield return Name;
                if (Aliases != null)
                {
                    foreach (string alias in Aliases)
                        yield return alias;
                }
            }
        }

        private IEnumerable<string> SwitchNamesSortedByLength
        {
            get { return Names.OrderBy(s => s, StringLengthComparer.Instance).Reverse(); }
        }

        private int ParseName(string switchName)
        {
            ParsedName = switchName;
            foreach (string item in Names)
            {
                if (string.Compare(item, switchName, true) == 0)
                {
                    NameMatchExactly = true;
                    MatchedName = item;
                    return switchName.Length;
                }
            }
            foreach (string item in SwitchNamesSortedByLength)
            {
                if (item.StartsWith(switchName, StringComparison.InvariantCultureIgnoreCase))
                {
                    MatchedName = item;
                    return switchName.Length;
                }
            }
            return 0;
        }

        private void Reset()
        {
            Value = DefaultValue;
            NameMatchExactly = false;
            MatchedName = string.Empty;
            ParsedName = string.Empty;
            ParsedLength = 0;
            ParsedArgsCount = 0;
            EndMarkerSwitch = false;
            RemainingArg = string.Empty;
        }

        #endregion Helpers
    }

    #endregion CommandLineSwitch

    #region CommandLineBooleanSwitch

    public class CommandLineBooleanSwitch : CommandLineSwitch
    {
        #region Initialization

        internal CommandLineBooleanSwitch(string name, string description)
            : base(name, description)
        {
        }

        #endregion Initialization

        #region CommandLineSwitch

        public override object DefaultValue
        {
            get
            {
                object value = GetTargetValue();
                if (value == null)
                    return true;
                else
                    return !(bool)value;
            }
        }

        public override Type ValueType { get { return typeof(bool); } }

        protected override int ParseValue(string switchValue)
        {
            bool result;
            if (bool.TryParse(switchValue, out result))
            {
                Value = result;
                return switchValue.Length;
            }
            return 0;
        }

        #endregion CommandLineSwitch
    }

    #endregion CommandLineBooleanSwitch

    #region CommandLineIntSwitch

    public class CommandLineIntSwitch : CommandLineSwitch
    {
        #region Constants

        private static readonly Regex IncRegex = new Regex(@"^\s*[+]+\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex DecRegex = new Regex(@"^\s*[-]+\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        #endregion Constants

        #region Initialization

        internal CommandLineIntSwitch(string name, string description)
            : base(name, description)
        {
        }

        #endregion Initialization

        #region CommandLineSwitch

        public override Type ValueType { get { return typeof(int); } }

        protected override int ParseValue(string switchValue)
        {
            int result;
            if (int.TryParse(switchValue, out result))
            {
                Value = result;
                return switchValue.Length;
            }
            else if (DecRegex.Match(switchValue).Success)
            {
                Value = (int)GetTargetValue() - 1;
                return switchValue.Length;
            }
            else if (IncRegex.Match(switchValue).Success)
            {
                Value = (int)GetTargetValue() + 1;
                return switchValue.Length;
            }
            return 0;
        }

        #endregion CommandLineSwitch
    }

    #endregion CommandLineIntSwitch

    #region CommandLineDoubleSwitch

    public class CommandLineDoubleSwitch : CommandLineSwitch
    {
        #region Initialization

        internal CommandLineDoubleSwitch(string name, string description)
            : base(name, description)
        {
        }

        #endregion Initialization

        #region CommandLineSwitch

        public override Type ValueType { get { return typeof(double); } }

        protected override int ParseValue(string switchValue)
        {
            double result;
            if (double.TryParse(switchValue, out result))
            {
                Value = result;
                return switchValue.Length;
            }
            return 0;
        }

        #endregion CommandLineSwitch
    }

    #endregion CommandLineDoubleSwitch

    #region CommandLineEnumSwitch

    public class CommandLineEnumSwitch : CommandLineSwitch
    {
        #region Constants

        private static readonly Regex IncRegex = new Regex(@"^\s*[+]+\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex DecRegex = new Regex(@"^\s*[-]+\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        #endregion Constants

        #region Members

        private Type _valueType;
        private Array _sortedValues;

        #endregion Members

        #region Initialization

        internal CommandLineEnumSwitch(string name, string description, Type valueType)
            : base(name, description)
        {
            if (!valueType.IsEnum)
                throw new ArgumentException("valueType should be an Enum");
            _valueType = valueType;
        }

        #endregion Initialization

        #region CommandLineSwitch

        public override Type ValueType { get { return _valueType; } }

        protected override int ParseValue(string switchValue)
        {
            string[] matchedNames = GetMatchedNames(switchValue).ToArray();
            if (matchedNames.Length > 1)
                throw new CommandLineSwitchAmbiguousException(switchValue, matchedNames);
            else if (matchedNames.Length == 1)
            {
                Value = Enum.Parse(ValueType, matchedNames[0], true);
                return switchValue.Length;
            }
            else if (DecRegex.Match(switchValue).Success)
            {
                Array sortedValues = SortedValues;
                object value = GetTargetValue();
                int index = Array.IndexOf(sortedValues, value);
                if (index == 0)
                    index = sortedValues.Length - 1;
                else
                    --index;
                Value = Enum.ToObject(ValueType, sortedValues.GetValue(index));
                return switchValue.Length;
            }
            else if (IncRegex.Match(switchValue).Success)
            {
                Array sortedValues = SortedValues;
                object value = GetTargetValue();
                int index = Array.IndexOf(sortedValues, value);
                if (index == sortedValues.Length - 1)
                    index = 0;
                else
                    ++index;
                Value = Enum.ToObject(ValueType, sortedValues.GetValue(index));
                return switchValue.Length;
            }
            return 0;
        }

        #endregion CommandLineSwitch

        #region Helpers

        private Array SortedValues
        {
            get
            {
                if (_sortedValues != null) return _sortedValues;
                _sortedValues = Enum.GetValues(ValueType).Clone() as Array;
                Array.Sort(_sortedValues);
                return _sortedValues;
            }
        }

        private IEnumerable<string> GetMatchedNames(string switchValue)
        {
            foreach (string item in Enum.GetNames(ValueType))
            {
                if (item.StartsWith(switchValue, StringComparison.InvariantCultureIgnoreCase))
                    yield return item;
            }
        }

        #endregion Helpers
    }

    #endregion CommandLineEnumSwitch

    #region CommandLineDateTimeSwitch

    public class CommandLineDateTimeSwitch : CommandLineSwitch
    {
        #region Initialization

        internal CommandLineDateTimeSwitch(string name, string description)
            : base(name, description)
        {
        }

        #endregion Initialization

        #region CommandLineSwitch

        public override Type ValueType { get { return typeof(DateTime); } }

        protected override int ParseValue(string switchValue)
        {
            DateTime result;
            if (DateTime.TryParse(switchValue, out result))
            {
                Value = result;
                return switchValue.Length;
            }
            return 0;
        }

        #endregion CommandLineSwitch
    }

    #endregion CommandLineDateTimeSwitch

    #region CommandLineStringSwitch

    public class CommandLineStringSwitch : CommandLineSwitch
    {
        #region Initialization

        internal CommandLineStringSwitch(string name, string description)
            : base(name, description)
        {
        }

        #endregion Initialization

        #region CommandLineSwitch

        public override Type ValueType { get { return typeof(string); } }

        protected override int ParseValue(string switchValue)
        {
            Value = switchValue;
            return switchValue.Length;
        }

        #endregion CommandLineSwitch
    }

    #endregion CommandLineStringSwitch

    #region CommandLineSwitchSyntaxException

    [Serializable]
    public class CommandLineSwitchSyntaxException : Exception
    {
        #region Initialization

        public CommandLineSwitchSyntaxException(string argument)
            : base(string.Format("Command line switch invalid value: '{0}'", argument))
        {
        }

        protected CommandLineSwitchSyntaxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Initialization
    }

    #endregion CommandLineSwitchSyntaxException

    #region CommandLineSwitchAmbiguousException

    [Serializable]
    public class CommandLineSwitchAmbiguousException : Exception
    {
        #region Constants

        private const string ParsedNameEntry = "ParsedName";
        private const string AmbiguousNamesEntry = "AmbiguousNames";

        #endregion Constants

        #region Members

        private string _parsedName;
        private string[] _ambiguousNames;

        #endregion Members

        #region Initialization

        public CommandLineSwitchAmbiguousException(string parsedName, string[] ambiguousNames)
        {
            _parsedName = parsedName;
            _ambiguousNames = ambiguousNames;
        }

        #endregion Initialization

        #region Serialization

        protected CommandLineSwitchAmbiguousException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _parsedName = info.GetValue(ParsedNameEntry, typeof(string)) as string;
            _ambiguousNames = info.GetValue(AmbiguousNamesEntry, typeof(string[])) as string[];
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(ParsedNameEntry, _parsedName);
            info.AddValue(AmbiguousNamesEntry, _ambiguousNames);
        }

        #endregion Serialization

        #region Queries

        public override string Message
        {
            get
            {
                return string.Format("Command line switch is ambiguous: '{0}' could be one of ({1})", _parsedName, string.Join(", ", _ambiguousNames));
            }
        }

        #endregion Queries
    }

    #endregion CommandLineSwitchAmbiguousException

    #region CommandLineSwitchAttribute

    /// <summary>Implements a basic command-line switch by taking the
    /// switching name and the associated description.</summary>
    /// <remark>Only currently is implemented for properties, so all
    /// auto-switching variables should have a get/set method supplied.</remark>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandLineSwitchAttribute : Attribute
    {
        #region Members

        private string _name = "";
        private string _description = "";

        #endregion Members

        #region Initialization

        public CommandLineSwitchAttribute(string name, string description)
        {
            _name = name;
            _description = description;
        }

        public CommandLineSwitchAttribute(string description)
        {
            _description = description;
        }

        #endregion Initialization

        #region Queries

        public string Name { get { return _name; } }

        public string Description { get { return _description; } }

        #endregion Queries
    }

    #endregion CommandLineSwitchAttribute

    #region CommandLineSwitchAliasAttribute

    /// <summary>
    /// This class implements an alias attribute to work in conjunction
    /// with the <see cref="CommandLineSwitchAttribute">CommandLineSwitchAttribute</see>
    /// attribute.  If the CommandLineSwitchAttribute exists, then this attribute
    /// defines an alias for it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CommandLineSwitchAliasAttribute : Attribute
    {
        #region Members

        private string _alias = "";

        #endregion Members

        #region Initialization

        public CommandLineSwitchAliasAttribute(string alias)
        {
            _alias = alias;
        }

        #endregion Initialization

        #region Queries

        public string Alias { get { return _alias; } }

        #endregion Queries
    }

    #endregion CommandLineSwitchAliasAttribute

    #region StringLenghtComparer

    public class StringLengthComparer : IComparer<string>
    {
        #region Singleton

        [WebFarmCompatibleAttribute(Compatible = true, Reason = "CommandLine interface n/a in Web sites")]
        private static StringLengthComparer _instance = new StringLengthComparer();

        public static StringLengthComparer Instance { get { return _instance; } }

        #endregion Singleton

        #region IComparer<string>

        public int Compare(string x, string y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;
            if (x.Length == y.Length)
                return 0;
            else if (x.Length < y.Length)
                return -1;
            else
                return 1;
        }

        #endregion IComparer<string>
    }

    #endregion StringLenghtComparer
}