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
    /// <summary>
    ///     Manages pet dialogues and provides methods to retrieve pet dialogues for specific events.
    /// </summary>
    public class PetDialogueManager : SerializedMonoBehaviour
    {
        public static PetDialogueManager Instance;
        [SerializeField] public PetWeatherData[] petWeatherDatas;
        [SerializeField] public Dictionary<PetType, PetDialogueData> petDialogueDatas = new();

        private void Awake()
        {
            Instance = this;
        }

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

        public string GetIdleText(PetType _type)
        {
            var strings = new List<string>();

            if (petDialogueDatas[_type].idle0 != "")
                strings.Add(Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].idle0));
            if (petDialogueDatas[_type].idle1 != "")
                strings.Add(Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].idle1));
            if (petDialogueDatas[_type].idle2 != "")
                strings.Add(Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].idle2));

            if (strings.Count == 0) return null;

            var rnd = Random.Range(0, strings.Count);
            return strings[rnd];
        }

        private string GetDialogFromData(PetType _type, Func<PetDialogueData, string> propertySelector)
        {
            var dialog = propertySelector(petDialogueDatas[_type]);
            return string.IsNullOrEmpty(dialog) ? null : Localize.GetLocalizedPetDialogue(dialog);
        }

        public string GetOnHitText(PetType _type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[_type].onHit)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onHit);
        }

        public string GetOnDragText(PetType _type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[_type].onDrag)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onDrag);
        }

        public string GetGameEnterString(PetType _type, GameType _gameType)
        {
            if (petDialogueDatas[_type].preferredGame == _gameType &&
                !string.IsNullOrEmpty(petDialogueDatas[_type].onPrefGameEnter))
                return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onPrefGameEnter);

            if (petDialogueDatas[_type].unpreferredGame == _gameType &&
                !string.IsNullOrEmpty(petDialogueDatas[_type].onUnpreferredGameEnter))
                return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onUnpreferredGameEnter);

            if (!string.IsNullOrEmpty(petDialogueDatas[_type].onGameEnter))
                return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onGameEnter);

            return null;
        }

        public string GetGameExitString(PetType _type, GameType _gameType)
        {
            if (petDialogueDatas[_type].preferredGame == _gameType &&
                !string.IsNullOrEmpty(petDialogueDatas[_type].onPrefGameExit))
                return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onPrefGameExit);

            if (petDialogueDatas[_type].unpreferredGame == _gameType &&
                !string.IsNullOrEmpty(petDialogueDatas[_type].onUnpreferredGameExit))
                return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onUnpreferredGameExit);

            if (!string.IsNullOrEmpty(petDialogueDatas[_type].onGameExit))
                return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onGameExit);

            return null;
        }

        public string GetOnTitleString(PetType _type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[_type].onTitle)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onTitle);
        }

        public string GetOnIslandString(PetType _type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[_type].onIsland)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onIsland);
        }

        public string GetShakeString(PetType _type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[_type].onTitleShake)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].onTitleShake);
        }

        public string GetNewFriendString(PetType _type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[_type].newFriend)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].newFriend);
        }

        public string GetWelcomeString(PetType _type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[_type].welcome)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].welcome);
        }

        public string GetDescrString(PetType _type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[_type].descr)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].descr);
        }

        public string GetDescr(PetType _type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[_type].descr)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].descr);
        }

        public string GetFrom(PetType _type)
        {
            if (string.IsNullOrEmpty(petDialogueDatas[_type].from)) return null;
            return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].from);
        }

        public char GetRank(PetType _type)
        {
            return petDialogueDatas[_type].rank;
        }

        public string GetPetScoreString(PetType _type, PetScoreType _scoreType)
        {
            switch (_scoreType)
            {
                case PetScoreType.NewBest:
                    if (string.IsNullOrEmpty(petDialogueDatas[_type].score_newBest))
                        return Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default]
                            .score_newBest);
                    return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].score_newBest);
                    break;
                case PetScoreType.Excellent:
                    if (string.IsNullOrEmpty(petDialogueDatas[_type].score_excelent))
                        return Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default]
                            .score_excelent);
                    return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].score_excelent);
                    break;
                case PetScoreType.Great:
                    if (string.IsNullOrEmpty(petDialogueDatas[_type].score_great))
                        return Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default]
                            .score_great);
                    return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].score_great);
                    break;
                case PetScoreType.Good:
                    if (string.IsNullOrEmpty(petDialogueDatas[_type].score_good))
                        return Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default].score_good);
                    return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].score_good);
                    break;
                case PetScoreType.Bad:
                    if (string.IsNullOrEmpty(petDialogueDatas[_type].score_bad))
                        return Localize.GetLocalizedPetDialogue(petDialogueDatas[PetType.Default].score_bad);
                    return Localize.GetLocalizedPetDialogue(petDialogueDatas[_type].score_bad);
                    break;
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

    public enum PetScoreType
    {
        NewBest,
        Excellent,
        Great,
        Good,
        Bad
    }
}