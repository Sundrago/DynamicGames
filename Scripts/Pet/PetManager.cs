using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using MyUtility;
using Firebase.Analytics;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class PetManager : SerializedMonoBehaviour
{
    // [SerializeField, TableList] public List<Petdata> petDatass;

    [SerializeField] private Dictionary<PetType, PetData> petDatas;
    
    public static PetManager Instance;

    [SerializeField] private GameObject newPetSparcle_prefab;
    [SerializeField] public PetDialogue petDialoguePrefab;
    [SerializeField] public Transform petDialogueHolder;
    [SerializeField, ReadOnly] private Dictionary<PetType, PetSaveData> petSaveDatas = new Dictionary<PetType, PetSaveData>();
    // [SerializeField] private AskForUserReview askForUserReview;
    
    private int petCount;
    
    private void Awake()
    {
        Instance = this;
        StartCoroutine(LoadData());
    }

    private void Start()
    {
        petCount = 0;
        PetDialogueManager.Instance.PlayWeatherDialogue();
    }

    public int GetTotalPetCount()
    {
        petCount = 0;
        foreach (PetType _type in Enum.GetValues((typeof(PetType))))
        {
            if (GetPetSaveData(_type).count > 0) petCount += 1;
            // print(_type + " : " + GetPetAge(_type));
        }
        return petCount;
    }

    public PetData GetPetDataByType(PetType type)
    {
        if (petDatas.ContainsKey(type)) return petDatas[type];
        else return null;
    }

    public int GetPetDataCount()
    {
        return petDatas.Count;
    }

    public List<PetData> GetActivePetDatas()
    {
        List<PetData> pets = new List<PetData>();
        foreach (var entry in petDatas)
        {
            if (entry.Value.obj.activeSelf)
            {
                pets.Add(entry.Value);
            }
        }
        return pets;
    }

    public PetData GetRandomPetData(int idx = -1)
    {
        if (idx == -1) idx = Random.Range(0, petDatas.Count);
        return petDatas.ElementAt(idx).Value;
    }

    // private void SetPetBirthDateAndLevel(PetType _type)
    // {
    //     PlayerPrefs.SetString("PetBirthDate_" + _type, Converter.DateTimeToString(DateTime.Now));
    //     PlayerPrefs.SetInt("PetLevel_" + _type, 1);
    //     PlayerPrefs.Save();
    // }

    public string GetPetAge(PetType _type)
    {
        DateTime birth = Converter.StringToDateTime(GetPetSaveData(_type).birthDate);

        TimeSpan age = DateTime.Now - birth;

        if (age.TotalDays >= 2f)
        {
            return Math.Floor(age.TotalDays) + " days";
        } else if (age.TotalDays >= 1f)
        {
            return "1 day";
        }
        else
        {
            if (age.TotalHours >= 1f)
            {
                return Math.Floor(age.TotalHours) + " hours";
            }
            else return "1 hour";
        }
    }
    
    public int GetPetCount(PetType _type)
    {
        return GetPetSaveData(_type).count;
    }

    public int GetPetLevel(PetType _type)
    {
        return GetPetSaveData(_type).level;
    }

    public int GetPetExp(PetType _type)
    {
        int count = GetPetCount(_type);
        int level = GetPetLevel(_type);

        int totalExpSpan = 0;
        for (int i = 0; i < level; i++)
        {
            totalExpSpan += 5 * i;
        }

        return count - totalExpSpan;
    }

    public bool PetLevelUP(PetType _type)
    {
        int exp = GetPetExp(_type);
        int level = GetPetLevel(_type);

        print("PetLevelUP " + _type + " : " + exp + "/" + level * 5);
        
        if (exp >= level * 5)
        {
            GetPetSaveData(_type).level += 1;
            return true;
        }

        return false;
    }

    public void GotNewPetFX(PetType _type)
    {
        GameObject fx = Instantiate(newPetSparcle_prefab, GetPetDataByType(_type).obj.transform, true);
        fx.transform.localPosition = Vector3.zero;
        fx.SetActive(true);
        DOVirtual.DelayedCall(20, () => {
            Destroy(fx);
        });

        List<PetData> pets = GetActivePetDatas();

        foreach (var pet in pets)
        {
            float thres = pets.Count < 3 ? 1 : 3f / pets.Count;
            if (UnityEngine.Random.Range(0f, 1f) < thres)
                pet.component.OnNewFriend();
        }
        
        GetPetDataByType(_type).obj.GetComponent<Pet>().OnIdle();
    }

    public Dictionary<PetType, PetData> GetPetDatas()
    {
        return petDatas;
    }
    
    public void AddPetCountByType(PetType _type)
    {
        PetSaveData data = GetPetSaveData(_type);
        data.count += 1;
        if (data.count == 1)
        {
            data.level = 1;
            data.birthDate = Converter.DateTimeToString(DateTime.Now);
        }

        StartCoroutine(LoadData());
    }
    
    private PetSaveData GetPetSaveData(PetType _type)
    {
        if (petSaveDatas.ContainsKey(_type)) return petSaveDatas[_type];
        
        if(PlayerPrefs.HasKey("PetSaveData_" + _type))
        {
            string data = PlayerPrefs.GetString("PetSaveData_" + _type);
            print(data);
            petSaveDatas.Add(_type, JsonUtility.FromJson<PetSaveData>(data));
        }
        else
        {
            PetSaveData data = new PetSaveData();
            data.count = PlayerPrefs.GetInt("PetCount_" + _type, 0);
            data.x = 0f;
            data.y = -1.25f;
            data.level = PlayerPrefs.GetInt("PetLevel_" + _type, 0);
            data.birthDate = PlayerPrefs.GetString("PetBirthDate_" + _type, Converter.DateTimeToString(DateTime.Now));
            data.isActive = true;

            petSaveDatas.Add(_type, data);
        }

        return petSaveDatas[_type];
    }

    private void SaveData()
    {
        foreach (var entry in petDatas)
        {
            PetData petdata = entry.Value;
            PetSaveData data = GetPetSaveData(petdata.type);
            data.x = petdata.obj.transform.position.x;
            data.y = petdata.obj.transform.position.y;
            PlayerPrefs.SetString("PetSaveData_" + petdata.type, JsonUtility.ToJson(data));
        }
        // print("PetSaveData : saved!");
        PlayerPrefs.Save();
    }

    private IEnumerator LoadData()
    {
        foreach (var entry in petDatas)
        {
            PetData petdata = entry.Value;
            PetSaveData data = GetPetSaveData(petdata.type);
            print(petdata.type + " : " + data.count);
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
        petSaveDatas = new Dictionary<PetType, PetSaveData>();
        LoadData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveData();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    public class PetSaveData
    {
        public int count;
        public bool isActive;
        public float x;
        public float y;
        public int level;
        public string birthDate;
    }
    
    
#if UNITY_EDITOR
    [Button]
    private void ImportPetsFromScene()
    {
        GameObject[] pets = GameObject.FindGameObjectsWithTag("PET");
        foreach (PetType type in Enum.GetValues(typeof(PetType)))
        {
            if (!petDatas.ContainsKey(type))
            {
                foreach (var obj in pets)
                {
                    if (obj.GetComponent<Pet>().type == type)
                    {
                        petDatas.Add(type, new PetData());
                        petDatas[type].obj = obj;
                        break;
                    }
                }
            }
        }
    }
    [Button]
    private void UpdatePetDatas()
    {
        foreach (var entry in petDatas)
        {
            PetData data = entry.Value;
            if(entry.Value.obj == null) continue;
            data.component = data.obj.GetComponent<Pet>();
            data.type = data.component.type;
            data.image = data.component.spriteRenderer.sprite;
        }
    }
#endif
}


[Serializable]
public class PetData
{
    public GameObject obj;
    [ReadOnly] public PetType type;
    [ReadOnly] public Pet component;
    [ReadOnly] public Sprite image;
}
public enum PetType { Fluffy, Cocoa, Brownie, Foxy, MrPinchy, Krabs, Dash, Batcat, Butter,Matata,Gcat, Kune, Lo, Ve, Default } //Caramel
