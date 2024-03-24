using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using MyUtility;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;


public class Build_GameManager : MonoBehaviour, IMiniGame
{
    [Header("Managers and Controllers")] 
    [SerializeField] private Build_StageManager stageManager;
    [SerializeField] private Build_SFXManager sfxManager;
    [SerializeField] private TextAsset stageIndicesCSV;
    
    [Header("UI Components")] [SerializeField]
    private UiComponents uiComponents;

    [Header("Stage Components")] [SerializeField]
    private StageComponents stageComponents;

    [Header("GamePlay Status")] 
    private GameplayStatus gameplayStatus;

    private Pet player = null;
    private List<StageItem> stageItems = new List<StageItem>();
    private List<StageItem> currentItems = new List<StageItem>();
    private int[] stageIndices;
    
    private const float GravityScale = -1;
    private const float LightDecreaseFactor = 0.97f;
    private const float Threshold = 0.015f;
    private const int MaxHeartCount = 5;

    private GameStatus gameStatus;

    private enum GameStatus
    {
        Ready,
        HorizontalMoving,
        VerticalMoving,
        GameOver
    }

    [System.Serializable]
    public class UiComponents
    {
        public TextMeshProUGUI currentScoreText;
        public TextMeshProUGUI highScoreText;
        public TextMeshProUGUI bestScoreText;
        public TextMeshProUGUI scoreOfThisGameText;
        public TextMeshProUGUI tutorialText;
        public Image tutorialImageA;
        public Image tutorialImageB;
        public Animator shaker;
        public Animator mainCanvas;
        public GameObject islandTop;
        public GameObject tutorial;
    }

    [System.Serializable]
    public class StageComponents
    {
        public GameObject startingPosition;
        public GameObject stageHolderPanel;
        public RectTransform leftEdgeObject;
        public RectTransform rightEdgeObject;
        public Light2D deathLight;
        public Transform dropBoundaryY;
        public Transform playerHolder;
        public Build_HeartsCtrl hearts;
    }

    [System.Serializable]
    public class GameplayStatus
    {
        public bool isFirstHit;
        public bool hasRevived;
        public float firstHitHeight;
        public float leftMovementAmount;
        public float rightMovementAmount;
        public float horizontalMovementAmount;
        public float movementSpeed;
        public float oldTime;
        public int currentStageIndex;
        public int currentScore;
        public int highScore;
        public int fallCount;
    }

    private class StageItem
    {
        public GameObject Item { get; }
        public RectTransform Rect { get; }
        public Vector2 DeltaPosition { get; set; }

        public StageItem(GameObject item, RectTransform rect, Vector2 deltaPosition)
        {
            Item = item;
            Rect = rect;
            DeltaPosition = deltaPosition;
        }
    }
    void Start() => InitGame();

    void InitGame()
    {
        stageIndices = Converter.DeserializeJSON<int>(stageIndicesCSV.text);
        ResetGame();
    }
    
    public void OnGameEnter()
    {
        ResetGame();
        CalculateInitialHitHeight();
        ShowTutorialIfNeeded();
    }

    private void ResetGame()
    {
        gameplayStatus = new GameplayStatus();
        ClearStageItems(currentItems);
        ClearStageItems(stageItems);
        UpdateScoreUI(updateAll: true);
        LoadStage(isNewGame: true);
    }

    private void ClearStageItems(List<StageItem> items)
    {
        foreach (var stageItem in items)
        {
            Destroy(stageItem.Item);
        }

        items.Clear();
    }

    private void CalculateInitialHitHeight()
    {
        RectTransform islandTopRect = uiComponents.islandTop.GetComponent<RectTransform>();
        gameplayStatus.firstHitHeight = islandTopRect.localPosition.y - islandTopRect.sizeDelta.y;
    }

    #region TutorialMethods

    private void ShowTutorialIfNeeded()
    {
        if (EndScoreCtrl.Instance.GetHighScore(GameType.build) < MaxHeartCount)
        {
            ShowTutorial();
        }
    }

    private void ShowTutorial()
    {
        if (uiComponents.tutorial.activeSelf) return;

        uiComponents.tutorial.SetActive(true);
        AnimateTutorialUIElements(0.6f, 2f);
    }

