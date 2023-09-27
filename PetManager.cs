using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;

public class PetManager : MonoBehaviour
{
    [SerializeField, TableList]
    public List<Petdata> petdatas;
    public static PetManager Instance;

    [SerializeField]
    private GameObject newPetSparcle_prefab;
    
    private void Awake()
    {
        Instance = this;
        UpdatePetObjActive();
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

    public int GetPetCountByType(PetType _type)
    {
        return PlayerPrefs.GetInt("PetCount_" + _type);
    }

    public void AddPetCountByType(PetType _type)
    {
        int count = PlayerPrefs.GetInt("PetCount_" + _type) + 1;
        PlayerPrefs.SetInt("PetCount_" + _type, count);
        
        int totalCount = PlayerPrefs.GetInt("PetTotalCount") + 1;
        PlayerPrefs.SetInt("PetTotalCount", totalCount);
        PlayerPrefs.Save();

        if (count == 1)
        {
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
            data.obj.SetActive(GetPetCountByType(data.type) != 0);
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
public enum PetType {Cocoa, Brownie, Fluffy, Foxy} //Caramel
