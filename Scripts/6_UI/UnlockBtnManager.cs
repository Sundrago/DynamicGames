using DG.Tweening;
using DynamicGames.MainPage;
using DynamicGames.MiniGames;
using DynamicGames.System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DynamicGames.UI
{
    /// <summary>
    /// Responsible for managing the unlocking button functionality.
    /// </summary>
    public class UnlockBtnManager : MonoBehaviour
    {
        [SerializeField] private Transform ticketbtn, coinBtn;
        [SerializeField] private Image ticketActive, coinActive, ticket, coin, arrow;
        [SerializeField] private Transform ticketGlow, coinGlow;
        [SerializeField] private GameObject unlockFX;
        [SerializeField] private BlockDragHandler targetBlockObj;

        private bool isAnimPlaying;
        private bool isHidden;
        private GameType targetGameType;
        
        public static UnlockBtnManager Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
            InitColor();
            gameObject.SetActive(false);
        }

        private void InitColor()
        {
            ticket.color = new Color(1, 1, 1, 0);
            coin.color = new Color(1, 1, 1, 0);
            ticketActive.color = new Color(1, 1, 1, 0);
            coinActive.color = new Color(1, 1, 1, 0);
        }

        public void SetBtnActive()
        {
            ticketbtn.GetComponent<Button>().interactable = true;
            coinBtn.GetComponent<Button>().interactable = true;

            var isTicketActive = MoneyManager.Instance.HasEnoughTicket(MoneyManager.RewardType.Ticket, 100) ? 1 : 0;
            var isCoinActive = MoneyManager.Instance.HasEnoughTicket(MoneyManager.RewardType.Key, 1) ? 1 : 0;

            DOTween.Kill(arrow);
            DOTween.Kill(ticketActive);
            DOTween.Kill(coinActive);
            ticketActive.DOFade(isTicketActive, 0.3f)
                .SetDelay(0.2f);
            coinActive.DOFade(isCoinActive, 0.3f)
                .SetDelay(0.2f);

            if (isTicketActive == 1 && !DOTween.IsTweening(ticketGlow))
            {
                ticketGlow.localPosition = new Vector3(-400, 0, 0);
                ticketGlow.DOLocalMoveX(400, 1)
                    .SetDelay(0.3f);
            }

            if (isCoinActive == 1 && !DOTween.IsTweening(coinGlow))
            {
                coinGlow.localPosition = new Vector3(-400, 0, 0);
                coinGlow.DOLocalMoveX(400, 1)
                    .SetDelay(0.3f);
            }

            if (isCoinActive == 1 && !DOTween.IsTweening(coinBtn.gameObject.transform))
                coinBtn.gameObject.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.5f, 1)
                    .SetLoops(-1, LoopType.Restart)
                    .SetEase(Ease.OutQuad);

            if (isCoinActive == 0 && DOTween.IsTweening(coinBtn.gameObject.transform))
            {
                DOTween.Kill(coinBtn.gameObject.transform);
                coinBtn.gameObject.transform.localScale = Vector3.one;
            }
        }

        public void TicketBtnClicked()
        {
            if (!MoneyManager.Instance.HasEnoughTicket(MoneyManager.RewardType.Ticket, 100))
            {
                AudioManager.Instance.PlaySfxByTag(SfxTag.Unable);
                if (!DOTween.IsTweening(ticketbtn)) ticketbtn.DOPunchPosition(new Vector3(10, 0, 0), 0.5f);
                return;
            }

            if (MoneyManager.Instance.SubtractMoney(MoneyManager.RewardType.Ticket, 100))
            {
                MoneyManager.Instance.Reward2DAnimation(MoneyManager.RewardType.Key, ticketbtn.transform.position, 1);
                AudioManager.Instance.PlaySfxByTag(SfxTag.AcquiredCoin);
            }

            SetBtnActive();
        }

        public void CoinBtnClicked()
        {
            if (!MoneyManager.Instance.HasEnoughTicket(MoneyManager.RewardType.Key, 1))
            {
                AudioManager.Instance.PlaySfxByTag(SfxTag.Unable);
                if (!DOTween.IsTweening(coinBtn)) coinBtn.DOPunchPosition(new Vector3(10, 0, 0), 0.5f);
                return;
            }

            if (!isAnimPlaying && MoneyManager.Instance.SubtractMoney(MoneyManager.RewardType.Key, 1)) InitKeyAnimation();

            TutorialManager.Instancee.GameUnlocked();
            SetBtnActive();
            targetBlockObj.OnButtonClicked();
            Hide(true);
        }

        [Button]
        public void InitKeyAnimation()
        {
            AudioManager.Instance.PlaySfxByTag(SfxTag.Sparkle);
            isAnimPlaying = true;
            var _velocity = 0.5f;
            var startPos = MoneyManager.Instance.keyHolder_ui.transform.position;
            var durationFactor = 2f;

            var path = new Vector3[3];
            path[2] = targetBlockObj.gameObject.transform.position;
            var obj = MoneyManager.Instance.obj_pools[(int)MoneyManager.RewardType.Key].Get();
            var velocity = _velocity * Random.Range(0.8f, 1.2f);

            obj.transform.localScale = Vector3.one;
            obj.transform.position = startPos;

            var angle = Random.Range(0.5f, 1.5f) * Mathf.PI;
            path[0] = startPos + new Vector3(Mathf.Sin(angle) * velocity, Mathf.Cos(angle) * velocity, 0);
            path[0] = Vector3.Lerp(path[0], path[2], 0.4f);

            var diff = startPos - path[0];
            path[1] = Vector3.Lerp(path[0], path[2], Random.Range(0.3f, 0.5f)) - diff * Random.Range(0.3f, 0.8f);

            DOVirtual.DelayedCall(durationFactor * 0.6f, () => { obj.GetComponent<SpriteAnimator>().pauseAtIdx = 10; });

            obj.transform.DORotate(Vector3.zero, 1f);
            obj.transform.DOMove(path[0], 0.2f * durationFactor)
                .SetEase(Ease.OutCirc)
                .OnComplete(() =>
                {
                    obj.transform.DOPath(path, 0.8f * durationFactor, PathType.CatmullRom, PathMode.TopDown2D, 1)
                        .SetEase(Ease.InOutCubic)
                        .OnComplete(() =>
                        {
                            MoneyManager.Instance.obj_pools[(int)MoneyManager.RewardType.Key].Release(obj);
                            AudioManager.Instance.PlaySfxByTag(SfxTag.InsertCoin);
                            isAnimPlaying = false;
                            var fx = Instantiate(unlockFX);
                            fx.transform.SetParent(targetBlockObj.transform, true);
                            fx.transform.localPosition = Vector3.zero;
                            fx.SetActive(true);
                            targetBlockObj.Unlock();
                            AudioManager.Instance.PlaySfxByTag(SfxTag.Key);
                        });
                    obj.transform.DOScale(Vector3.zero, 0.79f * durationFactor)
                        .SetEase(Ease.InQuart);
                });
        }

        public void Init(BlockDragHandler targetBlockObj)
        {
            this.targetBlockObj = targetBlockObj;
            SetupInitialState();

            gameObject.transform.SetParent(this.targetBlockObj.transform, true);
            gameObject.transform.DOLocalMove(new Vector3(5f, -12, 0), 0.5f);
            gameObject.transform.DOLocalRotate(Vector3.zero, 0.5f);

            ticket.DOFade(1, 2f);
            coin.DOFade(1, 2f);
            arrow.DOFade(1, 2f);
            SetBtnActive();
            isHidden = false;
        }

        private void SetupInitialState()
        {
            DOTween.Kill(coinActive);
            gameObject.SetActive(true);
            ticket.color = new Color(1, 1, 1, 0);
            coin.color = new Color(1, 1, 1, 0);
            ticketActive.color = new Color(1, 1, 1, 0);
            coinActive.color = new Color(1, 1, 1, 0);
        }

        public void Hide(bool forceHide = false)
        {
            if (!forceHide)
            {
                if (isHidden) return;
                if (DOTween.IsTweening(coinActive)) return;
            }

            KillTween();
            InitHideAnimation();
        }

        private void KillTween()
        {
            DOTween.Kill(ticket);
            DOTween.Kill(coin);
            DOTween.Kill(arrow);
            DOTween.Kill(coinActive);
            DOTween.Kill(ticketActive);
        }

        private void InitHideAnimation()
        {
            ticket.DOFade(0, 1f);
            coin.DOFade(0, 1f);
            coinActive.DOFade(0, 1f);
            arrow.DOFade(0, 1f);
            ticketActive.DOFade(0, 1f);
            arrow.DOFade(0, 1f)
                .OnComplete(() => { gameObject.SetActive(false); });
            ticketbtn.GetComponent<Button>().interactable = false;
            coinBtn.GetComponent<Button>().interactable = false;
            isHidden = true;
        }
    }
}