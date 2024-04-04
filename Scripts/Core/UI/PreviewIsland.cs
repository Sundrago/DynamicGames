using System.Collections.Generic;
using DG.Tweening;
using Febucci.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using Utility;

namespace Core.UI
{
    public class PreviewIsland : MonoBehaviour
    {
        [SerializeField] private RectTransform rect, notch;
        [SerializeField] private Transform notchPos;
        [SerializeField] private TypewriterByCharacter typewriter;
        [SerializeField] private List<GameObject> boards;

        public static PreviewIsland Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        [Button]
        public void OpenIslandAndPreview(int idx)
        {
            rect.sizeDelta = notch.sizeDelta;
            gameObject.transform.position = notchPos.position;
            rect.DOSizeDelta(new Vector2(700, 700), 0.5f).SetEase(Ease.OutBack).SetDelay(0.35f);
            SetupBoardActive(idx);
            gameObject.SetActive(true);
            SetupTypeWriter(idx);
        }

        private void SetupBoardActive(int idx)
        {
            for (var i = 0; i < boards.Count; i++) boards[i].gameObject.SetActive(i == idx);
        }

        private void SetupTypeWriter(int idx)
        {
            switch (idx)
            {
                case 0:
                    typewriter.ShowText(
                        Localize.GetLocalizedString("[previewIsland_0] Fluffy를 드래그해서 \n프렌즈 블록 위에 놓아보세요."));
                    break;
                case 1:
                    typewriter.ShowText(Localize.GetLocalizedString("[previewIsland_1] Fluffy를 게임 블록 위에 놓아보세요."));
                    break;
                case 2:
                    typewriter.ShowText(Localize.GetLocalizedString("[previewIsland_2] 게임을 터치해서 \n플레이하세요!"));
                    break;
            }

            typewriter.StartShowingText(true);
        }

        [Button]
        public void Close()
        {
            DOTween.Kill(rect);
            rect.DOSizeDelta(new Vector2(750, notch.sizeDelta.y), 0.4f).SetEase(Ease.InOutCubic);
            rect.DOSizeDelta(notch.sizeDelta, 0.5f).SetEase(Ease.InOutCubic).SetDelay(0.3f)
                .OnComplete(() => { gameObject.SetActive(false); });
        }
    }
}