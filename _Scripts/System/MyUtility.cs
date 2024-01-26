using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Random = UnityEngine.Random;

namespace MyUtility
{
    static class Converter {
        const string format = "yyyy/MM/dd HH:mm:ss";
        private static System.IFormatProvider provider;
        
        public static int StringToInt(string value) {
            int number;
            if (int.TryParse(value, out number))
                return number;
            else
                return 0;
        }

        public static string DateTimeToString(DateTime dateTime)
        {
            return dateTime.ToString(format);
        }

        public static DateTime StringToDateTime(string dateTimeString)
        {
            DateTime dateTime;
            bool success = DateTime.TryParseExact(dateTimeString, format, provider, DateTimeStyles.None, out dateTime);

            if (success) return dateTime;
            return DateTime.Now;
        }
    }
    
    static class Localize {
        public static string GetLocalizedString(string input)
        {
            if (input.Contains('[') && input.Contains(']'))
            {
                string[] sliced = input.Split('[', ']');
                string key = sliced[input.IndexOf('[') + 1];
                return LocalizationSettings.StringDatabase.GetLocalizedString("UI", key);
            }
            
            // Debug.Log("LocaleCodeNotFound for string : " + input);
            return input;
        }
        
        public static string GetLocalizedPetDialogue(string input)
        {
            if (input.Contains("<LOTTERY>"))
            {
                DateTime date =DateTime.Now;
                int weekOfMonth=(date.Day + ((int)date.DayOfWeek)) / 7 + 1;
                
                string lottery;
                int maxN = PlayerPrefs.GetString("language") == "ko" ? 46 : 70;
                
                float rnd = Random.Range(0f, 1f);
                if (rnd < 0.33f)
                {
                    Random.InitState(date.Year * date.Month + weekOfMonth);
                    lottery = Random.Range(1, maxN).ToString();
                    Random.InitState(date.Year * date.Month + weekOfMonth + 10);
                    lottery += " " + Random.Range(1, maxN);
                }
                else if(rnd < 0.66)
                {
                    Random.InitState(date.Year * date.Month + weekOfMonth + 11);
                    lottery = Random.Range(1, maxN).ToString();
                    Random.InitState(date.Year * date.Month + weekOfMonth + 12);
                    lottery += " " + Random.Range(1, maxN);
                    Random.InitState(date.Year * date.Month + weekOfMonth + 13);
                    lottery += " " + Random.Range(1, maxN);
                }
                else
                {
                    Random.InitState(date.Year * date.Month + weekOfMonth + 14);
                    lottery = Random.Range(1, 27).ToString();
                }

                Random.InitState((int)(Time.time * 1000f));
                return lottery;
            }
            
            if (input.Contains('[') && input.Contains(']'))
            {
                string[] sliced = input.Split('[', ']');
                string key = sliced[input.IndexOf('[') + 1];
                return LocalizationSettings.StringDatabase.GetLocalizedString("PetDialogue", key);
            }
            
            // Debug.Log("LocaleCodeNotFound for string : " + input);
            return input;
        }
    }
}