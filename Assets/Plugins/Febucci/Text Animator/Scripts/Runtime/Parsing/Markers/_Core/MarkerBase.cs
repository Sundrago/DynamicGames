using System;
using System.Text;

namespace Febucci.UI.Core.Parsing
{
    public abstract class MarkerBase : IComparable<MarkerBase>
    {
        public readonly int index;
        internal readonly int internalOrder;
        public readonly string name;
        public string[] parameters;

        public MarkerBase(string name, int index, int internalOrder, string[] parameters)
        {
            this.name = name;
            this.index = index;
            this.internalOrder = internalOrder;
            this.parameters = parameters;
        }

        /// <summary>
        ///     Checks if a marker is placed before another in text.
        ///     PS. Checks for internal order, since when a letter is shown (character index)
        ///     there might be multiple events happening
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(MarkerBase other)
        {
            return internalOrder.CompareTo(other.internalOrder);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(name);
            sb.Append(" internal order:");
            sb.Append(internalOrder);
            sb.Append(" index:");
            sb.Append(index);
            sb.Append('\n');
            for (var i = 0; i < parameters.Length; i++)
            {
                sb.Append(parameters[i]);
                sb.Append('\n');
            }

            return sb.ToString();
        }
    }
}