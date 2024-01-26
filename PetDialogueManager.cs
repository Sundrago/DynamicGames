using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;

public class PetDialogueManager : SerializedMonoBehaviour
{
    [SerializeField] public Dictionary<PetType, PetDialogueData> petDialogueDatas = new Dictionary<PetType, PetDialogueData>();
    [SerializeField] public PetWeatherData[] petWeatherDatas;

    public static PetDialogueManager Instance;
    
    [Serializable]
    public class PetDialogueData
    {
        public PetType type;
        public char rank;
        public string descr, descr_l, from;

        public string welcome, idle0, idle1, idle2;
        public string onDrag, onTitle, onTitleShake, onIsland, onHit;

        public GameType preferredGame;
        public string onPrefGameEnter, onPrefGameExit;

        public GameType unpreferredGame;
        public string onUnpreferredGameEnter, onUnpreferredGameExit;

        public string onGameEnter, onGameExit;

        public string score_newBest, score_excelent, score_great, score_good, score_bad;
        public string newFriend;

        public List<PetWeatherData> weatherDatas;
    }

    [Serializable]
    public class PetWeatherData
    {
        public PetType type;
        public int idx;
        public string data;
    }
    
    void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        // PlayWeatherDialogue();
        StartCoroutine(PetIdleDialogueTimer());
    }

#if UNITY_EDITOR
    [Button]
    private void LoadDialogueData()
    {
        string json = File.ReadAllText(Application.dataPath + "/Resources/DialogueData.json");
        petDialogueDatas = JsonConvert.DeserializeObject<Dictionary<PetType, PetDialogueData>>(json);
        print(json);
    }

    [Button]
    private void LoadWeatherData()
    {
        string json = File.ReadAllText(Application.dataPath + "/Resources/WeatherData.json");
        petWeatherDatas = JsonConvert.DeserializeObject<PetWeatherData[]>(json);
        print(json);

        foreach (var data in petDialogueDatas)
        {
            data.Value.weatherDatas = new List<PetWeatherData>();
        }

        foreach (var data in petWeatherDatas)
        {
            petDialogueDatas[data.type].weatherDatas.Add(data);
        }
    }
