using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DynamicGames.Utility
{
    internal static class PlayerData
    {
        public static bool HasKey(DataKey key)
        {
            return PlayerPrefs.HasKey(key.ToString());
        }

        public static string GetString(DataKey key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key.ToString(), defaultValue);
        }

        public static int GetInt(DataKey key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key.ToString(), defaultValue);
        }

        public static float GetFloat(DataKey key, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key.ToString(), defaultValue);
        }

        public static void SetInt(DataKey key, int value)
        {
            PlayerPrefs.SetInt(key.ToString(), value);
            PlayerPrefs.Save();
        }

        public static void SetString(DataKey key, string value)
        {
            PlayerPrefs.SetString(key.ToString(), value);
            PlayerPrefs.Save();
        }

        public static void SetFloat(DataKey key, float value)
        {
            PlayerPrefs.SetFloat(key.ToString(), value);
            PlayerPrefs.Save();
        }
    }

    public class JsonPlayerData
    {
        private const string FileName = "playerData.json";
        private Dictionary<string, object> data;
        private readonly string filePath;

        public JsonPlayerData()
        {
            filePath = Application.persistentDataPath + "/" + FileName;
            LoadData();
        }

        private void LoadData()
        {
            data = new Dictionary<string, object>();

            if (!File.Exists(filePath)) return;

            try
            {
                var jsonData = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(jsonData)) data = JsonUtility.FromJson<Dictionary<string, object>>(jsonData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading data from {filePath}: {e.Message}");
            }
        }

        private void SaveData()
        {
            var jsonData = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, jsonData);
        }

        public void SetInt(string key, int value)
        {
            data[key] = value;
            SaveData();
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            if (!data.ContainsKey(key)) return defaultValue;

            var value = data[key];
            if (value is int)
            {
                return (int)value;
            }

            Debug.LogError($"Key '{key}' has invalid data type (expected int)");
            return defaultValue;
        }

        public void SetFloat(string key, float value)
        {
            data[key] = value;
            SaveData();
        }

        public float GetFloat(string key, float defaultValue = 0f)
        {
            if (!data.ContainsKey(key)) return defaultValue;

            var value = data[key];
            if (value is float)
            {
                return (float)value;
            }

            Debug.LogError($"Key '{key}' has invalid data type (expected float)");
            return defaultValue;
        }

        public void SetString(string key, string value)
        {
            data[key] = value;
            SaveData();
        }

        public string GetString(string key, string defaultValue = "")
        {
            if (!data.ContainsKey(key)) return defaultValue;

            var value = data[key];
            if (value is string)
            {
                return (string)value;
            }

            Debug.LogError($"Key '{key}' has invalid data type (expected string)");
            return defaultValue;
        }

        public bool HasKey(string key)
        {
            return data.ContainsKey(key);
        }

        public void DeleteKey(string key)
        {
            if (data.ContainsKey(key))
            {
                data.Remove(key);
                SaveData();
            }
        }

        public void DeleteAll()
        {
            data.Clear();
            SaveData();
        }
    }

    public enum DataKey
    {
        debugMode,
        adDate,
        adCount,
        settings_bgm,
        settings_sfx,
        AskForUserReviewStatus,
        totalTicketCount,
        language,
        totalScoreCount,
        ticketCount,
        gachaCoinCount,
        keyCount
    }
}