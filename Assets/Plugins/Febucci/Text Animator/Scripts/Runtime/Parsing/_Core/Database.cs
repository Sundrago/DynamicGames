using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Febucci.UI.Core
{
    /// <summary>
    ///     Caches information about tag providers, so that
    ///     it's easier to access them
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Database<T> : ScriptableObject where T : ScriptableObject, ITagProvider
    {
        [SerializeField] private List<T> data = new();
        private bool built;

        private Dictionary<string, T> dictionary;
        public List<T> Data => data;

        public T this[string key]
        {
            get
            {
                BuildOnce();
                return dictionary[key];
            }
        }

        private void OnEnable()
        {
            //Prevents database from not refreshing on
            //different domain reload settings
            built = false;
        }

        public void Add(T element)
        {
            if (data == null) data = new List<T>();
            data.Add(element);

            // at runtime adds directly on database as well, without needing to rebuild
            if (built && Application.isPlaying)
            {
                var tag = element.TagID;
                if (dictionary.ContainsKey(tag))
                    Debug.LogError($"Text Animator: Tag {tag} is already present in the database. Skipping...");
                else
                    dictionary.Add(tag, element);
            }
            else
            {
                built = false;
            }
        }

        public void ForceBuildRefresh()
        {
            built = false;
            BuildOnce();
        }

        public void BuildOnce()
        {
            if (built) return;
            built = true;

            if (dictionary == null)
                dictionary = new Dictionary<string, T>();
            else
                dictionary.Clear();

            string tagId;
            foreach (var source in data)
            {
                if (!source)
                    continue;

                tagId = source.TagID;

                if (string.IsNullOrEmpty(tagId))
                {
                    Debug.LogError("Text Animator: Tag is null or empty. Skipping...");
                    continue;
                }

                if (dictionary.ContainsKey(tagId))
                {
                    Debug.LogError($"Text Animator: Tag {tagId} is already present in the database. Skipping...");
                    continue;
                }

                dictionary.Add(tagId, source);
            }

            OnBuildOnce();
        }

        protected virtual void OnBuildOnce()
        {
        }

        public bool ContainsKey(string key)
        {
            BuildOnce();
            return dictionary.ContainsKey(key);
        }

        public void DestroyImmediate(bool databaseOnly = false)
        {
            if (!databaseOnly)
                foreach (var element in data)
                    Object.DestroyImmediate(element);

            Object.DestroyImmediate(this);
        }
    }
}