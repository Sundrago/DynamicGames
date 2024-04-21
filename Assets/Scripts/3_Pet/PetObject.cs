using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DynamicGames.MiniGames;
using DynamicGames.System;
using DynamicGames.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace DynamicGames.Pet
{
    /// <summary>
    ///     manage the behavior and properties of a pet object in the game.
    /// </summary>
    [RequireComponent(typeof(PetSurfaceMovement2D))]
    public class PetObject : SerializedMonoBehaviour
    {
        [SerializeField] public PetType type;
        [SerializeField] public SpriteRenderer spriteRenderer; 
        [SerializeField] public PetSurfaceMovement2D petSurfaceMovement2D;
        [SerializeField] public Transform centerPoint;
        [SerializeField] private float defaultInterval = 0.2f;
        [SerializeField] private PetInfoPanelManager petInformation;
        
        [TitleGroup("CallBacks", alignment: TitleAlignments.Centered)] [SerializeField] [BoxGroup("CallBacks/IDLE")]
        private float idleTimeMin = 5;

        [SerializeField] [BoxGroup("CallBacks/IDLE")]
        private float idleTimeMax = 7;

        [SerializeField] [TableList] [BoxGroup("CallBacks/IDLE")]
        private List<ActionOption> onIdleAction = new() { new ActionOption("idle") };

        [SerializeField] [BoxGroup("CallBacks/MOVE")]
        private float moveTimeMin = 5;

        [SerializeField] [BoxGroup("CallBacks/MOVE")]
        private float moveTimeMax = 7;

        [SerializeField] [TableList] [BoxGroup("CallBacks/MOVE")]
        private List<ActionOption> onMoveAction = new() { new ActionOption("walk") };

        [SerializeField] [TableList] [BoxGroup("CallBacks/HIT")]
        private List<ActionOption> onHitAction = new() { new ActionOption("walk") };

        [SerializeField] [BoxGroup("CallBacks/JUMP")]
        private bool hasExitAnimation;

        [SerializeField] [TableList] [BoxGroup("CallBacks/JUMP")]
        private List<ActionOption> onJumpStartAction = new() { new ActionOption("jump") };

        [SerializeField] [TableList] [BoxGroup("CallBacks/JUMP")] [ShowIf("hasExitAnimation")]
        private List<ActionOption> onJumpEndAction = new() { new ActionOption("jump_end") };

        [SerializeField] [BoxGroup("CallBacks/JUMP")] [ShowIf("hasExitAnimation")]
        private float jumpFinishNormal = 0.5f;

        [SerializeField] private Sprite[] inGameJumpAnim, inGameInSpaceShip;
        public bool ignoreIdleDialogue;

        [SerializeField] [TitleGroup("Actions", alignment: TitleAlignments.Centered)]
        private readonly Dictionary<string, PetMotion> petMotions = new();


        [SerializeField] [TitleGroup("Sprites", alignment: TitleAlignments.Centered)]
        private readonly Dictionary<string, List<Sprite>> sprites = new();

        private float animationInterval, stateTimer, animationTimer, jumpStartDuration, jumpEndDuration, hitDuration;
        private Dictionary<PetStatus, List<ActionOption>> availableActions = new();

        private int currentFrame;
        private string currentMotionId, previousMotionID;
        private Vector3 initialPosition;
        private bool isShowingDragDialogue;
        private PetDialogueController petDialogueController;
        private float previousStatusTimer, previousMotionTimer, jumpStartTime, mouseDownTime;
        private Dictionary<PetStatus, Action> stateHandlers;

        private PetStatus status, previousStatus;


        private void Awake()
        {
            InitializePet();
            InitDictionary();
        }

        private void OnEnable()
        {
            StartCoroutine(AnimationRoutine());
        }

        private void OnMouseDown()
        {
            isShowingDragDialogue = false;
            petSurfaceMovement2D.enabled = false;
            mouseDownTime = Time.time;
        }

        private void OnMouseDrag()
        {
            if (Time.time - mouseDownTime < 0.15f) return;
            if (Time.time - mouseDownTime > 0.3f && !isShowingDragDialogue)
            {
                OnDragStart();
                isShowingDragDialogue = true;
            }

            gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void OnMouseUp()
        {
            petSurfaceMovement2D.enabled = true;

            if (Time.time - mouseDownTime < 0.3f)
                OnTouch();
            else BlockStatusManager.Instance.HandlePetDrop(Camera.main.ScreenToWorldPoint(Input.mousePosition), this);
        }

        private void InitializePet()
        {
            petSurfaceMovement2D = GetComponent<PetSurfaceMovement2D>();
            petSurfaceMovement2D.StartMovement();
            animationInterval = defaultInterval;
            currentMotionId = GetMotionID(onMoveAction);
            currentFrame = 0;
        }

        private void InitDictionary()
        {
            availableActions = new Dictionary<PetStatus, List<ActionOption>>
            {
                { PetStatus.Idle, onIdleAction },
                { PetStatus.JumpEnd, onJumpEndAction },
                { PetStatus.Move, onMoveAction },
                { PetStatus.Hit, onHitAction }
            };

            stateHandlers = new Dictionary<PetStatus, Action>
            {
                { PetStatus.Idle, () => UpdateStatus(PetStatus.Move) },
                { PetStatus.JumpStart, () => UpdateStatus(PetStatus.JumpEnd) },
                { PetStatus.JumpEnd, () => UpdateStatus(PetStatus.Move) },
                { PetStatus.Move, () => UpdateStatus(PetStatus.Idle) },
                { PetStatus.Hit, () => UpdateStatus(PetStatus.Move) }
            };
        }

        private void UpdateStatus(PetStatus status)
        {
            if (this.status == status) return;
            this.status = status;
            var nextMotionID = GetNextMotionID();
            SetTimer(status);
            UpdateMotion(nextMotionID);
        }

        private string GetNextMotionID()
        {
            string nextMotionID;

            if (availableActions.TryGetValue(status, out var action))
                nextMotionID = GetMotionID(action);
            else
                nextMotionID = GetMotionID(onIdleAction);

            return nextMotionID;
        }

        private void SetTimer(PetStatus status)
        {
            switch (status)
            {
                case PetStatus.Idle:
                    stateTimer = GetRandomTime(idleTimeMin, idleTimeMax);
                    break;
                case PetStatus.Move:
                    stateTimer = GetRandomTime(moveTimeMin, moveTimeMax);
                    break;
                case PetStatus.Hit:
                    stateTimer = Time.time + hitDuration;
                    break;
                default:
                    stateTimer = GetRandomTime(idleTimeMin, idleTimeMax);
                    break;
            }
        }

        private float GetRandomTime(float min, float max)
        {
            return Time.time + Random.Range(min, max);
        }

        private string GetMotionID(List<ActionOption> options)
        {
            float totalWeight = 0;
            float rnd = Random.Range(0, 1);
            foreach (var option in options)
            {
                totalWeight += option.weight;
                if (rnd <= totalWeight) return option.motionID;
            }

            return null;
        }

        private void UpdateCurrentMotion(PetMotion currentMotion)
        {
            currentFrame = 0;

            animationInterval = currentMotion.overideInterval ? currentMotion.interval : defaultInterval;

            animationTimer = Time.time + Random.Range(currentMotion.durationTimeMin, currentMotion.durationTimeMax);
            currentMotion.SetStartTimeAndGetEndTime();

            if (currentMotion.isMovement)
                petSurfaceMovement2D.ContinueMovement(currentMotion.movementSpeed * Random.Range(0.8f, 1.2f));
            else
                petSurfaceMovement2D.PauseMovement();
        }

        private PetMotion GetCurrentMotion(string _motionID)
        {
            var id = currentMotionId == _motionID ? currentMotionId : GetMotionID(_motionID);
            return petMotions[id];
        }

        private string GetMotionID(string _motionID)
        {
            return string.IsNullOrEmpty(_motionID) ? GetRandomMotionID() : _motionID;
        }

        private string GetRandomMotionID()
        {
            var randomMotionIndex = Random.Range(0, petMotions.Count);
            return petMotions.ElementAt(randomMotionIndex).Key;
        }

        private void UpdateMotion(string _motionID)
        {
            currentMotionId = GetMotionID(_motionID);
            var currentMotion = GetCurrentMotion(currentMotionId);

            if (currentMotion == null) return;

            UpdateCurrentMotion(currentMotion);
        }


        private IEnumerator AnimationRoutine()
        {
            for (;;)
            {
                yield return new WaitForSeconds(animationInterval);
                ExecuteActionBasedOnStatus();
                ExecuteMotionUpdate();
                UpdateAnimationFrames();
            }
        }

        private void ExecuteActionBasedOnStatus()
        {
            if (stateTimer < Time.time)
            {
                if (!petMotions[currentMotionId].canExit && animationTimer > Time.time) return;
                Action actionToExecute;
                if (stateHandlers.TryGetValue(status, out actionToExecute))
                    actionToExecute();
                else
                    UpdateStatus(PetStatus.Idle);
            }
        }

        private void ExecuteMotionUpdate()
        {
            if (animationTimer < Time.time)
            {
                string next;

                if (petMotions[currentMotionId].nextAction == PetMotion.NextAction.Idle)
                    next = GetMotionID(onIdleAction);
                else if (petMotions[currentMotionId].nextAction == PetMotion.NextAction.Move)
                    next = GetMotionID(onMoveAction);
                else next = GetMotionID(petMotions[currentMotionId].customActions);
                UpdateMotion(next);
            }
        }

        private void UpdateAnimationFrames()
        {
            if (currentFrame >= sprites[currentMotionId].Count) currentFrame = 0;
            spriteRenderer.sprite = sprites[currentMotionId][currentFrame];
            currentFrame += 1;

            if (petMotions[currentMotionId].loop == false && currentFrame >= sprites[currentMotionId].Count)
                currentFrame = sprites[currentMotionId].Count - 1;
        }

        public void JumpStart()
        {
            previousStatus = status;
            previousMotionID = currentMotionId;
            previousStatusTimer = stateTimer;
            previousMotionTimer = animationTimer;
            jumpStartTime = Time.time;
            UpdateStatus(PetStatus.JumpStart);
        }

        public void JumpUpdate(float normal)
        {
            if (normal > 0.95)
            {
                UpdateStatus(previousStatus);
                UpdateMotion(previousMotionID);
                var delay = Time.time - jumpStartTime;
                stateTimer = previousStatusTimer + delay;
                animationTimer = previousMotionTimer + delay;
            }
            else if (hasExitAnimation && normal > jumpFinishNormal)
            {
                UpdateStatus(PetStatus.JumpEnd);
            }
        }

        [Button]
        private void AddIdsToList()
        {
            AddActionOptions(onIdleAction);
            AddActionOptions(onMoveAction);
            AddActionOptions(onHitAction);
            AddActionOptions(onJumpStartAction);
            if (hasExitAnimation) AddActionOptions(onJumpEndAction);

            void AddActionOptions(List<ActionOption> nextAction)
            {
                foreach (var option in nextAction)
                {
                    petMotions.TryAdd(option.motionID, new PetMotion());
                    sprites.TryAdd(option.motionID, new List<Sprite>());
                }
            }

            for (var i = 0; i < petMotions.Count; i++)
            {
                var motion = petMotions.Values.ToList()[i];
                if (motion.nextAction == PetMotion.NextAction.Custom) AddActionOptions(motion.customActions);
            }
        }

        public void SetToIdle(float holdDuration)
        {
            UpdateStatus(PetStatus.Idle);
            stateTimer = Time.time + holdDuration;
            animationTimer = Time.time + holdDuration;
        }

        public Sprite[] GetJumpAnim()
        {
            return inGameJumpAnim;
        }

        public Sprite[] GetShipAnim()
        {
            return inGameInSpaceShip;
        }

        public PetType GetType()
        {
            return type;
        }

# if UNITY_EDITOR
        [Button]
        private void CopyJumpAnim()
        {
            var spritesList = new List<Sprite>();
            var jumpEndAnim = new List<Sprite>();
            spritesList = sprites[onJumpStartAction[0].motionID];
            jumpEndAnim = hasExitAnimation ? sprites[onJumpEndAction[0].motionID] : null;

            spritesList.AddRange(jumpEndAnim);

            inGameJumpAnim = new Sprite[spritesList.Count];
            for (var i = 0; i < spritesList.Count; i++) inGameJumpAnim[i] = spritesList[i];
        }
#endif

        public void ShowDialogue(string text, bool forceShow = false)
        {
            if (petDialogueController == null)
                petDialogueController = Instantiate(PetManager.Instance.petDialogueControllerPrefab,
                    PetManager.Instance.petDialogueHolder);
            petDialogueController.Init(text, gameObject.transform, forceShow);
        }

        public void OnHit()
        {
            var dialogue = PetDialogueManager.Instance.GetOnHitText(type);
            if (string.IsNullOrEmpty(dialogue)) return;
            ShowDialogue(dialogue);
        }

        private void OnTouch()
        {
            var dialogue = PetDialogueManager.Instance.GetIdleText(type);
            if (string.IsNullOrEmpty(dialogue)) return;
            ShowDialogue(dialogue);
        }

        public void OnIdle()
        {
            if (ignoreIdleDialogue) return;
            var dialogue = PetDialogueManager.Instance.GetIdleText(type);
            if (string.IsNullOrEmpty(dialogue)) return;
            ShowDialogue(dialogue);
        }

        private void OnDragStart()
        {
            var dialogue = PetDialogueManager.Instance.GetOnDragText(type);
            if (string.IsNullOrEmpty(dialogue)) return;
            ShowDialogue(dialogue, true);
        }

        public void OnGameEnter(GameType gameType)
        {
            var dialogue = PetDialogueManager.Instance.GetGameEnterString(type, gameType);
            if (string.IsNullOrEmpty(dialogue)) return;
            ShowDialogue(dialogue, true);
        }

        public void OnGameExit(GameType gameType)
        {
            var dialogue = PetDialogueManager.Instance.GetGameExitString(type, gameType);
            if (string.IsNullOrEmpty(dialogue)) return;
            ShowDialogue(dialogue, true);
        }

        public void OnTitle()
        {
            if (Time.time < 3) return;

            var dialogue = PetDialogueManager.Instance.GetOnTitleString(type);
            if (string.IsNullOrEmpty(dialogue)) return;
            ShowDialogue(dialogue, true);
        }

        public void OnIsland()
        {
            if (Time.time < 3) return;

            var dialogue = PetDialogueManager.Instance.GetOnIslandString(type);
            if (string.IsNullOrEmpty(dialogue)) return;
            if (petDialogueController == null)
                petDialogueController = Instantiate(PetManager.Instance.petDialogueControllerPrefab,
                    PetManager.Instance.petDialogueHolder);
            petDialogueController.Init(dialogue, gameObject.transform, true, -0.4f);
        }

        public void OnShake()
        {
            var dialogue = PetDialogueManager.Instance.GetShakeString(type);
            if (string.IsNullOrEmpty(dialogue)) return;
            ShowDialogue(dialogue);
        }

        public void OnNewFriend()
        {
            var dialogue = PetDialogueManager.Instance.GetNewFriendString(type);
            if (string.IsNullOrEmpty(dialogue)) return;
            ShowDialogue(dialogue);
        }

        public Sprite[] GetWalkAnim()
        {
            var walkAnim = new Sprite[sprites["walk"].Count];

            for (var i = 0; i < sprites["walk"].Count; i++) walkAnim[i] = sprites["walk"][i];

            return walkAnim;
        }

        public void SetSpriteAnimatorIdleAnimation(SpriteAnimator spriteAnimator)
        {
            var rnd = Random.Range(0, onIdleAction.Count);
            var idleAnim = sprites[onIdleAction[rnd].motionID];

            var walkAnim = new Sprite[idleAnim.Count];

            for (var i = 0; i < idleAnim.Count; i++) walkAnim[i] = idleAnim[i];

            spriteAnimator.sprites = walkAnim;
            if (!petMotions[onIdleAction[rnd].motionID].loop) spriteAnimator.RestartWithNoLoop();
            spriteAnimator.interval = petMotions[onIdleAction[rnd].motionID].interval;
        }

        public float GetInterval()
        {
            return defaultInterval;
        }

        private enum PetStatus
        {
            Idle,
            Move,
            JumpStart,
            JumpEnd,
            Hit
        }

        [Serializable]
        private class PetMotion
        {
            public enum NextAction
            {
                Idle,
                Move,
                Custom
            }

            [SerializeField] private DurationType durationMode;

            [ShowIf("@this.durationMode == DurationType.Time")] [LabelText("duration min")]
            public float durationTimeMin;

            [ShowIf("@this.durationMode == DurationType.Time")] [LabelText("duration max")]
            public float durationTimeMax;

            [SerializeField] [ShowIf("@this.durationMode == DurationType.LoopCount")] [LabelText("loop min")]
            private int durationCountMin;

            [SerializeField] [ShowIf("@this.durationMode == DurationType.LoopCount")] [LabelText("loop max")]
            private int durationCountMax;

            [HorizontalGroup("anim")] public bool loop = true, canExit = true, overideInterval;
            [ShowIf("overideInterval")] public float interval;

            [HorizontalGroup("movement")] public bool isMovement;

            [HorizontalGroup("movement")] [ShowIf("isMovement")] [LabelText("speed")]
            public float movementSpeed = 2.5f;


            public NextAction nextAction;

            [ShowIf("@nextAction == NextAction.custom")]
            public List<ActionOption> customActions = new();

            public float SetStartTimeAndGetEndTime()
            {
                return Time.time + Random.Range(durationTimeMin, durationTimeMax);
            }

            private enum DurationType
            {
                Time,
                LoopCount
            }
        }

        [Serializable]
        private class ActionOption
        {
            [SerializeField] [HorizontalGroup("Actions")] [LabelText("ID")]
            public string motionID;

            [SerializeField] [HorizontalGroup("Actions")]
            public float weight = 1f;

            public ActionOption(string ID)
            {
                motionID = ID;
                weight = 1f;
            }
        }
    }
}