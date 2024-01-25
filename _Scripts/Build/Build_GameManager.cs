using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Build_GameManager : MonoBehaviour
{
    [SerializeField] private Land_StageManager stageManager;
    //[SerializeField] EndScoreCtrl endScore;
    [SerializeField] private GameObject startingPosition;
    [SerializeField] private GameObject stageHolderPanel;
    [SerializeField] private Light2D deathLight;
    [SerializeField] private TextMeshProUGUI currentscore_text, highscore_text, bestscore_text, thisScore_text;
    [SerializeField] private GameObject shaker, islandTop, mainCanvas, hearts;

    [SerializeField] private GameObject leftEdgeObj, rightEdgeObj;
    [SerializeField] private GameObject surfaceMovement;
    [SerializeField] private Build_SFXManager sfxManager;

    [SerializeField] private GameObject tutorial;
    [SerializeField] private Image tutorialA, tutorialB;
    [SerializeField] private TextMeshProUGUI tutorialText;
    
    [SerializeField] private Transform dropEdgeY, playerHolder;
    private Pet player = null;
    
    private List<GameObject> stageItems = new List<GameObject>();
    private List<Vector2> stageItemsPos = new List<Vector2>();
    private List<GameObject> currentItems = new List<GameObject>();
    private List<Vector2> currentItemPos = new List<Vector2>();

    private bool horizonMove = false;
    private bool falling = false;
    private bool firstHit = false;
    private float firstHitHeight;
    private float leftAmount, rightAmount, horizonMoveAmount;
    private float moveSpeed;
    private float oldTime;
    private int currentStageIdx, currentScore, highScore, fallCount;

    private const float gravityScale = -1;
    private const float lightDecreaseFactor = 0.97f;
    private const float threshold = 0.015f;


    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreUI(true);
        //gameObject.SetActive(false);
    }

    public void StartGame() {
        LoadStage(true);
        currentScore = 0;
        highScore = 0;
        firstHitHeight = islandTop.GetComponent<RectTransform>().localPosition.y - islandTop.GetComponent<RectTransform>().sizeDelta.y;

        print(islandTop.GetComponent<RectTransform>().localPosition.y);
        print(islandTop.GetComponent<RectTransform>().sizeDelta.y);

        if (EndScoreCtrl.Instance.GetHighScore(GameType.build) < 5) ShowTutorial();
    }

    private void ShowTutorial()
    {
        if(tutorial.activeSelf) return;
        
        tutorial.SetActive(true);
        DOTween.Kill(tutorialA);
        DOTween.Kill(tutorialB);
        DOTween.Kill(tutorialText);
        tutorialText.DOFade(0.6f, 2f);
        tutorialA.DOFade(0.6f, 2f);
        tutorialB.DOFade(0.6f, 2f);
    }

    private void HideTutorial(float duration)
    {
        if (!tutorial.activeSelf) return;
        
        DOTween.Kill(tutorialA);
        DOTween.Kill(tutorialB);
        DOTween.Kill(tutorialText);
        tutorialText.DOFade(0, duration);
        tutorialA.DOFade(0, duration);
        tutorialB.DOFade(0, duration)
            .OnComplete(() =>
            {
                tutorial.SetActive(false);
            });
    }
    
    // Update is called once per frame
    void Update()
    {
        if(horizonMove) CalcHorizonMovement();

        if(deathLight.intensity != 0)
        {
            deathLight.intensity *= lightDecreaseFactor;
            if (deathLight.intensity <= 0.01f) deathLight.intensity = 0;
        }

        //shake on hit
        if(falling & !firstHit & stageItems.Count > 0)
        {
            if(stageItems[stageItems.Count-1].GetComponent<RectTransform>().localPosition.y + stageItems[stageItems.Count - 1].GetComponent<RectTransform>().localScale.y >= firstHitHeight - 100f)
            {
                shaker.GetComponent<Animator>().SetTrigger("up");
                firstHit = true;
                ThisScoreAnim(5);
            }
        }

        if (stageItems.Count > 0)
        {
            for (int i = stageItems.Count - 1; i >= 0; i--)
            {
                GameObject item = stageItems[i];
                if(item.transform.localPosition.y > dropEdgeY.localPosition.y)
                {
                    sfxManager.PlayFailSfx();
                    stageItems.Remove(item);
                    Destroy(item);
                    deathLight.intensity += 0.35f;
                    fallCount += 1;
                    shaker.GetComponent<Animator>().SetTrigger("small");
                    hearts.GetComponent<Build_HeartsCtrl>().SetHearts(5-fallCount);
                    if(fallCount == 1) hearts.GetComponent<Build_HeartsCtrl>().Show(true);

                    ThisScoreAnim(5-fallCount);
                }
            }
        }

        if(Time.time - oldTime > 0.5f & falling)
        {
            CheckIfFalling();
        }
    }

    public void LoadStage(bool newGame = false)
    {
        if(newGame)
        {
            currentStageIdx = 0;
            currentScore = 0;
            highScore = 0;
            UpdateScoreUI(true);
        }
        int idx = Random.Range(0, stageManager.stages.Count);

        switch (currentStageIdx)
        {
            case 0:
                idx = 0;
                break;
            case 1:
                idx = 1;
                break;
            case 2:
                idx = 0;
                break;
            case 3:
                idx = 0;
                break;
            case 4:
                break;
            case 5:
                idx = 0;
                break;
            case 6:
                idx = 1;
                break;
            case 7:
                idx = 2;
                break;
            case 8:
                idx = 1;
                break;
            case 9:
                idx = 0;
                break;
            case 10:
                break;
            case 11:
                idx = 2;
                break;
            case 12:
                idx = 1;
                break;
            case 13:
                idx = 0;
                break;
            case 14:
                break;
            case 15:
                idx = 2;
                break;
            case 16:
                idx = 3;
                break;
            case 17:
                idx = 2;
                break;
            case 18:
                idx = 3;
                break;
            case 19:
                break;
            case 20:
                idx = 2;
                break;
            default:
                idx = 3;
                break;
        }

        moveSpeed = Mathf.Lerp(1.5f,20, stageItems.Count > 150 ? 1 : stageItems.Count / 150f);
        print(moveSpeed);
        currentItems = new List<GameObject>();
        currentItemPos = new List<Vector2>();
        foreach (GameObject item in stageManager.stages[idx].GetComponent<Land_StageItemHolder>().items)
        {
            GameObject newObj = Instantiate(item, stageHolderPanel.transform);
            newObj.name = item.name;
            newObj.transform.position = new Vector2(newObj.transform.position.x , startingPosition.transform.position.y);
            newObj.GetComponent<BoxCollider2D>().enabled = false;
            newObj.GetComponent<Rigidbody2D>().gravityScale = 0;
            currentItems.Add(newObj);
            currentItemPos.Add(newObj.GetComponent<RectTransform>().localPosition);
        }
        float leftEdge = currentItems[0].GetComponent<RectTransform>().localPosition.x - currentItems[0].GetComponent<RectTransform>().sizeDelta.x/2;
        float rightEdge = currentItems[currentItems.Count-1].GetComponent<RectTransform>().localPosition.x + currentItems[currentItems.Count - 1].GetComponent<RectTransform>().sizeDelta.x/2;

        leftAmount = (leftEdgeObj.GetComponent<RectTransform>().localPosition.x - leftEdgeObj.GetComponent<RectTransform>().sizeDelta.x/2 - leftEdge);
        rightAmount = (rightEdgeObj.GetComponent<RectTransform>().localPosition.x + rightEdgeObj.GetComponent<RectTransform>().sizeDelta.x/2 - rightEdge);

        horizonMove = true;
        currentStageIdx += 1;
        fallCount = 0;
        hearts.GetComponent<Build_HeartsCtrl>().Show(false);

        CalcHorizonMovement();
    }

    private void CalcHorizonMovement()
    {
        if (!horizonMove) return;
        horizonMoveAmount = Mathf.Lerp(leftAmount, rightAmount, Mathf.Sin(Time.time * moveSpeed) / 2 + 0.5f);

        for (int i = 0; i < currentItems.Count; i++)
        {
            currentItems[i].GetComponent<RectTransform>().transform.localPosition = new Vector2(currentItemPos[i].x + horizonMoveAmount, currentItemPos[i].y);
        }
    }

    public void StopHorizonMove()
    {
        if (!horizonMove) return;
        HideTutorial(1f);
        horizonMove = false;
        falling = true;
        for (int i = currentItems.Count-1; i >= 0; i--)
        {
            GameObject item = currentItems[i];
            item.GetComponent<Rigidbody2D>().gravityScale = gravityScale;
            item.GetComponent<BoxCollider2D>().enabled = true;
            item.tag = "square";
            stageItems.Add(item);
            currentItems.RemoveAt(i);
            currentItemPos.RemoveAt(i);
        }
        //if (surfaceMovement.activeSelf) surfaceMovement.GetComponent<SurfaceMovement2D>().LoadSquare();

        hearts.GetComponent<Build_HeartsCtrl>().SetHearts(5);
    }

    private bool CheckIfFalling()
    {
        oldTime = Time.time;
        for (int i = stageItems.Count - 1; i >= 0; i--)
        {
            if (stageItemsPos.Count <= i)
            {
                UpdateOldPos();
                return true;
            }
            if (Vector2.Distance(stageItems[i].transform.position, stageItemsPos[i]) > threshold)
            {
                UpdateOldPos();
                return true;
            }
        }

        NextStage();
        falling = false;
        return false;
    }

    private void UpdateOldPos()
    {
        stageItemsPos.Clear();
        for (int i = 0; i < stageItems.Count; i++)
        {
            stageItemsPos.Add(stageItems[i].transform.position);
        }
    }

    private void NextStage()
    {
        if (fallCount >= 5)
        {
            GameFinished();
            return;
        }
        
        foreach(GameObject item in stageItems)
        {
            float height = item.GetComponent<RectTransform>().localPosition.y;
            if (firstHitHeight > height) firstHitHeight = height;
        }

        firstHit = false;
        LoadStage();

        //calculate score
        int score = 0;
        foreach(GameObject obj in stageItems) {
            int point = 0;
            if(int.TryParse(obj.name, out point)) {
                score += point;
            }
        }
        UpdateScore(score);
    }

    private void UpdateScore(int score)
    {
        currentScore = score;
        if (highScore < score) highScore = score;
        UpdateScoreUI();
    }

    private void UpdateScoreUI(bool updateAll = false)
    {
        currentscore_text.text = currentScore.ToString();
        highscore_text.text = highScore.ToString();

        if (updateAll) bestscore_text.text = PlayerPrefs.GetInt("highscore_build").ToString();
    }

    private void GameFinished()
    {
        shaker.GetComponent<Animator>().SetTrigger("large");
        mainCanvas.GetComponent<Animator>().SetTrigger("ending");
        EndScoreCtrl.Instance.ShowScore(highScore, GameType.build);
        hearts.GetComponent<Build_HeartsCtrl>().Show(false);
    }

    public void RestartGame()
    {
        ClearGame();
        mainCanvas.GetComponent<Animator>().SetTrigger("retry");
        LoadStage(true);
    }

    public void ClearGame()
    {
        print("clear game");

        foreach (GameObject obj in currentItems)
        {
            Destroy(obj);
        }

        foreach (GameObject obj in stageItems)
        {
            Destroy(obj);
        }

        stageItems.Clear();
        currentItems.Clear();
        currentItemPos.Clear();
        stageItemsPos.Clear();
        EndScoreCtrl.Instance.HideScore();
        hearts.SetActive(false);
    }

    private void ThisScoreAnim(int i) {
        if(i>=0) {
            thisScore_text.color = Color.white;
            thisScore_text.text = "+" + i.ToString();
        } else {
            thisScore_text.color = Color.black;
            thisScore_text.text = i.ToString();
        }

        thisScore_text.gameObject.transform.localScale = new Vector3(1f,1f,1f);
        thisScore_text.gameObject.transform.localEulerAngles = Vector3.zero;

        DOTween.Kill(thisScore_text);
        DOTween.Kill(thisScore_text.gameObject.transform);
        thisScore_text.gameObject.transform.DOPunchScale(new Vector3(1f,1f,1f), 0.5f)
            .SetEase(Ease.OutExpo);
        thisScore_text.gameObject.transform.DOPunchRotation(new Vector3(0f,0f,10f), 0.35f)
            .SetEase(Ease.InSine);
        thisScore_text.DOFade(0,1.5f)
            .SetDelay(1f)
            .SetEase(Ease.OutQuad);
    }
    
    [Button]
    public void SetPlayer(bool playAsPet, Pet pet = null)
    {
        if(player != null) Destroy(player);
        if (playAsPet)
        {
            player = Instantiate(pet, playerHolder);
            player.gameObject.SetActive(true);
        }
    }
}
