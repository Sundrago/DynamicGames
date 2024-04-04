using System.Collections.Generic;
using DG.Tweening;
using DynamicGames.Gachapon;
using DynamicGames.MiniGames;
using DynamicGames.MiniGames.Jump;
using DynamicGames.Pet;
using DynamicGames.System;
using DynamicGames.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace DynamicGames.MainPage
{
    /// <summary>
    ///     Handles main canvas in the game.
    /// </summary>
    public class MainPage : MonoBehaviour
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private SfxController sfxController;
        [SerializeField] private TransitionManager transitionManager;
        [SerializeField] private RankingManager rankingManager;
        [SerializeField] private GachaponManager gachaponManager;
        [SerializeField] private LeaderboardManger leaderboardManger;
        [SerializeField] private DailyTicketRewardsManager dailyTicketRewardsManager;
        [SerializeField] private GameManager jumpGameManager;
        [SerializeField] private MiniGames.Shoot.GameManager shootGameManager;
        
        [Header("UI Components")] 
        [SerializeField] private PetInventoryUIManager petInventoryUIManager;
        [SerializeField] private AskForUserReview askForUserReview;
        [SerializeField] private GameObject ranking_ui;
        [SerializeField] private TitleDragHandler title;
        [SerializeField] private List<BlockDragHandler> dragSprites;
        [SerializeField] private GameObject build, land, jump, shoot, main;
        
        private GameObject currentGameBtn;

        public static MainPage Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            sfxController.PlayBGM(3);
        }

        /// <summary>
        ///     Navigates to a specific game based on the provided block type.
        /// </summary>
        public void GotoSelectedGame(BlockStatusManager.BlockType blockType, GameObject gamebtn)
        {
            if (ranking_ui.activeSelf) return;

            DOTween.Kill(gamebtn.transform);
            currentGameBtn = gamebtn;
            var miniIsland = currentGameBtn.GetComponent<BlockDragHandler>().miniisland;
            if (miniIsland != null)
                if (DOTween.IsTweening(miniIsland.transform))
                    return;

            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Click);

            switch (blockType)
            {
                case BlockStatusManager.BlockType.build:
                    PetInGameManager.Instance.EnterGame(GameType.build);
                    transitionManager.canvas_B = build;
                    sfxController.PlayBGM(1);
                    sfxController.PlaySfx(2);
                    MoneyManager.Instance.HidePanel();
                    break;
                case BlockStatusManager.BlockType.land:
                    PetInGameManager.Instance.EnterGame(GameType.land);
                    transitionManager.canvas_B = land;
                    sfxController.PlayBGM(2);
                    sfxController.PlaySfx(2);
                    MoneyManager.Instance.HidePanel();
                    break;
                case BlockStatusManager.BlockType.jump:
                    PetInGameManager.Instance.EnterGame(GameType.jump);
                    jumpGameManager.ClearGame();
                    transitionManager.canvas_B = jump;
                    sfxController.PlayBGM(0);
                    sfxController.PlaySfx(2);
                    MoneyManager.Instance.HidePanel();
                    break;
                case BlockStatusManager.BlockType.shoot:
                    PetInGameManager.Instance.EnterGame(GameType.shoot);
                    shootGameManager.ClearGame();
                    transitionManager.canvas_B = shoot;
                    sfxController.PlayBGM(5);
                    sfxController.PlaySfx(2);
                    MoneyManager.Instance.HidePanel();
                    break;
                case BlockStatusManager.BlockType.leaderboard:
                    leaderboardManger.OpenLeaderboard();
                    break;
                case BlockStatusManager.BlockType.gacha:
                    gachaponManager.ShowPanel();
                    break;
                case BlockStatusManager.BlockType.friends:
                    petInventoryUIManager.ShowPanel();
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
            PlayIconAnimation();
        }

        private void PlayIconAnimation()
        {
            var miniisland = currentGameBtn.GetComponent<BlockDragHandler>().miniisland;
            if (currentGameBtn.GetComponent<BlockDragHandler>().miniisland != null)
            {
                miniisland = currentGameBtn.GetComponent<BlockDragHandler>().miniisland;
                miniisland.SetActive(true);
                miniisland.transform.localScale = new Vector3(0.2f, 0.2f, 1);
                miniisland.transform.DOScale(new Vector3(100, 100, 100), 1f)
                    .SetEase(Ease.InExpo)
                    .OnComplete(() =>
                    {
                        transitionManager.canvas_A = main;
                        transitionManager.OnTransition = true;
                        transitionManager.HideCanvasA();
                        transitionManager.OnTransitionPoint();
                    });
            }
        }

        public void ReturnToMainPage()
        {
            MoneyManager.Instance.ShowPanel();
            PetInGameManager.Instance.ExitGame();
            sfxController.PlayBGM(3);
            if (currentGameBtn == null) return;
            currentGameBtn.GetComponent<Rigidbody2D>().isKinematic = true;

            if (currentGameBtn.GetComponent<BlockDragHandler>().miniisland != null)
            {
                var miniisland = currentGameBtn.GetComponent<BlockDragHandler>().miniisland;
                DOTween.Kill(miniisland.transform);
                miniisland.transform.localScale = new Vector3(100, 100, 100);
                miniisland.transform.DOScale(new Vector3(0.2f, 0.2f, 1), 1f)
                    .SetEase(Ease.OutExpo)
                    .OnComplete(() =>
                    {
                        miniisland.SetActive(false);
                        currentGameBtn.GetComponent<BlockDragHandler>().Deactivate();
                        currentGameBtn = null;
                        leaderboardManger.Start();
                        StartCoroutine(rankingManager.UpdateRanks());
                        TutorialManager.Instancee.WentBackHome();
                    });
            }
        }

        public void DeactivateAllBlocksExcept(GameObject except = null)
        {
            foreach (var dragSprite in dragSprites)
            {
                if (dragSprite == null) continue;
                if (dragSprite.gameObject == except) continue;
                if (!dragSprite.gameObject.activeSelf) continue;
                dragSprite.Deactivate();
            }
        }

        public void DeactivateAllBlocks()
        {
            foreach (var dragSprite in dragSprites)
            {
                if (dragSprite == null) continue;
                if (!dragSprite.gameObject.activeSelf) continue;
                dragSprite.Deactivate();
            }
        }

        public void ResetBlockPositions()
        {
            title.ReturnToOriginalPosition();
            foreach (var dragSprite in dragSprites)
            {
                if (dragSprite == null) continue;
                if (!dragSprite.gameObject.activeSelf) continue;
                dragSprite.ReturnToOriginalPosition();
            }
        }
    }
}