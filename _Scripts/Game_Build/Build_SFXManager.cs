using System.Collections;
using System.Collections.Generic;
using MyUtility;
using UnityEngine;

/// <summary>
/// Class responsible for playing sound effects based on collision size and playing fail sound effects.
/// </summary>
public class Build_SFXManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] small;
    [SerializeField] private AudioClip[] mid;
    [SerializeField] private AudioClip[] large;
    [SerializeField] private AudioClip[] fail;
    [SerializeField]
    private AudioSource audioSource;

    /// <summary>
    /// Plays sound effects based on the hit size of a collision.
    /// </summary>
    /// <param name="size">The size of the collision.</param>
    public void PlaySFXByHitSize(float size)
    {
        StartCoroutine(PlaySfxEnumerator(size));
    }

    private IEnumerator PlaySfxEnumerator(float size)
    {
        yield return new WaitForSeconds(Random.Range(0, 0.05f));

        if (size > 6.5f) PlayLargeSfx();
        else if (size > 4f) PlayMidSfx();
        else if (size > 1f) PlaySmallSfx();
        else PlayXSmallSfx();
    }
    private void PlayAudioClip(AudioClip[] clips, float volumeMin, float volumeMax)
    {
        int rnd = Random.Range(0, clips.Length);
        audioSource.PlayOneShot(clips[rnd], PlayerData.GetFloat(DataKey.settings_sfx) * Random.Range(volumeMin, volumeMax));
    }

    private void PlayXSmallSfx()
    {
        PlayAudioClip(small, 0.3f, 0.8f);
    }

    private void PlaySmallSfx()
    {
        PlayAudioClip(small, 0.6f, 1.2f);
    }

    private void PlayMidSfx()
    {
        PlayAudioClip(mid, 0.6f, 1.2f);
    }

    private void PlayLargeSfx()
    {
        PlayAudioClip(large, 0.6f, 1.2f);
    }

    public void PlayFailSfx()
    {
        int rnd = Random.Range(0, fail.Length);
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(fail[rnd], PlayerData.GetFloat(DataKey.settings_sfx) * Random.Range(0.6f, 1f));
    }
}
