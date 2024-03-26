using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class SkyboxManager : MonoBehaviour
{
    [SerializeField]
    private GameObject sky_prefab;
    
    [SerializeField, TableList]
    private List<BGSpriteSet> bGSpriteSets;

    public static SkyboxManager Instance;
    
    private GameObject previousBG = null;
    private int imageIdx;
    public int weatherIdx;

    private void Awake()
    {
        GetBgIdxByTime();
        Instance = this;
    }

    private int GetBgIdxByTime()
    {
        System.DateTime time = System.DateTime.Now;

        float currentTime = time.Hour + time.Minute / 60f;
        // print("TIME : " + currentTime);

        if (currentTime < 5)
        {
            weatherIdx = 0;
            return 2;
        }
        else if (currentTime < 6.5)
        {
            weatherIdx = 1;
            return 3;
        }
        else if (currentTime < 8)
        {
            weatherIdx = 2;
            return 5;
        }
        else if (currentTime < 10)
        {
            weatherIdx = 3;
            return 1;
        }
        else if (currentTime < 12)
        {
            weatherIdx = 4;
            return 4;
        }
        else if (currentTime < 16)
        {
            weatherIdx = 5;
            return 0;
        }
        else if (currentTime < 17)
        {
            weatherIdx = 6;
            return 1;
        }
        else if (currentTime < 18)
        {
            weatherIdx = 7;
            return 5;
        }
        else if (currentTime < 19)
        {
            weatherIdx = 8;
            return 6;
        }
        else if (currentTime < 20.5)
        {
            weatherIdx = 9;
            return 3;
        }
        else if (currentTime < 22)
        {
            weatherIdx = 10;
            return 7;
        }
        else
        {
            weatherIdx = 0;
            return 2;
        }
    }
    
    private void OnEnable()
    {
        if (previousBG == null)
        {
            StartCoroutine(CreateBG(GetBgIdxByTime(), 2f));
        }
        else
        {
            if (imageIdx != GetBgIdxByTime())
            {
                imageIdx = GetBgIdxByTime();
                StartCoroutine(CreateBG(imageIdx, 2f));
                
            }
        }
    }

    [Button]
    private void CreateBGAtIdx(int idx)
    {
        StartCoroutine(CreateBG(idx));
    }
    
    private IEnumerator CreateBG(int idx = -1, float _transitionTime = 5f)
    {
        if(idx == -1) idx = Random.Range(0, bGSpriteSets.Count);
        GameObject BGHolder = new GameObject();
        BGHolder.transform.SetParent(gameObject.transform);
        BGHolder.AddComponent<RectTransform>();
        BGHolder.transform.localScale = Vector3.one;
        BGHolder.transform.localPosition = Vector3.zero;

        for (int i = 0; i < bGSpriteSets[idx].sprites.Count; i++)
        {
            GameObject bgImage = Instantiate(sky_prefab, BGHolder.transform);
            bgImage.SetActive(true);
            bgImage.GetComponent<Sky2DScroll>().Init(bGSpriteSets[idx].sprites[i], i*5.5f + 2f, _transitionTime);
            yield return new WaitForSeconds(0.5f);
        }
        
        yield return new WaitForSeconds(_transitionTime);
        if (previousBG != null)
        {
            Destroy(previousBG);
        }
        previousBG = BGHolder;
    }
    
    [Serializable]
    class BGSpriteSet
    {
        [SerializeField]
        public List<Sprite> sprites;
    }
}
