using System.Text;
using UnityEngine;

namespace Febucci.UI.Core.Parsing
{
    /// <summary>
    ///     Handles text parsing and rich text tags recognition
    /// </summary>
    public static class TextParser
    {
        public static string ParseText(string text, params TagParserBase[] rules)
        {
            if (rules == null || rules.Length == 0)
            {
                Debug.LogWarning("No rules were provided to parse the text. Skipping");
                return text;
            }

            //PS At the moment, only for avoiding fails on domain reload
            //and multiple tags on different text sets
            foreach (var rule in rules) rule.Initialize();

            /*
            P.S. Calculating tags etc. is done inside this single method (and not split for each rule etc.)
            so that the text is only parsed once, and not multiple times for each rule - improving performance
            */
            var result = new StringBuilder();

            // create an array of character from text
            var characters = text.ToCharArray();
            var len = characters.Length;
            bool foundTag;
            string fullTag;
            var allowParsing = true;

            //For every character in text
            for (int textIndex = 0, realTextIndex = 0; textIndex < len; textIndex++)
            {
                foundTag = false;

                //searches for noparse first
                if (characters[textIndex] == '<')
                {
                    var closeIndex = text.IndexOf('>', textIndex + 1);
                    if (closeIndex > 0)
                    {
                        var tagLength = closeIndex - textIndex + 1;

                        void PasteTagToText()
                        {
                            foundTag = true;
                            result.Append(fullTag);
                            textIndex = closeIndex;
                        }

                        fullTag = text.Substring(textIndex, tagLength);
                        switch (fullTag.ToLower())
                        {
                            case "<noparse>":
                                allowParsing = false;
                                PasteTagToText();
                                break;
                            case "</noparse>":
                                allowParsing = true;
                                PasteTagToText();
                                break;
                        }
                    }
                }

                if (allowParsing && !foundTag)
                    foreach (var rule in rules) //tries rich tags
                        if (characters[textIndex] == rule.startSymbol)
                            for (var endIndex = textIndex + 1; endIndex < len && !foundTag; endIndex++)
                            {
                                //If there's an opening symbol, skips since it's a new tag
                                if (characters[endIndex] == rule.startSymbol)
                                    break;

                                if (characters[endIndex] == rule.endSymbol)
                                {
                                    // Gets the length of the tag
                                    var tagLength = endIndex - textIndex - 1;

                                    if (tagLength == 0) //Skips empty tag
                                        break;

                                    if (rule.TryProcessingTag(
                                            text.Substring(textIndex + 1, tagLength),
                                            tagLength,
                                            ref realTextIndex,
                                            result,
                                            textIndex))
                                    {
                                        foundTag = true;
                                        textIndex = endIndex; //Tag processed, skips others
                                        break;
                                    }
                                }
                            }

                if (!foundTag)
                {
                    result.Append(characters[textIndex]);
                    realTextIndex++;
                }
            }

            return result.ToString();
        }
    }
}