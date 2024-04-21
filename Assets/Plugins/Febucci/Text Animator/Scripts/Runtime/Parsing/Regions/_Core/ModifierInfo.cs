using System;

namespace Febucci.UI
{
    public struct ModifierInfo : IEquatable<ModifierInfo>
    {
        public string name;
        public float value;

        public ModifierInfo(string name, float value)
        {
            this.name = name;
            this.value = value;
        }

        public bool Equals(ModifierInfo other)
        {
            return value.Equals(other.value) && name.Equals(other.name);
        }

        public override string ToString()
        {
            return $"{name}={value}";
        }
    }
}