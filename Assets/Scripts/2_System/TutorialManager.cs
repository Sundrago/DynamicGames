using System;
using DG.Tweening;
using DynamicGames.MainPage;
using DynamicGames.MiniGames;
using DynamicGames.Pet;
using DynamicGames.UI;
using DynamicGames.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
#if UNITY_IOS && !UNITY_EDITOR
using Firebase.Analytics;
#endif

namespace DynamicGames.System
{
    /// <summary>
    /// Manages the tutorials in the game.
    /// </summary>
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

        [SerializeField] private GameObject gachaBlock,
            tutorialBlock,
            jumpBlock,
            pet_ticketBtn,
            pet_coinBtn,
            unlockBtn,
            keyBtn,
            firendsBlock;
        [SerializeField] private MainPage.MainPage main;
        [SerializeField] private GameObject cursor;
        [SerializeField] private Image cursor_image;
        [SerializeField] private ExclamationMarkButton exclamationMarkButtonPrefab;
        [SerializeField] private Transform mainCanvas;
        
        public static TutorialManager Instancee { get; private set; }

        private TutorialStatus status = TutorialStatus.finished;

        private int tutorialB_check;

        private void Awake()
        {
            Instancee = this;
        }

        private void Start()
        {
            if (Time.time < 2f) return;

            if (!gachaBlock.gameObject.activeSelf && BlockStatusManager.Instance.IsAllGameLocked() &&
                MoneyManager.Instance.GetCount(MoneyManager.RewardType.Ticket) == 0 &&
                status == TutorialStatus.finished)
                status = TutorialStatus.B;

            if (firendsBlock.gameObject.activeSelf && PetManager.Instance.GetPetCount(PetType.Fluffy) > 0 &&
                PlayerPrefs.GetInt("tutorialG", 0) == 0 && status == TutorialStatus.finished)
                StartTutorialG();

            if (!firendsBlock.gameObject.activeSelf && status != TutorialStatus.F &&
                PetManager.Instance.GetTotalPetCount() > 0 &&
                PlayerPrefs.GetInt("tutorialF", 0) == 0 && status == TutorialStatus.finished)
            {
                status = TutorialStatus.F;
                GotPet();
            }
        }

        private void Update()
        {
            if (Time.frameCount % 30 == 0) CursorUpdate();
            else if (Time.frameCount % 155 == 0) Start();
        }

