using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DynamicGames.Pet;
using DynamicGames.System;
using DynamicGames.UI;
using Sirenix.OdinInspector;
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
        [FormerlySerializedAs("aiEnumorators")] [FormerlySerializedAs("aiManager")] [SerializeField] private AIEnumerator aiEnumerator;
        [SerializeField] private Dictionary<PetType, Vector3> CustomPetPos;
        [SerializeField] private AIManager aiManager;

        
        [SerializeField] private Animator doorLeftAnimator; 
        [SerializeField] private Animator doorRightAnimator;
        [SerializeField] public Transform player, island;
        [SerializeField] private Transform startPosition, loadPosition;
        [FormerlySerializedAs("adj_transition_notch")] [SerializeField] private GameObject adjTransitionNotch;
        [FormerlySerializedAs("face")] [SerializeField] private Animator faceAnimator;
        [FormerlySerializedAs("hand")] [SerializeField] private TutorialAnimation tutorialAnimation;

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

        public int stage;

        private bool hasRevived;
        private float spinTime;
        private int stageFinished, currentStagePlaying = -1;

        private void Awake()
        {
            Instacne = this;
            gameObject.SetActive(false);
        }
        
        private void Start()
        {
            audioManager = AudioManager.Instance;

            ChangeStatus(ShootGameState.Ready);
            stage = 0;
            stageFinished = 0;
            SetDefaultAttackState();

            tutorialAnimation.gameObject.SetActive(false);
            tutorial.SetActive(false);
        }

        private void SetDefaultAttackState()
        {
            aiManager.
                SetDefaultState();
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
            stageFinished = 0;
            enemyManager.KillAll();
            itemManager.KillAll();
            bulletManager.Restart();
            stage = 0;
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
            if (Time.frameCount % 20 != 0) return;
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

        public enum FaceState
        {
            Idle, TurnRed, Angry01,
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

        public enum IslandState
        {
            Open,
            Close
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
        
        private async Task SetStage(int _stage)
        {
            if (state != ShootGameState.Playing) return;
            if (stage >= _stage) return;
            if (currentStagePlaying != -1) return;

            stage = stageFinished + 1;
            currentStagePlaying = stage;
            print("Stage : [ " + stage + " ] Started");

            int rnd;
            switch (stage)
            {
                case 0:
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(1000, 0, 0);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(1100, 0, 0);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(1200, 0, 0);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(1300, 0, 0);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInSpiral.Initialize(1400, 0, 0);
                    aiManager.CurrentAutoAttackInfo.CreateItem.Initialize(3500, 1, 1, 0.5f);
                    itemManager.SpawnItem();
                    break;
                case 1:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("turnRed");
                    await Task.Delay(2000);
                    rnd = Random.Range(0, 3);
                    switch (rnd)
                    {
                        case 0:
                            CreateMetheor();
                            await Task.Delay(2000);
                            break;
                        case 1:
                            enemyManager.SpawnEnemyAtRandomPos();
                            enemyManager.SpawnEnemyAtRandomPos();
                            enemyManager.SpawnEnemyAtRandomPos();
                            await Task.Delay(1000);
                            break;
                        case 2:
                            SpawnOnLeft(2);
                            SpawnOnRight(2);
                            await Task.Delay(3000);
                            break;
                    }

                    islandSizeController.CloseIsland();
                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(12000, 0, 1);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(3000, 0, 2);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(1000, 0, 0);
                    itemManager.SpawnItem();
                    break;
                case 2:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(2000);
                    CreateMetheor();
                    await Task.Delay(1000);

                    rnd = Random.Range(0, 3);
                    switch (rnd)
                    {
                        case 0:
                            CreateMetheor();
                            await Task.Delay(1000);
                            CreateMetheor();
                            await Task.Delay(1000);
                            islandSizeController.CloseIsland();
                            break;
                        case 1:
                            enemyManager.SpawnEnemyAtRandomPos();
                            enemyManager.SpawnEnemyAtRandomPos();
                            enemyManager.SpawnEnemyAtRandomPos();
                            enemyManager.SpawnEnemyAtRandomPos();
                            await Task.Delay(1000);
                            islandSizeController.CloseIsland();
                            break;
                        case 2:
                            SpawnOnLeft(4);
                            SpawnOnRight(4);
                            await Task.Delay(4000);
                            islandSizeController.CloseIsland();
                            break;
                    }

                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(10000, 0, 1);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(2900, 0, 3);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(1000, 0, 0);
                    break;
                case 3:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);

                    rnd = Random.Range(0, 3);
                    switch (rnd)
                    {
                        case 0:
                            CreateMetheor();
                            await Task.Delay(1000);
                            CreateMetheor();
                            await Task.Delay(3000);
                            CreateMetheor();
                            await Task.Delay(1000);
                            islandSizeController.CloseIsland();
                            break;
                        case 1:
                            enemyManager.SpawnEnemyAtRandomPos();
                            enemyManager.SpawnEnemyAtRandomPos();
                            enemyManager.SpawnEnemyAtRandomPos();
                            enemyManager.SpawnEnemyAtRandomPos();
                            enemyManager.SpawnEnemyAtRandomPos();
                            await Task.Delay(1000);
                            islandSizeController.CloseIsland();
                            break;
                        case 2:
                            SpawnOnLeft(5);
                            SpawnOnRight(5);
                            await Task.Delay(4000);
                            islandSizeController.CloseIsland();
                            break;
                    }

                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(10000, 0, 4);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(2900, 0, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(9000, 0, 1);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(7500, 3, 5, 0.3f);
                    break;
                case 4:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);
                    enemyManager.SpawnEnemyInLineY(6);

                    rnd = Random.Range(0, 4);
                    switch (rnd)
                    {
                        case 0:
                            CreateMetheor();
                            await Task.Delay(1000);
                            CreateMetheor();
                            await Task.Delay(1000);
                            CreateMetheor();
                            await Task.Delay(1000);
                            CreateMetheor();
                            await Task.Delay(1000);
                            islandSizeController.CloseIsland();
                            break;
                        case 1:
                            enemyManager.SpawnEnemyInCircle(1f, Random.Range(5, 8));
                            await Task.Delay(1000);
                            islandSizeController.CloseIsland();
                            break;
                        case 2:
                            SpawnOnLeft(5);
                            SpawnOnRight(5);
                            await Task.Delay(4000);
                            islandSizeController.CloseIsland();
                            break;
                        case 3:
                            await enemyManager.SpawnEnemyInSpiral(0.5f, 1.6f, 20, 1.3f, 30, 0.75f);
                            break;
                    }

                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(10000, 2, 6);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(2900, 0, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(9000, 0, 1);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(7500, 3, 8, 0.3f);
                    break;
                case 5:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);
                    enemyManager.SpawnEnemyInLineY(8);

                    rnd = Random.Range(0, 4);
                    switch (rnd)
                    {
                        case 0:
                            CreateMetheor();
                            await Task.Delay(500);
                            CreateMetheor();
                            await Task.Delay(500);
                            CreateMetheor();
                            await Task.Delay(500);
                            islandSizeController.CloseIsland();
                            break;
                        case 1:
                            enemyManager.SpawnEnemyInCircle(1f, Random.Range(5, 12));
                            await Task.Delay(1000);
                            islandSizeController.CloseIsland();
                            break;
                        case 2:
                            SpawnOnLeft(5);
                            SpawnOnRight(5);
                            await Task.Delay(4000);
                            islandSizeController.CloseIsland();
                            break;
                        case 3:
                            await enemyManager.SpawnEnemyInSpiral(0.5f, 1.7f, 25, 1.35f, 30);
                            break;
                    }

                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(10000, 2, 6);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(2900, 1, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(9000, 0, 2);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(7500, 3, 8, 0.4f);
                    break;
                case 6:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);
                    enemyManager.SpawnEnemyInLineY(10);

                    rnd = Random.Range(0, 4);
                    switch (rnd)
                    {
                        case 0:
                            CreateMetheor();
                            await Task.Delay(500);
                            CreateMetheor();
                            await Task.Delay(500);
                            CreateMetheor();
                            await Task.Delay(2000);
                            CreateMetheor();
                            await Task.Delay(500);
                            islandSizeController.CloseIsland();
                            break;
                        case 1:
                            enemyManager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                            await Task.Delay(300);
                            enemyManager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                            await Task.Delay(1000);
                            islandSizeController.CloseIsland();
                            break;
                        case 2:
                            SpawnOnLeft(5);
                            SpawnOnRight(5);
                            await Task.Delay(4000);
                            islandSizeController.CloseIsland();
                            break;
                        case 3:
                            await enemyManager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30);
                            break;
                    }

                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(10000, 3, 12);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(2900, 1, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(8000, 0, 2);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(7500, 5, 10, 0.45f);
                    break;
                case 7:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);

                    for (var i = 0; i < 2; i++)
                    {
                        rnd = Random.Range(0, 5);
                        switch (rnd)
                        {
                            case 0:
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(500);
                                break;
                            case 1:
                                enemyManager.SpawnEnemyInCircle(1f, Random.Range(8, 15));
                                await Task.Delay(3000);
                                break;
                            case 2:
                                SpawnOnLeft(5);
                                SpawnOnRight(5);
                                await Task.Delay(4000);
                                break;
                            case 3:
                                await enemyManager.SpawnEnemyInSpiral(0.5f, 1.7f, 20, 1.5f, 30);
                                await Task.Delay(1500);
                                break;
                            case 4:
                                await Task.Delay(3000);
                                enemyManager.SpawnEnemyInLineY(7, 0.7f);
                                await Task.Delay(1500);
                                enemyManager.SpawnEnemyInLineY(7);
                                await Task.Delay(5000);
                                break;
                        }
                    }

                    islandSizeController.CloseIsland();
                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(10000, 3, 12);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(2900, 1, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(4900, 1, 1, 0.5f);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(7500, 5, 10, 0.45f);
                    break;
                case 8:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);

                    for (var i = 0; i < 2; i++)
                    {
                        rnd = Random.Range(0, 5);
                        switch (rnd)
                        {
                            case 0:
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(500);
                                break;
                            case 1:
                                enemyManager.SpawnEnemyInCircle(1f, Random.Range(8, 15));
                                await Task.Delay(2500);
                                break;
                            case 2:
                                SpawnOnLeft(5);
                                SpawnOnRight(5);
                                await Task.Delay(3000);
                                break;
                            case 3:
                                await enemyManager.SpawnEnemyInSpiral(0.5f, 1.7f, 22, 1.5f, 30);
                                await Task.Delay(1500);
                                break;
                            case 4:
                                await Task.Delay(3000);
                                enemyManager.SpawnEnemyInLineY(7, 0.7f);
                                await Task.Delay(1500);
                                enemyManager.SpawnEnemyInLineY(7);
                                await Task.Delay(4000);
                                break;
                        }
                    }

                    islandSizeController.CloseIsland();
                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(10000, 3, 12);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(2400, 1, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(4900, 1, 1, 0.5f);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(6500, 5, 10, 0.45f);
                    break;
                case 9:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(500);
                    CreateMetheor();
                    await Task.Delay(500);

                    for (var i = 0; i < 2; i++)
                    {
                        rnd = Random.Range(0, 5);
                        switch (rnd)
                        {
                            case 0:
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(500);
                                break;
                            case 1:
                                enemyManager.SpawnEnemyInCircle(1f, Random.Range(15, 30));
                                await Task.Delay(3200);
                                break;
                            case 2:
                                SpawnOnLeft(5);
                                SpawnOnRight(5);
                                await Task.Delay(2500);
                                break;
                            case 3:
                                await enemyManager.SpawnEnemyInSpiral(0.5f, 1.7f, 25, 1.5f, 30);
                                await Task.Delay(2500);
                                break;
                            case 4:
                                await Task.Delay(3000);
                                enemyManager.SpawnEnemyInLineY(7, 0.7f);
                                await Task.Delay(1500);
                                enemyManager.SpawnEnemyInLineY(7);
                                await Task.Delay(3000);
                                break;
                        }
                    }

                    islandSizeController.CloseIsland();
                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(10000, 3, 12);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(2400, 1, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(4900, 1, 1, 0.5f);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(7500, 5, 10, 0.45f);
                    break;
                case 10:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);
                    enemyManager.SpawnEnemyInLineY(10);

                    for (var i = 0; i < 2; i++)
                    {
                        rnd = Random.Range(0, 5);
                        switch (rnd)
                        {
                            case 0:
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(1000);
                                CreateMetheor();
                                await Task.Delay(500);
                                break;
                            case 1:
                                enemyManager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                                await Task.Delay(300);
                                enemyManager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                                await Task.Delay(1000);
                                break;
                            case 2:
                                SpawnOnLeft(5);
                                SpawnOnRight(5);
                                await Task.Delay(4000);
                                break;
                            case 3:
                                await enemyManager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30);
                                break;
                            case 4:
                                await Task.Delay(3000);
                                enemyManager.SpawnEnemyInLineY(10, 0.7f);
                                await Task.Delay(1500);
                                enemyManager.SpawnEnemyInLineY(10);
                                break;
                        }
                    }

                    islandSizeController.CloseIsland();
                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(10000, 3, 12);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(2900, 1, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(4900, 1, 1, 0.5f);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(7500, 5, 10, 0.45f);
                    break;
                case 11:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);
                    enemyManager.SpawnEnemyInLineY(15);

                    for (var i = 0; i < 3; i++)
                    {
                        rnd = Random.Range(0, 5);
                        switch (rnd)
                        {
                            case 0:
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(1000);
                                CreateMetheor();
                                await Task.Delay(500);
                                break;
                            case 1:
                                enemyManager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                                await Task.Delay(300);
                                enemyManager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                                await Task.Delay(1000);
                                break;
                            case 2:
                                SpawnOnLeft(5);
                                SpawnOnRight(5);
                                await Task.Delay(4000);
                                break;
                            case 3:
                                await enemyManager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30);
                                break;
                            case 4:
                                await Task.Delay(3000);
                                enemyManager.SpawnEnemyInLineY(10, 0.7f);
                                await Task.Delay(1500);
                                enemyManager.SpawnEnemyInLineY(10);
                                break;
                        }
                    }

                    islandSizeController.CloseIsland();
                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(8000, 3, 15);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(1400, 1, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(3900, 1, 1, 0.5f);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(6900, 5, 10, 0.45f);
                    break;
                case 12:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);
                    enemyManager.SpawnEnemyInLineY(15);

                    for (var i = 0; i < 3; i++)
                    {
                        rnd = Random.Range(0, 5);
                        switch (rnd)
                        {
                            case 0:
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(1000);
                                CreateMetheor();
                                await Task.Delay(500);
                                break;
                            case 1:
                                enemyManager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                                await Task.Delay(300);
                                enemyManager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                                await Task.Delay(1000);
                                break;
                            case 2:
                                SpawnOnLeft(5);
                                SpawnOnRight(5);
                                await Task.Delay(4000);
                                break;
                            case 3:
                                await enemyManager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30);
                                break;
                            case 4:
                                await Task.Delay(3000);
                                enemyManager.SpawnEnemyInLineY(10, 0.7f);
                                await Task.Delay(1500);
                                enemyManager.SpawnEnemyInLineY(10);
                                break;
                        }
                    }

                    islandSizeController.CloseIsland();
                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(8000, 7, 15);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(1400, 1, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(2900, 1, 1, 0.5f);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(6900, 5, 10, 0.45f);
                    break;
                case 13:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);
                    enemyManager.SpawnEnemyInLineY(15);

                    for (var i = 0; i < 4; i++)
                    {
                        rnd = Random.Range(0, 5);
                        switch (rnd)
                        {
                            case 0:
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(1000);
                                CreateMetheor();
                                await Task.Delay(500);
                                break;
                            case 1:
                                enemyManager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                                await Task.Delay(300);
                                enemyManager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                                await Task.Delay(1000);
                                break;
                            case 2:
                                SpawnOnLeft(5);
                                SpawnOnRight(5);
                                await Task.Delay(4000);
                                break;
                            case 3:
                                await enemyManager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30);
                                break;
                            case 4:
                                await Task.Delay(3000);
                                enemyManager.SpawnEnemyInLineY(10, 0.7f);
                                await Task.Delay(1500);
                                enemyManager.SpawnEnemyInLineY(10);
                                break;
                        }
                    }

                    islandSizeController.CloseIsland();
                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(7000, 7, 15);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(900, 1, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(2900, 1, 1, 0.5f);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(6900, 5, 10, 0.45f);
                    break;
                case 14:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);
                    enemyManager.SpawnEnemyInLineY(15);

                    for (var i = 0; i < 4; i++)
                    {
                        rnd = Random.Range(0, 5);
                        switch (rnd)
                        {
                            case 0:
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(1000);
                                CreateMetheor();
                                await Task.Delay(500);
                                break;
                            case 1:
                                enemyManager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                                await Task.Delay(300);
                                enemyManager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                                await Task.Delay(1000);
                                break;
                            case 2:
                                SpawnOnLeft(5);
                                SpawnOnRight(5);
                                await Task.Delay(4000);
                                break;
                            case 3:
                                await enemyManager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30);
                                break;
                            case 4:
                                await Task.Delay(3000);
                                enemyManager.SpawnEnemyInLineY(10, 0.7f);
                                await Task.Delay(1500);
                                enemyManager.SpawnEnemyInLineY(10);
                                break;
                        }
                    }

                    islandSizeController.CloseIsland();
                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(4100, 7, 15);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(800, 1, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(1100, 1, 1, 0.3f);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(4900, 5, 10, 0.45f);
                    break;
                case 15:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);
                    enemyManager.SpawnEnemyInLineY(15);

                    for (var i = 0; i < 5; i++)
                    {
                        rnd = Random.Range(0, 5);
                        switch (rnd)
                        {
                            case 0:
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(1000);
                                CreateMetheor();
                                await Task.Delay(500);
                                break;
                            case 1:
                                enemyManager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                                await Task.Delay(300);
                                enemyManager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                                await Task.Delay(1000);
                                break;
                            case 2:
                                SpawnOnLeft(5);
                                SpawnOnRight(5);
                                await Task.Delay(4000);
                                break;
                            case 3:
                                await enemyManager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30);
                                break;
                            case 4:
                                await Task.Delay(3000);
                                enemyManager.SpawnEnemyInLineY(10, 0.7f);
                                await Task.Delay(1500);
                                enemyManager.SpawnEnemyInLineY(10);
                                break;
                        }
                    }

                    islandSizeController.CloseIsland();
                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(4100, 7, 15);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(500, 1, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(1100, 1, 1, 0.5f);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(4900, 5, 10, 0.45f);
                    break;
                case 16:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);
                    enemyManager.SpawnEnemyInLineY(15);

                    for (var i = 0; i < 5; i++)
                    {
                        rnd = Random.Range(0, 5);
                        switch (rnd)
                        {
                            case 0:
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(1000);
                                CreateMetheor();
                                await Task.Delay(500);
                                break;
                            case 1:
                                enemyManager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                                await Task.Delay(300);
                                enemyManager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                                await Task.Delay(1000);
                                break;
                            case 2:
                                SpawnOnLeft(5);
                                SpawnOnRight(5);
                                await Task.Delay(4000);
                                break;
                            case 3:
                                await enemyManager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30);
                                break;
                            case 4:
                                await Task.Delay(3000);
                                enemyManager.SpawnEnemyInLineY(10, 0.7f);
                                await Task.Delay(1500);
                                enemyManager.SpawnEnemyInLineY(10);
                                break;
                        }
                    }

                    islandSizeController.CloseIsland();
                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(2100, 7, 15);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(500, 1, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(700, 1, 1, 0.5f);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(3900, 5, 10, 0.45f);
                    break;
                case 17:
                    islandSizeController.OpenIsland();
                    await Task.Delay(1000);
                    faceAnimator.SetTrigger("angry01");
                    await Task.Delay(1000);
                    CreateMetheor();
                    await Task.Delay(1000);
                    enemyManager.SpawnEnemyInLineY(15);

                    for (var i = 0; i < 5; i++)
                    {
                        rnd = Random.Range(0, 5);
                        switch (rnd)
                        {
                            case 0:
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(500);
                                CreateMetheor();
                                await Task.Delay(500);
                                break;
                            case 1:
                                enemyManager.SpawnEnemyInCircle(0.8f, Random.Range(5, 10));
                                await Task.Delay(300);
                                enemyManager.SpawnEnemyInCircle(1.2f, Random.Range(10, 20));
                                await Task.Delay(1000);
                                break;
                            case 2:
                                SpawnOnLeft(8);
                                SpawnOnRight(8);
                                await Task.Delay(4000);
                                break;
                            case 3:
                                await enemyManager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 10);
                                break;
                            case 4:
                                await Task.Delay(3000);
                                enemyManager.SpawnEnemyInLineY(15, 0.7f);
                                await Task.Delay(1500);
                                enemyManager.SpawnEnemyInLineY(15);
                                break;
                        }
                    }

                    islandSizeController.CloseIsland();
                    await Task.Delay(1000);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle.Initialize(1100, 7, 15);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos.Initialize(300, 1, 4);
                    aiManager.CurrentAutoAttackInfo.CreateMeteor.Initialize(500, 1, 1, 0.5f);
                    aiManager.CurrentAutoAttackInfo.CreateEnemyInLine.Initialize(2900, 5, 10, 0.45f);
                    break;
            }

            print("Stage : [ " + stage + " ] Finished");
            await Task.Delay(1000);
            stageFinished = stage;
            currentStagePlaying = -1;
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
                    stageFinished = 0;
                    currentStagePlaying = -1;
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
                    aiEnumerator.StartTasks();
                    bulletManager.StartSpawnBulletTimer();
                    stageFinished = 0;
                    currentStagePlaying = -1;
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
            currentStagePlaying = -1;
            stage -= 1;
            SetStage(stage + 1);
            inputManager.gameObject.SetActive(true);
            aiEnumerator.StartTasks();
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