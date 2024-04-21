namespace Febucci.UI.Core.Parsing
{
    /// <summary>
    ///     Contains information about an action tag in text.
    /// </summary>
    public sealed class ActionMarker : MarkerBase
    {
        public ActionMarker(string name, int index, int internalOrder, string[] parameters) : base(name, index,
            internalOrder, parameters)
        {
        }
    }
}