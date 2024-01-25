using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainCanvas : MonoBehaviour
{
    [SerializeField] SFXCTRL sfx;
    [SerializeField] transition_test transition;
    [SerializeField] GameObject build, land, jump, shoot,main;
    [SerializeField] LeaderboardManger leaderboard;
    [SerializeField] private RankingManager rangkingManager;
    [SerializeField] private GachaponManager gachaponManager;
    [SerializeField] private PetDrawer petDrawer;
    [SerializeField] private AskForUserReview askForUserReview;
    [SerializeField] List<DragSprite> dragSprites;
    [SerializeField] private GameObject ranking_ui;
    [SerializeField] private TitleDrag title;

    [SerializeField] private Jump_GameManager jumpGameManager;
    [SerializeField] private Shoot_GameManager shootGameManager;
    
    private GameObject currentGameBtn = null;
    public static MainCanvas Instance;


    private void Awake()
    {
        Instance = this;
        
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        sfx.PlayBGM(3);
    }

    public void GotoGame(BlockStatusManager.BlockType blockType, GameObject gamebtn) {
        if(ranking_ui.activeSelf) return;
        
        DOTween.Kill(gamebtn.transform);
        currentGameBtn = gamebtn;
        GameObject miniisland = currentGameBtn.GetComponent<DragSprite>().miniisland;
        if(miniisland != null)
            if(DOTween.IsTweening(miniisland.transform)) return;
        
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.click);
        
        switch(blockType) {
            case BlockStatusManager.BlockType.build : 
                PetInGameManager.Instance.EnterGame(GameType.build);
                transition.canvas_B = build;
                sfx.PlayBGM(1);
                sfx.PlaySfx(2);
                MoneyManager.Instance.HidePanel();
                break;
            case BlockStatusManager.BlockType.land : 
                PetInGameManager.Instance.EnterGame(GameType.land);
                transition.canvas_B = land;
                sfx.PlayBGM(2);
                sfx.PlaySfx(2);
                MoneyManager.Instance.HidePanel();
                break;
            case BlockStatusManager.BlockType.jump : 
                PetInGameManager.Instance.EnterGame(GameType.jump);
                jumpGameManager.ClearGame();
                transition.canvas_B = jump;
                sfx.PlayBGM(0);
                sfx.PlaySfx(2);
                MoneyManager.Instance.HidePanel();
                break;
            case BlockStatusManager.BlockType.shoot : 
                PetInGameManager.Instance.EnterGame(GameType.shoot);
                shootGameManager.ClearGame();
                transition.canvas_B = shoot;
                sfx.PlayBGM(5);
                sfx.PlaySfx(2);
                MoneyManager.Instance.HidePanel();
                break;
            case BlockStatusManager.BlockType.leaderboard :
                leaderboard.OpenLeaderboard();
                break;
            case BlockStatusManager.BlockType.gacha :
                gachaponManager.ShowPanel();
                break;
            case BlockStatusManager.BlockType.friends :
                petDrawer.ShowPanel();
                break;
            case BlockStatusManager.BlockType.review :
                askForUserReview.ShowPanel();
                break;
            case BlockStatusManager.BlockType.tutorial :
                TutorialManager.Instancee.tutorial_01();
                break;
            case BlockStatusManager.BlockType.tv :
                Ads.Instance.WatchAdsBtnClicked();
                break;
        }

        if (blockType != BlockStatusManager.BlockType.tutorial) TutorialManager.Instancee.HideCursor();

        currentGameBtn.GetComponent<Rigidbody2D>().isKinematic = true;
        if(currentGameBtn.GetComponent<DragSprite>().miniisland != null) {
            miniisland = currentGameBtn.GetComponent<DragSprite>().miniisland;
            miniisland.SetActive(true);
            miniisland.transform.localScale = new Vector3(0.2f, 0.2f, 1);
            miniisland.transform.DOScale(new Vector3(100,100,100), 1f)
                .SetEase(Ease.InExpo)
                .OnComplete(()=> {
                    transition.canvas_A = main;
                    transition.OnTransition = true;
                    
                    transition.HideCanvasA();
                    transition.OnTransitionPoint();
                });
        }
    }

    public void WentBackHome() {
        MoneyManager.Instance.ShowPanel();
        PetInGameManager.Instance.ExitGame();
        sfx.PlayBGM(3);
        if(currentGameBtn == null) return;
        currentGameBtn.GetComponent<Rigidbody2D>().isKinematic = true;

        if(currentGameBtn.GetComponent<DragSprite>().miniisland != null) {
            GameObject miniisland = currentGameBtn.GetComponent<DragSprite>().miniisland;
            DOTween.Kill(miniisland.transform);
            miniisland.transform.localScale = new Vector3(100, 100, 100);
            miniisland.transform.DOScale(new Vector3(0.2f, 0.2f, 1), 1f)
                .SetEase(Ease.OutExpo)
                .OnComplete(()=> {
                    miniisland.SetActive(false);
                    currentGameBtn.GetComponent<DragSprite>().Off();
                    currentGameBtn = null;
                    leaderboard.Start();
                    StartCoroutine(rangkingManager.UpdatetRanks());
                    TutorialManager.Instancee.TutorialD_Check();
                });
        }
    }

    public void Offall(GameObject except = null) {
        foreach(DragSprite dragSprite in dragSprites) {
            if(dragSprite == null) continue;
            if(dragSprite.gameObject == except) continue;
            if(!dragSprite.gameObject.activeSelf) continue;
            dragSprite.Off();
        }
    }

    public void Offall() {
        foreach(DragSprite dragSprite in dragSprites) {
            if(dragSprite == null) continue;
            if(!dragSprite.gameObject.activeSelf) continue;
            dragSprite.Off();
        }
    }
    
    public void ReturnToOriginalPos()
    {
        title.ReturnToOriginalPos();
        foreach(DragSprite dragSprite in dragSprites) {
            if(dragSprite == null) continue;
            if(!dragSprite.gameObject.activeSelf) continue;
            dragSprite.ReturnToOriginalPos();
        }
    }
}
