using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
public class AskForUserReview : MonoBehaviour
{
    [SerializeField]
    private Transform panel;

    [SerializeField]
    private GameObject msgbox_enjoying, msgbx_yes, msgbox_no;

    [SerializeField]
    private GameObject reviewCube;
    [SerializeField]
    private TextMeshProUGUI guideText;
    [SerializeField]
    private Image bgImage;
    
    enum reviewStatus { NotRevealed, Revealed, NeverShowAgain };
    private reviewStatus status;
    
    
    private void Start()
    {
        status = (reviewStatus)PlayerPrefs.GetInt("AskForUserReviewStatus", 0);
        ChangeAndSaveStatus(status);
        gameObject.SetActive(false);


        if (status == reviewStatus.NotRevealed)
        {
            reviewCube.SetActive(false);
            return;
        }
        
        if (status == reviewStatus.Revealed)
        {
            guideText.text = "Already Did! / Don't ask again";
            reviewCube.SetActive(true);
            return;
        }
        
        if (status == reviewStatus.NeverShowAgain)
        {
            Destroy(reviewCube);
            Destroy(gameObject);
            return;
        }
        
    }
    
    public void CubeFall()
    {
        if(status != reviewStatus.NotRevealed) return;

        DOVirtual.DelayedCall(5f, () => {
            AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.cube_fall);
            ChangeAndSaveStatus(reviewStatus.Revealed);
            reviewCube.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            reviewCube.SetActive(true);
        });

    }
    
    private void ChangeAndSaveStatus(reviewStatus _status)
    {
        status = _status;
        PlayerPrefs.SetInt("AskForUserReviewStatus", (int)status);
        PlayerPrefs.Save();
    }

    public void ShowPanel()
    {
        if(gameObject.activeSelf) return;
        panel.transform.position = Vector3.zero;
        panel.transform.eulerAngles = Vector3.zero;
        bgImage.DOFade(0.2f, 1f);
        
        if(DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);

        panel.transform.DOLocalMoveY(-2500f, 1f)
            .From()
            .SetEase(Ease.OutExpo);
        panel.transform.DORotate(new Vector3(0f,0f,50f), 0.75f)
            .From()
            .SetEase(Ease.OutBack);

        gameObject.SetActive(true);
        msgbox_enjoying.SetActive(true);
        msgbox_no.SetActive(false);
        msgbx_yes.SetActive(false);

        panel.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.5f);
    }

    public void YesBtnClicked()
    {
        msgbox_enjoying.SetActive(false);
        msgbox_no.SetActive(false);
        msgbx_yes.SetActive(true);
        panel.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.5f);

    }
    
    public void NoBtnClicked()
    {
        msgbox_enjoying.SetActive(false);
        msgbox_no.SetActive(true);
        msgbx_yes.SetActive(false);
        panel.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.5f);
    }

    public void LeaveReviewBtnClicked()
    {
#if UNITY_IOS
        UnityEngine.iOS.Device.RequestStoreReview();
        Application.OpenURL("https://apps.apple.com/us/app/dynamic-games-games-on-island/id6443782791");
# endif
        HidePanel();;
    }

    public void NeverShowAgainClicked()
    {
        ChangeAndSaveStatus(reviewStatus.NeverShowAgain);
        HidePanel();
        DOVirtual.DelayedCall(2f, () => {
            FXManager.Instance.CreateFX(FXType.rocketHit, reviewCube.transform.position);
            reviewCube.SetActive(false);
            Destroy(gameObject);
        });
    }

    public void HidePanel()
    {
        if(gameObject.activeSelf == false) return;
        if(DOTween.IsTweening(panel.transform)) return;
        
        panel.transform.position = Vector3.zero;
        panel.transform.eulerAngles = Vector3.zero;

        bgImage.DOFade(0f, 1f);
        panel.transform.DORotate(new Vector3(0f,0f,100f), 0.75f)
            .SetEase(Ease.OutExpo);
        panel.transform.DOLocalMoveY(-2500f, 1f)
            .SetEase(Ease.OutExpo)
            .OnComplete(()=>{gameObject.SetActive(false);});
    }
}
