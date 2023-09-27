using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;

public class PetAnim : MonoBehaviour
{
    [SerializeField] public Sprite[] walks, jumps, idles;
    [SerializeField]
    private float interval;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private int idx = 0;
    private bool walk;
    
    public enum PetStatus { walk, jump, idle };
    private PetStatus status = PetStatus.walk;
    private bool pauseAnim = false;
    
[Button]
    public void SetStatus(PetStatus _status)
    {
        if(status == _status) return;

        idx = 0;
        status = _status;

        switch (status) 
        {
            case PetStatus.idle:
                spriteRenderer.sprite = idles[idx];
                break;
            case PetStatus.walk:
                spriteRenderer.sprite = walks[idx];
                break;
            case PetStatus.jump:
                spriteRenderer.sprite = jumps[idx];
                break;
        }
    }

    private void Start()
    {
        StartCoroutine(WalkAnim());
    }
    
    IEnumerator WalkAnim() {
        for(;;)
        {
            yield return new WaitForSeconds(interval);
            if (!pauseAnim)
            {
                idx += 1;
                switch (status) 
                {
                    case PetStatus.idle:
                        if (idx >= idles.Length) idx = 0;
                        spriteRenderer.sprite = idles[idx];
                        break;
                    case PetStatus.walk:
                        if (idx >= walks.Length) idx = 0;
                        spriteRenderer.sprite = walks[idx];
                        break;
                    case PetStatus.jump:
                        if (idx >= jumps.Length) idx = 0;
                        spriteRenderer.sprite = jumps[idx];
                        break;
                }
            }
        }
    }
}
