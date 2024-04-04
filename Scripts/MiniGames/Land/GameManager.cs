using System.Collections.Generic;
using DG.Tweening;
using DynamicGames.Pet;
using DynamicGames.System;
using DynamicGames.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicGames.MiniGames.Land
{
    /// <summary>
    ///     Responsible for managing the game logic and components of the game.
    /// </summary>
    public class GameManager : MiniGameManager, IMiniGame
    {
        private const float DeltaTimeVelocity = 50f;

        [Header("Managers and Controllers")] 
        [SerializeField] private StageManager stageManager;
        [SerializeField] private IslandSizeController islandSizeController;
        [SerializeField] private PetManager petManager;
        [SerializeField] private SoundEffectsController sfxController;
        [SerializeField] private ParticleFXManager particleFXManager;
        [SerializeField] private BackgroundElementsManager bgAnimator;

        [Header("UI Components")] 
        [SerializeField] private UIManager uiManager;
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private SpriteAnimator playerRenderer;

        [Header("Game Components")] 
        [SerializeField] public StageLevelAnimator stageLevelAnimator;
        [SerializeField] private Transform rocketDummy;
        [SerializeField] private Animator rocket, rocketCanvas;
        [SerializeField] private Animator screenShakeAnimator;
        [SerializeField] private GameObject playerPlaceHolder;

        public Vector2 failPosition;
        public Quaternion failRotation;
        [SerializeField] private Dictionary<PetType, Vector3> CustomPetPos;

        private Vector3 initialRocketPos;
        private Quaternion initialRocketRot;
        private bool isBodyCollided, isLeftFootCollided, isRightFootCollided, hasRevived;
        private bool isInitialLaunch = true;
        private bool playing = true;
        private Rigidbody2D rocketRigidBody2D;
        private int score, highScore, currentLevel;
        public bool isButtonUpPressed { get; private set; }
        public bool isButtonLeftPressed { get; private set; }
        public bool isButtonRightPressed { get; private set; }
        public bool resetAnimPlaying { get; private set; }
        public bool holdTransition { get; private set; }

        private void Start()
        {
            uiManager.InitializeTutorialPanel();
            InitializeRocket();
            particleFXManager.InitializeParticleSystem();
        }

        private void Update()
        {
            bgAnimator.ProcessCircles(rocket.transform);
            if (!playing & !resetAnimPlaying) return;
            HandleResetAnimation();
            HandleButtonEvents();
            HandleGameStatus();
        }

        public override void ClearGame()
        {
            playerRenderer.PauseAnim();
            rocket.transform.position = initialRocketPos;
            rocket.transform.rotation = initialRocketRot;
            RocketIsReady();
            OnGameEnter();
            bgAnimator.RemoveCircles();
            GameScoreManager.Instance.HideScore();
            isInitialLaunch = true;
            hasRevived = false;
        }

        public override void OnGameEnter()
        {
            playing = true;
            LoadStage(1);
            stageLevelAnimator.PlayAnim();
            score = 0;
            GameScoreManager.Instance.HideScore();
            isInitialLaunch = true;
            hasRevived = false;
        }

        [Button]
        public override void SetupPet(bool isPlayingWithPet, PetObject petObject = null)
        {
            playerPlaceHolder.SetActive(!isPlayingWithPet);
            playerRenderer.gameObject.SetActive(isPlayingWithPet);

            if (isPlayingWithPet)
            {
                playerRenderer.sprites = petObject.GetShipAnim();
                playerRenderer.GetComponent<Image>().sprite = playerRenderer.sprites[0];

                playerRenderer.gameObject.transform.localRotation = petObject.spriteRenderer.transform.localRotation;

                if (CustomPetPos.ContainsKey(petObject.GetType()))
                    playerRenderer.gameObject.transform.localPosition = CustomPetPos[petObject.GetType()];
                else
                    playerRenderer.gameObject.transform.localPosition =
                        petObject.spriteRenderer.transform.localPosition;

                playerRenderer.gameObject.transform.localScale = petObject.spriteRenderer.transform.localScale;

                playerRenderer.interval = 0.9f / playerRenderer.sprites.Length;
            }
        }

        public override void RestartGame()
        {
            ResetGame();
        }

        private void InitializeRocket()
        {
            initialRocketRot = rocket.transform.rotation;
            initialRocketPos = rocket.transform.position;
            rocketRigidBody2D = rocket.GetComponent<Rigidbody2D>();
            rocket.SetTrigger("hide");
        }

        private void HandleResetAnimation()
        {
            if (resetAnimPlaying)
            {
                if (rocket.GetCurrentAnimatorClipInfo(0)[0].clip.name !=
                    "rocket_reset") return;
                Vector2 targetPosition = rocketDummy.transform.position;
                targetPosition.x -= (targetPosition.x - rocket.transform.position.x) / 10f;
                targetPosition.y -= (targetPosition.y - rocket.transform.position.y) / 10f;
                rocketDummy.transform.position = targetPosition;
                rocketDummy.transform.rotation = rocket.transform.rotation;
            }
        }

        private void HandleButtonEvents()
        {
            if (isButtonUpPressed & playing) rocketRigidBody2D.AddForce(rocketRigidBody2D.transform.up * 1.25f);

            if (isButtonLeftPressed & playing) rocketRigidBody2D.rotation += 2.5f * Time.deltaTime * DeltaTimeVelocity;

            if (isButtonRightPressed & playing) rocketRigidBody2D.rotation -= 2.5f * Time.deltaTime * DeltaTimeVelocity;
        }

        private void HandleGameStatus()
        {
            //success
            if (isLeftFootCollided & isRightFootCollided & playing)
            {
                rocketRigidBody2D.constraints = RigidbodyConstraints2D.FreezeAll;
                playing = false;
                OnSucceed();
            }

            //failed
            if (isBodyCollided & playing)
            {
                playing = false;
                rocketRigidBody2D.constraints = RigidbodyConstraints2D.FreezeAll;
                OnFailed();
            }
        }

        public void ChangeButtonState(string buttonSource, bool pressed)
        {
            if (pressed)
            {
                playerRenderer.ResumeAnim();
            }
            else
            {
                playerRenderer.PauseAnim();
                sfxController.KillAllSFX();
            }

            HandleButtonActin(buttonSource, pressed);
        }

        private void HandleButtonActin(string buttonSource, bool pressed)
        {
            switch (buttonSource)
            {
                case "left":
                    if (!playing) return;
                    particleFXManager.SetThrustLeftParticleEmission(pressed);
                    isButtonLeftPressed = pressed;
                    break;
                case "right":
                    if (!playing) return;
                    particleFXManager.SetThrustRightParticleEmission(pressed);
                    isButtonRightPressed = pressed;
                    break;
                case "up":
                    if (!playing) return;
                    isButtonUpPressed = pressed;
                    particleFXManager.SetThrustParticleEmission(pressed);
                    if (pressed) LaunchRocket();
                    break;
            }
        }

        private void LaunchRocket()
        {
            if (isInitialLaunch)
            {
                sfxController.PlaySFX(SfxType.LargeLaunch);
                screenShakeAnimator.enabled = false;
                screenShakeAnimator.gameObject.transform.DOPunchPosition(new Vector3(3f, 3f, 0f), 2.5f, 7)
                    .OnComplete(() =>
                    {
                        screenShakeAnimator.enabled = true;
                        uiManager.HideTutorial(2);
                    });
                isInitialLaunch = false;
            }
            else
            {
                sfxController.PlaySFX(SfxType.MiddleLaunch);
            }
        }

        public void ChangeColliderState(string colliderSource, bool collided)
        {
            switch (colliderSource)
            {
                case "left":
                    isLeftFootCollided = collided;
                    break;
                case "right":
                    isRightFootCollided = collided;
                    break;
                case "body":
                    isBodyCollided = collided;
                    break;
            }
        }

        private void OnSucceed()
        {
            playerRenderer.PauseAnim();
            rocketDummy.transform.position = rocket.transform.position;
            rocketDummy.transform.rotation = rocket.transform.rotation;
            rocketDummy.gameObject.SetActive(true);
            islandSizeController.OpenIsland();
            uiManager.successMessageText.DOFade(1, 1f)
                .OnComplete(ResetRocket);

            particleFXManager.PlaySucceedFx(rocket.transform);

            screenShakeAnimator.SetTrigger("up");
            stageLevelAnimator.NextLevel();
            AudioManager.Instance.PlaySfxByTag(SfxTag.RocketClear);
        }

        private void OnFailed()
        {
            playerRenderer.PauseAnim();
            particleFXManager.PlayFailedFx(rocket.transform, failPosition);
            FXManager.Instance.CreateFX(FXType.RocketHit, failPosition);
            bgAnimator.RemoveCircles();
            screenShakeAnimator.SetTrigger("large");

            if (hasRevived) ShowScore();
            else WatchAdsContinueGame.Instance.Init(Revive, ShowScore, "Land_Revibe");
        }

        private void ShowScore()
        {
            stageLevelAnimator.SetLevel(1);
            score = currentLevel;
            GameScoreManager.Instance.ShowScore(score, GameType.land);
            rocketCanvas.SetTrigger("ending");
            hasRevived = false;
        }

        private void Revive()
        {
            hasRevived = true;
            currentLevel -= 1;
            ResetRocket();
        }

        public void ResetRocket()
        {
            playerRenderer.PauseAnim();
            rocket.enabled = true;
            rocket.SetTrigger("reset");
            islandSizeController.CloseIsland();
            uiManager.DoFadeSuccessText(0);
            resetAnimPlaying = true;
            holdTransition = true;
            particleFXManager.SetThrustParticleEmission(false);
            sfxController.PlaySFX(SfxType.SmallLaunch);
        }

        public void OpenNextStage()
        {
            holdTransition = false;
            bgAnimator.RemoveCircles();
            LoadStage(currentLevel + 1);
        }

        public void RocketIsReady()
        {
            rocketRigidBody2D.constraints = RigidbodyConstraints2D.None;
            rocket.enabled = false;
            resetAnimPlaying = false;
            ResetToInitialState();
            if (stageLevelAnimator != null) stageLevelAnimator.PlayAnim();
        }

        private void ResetToInitialState()
        {
            playing = true;
            isButtonUpPressed = false;
            isButtonLeftPressed = false;
            isButtonRightPressed = false;
            isBodyCollided = false;
            isLeftFootCollided = false;
            isRightFootCollided = false;
            particleFXManager.SetThrustParticleEmission(false);
            isInitialLaunch = true;
        }

        private void LoadStage(int idx)
        {
            currentLevel = idx;
            bgAnimator.LoadCircles(stageManager.stages[idx - 1].items);
            stageLevelAnimator.SetLevel(idx);

            if (idx > 1)
            {
                uiManager.HideTutorial(1);
            }
            else
            {
                if (GameScoreManager.Instance.GetHighScore(GameType.land) <= 1 &&
                    GameScoreManager.Instance.GetHighScore(GameType.land) < 4) uiManager.ShowTutorial();
            }
        }

        public void ResetGame()
        {
            GameScoreManager.Instance.HideScore();
            rocketCanvas.SetTrigger("retry");
            bgAnimator.RemoveCircles();
            LoadStage(1);
            rocket.enabled = true;
            rocket.SetTrigger("begin");
            isInitialLaunch = true;
        }
    }
}