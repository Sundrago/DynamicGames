using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MyUtility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Core.Pet
{
    public class PetManager : SerializedMonoBehaviour
    {
        public static PetManager Instance { get; private set; }
        
        [SerializeField] public Transform petDialogueHolder;
        [SerializeField] private GameObject newPetEffectsPrefab;
        [SerializeField] public PetDialogueController petDialogueControllerPrefab;
        [SerializeField] private Dictionary<PetType, PetConfig> petConfigs;
        [SerializeField] [ReadOnly] private Dictionary<PetType, PetState> petStates = new();

        private int petCount;

        private void Awake()
        {
            Instance = this;
            StartCoroutine(LoadPetConfigs());
        }

        private void Start()
        {
            petCount = GetTotalPetCount();
            PetDialogueManager.Instance.PlayWeatherDialogue();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveData();
        }

        private void OnApplicationQuit()
        {
            SaveData();
        }

        public int GetTotalPetCount()
        {
            petCount = 0;
            foreach (PetType _type in Enum.GetValues(typeof(PetType)))
                if (GetPetState(_type).count > 0)
                    petCount += 1;
            return petCount;
        }

        public PetConfig GetPetDataByType(PetType type)
        {
            if (petConfigs.ContainsKey(type)) return petConfigs[type];
            return null;
        }

        public int GetPetConfigCount()
        {
            return petConfigs.Count;
        }

        public List<PetConfig> GetActivePetConfigs()
        {
            var pets = new List<PetConfig>();
            foreach (var entry in petConfigs)
                if (entry.Value.obj.activeSelf)
                    pets.Add(entry.Value);

            return pets;
        }

        public PetConfig GetRandomPetConfig(int idx = -1)
        {
            if (idx == -1) idx = Random.Range(0, petConfigs.Count);
            return petConfigs.ElementAt(idx).Value;
        }

        public string GetPetAge(PetType _type)
        {
            var birth = Converter.StringToDateTime(GetPetState(_type).birthDate);

            var age = DateTime.Now - birth;

            if (age.TotalDays >= 2f) return Math.Floor(age.TotalDays) + " days";
            if (age.TotalDays >= 1f) return "1 day";
            if (age.TotalHours >= 1f)
                return Math.Floor(age.TotalHours) + " hours";
            return "1 hour";
        }

        public int GetPetCount(PetType _type)
        {
            return GetPetState(_type).count;
        }

        public int GetPetLevel(PetType _type)
        {
            return GetPetState(_type).level;
        }

        public int GetPetExp(PetType _type)
        {
            var count = GetPetCount(_type);
            var level = GetPetLevel(_type);

            var totalExpSpan = 0;
            for (var i = 0; i < level; i++) totalExpSpan += 5 * i;

            return count - totalExpSpan;
        }

        public bool PetLevelUP(PetType _type)
        {
            var exp = GetPetExp(_type);
            var level = GetPetLevel(_type);

            if (exp >= level * 5)
            {
                GetPetState(_type).level += 1;
                return true;
            }

            return false;
        }

        public void InstantiateNewPetFX(PetType _type)
        {
            var fx = Instantiate(newPetEffectsPrefab, GetPetDataByType(_type).obj.transform, true);
            fx.transform.localPosition = Vector3.zero;
            fx.SetActive(true);
            DOVirtual.DelayedCall(20, () => { Destroy(fx); });

            var pets = GetActivePetConfigs();

            foreach (var pet in pets)
            {
                var thres = pets.Count < 3 ? 1 : 3f / pets.Count;
                if (Random.Range(0f, 1f) < thres)
                    pet.component.OnNewFriend();
            }

            GetPetDataByType(_type).obj.GetComponent<PetController>().OnIdle();
        }

        public Dictionary<PetType, PetConfig> GetPetDatas()
        {
            return petConfigs;
        }

        public void AddPetCountByType(PetType _type)
        {
            var data = GetPetState(_type);
            data.count += 1;
            if (data.count == 1)
            {
                data.level = 1;
                data.birthDate = Converter.DateTimeToString(DateTime.Now);
            }

            StartCoroutine(LoadPetConfigs());
        }

        private PetState GetPetState(PetType _type)
        {
            if (petStates.ContainsKey(_type)) return petStates[_type];

            if (PlayerPrefs.HasKey("PetSaveData_" + _type))
            {
                var data = PlayerPrefs.GetString("PetSaveData_" + _type);
                petStates.Add(_type, JsonUtility.FromJson<PetState>(data));
            }
            else
            {
                var data = new PetState();
                data.count = PlayerPrefs.GetInt("PetCount_" + _type, 0);
                data.x = 0f;
                data.y = -1.25f;
                data.level = PlayerPrefs.GetInt("PetLevel_" + _type, 0);
                data.birthDate =
                    PlayerPrefs.GetString("PetBirthDate_" + _type, Converter.DateTimeToString(DateTime.Now));
                data.isActive = true;

                petStates.Add(_type, data);
            }

            return petStates[_type];
        }

        private void SaveData()
        {
            foreach (var entry in petConfigs)
            {
                var petdata = entry.Value;
                var data = GetPetState(petdata.type);
                data.x = petdata.obj.transform.position.x;
                data.y = petdata.obj.transform.position.y;
                PlayerPrefs.SetString("PetSaveData_" + petdata.type, JsonUtility.ToJson(data));
            }

            PlayerPrefs.Save();
        }

        private IEnumerator LoadPetConfigs()
        {
            foreach (var entry in petConfigs)
            {
                var petdata = entry.Value;
                var data = GetPetState(petdata.type);
                if (data.count > 0)
                {
                    petdata.obj.transform.position = new Vector3(data.x, data.y, petdata.obj.transform.position.z);
                    petdata.obj.SetActive(true);
                }
                else
                {
                    petdata.obj.SetActive(false);
                }
            }

            yield return null;
        }

        public void RemoveData()
        {
            petStates = new Dictionary<PetType, PetState>();
            LoadPetConfigs();
        }
#if UNITY_EDITOR
        [Button]
        private void ImportPetsFromScene()
        {
            var pets = GameObject.FindGameObjectsWithTag("PET");
            Debug.Log(pets.Length);
            foreach (PetType type in Enum.GetValues(typeof(PetType)))
                if (!petConfigs.ContainsKey(type))
                    foreach (var obj in pets)
                        if (obj.GetComponent<PetController>().type == type)
                        {
                            petConfigs.Add(type, new PetConfig());
                            petConfigs[type].obj = obj;
                            break;
                        }
        }

        [Button]
        private void UpdatePetDatas()
        {
            foreach (var entry in petConfigs)
            {
                var data = entry.Value;
                if (entry.Value.obj == null) continue;
                data.component = data.obj.GetComponent<PetController>();
                data.type = data.component.type;
                data.image = data.component.spriteRenderer.sprite;
            }
        }
    }
#endif

    public class PetState
    {
        public string birthDate;
        public int count;
        public bool isActive;
        public int level;
        public float x;
        public float y;
    }

    [Serializable]
    public class PetConfig
    {
        public GameObject obj;
        [ReadOnly] public PetType type;
        [ReadOnly] public PetController component;
        [ReadOnly] public Sprite image;
    }

    public enum PetType
    {
        Fluffy,
        Cocoa,
        Brownie,
        Foxy,
        MrPinchy,
        Krabs,
        Dash,
        Batcat,
        Butter,
        Matata,
        Gcat,
        Kune,
        Lo,
        Ve,
        Default
    } //Caramel
}