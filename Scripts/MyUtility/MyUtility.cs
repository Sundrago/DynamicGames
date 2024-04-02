using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Random = UnityEngine.Random;

namespace MyUtility
{
    /// <summary>
    /// The Converter class provides utility methods for converting values between different data types.
    /// </summary>
    static class Converter
    {
        const string format = "yyyy/MM/dd HH:mm:ss";
        private static System.IFormatProvider provider = CultureInfo.InvariantCulture;
        
        public static int StringToInt(string value)
        {
            if (int.TryParse(value, out var number))
                return number;
            throw new ArgumentException("Unable to parse string to integer : " + value);
        }

        public static string DateTimeToString(DateTime dateTime)
        {
            return dateTime.ToString(format);
        }

        public static DateTime StringToDateTime(string dateTimeString)
        {
            if (DateTime.TryParseExact(dateTimeString, format, provider, DateTimeStyles.None, out var dateTime))
                return dateTime;
            throw new ArgumentException( "Unable to parse string to dateTime : " + dateTimeString);
        }

        public static int GetWeekOfMonth(DateTime date)
        {
            int weekOfYear =
                CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Monday);
            int weekOfYearOfFirstDayOfMonth = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                new DateTime(date.Year, date.Month, 1), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            return weekOfYear - weekOfYearOfFirstDayOfMonth + 1;
        }
        
        public static T[] DeserializeCSV<T>(string csv) where T : new()
        {
            List<T> objects = new List<T>();
            
            string[] lines = csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
            {
                throw new ArgumentException("CSV string is empty.");
            }
            
            string[] headers = lines[0].Split(',');
            
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');

                if (values.Length != headers.Length)
                {
                    throw new FormatException($"Number of columns in line {i + 1} does not match the number of headers.");
                }

                T obj = new T();
                
                for (int j = 0; j < headers.Length; j++)
                {
                    var property = typeof(T).GetProperty(headers[j].Trim());

                    if (property != null)
                    {
                        object parsedValue = Convert.ChangeType(values[j].Trim(), property.PropertyType);
                        property.SetValue(obj, parsedValue);
                    }
                }
                objects.Add(obj);
            }

            return objects.ToArray();
        }
        
        public static T[] DeserializeJSON<T>(string jsonData)
        {
            try
            {
                return JsonConvert.DeserializeObject<T[]>(jsonData);
            }
            catch (JsonSerializationException ex)
            {
                throw new FormatException("Error deserializing JSON data.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during JSON deserialization.", ex);
            }
        }

        public static T[] DeserializeJSONToArray<T>(string jsonData)
        {
            var dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, T>>(jsonData);
            return dictionary.Values.ToArray();
        }

    }

    /// <summary>
    /// The Localize class provides methods for localizing strings.
    /// </summary>
    static class Localize
    {
        public static string GetLocalizedString(string input)
        {
            string key = GetLocalizationKey(input);
            return string.IsNullOrEmpty(key)
                ? input
                : LocalizationSettings.StringDatabase.GetLocalizedString("UI", key);
        }

        public static string GetLocalizedPetDialogue(string input)
        {
            if (input.Contains("<LOTTERY>"))
            {
                return GenerateRandomLotteryNumber();
            }

            string key = GetLocalizationKey(input);
            return string.IsNullOrEmpty(key)
                ? input
                : LocalizationSettings.StringDatabase.GetLocalizedString("PetDialogue", key);
        }

        private static string GetLocalizationKey(string input)
        {
            if (input.Contains('[') && input.Contains(']'))
            {
                string[] sliced = input.Split('[', ']');
                return sliced[1];
            }

            return null;
        }

        private static string GenerateRandomLotteryNumber()
        {
            DateTime date = DateTime.Now;
            int weekOfMonth = (date.Day + ((int)date.DayOfWeek)) / 7 + 1;
            int maxNumber = PlayerPrefs.GetString("language") == "ko" ? 46 : 70;
            string lottery = "";
            float rnd = Random.Range(0f, 1f);
            Random.InitState(date.Year * date.Month + weekOfMonth);

            if (rnd < 0.33f)
            {
                lottery += GenerateRandomNumber(maxNumber) + " ";
                lottery += GenerateRandomNumber(maxNumber);
            }
            else if (rnd < 0.66)
            {
                lottery += GenerateRandomNumber(maxNumber) + " ";
                lottery += GenerateRandomNumber(maxNumber) + " ";
                lottery += GenerateRandomNumber(maxNumber);
            }
            else
            {
                lottery += GenerateRandomNumber(27).ToString();
            }
            Random.InitState((int)(Time.time * 1000f));
            return lottery;
        }
        static int GenerateRandomNumber(int maxNumber)
        {
            return Random.Range(1, maxNumber);
        }
    }
}