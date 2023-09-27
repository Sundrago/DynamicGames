using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteAnimator : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private float interval = 1f;
    private Image image;
    private int idx;
    public bool start = true;
    public int pauseAtIdx = -1;
    
    private IEnumerator SpriteAnim()
    {
        while (start)
        {
            yield return new WaitForSeconds(interval);
            
            idx += 1;
            if (idx >= sprites.Length) idx = 0;
            image.sprite = sprites[idx];
            
            if (pauseAtIdx == idx) start = false;
        }
    }

    private void OnEnable()
    {
        pauseAtIdx = -1;
        start = true;
        image = GetComponent<Image>();
        StartCoroutine(SpriteAnim());
    }
}
