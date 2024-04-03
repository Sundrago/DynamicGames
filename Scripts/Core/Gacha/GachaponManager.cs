using Core.Main;
using Core.System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Core.Gacha
{
    /// <summary>
    ///     Class responsible for managing the overall functionality of a Gachapon machine.
    /// </summary>
    public class GachaponManager : MonoBehaviour, IPanelObject
    {
        public enum GachaponStatus
        {
            Idle,
            InsertCoin,
            RotateLever,
            ClosUp
        }

        [Header("Managers and Controllers")] 
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private SfxController sfxController;
        [SerializeField] private BlockDragHandler GachaponBtn;
        [SerializeField] private CapsuleAnimation capsuleAnimation;
        [SerializeField] private MoneyManager moneyManager;

        [Header("UI Components")] 
        [SerializeField] private TextMeshProUGUI guideText;
        [SerializeField] private Transform takeoutCapsule, lever, gachapon, coinInsertPos;
        [SerializeField] private Image myCapsule_s, myCapsule_l, bg;
        [SerializeField] private Transform ticketbtn, coinBtn;
        [SerializeField] private Image ticketActive, coinActive;
        [SerializeField] private Transform ticketGlow, coinGlow;
        [SerializeField] private Image[] gacapon_caplsule_ui;

        [Header("Sprites")] 
        [SerializeField] private Sprite[] gachapon_capsule_sprites;
        [SerializeField] private Sprite[] gachapon_capsule_fullsized_sprites;

        private bool isAnimPlaying;

        private GachaponStatus status = GachaponStatus.Idle;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void ShowPanel()
        {
            if (gameObject.activeSelf || DOTween.IsTweening(bg)) return;

            GachaponBtn.KillFX();
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
            SetBtnActive();
            RnadomizePonCapsule();
            ShowPanelAnimation();
        }

        public void HidePanel()
        {
            if (status != GachaponStatus.Idle)
            {
                PonBtnClicked();
                return;
            }

            if (DOTween.IsTweening(bg)) return;

            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Close);
            HidePanelAnimation();
        }

        private void ShowPanelAnimation()
        {
            gachapon.transform.position = Vector3.zero;
            gachapon.transform.eulerAngles = Vector3.zero;

            gachapon.transform.DOLocalMoveY(-2500f, 0.8f)
                .From()
                .SetEase(Ease.OutExpo);

            gachapon.transform.DORotate(new Vector3(0f, 0f, 30f), 0.8f)
                .From()
                .SetEase(Ease.OutBack)
                .OnComplete(() => { TutorialManager.Instancee.GachaponPanelOpend(); });

            bg.DOFade(0.2f, 1f);
            gameObject.SetActive(true);
        }

        private void HidePanelAnimation()
        {
            gachapon.transform.position = Vector3.zero;
            gachapon.transform.eulerAngles = Vector3.zero;

            bg.DOFade(0, 1f);
            gachapon.transform.DOLocalMoveY(-3000f, 1f)
                .SetEase(Ease.InOutExpo)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    MainCanvas.Instance.Offall();
                });
            gachapon.transform.DORotate(new Vector3(0f, 0f, 10), 0.8f)
                .SetEase(Ease.OutExpo);
        }

        public void PonBtnClicked()
        {
            if (isAnimPlaying) return;
            switch (status)
            {
                case GachaponStatus.InsertCoin:
                    RotateLever();
                    sfxController.ChangeBGMVolume(0.5f);
                    break;
                case GachaponStatus.RotateLever:
                    CapsuleCloseUp();
                    break;
            }
        }

        public void SetBtnActive()
        {
            var isNewTicketActive = MoneyManager.Instance.HasEnoughTicket(MoneyManager.RewardType.Ticket, 50);
            var isNewCoinActive = MoneyManager.Instance.HasEnoughTicket(MoneyManager.RewardType.GachaCoin, 1);

            ticketActive.DOFade(isNewTicketActive ? 1 : 0, 0.3f);
            coinActive.DOFade(isNewCoinActive ? 1 : 0, 0.3f);

            if (isNewTicketActive) StartBtnGlowAnimation(ticketGlow);
            if (isNewCoinActive) StartBtnGlowAnimation(coinGlow);

            if (isNewCoinActive && !DOTween.IsTweening(coinBtn.gameObject.transform) && !isAnimPlaying &&
                status == GachaponStatus.Idle)
                StartBtnBounceAnimation();

            if (isAnimPlaying || isNewCoinActive || status != GachaponStatus.Idle) StopBtnBounceAnimation();

            var shouldGuideTextBeActive = !isNewTicketActive && !isNewCoinActive && isAnimPlaying == false &&
                                          status == GachaponStatus.Idle;
            guideText.gameObject.SetActive(shouldGuideTextBeActive);
        }

        private void StartBtnGlowAnimation(Transform transform)
        {
            if (DOTween.IsTweening(transform)) return;
            transform.localPosition = new Vector3(-400, 0, 0);
            transform.DOLocalMoveX(400, 1);
        }

        private void StartBtnBounceAnimation()
        {
            coinBtn.gameObject.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.5f, 1)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.OutQuad);
        }

        private void StopBtnBounceAnimation()
        {
            DOTween.Kill(coinBtn.gameObject.transform);
            coinBtn.gameObject.transform.localScale = Vector3.one;
        }

        public void TicketBtnClicked()
        {
            if (moneyManager.SubtractMoney(MoneyManager.RewardType.Ticket, 50))
            {
                moneyManager.Coin2DAnim(MoneyManager.RewardType.GachaCoin, ticketbtn.transform.position, 1);
                audioManager.PlaySfxByTag(SfxTag.AcquiredCoin);
                TutorialManager.Instancee.TicketBtnClicked();
                SetBtnActive();
                return;
            }

            if (!moneyManager.HasEnoughTicket(MoneyManager.RewardType.Ticket, 50))
            {
                audioManager.PlaySfxByTag(SfxTag.NotEnoughMoney);
                if (!DOTween.IsTweening(ticketbtn)) ticketbtn.DOPunchPosition(new Vector3(10, 0, 0), 0.5f);
            }

            SetBtnActive();
        }

        public void CoinBtnClicked()
        {
            if (!isAnimPlaying && status == GachaponStatus.Idle &&
                moneyManager.SubtractMoney(MoneyManager.RewardType.GachaCoin, 1))
            {
                InsertCoinAnimation();
                audioManager.PlaySfxByTag(SfxTag.UI_Popup2);
                TutorialManager.Instancee.CoinBtnClicked();
                SetBtnActive();
                return;
            }

            if (!moneyManager.HasEnoughTicket(MoneyManager.RewardType.GachaCoin, 1))
            {
                audioManager.PlaySfxByTag(SfxTag.NotEnoughMoney);
                if (!DOTween.IsTweening(coinBtn)) coinBtn.DOPunchPosition(new Vector3(10, 0, 0), 0.5f);
            }

            SetBtnActive();
        }

        public void InsertCoinAnimation()
        {
            isAnimPlaying = true;
            var velocityMultiplier = Random.Range(0.8f, 1.2f);
            var angleMultiplier = Random.Range(0.5f, 1.5f);
            var durationFactor = 2f;

            var startPos = MoneyManager.Instance.gachaCoinHolder_ui.transform.position;
            var path = CalculatePath(startPos, velocityMultiplier, angleMultiplier);

            var coinObject = MoneyManager.Instance.obj_pools[(int)MoneyManager.RewardType.GachaCoin].Get();
            InitializeCoinObject(coinObject, startPos);

            DOVirtual.DelayedCall(durationFactor * 0.6f,
                () => { coinObject.GetComponent<SpriteAnimator>().pauseAtIdx = 3; });

            PerformCoinAnimation(coinObject, path, durationFactor);
        }

        private Vector3[] CalculatePath(Vector3 startPos, float velocityMultiplier, float angleMultiplier)
        {
            var path = new Vector3[3];
            path[2] = coinInsertPos.position;

            var velocity = 0.5f * velocityMultiplier;
            var angle = angleMultiplier * Mathf.PI;

            path[0] = startPos + new Vector3(Mathf.Sin(angle) * velocity, Mathf.Cos(angle) * velocity, 0);
            path[0] = Vector3.Lerp(path[0], path[2], 0.4f);

            var diff = startPos - path[0];
            path[1] = Vector3.Lerp(path[0], path[2], Random.Range(0.3f, 0.5f)) - diff * Random.Range(0.3f, 0.8f);

            return path;
        }

        private void InitializeCoinObject(GameObject coinObject, Vector3 startPos)
        {
            coinObject.transform.localScale = Vector3.one;
            coinObject.transform.position = startPos;
            coinObject.transform.DORotate(Vector3.zero, 1f);
        }

        private void PerformCoinAnimation(GameObject coinObject, Vector3[] path, float durationFactor)
        {
            coinObject.transform.DOMove(path[0], 0.2f * durationFactor)
                .SetEase(Ease.OutCirc)
                .OnComplete(() =>
                {
                    coinObject.transform.DOPath(path, 0.8f * durationFactor, PathType.CatmullRom, PathMode.TopDown2D, 1)
                        .SetEase(Ease.InOutCubic)
                        .OnComplete(() => { FinishCoinAnimation(coinObject); });
                    coinObject.transform.DOScale(Vector3.zero, 0.79f * durationFactor)
                        .SetEase(Ease.InQuart);
                });
        }

        private void FinishCoinAnimation(GameObject coinObject)
        {
            MoneyManager.Instance.obj_pools[(int)MoneyManager.RewardType.GachaCoin].Release(coinObject);
            AudioManager.Instance.PlaySfxByTag(SfxTag.InsertCoin);
            status = GachaponStatus.InsertCoin;
            lever.DOPunchScale(Vector3.one, 0.5f, 5).SetLoops(-1);
            isAnimPlaying = false;
        }

        #region CapsuleAnimation

        private void RnadomizePonCapsule()
        {
            foreach (var img in gacapon_caplsule_ui) RandomizeImageAttributes(img);

            takeoutCapsule.localPosition = new Vector3(0, 20, 0);
            var rnd = Random.Range(0, gachapon_capsule_sprites.Length);

            myCapsule_s.sprite = gachapon_capsule_sprites[rnd];
            myCapsule_l.sprite = gachapon_capsule_fullsized_sprites[rnd];
        }

        private void RandomizeImageAttributes(Image image)
        {
            image.gameObject.transform.DOShakePosition(0.3f * RandomRangeMultiplier(),
                    new Vector3(0.1f * RandomRangeMultiplier(), 0.5f * RandomRangeMultiplier(), 0), 4)
                .SetEase(Ease.InOutSine);

            image.sprite = gachapon_capsule_sprites[Random.Range(0, gachapon_capsule_sprites.Length)];
            image.gameObject.transform.localEulerAngles = new Vector3(0, 0, 90 * Random.Range(-1, 2));
        }

        private float RandomRangeMultiplier()
        {
            return Random.Range(0.85f, 1.15f);
        }

        private void RotateLever()
        {
            isAnimPlaying = true;

            audioManager.PlaySfxByTag(SfxTag.GachaSimpleBgm);
            audioManager.PlaySfxByTag(SfxTag.GachaRotateLever);

            ResetLeverScaleAndAngle();
            RotateLeverAnimation();
        }

        private void ResetLeverScaleAndAngle()
        {
            DOTween.Kill(lever.transform);
            lever.transform.localScale = Vector3.one * 10f;
            lever.eulerAngles = Vector3.zero;
            takeoutCapsule.localPosition = new Vector3(0, 20, 0);
        }

        private void RotateLeverAnimation()
        {
            lever.DORotate(new Vector3(0, 0, -180), 0.7f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    audioManager.PlaySfxByTag(SfxTag.GachaRotateLever);
                    lever.DORotate(new Vector3(0, 0, -360), 0.7f)
                        .SetEase(Ease.InOutQuart);
                });

            DOVirtual.DelayedCall(0.7f, ShakePonCapsulesAndGet);
        }

        private void ShakePonCapsulesAndGet()
        {
            audioManager.PlaySfxByTag(SfxTag.GachaCapsules);
            DOTween.Kill(takeoutCapsule.transform);

            ShakeAllCapsules();
            MoveTakeoutCapsule();
        }

        private void ShakeAllCapsules()
        {
            foreach (var img in gacapon_caplsule_ui) ShakeCapsule(img.gameObject.transform);

            void ShakeCapsule(Transform capsuleTransform)
            {
                var randomFactor = Random.Range(0.85f, 1.15f);

                capsuleTransform.DOShakePosition(1.5f * randomFactor,
                        new Vector3(0.1f * randomFactor, 0.5f * randomFactor, 0), 4)
                    .SetEase(Ease.InOutSine);
            }
        }

        private void MoveTakeoutCapsule()
        {
            takeoutCapsule.DOLocalMoveY(0, 1f)
                .SetEase(Ease.OutBounce)
                .SetDelay(0.5f)
                .OnComplete(OnTakeoutCapsuleMoveComplete)
                .OnStart(() => { audioManager.PlaySfxByTag(SfxTag.GachaDrop); });
        }

        private void OnTakeoutCapsuleMoveComplete()
        {
            isAnimPlaying = false;
            status = GachaponStatus.RotateLever;
            takeoutCapsule.transform.DOScale(1.05f, 0.2f)
                .SetEase(Ease.InOutQuad)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void CapsuleCloseUp()
        {
            isAnimPlaying = true;

            DOTween.Kill(takeoutCapsule.transform);
            takeoutCapsule.transform.localScale = Vector3.one;

            takeoutCapsule.DOLocalMoveY(-500, 1f)
                .OnComplete(() =>
                {
                    isAnimPlaying = true;
                    status = GachaponStatus.ClosUp;
                })
                .SetEase(Ease.InOutSine);
            takeoutCapsule.DORotate(new Vector3(0, 0, 30), 0.5f);
            capsuleAnimation.PlayReadyAnimation();
        }

        public void CapsuleAnimFinished(bool isNew)
        {
            isAnimPlaying = false;
            status = GachaponStatus.Idle;
            sfxController.ChangeBGMVolume();
            RnadomizePonCapsule();
            TutorialManager.Instancee.GotPet();
            if (isNew) DOVirtual.DelayedCall(0.5f, HidePanel);
        }

        #endregion
    }
}