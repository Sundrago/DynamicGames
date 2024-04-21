namespace Febucci.UI.Core
{
    /// <summary>
    ///     A way to store information about the typing progress between coroutines,
    ///     also allowing to keep track of time between frames and characters/words showed
    /// </summary>
    public class TypingInfo
    {
        public float speed = 1;

        public TypingInfo()
        {
            speed = 1;
            timePassed = 0;
        }

        public float timePassed { get; internal set; }
    }
}