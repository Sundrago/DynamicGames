using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shoot_AI : MonoBehaviour
{
    [SerializeField] private Shoot_GameManager gm;
    [SerializeField] private Shoot_Enemy_Manager enemy_Manager;
    [SerializeField] private Shoot_item shoot_Item;
    
    private bool CreateEnemyAtRandomPos_on = false;
    private bool CreateEnemyAtPlayerInCircle_on = false;
    private bool CreateMetheors_on = false;
    private bool CreateEnemyInLine_on = false;
    private bool CreateEnemyInSpiral_on = false;
    private bool CreateItem_on = false;


    public void StartTasks()
    {
        if (!CreateEnemyAtRandomPos_on) Task.Run(CreateEnemyAtRandomPos);
        if (!CreateEnemyAtPlayerInCircle_on) Task.Run(CreateEnemyAtPlayerInCircle);
        if (!CreateMetheors_on) Task.Run(CreateMetheors);
        if (!CreateEnemyInLine_on) Task.Run(CreateEnemyInLine);
        if (!CreateEnemyInSpiral_on) Task.Run(CreateEnemyInSpiral);
        if (!CreateItem_on) Task.Run(CreateItem);
    }

    async Task CreateEnemyAtRandomPos()
    {
        if (gm.state != Shoot_GameManager.ShootGameState.playing)
        {
            CreateEnemyAtRandomPos_on = false;
            return;
        }

        CreateEnemyAtRandomPos_on = true;
        Shoot_GameManager.AutoAttackInfo info = gm.createEnemyRandomPos;
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
        if (gm.state != Shoot_GameManager.ShootGameState.playing)
        {
            CreateEnemyAtPlayerInCircle_on = false;
            return;
        }

        CreateEnemyAtPlayerInCircle_on = true;
        Shoot_GameManager.AutoAttackInfo info = gm.createEnemyInCircle;
        if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
        {
            enemy_Manager.SpawnEnemyInCircle(1f, Random.Range(info.min, info.max));
        }
        await Task.Delay(info.delay);
        CreateEnemyAtPlayerInCircle();
    }

    async Task CreateMetheors()
    {
        if (gm.state != Shoot_GameManager.ShootGameState.playing)
        {
            CreateMetheors_on = false;
            return;
        }

        CreateMetheors_on = true;
        Shoot_GameManager.AutoAttackInfo info = gm.createMetheor;
        if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
        {
            int amt = Random.Range(info.min, info.max + 1);
            for (int i = 0; i < amt; i++)
            {
                await Task.Delay(2000);
                gm.CreateMetheor();
            }
        }
        await Task.Delay(info.delay);
        CreateMetheors();
    }

    async Task CreateEnemyInLine()
    {
        if (gm.state != Shoot_GameManager.ShootGameState.playing)
        {
            CreateEnemyInLine_on = false;
            return;
        }

        CreateEnemyInLine_on = true;
        Shoot_GameManager.AutoAttackInfo info = gm.createEnemyInLine;
        if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
        {
            enemy_Manager.SpawnEnemyInLineY(Random.Range(info.min, info.max+ 1));
        }

        await Task.Delay(info.delay);
        CreateEnemyInLine();
    }

    async Task CreateEnemyInSpiral()
    {
        if (gm.state != Shoot_GameManager.ShootGameState.playing)
        {
            CreateEnemyInSpiral_on = false;
            return;
        }

        CreateEnemyInSpiral_on = true;
        Shoot_GameManager.AutoAttackInfo info = gm.createEnemyInSpira;
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
        if (gm.state !=Shoot_GameManager.ShootGameState.playing)
        {
            CreateItem_on = false;
            return;
        }
        
        Shoot_GameManager.AutoAttackInfo info = gm.createItem;
        CreateItem_on = true;
        int count = shoot_Item.items.Count;
        info.probability = (1 - 0.4f * count) * 0.85f;
        
        if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
        {
            shoot_Item.SpawnItem();
        }

        await Task.Delay(info.delay);
        CreateItem();
    }
}
