using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Febucci.UI;
using Sirenix.OdinInspector;

namespace Core.Pet
{
    public class NewPetAnim_PreviewSequence : MonoBehaviour
    {
        [SerializeField] private Image WhiteBG, blackBG, petImage;

        [SerializeField] private Transform rankPlate;

        // [SerializeField] private List<Image> BGs;
        [SerializeField] private TextMeshProUGUI title, title_shadow, rank, rank_shadow, name, descr;
        [SerializeField] private TypewriterByWord descr_writer;
        [SerializeField] private TypewriterByCharacter name_writer;

        private Color WhiteTransparent = new Color(1, 1, 1, 0);
        private SpriteRenderer spriteRenderer;

        [Button]
        public void Init(string name_string, string descr_string, string rank_string)
        {
            DOTween.Kill(WhiteBG);
            DOTween.Kill(petImage);
            DOTween.Kill(rankPlate);
            DOTween.Kill(title);
            DOTween.Kill(title_shadow);
            DOTween.Kill(rank);
            DOTween.Kill(name.transform);
            DOTween.Kill(descr);
            DOTween.Kill(rankPlate.transform);
            DOTween.Kill(blackBG);

            name.text = "";
            descr.text = "";
            rank.text = rank_string;
            rank_shadow.text = rank_string;

            WhiteBG.color = WhiteTransparent;
            title.color = WhiteTransparent;
            name.color = WhiteTransparent;
            title_shadow.color = new Color(title_shadow.color.r, title_shadow.color.g, title_shadow.color.b, 0);
            rank_shadow.gameObject.SetActive(false);
            // name.transform.localScale = Vector3.zero;
            descr.color = WhiteTransparent;
            // foreach (var bg in BGs)
            // {
            //     DOTween.Kill(bg);
            //     bg.color = WhiteTransparent;
            // }

            petImage.color = new Color(0, 0, 0, 0);
            blackBG.color = new Color(0, 0, 0, 0);
            rankPlate.localScale = Vector3.zero;

            WhiteBG.DOFade(1, 0.25f).SetEase(Ease.OutExpo);
            petImage.DOFade(1, 0.1f);

            //01
            float bgInDelay = 0.5f;
            float bgInDuration = 1.5f;
            // foreach (var bg in BGs)
            // {
            //     bg.DOFade(1, bgInDuration).SetDelay(bgInDelay);
            // }
            WhiteBG.DOFade(0, bgInDuration).SetDelay(bgInDelay);

            blackBG.DOFade(0.3f, 1f).SetDelay(bgInDelay);
            ;
            petImage.DOColor(Color.white, 0.8f).SetDelay(bgInDelay);

            //02
            float rankInDelay = 1.5f;
            rankPlate.DOScale(1, 0.3f).SetDelay(rankInDelay + 0.2f);
            rank.transform.localScale = Vector3.one * 1000f;
            rank.DOScale(1, 0.5f).SetEase(Ease.OutQuint)
                .OnStart(() => rank_shadow.gameObject.SetActive(true))
                .SetDelay(rankInDelay);
            // name.DOScale(1, 0.5f).SetDelay(rankInDelay);
            name.DOFade(1, 0.5f).SetDelay(rankInDelay).OnStart(() =>
            {
                name_writer.ShowText(name_string);
                name_writer.StartShowingText();
            });
            descr.DOFade(1, 1f).SetDelay(rankInDelay)
                .OnStart(() =>
                {
                    descr_writer.ShowText(descr_string);
                    descr_writer.StartShowingText();
                });
            title.DOFade(1, 1f).SetDelay(rankInDelay);
            title_shadow.DOFade(1, 1f).SetDelay(rankInDelay);

            gameObject.SetActive(true);
        }

    }
}