    private void HideTutorial(float duration)
    {
        if (!uiComponents.tutorial.activeSelf) return;

        AnimateTutorialUIElements(0, duration, () => uiComponents.tutorial.SetActive(false));
    }

    private void AnimateTutorialUIElements(float targetOpacity, float duration, Action onCompleteAction = null)
    {
        DOTween.Kill(uiComponents.tutorialImageA);
        DOTween.Kill(uiComponents.tutorialImageB);
        DOTween.Kill(uiComponents.tutorialText);

        uiComponents.tutorialText.DOFade(targetOpacity, duration);
        uiComponents.tutorialImageA.DOFade(targetOpacity, duration);
        uiComponents.tutorialImageB.DOFade(targetOpacity, duration).OnComplete(() => onCompleteAction?.Invoke());
    }

    #endregion

    void Update()
    {
        HandleGameStatus();

        if (stageComponents.deathLight.intensity != 0)
        {
            UpdateLightIntensity();
        }

        if (stageItems.Count > 0)
        {
            ManageStageItems();
        }
    }

    private void HandleGameStatus()
    {
        switch (gameStatus)
        {
            case GameStatus.HorizontalMoving:
                CalculateHorizontalMovement();
                break;
            case GameStatus.VerticalMoving:
                HandleVerticalMovement();
                break;
            case GameStatus.GameOver:
                return;
        }
    }

    private void HandleVerticalMovement()
    {
        if (!gameplayStatus.isFirstHit & stageItems.Count > 0)
        {
            HandleFirstHit();
        }

        if (Time.time - gameplayStatus.oldTime > 0.5f)
        {
            CheckIfFalling();
        }
    }

    private void HandleFirstHit()
    {
        var lastItem = stageItems[stageItems.Count - 1];

        if (lastItem.Rect.localPosition.y + lastItem.Rect.localScale.y >= gameplayStatus.firstHitHeight - 50f)
        {
            TriggerShakerAnimation("up");
            gameplayStatus.isFirstHit = true;
            ThisScoreAnim(5);
        }
    }

    private void UpdateLightIntensity()
    {
        stageComponents.deathLight.intensity *= LightDecreaseFactor;

        if (stageComponents.deathLight.intensity <= 0.01f)
        {
            stageComponents.deathLight.intensity = 0;
        }
    }

    private void ManageStageItems()
    {
        for (int i = stageItems.Count - 1; i >= 0; i--)
        {
            HandleItemFallingOffStage(stageItems[i]);
        }
    }

    private void HandleItemFallingOffStage(StageItem item)
    {
        if (item.Rect.localPosition.y > stageComponents.dropBoundaryY.localPosition.y)
        {
            sfxManager.PlayFailSfx();
            stageItems.Remove(item);
            Destroy(item.Item);
            stageComponents.deathLight.intensity += 0.35f;
            gameplayStatus.fallCount += 1;
            TriggerShakerAnimation("small");
            UpdateHeartsStatus();

            ThisScoreAnim(5 - gameplayStatus.fallCount);
        }
    }

    private void TriggerShakerAnimation(string animationName)
    {
        uiComponents.shaker.SetTrigger(animationName);
    }

    private void UpdateHeartsStatus()
    {
        stageComponents.hearts.SetHearts(MaxHeartCount - gameplayStatus.fallCount);

        if (gameplayStatus.fallCount == 1)
        {
            stageComponents.hearts.Show(true);
        }
    }

    public void LoadStage(bool isNewGame = false)
    {
        ResetGameStatus(isNewGame);
        int idx = GetRandomStageIndex();
        UpdateMovementSpeed();
        InitializeCurrentItems(idx);
        CalculateMovementLimits();
        SetGameStatusToHorizonMove();
    }

    private void ResetGameStatus(bool newGame)
    {
        if (newGame)
        {
            gameplayStatus.currentStageIndex = 0;
            gameplayStatus.currentScore = 0;
            gameplayStatus.highScore = 0;
            UpdateScoreUI(true);
        }
    }

    private int GetRandomStageIndex()
    {
        if (gameplayStatus.currentStageIndex < stageIndices.Length)
            return stageIndices[gameplayStatus.currentStageIndex];
        else return 3;
    }

    private void UpdateMovementSpeed()
    {
        gameplayStatus.movementSpeed = Mathf.Lerp(1.5f, 20, stageItems.Count > 150 ? 1 : stageItems.Count / 150f);
    }

