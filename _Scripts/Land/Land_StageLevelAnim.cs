using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Land_StageLevelAnim : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stage_lv_text;
    public int currnet_stage_lv = 1;

    public void SetLevel(int lv)
    {
        currnet_stage_lv = lv;
        if (lv <= 9) stage_lv_text.text = '0' + lv.ToString();
        else stage_lv_text.text = lv.ToString();
    }

    public void PlayAnim()
    {
        gameObject.GetComponent<Animator>().SetTrigger("show");
        AudioManager.Instance.PlaySFXbyTag(SFX_tag.rocket_newLevel);
    }

    public void HideAnim()
    {
        gameObject.GetComponent<Animator>().SetTrigger("hide");
    }

    public void NextLevel()
    {
        SetLevel(currnet_stage_lv + 1);
    }
}
