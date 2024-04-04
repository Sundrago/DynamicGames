using System;
using System.Collections.Generic;
using DG.Tweening;
using DynamicGames.MainPage;
using DynamicGames.MiniGames;
using DynamicGames.Pet;
using DynamicGames.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace DynamicGames.System
{
    /// <summary>
    ///     Manages the status and behavior of button blocks in the main page.
    /// </summary>
    public class BlockStatusManager : MonoBehaviour
    {
        public enum BlockStatus
        {
            Locked,
            Unrevealed,
            Unlocked,
            Destroyed
        }

        public enum BlockType
        {
            shoot,
            jump,
            land,
            build,
            leaderboard,
            gacha,
            friends,
            tutorial,
            locale,
            review,
            tv
        }

        [Header("Managers and Controllers")] 
        [SerializeField] [TableList] private List<BlockStatusData> BlockStatusDatas = new();
        [SerializeField] private PetInfoPanelManager petInfoPanelManager;

        [Header("Game Components")] [SerializeField]
        private Transform appearPosY;
        public GameObject FX_appear_prefab, FX_explode_prefab, FX_explode_B_prefab;

        public static BlockStatusManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            FirstTimeInitialization();
        }

        public bool IsAllGameLocked()
        {
            foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
            {
                var data = GetDataByType(GetBlockTypeByGameType(gameType));
                if (data.status == BlockStatus.Unlocked) return false;
            }

            return true;
        }

        public int CountUnlockedGames()
        {
            var count = 0;
            foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
            {
                var data = GetDataByType(GetBlockTypeByGameType(gameType));
                if (data.status == BlockStatus.Unlocked) count += 1;
            }

            return count;
        }


        [Button]
        private void AddItemToList()
        {
            foreach (BlockType blockType in Enum.GetValues(typeof(BlockType)))
            {
                var flag = false;
                foreach (var data in BlockStatusDatas)
                    if (blockType == data.type)
                        flag = true;

                if (!flag)
                {
                    var data = new BlockStatusData();
                    data.type = blockType;
                    BlockStatusDatas.Add(data);
                }
            }
        }

        private void FirstTimeInitialization()
        {
            foreach (var data in BlockStatusDatas) data.obj.GetComponent<BlockDragHandler>().Start();

            if (!PlayerPrefs.HasKey("BlockInited"))
            {
                //Check If First Load
                var count = 0;
                foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
                {
                    var data = GetDataByType(GetBlockTypeByGameType(gameType));
                    var highScore = PlayerPrefs.GetInt("highscore_" + gameType);
                    if (highScore > 0)
                    {
                        data.status = BlockStatus.Unlocked;
                        count += 1;
                    }
                    else
                    {
                        data.status = (BlockStatus)PlayerPrefs.GetInt("BlockData_" + data.type);
                    }
                }

                //New User
                if (count == 0)
                {
                    foreach (var data in BlockStatusDatas)
                        data.status = data.type == BlockType.tutorial ? BlockStatus.Unlocked : BlockStatus.Unrevealed;

                    RevealBlock(BlockType.tutorial);
                }
                //old user
                else
                {
                    foreach (var data in BlockStatusDatas)
                        data.status = data.type == BlockType.tutorial ? BlockStatus.Destroyed : BlockStatus.Unlocked;
                }

                SaveData();
                PlayerPrefs.SetInt("BlockInited", 1);
            }

            LoadData();
        }

        private void LoadData()
        {
            foreach (var data in BlockStatusDatas)
            {
                data.status = (BlockStatus)PlayerPrefs.GetInt("BlockData_" + data.type);
                switch (data.status)
                {
                    case BlockStatus.Locked:
                        data.obj.isLocked = true;
                        break;
                    case BlockStatus.Unlocked:
                        data.obj.isLocked = false;
                        break;
                    case BlockStatus.Unrevealed:
                        data.obj.isLocked = true;
                        data.obj.gameObject.SetActive(false);
                        break;
                    case BlockStatus.Destroyed:
                        data.obj.gameObject.SetActive(false);
                        break;
                }

                data.obj.Init();
            }
        }

        public BlockType GetBlockTypeByGameType(GameType gameType)
        {
            foreach (BlockType blockType in Enum.GetValues(typeof(BlockType)))
                if (blockType.ToString() == gameType.ToString())
                    return blockType;

            Debug.LogWarning("[GetDataByType] No data found for GameType : " + gameType);
            return BlockType.build;
        }

        private BlockStatusData GetDataByType(BlockType blockType)
        {
            foreach (var data in BlockStatusDatas)
                if (data.type == blockType)
                    return data;

            Debug.LogWarning("[GetDataByType] No data found for GameType : " + blockType);
            return null;
        }

        public void SetBlockStatus(BlockType blockType, BlockStatus status)
        {
            print("SetBlockStatus : " + blockType + "-" + status);
            var data = GetDataByType(blockType);
            data.status = status;
            SaveData();
        }

        private void SaveData()
        {
            print("block data saved");
            foreach (var data in BlockStatusDatas) PlayerPrefs.SetInt("BlockData_" + data.type, (int)data.status);
            PlayerPrefs.Save();
        }

        [Button]
        public void RevealBlock(BlockType blockType)
        {
            var data = GetDataByType(blockType);
            var rigidbody2D = data.obj.GetComponent<Rigidbody2D>();
            var blockDragHandler = data.obj.GetComponent<BlockDragHandler>();
            var squareBlockCtrl = data.obj.GetComponent<SquareBlockCtrl>();

            SetBlockStatus(blockType, BlockStatus.Locked);

            var defaultPos = blockDragHandler.initialPos;
            defaultPos.y = appearPosY.position.y;

            AudioManager.Instance.PlaySfxByTag(SfxTag.Sparkle);
            data.obj.transform.position = defaultPos;
            data.obj.transform.eulerAngles = Vector3.zero;
            rigidbody2D.isKinematic = true;
            blockDragHandler.InstantiateEnergyFX();
            squareBlockCtrl.Reveal();
            data.obj.gameObject.SetActive(true);
            data.obj.transform.DOShakePosition(1f, 15, 20)
                .SetEase(Ease.InOutCubic);

            var fx = Instantiate(FX_appear_prefab);
            fx.transform.SetParent(data.obj.gameObject.transform, true);
            fx.transform.localPosition = Vector3.zero;

            fx.SetActive(true);

            DOVirtual.DelayedCall(1.5f, () =>
            {
                AudioManager.Instance.PlaySfxByTag(SfxTag.CubeFall);
                blockDragHandler.Deactivate();
                rigidbody2D.velocity = Vector2.zero;
            });
        }

        [Button]
        public void ExplodeBlock(BlockType blockType)
        {
            var data = GetDataByType(blockType);
            var rigidbody2D = data.obj.GetComponent<Rigidbody2D>();
            var blockDragHandler = data.obj.GetComponent<BlockDragHandler>();
            var squareBlockCtrl = data.obj.GetComponent<SquareBlockCtrl>();

            AudioManager.Instance.PlaySfxByTag(SfxTag.BlockExplode);

            var fx = Instantiate(FX_explode_prefab);
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
                    var fx2 = Instantiate(FX_explode_B_prefab, data.obj.gameObject.transform.parent.transform);
                    fx2.transform.position = data.obj.transform.position;
                    fx2.SetActive(true);
                    data.obj.gameObject.SetActive(false);
                    blockDragHandler.Deactivate();
                    print("hide oncomplete");
                    SetBlockStatus(data.type, BlockStatus.Destroyed);
                });
        }

        public void HandlePetDrop(Vector3 pos, PetObject petObject)
        {
            var distMin = float.MaxValue;
            BlockStatusData minData = null;

            foreach (var data in BlockStatusDatas)
            {
                if (!data.obj.gameObject.activeSelf) continue;

                var dist = Vector2.Distance(data.obj.gameObject.transform.position, pos);
                if (dist < distMin)
                {
                    distMin = dist;
                    minData = data;
                }
            }

            if (distMin > 0.3f) return;

            if (!minData.obj.isNotGame && !minData.obj.isLocked)
            {
                var obj = minData.obj.gameObject;
                MainPage.MainPage.Instance.DeactivateAllBlocksExcept(obj);
                PetInGameManager.Instance.PetSelected(minData, petObject);
                minData.obj.blockDragHandler.OnButtonClicked();
                AudioManager.Instance.PlaySfxByTag(SfxTag.PlayWithPet);
            }
            else
            {
                if (minData.type == BlockType.friends)
                {
                    petInfoPanelManager.ShowPanel(petObject.type);
                    AudioManager.Instance.PlaySfxByTag(SfxTag.PlayWithPet);
                }
            }

            TutorialManager.Instancee.PetDrop(minData.type);
        }

        public GameType GetGameType(BlockType blockType)
        {
            foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
                if (gameType.ToString() == blockType.ToString())
                    return gameType;

            return GameType.@null;
        }

        [Serializable]
        public class BlockStatusData
        {
            public BlockType type;
            public SquareBlockCtrl obj;
            [ReadOnly] public BlockStatus status;
        }
    }
}