    private void InitializeCurrentItems(int idx)
    {
        currentItems = new List<StageItem>();
        foreach (GameObject item in stageManager.stages[idx].items)
        {
            GameObject newObj = Instantiate(item, stageComponents.stageHolderPanel.transform);
            newObj.name = item.name;
            newObj.transform.position = new Vector2(newObj.transform.position.x,
                stageComponents.startingPosition.transform.position.y);
            newObj.GetComponent<BoxCollider2D>().enabled = false;
            newObj.GetComponent<Rigidbody2D>().gravityScale = 0;

            currentItems.Add(new StageItem(newObj, newObj.GetComponent<RectTransform>(),
                newObj.GetComponent<RectTransform>().localPosition));
        }
    }

    private void CalculateMovementLimits()
    {
        float leftEdge = currentItems[0].Rect.localPosition.x - currentItems[0].Rect.sizeDelta.x / 2;
        float rightEdge = currentItems[currentItems.Count - 1].Rect.localPosition.x +
                          currentItems[currentItems.Count - 1].Rect.sizeDelta.x / 2;

        gameplayStatus.leftMovementAmount = (stageComponents.leftEdgeObject.localPosition.x -
                                             stageComponents.leftEdgeObject.sizeDelta.x / 2 - leftEdge);
        gameplayStatus.rightMovementAmount = (stageComponents.rightEdgeObject.localPosition.x +
            stageComponents.rightEdgeObject.sizeDelta.x / 2 - rightEdge);
    }

    private void SetGameStatusToHorizonMove()
    {
        gameStatus = GameStatus.HorizontalMoving;
        gameplayStatus.currentStageIndex += 1;
        gameplayStatus.fallCount = 0;
        stageComponents.hearts.Show(false);

        CalculateHorizontalMovement();
    }


    private void CalculateHorizontalMovement()
    {
        if (gameStatus != GameStatus.HorizontalMoving) return;

        gameplayStatus.horizontalMovementAmount = Mathf.Lerp(gameplayStatus.leftMovementAmount,
            gameplayStatus.rightMovementAmount, Mathf.Sin(Time.time * gameplayStatus.movementSpeed) / 2 + 0.5f);

        foreach (StageItem stageItem in currentItems)
        {
            stageItem.Rect.transform.localPosition = new Vector2(
                stageItem.DeltaPosition.x + gameplayStatus.horizontalMovementAmount,
                stageItem.DeltaPosition.y);
        }
    }

    public void StopHorizontalMovement()
    {
        if (gameStatus != GameStatus.HorizontalMoving) return;
        HideTutorial(1f);
        gameStatus = GameStatus.VerticalMoving;

        for (int stageIndex = currentItems.Count - 1; stageIndex >= 0; stageIndex--)
        {
            StageItem stageItem = currentItems[stageIndex];
            SetComponentStates(stageItem);
            currentItems.RemoveAt(stageIndex);
        }

        stageComponents.hearts.SetHearts(MaxHeartCount);
    }

    private void SetComponentStates(StageItem stageItem)
    {
        Rigidbody2D itemRigidbody2D = stageItem.Item.GetComponent<Rigidbody2D>();
        if (itemRigidbody2D != null)
        {
            itemRigidbody2D.gravityScale = GravityScale;
        }

        BoxCollider2D itemBoxCollider2D = stageItem.Item.GetComponent<BoxCollider2D>();
        if (itemBoxCollider2D != null)
        {
            itemBoxCollider2D.enabled = true;
        }

        stageItem.Item.tag = "square";
        stageItems.Add(stageItem);
    }

    private bool CheckIfFalling()
    {
        gameplayStatus.oldTime = Time.time;
        for (int stageIndex = stageItems.Count - 1; stageIndex >= 0; stageIndex--)
        {
            if (stageItems.Count <= stageIndex)
            {
                UpdateDeltaPos();
                return true;
            }

            float distance =
                Vector2.Distance(stageItems[stageIndex].Rect.position, stageItems[stageIndex].DeltaPosition);
            if (distance > Threshold)
            {
                UpdateDeltaPos();
                return true;
            }
        }

        InitializeNextStage();
        return false;
    }

    private void UpdateDeltaPos()
    {
        for (int i = 0; i < stageItems.Count; i++)
        {
            stageItems[i].DeltaPosition = stageItems[i].Rect.position;
        }
    }

