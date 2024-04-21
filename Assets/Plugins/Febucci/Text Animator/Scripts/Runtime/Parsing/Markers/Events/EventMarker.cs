namespace Febucci.UI.Core.Parsing
{
    /// <summary>
    ///     Contains information about an event called in text
    /// </summary>
    public class EventMarker : MarkerBase
    {
        public EventMarker(string name, int index, int internalOrder, string[] parameters) : base(name, index,
            internalOrder, parameters)
        {
        }
    }
}