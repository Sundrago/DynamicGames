using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class TutorialManager : MonoBehaviour
{
    /*
     * TUTORIAL STATE
     * A : Clcik Q Block        | Gets Block
     * B : Try to unlock 2times | Gets Key
     * C : Unlock First Game    | none
     * D : Finish Any game      | Gets Pets Block
     * E : Gets First Pet       | Gets Leaderboard/friends block
     */
    
    public static TutorialManager Instancee;
    [SerializeField] private GameObject gachaBlock, tutorialBlock, jumpBlock, pet_ticketBtn, pet_coinBtn, unlockBtn, keyBtn;
    [SerializeField] private MainCanvas main;
    [SerializeField] private GameObject cursor;
    [SerializeField] private Image cursor_image;
    
    private string tutorialStatus = "begin";
    
    private void Awake()
    {
        Instancee = this;
        // PlayerPrefs.DeleteAll();
         
    }

    private void Update()
    {
        if(Time.frameCount%30 == 0) CursorUpdate();
    }

    private void CursorUpdate()
    {
        if (tutorialBlock.activeSelf && tutorialStatus == "begin")
        {
            ShowCursor(tutorialBlock.transform);
            tutorialStatus = null;
            return;
        }

        if (tutorialStatus == "key")
        {
            if(unlockBtn.activeSelf) ShowCursor(keyBtn.transform);
            else ShowCursor(jumpBlock.transform);
        }
    }

    private void ShowCursor(Transform transform)
    {
        cursor.transform.SetParent(transform);
        cursor.transform.localPosition = Vector3.zero;
        if (!cursor.activeSelf)
        {
            cursor.SetActive(true);
            cursor_image.color = new Color(1, 1, 1, 0);
            cursor_image.DOFade(1, 3f);
        }
    }

    public void HideCursor()
    {
        cursor.transform.SetParent(gameObject.transform);
        cursor.SetActive(false);
    }

    [Button]
    public void tutorial_01()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_0] <rainb>다이나믹 게임천국</rainb>에 <br>온 것을 환영한다옹!", tutorial_02, "[tutorial_1] 반가워요!");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup1);
    }
    
    public void tutorial_02()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_2]<rainb>다이나믹 게임천국</rainb>에서 다양한 미니게임을 즐겨보세옹!", tutorial_03);
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
    }
    
    public void tutorial_03()
    {
        PopupTextManager.Instance.ShowYesNoPopup("[tutorial_3]준비되었다면 미니게임을 불러올까요?", tutorial_04);
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
    }
    
    public void tutorial_04()
    {
        HideCursor();
        BlockStatusManager.Instance.ExplodeBlock(BlockStatusManager.BlockType.tutorial);
        DOVirtual.DelayedCall(2.1f,
            () => { BlockStatusManager.Instance.RevealBlock(BlockStatusManager.BlockType.jump); });
        DOVirtual.DelayedCall(2.4f,
            () => { BlockStatusManager.Instance.RevealBlock(BlockStatusManager.BlockType.shoot); });
        DOVirtual.DelayedCall(3f,
            () => { BlockStatusManager.Instance.RevealBlock(BlockStatusManager.BlockType.land); });
        DOVirtual.DelayedCall(3.3f,
            () => { BlockStatusManager.Instance.RevealBlock(BlockStatusManager.BlockType.build); });
        
        DOVirtual.DelayedCall(5f, ()=>
        {
           ShowCursor(jumpBlock.transform);
        });
    }
    
    
    //tutorial_B
    [Button]
    public void tutorial_B01()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_B01]잠겨있는 게임을 열려면 <pend>열쇠</pend>가 필요하다옹!", tutorial_B02, "[tutorial_B02]열쇠요?");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
    }
    public void tutorial_B02()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_B03]열쇠를 선물로 준다옹! 원하는 게임을 언락해보라옹!", tutorial_B03, "[tutorial_B04]고마워요!");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup1);
    }

    public void tutorial_B03()
    {
        tutorialStatus = "key";
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.gotCoin);
        MoneyManager.Instance.Coin2DAnim(MoneyManager.RewardType.Key, Vector3.zero, 1);
    }

    private int tutorialB_check = 0;
    public void tutorialB_Check()
    {
        if (BlockStatusManager.Instance.IsAllGameLocked() &&
            MoneyManager.Instance.GetCount(MoneyManager.RewardType.Key) == 0)
        {
            tutorialB_check += 1;
            if (tutorialB_check == 2) tutorial_B01();
        }
    }

    public void GameUnlocked()
    {
        if(tutorialStatus!="key") return;
        tutorialStatus = null;
        HideCursor();
        DOVirtual.DelayedCall(4.5f, tutorial_C01);
    }
    
    //tutorial_C
    [Button]
    public void tutorial_C01()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_C01]축하한다옹!! 첫 게임을 언락했다옹!", tutorial_C02, "[ask4review_3]네!");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup1);
    }
    
    public void tutorial_C02()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_C02]언락한 게임을 터치해서 플레이해보자옹!", null, "[DEFAULT_OKAY]좋아요!");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
        tutorialStatus = "enterGame";
        ShowCursor(jumpBlock.transform);
    }
    
    //tutorial_C_2
    public void tutorialC2_Check()
    {
        HideCursor();
        if(tutorialStatus == "enterGame")
            DOVirtual.DelayedCall(0.5f,tutorial_C02_2);
    }
    
    public void tutorial_C02_2()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_C02_2]첫 게임으로 들어왔다옹!! 높은 점수를 얻고 티켓을 모으라옹!", null, "[tutorial_C02_2a]가보자구!");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
        tutorialStatus = null;
    }
    
    //tutorial_D
    [Button]
    public void tutorial_D01()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_D01]우왕, 게임을 플레이하고 티켓을 얻었다옹!", tutorial_D02, "[DEFAULT_YES]네!");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup1);
    }
    
    public void tutorial_D02()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_D02]티켓으로는 새로운 게임을 언락하거나 갓챠에서 펫을 뽑을 수 있다옹!", tutorial_D03, "[DEFAULT_YES]네!");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
    }
    
    public void tutorial_D03()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_D03]리더보드 블록과 갓챠 블록을 주겠다옹!", tutorial_D04);
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
    }

    public void tutorial_D04()
    {
        main.ReturnToOriginalPos();
        BlockStatusManager.Instance.RevealBlock(BlockStatusManager.BlockType.gacha);
        BlockStatusManager.Instance.RevealBlock(BlockStatusManager.BlockType.leaderboard);
    }
    
    public void TutorialD_Check()
    {
        if(gachaBlock.activeSelf) return;
        tutorial_D01();
    }

    //E
    public void tutorial_E01()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_E01]펫 가챠에서는 펫을 뽑을 수 있다옹!", tutorial_E02, "[tutorial_E01a]펫이요?");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup1);
    }
    
    public void tutorial_E02()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_E02]귀여운 펫을 모아보라옹!", tutorial_E03, "[tutorial_E02a]얼마에요?");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
    }
    
    public void tutorial_E03()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_E03]갓챠에서 펫을 한번 뽑는데는 티켓 50장이 필요하다옹", tutorial_E04, "[tutorial_E03a]해볼래요!");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
    }
    
    public void tutorial_E04()
    {
        int ticketCount = MoneyManager.Instance.GetCount(MoneyManager.RewardType.Ticket);

        if (ticketCount >= 50)
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_E04_2]펫을 뽑아보라옹!!");
        }
        else
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_E04]티켓을 줄테니 펫을 뽑아보라옹!", tutorial_E05, "[tutorial_E04a]감사합니다!");
        }
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
        ShowCursor(pet_ticketBtn.transform);
    }

    public void tutorial_E05()
    {
        int ticketCount = MoneyManager.Instance.GetCount(MoneyManager.RewardType.Ticket);
        tutorialStatus = "gacha";
        if (ticketCount < 50)
        {
            MoneyManager.Instance.Coin2DAnim(MoneyManager.RewardType.Ticket, Vector3.zero, 50 - ticketCount);
        }
    }

    public void TicketBtnClicked()
    {
        if(!cursor.activeSelf) return;
        if(tutorialStatus!="gacha") return;
        ShowCursor(pet_coinBtn.transform);
    }

    public void CoinBtnClicked()
    {
        if(!cursor.activeSelf) return;
        tutorialStatus = "pet";
        HideCursor();
    }

    public void TutorialE_check()
    {
        if(PetManager.Instance.GetTotalPetCount() != 0) return;
        tutorial_E01();
    }
    
    //tutorial_F
    [Button]
    public void tutorial_F01()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_F01]축하한다옹! 첫 펫을 뽑았다옹!!", tutorial_F02, "[DEFAULT_YES]네!");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup1);
    }
    
    public void tutorial_F02()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_F02]펫 정보를 볼 수 있는 프렌즈 블록이다옹!", tutorial_F03, "[DEFAULT_YES]네!");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
    }
    
    public void tutorial_F03()
    {
        main.ReturnToOriginalPos();
        BlockStatusManager.Instance.RevealBlock(BlockStatusManager.BlockType.friends);
        BlockStatusManager.Instance.RevealBlock(BlockStatusManager.BlockType.tv);
        
        PopupTextManager.Instance.ShowOKPopup("[tutorial_F03]튜토리얼은 여기까지다옹!", tutorial_F03A, "[DEFAULT_YES]네!");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
    }
    
    public void tutorial_F03A()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_F03A]티켓이 부족하면 광고를 보고 받을 수도 있다옹", tutorial_F04, "[DEFAULT_YES]네!");
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
    }
    
    public void tutorial_F04()
    {
        PopupTextManager.Instance.ShowOKPopup("[tutorial_F04]게임을 플레이하면서 더 많은 게임을 언락하고 새로운 펫을 뽑아보라옹!", tutorial_F05);
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
    }

    public void tutorial_F05()
    {
        // MoneyManager.Instance.AddTicket(MoneyManager.RewardType.Ticket, 20);
    }
    
    public void TutorialF_Check()
    {
        if(tutorialStatus !="pet") return;
        DOVirtual.DelayedCall(2f, () =>
        {
            tutorial_F01();
        });
        tutorialStatus = null;
    }
}
