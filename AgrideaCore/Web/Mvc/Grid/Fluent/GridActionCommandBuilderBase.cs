using System;
using Agridea.Web.Mvc.Grid.Command;
using Microsoft.Ajax.Utilities;
using Microsoft.SqlServer.Management.Smo;

namespace Agridea.Web.Mvc.Grid.Fluent
{
    public abstract class GridActionCommandBuilderBase<T, TCommand, TBuilder>
        where TCommand : ICommand<T>
        where TBuilder : GridActionCommandBuilderBase<T, TCommand, TBuilder>
    {
        protected GridActionCommandBuilderBase(TCommand command)
        {
            Command = command;
        }


        protected TCommand Command { get; private set; }

        public  TBuilder  TextOnly(string alternateText)
        {
            Command.TextOnly = true;
            Command.AlternateText = alternateText;
            return this as TBuilder;
        }

        public TBuilder SetAlternateText(string value)
        {
            Command.AlternateText = value;
            return this as TBuilder;
        }

        public TBuilder HideWhen(Func<T, bool> hideWhenFunc)
        {
            Command.HideWhenFunc = hideWhenFunc;
            return this as TBuilder;
        }

        public TBuilder HideWhen(bool value)
        {
            Command.IsHidden = value;
            return this as TBuilder;
        }
    }
}
