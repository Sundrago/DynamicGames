using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SurfaceMovement2D))]
public class Pet : SerializedMonoBehaviour
{
    [SerializeField]
    private PetType type;
    [SerializeField]
    private float defaultInterval = 0.2f;
    [SerializeField]
    public SpriteRenderer spriteRenderer;

    [SerializeField] private PetInfo_UI petinfo;
    [SerializeField] public Transform centerPoint;
    
    
    [TitleGroup("CallBacks", alignment: TitleAlignments.Centered)]
    [SerializeField, BoxGroup("CallBacks/IDLE")]
    private float idleTimeMin = 5;
    [SerializeField, BoxGroup("CallBacks/IDLE")]
    private float idleTimeMax = 7;
    [SerializeField, TableList, BoxGroup("CallBacks/IDLE")]
    List<ActionOption> onIdleAction = new List<ActionOption>() {new ActionOption("idle")};

    [SerializeField, BoxGroup("CallBacks/MOVE")]
    float moveTimeMin = 5;
    [SerializeField, BoxGroup("CallBacks/MOVE")]
    float moveTimeMax = 7;
    [SerializeField, TableList, BoxGroup("CallBacks/MOVE")]
    List<ActionOption> onMoveAction = new List<ActionOption>() {new ActionOption("walk")};
    
    [SerializeField, TableList, BoxGroup("CallBacks/HIT")]
    List<ActionOption> onHitAction = new List<ActionOption>() {new ActionOption("walk")};

    [SerializeField, BoxGroup("CallBacks/JUMP")]
    bool hasExitAnimation;
    [SerializeField, TableList, BoxGroup("CallBacks/JUMP")]
    List<ActionOption> onJumpStartAction = new List<ActionOption>() {new ActionOption("jump")};
    [SerializeField, TableList, BoxGroup("CallBacks/JUMP"), ShowIf("hasExitAnimation")]
    List<ActionOption> onJumpEndAction = new List<ActionOption>() {new ActionOption("jump_end")};
    [SerializeField, BoxGroup("CallBacks/JUMP"), ShowIf("hasExitAnimation")]
    private float jumpFinishNormal = 0.5f;
    
    [SerializeField, TitleGroup("Actions", alignment: TitleAlignments.Centered)]
    Dictionary<string, PetMotion> petMotions = new Dictionary<string, PetMotion>();

    [SerializeField, TitleGroup("Sprites", alignment: TitleAlignments.Centered)]
    Dictionary<string, List<Sprite>> sprites = new Dictionary<string, List<Sprite>>();
    private SurfaceMovement2D surfaceMovement2D;
    
    private enum PetStatus { Idle, Move, JumpStart, JumpEnd, Hit};
    private PetStatus status;
    private string motionID;
    private int motionFrame;

    private float interval;
    
    private float statusTimer;
    private float motionTimer;

    private float jumpStartDuration, jumpEndDuration, hitDuration;
    
    private void UpdateStatus(PetStatus _status)
    {
        if(status == _status) return;
        status = _status;

        string nextMotionID;
        switch (status)
        {
            case PetStatus.Idle :
                statusTimer = Time.time + Random.Range(idleTimeMin, idleTimeMax);
                nextMotionID = GetMotionID(onIdleAction);
                break;
            case PetStatus.JumpStart :
                nextMotionID = GetMotionID(onJumpStartAction);
                break;
            case PetStatus.JumpEnd :
                nextMotionID = GetMotionID(onJumpEndAction);
                break;
            case PetStatus.Move :
                statusTimer = Time.time + Random.Range(moveTimeMin, moveTimeMax);
                nextMotionID = GetMotionID(onMoveAction);
                break;
            case PetStatus.Hit :
                statusTimer = Time.time + hitDuration;
                nextMotionID = GetMotionID(onHitAction);
                break;
            default :
                statusTimer = Time.time + Random.Range(idleTimeMin, idleTimeMax);
                nextMotionID = GetMotionID(onIdleAction);
                break;
        }
        
        UpdateMotion(nextMotionID);
    }

    private string GetMotionID(List<ActionOption> options)
    {
        float totalWeight = 0;
        float startWeight = 0;
        foreach (ActionOption option in options)
        {
            totalWeight += option.weight;
        }

        float rnd = Random.Range(0, totalWeight);

        for (int i = 0; i < options.Count; i++)
        {
            if (rnd >= startWeight && rnd < startWeight + options[i].weight) return options[i].motionID;
            startWeight += options[i].weight;
        }

        print("GetMotionID FAILED");
        return null;
    }

    private void UpdateMotion(string _motionID)
    {
        if(motionID == _motionID) return;
        motionID = _motionID;
        motionFrame = 0;

        if (petMotions[motionID].overideInterval) interval = petMotions[motionID].interval;
        else interval = defaultInterval;

        motionTimer = Time.time + Random.Range(petMotions[motionID].durationTimeMin, petMotions[motionID].durationTimeMax);
        petMotions[motionID].SetStartTimeAndGetEndTime();

        if (petMotions[motionID].isMovement) surfaceMovement2D.ContinueMovement(petMotions[motionID].movementSpeed * Random.Range(0.8f, 1.2f));
        else surfaceMovement2D.PauseMovement();
        
        // print(type + " : " + motionID);
    }

    private void Awake()
    {
        surfaceMovement2D = GetComponent<SurfaceMovement2D>();
        surfaceMovement2D.StartMovement();
        interval = defaultInterval;
        motionID = GetMotionID(onMoveAction);
        motionFrame = 0;
    }

    private void OnEnable()
    {
        StartCoroutine(Animator());
    }

