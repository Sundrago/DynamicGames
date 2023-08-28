using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class Shoot_GameManager : MonoBehaviour
{
    [SerializeField] Animator door_left, door_right;
    [SerializeField] Shoot_Enemy_Manager enemy_Manager;
    [SerializeField] Transform player, island;
    [SerializeField] FXManager fXManager;
    // Start is called before the first frame update
    void Start()
    {
        SpawnOnLeft(5);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 120 == 0) CreateMeteo();
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

        float ydiff = Mathf.Abs(player.transform.position.y - island.transform.position.y);
        float xdiff = map(ydiff, 0, 5, 0.2f, 1.5f);
        path[1] = Vector3.Lerp(path[0], path[2], 0.5f);
        path[1].x = player.position.x < 0 ? -xdiff : xdiff;

        GameObject meteo = fXManager.CreateFX(FXType.ShadowMissile, island);
        meteo.transform.DOPath(path, Random.Range(1.8f, 2.2f), PathType.CatmullRom, PathMode.TopDown2D, 1, Color.red)
            .SetEase(Ease.OutQuart)
            .OnComplete(()=> {
                //fXManager.KillFX(meteo.GetComponent<FX>());
                print("bobmd");
                fXManager.CreateFX(FXType.ShadowBomb, meteo.transform);
            });

        float map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }
    }
}