        private void CursorUpdate()
        {
            if (tutorialBlock.activeSelf && status == TutorialStatus.Begin)
            {
                ShowCursor(tutorialBlock.transform);
                status = TutorialStatus.A;
                return;
            }

            if (status == TutorialStatus.B_key)
            {
                if (unlockBtn.activeSelf) ShowCursor(keyBtn.transform);
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
            PopupTextManager.Instance.ShowOKPopup("[tutorial_0] <rainb>다이나믹 게임천국</rainb>에 <br>온 것을 환영한다옹!", tutorial_02,
                "[tutorial_1] 반가워요!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup1);
#if UNITY_IOS && !UNITY_EDITOR
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventTutorialBegin);
#endif
        }

        private void tutorial_02()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_2]<rainb>다이나믹 게임천국</rainb>에서 다양한 미니게임을 즐겨보세옹!",
                tutorial_03);
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
        }

        private void tutorial_03()
        {
            PopupTextManager.Instance.ShowYesNoPopup("[tutorial_3]준비되었다면 미니게임을 불러올까요?", tutorial_04);
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
        }

        private void tutorial_04()
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

            DOVirtual.DelayedCall(5f, () => { ShowCursor(jumpBlock.transform); });
            status = TutorialStatus.B;
        }


        //tutorial_B
        [Button]
        public void tutorial_B01()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_B01]잠겨있는 게임을 열려면 <pend>열쇠</pend>가 필요하다옹!", tutorial_B02,
                "[tutorial_B02]열쇠요?");
            jumpBlock.GetComponent<BlockDragHandler>().hold = true;
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
        }

        private void tutorial_B02()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_B03]열쇠를 선물로 준다옹! 원하는 게임을 언락해보라옹!", tutorial_B03,
                "[tutorial_B04]고마워요!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup1);
        }

        private void tutorial_B03()
        {
            status = TutorialStatus.B_key;
            AudioManager.Instance.PlaySfxByTag(SfxTag.AcquiredCoin);
            MoneyManager.Instance.Reward2DAnimation(MoneyManager.RewardType.Key, Vector3.zero, 1);
        }

        public void DragSpriteBtnClicked()
        {
            if (status != TutorialStatus.B) return;
            if (BlockStatusManager.Instance.IsAllGameLocked() &&
                MoneyManager.Instance.GetCount(MoneyManager.RewardType.Key) == 0)
            {
                tutorialB_check += 1;
                if (tutorialB_check == 2) tutorial_B01();
            }
        }

        public void GameUnlocked()
        {
            if (status != TutorialStatus.B_key) return;
            status = TutorialStatus.C;
            HideCursor();
            DOVirtual.DelayedCall(4.5f, tutorial_C01);
        }

        //tutorial_C
        [Button]
        private void tutorial_C01()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_C01]축하한다옹!! 첫 게임을 언락했다옹!", tutorial_C02,
                "[ask4review_3]네!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup1);
        }

        private void tutorial_C02()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_C02]언락한 게임을 터치해서 플레이해보자옹!", null, "[DEFAULT_OKAY]좋아요!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
            status = TutorialStatus.C_EnterGame;
            ShowCursor(jumpBlock.transform);
        }

        //tutorial_C_2
        public void JumpGameEntered()
        {
            HideCursor();
            jumpBlock.GetComponent<BlockDragHandler>().hold = false;
            if (status == TutorialStatus.C_EnterGame)
                DOVirtual.DelayedCall(0.5f, tutorial_C02_2);
        }

        private void tutorial_C02_2()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_C02_2]첫 게임으로 들어왔다옹!! 높은 점수를 얻고 티켓을 모으라옹!", null,
                "[tutorial_C02_2a]가보자구!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
            status = TutorialStatus.D;
        }

        //tutorial_D
        [Button]
        private void tutorial_D01()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_D01]우왕, 게임을 플레이하고 티켓을 얻었다옹!", tutorial_D02,
                "[DEFAULT_YES]네!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup1);
        }

        private void tutorial_D02()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_D02]티켓으로는 새로운 게임을 언락하거나 갓챠에서 펫을 뽑을 수 있다옹!", tutorial_D03,
                "[DEFAULT_YES]네!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
        }

        private void tutorial_D03()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_D03]리더보드 블록과 갓챠 블록을 주겠다옹!", tutorial_D04);
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
        }

        private void tutorial_D04()
        {
            main.ResetBlockPositions();
            BlockStatusManager.Instance.RevealBlock(BlockStatusManager.BlockType.gacha);
            BlockStatusManager.Instance.RevealBlock(BlockStatusManager.BlockType.leaderboard);
            status = TutorialStatus.E;
        }

        public void WentBackHome()
        {
            if (status == TutorialStatus.D || !gachaBlock.gameObject.activeSelf) tutorial_D01();
            if (status == TutorialStatus.G_EnterGameWithPet_play) tutorial_G08();
        }

        //E
        private void tutorial_E01()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_E01]펫 가챠에서는 펫을 뽑을 수 있다옹!", tutorial_E02,
                "[tutorial_E01a]펫이요?");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup1);
        }

        private void tutorial_E02()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_E02]귀여운 펫을 모아보라옹!", tutorial_E03, "[tutorial_E02a]얼마에요?");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
        }

        private void tutorial_E03()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_E03]갓챠에서 펫을 한번 뽑는데는 티켓 50장이 필요하다옹", tutorial_E04,
                "[tutorial_E03a]해볼래요!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
        }

        private void tutorial_E04()
        {
            var ticketCount = MoneyManager.Instance.GetCount(MoneyManager.RewardType.Ticket);

            if (ticketCount >= 50)
                PopupTextManager.Instance.ShowOKPopup("[tutorial_E04_2]펫을 뽑아보라옹!!");
            else
                PopupTextManager.Instance.ShowOKPopup("[tutorial_E04]티켓을 줄테니 펫을 뽑아보라옹!", tutorial_E05,
                    "[tutorial_E04a]감사합니다!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
            ShowCursor(pet_ticketBtn.transform);
        }

        private void tutorial_E05()
        {
            var ticketCount = MoneyManager.Instance.GetCount(MoneyManager.RewardType.Ticket);
            status = TutorialStatus.E_Gacha;
            if (ticketCount < 50)
                MoneyManager.Instance.Reward2DAnimation(MoneyManager.RewardType.Ticket, Vector3.zero, 50 - ticketCount);
        }

        public void TicketBtnClicked()
        {
            if (status != TutorialStatus.E_Gacha) return;
            status = TutorialStatus.E_Coin;
            ShowCursor(pet_coinBtn.transform);
        }

        public void CoinBtnClicked()
        {
            if (status != TutorialStatus.E_Coin) return;
            status = TutorialStatus.F;
            HideCursor();
        }

        public void GachaponPanelOpend()
        {
            if (PetManager.Instance.GetTotalPetCount() != 0) return;
            tutorial_E01();
        }

        //tutorial_F
        [Button]
        public void tutorial_F01()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_F01]축하한다옹! 첫 펫을 뽑았다옹!!", tutorial_F02, "[DEFAULT_YES]네!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup1);
        }

        private void tutorial_F02()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_F02]펫 정보를 볼 수 있는 프렌즈 블록이다옹!", tutorial_F03A,
                "[DEFAULT_YES]네!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
        }

        private void tutorial_F03()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_F03]튜토리얼은 여기까지다옹!", tutorial_F03A, "[DEFAULT_YES]네!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
        }

        private void tutorial_F03A()
        {
            main.ResetBlockPositions();
            BlockStatusManager.Instance.RevealBlock(BlockStatusManager.BlockType.friends);
            BlockStatusManager.Instance.RevealBlock(BlockStatusManager.BlockType.tv);

            PopupTextManager.Instance.ShowOKPopup("[tutorial_F03A]티켓이 부족하면 광고를 보고 받을 수도 있다옹", tutorial_F04,
                "[DEFAULT_YES]네!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
        }

        private void tutorial_F04()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_F04]게임을 플레이하면서 더 많은 게임을 언락하고 새로운 펫을 뽑아보라옹!", tutorial_F05);
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
            status = TutorialStatus.finished;
            PlayerPrefs.SetInt("tutorialF", 1);
        }

        private void tutorial_F05()
        {
            // MoneyManager.Instance.AddTicket(MoneyManager.RewardType.Ticket, 20);
        }

        public void GotPet()
        {
            if (status == TutorialStatus.F || !firendsBlock.gameObject.activeSelf)
            {
                DOVirtual.DelayedCall(2f, () => { tutorial_F01(); });
                Start();
            }
        }

        [Button]
        public void StartTutorialG()
        {
            var fluffy = PetManager.Instance.GetPetDataByType(PetType.Fluffy);
            var exclamation = Instantiate(exclamationMarkButtonPrefab, mainCanvas);
            exclamation.Init(fluffy.obj.transform, tutorial_G01);
            fluffy.component.ignoreIdleDialogue = true;
            status = TutorialStatus.G;
        }

        [Button]
        public void tutorial_G01()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_G01]안냥! 우리 게임에서 펫으로 플레이할 수 있다는 걸 알고 있었냥?", tutorial_G02,
                "[tutorial_G01a]정말? 어떻게?");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup1);
        }

        private void tutorial_G02()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_G02]쉽다냥! 펫을 손가락으로 드래그해서 블록 위에 놓아보라냥.", tutorial_G03,
                "[tutorial_G02a]오 신기하다!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
        }

        private void tutorial_G03()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_G03]연습으로 나를 드래그해서 프렌즈 블록 위에 올려보라냥!", tutorial_G04,
                "[tutorial_G03a]좋아, 해볼게!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
            status = TutorialStatus.G;
        }

        private void tutorial_G04()
        {
            PreviewIsland.Instance.OpenIslandAndPreview(0);
            var fluffy = PetManager.Instance.GetPetDataByType(PetType.Fluffy).obj.transform;
            if (fluffy.position.y > 0)
            {
                fluffy.GetComponent<PetSurfaceMovement2D>().enabled = false;
                fluffy.position = new Vector3(0, -1, 0);
                fluffy.GetComponent<PetSurfaceMovement2D>().enabled = true;
            }
        }

        private void tutorial_G05()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_G05]잘했다냥!! 펫을 Friends 블록에 드롭하면 펫에 대한 정보를 바로 확인할 수 있다냥",
                tutorial_G06, "[tutorial_G05a]오호라!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
        }

        public void PetDrop(BlockStatusManager.BlockType type)
        {
            if (status == TutorialStatus.G && type == BlockStatusManager.BlockType.friends)
            {
                PreviewIsland.Instance.Close();
                status = TutorialStatus.G_FriendBlockClosed;
                AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup2);
                PetManager.Instance.GetPetDataByType(PetType.Fluffy).obj.GetComponent<PetObject>()
                    .ShowDialogue(Localize.GetLocalizedString("[fluffy_tutorial0]보기보다 똑똑하구냥"), true);
            }

            if (status == TutorialStatus.G_EnterGameWithPet_drag)
            {
                var flag = false;
                foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
                    if (type.ToString() == gameType.ToString())
                    {
                        flag = true;
                        break;
                    }

                if (flag)
                {
                    tutorial_G07();
                    PetManager.Instance.GetPetDataByType(PetType.Fluffy).obj.GetComponent<PetObject>()
                        .ShowDialogue(Localize.GetLocalizedString("[fluffy_tutorial1]가보자냥~!"), true);
                    DOVirtual.DelayedCall(6f, () =>
                    {
                        if (status == TutorialStatus.G_EnterGameWithPet_select)
                        {
                            status = TutorialStatus.G_EnterGameWithPet_drag;
                            PreviewIsland.Instance.OpenIslandAndPreview(1);
                        }
                    });
                }
            }
        }

        public void FriendsPanelClosed()
        {
            if (status == TutorialStatus.G_FriendBlockClosed) tutorial_G05();
            Start();
        }

        private void tutorial_G06()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_G06]이뿐만 아니라 펫으로 게임을 플레이할 수도 있다냥!!", tutorial_G06A,
                "[tutorial_G01a]정말? 어떻게?");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
        }

        private void tutorial_G06A()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_G06A]나를 게임 블록 위에 드롭해보라냥!!", tutorial_G06B,
                "[tutorial_G03a]좋아 해볼게!");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
            status = TutorialStatus.G_EnterGameWithPet_drag;
        }

        private void tutorial_G06B()
        {
            PreviewIsland.Instance.OpenIslandAndPreview(1);
        }

        private void tutorial_G07()
        {
            PreviewIsland.Instance.OpenIslandAndPreview(2);
            status = TutorialStatus.G_EnterGameWithPet_select;
        }

        private void tutorial_G08()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_G08]잘했다냥! 이렇게 펫을 게임 블록 위에 올려서 원하는 펫으로 플레이 할 수 있다냥!",
                tutorial_G09, "[tutorial_G08a]고마워");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup1);
        }

        private void tutorial_G09()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_G09]튜토리얼은 여기까지다냥. 앞으로도 새로운 기능이 업데이트 되면 또 알려주겠다냥!",
                tutorial_G10, "[tutorial_G09a]최고야");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup3);
        }

        private void tutorial_G10()
        {
            PopupTextManager.Instance.ShowOKPopup("[tutorial_G10]선물로 티켓 30장을 주겠다냥. 다이나믹 아일랜드에서 재미있는 시간 보내라냥!",
                tutorial_G11,
                "[tutorial_G08a]고마워");
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Popup2);
            status = TutorialStatus.finished;
            PetManager.Instance.GetPetDataByType(PetType.Fluffy).obj.GetComponent<PetObject>().ignoreIdleDialogue =
                false;
            PlayerPrefs.SetInt("tutorialG", 1);
