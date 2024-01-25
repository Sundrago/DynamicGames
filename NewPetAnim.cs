using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class NewPetAnim : MonoBehaviour
{
    [SerializeField] private NewPetAnim_1Intro PetAnim1Intro;
    [SerializeField] private NewPetAnim_2Preview PetAnim2Preview;
    [SerializeField] private Volume PostVolumeGlow;
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private SpriteAnimator spriteAnimator;
    [SerializeField] private Image bg;
    [SerializeField] private SFXCTRL sfx;

    private bool animFinished;
    private PetType type;
    
    [Button]
    public void Init(PetType _type)
    {
        sfx.PauseBGM();
        
        type = _type;
        gameObject.SetActive(true);
        gameObject.transform.localPosition = Vector3.zero;
        animFinished = false;
        
        AudioSource.Play();
        PostVolumeGlow.weight = 0;
        PetAnim1Intro.gameObject.SetActive(false);
        PetAnim2Preview.gameObject.SetActive(false);
        bg.color = Color.white;
        bg.DOColor(Color.black, 2f);

        spriteAnimator.sprites = PetManager.Instance.GetPetDataByType(type).obj.GetComponent<Pet>().GetWalkAnim();
        
        //anim1
        PetAnim1Intro.Init(PetDialogueManager.Instance.GetWelcomeString(type));
        DOVirtual.Float(0f, 1f, 2f, (x) =>
        {
            PostVolumeGlow.weight = x;
        });

        //anim2
        float anim2Delay = 5.2f;
        DOVirtual.DelayedCall(anim2Delay, () =>
        {
            PetAnim2Preview.Init(type.ToString().ToUpper(), PetDialogueManager.Instance.GetDescrString(type), PetDialogueManager.Instance.GetRank(type));
        });
        DOVirtual.Float(1f, 0.55f, 1f, (x) =>
        {
            PostVolumeGlow.weight = x;
        }).SetDelay(anim2Delay);

        DOVirtual.DelayedCall(7f, () =>
        {
            sfx.UnPauseBGM();
            animFinished = true;
        });
    }
    
    public void Close()
    {
        if(!animFinished) return;
        
        gameObject.transform.DOMoveY(-3000, 1f).SetEase(Ease.InOutExpo)
            .OnComplete(()=>{gameObject.SetActive(false);});
        DOVirtual.Float(0.55f, 0, 0.6f, (x) =>
        {
            PostVolumeGlow.weight = x;
        });
    }
}
