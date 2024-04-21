using System.Collections.Generic;
using DG.Tweening;
using DynamicGames.Pet;
using DynamicGames.System;
using DynamicGames.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicGames.MiniGames.Jump
{
    /// <summary>
    ///     Handles the management and control of the MiniGame Jump.
    /// </summary>
    public class GameManager : MiniGameManager, IMiniGame
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private SfxController sfxController;
        [SerializeField] private StageManager stageManager;
        [SerializeField] private PetManager petManager;
        [SerializeField] private TouchInputController inputController;

        [Header("UI Components")] 
        [SerializeField] private TextMeshProUGUI title, score_ui, highscore_ui;
        [SerializeField] private GameObject tutorial, tutorial_cursor, tutorial_text;

        [Header("Game Components")] 
        [SerializeField] private Rigidbody player;
        [SerializeField] private Transform footstepHolder, palyerScaler;
        [SerializeField] private GameObject footstep_posA, footstep_posB, footstep_posC, cylindar, playerPlaceHolder;
        [SerializeField] private SpriteAnimator playerRenderer;
        [SerializeField] private ItemObject item_prefab;

        private const float ScrollSpeedMax = 0.35f;
        private const float ScrollSpeedMin = 0.05f;
        private const float Friction = 0.015f;
        
        private readonly List<GameObject> footsteps = new();
        private bool firstGame, hasRevived, highFXShown;
        private float scrollSpeed, reviveTimer;
        private GameStatus status;
        public int CurrentScore { get; set; }
        public int HighScore { get; private set; }

        private enum GameStatus
        {
            PreGame,
            Playing,
            GameOver,
            Revive
        }

        private void Start()
        {
            title.color = new Color(1f, 1f, 1f, 0f);
            cylindar.transform.localEulerAngles = new Vector3(0f, 210f, 0f);
            tutorial.SetActive(false);
        }

        private void Update()
        {
            switch (status)
            {
                case GameStatus.PreGame:
                    HandlePreGameStatus();
                    break;
                case GameStatus.Playing:
                    HandlePlayingStatus();
                    break;
                case GameStatus.Revive:
                    HandleReviveStatus();
                    break;
            }
        }

        public override void RestartGame()
        {
            ResetUI();
            Time.timeScale = 1f;
            CurrentScore = 0;
            player.gameObject.SetActive(true);
            highFXShown = false;
            DOTween.Kill(cylindar.transform);
            player.isKinematic = false;
            inputController.gameObject.SetActive(true);
            GameScoreManager.Instance.StartGame(GameType.jump);
        }

        public override void ClearGame()
        {
            ResetUI();
            Time.timeScale = 1f;
            DestroyFootsteps();
            gameObject.transform.localScale = new Vector3(1, 0, 1);
            player.isKinematic = true;
            player.velocity = Vector3.zero;
            player.transform.localPosition = new Vector3(0f, -5f, 1.25f);
        }

        public override void OnGameEnter()
        {
            player.gameObject.SetActive(true);
            gameObject.transform.parent.gameObject.SetActive(true);
            tutorial.SetActive(false);
            firstGame = true;
            GameScoreManager.Instance.HideScore();
            gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), 1.5f)
                .OnComplete(() =>
                {
                    if (GameScoreManager.Instance.GetHighScore(GameType.jump) < 15)
                    {
                        tutorial_cursor.GetComponent<Image>().DOFade(0.5f, 0.5f);
                        tutorial_text.transform.DOScale(Vector3.one, 0.5f);
                        tutorial.SetActive(true);
                    }

                    TutorialManager.Instancee.JumpGameEntered();
                });

            LoadGame();
            DOVirtual.DelayedCall(3f, () => { RestartGame(); });
            firstGame = false;
        }

        public override void SetupPet(bool isPlayingWithPet, PetObject petObject = null)
        {
            playerPlaceHolder.SetActive(!isPlayingWithPet);
            playerRenderer.gameObject.SetActive(isPlayingWithPet);

            if (isPlayingWithPet)
            {
                playerRenderer.sprites = petObject.GetJumpAnim();
                playerRenderer.GetComponent<SpriteRenderer>().sprite = playerRenderer.sprites[0];

                playerRenderer.gameObject.transform.localRotation = petObject.spriteRenderer.transform.localRotation;
                playerRenderer.gameObject.transform.localPosition = petObject.spriteRenderer.transform.localPosition;
                playerRenderer.gameObject.transform.localScale = petObject.spriteRenderer.transform.localScale;

                playerRenderer.interval = 0.9f / playerRenderer.sprites.Length;
            }
        }

        private void HandlePreGameStatus()
        {
            if (player.transform.position.y > -0.25)
            {
                tutorial_text.transform.DOScale(Vector3.zero, 0.5f);
                tutorial_cursor.GetComponent<Image>().DOFade(0f, 0.5f)
                    .OnComplete(() => { tutorial.SetActive(false); });
                UpdateGameStatus(GameStatus.Playing);
            }

            UpdateFootsteps();
        }

        private void HandlePlayingStatus()
        {
            AdjustGamePace();
            UpdateScrollSpeed();
            PerformScrollMotion();
            UpdateFootsteps();
            CheckGameOver();
            UpdateScore();
        }

        private void AdjustGamePace()
        {
            Time.timeScale = 1 + 0.008f * CurrentScore;
        }

        private void UpdateScrollSpeed()
        {
            var max = ScrollSpeedMax * (CurrentScore + 80) / 80;
            var min = ScrollSpeedMin * (CurrentScore + 80) / 80;

            if (player.transform.position.y > -0.25f)
            {
                if (scrollSpeed < max)
                    scrollSpeed += (max - scrollSpeed) * Friction * Mathf.Abs(player.transform.position.y + 0.25f) / 2;
            }
            else
            {
                if (scrollSpeed > min)
                    scrollSpeed += (min - scrollSpeed) * Friction * Mathf.Abs(-player.transform.position.y - 0.25f) / 2;
            }
        }

        private void PerformScrollMotion()
        {
            var updatePos = footstepHolder.transform.localPosition;
            updatePos.y -= scrollSpeed * Time.deltaTime;
            footstepHolder.transform.localPosition = updatePos;
        }

        private void HandleReviveStatus()
        {
            if (Time.time > reviveTimer + 4f)
            {
                UpdateGameStatus(GameStatus.Playing);
                player.isKinematic = false;
            }
        }

        private void UpdateFootsteps()
        {
            foreach (var obj in footsteps)
                if (obj.transform.position.y >= footstep_posA.transform.position.y)
                {
                    obj.transform.localScale = new Vector3(0, 0, 0);
                }
                else if (obj.transform.position.y <= footstep_posB.transform.position.y)
                {
                    obj.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    var normal = 1 - (obj.transform.position.y - footstep_posB.transform.position.y) /
                        (footstep_posA.transform.position.y - footstep_posB.transform.position.y);
                    normal = EaseOutBack(normal);
                    obj.transform.localScale = new Vector3(normal, normal, normal);
                }

            float EaseOutBack(float normal)
            {
                const float c1 = 1.70158f;
                const float c3 = c1 + 1f;

                return 1 + c3 * Mathf.Pow(normal - 1, 3) + c1 * Mathf.Pow(normal - 1, 2);
            }
        }

        private void CheckGameOver()
        {
            if (player.transform.position.y < -10f) UpdateGameStatus(GameStatus.GameOver);
        }

        private void UpdateScore()
        {
            if (footsteps.Count > 0 && footsteps[0].transform.position.y <= footstep_posC.transform.position.y)
            {
                Destroy(footsteps[0]);
                footsteps.RemoveAt(0);
                if (footsteps.Count < 15) stageManager.GenerateStageByDifficulty(0.05f, 0.15f, 0.7f);
            }
        }

        private void UpdateGameStatus(GameStatus _status)
        {
            if (status == _status) return;
            status = _status;

            switch (_status)
            {
                case GameStatus.PreGame:
                    hasRevived = false;
                    break;
                case GameStatus.Playing:
                    break;
                case GameStatus.GameOver:
                    if (!hasRevived) GameOver();
                    else ShowScore();
                    break;
                case GameStatus.Revive:
                    break;
            }
        }

        private void GameOver()
        {
            sfxController.PlaySfx(0);
            WatchAdsContinueGame.Instance.Init(ContinueGameAfterAdWatch, ShowScore, "Jump_Revive");
        }

        private void ShowScore()
        {
            player.gameObject.SetActive(false);
            Time.timeScale = 1f;
            GameScoreManager.Instance.ShowScore(CurrentScore, GameType.jump);
            LoadGame();
        }

        private void ContinueGameAfterAdWatch()
        {
            UpdateGameStatus(GameStatus.Revive);
            player.isKinematic = true;
            player.velocity = Vector3.zero;
            player.transform.localPosition = new Vector3(0f, -5f, 1.25f);
            player.transform.DOLocalMove(new Vector3(0f, -1.5f, 1.25f), 1.5f)
                .SetEase(Ease.OutExpo);
            reviveTimer = Time.time;
            hasRevived = true;
        }

        public void DestroyFootsteps()
        {
            for (var i = footsteps.Count - 1; i >= 0; i--) Destroy(footsteps[i]);
            footsteps.Clear();
        }

        public void AddFootstepObject(GameObject obj)
        {
            footsteps.Add(obj);
        }

        public void UpdateScoreUI(int score)
        {
            score_ui.text = score.ToString();
            score_ui.transform.localScale = new Vector3(1, 1, 1);
            score_ui.transform.DOShakeScale(0.5f, 0.1f);

            HighScore = PlayerPrefs.GetInt("highscore_jump");

            if (score > HighScore)
            {
                if (!highFXShown && HighScore > 0)
                {
                    highFXShown = true;
                    GameScoreManager.Instance.ShowNewHighFX();
                }

                highscore_ui.transform.localScale = new Vector3(1, 1, 1);
                highscore_ui.transform.DOShakeScale(0.75f, 0.15f);
                HighScore = score;
            }

            highscore_ui.text = HighScore.ToString();
        }

        public void LoadGame()
        {
            StageLoadingAnimation();
            UpdateGameStatus(GameStatus.PreGame);

            if (firstGame)
                SetUpFirstGame();
            else
                SetUpRegularGame();
        }

        private void StageLoadingAnimation()
        {
            DOVirtual.DelayedCall(0.2f, () => { stageManager.GenerateStages(true); });
            DOTween.Kill(footstepHolder.transform);
            DOTween.Kill(player.transform);

            footstepHolder.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            footstepHolder.transform.DOLocalMove(Vector3.zero, 3f)
                .SetEase(Ease.OutQuint);
            footstep_posA.transform.DOMoveY(-5f, 1.5f)
                .From().SetEase(Ease.Linear);
            footstep_posB.transform.DOMoveY(-5f, 1.5f)
                .From().SetEase(Ease.Linear)
                .SetDelay(0.5f);
            player.isKinematic = true;
            player.velocity = Vector3.zero;
            player.transform.localPosition = new Vector3(0f, -5f, 1.25f);
            player.transform.DOLocalMove(new Vector3(0f, -1.5f, 1.25f), 2f)
                .SetEase(Ease.OutBack);
        }

        private void SetUpFirstGame()
        {
            cylindar.transform.localEulerAngles = new Vector3(0f, 210f, 0f);
            inputController.ResetRotation();
            title.gameObject.SetActive(true);

            if (DOTween.IsTweening(title)) DOTween.Kill(title);

            title.color = new Color(1f, 1f, 1f, 0f);
            title.DOFade(1f, 3f);
            title.DOFade(0f, 2f).SetDelay(3f).OnComplete(() => { title.gameObject.SetActive(false); });
        }

        private void SetUpRegularGame()
        {
            cylindar.transform.DOLocalRotate(new Vector3(0f, -720, 0f), 50f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetRelative(true);
            inputController.gameObject.SetActive(false);
        }

        private void ResetUI()
        {
            CurrentScore = 0;
            UpdateScoreUI(CurrentScore);
            GameScoreManager.Instance.HideScore();
        }
    }
}