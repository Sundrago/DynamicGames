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
        
        [Header("Managers and Controllers")]
        [SerializeField] public EnemyManager enemyManager;
        [SerializeField] private BulletManager bulletManager;
        [SerializeField] private FXManager fXManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] public ItemManager itemManager;
        [SerializeField] private IslandSizeController islandSizeController;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private SfxController sfxController;
        [SerializeField] private PetManager petManager;
        [FormerlySerializedAs("aiEnumerator")] [SerializeField] private AIRoutineEnumerator aiRoutineEnumerator;
        [SerializeField] private Dictionary<PetType, Vector3> CustomPetPos;
        [SerializeField] private AIManager aiManager;

        
        [SerializeField] private Animator doorLeftAnimator; 
        [SerializeField] private Animator doorRightAnimator;
        [SerializeField] public Transform player, island;
        [SerializeField] private Transform startPosition, loadPosition;
        [SerializeField] private GameObject adjTransitionNotch;
        [SerializeField] private Animator faceAnimator;
        [SerializeField] private TutorialAnimation tutorialAnimation;

        [SerializeField] public ItemInformationUI itemInformationUIAtk;
        [SerializeField] public ItemInformationUI itemInformationUIShield;
        [SerializeField] public ItemInformationUI itemInformationUIBounce;
        [SerializeField] public ItemInformationUI itemInformationUISpin;
        [SerializeField] private GameObject tutorial;
        [SerializeField] private SpriteAnimator playerRenderer;
        [SerializeField] private GameObject playerPlaceHolder;
        
        [NonSerialized] public FXController shield;
        private AudioManager audioManager;

        public ShootGameState state;
        public bool spinMode;

        public int currentStageIdx;

        private bool hasRevived;
        private float spinTime;

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

            tutorialAnimation.gameObject.SetActive(false);
            tutorial.SetActive(false);
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
            DestroyShield();

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
        
        public void SetFaceAnimation(FaceState state)
        {
            switch (state)
            {
                case FaceState.Idle:
                    faceAnimator.SetTrigger("idle");
                    break;
                case FaceState.TurnRed:
                    faceAnimator.SetTrigger("turnRed");
                    break;
                case FaceState.Angry01:
                    faceAnimator.SetTrigger("angry01");
                    break;
            }
        }
        
        public void SetIslandAnimation(IslandState state)
        {
            switch (state)
            {
                case IslandState.Open:
                    islandSizeController.OpenIsland();
                    break;
                case IslandState.Close:
                    islandSizeController.CloseIsland();
                    break;
            }
        }
        
        private void SetStage(int stage)
        {
            if (state != ShootGameState.Playing) return;
            if (this.currentStageIdx > stage) return;
            if (aiManager.IsRunningTasks) return;
            
            aiManager.StartStage(currentStageIdx);
            currentStageIdx += 1;
        }

        public async Task SpawnOnLeft(int count)
        {
            if (state != ShootGameState.Playing) return;
            doorLeftAnimator.SetTrigger("open");
            await Task.Delay(400);
            for (var i = 0; i < count; i++)
            {
                enemyManager.SpawnOnIsland(180, -1.5f, 0f);
                await Task.Delay(1000);
            }

            doorLeftAnimator.SetTrigger("close");
        }

        public async Task SpawnOnRight(int count)
        {
            if (state != ShootGameState.Playing) return;
            doorRightAnimator.SetTrigger("open");
            await Task.Delay(400);
            for (var i = 0; i < count; i++)
            {
                enemyManager.SpawnOnIsland(0, 1.5f, 0f);
                await Task.Delay(1000);
            }

            doorRightAnimator.SetTrigger("close");
        }

        public void CreateMetheor()
        {
            if (state != ShootGameState.Playing) return;
            var path = new Vector3[3];
            path[0] = island.transform.position;
            path[2] = player.transform.position;
            path[0].z = path[2].z;

            var ydiff = Mathf.Abs(player.transform.position.y - island.transform.position.y);
            var xdiff = map(ydiff, 0, 5, 0.2f, 1.5f);
            path[1] = Vector3.Lerp(path[0], path[2], 0.5f);
            path[1].x = player.position.x < 0 ? -xdiff : xdiff;

            var metheor = fXManager.CreateFX(FXType.ShadowMissile, path[0]);
            metheor.transform.DOPath(path, Random.Range(1.8f, 2.2f), PathType.CatmullRom, PathMode.TopDown2D, 1,
                    Color.red)
                .SetEase(Ease.OutQuart)
                .OnComplete(() =>
                {
                    //fXManager.KillFX(metheor.GetComponent<FX>());
                    fXManager.CreateFX(FXType.ShadowBomb, metheor.transform);
                });

            float map(float s, float a1, float a2, float b1, float b2)
            {
                return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
            }
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
                        tutorialAnimation.Show();
                        tutorial.SetActive(true);
                    }
                    else
                    {
                        tutorialAnimation.gameObject.SetActive(false);
                        tutorial.SetActive(false);
                    }

                    hasRevived = false;
                    break;
                case ShootGameState.Playing:
                    aiRoutineEnumerator.StartTasks();
                    bulletManager.StartSpawnBulletTimer();
                    if (tutorialAnimation.gameObject.activeSelf)
                    {
                        tutorialAnimation.Hide();
                        tutorial.SetActive(false);
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
            itemInformationUIAtk.HideUI();
            itemInformationUIShield.HideUI();
            itemInformationUIBounce.HideUI();
            itemInformationUISpin.HideUI();
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


        public void GetShield()
        {
            if (shield != null) return;
            shield = fXManager.CreateFX(FXType.Shield, player.transform).GetComponent<FXController>();
            shield.gameObject.transform.SetParent(player.transform, true);
            shield.transform.localPosition = Vector3.zero;
        }

        private void DestroyShield()
        {
            if (shield == null) return;
            itemInformationUIShield.HideUI();
            fXManager.CreateFX(FXType.ShieldPop, shield.gameObject.transform);
            fXManager.CreateFX(FXType.Bomb, shield.gameObject.transform);
            fXManager.KillFX(shield);
            audioManager.PlaySfxByTag(SfxTag.ShieldPop);
            shield = null;
        }

        public void GetAttack()
        {
            if (state != ShootGameState.Playing) return;

            if (shield != null)
            {
                DestroyShield();
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
            tutorialAnimation.gameObject.SetActive(false);
            tutorial.gameObject.SetActive(false);
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