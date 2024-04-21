using System;

namespace Febucci.UI.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DefaultValueAttribute : Attribute
    {
        public readonly string variableName;
        public readonly float variableValue;

        public DefaultValueAttribute(string variableName, float variableValue)
        {
            this.variableName = variableName;
            this.variableValue = variableValue;
        }
    }
}