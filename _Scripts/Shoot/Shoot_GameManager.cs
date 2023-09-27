using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;

public class Shoot_GameManager : MonoBehaviour
{
    [SerializeField] Animator door_left, door_right;
    [SerializeField] Shoot_Enemy_Manager enemy_Manager;
    [SerializeField] Shoot_Bullet_Manager bullet_Manager;
    [SerializeField] public Transform player, island;
    [SerializeField] FXManager fXManager;
    [SerializeField] Shoot_joystick joystick;
    [SerializeField] Shoot_item shoot_Item;

    [SerializeField] Animator face;
    [SerializeField] IslandSizeCtrl islandSizeCtrl;
    //[SerializeField] EndScoreCtrl endScore;
    [SerializeField] ShootScoreManager score;
    [SerializeField] Transform startPosition, loadPosition;
    [SerializeField] SFXCTRL sfx;
    [SerializeField] private GameObject adj_transition_notch;
    [SerializeField] private shoot_guide_hand hand;

    public static Shoot_GameManager Instacne;
    
    private AutoAttackInfo createEnemyInCircle = new AutoAttackInfo();
    private AutoAttackInfo createEnemyRandomPos = new AutoAttackInfo();
    private AutoAttackInfo createMetheor = new AutoAttackInfo();
    private AutoAttackInfo createEnemyInSpira = new AutoAttackInfo();
    private AutoAttackInfo createEnemyInLine = new AutoAttackInfo();
    private AutoAttackInfo createItem = new AutoAttackInfo();
    private AudioCtrl audioCtrl;

    public enum ShootGameState { ready, dead, playing }

    [ReadOnly]
    public FX shield = null;
    public ShootGameState state;

    public bool spinMode = false;
    private float spinTime;
    private int stageFinished, currentStagePlaying = -1;

