using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class Shoot_GameManager : MonoBehaviour
{
    [SerializeField] Animator door_left, door_right;
    [SerializeField] Shoot_Enemy_Manager enemy_Manager;
    [SerializeField] Shoot_Bullet_Manager bullet_Manager;
    [SerializeField] public Transform player, island;
    [SerializeField] FXManager fXManager;
    [SerializeField] Shoot_joystick joystick;

    [SerializeField] Animator face;
    [SerializeField] IslandSizeCtrl islandSizeCtrl;
    [SerializeField] EndScoreCtrl endScore;
    [SerializeField] ShootScoreManager score;
    [SerializeField] Transform startPosition;

    public static Shoot_GameManager Instacne;

    public enum ShootGameState { ready, dead, playing }
    public FX shield = null;

    public ShootGameState state;

    private void Awake()
    {
        Instacne = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetShootGameStatus(ShootGameState.playing);
        SpawnOnLeft(5);
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.frameCount % 120 == 0)
        {
            if (state == ShootGameState.playing)
                CreateMeteo();
        }
    }

    void Init()
    {
        DestroyShield();

        face.SetTrigger("idle");
        islandSizeCtrl.CloseIsland();
        score.ResetScore();

        player.DOMove(startPosition.position, 0.5f)
            .SetEase(Ease.InOutCubic);
        player.DOLocalRotate(startPosition.localEulerAngles, 0.5f)
            .SetEase(Ease.InOutCubic)
            .OnComplete(()=> {
                state = ShootGameState.ready;
            });
    }

    async Task SpawnOnLeft(int count)
    {
        door_left.SetTrigger("open");
        await Task.Delay(1000);
        for(int i = 0; i<count; i++)
        {
            enemy_Manager.SpawnOnIsland(180, -1.5f, 0f);
            await Task.Delay(1000);
        }
        door_left.SetTrigger("close");
        SpawnOnRight(5);
    }

    async Task SpawnOnRight(int count)
    {
        door_right.SetTrigger("open");
        await Task.Delay(1000);
        for (int i = 0; i < count; i++)
        {
            enemy_Manager.SpawnOnIsland(0, 1.5f, 0f);
            await Task.Delay(1000);
        }
        door_right.SetTrigger("close");
        SpawnOnLeft(5);
    }

    public void CreateMeteo()
    {
        Vector3[] path = new Vector3[3];
        path[0] = island.transform.position;
        path[2] = player.transform.position;
        path[0].z = path[2].z;

        float ydiff = Mathf.Abs(player.transform.position.y - island.transform.position.y);
        float xdiff = map(ydiff, 0, 5, 0.2f, 1.5f);
        path[1] = Vector3.Lerp(path[0], path[2], 0.5f);
        path[1].x = player.position.x < 0 ? -xdiff : xdiff;

        GameObject meteo = fXManager.CreateFX(FXType.ShadowMissile, path[0]);
        meteo.transform.DOPath(path, Random.Range(1.8f, 2.2f), PathType.CatmullRom, PathMode.TopDown2D, 1, Color.red)
            .SetEase(Ease.OutQuart)
            .OnComplete(()=> {
                //fXManager.KillFX(meteo.GetComponent<FX>());
                fXManager.CreateFX(FXType.ShadowBomb, meteo.transform);
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
                break;
            case ShootGameState.playing:
                bullet_Manager.StartSpawnBulletTimer();
                break;
            case ShootGameState.dead:
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
    }
}


