using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using MyUtility;

public class PetManager : MonoBehaviour
{
    [SerializeField, TableList]
    public List<Petdata> petdatas;
    public static PetManager Instance;

    [SerializeField]
    private GameObject newPetSparcle_prefab;

    [SerializeField]
    private AskForUserReview askForUserReview;
    
    private void Awake()
    {
        Instance = this;
        UpdatePetObjActive();
    }

    private void Start()
    {
        foreach (PetType _type in Enum.GetValues((typeof(PetType))))
        {
            if (GetPetCount(_type) > 0 && !PlayerPrefs.HasKey("PetBirthDate_" + _type))
            {
                SetPetBirthDateAndLevel(_type);
            }
            
            print(_type + " : " + GetPetAge(_type));
        }
    }

    public Petdata GetPetDataByType(PetType type)
    {
        foreach (Petdata data in petdatas)
        {
            if (data.type == type)
            {
                return data;
            }
        }
        return null;
    }

    private void SetPetBirthDateAndLevel(PetType _type)
    {
        PlayerPrefs.SetString("PetBirthDate_" + _type, Converter.DateTimeToString(DateTime.Now));
        PlayerPrefs.SetInt("PetLevel_" + _type, 1);
        PlayerPrefs.Save();
    }

    public string GetPetAge(PetType _type)
    {
        if (!PlayerPrefs.HasKey("PetBirthDate_" + _type))
            return null;

        print(PlayerPrefs.GetString("PetBirthDate_" + _type));
        DateTime birth = Converter.StringToDateTime(PlayerPrefs.GetString("PetBirthDate_" + _type));

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
        return PlayerPrefs.GetInt("PetCount_" + _type);
    }

    public int GetPetLevel(PetType _type)
    {
        return PlayerPrefs.GetInt("PetLevel_" + _type);
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
            PlayerPrefs.SetInt("PetLevel_" + _type, PlayerPrefs.GetInt("PetLevel_" + _type) + 1);
            return true;
        }

        return false;
    }

    public void GotNewPet(PetType _type)
    {
        SetPetBirthDateAndLevel(_type);
    }
    
    public void AddPetCountByType(PetType _type)
    {
        int count = PlayerPrefs.GetInt("PetCount_" + _type) + 1;
        PlayerPrefs.SetInt("PetCount_" + _type, count);
        
        int totalCount = PlayerPrefs.GetInt("PetTotalCount") + 1;
        PlayerPrefs.SetInt("PetTotalCount", totalCount);
        PlayerPrefs.Save();

        if(totalCount == 1) TutorialManager.Instancee.TutorialD_Check();
        
        // if (totalCount >= 3)
        // {
        //     if (askForUserReview != null)
        //     {
        //         // askForUserReview.CubeFall();
        //     }
        // } 

        if (count == 1)
        {
            GotNewPet(_type);
            GameObject fx = Instantiate(newPetSparcle_prefab, GetPetDataByType(_type).obj.transform, true);
            fx.transform.localPosition = Vector3.zero;
            fx.SetActive(true);
            DOVirtual.DelayedCall(20, () => {
                Destroy(fx);
            });
        }
        UpdatePetObjActive();
    }

    public void UpdatePetObjActive()
    {
        foreach (Petdata data in petdatas)
        {
            data.obj.SetActive(GetPetCount(data.type) != 0);
        }
    }
    
#if UNITY_EDITOR
    [Button]
    private void AddPetTypeToList()
    {
        foreach (PetType type in Enum.GetValues(typeof(PetType)))
        {
            if(GetPetDataByType(type) == null) petdatas.Add(new Petdata(type));
        }
    }
    
    [Button]
    private void GetImage()
    {
        foreach (Petdata data in petdatas)
        {
            if (data.image == null) data.image = data.obj.GetComponent<PetAnim>().idles[0];
        }
    }
#endif
}

[Serializable]
public class Petdata
{
    public PetType type;
    public Sprite image;
    public GameObject obj;

    public Petdata(PetType _type)
    {
        type = _type;
    }
}
public enum PetType {Cocoa, Brownie, Fluffy, Foxy, MrPinchy, Krabs, Dash } //Caramel
