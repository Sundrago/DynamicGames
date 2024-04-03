using System;
using System.Collections;
using System.Collections.Generic;
using Core.Gacha;
using Core.Pet;
using Core.System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

namespace Core.Main
{
    /// <summary>
    /// Handles main canvas in the game.
    /// </summary>
    public class MainCanvas : MonoBehaviour
    {
        [SerializeField] SfxController sfx;
        [SerializeField] TransitionManager transition;
        [SerializeField] GameObject build, land, jump, shoot, main;
        [SerializeField] LeaderboardManger leaderboard;
        [SerializeField] private RankingManager rangkingManager;
        [SerializeField] private GachaponManager gachaponManager;
        [FormerlySerializedAs("inventory")] [FormerlySerializedAs("petDrawer")] [SerializeField] private PetInventory petInventory;
        [SerializeField] private AskForUserReview askForUserReview;
        [SerializeField] List<BlockDragHandler> dragSprites;
        [SerializeField] private GameObject ranking_ui;
        [SerializeField] private TitleDragHandler title;

        [SerializeField] private Games.Jump.GameManager jumpGameManager;

        [FormerlySerializedAs("shootGameManager")] [SerializeField]
        private Games.Shoot.GameManager gameManager;

        [SerializeField] private DailyTicketRewardsManager dailyTicketRewardsManager;

        private GameObject currentGameBtn = null;
        public static MainCanvas Instance;


        private void Awake()
        {
            Instance = this;

        }

        void Start()
        {
            Application.targetFrameRate = 60;
            sfx.PlayBGM(3);
        }

        public void GotoGame(BlockStatusManager.BlockType blockType, GameObject gamebtn)
        {
            if (ranking_ui.activeSelf) return;

            DOTween.Kill(gamebtn.transform);
            currentGameBtn = gamebtn;
            GameObject miniisland = currentGameBtn.GetComponent<BlockDragHandler>().miniisland;
            if (miniisland != null)
                if (DOTween.IsTweening(miniisland.transform))
                    return;

            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Click);

            switch (blockType)
            {
                case BlockStatusManager.BlockType.build:
                    PetInGameManager.Instance.EnterGame(GameType.build);
                    transition.canvas_B = build;
                    sfx.PlayBGM(1);
                    sfx.PlaySfx(2);
                    MoneyManager.Instance.HidePanel();
                    break;
                case BlockStatusManager.BlockType.land:
                    PetInGameManager.Instance.EnterGame(GameType.land);
                    transition.canvas_B = land;
                    sfx.PlayBGM(2);
                    sfx.PlaySfx(2);
                    MoneyManager.Instance.HidePanel();
                    break;
                case BlockStatusManager.BlockType.jump:
                    PetInGameManager.Instance.EnterGame(GameType.jump);
                    jumpGameManager.ClearGame();
                    transition.canvas_B = jump;
                    sfx.PlayBGM(0);
                    sfx.PlaySfx(2);
                    MoneyManager.Instance.HidePanel();
                    break;
                case BlockStatusManager.BlockType.shoot:
                    PetInGameManager.Instance.EnterGame(GameType.shoot);
                    gameManager.ClearGame();
                    transition.canvas_B = shoot;
                    sfx.PlayBGM(5);
                    sfx.PlaySfx(2);
                    MoneyManager.Instance.HidePanel();
                    break;
                case BlockStatusManager.BlockType.leaderboard:
                    leaderboard.OpenLeaderboard();
                    break;
                case BlockStatusManager.BlockType.gacha:
                    gachaponManager.ShowPanel();
                    break;
                case BlockStatusManager.BlockType.friends:
                    petInventory.ShowPanel();
                    break;
                case BlockStatusManager.BlockType.review:
                    askForUserReview.ShowPanel();
                    break;
                case BlockStatusManager.BlockType.tutorial:
                    TutorialManager.Instancee.tutorial_01();
                    break;
                case BlockStatusManager.BlockType.tv:
                    dailyTicketRewardsManager.WatchAdsBtnClicked();
                    break;
            }

            if (blockType != BlockStatusManager.BlockType.tutorial) TutorialManager.Instancee.HideCursor();

            currentGameBtn.GetComponent<Rigidbody2D>().isKinematic = true;
            if (currentGameBtn.GetComponent<BlockDragHandler>().miniisland != null)
            {
                miniisland = currentGameBtn.GetComponent<BlockDragHandler>().miniisland;
                miniisland.SetActive(true);
                miniisland.transform.localScale = new Vector3(0.2f, 0.2f, 1);
                miniisland.transform.DOScale(new Vector3(100, 100, 100), 1f)
                    .SetEase(Ease.InExpo)
                    .OnComplete(() =>
                    {
                        transition.canvas_A = main;
                        transition.OnTransition = true;

                        transition.HideCanvasA();
                        transition.OnTransitionPoint();
                    });
            }
        }

        public void WentBackHome()
        {
            MoneyManager.Instance.ShowPanel();
            PetInGameManager.Instance.ExitGame();
            sfx.PlayBGM(3);
            if (currentGameBtn == null) return;
            currentGameBtn.GetComponent<Rigidbody2D>().isKinematic = true;

            if (currentGameBtn.GetComponent<BlockDragHandler>().miniisland != null)
            {
                GameObject miniisland = currentGameBtn.GetComponent<BlockDragHandler>().miniisland;
                DOTween.Kill(miniisland.transform);
                miniisland.transform.localScale = new Vector3(100, 100, 100);
                miniisland.transform.DOScale(new Vector3(0.2f, 0.2f, 1), 1f)
                    .SetEase(Ease.OutExpo)
                    .OnComplete(() =>
                    {
                        miniisland.SetActive(false);
                        currentGameBtn.GetComponent<BlockDragHandler>().Deactivate();
                        currentGameBtn = null;
                        leaderboard.Start();
                        StartCoroutine(rangkingManager.UpdateRanks());
                        TutorialManager.Instancee.WentBackHome();
                    });
            }
        }

        public void Offall(GameObject except = null)
        {
            foreach (BlockDragHandler dragSprite in dragSprites)
            {
                if (dragSprite == null) continue;
                if (dragSprite.gameObject == except) continue;
                if (!dragSprite.gameObject.activeSelf) continue;
                dragSprite.Deactivate();
            }
        }

        public void Offall()
        {
            foreach (BlockDragHandler dragSprite in dragSprites)
            {
                if (dragSprite == null) continue;
                if (!dragSprite.gameObject.activeSelf) continue;
                dragSprite.Deactivate();
            }
        }

        public void ReturnToOriginalPos()
        {
            title.ReturnToOriginalPosition();
            foreach (BlockDragHandler dragSprite in dragSprites)
            {
                if (dragSprite == null) continue;
                if (!dragSprite.gameObject.activeSelf) continue;
                dragSprite.ReturnToOriginalPosition();
            }
        }
    }
}