using System.Collections.Generic;
using System.Linq;
using System.Text;
using Febucci.UI.Effects;

namespace Febucci.UI.Core.Parsing
{
    /// <summary>
    ///     Rules how to parse a rich text tag that has an opening and ending
    /// </summary>
    public class AnimationParser<T> : TagParserBase where T : AnimationScriptableBase
    {
        private const char middleSymbolDefault = '\n'; //this will never be set... right? right???

        //--- RESULTS ---
        private Dictionary<string, AnimationRegion> _results;

        //--- DATABASE ---
        public Database<T> database;
        private readonly char middleSymbol;
        private readonly VisibilityMode visibilityMode;

        //--- CONSTRUCTORS ---
        public AnimationParser(char startSymbol, char closingSymbol, char endSymbol, VisibilityMode visibilityMode,
            Database<T> database) : base(startSymbol, closingSymbol, endSymbol)
        {
            this.visibilityMode = visibilityMode;
            this.database = database;
            middleSymbol = middleSymbolDefault;
        }

        public AnimationParser(char startSymbol, char closingSymbol, char middleSymbol, char endSymbol,
            VisibilityMode visibilityMode, Database<T> database) : base(startSymbol, closingSymbol, endSymbol)
        {
            this.visibilityMode = visibilityMode;
            this.database = database;
            this.middleSymbol = middleSymbol;
        }

        public AnimationRegion[] results => _results.Values.ToArray(); //TODO cache

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _results = new Dictionary<string, AnimationRegion>();
            if (database) database.BuildOnce();
        }


        public override bool TryProcessingTag(string textInsideBrackets, int tagLength, ref int realTextIndex,
            StringBuilder finalTextBuilder, int internalOrder)
        {
            if (!database) return false;

            textInsideBrackets = textInsideBrackets.ToLower(); //animations are case insensitive

            //Makes sure the database is built
            database.BuildOnce();

            //If the first character is a closing symbol, then it's a closing tag
            var isClosing = textInsideBrackets[0] == closingSymbol;
            //tries closing all previous regions if tag is /
            if (isClosing && tagLength == 1)
            {
                foreach (var range in _results.Values) range.CloseAllOpenedRanges(realTextIndex);
                return true;
            }

            var tagStart = isClosing ? 1 : 0;

            var fullTag = textInsideBrackets.Substring(tagStart);
            var tempTagWords = fullTag.Split();
            var tempTagName = tempTagWords[0];

            //invalid closing tag, since there are modifiers
            if (isClosing && tempTagWords.Length > 1)
                return false;

            //----CHECKS IF TAG IS RECOGNIZED----

            //removes middle symbol if present
            //so that it can also work with disappearance effects etc.
            //e.g. {#shake}
            //TODO tests for this
            if (middleSymbol != middleSymbolDefault)
            {
                if (tempTagName[0] != middleSymbol) return false;
                tempTagName = tempTagName.Substring(1);
            }

            if (!database.ContainsKey(tempTagName)) return false; //Skips unrecognized tags

            //----ADDS RESULT----
            if (isClosing)
            {
                if (_results.ContainsKey(tempTagName))
                    _results[tempTagName].TryClosingRange(realTextIndex);
            }
            else
            {
                //Creates new region if it doesn't exist yet
                if (!_results.ContainsKey(tempTagName))
                    _results.Add(tempTagName, new AnimationRegion(tempTagName, visibilityMode, database[tempTagName]));

                _results[tempTagName].OpenNewRange(realTextIndex, tempTagWords);
            }

            /*
            Returns true nonetheless, since even if the tag might have not been processed correctly,
            it's still a Text Animator tag that shouldn't appear in the final text
            */
            return true;
        }
    }
}