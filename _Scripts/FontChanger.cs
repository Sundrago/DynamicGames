using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
// using Sirenix.OdinInspector;
using TMPro;

public class FontChanger : MonoBehaviour
{
    TMPro.TMP_Text[] texts;
    public TMPro.TMP_FontAsset ko;
    public TMPro.TMP_FontAsset zh;
    public TMPro.TMP_FontAsset en;
    TMPro.TMP_FontAsset myFont;

    public List<Button> btns = new List<Button>();

    
    // [Button]
    public void ChangeFonts()
    {
        texts = GameObject.FindObjectsOfType<TMPro.TMP_Text>(true);
        string code;
        if (PlayerPrefs.HasKey("language"))
        {
            code = PlayerPrefs.GetString("language");
            LoadLocale(code);
        }
        else
        {
            Locale currentSelectedLocale = LocalizationSettings.SelectedLocale;
            code = currentSelectedLocale.Identifier.Code;
        }
        
        switch (code)
        {
            case "ko":
                myFont = ko;
                break;
            case "en":
                myFont = en;
                break;
            default:
                myFont = zh;
                break;
        }

        foreach (TMPro.TMP_Text text in texts)
        {
            if (text.gameObject.tag == "localizeText")
            {
                text.font = myFont;
                // if (code == "ar" || code == "ko" || code == "ja")
                //     text.fontStyle = FontStyles.Bold;
                // else
                //     text.fontStyle = FontStyles.Normal;
            }

        }
    }

    public void FontSelected(int idx)
    {
        switch(idx)
        {
            case 0:
                LoadLocale("zh-CN");
                break;
            case 1:
                LoadLocale("zh-hant");
                break;
            case 2:
                LoadLocale("nl");
                break;
            case 3:
                LoadLocale("en");
                break;
            case 4:
                LoadLocale("fr");
                break;
            case 5:
                LoadLocale("de");
                break;
            case 6:
                LoadLocale("ja");
                break;
            case 7:
                LoadLocale("ko");
                break;
            case 8:
                LoadLocale("pt");
                break;
            case 9:
                LoadLocale("ru");
                break;
            case 10:
                LoadLocale("es");
                break;
        }
        foreach (Button btn in btns)
        {
            btn.interactable = true;
        }
        if(btns.Count != 0 && idx < btns.Count) btns[idx].interactable = false;

        ChangeFonts();
    }

    public void LoadLocale(string languageIdentifier)
    {
        print("laodlocale : " + languageIdentifier);
        // if(lang != null) lang.ChangeLanguage(languageIdentifier);
        LocaleIdentifier localeCode = new LocaleIdentifier(languageIdentifier);
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            Locale aLocale = LocalizationSettings.AvailableLocales.Locales[i];
            LocaleIdentifier anIdentifier = aLocale.Identifier;
            if (anIdentifier == localeCode)
            {
                LocalizationSettings.SelectedLocale = aLocale;
                PlayerPrefs.SetString("language", languageIdentifier);
                print("localeFound : " + languageIdentifier);
            }
        }
    }

    private void Awake()
    {
        if (PlayerPrefs.HasKey("language"))
        {
            LoadLocale(PlayerPrefs.GetString("language"));
        }
        else
        {
            var locale = LocalizationSettings.SelectedLocale;
            if (locale == null)
            {
                LoadLocale("en");
            } else 
                ChangeFonts();
        }
        
        foreach (Button btn in btns)
        {
            btn.interactable = true;
        }
        ChangeFonts();
    }
}