#endif

    public void PlayWeatherDialogue()
    {
        foreach (var data in petWeatherDatas)
        {
            if(SkyboxManager.Instance.weatherIdx != data.idx) continue;

            Petdata petdata = PetManager.Instance.GetPetDataByType(data.type);
            if(petdata == null) continue;
            
            if(!petdata.obj.activeSelf) continue;
            petdata.obj.GetComponent<Pet>().ShowDialogue(MyUtility.Localize.GetLocalizedPetDialogue(data.data));
        }
    }
    
    public string GetIdleText(PetType _type)
    {
        List<string> strings = new List<string>();

        if (petDialogueDatas[_type].idle0 != "") strings.Add(MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].idle0));
        if (petDialogueDatas[_type].idle1 != "") strings.Add(MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].idle1));
        if (petDialogueDatas[_type].idle2 != "") strings.Add(MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].idle2));

        if (strings.Count == 0) return null;

        int rnd = Random.Range(0, strings.Count);
        return strings[rnd];
    }

    public string GetOnHitText(PetType _type)
    {
        if (string.IsNullOrEmpty(petDialogueDatas[_type].onHit)) return null;
        return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onHit);
    }
    
    public string GetOnDragText(PetType _type)
    {
        if (string.IsNullOrEmpty(petDialogueDatas[_type].onDrag)) return null;
        return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onDrag);
    }

    public string GetGameEnterString(PetType _type, GameType _gameType)
    {
        if(petDialogueDatas[_type].preferredGame == _gameType && !string.IsNullOrEmpty(petDialogueDatas[_type].onPrefGameEnter))
            return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onPrefGameEnter);
        
        if(petDialogueDatas[_type].unpreferredGame == _gameType && !string.IsNullOrEmpty(petDialogueDatas[_type].onUnpreferredGameEnter))
            return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onUnpreferredGameEnter);
        
        if(!string.IsNullOrEmpty(petDialogueDatas[_type].onGameEnter))
            return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onGameEnter);

        return null;
    }
    
    public string GetGameExitString(PetType _type, GameType _gameType)
    {
        if(petDialogueDatas[_type].preferredGame == _gameType && !string.IsNullOrEmpty(petDialogueDatas[_type].onPrefGameExit))
            return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onPrefGameExit);
        
        if(petDialogueDatas[_type].unpreferredGame == _gameType && !string.IsNullOrEmpty(petDialogueDatas[_type].onUnpreferredGameExit))
            return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onUnpreferredGameExit);
        
        if(!string.IsNullOrEmpty(petDialogueDatas[_type].onGameExit))
            return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onGameExit);

        return null;
    }

    public string GetOnTitleString(PetType _type)
    {
        if (string.IsNullOrEmpty(petDialogueDatas[_type].onTitle)) return null;
        return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onTitle);
    }

    public string GetOnIslandString(PetType _type)
    {
        if (string.IsNullOrEmpty(petDialogueDatas[_type].onIsland)) return null;
        return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onIsland);
    }

    public string GetShakeString(PetType _type)
    {
        if (string.IsNullOrEmpty(petDialogueDatas[_type].onTitleShake)) return null;
        return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onTitleShake);
    }

    public string GetNewFriendString(PetType _type)
    {
        if (string.IsNullOrEmpty(petDialogueDatas[_type].newFriend)) return null;
        return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].newFriend);
    }

    public string GetWelcomeString(PetType _type)
    {
        if (string.IsNullOrEmpty(petDialogueDatas[_type].welcome)) return null;
        return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].welcome);
    }
    
    public string GetDescrString(PetType _type)
    {
        if (string.IsNullOrEmpty(petDialogueDatas[_type].descr)) return null;
        return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].descr);
    }

    public string GetRank(PetType _type)
    {
        if (string.IsNullOrEmpty(petDialogueDatas[_type].rank.ToString())) return null;
        return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].rank.ToString());
    }

    public string GetDescr(PetType _type)
    {
        if (string.IsNullOrEmpty(petDialogueDatas[_type].descr.ToString())) return null;
        return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].descr.ToString());
    }

    public string GetFrom(PetType _type)
    {
        if (string.IsNullOrEmpty(petDialogueDatas[_type].from.ToString())) return null;
        return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].from.ToString());
    }
    
    public enum PetScoreType
    {
        score_newBest, score_excelent, score_great, score_good, score_bad
    }

    public string GetPetScoreString(PetType _type, PetScoreType _scoreType)
    {
        switch (_scoreType)
        {
            case PetScoreType.score_newBest:
                if (string.IsNullOrEmpty(petDialogueDatas[_type].score_newBest))
                    return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default].score_newBest);
                return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].score_newBest);
                break;
            case PetScoreType.score_excelent:
                if (string.IsNullOrEmpty(petDialogueDatas[_type].score_excelent))
                    return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default].score_excelent);
                return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].score_excelent);
                break;
            case PetScoreType.score_great:
                if (string.IsNullOrEmpty(petDialogueDatas[_type].score_great))
                    return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default].score_great);
                return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].score_great);
                break;
            case PetScoreType.score_good:
                if (string.IsNullOrEmpty(petDialogueDatas[_type].score_good))
                    return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default].score_good);
                return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].score_good);
                break;
            case PetScoreType.score_bad:
                if (string.IsNullOrEmpty(petDialogueDatas[_type].score_bad))
                    return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default].score_bad);
                return MyUtility.Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].score_bad);
                break;
        }

        return null;
    }

    private IEnumerator PetIdleDialogueTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f,15f));

            List<GameObject> activePet = new List<GameObject>();
            foreach (var petData in PetManager.Instance.petdatas)
            {
                if(petData.obj.activeSelf) activePet.Add(petData.obj);
            }

            if (activePet.Count != 0)
            {
                int rnd = Random.Range(0, activePet.Count);
                activePet[rnd].GetComponent<Pet>().OnIdle();
            }
        }
    }
}
