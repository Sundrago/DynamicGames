using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] EndScoreCtrl endScore;
    [SerializeField] ShootScoreManager score;
    [SerializeField] Transform startPosition;
    [SerializeField] SFXCTRL sfx;

    public static Shoot_GameManager Instacne;
    
    private AutoAttackInfo createEnemyInCircle = new AutoAttackInfo();
    private AutoAttackInfo createEnemyRandomPos = new AutoAttackInfo();
    private AutoAttackInfo createMetheor = new AutoAttackInfo();
    private AutoAttackInfo createEnemyInSpira = new AutoAttackInfo();
    private AutoAttackInfo createEnemyInLine = new AutoAttackInfo();

    public enum ShootGameState { ready, dead, playing }

    [ReadOnly]
    public FX shield = null;

    public ShootGameState state;

    private void Awake()
    {
        Instacne = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        SetShootGameStatus(ShootGameState.ready);
        sfx.PlayBGM(5);
        stage = 0;
        createEnemyInCircle.Init(1000, 0, 0);
        createEnemyRandomPos.Init(1000, 0, 0);
        createMetheor.Init(1000,0,0);
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
        if (joystick.vecNormal != Vector3.zero) SetShootGameStatus(ShootGameState.playing);
    }

    private void Update_Playing()
    {
        if(Time.frameCount % 20 != 0) return;

        int myScofre = score.GetScore();

        if (myScofre < 5) SetStage(0);
        else if (myScofre < 30) SetStage(1);
        else if (myScofre < 80) SetStage(2);
        else if (myScofre < 150) SetStage(3);
        else if (myScofre < 250) SetStage(4);
        else if (myScofre < 400) SetStage(5);
        else if (myScofre < 600) SetStage(6);
        else if (myScofre < 900) SetStage(7);
    }

    public int stage;
    
    async Task SetStage(int _stage)
    {
        if(state != ShootGameState.playing) return;
        if(stage >= _stage) return;

        stage = _stage;
        int rnd;

        switch (stage)
        {
            case 0:
                createEnemyInCircle.Init(1000, 0, 0);
                createEnemyRandomPos.Init(1100, 0, 0);
                createMetheor.Init(1200,0,0);
                createEnemyInLine.Init(1300, 0, 0);
                createEnemyInSpira.Init(1400, 0, 0);
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
                createEnemyRandomPos.Init(4000, 0, 2);
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
                createEnemyRandomPos.Init(3900, 1, 3);
                createMetheor.Init(1000,0,0);
                break;
            case 3:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry01");
                await Task.Delay(1000);
                CreateMetheor();
                await Task.Delay(1000);
                
                rnd = Random.Range(0, 4);
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
                    case 3:
                        await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 20, 1.35f, 30, 0.75f);
                        break;
                }
                await Task.Delay(1000);
                createEnemyInCircle.Init(10000, 0, 4);
                createEnemyRandomPos.Init(3900, 1, 4);
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
                        await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.7f, 25, 1.35f, 30, 0.75f);
                        break;
                }
                await Task.Delay(1000);
                createEnemyInCircle.Init(10000, 2, 6);
                createEnemyRandomPos.Init(3900, 1, 4);
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
                createEnemyRandomPos.Init(3900, 1, 4);
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
                createEnemyRandomPos.Init(3900, 1, 4);
                createMetheor.Init(8000,0,2);
                createEnemyInLine.Init(7500, 5, 10, 0.45f);
                break;
            case 7:
                islandSizeCtrl.OpenIsland();
                await Task.Delay(1000);
                face.SetTrigger("angry02");
                enemy_Manager.SpawnEnemyInLineY(10);
                await Task.Delay(500);
                CreateMetheor();
                await Task.Delay(500);
                CreateMetheor();
                enemy_Manager.SpawnEnemyInCircle(0.9f, Random.Range(5, 8));
                await Task.Delay(500);
                
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
                        await enemy_Manager.SpawnEnemyInSpiral(0.5f, 1.8f, 30, 1.75f, 25, 0.65f);
                        break;
                }
                await Task.Delay(1000);
                enemy_Manager.SpawnEnemyInLineY(10);
                
                createEnemyInCircle.Init(6100, 5, 10);
                createEnemyRandomPos.Init(1900, 0, 4);
                createMetheor.Init(4000,0,2);
                createEnemyInLine.Init(7500, 5, 10, 0.5f);
                break;
        }
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
        if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
        {
            await enemy_Manager.SpawnEnemyInSpiral(0.6f * Random.Range(0.9f, 1.1f),
                1.5f * Random.Range(0.85f, 1.3f), Random.Range(info.min, info.max+ 1)
                , 1.5f * Random.Range(0.7f, 1.3f), 35, 0.6f * Random.Range(0.8f, 1.2f));
        }

        await Task.Delay(info.delay);
        CreateEnemyInSpiral();
    }
    
    
    public void RestartGame()
    {
        endScore.HideScore();
        DestroyShield();

        face.SetTrigger("idle");
        islandSizeCtrl.CloseIsland();
        score.ResetScore();

        player.DOMove(startPosition.position, 0.5f)
            .SetEase(Ease.InOutCubic);
        player.DOLocalRotate(startPosition.localEulerAngles, 0.5f)
            .SetEase(Ease.InOutCubic)
            .OnComplete(()=> {
                SetShootGameStatus(ShootGameState.ready);
                joystick.gameObject.SetActive(true);
            });

        enemy_Manager.KillAll();
        shoot_Item.KillAll();
        bullet_Manager.Restart();
        stage = 0;
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

    private void SetShootGameStatus(ShootGameState _state)
    {
        if (state == _state) return;

        state = _state;
        switch(state) {
            case ShootGameState.ready :
                joystick.vecNormal = Vector3.zero;
                break;
            case ShootGameState.playing:
                CreateMetheors();
                CreateEnemyAtRandomPos();
                CreateEnemyAtPlayerInCircle();
                CreateEnemyInLine();
                CreateEnemyInSpiral();
                bullet_Manager.StartSpawnBulletTimer();
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
        fXManager.KillFX(shield);
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
        endScore.ShowScore(score.GetScore(), GameType.shoot) ;
        joystick.gameObject.SetActive(false);
    }

    class AutoAttackInfo
    {
        public int delay, min, max;
        public float probability;

        public void Init(int _delay, int _min, int _max, float _probability = 1)
        {
            delay = _delay;
            min = _min;
            max = _max;
            probability = _probability;
        }
    }
}


