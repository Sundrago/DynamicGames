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

    private GameObject previousBG = null;

    private void OnEnable()
    {
        if (previousBG == null)
        {
            StartCoroutine(CreateBG(-1, 2f));
        }
        else
        {
            if(Random.Range(0f,1f) < 0.33f) StartCoroutine(CreateBG());
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
