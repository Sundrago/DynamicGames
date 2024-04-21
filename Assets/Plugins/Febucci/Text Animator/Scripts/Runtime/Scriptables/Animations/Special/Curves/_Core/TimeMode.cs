using System;

namespace Febucci.UI.Effects
{
    [Serializable]
    public struct TimeMode
    {
        public float startDelay;
        public bool useUniformTime;
        public float waveSize;
        public float timeSpeed;

        private float tempTime;

        public TimeMode(bool useUniformTime)
        {
            this.useUniformTime = useUniformTime;
            waveSize = 0;
            timeSpeed = 1;
            startDelay = 0;
            tempTime = 0;
        }

        public float GetTime(float animatorTime, float charTime, int charIndex)
        {
            tempTime = ((useUniformTime ? animatorTime : charTime) - startDelay) * timeSpeed - waveSize * charIndex;
            if (tempTime < startDelay)
                return -1;
            return tempTime;
        }
    }
}