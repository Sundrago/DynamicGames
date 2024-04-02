using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] public Sprite[] sprites;
    [SerializeField] public float interval = 1f;
    private Image image = null;
    private SpriteRenderer spriteRenderer = null;
    private int idx;
    public bool start = true;
    public int pauseAtIdx = -1;
    private bool paused = false;
    
    private IEnumerator SpriteAnim()
    {
        while (start)
        {
            yield return new WaitForSeconds(interval);
            
            if(!paused) idx += 1;
            if (idx >= sprites.Length) idx = 0;
            
            if(image!=null)
                image.sprite = sprites[idx];
            if(spriteRenderer!=null)
                spriteRenderer.sprite = sprites[idx];
            
            if (pauseAtIdx == idx) start = false;
        }
    }

    public void RestartWithNoLoop()
    {
        idx = 0;
        pauseAtIdx = sprites.Length - 1;
        if (start == false)
        {
            start = true;
            StartCoroutine(SpriteAnim());
        }
    }
    
    private void OnEnable()
    {
        pauseAtIdx = -1;
        start = true;
        if(GetComponent<Image>() != null)
            image = GetComponent<Image>();
        if(GetComponent<SpriteRenderer>() != null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(SpriteAnim());
    }

    public void PauseAnim()
    {
        paused = true;
    }

    public void UnPauseAnim()
    {
        paused = false;
    }
}
