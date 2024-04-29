using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DynamicGames.Pet;
using DynamicGames.System;
using DynamicGames.UI;
using Sirenix.OdinInspector;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    ///     Manages the gameplay and state of a shooting mini-game.
    /// </summary>
    public class GameManager : MiniGameManager, IMiniGame
    {
        public enum ShootGameState
        {
            Ready,
            Dead,
            Playing,
            Revive
        }

        public static GameManager Instacne { get; private set;  }

        public AIDoorController AIDoorController => aiDoorController;
        public UIManager UIManager => uiManager;
        public UIManager UIManager1 => uiManager;
        public Animator FaceAnimator
        {
            set => faceAnimator = value;
            get => faceAnimator;
        }

        public AIDoorController AIDoorController1 => aiDoorController;

        public IslandSizeController IslandSizeController
        {
            set => islandSizeController = value;
            get => islandSizeController;
        }

        public ItemHandler ItemHandler => itemHandler;

        [Header("Managers and Controllers")]
        [SerializeField] public EnemyManager enemyManager;
        [SerializeField] private BulletManager bulletManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] public ItemManager itemManager;
        [SerializeField] private IslandSizeController islandSizeController;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private SfxController sfxController;
        [SerializeField] private PetManager petManager;
        [SerializeField] private AIRoutineEnumerator aiRoutineEnumerator;
        [SerializeField] private Dictionary<PetType, Vector3> CustomPetPos;
        [SerializeField] private AIManager aiManager;
        [SerializeField] public AudioManager audioManager;
        
        [Header("Game Components")]

        [SerializeField] private Animator faceAnimator;
        [SerializeField] public Transform player, island;
        [SerializeField] private Transform startPosition, loadPosition;
        [SerializeField] private GameObject adjTransitionNotch;
        [SerializeField] private SpriteAnimator playerRenderer;
        [SerializeField] private GameObject playerPlaceHolder;
        [NonSerialized] public FXController shield;

        private readonly AIDoorController aiDoorController;
        private readonly UIManager uiManager;
        private readonly ItemHandler itemHandler;
        public ShootGameState state;
        public bool spinMode;
        public int currentStageIdx;
        private bool hasRevived;
        private float spinTime;

        public GameManager()
        {
            aiDoorController = new AIDoorController(this);
            uiManager = new UIManager();
            itemHandler = new ItemHandler(this);
        }

        private void Awake()
        {
            Instacne = this;
            gameObject.SetActive(false);
        }
        
        private void Start()
        {
            audioManager = AudioManager.Instance;

            ChangeStatus(ShootGameState.Ready);
            currentStageIdx = 0;
            SetDefaultAttackState();

            UIManager.TutorialAnimation.gameObject.SetActive(false);
            UIManager.Tutorial.SetActive(false);
        }

        private void SetDefaultAttackState()
        {
            aiManager.SetDefaultState();
        }
        
        private void Update()
        {
            switch (state)
            {
                case ShootGameState.Ready:
                    Update_ready();
                    break;
                case ShootGameState.Playing:
                    Update_Playing();
                    break;
                case ShootGameState.Dead:
                    break;
            }
        }

        public override void RestartGame()
        {
            GameScoreManager.Instance.HideScore();
            ItemHandler.DestroyShield();

            faceAnimator.SetTrigger("idle");
            islandSizeController.CloseIsland();
            scoreManager.ResetScore();

            player.DOMove(startPosition.position, 0.5f)
                .SetEase(Ease.InOutCubic);
            player.DOLocalRotate(startPosition.localEulerAngles, 0.5f)
                .SetEase(Ease.InOutCubic)
                .OnComplete(() =>
                {
                    ChangeStatus(ShootGameState.Ready);
                    inputManager.gameObject.SetActive(true);
                });
            enemyManager.KillAll();
            itemManager.KillAll();
            bulletManager.Restart();
            currentStageIdx = 0;
            spinMode = false;
        }

        public override void ClearGame()
        {
            GameScoreManager.Instance.HideScore();
            state = ShootGameState.Dead;
            inputManager.ResetJoystick();
            gameObject.SetActive(false);
        }
        
        public override void SetupPet(bool isPlayingWithPet, PetObject petObject = null)
        {
            playerPlaceHolder.SetActive(!isPlayingWithPet);
            playerRenderer.gameObject.SetActive(isPlayingWithPet);

            if (isPlayingWithPet)
            {
                playerRenderer.sprites = petObject.GetShipAnim();
                playerRenderer.GetComponent<SpriteRenderer>().sprite = playerRenderer.sprites[0];

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

        public override void OnGameEnter()
        {
        }

        private void Update_ready()
        {
            if (inputManager.NormalVector != Vector3.zero) ChangeStatus(ShootGameState.Playing);
        }

        private void Update_Playing()
        {
            if (Time.frameCount % 30 != 0) return;
            if (spinMode && Time.time > spinTime) spinMode = false;
            
            var myScofre = scoreManager.GetScore();

            if (myScofre < 5) SetStage(0);
            else if (myScofre < 30) SetStage(1);
            else if (myScofre < 80) SetStage(2);
            else if (myScofre < 200) SetStage(3);
            else if (myScofre < 250) SetStage(4);
            else if (myScofre < 400) SetStage(5);
            else if (myScofre < 650) SetStage(6);
            else if (myScofre < 900) SetStage(7);
            else if (myScofre < 1200) SetStage(8);
            else if (myScofre < 1600) SetStage(9);
            else if (myScofre < 2000) SetStage(10);
            else if (myScofre < 2500) SetStage(11);
            else if (myScofre < 3000) SetStage(12);
            else if (myScofre < 3500) SetStage(13);
            else if (myScofre < 4000) SetStage(14);
            else if (myScofre < 5000) SetStage(15);
            else if (myScofre < 6000) SetStage(16);
            else SetStage(17);
        }

        private void SetStage(int stage)
        {
            if (state != ShootGameState.Playing) return;
            if (this.currentStageIdx > stage) return;
            if (aiManager.IsRunningTasks) return;
            
            aiManager.StartStage(currentStageIdx);
            currentStageIdx += 1;
        }

        private void ChangeStatus(ShootGameState _state)
        {
            if (state == _state) return;

            state = _state;
            switch (state)
            {
                case ShootGameState.Ready:
                    inputManager.ResetJoystick();
                    if (GameScoreManager.Instance.GetHighScore(GameType.shoot) < 200)
                    {
                        UIManager.TutorialAnimation.Show();
                        UIManager.Tutorial.SetActive(true);
                    }
                    else
                    {
                        UIManager.TutorialAnimation.gameObject.SetActive(false);
                        UIManager.Tutorial.SetActive(false);
                    }

                    hasRevived = false;
                    break;
                case ShootGameState.Playing:
                    aiRoutineEnumerator.StartTasks();
                    bulletManager.StartSpawnBulletTimer();
                    if (UIManager.TutorialAnimation.gameObject.activeSelf)
                    {
                        UIManager.TutorialAnimation.Hide();
                        UIManager.Tutorial.SetActive(false);
                    }

                    break;
                case ShootGameState.Dead:
                    FXManager.Instance.CreateFX(FXType.DeadExplosion, player);
                    enemyManager.GameOver();
                    faceAnimator.SetTrigger("idle");
                    islandSizeController.CloseIsland();
                    inputManager.gameObject.SetActive(false);
                    inputManager.ResetJoystick();
                    aiManager.cancelRequest = true;
                    if (!hasRevived) WatchAdsContinueGame.Instance.Init(Revive, ShowScore, "Shoot_Revive");
                    else ShowScore();
                    break;
            }
        }

        private void ShowScore()
        {
            GameScoreManager.Instance.ShowScore(scoreManager.GetScore(), GameType.shoot);
            UIManager.itemInformationUIAtk.HideUI();
            UIManager.itemInformationUIShield.HideUI();
            UIManager.itemInformationUIBounce.HideUI();
            UIManager.itemInformationUISpin.HideUI();
        }

        private void Revive()
        {
            enemyManager.KillAll();
            faceAnimator.SetTrigger("turnRed");
            state = ShootGameState.Playing;
            currentStageIdx -= 1;
            SetStage(currentStageIdx + 1);
            inputManager.gameObject.SetActive(true);
            aiRoutineEnumerator.StartTasks();
            bulletManager.StartSpawnBulletTimer();
            hasRevived = true;
        }


        public void GetAttack()
        {
            if (state != ShootGameState.Playing) return;

            if (shield != null)
            {
                ItemHandler.DestroyShield();
                return;
            }
            
            ChangeStatus(ShootGameState.Dead);
        }

        public void SetSpinMode(float duration)
        {
            spinMode = true;
            spinTime = Time.time + duration;
        }

        public void PreLoad()
        {
            adjTransitionNotch.SetActive(false);
            GameScoreManager.Instance.gameObject.SetActive(false);
            RestartGame();
            state = ShootGameState.Dead;
            DOTween.Kill(player.transform);
            player.transform.position = loadPosition.position;
            inputManager.gameObject.SetActive(false);
            Greetings();
            SetDefaultAttackState();
            UIManager.TutorialAnimation.gameObject.SetActive(false);
            UIManager.Tutorial.gameObject.SetActive(false);
        }
        
        private async Task Greetings()
        {
            DOTween.Kill(islandSizeController.GetComponent<RectTransform>());
            islandSizeController.OpenIsland();
            faceAnimator.SetTrigger("idle");
            await Task.Delay(1000);
            islandSizeController.CloseIsland();
        }

        public void ReadyToPlay()
        {
            player.DOMove(startPosition.position, 2f)
                .SetEase(Ease.OutQuart);
            player.DOLocalRotate(startPosition.localEulerAngles, 2f)
                .SetEase(Ease.OutQuart)
                .OnComplete(() =>
                {
                    ChangeStatus(ShootGameState.Ready);
                    inputManager.gameObject.SetActive(true);
                });
        }
    }
}