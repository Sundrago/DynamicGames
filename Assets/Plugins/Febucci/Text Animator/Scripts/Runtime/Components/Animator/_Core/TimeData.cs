using System;

namespace Febucci.UI
{
    /// <summary>
    ///     Contains TextAnimator's current time values.
    /// </summary>
    [Serializable]
    public struct TimeData
    {
        /// <summary>
        ///     Time passed since the textAnimator started showing the very first letter
        /// </summary>
        public float timeSinceStart { get; private set; }

        /// <summary>
        ///     TextAnimator's Component delta time, could be Scaled or Unscaled
        /// </summary>
        public float deltaTime { get; private set; }

        public void RestartTime()
        {
            timeSinceStart = 0;
        }

        internal void IncreaseTime()
        {
            timeSinceStart += deltaTime;
        }

        internal void UpdateDeltaTime(float deltaTime)
        {
            this.deltaTime = deltaTime;

            //To avoid possible desync errors etc., effects can't be played backwards. 
            if (deltaTime < 0)
                deltaTime = 0;
        }
    }
}