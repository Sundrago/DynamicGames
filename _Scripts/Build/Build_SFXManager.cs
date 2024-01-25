using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build_SFXManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] small;
    [SerializeField] private AudioClip[] mid;
    [SerializeField] private AudioClip[] large;
    [SerializeField] private AudioClip[] fail;

    [SerializeField]
    private AudioSource audioSource;

    private void PlayXSmallSfx()
    {
        int rnd = Random.Range(0, small.Length);
        audioSource.PlayOneShot(small[rnd], PlayerPrefs.GetFloat("settings_sfx") * Random.Range(0.3f, 0.8f));
    }
    private void PlaySmallSfx()
    {
        int rnd = Random.Range(0, small.Length);
        audioSource.PlayOneShot(small[rnd], PlayerPrefs.GetFloat("settings_sfx") * Random.Range(0.6f, 1.2f));
    }
    
    private void PlayMidSfx()
    {
        int rnd = Random.Range(0, mid.Length);
        audioSource.PlayOneShot(mid[rnd], PlayerPrefs.GetFloat("settings_sfx") * Random.Range(0.6f, 1.2f));
    }
    
    private void PlayLargeSfx()
    {
        int rnd = Random.Range(0, large.Length);
        audioSource.PlayOneShot(large[rnd], PlayerPrefs.GetFloat("settings_sfx") * Random.Range(0.6f, 1.2f));
    }
    
    public void PlayFailSfx()
    {
        int rnd = Random.Range(0, fail.Length);
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(fail[rnd], PlayerPrefs.GetFloat("settings_sfx") * Random.Range(0.6f, 1f));
    }

    public IEnumerator PlaySFXByHitSize(float size)
    {
        yield return new WaitForSeconds(Random.Range(0, 0.3f));
        if(size > 6.5f) PlayLargeSfx();
        else if(size > 4f) PlayMidSfx();
        else if(size > 1f) PlaySmallSfx();
        else if (size > 0.5f) PlayXSmallSfx();
    }
}
