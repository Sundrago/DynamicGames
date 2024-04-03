using DG.Tweening;
using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Pet
{
    public class NewPetAnimPreviewSequence : MonoBehaviour
    {
        private const float BackgroundFadeInDelay = 0.5f;
        private const float BackgroundFadeInDuration = 1.5f;
        private const float RankDisplayDelay = 1.5f;
        
        [SerializeField] private Image WhiteBG, blackBG, petImage;
        [SerializeField] private Transform rankPlate;
        [SerializeField] private TextMeshProUGUI title, title_shadow, rank, rank_shadow, name, descr;
        [SerializeField] private TypewriterByWord descr_writer;
        [SerializeField] private TypewriterByCharacter name_writer;
        
        private readonly Color WhiteTransparent = new(1, 1, 1, 0);

        public void Init(string nameString, string descrString, string rankString)
        {
            InitCommonConfigurations(rankString);
            InitFadeInAnimations();
            InitRankAndDescriptionAnimation(nameString, descrString);
            gameObject.SetActive(true);
        }

        private void InitCommonConfigurations(string rankString)
        {
            KillTween();
            ResetTitles(rankString);
            ResetColors();
            rank_shadow.gameObject.SetActive(false);
            rankPlate.localScale = Vector3.zero;
            WhiteBG.DOFade(1, 0.25f).SetEase(Ease.OutExpo);
            petImage.DOFade(1, 0.1f);
        }

        private void InitFadeInAnimations()
        {
            WhiteBG.DOFade(0, BackgroundFadeInDuration).SetDelay(BackgroundFadeInDelay);
            blackBG.DOFade(0.3f, 1f).SetDelay(BackgroundFadeInDelay);
            petImage.DOColor(Color.white, 0.8f).SetDelay(BackgroundFadeInDelay);
        }

        private void KillTween()
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
        }

        private void ResetTitles(string rank_string)
        {
            name.text = "";
            descr.text = "";
            rank.text = rank_string;
            rank_shadow.text = rank_string;
        }

        private void ResetColors()
        {
            WhiteBG.color = WhiteTransparent;
            title.color = WhiteTransparent;
            name.color = WhiteTransparent;
            title_shadow.color = new Color(title_shadow.color.r, title_shadow.color.g, title_shadow.color.b, 0);
            descr.color = WhiteTransparent;
            petImage.color = new Color(0, 0, 0, 0);
            blackBG.color = new Color(0, 0, 0, 0);
        }

        private void InitRankAndDescriptionAnimation(string nameString, string descrString)
        {
            rankPlate.DOScale(1, 0.3f).SetDelay(RankDisplayDelay + 0.2f);
            rank.transform.localScale = Vector3.one * 1000f;
            rank.DOScale(1, 0.5f).SetEase(Ease.OutQuint)
                .OnStart(() => rank_shadow.gameObject.SetActive(true))
                .SetDelay(RankDisplayDelay);
            name.DOFade(1, 0.5f).SetDelay(RankDisplayDelay).OnStart(() =>
            {
                name_writer.ShowText(nameString);
                name_writer.StartShowingText();
            });
            descr.DOFade(1, 1f).SetDelay(RankDisplayDelay)
                .OnStart(() =>
                {
                    descr_writer.ShowText(descrString);
                    descr_writer.StartShowingText();
                });
            title.DOFade(1, 1f).SetDelay(RankDisplayDelay);
            title_shadow.DOFade(1, 1f).SetDelay(RankDisplayDelay);
        }
    }
}