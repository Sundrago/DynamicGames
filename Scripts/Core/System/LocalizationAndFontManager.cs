using System.Collections.Generic;
using MyUtility;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Core.System
{
    public class LocalizationAndFontManager : MonoBehaviour
    {
        [Header("Font Asset")] [SerializeField]
        private TMP_FontAsset lunaPixelFont;

        [SerializeField] private List<Button> btns = new();

        private readonly Dictionary<int, string> languageIndexMap = new()
        {
            { 0, "zh-CN" },
            { 1, "zh-hant" },
            { 2, "nl" },
            { 3, "en" },
            { 4, "fr" },
            { 5, "de" },
            { 6, "ja" },
            { 7, "ko" },
            { 8, "pt" },
            { 9, "ru" },
            { 10, "es" }
        };

        private TMP_FontAsset myFont;

        private TMP_Text[] texts;

        private void Awake()
        {
            if (PlayerData.HasKey(DataKey.language))
            {
                LoadLocale(PlayerData.GetString(DataKey.language));
            }
            else
            {
                var locale = LocalizationSettings.SelectedLocale;
                if (locale == null)
                    LoadLocale("en");
                else
                    ChangeFonts();
            }

            foreach (var btn in btns) btn.interactable = true;
            ChangeFonts();
        }

        public void ChangeFonts()
        {
            texts = FindObjectsOfType<TMP_Text>(true);
            string code;
            if (PlayerData.HasKey(DataKey.language))
            {
                code = PlayerData.GetString(DataKey.language);
                LoadLocale(code);
            }
            else
            {
                var currentSelectedLocale = LocalizationSettings.SelectedLocale;
                code = currentSelectedLocale.Identifier.Code;
            }

            foreach (var text in texts)
                if (text.gameObject.tag == "localizeText")
                {
                    text.font = lunaPixelFont;
                    text.fontStyle = FontStyles.Normal;
                }
        }

        public void FontSelected(int idx)
        {
            if (languageIndexMap.TryGetValue(idx, out var language)) LoadLocale(language);

            foreach (var btn in btns) btn.interactable = true;
            if (btns.Count != 0 && idx < btns.Count) btns[idx].interactable = false;

            ChangeFonts();
        }

        public void LoadLocale(string languageIdentifier)
        {
            print("laodlocale : " + languageIdentifier);
            var localeCode = new LocaleIdentifier(languageIdentifier);
            for (var i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                var aLocale = LocalizationSettings.AvailableLocales.Locales[i];
                var anIdentifier = aLocale.Identifier;
                if (anIdentifier == localeCode)
                {
                    LocalizationSettings.SelectedLocale = aLocale;
                    PlayerData.SetString(DataKey.language, languageIdentifier);
                    print("localeFound : " + languageIdentifier);
                }
            }
        }
    }
}