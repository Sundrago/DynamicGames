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
    
    private GameObject currentGameBtn = null;

    void Start()
    {
        Application.targetFrameRate = 60;
        sfx.PlayBGM(3);
    }

    public void GotoGame(string name, GameObject gamebtn) {
        if(ranking_ui.activeSelf) return;
        
        DOTween.Kill(gamebtn.transform);
        currentGameBtn = gamebtn;
        GameObject miniisland = currentGameBtn.GetComponent<DragSprite>().miniisland;
        if(miniisland != null)
            if(DOTween.IsTweening(miniisland.transform)) return;
        
        switch(name) {
            case "build" : 
                transition.canvas_B = build;
                sfx.PlayBGM(1);
                sfx.PlaySfx(2);
                MoneyManager.Instance.HidePanel();
                break;
            case "land" : 
                transition.canvas_B = land;
                sfx.PlayBGM(2);
                sfx.PlaySfx(2);
                MoneyManager.Instance.HidePanel();
                break;
            case "jump" : 
                transition.canvas_B = jump;
                sfx.PlayBGM(0);
                sfx.PlaySfx(2);
                MoneyManager.Instance.HidePanel();
                break;
            case "shoot" : 
                transition.canvas_B = shoot;
                sfx.PlayBGM(5);
                sfx.PlaySfx(2);
                MoneyManager.Instance.HidePanel();
                break;
            case "leaderboard" :
                leaderboard.OpenLeaderboard();
                break;
            case "gacha" :
                gachaponManager.ShowPanel();
                break;
            case "friends" :
                petDrawer.ShowPanel();
                break;
            case "review" :
                askForUserReview.ShowPanel();
                break;
        }
        

        currentGameBtn.GetComponent<Rigidbody2D>().isKinematic = true;
        if(currentGameBtn.GetComponent<DragSprite>().miniisland != null) {
            print("mini anim");
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
                });
        }
    }

    public void Offall(GameObject except = null) {
        foreach(DragSprite dragSprite in dragSprites) {
            if(dragSprite == null) continue;
            if(dragSprite.gameObject == except) continue;
            dragSprite.Off();
        }
    }

    public void ReturnToOriginalPos() {
        foreach(DragSprite dragSprite in dragSprites) {
            if(dragSprite == null) continue;
            dragSprite.ReturnToOriginalPos();
        }
    }
}