    private void InitializeNextStage()
    {
        if (gameplayStatus.fallCount >= MaxHeartCount)
        {
            if (gameStatus != GameStatus.GameOver) GameFinished();
            return;
        }

        foreach (StageItem item in stageItems)
        {
            float height = item.Rect.localPosition.y;
            if (gameplayStatus.firstHitHeight > height) gameplayStatus.firstHitHeight = height;
        }

        gameplayStatus.isFirstHit = false;
        LoadStage();

        int score = CalculateScore();
        UpdateScore(score);
    }

    private int CalculateScore()
    {
        int score = 0;
        foreach (StageItem item in stageItems)
        {
            if (int.TryParse(item.Item.name, out var point))
            {
                score += point;
            }
        }
    
        return score;
    }

    private void UpdateScore(int score)
    {
        gameplayStatus.currentScore = score;
        if (gameplayStatus.highScore < score) gameplayStatus.highScore = score;
        UpdateScoreUI();
    }

    private void UpdateScoreUI(bool updateAll = false)
    {
        uiComponents.currentScoreText.text = gameplayStatus.currentScore.ToString();
        uiComponents.highScoreText.text = gameplayStatus.highScore.ToString();

        if (updateAll) uiComponents.bestScoreText.text = PlayerPrefs.GetInt("highscore_build").ToString();
    }

    private void GameFinished()
    {
        gameStatus = GameStatus.GameOver;
        TriggerShakerAnimation("large");

        if (gameplayStatus.hasRevived) ShowScore();
        else WatchAdsContinue.Instance.Init(ContinueGameAfterAdWatch, ShowScore, "Build_Revive");
    }

    private void ShowScore()
    {
        uiComponents.mainCanvas.SetTrigger("ending");
        EndScoreCtrl.Instance.ShowScore(gameplayStatus.highScore, GameType.build);
        stageComponents.hearts.Show(false);
        gameplayStatus.hasRevived = false;
    }

    private void ContinueGameAfterAdWatch()
    {
        gameplayStatus.hasRevived = true;
        stageComponents.hearts.SetHearts(MaxHeartCount);
        gameplayStatus.fallCount = 0;
        gameplayStatus.currentStageIndex -= 1;
        InitializeNextStage();
    }

    public void RestartGame()
    {
        ClearGame();
        uiComponents.mainCanvas.SetTrigger("retry");
        LoadStage(true);
    }

    public void ClearGame()
    {
        ClearStageItems(stageItems);
        ClearStageItems(currentItems);
        
        EndScoreCtrl.Instance.HideScore();
        stageComponents.hearts.gameObject.SetActive(false);
        gameplayStatus.hasRevived = false;
    }

    private void ThisScoreAnim(int i)
    {
        if (i >= 0)
        {
            uiComponents.scoreOfThisGameText.color = Color.white;
            uiComponents.scoreOfThisGameText.text = "+" + i.ToString();
        }
        else
        {
            uiComponents.scoreOfThisGameText.color = Color.black;
            uiComponents.scoreOfThisGameText.text = i.ToString();
        }

        uiComponents.scoreOfThisGameText.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        uiComponents.scoreOfThisGameText.gameObject.transform.localEulerAngles = Vector3.zero;

        DOTween.Kill(uiComponents.scoreOfThisGameText);
        DOTween.Kill(uiComponents.scoreOfThisGameText.gameObject.transform);
        uiComponents.scoreOfThisGameText.gameObject.transform.DOPunchScale(new Vector3(1f, 1f, 1f), 0.5f)
            .SetEase(Ease.OutExpo);
        uiComponents.scoreOfThisGameText.gameObject.transform.DOPunchRotation(new Vector3(0f, 0f, 10f), 0.35f)
            .SetEase(Ease.InSine);
        uiComponents.scoreOfThisGameText.DOFade(0, 1.5f)
            .SetDelay(1f)
            .SetEase(Ease.OutQuad);
    }

    public void SetPlayer(bool playAsPet, Pet pet = null)
    {
        if (player != null) Destroy(player.gameObject);
        if (playAsPet)
        {
            player = Instantiate(pet, stageComponents.playerHolder);
            player.gameObject.transform.localScale *= 350f;
            player.gameObject.SetActive(true);
        }
    }
}