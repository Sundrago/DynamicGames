using System.Collections;
using Febucci.UI.Core;
using Febucci.UI.Core.Parsing;
using UnityEngine;

namespace Febucci.UI.Examples
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    internal class ExampleEvents : MonoBehaviour
    {
        // ---- OTHER PART OF THE SCRIPT ----
        // This makes the script run faking a dialogue system
        [SerializeField] private TypewriterCore typewriter;
        [SerializeField] [TextArea(1, 5)] private string[] dialoguesLines;
        [SerializeField] private Sprite[] faces;
        [SerializeField] private SpriteRenderer faceRenderer;
        [SerializeField] private GameObject continueText;
        [SerializeField] private Transform[] crates;
        private Vector3[] cratesInitialScale;
        private bool currentLineShown;

        private int dialogueIndex;
        private int dialogueLength;

        private bool CurrentLineShown
        {
            get => currentLineShown;
            set
            {
                currentLineShown = value;
                continueText.SetActive(value);
            }
        }

        private void Awake()
        {
            cratesInitialScale = new Vector3[crates.Length];
            for (var i = 0; i < crates.Length; i++) cratesInitialScale[i] = crates[i].localScale;

            dialogueLength = dialoguesLines.Length;
            typewriter.onTextShowed.AddListener(() => CurrentLineShown = true);
        }
        // ---- PART OF THE SCRIPT THAT YOU'RE PROBABLY INTERESTED IT ----

        private void Start()
        {
            //Subscribe to the event
            typewriter.onMessage.AddListener(OnMessage);


            dialogueIndex = 0;
            CurrentLineShown = false;
            typewriter.ShowText(dialoguesLines[dialogueIndex]);
        }

        private void Update()
        {
            if (Input.anyKeyDown && CurrentLineShown) ContinueSequence();
        }

        private void OnDestroy()
        {
            if (typewriter) typewriter.onMessage.RemoveListener(OnMessage);
        }

        private bool TryGetInt(string parameter, out int result)
        {
            if (FormatUtils.TryGetFloat(parameter, 0, out var resultFloat))
            {
                result = (int)resultFloat;
                return true;
            }

            result = -1;
            return false;
        }

        private void OnMessage(EventMarker eventData)
        {
            switch (eventData.name)
            {
                case "face":
                    if (eventData.parameters.Length <= 0)
                    {
                        Debug.LogWarning($"You need to specify a sprite index! Dialogue: {dialogueIndex}");
                        return;
                    }

                    if (TryGetInt(eventData.parameters[0], out var spriteIndex))
                    {
                        if (spriteIndex >= 0 && spriteIndex < faces.Length)
                            faceRenderer.sprite = faces[spriteIndex];
                        else
                            Debug.Log($"Sprite index was out of range. Dialogue: {dialogueIndex}");
                    }

                    break;

                case "crate":
                    if (eventData.parameters.Length <= 0)
                    {
                        Debug.LogWarning($"You need to specify a crate index! Dialogue: {dialogueIndex}");
                        return;
                    }

                    if (TryGetInt(eventData.parameters[0], out var crateIndex))
                    {
                        if (crateIndex >= 0 && crateIndex < crates.Length)
                            StartCoroutine(AnimateCrate(crateIndex));
                        else
                            Debug.Log($"Sprite index was out of range. Dialogue: {dialogueIndex}");
                    }

                    break;
            }
        }

        private void ContinueSequence()
        {
            CurrentLineShown = false;
            dialogueIndex++;
            if (dialogueIndex < dialogueLength)
                typewriter.ShowText(dialoguesLines[dialogueIndex]);
            else
                typewriter.StartDisappearingText();
        }

        private IEnumerator AnimateCrate(int crateIndex)
        {
            var crate = crates[crateIndex];
            var initialScale = cratesInitialScale[crateIndex];
            var targetScale = new Vector3(initialScale.x * 1.2f, initialScale.y * .6f, initialScale.z);
            float t = 0;
            const float duration = .4f;

            while (t <= duration)
            {
                t += Time.unscaledDeltaTime;
                var pct = t / duration;
                if (pct < .5f) pct = pct / .5f;
                else pct = 1 - (pct - .5f) / .5f;

                crate.localScale = Vector3.LerpUnclamped(initialScale, targetScale, pct);
                yield return null;
            }

            crate.localScale = initialScale;
        }
    }
}