using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DynamicGames.UI
{
    /// <summary>
    /// Manages the skybox for weather conditions in the game.
    /// </summary>
    public class SkyboxManager : MonoBehaviour
    {
        [SerializeField] private GameObject sky_prefab;
        [SerializeField] [TableList] private List<BGSpriteSet> bGSpriteSets;
        
        private static readonly List<(float hour, (int WeatherIndex, int SpriteIndex))> TimeWeatherMapping = new()
        {
            (5, (0, 2)),
            (6.5f, (1, 3)),
            (8, (2, 5)),
            (10, (3, 1)),
            (12, (4, 4)),
            (16, (5, 0)),
            (17, (6, 1)),
            (18, (7, 5)),
            (19, (8, 6)),
            (20.5f, (9, 3)),
            (22, (10, 7)),
            (float.MaxValue, (0, 2))
        };

        public int weatherIdx;
        private int imageIdx;
        private GameObject previousBG;

        public static SkyboxManager Instance { get; set; }

        private void Awake()
        {
            GetBackgroundIndexByTime();
            Instance = this;
        }

        private void OnEnable()
        {
            if (previousBG == null)
            {
                StartCoroutine(CreateBackgroundWithTransition(GetBackgroundIndexByTime(), 2f));
            }
            else
            {
                if (imageIdx != GetBackgroundIndexByTime())
                {
                    imageIdx = GetBackgroundIndexByTime();
                    StartCoroutine(CreateBackgroundWithTransition(imageIdx, 2f));
                }
            }
        }

        private int GetBackgroundIndexByTime()
        {
            var time = DateTime.Now;
            var currentTimeInHour = time.Hour + time.Minute / 60f;

            foreach (var item in TimeWeatherMapping)
                if (currentTimeInHour < item.hour)
                {
                    weatherIdx = item.Item2.WeatherIndex;
                    return item.Item2.SpriteIndex;
                }

            return -1;
        }

        [Button]
        private void CreateBackgroundAtIndex(int idx)
        {
            StartCoroutine(CreateBackgroundWithTransition(idx));
        }

        private IEnumerator CreateBackgroundWithTransition(int idx = -1, float transitionTime = 5f)
        {
            if (idx == -1) idx = Random.Range(0, bGSpriteSets.Count);
            var BGHolder = new GameObject();
            BGHolder.transform.SetParent(gameObject.transform);
            BGHolder.AddComponent<RectTransform>();
            BGHolder.transform.localScale = Vector3.one;
            BGHolder.transform.localPosition = Vector3.zero;

            for (var i = 0; i < bGSpriteSets[idx].sprites.Count; i++)
            {
                var bgImage = Instantiate(sky_prefab, BGHolder.transform);
                bgImage.SetActive(true);
                bgImage.GetComponent<BGSkyAnimator>()
                    .InitializeSkyAnimator(bGSpriteSets[idx].sprites[i], i * 5.5f + 2f, transitionTime);
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(transitionTime);
            if (previousBG != null) Destroy(previousBG);
            previousBG = BGHolder;
        }

        [Serializable]
        private class BGSpriteSet
        {
            public List<Sprite> sprites;
        }
    }
}