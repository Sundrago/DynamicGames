using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DynamicGames.MiniGames;
using DynamicGames.UI;
using DynamicGames.Utility;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DynamicGames.Pet
{
    public enum PetScoreType
    {
        NewBest,
        Excellent,
        Great,
        Good,
        Bad
    }
    
    /// <summary>
    ///     Manages pet dialogues and provides methods to retrieve pet dialogues for specific events.
    /// </summary>
    public class PetDialogueManager : SerializedMonoBehaviour
    {
        public static PetDialogueManager Instance;
        [SerializeField] public PetWeatherData[] petWeatherDatas;
        [SerializeField] public Dictionary<PetType, PetDialogueData> petDialogueDatas = new();

        private void Awake() => Instance = this;

        private void OnEnable()
        {
            StartCoroutine(PetIdleDialogueTimer());
        }

        public void PlayWeatherDialogue()
        {
            foreach (var data in petWeatherDatas)
            {
                if (SkyboxManager.Instance.weatherIdx != data.idx) continue;

                var petdata = PetManager.Instance.GetPetDataByType(data.type);
                if (petdata == null) continue;

                if (!petdata.obj.activeSelf) continue;
                petdata.component.ShowDialogue(Localize.GetLocalizedPetDialogue(data.data));
            }
        }

        public string GetIdleText(PetType type)
        {
            var strings = new List<string>();

            if (petDialogueDatas[type].idle0 != "")
                strings.Add(Localize.GetLocalizedPetDialogue(petDialogueDatas[type].idle0));
            if (petDialogueDatas[type].idle1 != "")
                strings.Add(Localize.GetLocalizedPetDialogue(petDialogueDatas[type].idle1));
            if (petDialogueDatas[type].idle2 != "")
                strings.Add(Localize.GetLocalizedPetDialogue(petDialogueDatas[type].idle2));

            if (strings.Count == 0) return null;

            var rnd = Random.Range(0, strings.Count);
            return strings[rnd];
        }

        private string GetDialogFromData(PetType type, Func<PetDialogueData, string> propertySelector)
        {
            var dialog = propertySelector(petDialogueDatas[type]);
            return string.IsNullOrEmpty(dialog) ? null : Localize.GetLocalizedPetDialogue(dialog);
        }

        public string GetOnHitText(PetType type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[type].onHit)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].onHit);
        }

        public string GetOnDragText(PetType type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[type].onDrag)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].onDrag);
        }

        public string GetGameEnterString(PetType type, GameType gameType)
        {
            if (petDialogueDatas[type].preferredGame == gameType &&
                !string.IsNullOrEmpty(petDialogueDatas[type].onPrefGameEnter))
                return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].onPrefGameEnter);

            if (petDialogueDatas[type].unpreferredGame == gameType &&
                !string.IsNullOrEmpty(petDialogueDatas[type].onUnpreferredGameEnter))
                return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].onUnpreferredGameEnter);

            if (!string.IsNullOrEmpty(petDialogueDatas[type].onGameEnter))
                return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].onGameEnter);

            return null;
        }

        public string GetGameExitString(PetType type, GameType gameType)
        {
            if (petDialogueDatas[type].preferredGame == gameType &&
                !string.IsNullOrEmpty(petDialogueDatas[type].onPrefGameExit))
                return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].onPrefGameExit);

            if (petDialogueDatas[type].unpreferredGame == gameType &&
                !string.IsNullOrEmpty(petDialogueDatas[type].onUnpreferredGameExit))
                return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].onUnpreferredGameExit);

            if (!string.IsNullOrEmpty(petDialogueDatas[type].onGameExit))
                return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].onGameExit);

            return null;
        }

        public string GetOnTitleString(PetType type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[type].onTitle)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].onTitle);
        }

        public string GetOnIslandString(PetType type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[type].onIsland)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].onIsland);
        }

        public string GetShakeString(PetType type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[type].onTitleShake)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].onTitleShake);
        }

        public string GetNewFriendString(PetType type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[type].newFriend)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].newFriend);
        }

        public string GetWelcomeString(PetType type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[type].welcome)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].welcome);
        }

        public string GetDescrString(PetType type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[type].descr)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].descr);
        }

        public string GetDescr(PetType type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[type].descr)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].descr);
        }

        public string GetFrom(PetType type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[type].from)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].from);
        }

        public char GetRank(PetType type)
        {
            return petDialogueDatas[type].rank;
        }

        public string GetPetScoreString(PetType type, PetScoreType scoreType)
        {
            switch (scoreType)
            {
                case PetScoreType.NewBest:
                    if (string.IsNullOrEmpty(petDialogueDatas[type].score_newBest))
                        return Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default]
                            .score_newBest);
                    return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].score_newBest);
                case PetScoreType.Excellent:
                    if (string.IsNullOrEmpty(petDialogueDatas[type].score_excelent))
                        return Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default]
                            .score_excelent);
                    return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].score_excelent);
                case PetScoreType.Great:
                    if (string.IsNullOrEmpty(petDialogueDatas[type].score_great))
                        return Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default]
                            .score_great);
                    return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].score_great);
                case PetScoreType.Good:
                    if (string.IsNullOrEmpty(petDialogueDatas[type].score_good))
                        return Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default].score_good);
                    return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].score_good);
                case PetScoreType.Bad:
                    if (string.IsNullOrEmpty(petDialogueDatas[type].score_bad))
                        return Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default].score_bad);
                    return Localize.GetLocalizedPetDialogue(petDialogueDatas[type].score_bad);
            }

            return null;
        }

        private IEnumerator PetIdleDialogueTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(5f, 15f));

                var activePet = PetManager.Instance.GetActivePetConfigs();

                if (activePet.Count != 0)
                {
                    var rnd = Random.Range(0, activePet.Count);
                    activePet[rnd].component.OnIdle();
                }
            }
        }

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

#if UNITY_EDITOR
        [Button]
        private void LoadDialogueData()
        {
            var json = File.ReadAllText(Application.dataPath + "/Resources/DialogueData.json");
            petDialogueDatas = JsonConvert.DeserializeObject<Dictionary<PetType, PetDialogueData>>(json);
            print(json);
        }

        [Button]
        private void LoadWeatherData()
        {
            var json = File.ReadAllText(Application.dataPath + "/Resources/WeatherData.json");
            petWeatherDatas = JsonConvert.DeserializeObject<PetWeatherData[]>(json);
            print(json);

            foreach (var data in petDialogueDatas) data.Value.weatherDatas = new List<PetWeatherData>();
            foreach (var data in petWeatherDatas) petDialogueDatas[data.type].weatherDatas.Add(data);
        }
#endif
    }
}