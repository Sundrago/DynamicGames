namespace Febucci.UI.Core
{
    public struct WordInfo
    {
        public readonly int firstCharacterIndex;
        public readonly int lastCharacterIndex;
        public readonly string text;

        public WordInfo(int firstCharacterIndex, int lastCharacterIndex, string text)
        {
            this.firstCharacterIndex = firstCharacterIndex;
            this.lastCharacterIndex = lastCharacterIndex;
            this.text = text;
        }
    }
}