    private IEnumerator Animator()
    {
        for (;;)
        {
            yield return new WaitForSeconds(interval);
            //Check If Need Animation Update
            if (statusTimer < Time.time)
            {
                if(!petMotions[motionID].canExit && motionTimer > Time.time) continue;
                switch (status)
                {
                    case PetStatus.Idle :
                        UpdateStatus(PetStatus.Move);
                        break;
                    case PetStatus.JumpStart :
                        UpdateStatus(PetStatus.JumpEnd);
                        break;
                    case PetStatus.JumpEnd :
                        UpdateStatus(PetStatus.Move);
                        break;
                    case PetStatus.Move :
                        UpdateStatus(PetStatus.Idle);
                        break;
                    case PetStatus.Hit :
                        UpdateStatus(PetStatus.Move);
                        break;
                    default :
                        UpdateStatus(PetStatus.Idle);
                        break;
                }
            }
            else if(motionTimer < Time.time)
            {
                string next;
                
                if(petMotions[motionID].nextAction == PetMotion.NextAction.idle) next = GetMotionID(onIdleAction);
                else if (petMotions[motionID].nextAction == PetMotion.NextAction.move) next = GetMotionID(onMoveAction);
                else next = GetMotionID(petMotions[motionID].customActions);
                UpdateMotion(next);
            }
            
            //Update Anim
            if (motionFrame >= sprites[motionID].Count) motionFrame = 0;
            spriteRenderer.sprite = sprites[motionID][motionFrame];
            motionFrame += 1;

            if (petMotions[motionID].loop == false && motionFrame >= sprites[motionID].Count) motionFrame = sprites[motionID].Count - 1;
        }
    }

    private PetStatus previousStatus;
    private string previoueMotion;
    private float previousStatusTimer, previousMotionTimer, jumpStartTime;
    public void JumpStart()
    {
        previousStatus = status;
        previoueMotion = motionID;
        previousStatusTimer = statusTimer;
        previousMotionTimer = motionTimer;
        jumpStartTime = Time.time;
        UpdateStatus(PetStatus.JumpStart);
    }

    public void JumpUpdate(float normal)
    {
        if (normal > 0.95)
        {
            UpdateStatus(previousStatus);
            UpdateMotion(previoueMotion);
            float delay = Time.time - jumpStartTime;
            statusTimer = previousStatusTimer + delay;
            motionTimer = previousMotionTimer + delay;
        }
        else if(hasExitAnimation && normal>jumpFinishNormal) UpdateStatus(PetStatus.JumpEnd);
    }
    
    // --------- --------- --------- --------- --------- --------- --------- --------- --------- //
    
    [Button]
    private void AddIdsToList()
    {
        AddActionOptions(onIdleAction);
        AddActionOptions(onMoveAction);
        AddActionOptions(onHitAction);
        AddActionOptions(onJumpStartAction);
        if(hasExitAnimation) AddActionOptions(onJumpEndAction);

        void AddActionOptions(List<ActionOption> nextAction)
        {
            foreach (ActionOption option in nextAction)
            {
                petMotions.TryAdd(option.motionID, new PetMotion());
                sprites.TryAdd(option.motionID, new List<Sprite>());
            }
        }
        
        for (int i = 0; i < petMotions.Count; i++)
        {
            PetMotion motion = petMotions.Values.ToList()[i];
            if (motion.nextAction == PetMotion.NextAction.custom)
            {
                AddActionOptions(motion.customActions);
            }
        }
    }

    // public void MotionTest(PetMotion petmotion)
    // {
    //     string key = petMotions.FirstOrDefault(x => x.Value == petmotion).Key;
    //     UpdateMotion(key);
    // }
    
    // --------- --------- --------- --------- --------- --------- --------- --------- --------- //
    
    [Serializable]
    class PetMotion
    {
        enum DurationType { Time, LoopCount };
        public enum NextAction { idle, move, custom };
        [SerializeField] DurationType durationMode;

        [ShowIf("@this.durationMode == DurationType.Time"), LabelText("duration min")]
        public float durationTimeMin;
        [ShowIf("@this.durationMode == DurationType.Time"), LabelText("duration max")]
        public float durationTimeMax;

        [SerializeField, ShowIf("@this.durationMode == DurationType.LoopCount"), LabelText("loop min")]
        private int durationCountMin;
        [SerializeField, ShowIf("@this.durationMode == DurationType.LoopCount"), LabelText("loop max")]
        private int durationCountMax;

        [HorizontalGroup("anim")] public bool loop = true, canExit = true, overideInterval=false;
        [ShowIf("overideInterval")]
        public float interval;

        [HorizontalGroup("movement")] public bool isMovement = false;
        [HorizontalGroup("movement"), ShowIf("isMovement"), LabelText("speed")] public float movementSpeed = 2.5f;

        
        public NextAction nextAction;
        [ShowIf("@nextAction == NextAction.custom")]
        public List<ActionOption> customActions = new List<ActionOption>();

        public float SetStartTimeAndGetEndTime()
        {
            return Time.time + Random.Range(durationTimeMin, durationTimeMax);
        }
    }

    [Serializable]
    class ActionOption
    {
        [SerializeField, HorizontalGroup("Actions"), LabelText("ID")]
        public string motionID;
        [SerializeField, HorizontalGroup("Actions")]
        public float weight = 1f;

        public ActionOption(string ID)
        {
            motionID = ID;
            weight = 1f;
        }
    }
    
    //Click Event
    void OnMouseDown()
    {
        print(type);
        petinfo.ShowPanel(type);
    }
}
