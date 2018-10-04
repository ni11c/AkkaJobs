using Agridea.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agridea.UI.CommandLine
{
    /// <summary>
    /// Parse the command line and extract switches
    /// </summary>
    /// <remarks>
    /// The grammar is created from classes containing attributed properties.
    /// The <see cref="CommandLineSwitchAttribute"/> combined with <see cref="CommandLineSwitchAliasAttribute"/>s
    /// applied to class properties elect them as switch targets. The attributed properties are modified during
    /// the execution of the <see cref="Parse"/> method with a property injection mechanism.
    /// Currently only properties of type <see cref="bool"/>, <see cref="int"/>,
    /// <see cref="double"/>, <see cref="Enum"/> and <see cref="string"/> are supported.
    /// </remarks>
    /// <example>
    /// In the following example a <c>Person</c> class is implemented that defines the <c>Age</c> property.
    /// To export this property as a command line switch, we decorate it with the <see cref="CommandLineSwitchAttribute"/>.
    /// <c>
    ///public class Person
    ///{
    ///  public Person(string name) { Name = name; }
    ///
    ///  public string Name { get; set; }
    ///
    ///  [CommandLineSwitch("set the age of this person")]
    ///  [CommandLineSwitchAlias("old")]
    ///  public int Age { get; set; }
    ///
    ///  public override string ToString() { return string.Format("{0} is {1} years old", Name, Age); }
    ///}
    ///public static class MyApp
    ///{
    ///  public static void Main()
    ///   {
    ///     Person p = new Person("John Smith");
    ///     CommandLineSwitches switches = new CommandLineSwitches(p);
    ///     switches.Parse(Environment.GetCommandLineArgs());
    ///     Console.WriteLine(p);
    ///   }
    ///}
    /// </c>
    /// Assuming the name of the executable is test, the following commands:
    /// <c>
    /// test -a 51
    /// test -ag 51
    /// test -age 51
    /// test -old 51
    /// </c>
    /// are similar and will all produce the same following output:
    /// <c>
    /// John Smith is 51 years old
    /// </c>
    /// </example>
    public class CommandLineSwitches
    {
        #region ValueTypeComparer
        /// <summary>
        /// This comparer is used for switches ordering by type so that
        /// the string type has the lowest priority. 
        /// </summary>
        private class ValueTypeComparer : IComparer<Type>
        {
            #region Singleton
            [WebFarmCompatibleAttribute(Compatible = true, Reason = "CommandLine interface n/a in Web sites")]
            private static ValueTypeComparer _instance = new ValueTypeComparer();
            public static ValueTypeComparer Instance { get { return _instance; } }
            #endregion

            #region IComparer<Type>
            public int Compare(Type x, Type y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;
                if (x == null)
                    return -1;
                if (y == null)
                    return 1;
                if (x == typeof(string))
                    return 1;
                else if (y == typeof(string))
                    return -1;
                else
                    return Comparer<int>.Default.Compare(x.GetHashCode(), y.GetHashCode());
            }
            #endregion
        }
        #endregion

        #region Factory
        //public static CommandLineSwitches CreateSwitchesForType(object objectWithSwitchAttributes)
        //{
        //  CommandLineSwitches switches = new CommandLineSwitches();
        //  switches.AddSwitchesForType(objectWithSwitchAttributes);
        //  return switches;
        //}
        #endregion

        #region Members
        private Dictionary<string, CommandLineSwitch> _switches = new Dictionary<string, CommandLineSwitch>(StringComparer.InvariantCultureIgnoreCase);
        #endregion

        #region Initialization
        public CommandLineSwitches() { }
        /// <summary>
        /// Initialize a <see cref="CommandLineSwitches"/> with attributed properties of given <paramref name="objectWithSwitchAttributes"/> type.
        /// </summary>
        /// <param name="objectWithSwitchAttributes">The attributed object from which the switches will be extracted</param>
        public CommandLineSwitches(object objectWithSwitchAttributes) { AddSwitchesForType(objectWithSwitchAttributes); }
        #endregion

        #region Properties
        public CommandLineSwitch this[string name]
        {
            get { return _switches[name]; }
            set { _switches[name] = value; }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Add command line switches for given <paramref name="objectWithSwitchAttributes"/> type.
        /// </summary>
        /// <param name="objectWithSwitchAttributes">The attributed object from which the switches will be created</param>
        public void AddSwitchesForType(object objectWithSwitchAttributes)
        {
            foreach (CommandLineSwitch item in CommandLineSwitch.CreateSwitchesForType(objectWithSwitchAttributes))
                _switches[item.Name] = item;
        }

        /// <summary>
        /// Parse given arguments.
        /// </summary>
        /// <param name="args">The arguments to parse.</param>
        /// <returns>Remaining arguments.</returns>
        public string[] Parse(string[] args)
        {
            List<string> remainingArgs = new List<string>();
            bool lastSwitch = false;
            for (int i = 0; i < args.Length; )
            {
                if (lastSwitch)
                {
                    remainingArgs.Add(args[i++]);
                }
                else
                {
                    CommandLineSwitch clSwitch = GetBestMatch(args, i);
                    if (clSwitch == null)
                        remainingArgs.Add(args[i++]);
                    else if (clSwitch.ParsedArgsCount == 0)
                        throw new CommandLineSwitchSyntaxException(args[i++]);
                    else
                    {
                        i += clSwitch.Commit();
                        lastSwitch = lastSwitch || clSwitch.EndMarkerSwitch;
                        if (clSwitch.RemainingArg.Length > 0)
                            remainingArgs.Add(clSwitch.RemainingArg);
                    }
                }
            }
            return remainingArgs.ToArray();
        }
        /// <summary>
        /// Get the switch that best matches given arguments.
        /// </summary>
        /// <param name="args">The arguments array to parse.</param>
        /// <param name="startIndex">The index into the <paramref name="args"/> array where to start parsing.</param>
        /// <returns>The <see cref="CommandLineSwitch"/> that best matches given arguments</returns>
        public CommandLineSwitch GetBestMatch(string[] args, int startIndex)
        {
            CommandLineSwitch bestMatchSwitch = null;
            foreach (CommandLineSwitch item in SwitchesOrderedByType)
            {
                item.Parse(args, startIndex);
                if (item.ParsedLength == 0)
                {
                    if (item.EndMarkerSwitch) return item;
                    continue;
                }
                if (bestMatchSwitch == null
                  || item.ParsedLength > bestMatchSwitch.ParsedLength
                  || item.ParsedLength == bestMatchSwitch.ParsedLength && item.NameMatchExactly && !bestMatchSwitch.NameMatchExactly)
                {
                    bestMatchSwitch = item;
                }
                else if (item.NameMatchExactly == bestMatchSwitch.NameMatchExactly && item.ParsedLength == bestMatchSwitch.ParsedLength)
                {
                    throw new CommandLineSwitchAmbiguousException(item.ParsedName, new string[] { item.Name, bestMatchSwitch.Name });
                }
            }
            return bestMatchSwitch;
        }
        public string GetHelp()
        {
            var sb = new StringBuilder();
            int maxDescriptionLength = _switches.Values.Max(s => s.Description.Length);
            int maxAliasLength = _switches.Values.Max(s => s.Aliases.OrderBy(a => a.Length).First().Length);
            foreach (string name in OrderedSwitchNames)
            {
                var clSwitch = _switches[name];
                var shortestAlias = clSwitch.Aliases.OrderBy(x => x.Length).First();
                sb.Append(" -" + shortestAlias);
                sb.Append(' ', maxAliasLength - shortestAlias.Length);
                sb.Append(" : ");
                sb.Append(clSwitch.Description);
                sb.Append(' ', maxDescriptionLength - clSwitch.Description.Length);
                sb.Append(" [" + name + "]");

                sb.AppendLine();
            }
            return sb.ToString();
        }
        #endregion

        #region Helpers
        private IEnumerable<CommandLineSwitch> SwitchesOrderedByType { get { return _switches.Values.OrderBy(s => s.ValueType, ValueTypeComparer.Instance); } }
        private IEnumerable<string> OrderedSwitchNames { get { return _switches.Keys.OrderBy(k => k, StringComparer.InvariantCultureIgnoreCase); } }
        #endregion
    }
}
