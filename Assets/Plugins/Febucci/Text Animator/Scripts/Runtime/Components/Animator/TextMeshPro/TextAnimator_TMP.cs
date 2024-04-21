using System;
using Febucci.UI.Core;
using Febucci.UI.Core.Parsing;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Febucci.UI
{
    /// <summary>
    ///     Animates a TMP text component, both UI or World.
    ///     See <see cref="TAnimCore" /> for the base class information.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    [AddComponentMenu("Febucci/TextAnimator/Text Animator - Text Mesh Pro")]
    public sealed class TextAnimator_TMP : TAnimCore
    {
        private TMP_InputField attachedInputField;

        //----- Values cache -----
        private bool autoSize;
        //-----

        private bool componentsCached;
        private bool isUI;
        private Color sourceColor;
        private Rect sourceRect;
        private TMP_TextInfo textInfo;

        private TMP_Text tmpComponent;
        private int tmpFirstVisibleCharacter;
        private int tmpMaxVisibleCharacters;

        /// <summary>
        ///     The TextMeshPro text component linked to this Text Animator
        /// </summary>
        public TMP_Text TMProComponent
        {
            get
            {
                if (tmpComponent) return tmpComponent;
                CacheComponentsOnce();
                return tmpComponent;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            textInfo = TMProComponent.textInfo;
        }

        private void CacheComponentsOnce()
        {
            if (componentsCached) return;

            if (!gameObject.TryGetComponent(out tmpComponent))
                Debug.LogError($"TextAnimator_TMP {name} requires a TMP_Text component to work.", gameObject);

            gameObject.TryGetComponent(out attachedInputField);
            componentsCached = true;
            isUI = tmpComponent is TextMeshProUGUI;
        }

        protected override void OnInitialized()
        {
            CacheComponentsOnce();

            //prevents the text from being rendered at startup
            //e.g. in case user has stuff on the inspector
            tmpComponent.renderMode = TextRenderFlags.DontRender;
        }

        protected override bool IsReady()
        {
            return componentsCached && (!isUI || tmpComponent.canvas);
        }

        #region Characters

        protected override int GetCharactersCount()
        {
            return textInfo.characterCount;
        }

        #endregion

        #region Text

        protected override TagParserBase[] GetExtraParsers()
        {
            return new TagParserBase[1] { new TMPTagParser(tmpComponent.richText, '<', '/', '>') };
        }

        public override string GetOriginalTextFromSource()
        {
            return TMProComponent.text;
        }

        public override string GetStrippedTextFromSource()
        {
            return tmpComponent.GetParsedText();
        }

        /// <summary>
        ///     Equivalent to setting the text to the TMP component, without parsing it.
        ///     Please use <see cref="TAnimCore.SetText(string)" /> or <see cref="TAnimCore.SetText(string, bool)" /> instead.
        /// </summary>
        /// <param name="text"></param>
        public override void SetTextToSource(string text)
        {
            //Avoids rendering the text for half a frame
            TMProComponent.renderMode = TextRenderFlags.DontRender;

            //--generates mesh and text info--
            if (attachedInputField) attachedInputField.text = text; //renders input field
            else tmpComponent.text = text; //<-- sets the text

            OnForceMeshUpdate();

            textInfo = tmpComponent.GetTextInfo(tmpComponent.text);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                //needed to update tmp mesh from editor
                tmpComponent.havePropertiesChanged = true;
                EditorUtility.SetDirty(tmpComponent);
            }
            else
#endif
            {
                tmpComponent.renderMode = TextRenderFlags.DontRender;
            }
        }

        #endregion

        #region Checks

        protected override bool HasChangedRenderingSettings()
        {
            return tmpComponent.havePropertiesChanged
                   //changing the properties below doesn't seem to trigger 'havePropertiesChanged', so we're checking them manually
                   || tmpComponent.enableAutoSizing != autoSize
                   || tmpComponent.rectTransform.rect != sourceRect
                   || tmpComponent.color != sourceColor
                   || tmpComponent.firstVisibleCharacter != tmpFirstVisibleCharacter
                   || tmpComponent.maxVisibleCharacters != tmpMaxVisibleCharacters;
        }

        protected override bool HasChangedText(string strippedText)
        {
            if (string.IsNullOrEmpty(tmpComponent.text) && string.IsNullOrEmpty(strippedText))
                return false;

            if (string.IsNullOrEmpty(tmpComponent.text) != string.IsNullOrEmpty(strippedText))
                return true;

            return !tmpComponent.text.Equals(strippedText);
        }

        #endregion

        #region Mesh

        protected override void CopyMeshFromSource(ref CharacterData[] characters)
        {
            autoSize = tmpComponent.enableAutoSizing;
            sourceRect = tmpComponent.rectTransform.rect;
            sourceColor = tmpComponent.color;
            tmpFirstVisibleCharacter = tmpComponent.firstVisibleCharacter;
            tmpMaxVisibleCharacters = tmpComponent.maxVisibleCharacters;

            TMP_CharacterInfo currentCharInfo;

            //Updates the characters sources
            for (var i = 0; i < textInfo.characterCount && i < characters.Length; i++)
            {
                currentCharInfo = textInfo.characterInfo[i];
                characters[i].info.isRendered = currentCharInfo.isVisible;
                characters[i].info.character = currentCharInfo.character;
                //Updates TMP char info
                //characters[i].current.tmp_CharInfo = textInfo.characterInfo[i];

                //Copies source data from the mesh info only if the character is valid, otherwise its vertices array will be null and tAnim will start throw errors
                if (!currentCharInfo.isVisible) continue;

                characters[i].info.pointSize = currentCharInfo.pointSize;

                //Updates vertices
                for (byte k = 0; k < TextUtilities.verticesPerChar; k++)
                    characters[i].source.positions[k] = textInfo.meshInfo[currentCharInfo.materialReferenceIndex]
                        .vertices[currentCharInfo.vertexIndex + k];

                //Updates colors
                for (byte k = 0; k < TextUtilities.verticesPerChar; k++)
                    characters[i].source.colors[k] = textInfo.meshInfo[currentCharInfo.materialReferenceIndex]
                        .colors32[currentCharInfo.vertexIndex + k];
            }
        }

        protected override void PasteMeshToSource(CharacterData[] characters)
        {
            TMP_CharacterInfo currentCharInfo;

            //Updates the mesh
            for (var i = 0; i < textInfo.characterCount && i < CharactersCount; i++)
            {
                currentCharInfo = textInfo.characterInfo[i];
                //Avoids updating if we're on an invisible character, like a spacebar
                //Do not switch this with "i<visibleCharacters", since the plugin has to update not yet visible characters
                if (!currentCharInfo.isVisible) continue;

                //Updates TMP char info
                //textInfo.characterInfo[i] = characters[i].data.tmp_CharInfo;

                //Updates vertices
                for (byte k = 0; k < TextUtilities.verticesPerChar; k++)
                    textInfo.meshInfo[currentCharInfo.materialReferenceIndex]
                        .vertices[currentCharInfo.vertexIndex + k] = characters[i].current.positions[k];

                //Updates colors
                for (byte k = 0; k < TextUtilities.verticesPerChar; k++)
                    textInfo.meshInfo[currentCharInfo.materialReferenceIndex]
                        .colors32[currentCharInfo.vertexIndex + k] = characters[i].current.colors[k];
            }

            tmpComponent.UpdateVertexData();
        }

        protected override void OnForceMeshUpdate()
        {
            tmpComponent.ForceMeshUpdate(true);
        }

        #endregion

        #region Obsolete

        [Obsolete("This method is Obsolete. Please check through the 'Characters' array instead.")]
        public bool TryGetNextCharacter(out TMP_CharacterInfo result)
        {
            if (latestCharacterShown.index < CharactersCount - 1)
            {
                result = textInfo.characterInfo[latestCharacterShown.index + 1];
                return true;
            }

            result = default;
            return false;
        }

        [Obsolete("Please use TMProComponent instead.")]
        public TMP_Text tmproText => TMProComponent;

        #endregion
    }
}