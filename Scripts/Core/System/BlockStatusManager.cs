using System;
using System.Collections;
using System.Collections.Generic;
using Core.Main;
using Core.Pet;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

public class BlockStatusManager : MonoBehaviour
{
    public static BlockStatusManager Instance;
    public GameObject FX_appear_prefab, FX_explode_prefab, FX_explode_B_prefab;
    public enum BlockStatus
    {
        locked, unrevealed, unlocked, destroyed
    }

    public enum BlockType
    {
        shoot, jump, land, build, leaderboard, gacha, friends, tutorial, locale, review, tv
    }

    [SerializeField, TableList] private List<BlockStatusData> BlockStatusDatas = new List<BlockStatusData>();
    [SerializeField] private Transform appearPosY;
    [SerializeField] private PetInfo_UI petinfo;
    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        FirstTimeInit();
    }

    public bool IsAllGameLocked()
    {
        foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
        {
            BlockStatusData data = GetDataByType(GetBlockTypeByGameType(gameType));
            if (data.status == BlockStatus.unlocked) return false;
        }

        return true;
    }
    
    public int GetUnlockedGame()
    {
        int count = 0;
        foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
        {
            BlockStatusData data = GetDataByType(GetBlockTypeByGameType(gameType));
            if (data.status == BlockStatus.unlocked) count += 1;
        }

        return count;
    }
    
    
    [Button]
    private void AddItemToList()
    {
        foreach (BlockType blockType in Enum.GetValues(typeof(BlockType)))
        {
            bool flag = false;
            foreach (BlockStatusData data in BlockStatusDatas)
            {
                if (blockType == data.type) flag = true;
            }
            
            if (!flag)
            {
                BlockStatusData data = new BlockStatusData();
                data.type = blockType;
                BlockStatusDatas.Add(data);
            }
        }

       
    }

    private void FirstTimeInit()
    {
        foreach (BlockStatusData data in BlockStatusDatas)
        {
            data.obj.GetComponent<BlockDragHandler>().Start();
        }

        if (!PlayerPrefs.HasKey("BlockInited"))
        {
            //Check If First Load
            int count = 0;
            foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
            {
                BlockStatusData data = GetDataByType(GetBlockTypeByGameType(gameType));
                int highScore = PlayerPrefs.GetInt("highscore_" + gameType.ToString());
                if (highScore > 0)
                {
                    data.status = BlockStatus.unlocked;
                    count += 1;
                }
                else
                {
                    data.status = (BlockStatus)PlayerPrefs.GetInt("BlockData_" + data.type);
                }
            }
            
            //New User
            print("new user");
            if (count == 0)
            {
                foreach (BlockStatusData data in BlockStatusDatas)
                {
                    data.status = (data.type == BlockType.tutorial) ? BlockStatus.unlocked : BlockStatus.unrevealed;
                }

                RevealBlock(BlockType.tutorial);
            }
            //old user
            else
            {
                print("old user");
                foreach (BlockStatusData data in BlockStatusDatas)
                {
                    data.status = (data.type == BlockType.tutorial) ? BlockStatus.destroyed : BlockStatus.unlocked;
                }
            }
            
            SaveData();
            PlayerPrefs.SetInt("BlockInited", 1);
        }
        
        LoadData();
    }
    private void LoadData()
    {
        foreach (BlockStatusData data in BlockStatusDatas)
        {
            data.status = (BlockStatus)PlayerPrefs.GetInt("BlockData_" + data.type);
            
            print("LoadData : " + data.type + "-" + data.status);
            
            switch (data.status)
            {
                case BlockStatus.locked:
                    data.obj.isLocked = true;
                    break;
                case BlockStatus.unlocked:
                    data.obj.isLocked = false;
                    break;
                case BlockStatus.unrevealed:
                    data.obj.isLocked = true;
                    data.obj.gameObject.SetActive(false);
                    break;
                case BlockStatus.destroyed:
                    data.obj.gameObject.SetActive(false);
                    break;
            }
            
            data.obj.Init();
        }
    }

    public BlockType GetBlockTypeByGameType(GameType gameType)
    {
        foreach (BlockType blockType in Enum.GetValues(typeof(BlockType)))
        {
            if (blockType.ToString() == gameType.ToString())
                return blockType;
        }

        Debug.LogWarning("[GetDataByType] No data found for GameType : " + gameType);
        return BlockType.build;
    }
    
    private BlockStatusData GetDataByType(BlockType blockType)
    {
        foreach (BlockStatusData data in BlockStatusDatas)
        {
            if (data.type == blockType)
                return data;
        }

        Debug.LogWarning("[GetDataByType] No data found for GameType : " + blockType);
        return null;
    }

    public void SetBlockStatus(BlockType blockType, BlockStatus status)
    {
        print("SetBlockStatus : " + blockType + "-" + status);
        BlockStatusData data = GetDataByType(blockType);
        data.status = status;
        SaveData();
    }

    private void SaveData()
    {
        print("block data saved");
        foreach (BlockStatusData data in BlockStatusDatas)
        {
            PlayerPrefs.SetInt("BlockData_" + data.type, (int)data.status);
        }
        PlayerPrefs.Save();
    }

    // private void OnApplicationPause(bool pauseStatus)
    // {
    //     SaveData();
    // }
    //
    // private void OnApplicationQuit()
    // {
    //     SaveData();
    // }

    [Button]
    public void RevealBlock(BlockType blockType)
    {
        BlockStatusData data = GetDataByType(blockType);
        Rigidbody2D rigidbody2D = data.obj.GetComponent<Rigidbody2D>();
        BlockDragHandler blockDragHandler = data.obj.GetComponent<BlockDragHandler>();
        SquareBlockCtrl squareBlockCtrl = data.obj.GetComponent<SquareBlockCtrl>();
        
        SetBlockStatus(blockType, BlockStatus.locked);

        Vector3 defaultPos = blockDragHandler.initialPos;
        defaultPos.y = appearPosY.position.y;

        AudioManager.Instance.PlaySFXbyTag(SfxTag.sparkle);
        data.obj.transform.position = defaultPos;
        data.obj.transform.eulerAngles = Vector3.zero;
        rigidbody2D.isKinematic = true;
        blockDragHandler.InstantiateEnergyFX();
        squareBlockCtrl.Reveal();
        data.obj.gameObject.SetActive(true);
        data.obj.transform.DOShakePosition(1f, 15, 20)
            .SetEase(Ease.InOutCubic);

        GameObject fx = Instantiate(FX_appear_prefab);
        fx.transform.SetParent(data.obj.gameObject.transform, true);
        fx.transform.localPosition = Vector3.zero;
        
        fx.SetActive(true);

        DOVirtual.DelayedCall(1.5f, () =>
        {
            AudioManager.Instance.PlaySFXbyTag(SfxTag.cube_fall);
            blockDragHandler.Deactivate();
            rigidbody2D.velocity = Vector2.zero;
        });
    }
    
    [Button]
    public void ExplodeBlock(BlockType blockType)
    {
        BlockStatusData data = GetDataByType(blockType);
        Rigidbody2D rigidbody2D = data.obj.GetComponent<Rigidbody2D>();
        BlockDragHandler blockDragHandler = data.obj.GetComponent<BlockDragHandler>();
        SquareBlockCtrl squareBlockCtrl = data.obj.GetComponent<SquareBlockCtrl>();

        //ChangeAndSaveStatus(reviewStatus.Revealed);
        AudioManager.Instance.PlaySFXbyTag(SfxTag.block_explode);
        
        GameObject fx = Instantiate(FX_explode_prefab);
        fx.transform.SetParent(data.obj.gameObject.transform, true);
        fx.transform.localPosition = Vector3.zero;
        fx.SetActive(true);
        
        rigidbody2D.isKinematic = true;
        squareBlockCtrl.Hide();
        data.obj.gameObject.SetActive(true);
        data.obj.transform.DOShakePosition(0.5f, 10, 10, 90f, false, false)
            .SetEase(Ease.Linear);
        data.obj.transform.DOShakePosition(0.5f, 15, 15, 90f, false, false)
            .SetEase(Ease.Linear).SetDelay(0.5f);
        data.obj.transform.DOShakePosition(0.5f, 20, 20, 90f, false, false)
            .SetEase(Ease.Linear).SetDelay(1f);
        data.obj.transform.DOShakePosition(0.5f, 20, 30, 90f, false, false)
            .SetEase(Ease.Linear).SetDelay(1.5f);
        data.obj.transform.DOScale(Vector3.one * 15f, 1.95f).SetEase(Ease.InCirc)
            .OnComplete(() =>
            {
                GameObject fx2 = Instantiate(FX_explode_B_prefab, data.obj.gameObject.transform.parent.transform);
                fx2.transform.position = data.obj.transform.position;
                fx2.SetActive(true);
                data.obj.gameObject.SetActive(false);
                blockDragHandler.Deactivate();
                print("hide oncomplete");
                SetBlockStatus(data.type, BlockStatus.destroyed);
            });
    }

    [Serializable]
    public class BlockStatusData
    {
        public BlockType type;
        public SquareBlockCtrl obj;
        [ReadOnly] public BlockStatus status;
    }

    public void PetDrop(Vector3 pos, Pet pet)
    {
        float distMin = float.MaxValue;
        BlockStatusData minData = null;

        foreach (BlockStatusData data in BlockStatusDatas)
        {
            if(!data.obj.gameObject.activeSelf) continue;
            
            float dist = Vector2.Distance(data.obj.gameObject.transform.position, pos);
            if (dist < distMin)
            {
                distMin = dist;
                minData = data;
            }
        }

        if(distMin > 0.3f) return;
        
        if (!minData.obj.isNotGame && !minData.obj.isLocked)
        {
            GameObject obj = minData.obj.gameObject;
            MainCanvas.Instance.Offall(obj);
            PetInGameManager.Instance.PetSelected(minData, pet);
            minData.obj.blockDragHandler.OnButtonClicked();
            AudioManager.Instance.PlaySFXbyTag(SfxTag.playWithPet);
        }
        else
        {
            if (minData.type == BlockType.friends)
            {
                petinfo.ShowPanel(pet.type);
                AudioManager.Instance.PlaySFXbyTag(SfxTag.playWithPet);

            }
        }

        TutorialManager.Instancee.PetDrop(minData.type);
    }
    
    public GameType GetGameType(BlockType blockType)
    {
        foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
        {
            if (gameType.ToString() == blockType.ToString()) return gameType;
        }

        return GameType.Null;
    }
}