#if UNITY_IOS && !UNITY_EDITOR
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventTutorialComplete);
#endif
        }

        private void tutorial_G11()
        {
            MoneyManager.Instance.Reward2DAnimation(MoneyManager.RewardType.Ticket, Vector3.zero, 30);
            PetManager.Instance.GetPetDataByType(PetType.Fluffy).obj.GetComponent<PetObject>()
                .ShowDialogue(Localize.GetLocalizedString("[fluffy_tutorial2]다른 친구들도 보고싶다냥"), true);
        }

        public void EnteredGameWithPet()
        {
            if (status == TutorialStatus.G_EnterGameWithPet_select || status == TutorialStatus.G_EnterGameWithPet_drag)
            {
                status = TutorialStatus.G_EnterGameWithPet_play;
                PreviewIsland.Instance.Close();
            }

            print("EnteredGameWithPet");
        }

        // private string tutorialStatus = "begin";
        private enum TutorialStatus
        {
            Begin,
            A,
            B,
            B_key,
            C,
            C_EnterGame,
            D,
            E,
            E_Gacha,
            E_Coin,
            F,
            G,
            G_FriendBlockClosed,
            G_EnterGameWithPet_drag,
            G_EnterGameWithPet_select,
            G_EnterGameWithPet_play,
            finished
        }
    }
}