    private void Awake()
    {
        Instacne = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        audioCtrl = AudioCtrl.Instance;
        
        ChangeStatus(ShootGameState.ready);
        stage = 0;
        stageFinished = 0;
        createEnemyInCircle.Init(1000, 0, 0);
        createEnemyRandomPos.Init(1100, 0, 0);
        createMetheor.Init(1200,0,0);
        createEnemyInLine.Init(1300,0,0);
        createEnemyInSpira.Init(1400,0,0);
        createItem.Init(3700,1,1, 0.5f);
        
        hand.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case ShootGameState.ready:
                Update_ready();
                break;
            case ShootGameState.playing:
                Update_Playing();
                break;
            case ShootGameState.dead:
                break;
        }
    }

    private void Update_ready()
    {
        if (joystick.vecNormal != Vector3.zero) ChangeStatus(ShootGameState.playing);
    }

    private void Update_Playing()
    {
        if(Time.frameCount % 20 != 0) return;
        if (spinMode && Time.time > spinTime) spinMode = false;
        
        
        int myScofre = score.GetScore();

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

    public int stage;
    
    async Task SetStage(int _stage)
    {
        if(state != ShootGameState.playing) return;
        if(stage >= _stage) return;
        if(currentStagePlaying != -1) return;

        stage = stageFinished + 1;
        currentStagePlaying = stage;
        print("Stage : [ " + stage + " ] Started");
        
        int rnd;
        switch (stage)
        {
            case 0:
                createEnemyInCircle.Init(1000, 0, 0);
                createEnemyRandomPos.Init(1100, 0, 0);
                createMetheor.Init(1200,0,0);
                createEnemyInLine.Init(1300, 0, 0);
                createEnemyInSpira.Init(1400, 0, 0);
                createItem.Init(3500,1,1, 0.5f);
                shoot_Item.SpawnItem();
                break;
            case 1:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("turnRed");
                await Task.Delay(2000);
                rnd = Random.Range(0, 3);
                switch (rnd)
                {
                    case 0:
                        CreateMetheor();
                        await Task.Delay(2000);
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 1:
                        enemy_Manager.SpawnEnemyAtRandomPos();
                        enemy_Manager.SpawnEnemyAtRandomPos();
                        enemy_Manager.SpawnEnemyAtRandomPos();
                        await Task.Delay(1000);
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 2:
                        SpawnOnLeft(2);
                        SpawnOnRight(2);
                        await Task.Delay(3000);
                        islandSizeCtrl.CloseIsland();
                        break;
                }
                await Task.Delay(1000);
                createEnemyInCircle.Init(12000, 0, 1);
                createEnemyRandomPos.Init(3000, 0, 2);
                createMetheor.Init(1000,0,0);
                shoot_Item.SpawnItem();
                break;
            case 2:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
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
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 1:
                        enemy_Manager.SpawnEnemyAtRandomPos();
                        enemy_Manager.SpawnEnemyAtRandomPos();
                        enemy_Manager.SpawnEnemyAtRandomPos();
                        enemy_Manager.SpawnEnemyAtRandomPos();
                        await Task.Delay(1000);
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 2:
                        SpawnOnLeft(4);
                        SpawnOnRight(4);
                        await Task.Delay(4000);
                        islandSizeCtrl.CloseIsland();
                        break;
                }
                await Task.Delay(1000);
                createEnemyInCircle.Init(10000, 0, 1);
                createEnemyRandomPos.Init(2900, 0, 3);
                createMetheor.Init(1000,0,0);
                break;
            case 3:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
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
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 1:
                        enemy_Manager.SpawnEnemyAtRandomPos();
                        enemy_Manager.SpawnEnemyAtRandomPos();
                        enemy_Manager.SpawnEnemyAtRandomPos();
                        enemy_Manager.SpawnEnemyAtRandomPos();
                        enemy_Manager.SpawnEnemyAtRandomPos();
                        await Task.Delay(1000);
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 2:
                        SpawnOnLeft(5);
                        SpawnOnRight(5);
                        await Task.Delay(4000);
                        islandSizeCtrl.CloseIsland();
                        break;
                }
                await Task.Delay(1000);
                createEnemyInCircle.Init(10000, 0, 4);
                createEnemyRandomPos.Init(2900, 0, 4);
                createMetheor.Init(9000,0,1);
                createEnemyInLine.Init(7500, 3, 5, 0.3f);
                break;
            case 4:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);
                enemy_Manager.SpawnEnemyInLineY(6);
                
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
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 1:
                        enemy_Manager.SpawnEnemyInCircle(1f, Random.Range(5, 8));
                        await Task.Delay(1000);
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 2:
                        SpawnOnLeft(5);
                        SpawnOnRight(5);
                        await Task.Delay(4000);
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 3:
                        await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.6f, 20, 1.3f, 30, 0.75f);
                        break;
                }
                await Task.Delay(1000);
                createEnemyInCircle.Init(10000, 2, 6);
                createEnemyRandomPos.Init(2900, 0, 4);
                createMetheor.Init(9000,0,1);
                createEnemyInLine.Init(7500, 3, 8, 0.3f);
                break;
            case 5:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);
                enemy_Manager.SpawnEnemyInLineY(8);
                
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
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 1:
                        enemy_Manager.SpawnEnemyInCircle(1f, Random.Range(5, 12));
                        await Task.Delay(1000);
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 2:
                        SpawnOnLeft(5);
                        SpawnOnRight(5);
                        await Task.Delay(4000);
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 3:
                        await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 25, 1.35f, 30, 0.5f);
                        break;
                }
                await Task.Delay(1000);
                createEnemyInCircle.Init(10000, 2, 6);
                createEnemyRandomPos.Init(2900, 1, 4);
                createMetheor.Init(9000,0,2);
                createEnemyInLine.Init(7500, 3, 8, 0.4f);
                break;
            case 6:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);
                enemy_Manager.SpawnEnemyInLineY(10);
                
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
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 1:
                        enemy_Manager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                        await Task.Delay(300);
                        enemy_Manager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                        await Task.Delay(1000);
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 2:
                        SpawnOnLeft(5);
                        SpawnOnRight(5);
                        await Task.Delay(4000);
                        islandSizeCtrl.CloseIsland();
                        break;
                    case 3:
                        await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30, 0.5f);
                        break;
                }
                await Task.Delay(1000);
                createEnemyInCircle.Init(10000, 3, 12);
                createEnemyRandomPos.Init(2900, 1, 4);
                createMetheor.Init(8000,0,2);
                createEnemyInLine.Init(7500, 5, 10, 0.45f);
                break;
            case 7:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);

                for (int i = 0; i < 2; i++)
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
                            enemy_Manager.SpawnEnemyInCircle(1f, Random.Range(8, 15));
                            await Task.Delay(3000);
                            break;
                        case 2:
                            SpawnOnLeft(5);
                            SpawnOnRight(5);
                            await Task.Delay(4000);
                            break;
                        case 3:
                            await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 20, 1.5f, 30, 0.5f);
                            await Task.Delay(1500);
                            break;
                        case 4:
                            await Task.Delay(3000);
                            enemy_Manager.SpawnEnemyInLineY(7, 0.7f);
                            await Task.Delay(1500);
                            enemy_Manager.SpawnEnemyInLineY(7);
                            await Task.Delay(5000);
                            break;
                    }
                }
                islandSizeCtrl.CloseIsland();
                await Task.Delay(1000);
                createEnemyInCircle.Init(10000, 3, 12);
                createEnemyRandomPos.Init(2900, 1, 4);
                createMetheor.Init(4900,1,1, 0.5f);
                createEnemyInLine.Init(7500, 5, 10, 0.45f);
                break;
            case 8:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);

                for (int i = 0; i < 2; i++)
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
                            enemy_Manager.SpawnEnemyInCircle(1f, Random.Range(8, 15));
                            await Task.Delay(2500);
                            break;
                        case 2:
                            SpawnOnLeft(5);
                            SpawnOnRight(5);
                            await Task.Delay(3000);
                            break;
                        case 3:
                            await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 22, 1.5f, 30, 0.5f);
                            await Task.Delay(1500);
                            break;
                        case 4:
                            await Task.Delay(3000);
                            enemy_Manager.SpawnEnemyInLineY(7, 0.7f);
                            await Task.Delay(1500);
                            enemy_Manager.SpawnEnemyInLineY(7);
                            await Task.Delay(4000);
                            break;
                    }
                }
                islandSizeCtrl.CloseIsland();
                await Task.Delay(1000);
                createEnemyInCircle.Init(10000, 3, 12);
                createEnemyRandomPos.Init(2400, 1, 4);
                createMetheor.Init(4900,1,1, 0.5f);
                createEnemyInLine.Init(6500, 5, 10, 0.45f);
                break;
            case 9:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(500);
                CreateMetheor();
                await Task.Delay(500);

                for (int i = 0; i < 2; i++)
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
                            enemy_Manager.SpawnEnemyInCircle(1f, Random.Range(15, 30));
                            await Task.Delay(3200);
                            break;
                        case 2:
                            SpawnOnLeft(5);
                            SpawnOnRight(5);
                            await Task.Delay(2500);
                            break;
                        case 3:
                            await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 25, 1.5f, 30, 0.5f);
                            await Task.Delay(2500);
                            break;
                        case 4:
                            await Task.Delay(3000);
                            enemy_Manager.SpawnEnemyInLineY(7, 0.7f);
                            await Task.Delay(1500);
                            enemy_Manager.SpawnEnemyInLineY(7);
                            await Task.Delay(3000);
                            break;
                    }
                }
                islandSizeCtrl.CloseIsland();
                await Task.Delay(1000);
                createEnemyInCircle.Init(10000, 3, 12);
                createEnemyRandomPos.Init(2400, 1, 4);
                createMetheor.Init(4900,1,1, 0.5f);
                createEnemyInLine.Init(7500, 5, 10, 0.45f);
                break;
            case 10:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);
                enemy_Manager.SpawnEnemyInLineY(10);

                for (int i = 0; i < 2; i++)
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
                            enemy_Manager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                            await Task.Delay(300);
                            enemy_Manager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                            await Task.Delay(1000);
                            break;
                        case 2:
                            SpawnOnLeft(5);
                            SpawnOnRight(5);
                            await Task.Delay(4000);
                            break;
                        case 3:
                            await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30, 0.5f);
                            break;
                        case 4:
                            await Task.Delay(3000);
                            enemy_Manager.SpawnEnemyInLineY(10, 0.7f);
                            await Task.Delay(1500);
                            enemy_Manager.SpawnEnemyInLineY(10);
                            break;
                    }
                }
                
                islandSizeCtrl.CloseIsland();
                await Task.Delay(1000);
                createEnemyInCircle.Init(10000, 3, 12);
                createEnemyRandomPos.Init(2900, 1, 4);
                createMetheor.Init(4900,1,1, 0.5f);
                createEnemyInLine.Init(7500, 5, 10, 0.45f);
                break;
            case 11:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);
                enemy_Manager.SpawnEnemyInLineY(15);

                for (int i = 0; i < 3; i++)
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
                            enemy_Manager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                            await Task.Delay(300);
                            enemy_Manager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                            await Task.Delay(1000);
                            break;
                        case 2:
                            SpawnOnLeft(5);
                            SpawnOnRight(5);
                            await Task.Delay(4000);
                            break;
                        case 3:
                            await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30, 0.5f);
                            break;
                        case 4:
                            await Task.Delay(3000);
                            enemy_Manager.SpawnEnemyInLineY(10, 0.7f);
                            await Task.Delay(1500);
                            enemy_Manager.SpawnEnemyInLineY(10);
                            break;
                    }
                }
                
                islandSizeCtrl.CloseIsland();
                await Task.Delay(1000);
                createEnemyInCircle.Init(8000, 3, 15);
                createEnemyRandomPos.Init(1400, 1, 4);
                createMetheor.Init(3900,1,1, 0.5f);
                createEnemyInLine.Init(6900, 5, 10, 0.45f);
                break;
            case 12:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);
                enemy_Manager.SpawnEnemyInLineY(15);

                for (int i = 0; i < 3; i++)
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
                            enemy_Manager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                            await Task.Delay(300);
                            enemy_Manager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                            await Task.Delay(1000);
                            break;
                        case 2:
                            SpawnOnLeft(5);
                            SpawnOnRight(5);
                            await Task.Delay(4000);
                            break;
                        case 3:
                            await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30, 0.5f);
                            break;
                        case 4:
                            await Task.Delay(3000);
                            enemy_Manager.SpawnEnemyInLineY(10, 0.7f);
                            await Task.Delay(1500);
                            enemy_Manager.SpawnEnemyInLineY(10);
                            break;
                    }
                }
                
                islandSizeCtrl.CloseIsland();
                await Task.Delay(1000);
                createEnemyInCircle.Init(8000, 7, 15);
                createEnemyRandomPos.Init(1400, 1, 4);
                createMetheor.Init(2900,1,1, 0.5f);
                createEnemyInLine.Init(6900, 5, 10, 0.45f);
                break;
            case 13:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);
                enemy_Manager.SpawnEnemyInLineY(15);

                for (int i = 0; i < 4; i++)
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
                            enemy_Manager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                            await Task.Delay(300);
                            enemy_Manager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                            await Task.Delay(1000);
                            break;
                        case 2:
                            SpawnOnLeft(5);
                            SpawnOnRight(5);
                            await Task.Delay(4000);
                            break;
                        case 3:
                            await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30, 0.5f);
                            break;
                        case 4:
                            await Task.Delay(3000);
                            enemy_Manager.SpawnEnemyInLineY(10, 0.7f);
                            await Task.Delay(1500);
                            enemy_Manager.SpawnEnemyInLineY(10);
                            break;
                    }
                }
                
                islandSizeCtrl.CloseIsland();
                await Task.Delay(1000);
                createEnemyInCircle.Init(7000, 7, 15);
                createEnemyRandomPos.Init(900, 1, 4);
                createMetheor.Init(2900,1,1, 0.5f);
                createEnemyInLine.Init(6900, 5, 10, 0.45f);
                break;
            case 14:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);
                enemy_Manager.SpawnEnemyInLineY(15);

                for (int i = 0; i < 4; i++)
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
                            enemy_Manager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                            await Task.Delay(300);
                            enemy_Manager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                            await Task.Delay(1000);
                            break;
                        case 2:
                            SpawnOnLeft(5);
                            SpawnOnRight(5);
                            await Task.Delay(4000);
                            break;
                        case 3:
                            await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30, 0.5f);
                            break;
                        case 4:
                            await Task.Delay(3000);
                            enemy_Manager.SpawnEnemyInLineY(10, 0.7f);
                            await Task.Delay(1500);
                            enemy_Manager.SpawnEnemyInLineY(10);
                            break;
                    }
                }
                
                islandSizeCtrl.CloseIsland();
                await Task.Delay(1000);
                createEnemyInCircle.Init(4100, 7, 15);
                createEnemyRandomPos.Init(800, 1, 4);
                createMetheor.Init(1100,1,1, 0.3f);
                createEnemyInLine.Init(4900, 5, 10, 0.45f);
                break;
            case 15:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);
                enemy_Manager.SpawnEnemyInLineY(15);

                for (int i = 0; i < 5; i++)
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
                            enemy_Manager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                            await Task.Delay(300);
                            enemy_Manager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                            await Task.Delay(1000);
                            break;
                        case 2:
                            SpawnOnLeft(5);
                            SpawnOnRight(5);
                            await Task.Delay(4000);
                            break;
                        case 3:
                            await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30, 0.5f);
                            break;
                        case 4:
                            await Task.Delay(3000);
                            enemy_Manager.SpawnEnemyInLineY(10, 0.7f);
                            await Task.Delay(1500);
                            enemy_Manager.SpawnEnemyInLineY(10);
                            break;
                    }
                }
                
                islandSizeCtrl.CloseIsland();
                await Task.Delay(1000);
                createEnemyInCircle.Init(4100, 7, 15);
                createEnemyRandomPos.Init(500, 1, 4);
                createMetheor.Init(1100,1,1, 0.5f);
                createEnemyInLine.Init(4900, 5, 10, 0.45f);
                break;
            case 16:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);
                enemy_Manager.SpawnEnemyInLineY(15);

                for (int i = 0; i < 5; i++)
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
                            enemy_Manager.SpawnEnemyInCircle(0.8f, Random.Range(3, 5));
                            await Task.Delay(300);
                            enemy_Manager.SpawnEnemyInCircle(1.2f, Random.Range(5, 8));
                            await Task.Delay(1000);
                            break;
                        case 2:
                            SpawnOnLeft(5);
                            SpawnOnRight(5);
                            await Task.Delay(4000);
                            break;
                        case 3:
                            await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 30, 0.5f);
                            break;
                        case 4:
                            await Task.Delay(3000);
                            enemy_Manager.SpawnEnemyInLineY(10, 0.7f);
                            await Task.Delay(1500);
                            enemy_Manager.SpawnEnemyInLineY(10);
                            break;
                    }
                }
                
                islandSizeCtrl.CloseIsland();
                await Task.Delay(1000);
                createEnemyInCircle.Init(2100, 7, 15);
                createEnemyRandomPos.Init(500, 1, 4);
                createMetheor.Init(700,1,1, 0.5f);
                createEnemyInLine.Init(3900, 5, 10, 0.45f);
                break;
            case 17:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);
                enemy_Manager.SpawnEnemyInLineY(15);

                for (int i = 0; i < 5; i++)
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
                            enemy_Manager.SpawnEnemyInCircle(0.8f, Random.Range(5, 10));
                            await Task.Delay(300);
                            enemy_Manager.SpawnEnemyInCircle(1.2f, Random.Range(10, 20));
                            await Task.Delay(1000);
                            break;
                        case 2:
                            SpawnOnLeft(8);
                            SpawnOnRight(8);
                            await Task.Delay(4000);
                            break;
                        case 3:
                            await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 30, 1.5f, 10, 0.5f);
                            break;
                        case 4:
                            await Task.Delay(3000);
                            enemy_Manager.SpawnEnemyInLineY(15, 0.7f);
                            await Task.Delay(1500);
                            enemy_Manager.SpawnEnemyInLineY(15);
                            break;
                    }
                }
                
                islandSizeCtrl.CloseIsland();
                await Task.Delay(1000);
                createEnemyInCircle.Init(1100, 7, 15);
                createEnemyRandomPos.Init(300, 1, 4);
                createMetheor.Init(500,1,1, 0.5f);
                createEnemyInLine.Init(2900, 5, 10, 0.45f);
                break;
        }

        print("Stage : [ " + stage + " ] Finished");
        await Task.Delay(1000);
        stageFinished = stage;
        currentStagePlaying = -1;
    }

    async Task CreateEnemyAtRandomPos()
    {
        if(state != ShootGameState.playing) return;
        
        AutoAttackInfo info = createEnemyRandomPos;
        if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
        {
            int amt = Random.Range(info.min, info.max + 1);
            for (int i = 0; i < amt; i++)
            {
                enemy_Manager.SpawnEnemyAtRandomPos();
            }
        }
        await Task.Delay(info.delay);
        CreateEnemyAtRandomPos();
    }

    async Task CreateEnemyAtPlayerInCircle()
    {
        if(state != ShootGameState.playing) return;

        AutoAttackInfo info = createEnemyInCircle;
        if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
        {
            enemy_Manager.SpawnEnemyInCircle(1f, Random.Range(info.min, info.max));
        }
        await Task.Delay(info.delay);
        CreateEnemyAtPlayerInCircle();
    }

    async Task CreateMetheors()
    {
        if(state != ShootGameState.playing) return;
        
        AutoAttackInfo info = createMetheor;
        
        if (stage <= 2)
        {
            await Task.Delay(info.delay);
            CreateEnemyInLine();
            return;
        }
        
        if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
        {
            int amt = Random.Range(info.min, info.max + 1);
            for (int i = 0; i < amt; i++)
            {
                await Task.Delay(2000);
                CreateMetheor();
            }
        }
        await Task.Delay(info.delay);
        CreateMetheors();
    }

    async Task CreateEnemyInLine()
    {
        if(state != ShootGameState.playing) return;
        AutoAttackInfo info = createEnemyInLine;
        
        if (stage <= 3)
        {
            await Task.Delay(info.delay);
            CreateEnemyInLine();
            return;
        }

        if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
        {
            enemy_Manager.SpawnEnemyInLineY(Random.Range(info.min, info.max+ 1));
        }

        await Task.Delay(info.delay);
        CreateEnemyInLine();
    }

    async Task CreateEnemyInSpiral()
    {
        if(state != ShootGameState.playing) return;
        AutoAttackInfo info = createEnemyInSpira;
        
        if (stage <= 3)
        {
            await Task.Delay(info.delay);
            CreateEnemyInLine();
            return;
        }

        if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
        {
            await enemy_Manager.SpawnEnemyInSpiral(0.6f * Random.Range(0.9f, 1.1f),
                1.5f * Random.Range(0.85f, 1.3f), Random.Range(info.min, info.max+ 1)
                , 1.5f * Random.Range(0.7f, 1.3f), 35, 0.6f * Random.Range(0.8f, 1.2f));
        }

        await Task.Delay(info.delay);
        CreateEnemyInSpiral();
    }

    async Task CreateItem()
    {
        if(state != ShootGameState.playing) return;
        
        AutoAttackInfo info = createItem;

        int count = shoot_Item.items.Count;
        info.probability = (1 - 0.4f * count) * 0.85f;
        
        if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
        {
            shoot_Item.SpawnItem();
        }

        await Task.Delay(info.delay);
        CreateItem();
    }
    
    
    public void RestartGame()
    {
        EndScoreCtrl.Instance.HideScore();
        DestroyShield();

        face.SetTrigger("idle");
        islandSizeCtrl.CloseIsland();
        score.ResetScore();

        player.DOMove(startPosition.position, 0.5f)
            .SetEase(Ease.InOutCubic);
        player.DOLocalRotate(startPosition.localEulerAngles, 0.5f)
            .SetEase(Ease.InOutCubic)
            .OnComplete(()=> {
                ChangeStatus(ShootGameState.ready);
                joystick.gameObject.SetActive(true);
            });
        stageFinished = 0;
        enemy_Manager.KillAll();
        shoot_Item.KillAll();
        bullet_Manager.Restart();
        stage = 0;
        spinMode = false;
    }

    async Task SpawnOnLeft(int count)
    {
        door_left.SetTrigger("open");
        await Task.Delay(400);
        for(int i = 0; i<count; i++)
        {
            enemy_Manager.SpawnOnIsland(180, -1.5f, 0f);
            await Task.Delay(1000);
        }
        door_left.SetTrigger("close");
    }

    async Task SpawnOnRight(int count)
    {
        door_right.SetTrigger("open");
        await Task.Delay(400);
        for (int i = 0; i < count; i++)
        {
            enemy_Manager.SpawnOnIsland(0, 1.5f, 0f);
            await Task.Delay(1000);
        }
        door_right.SetTrigger("close");
    }
    
    private void CreateMetheor()
    {
        Vector3[] path = new Vector3[3];
        path[0] = island.transform.position;
        path[2] = player.transform.position;
        path[0].z = path[2].z;

        float ydiff = Mathf.Abs(player.transform.position.y - island.transform.position.y);
        float xdiff = map(ydiff, 0, 5, 0.2f, 1.5f);
        path[1] = Vector3.Lerp(path[0], path[2], 0.5f);
        path[1].x = player.position.x < 0 ? -xdiff : xdiff;

        GameObject metheor = fXManager.CreateFX(FXType.ShadowMissile, path[0]);
        metheor.transform.DOPath(path, Random.Range(1.8f, 2.2f), PathType.CatmullRom, PathMode.TopDown2D, 1, Color.red)
            .SetEase(Ease.OutQuart)
            .OnComplete(()=> {
                //fXManager.KillFX(metheor.GetComponent<FX>());
                fXManager.CreateFX(FXType.ShadowBomb, metheor.transform);
            });

        float map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }
    }

    public void PlayerDead()
    {
        if (state == ShootGameState.dead) return;

        state = ShootGameState.dead;
    }

    private void ChangeStatus(ShootGameState _state)
    {
        if (state == _state) return;

        state = _state;
        switch(state) {
            case ShootGameState.ready :
                joystick.vecNormal = Vector3.zero;
                stageFinished = 0;
                currentStagePlaying = -1;
                if(EndScoreCtrl.Instance.GetHighScore(GameType.shoot) < 200) hand.Show();
                else hand.gameObject.SetActive(false);
                break;
            case ShootGameState.playing:
                CreateMetheors();
                CreateEnemyAtRandomPos();
                CreateEnemyAtPlayerInCircle();
                CreateEnemyInLine();
                CreateEnemyInSpiral();
                CreateItem();
                bullet_Manager.StartSpawnBulletTimer();
                stageFinished = 0;
                currentStagePlaying = -1;
                if(hand.gameObject.activeSelf) hand.Hide();
                break;
            case ShootGameState.dead:
                joystick.Reset();
                break;

        }
    }

    public void GetShield()
    {
        if (shield != null) return;
        shield = fXManager.CreateFX(FXType.shield, player.transform).GetComponent<FX>();
        shield.gameObject.transform.SetParent(player.transform, true);
        shield.transform.localPosition = Vector3.zero;
    }

    private void DestroyShield()
    {
        if (shield == null) return;
        fXManager.CreateFX(FXType.shield_pop, shield.gameObject.transform);
        fXManager.CreateFX(FXType.Bomb, shield.gameObject.transform);
        fXManager.KillFX(shield);
        audioCtrl.PlaySFXbyTag(SFX_tag.shiealdPop);
        shield = null;
    }

    public void GetAttack()
    {
        if (state != ShootGameState.playing) return;

        if (shield != null)
        {
            DestroyShield();
            return;
        }

        //dead
        FXManager.Instance.CreateFX(FXType.deadExplosion, player);
        state = ShootGameState.dead;
        enemy_Manager.GameOver();
        face.SetTrigger("idle");
        islandSizeCtrl.CloseIsland();
        EndScoreCtrl.Instance.ShowScore(score.GetScore(), GameType.shoot) ;
        joystick.gameObject.SetActive(false);
    }

    public void SetSpinMode(float duration)
    {
        spinMode = true;
        spinTime = Time.time + duration;
    }

    public void PreLoad()
    {
        adj_transition_notch.SetActive(false);
        EndScoreCtrl.Instance.gameObject.SetActive(false);
        RestartGame();
        state = ShootGameState.dead;
        DOTween.Kill(player.transform);
        player.transform.position = loadPosition.position;
        joystick.gameObject.SetActive(false);
        Greetings();
        
        createEnemyInCircle.Init(1000, 0, 0);
        createEnemyRandomPos.Init(1100, 0, 0);
        createMetheor.Init(1200,0,0);
        createEnemyInLine.Init(1300,0,0);
        createEnemyInSpira.Init(1400,0,0);
        createItem.Init(3700,1,1, 0.5f);
        hand.gameObject.SetActive(false);
    }

    async Task Greetings()
    {
        DOTween.Kill(islandSizeCtrl.GetComponent<RectTransform>());
        islandSizeCtrl.OpenIsland();
        face.SetTrigger("idle");
        await Task.Delay(1000);
        islandSizeCtrl.CloseIsland();
    }

    public void ReadyToPlay()
    {
        player.DOMove(startPosition.position, 2f)
            .SetEase(Ease.OutQuart);
        player.DOLocalRotate(startPosition.localEulerAngles, 2f)
            .SetEase(Ease.OutQuart)
            .OnComplete(()=> {
                ChangeStatus(ShootGameState.ready);
                joystick.gameObject.SetActive(true);
            });
    }

    public void ClearGame()
    {
        EndScoreCtrl.Instance.HideScore();
        ChangeStatus(ShootGameState.dead);
        gameObject.SetActive(false);
    }

    class AutoAttackInfo
    {
        public int delay = 1000, min = 0, max = 0;
        public float probability = 0;

        public void Init(int _delay, int _min, int _max, float _probability = 1)
        {
            delay = _delay;
            min = _min;
            max = _max;
            probability = _probability;
        }
    }
}


