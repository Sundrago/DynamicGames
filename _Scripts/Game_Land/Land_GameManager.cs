using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Land_GameManager : SerializedMonoBehaviour
{
    [SerializeField] private Land_StageManager stageManager;
    [SerializeField] private Land_StageLevelAnim stage_level;
    [SerializeField] private GameObject rocket, rocket_dummy, big_island, puff_l, puff_r, puff_fail, circleCanvas;
    [SerializeField] private GameObject RocketCanvas, shake;
    [SerializeField] private GameObject tutorial;
    [SerializeField] private Image tutorialA, tutorialB;
    [SerializeField] private TextMeshProUGUI tutorialText, clearText;
    [SerializeField] private IslandSizeCtrl islandSizeCtrl;

    [SerializeField] private SpriteAnimator playerRenderer;
    [SerializeField] private GameObject playerPlaceHolder;
    [SerializeField] private PetManager petManager;
    
    [SerializeField] private ParticleSystem thrustFX, thrustFX2, thrust_left, thrust_right;
    private ParticleSystem.EmissionModule thrustEmission, thrustEmission2, thrustLeftEmission, thrustRightEmission;
    
    public Vector2 failPosition;
    public Quaternion failRotation;
    public bool btn_up = false;
    public bool btn_left = false;
    public bool btn_right = false;

    private const float deltaTimeVelocity = 50f;
    private Rigidbody2D rb2D;
    private List<GameObject> circles = new List<GameObject>();
    private List<Vector2> circlePos = new List<Vector2>();

    private bool playing = true;
    private bool resetAnimPlaying = false;
    private bool holdTransition = false;
    private bool col_body = false;
    private bool col_left = false;
    private bool col_right = false;

    private int score, highscore;
    private int currentLevel;

    Vector3 initialRocketPos;
    Quaternion initialRocketRot;

    [SerializeField]
    private AudioSource sfx_launch, sfx_middle, sfx_small;
    private bool isInitialLaunch = true;
    private bool hasRevibed = false;
    
    //
    // private enum GameStatus
    // {
    //     pre, playing, dead, revibe 
    //     
    // }
    //
    // private GameStatus status;
    //
    // private void SetStatus(GameStatus _status)
    // {
    //     if(status != _status) return;
    //
    //     status = _status;
    //     switch (status)
    //     {
    //         case GameStatus.pre:
    //             break;
    //         case GameStatus.playing:
    //             break;
    //         case GameStatus.dead:
    //             Failed();
    //             break;
    //         case GameStatus.revibe:
    //             break;
    //     }
    // }
    //
    void Start()
    {
        tutorialA.color = new Color(1, 1, 1, 0);
        tutorialB.color = new Color(1, 1, 1, 0);
        clearText.color = new Color(1, 1, 1, 0);
        tutorial.SetActive(false);
        
        Application.targetFrameRate = 60;
        thrustEmission = thrustFX.emission;
        thrustEmission2 = thrustFX2.emission;
        thrustLeftEmission = thrust_left.emission;
        thrustRightEmission = thrust_right.emission;
        initialRocketRot = rocket.transform.rotation;
        initialRocketPos = rocket.transform.position;
        rb2D = rocket.GetComponent<Rigidbody2D>();
        rocket.GetComponent<Animator>().SetTrigger("hide");
        tutorial.SetActive(false);
    }

    void Update()
    {
        if (circles.Count > 0)
        {
            float x = rocket.transform.localPosition.x / Screen.width;
            float y = rocket.transform.localPosition.y / Screen.height;

            for (int i = circles.Count - 1; i > 0; i--) {
                if(circles[i].transform.position.x > 8f)
                {
                    if(circlePos[i].x > 8f) {
                        Destroy(circles[i]);
                        circles.Remove(circles[i]);
                        circlePos.Remove(circlePos[i]);
                    }
                }
            }

            for (int i = 0; i < circles.Count; i++)
            {
                Vector2 targetPos, newPos;
                int lerpFactor = 20;

                if (circles[i].tag == "ui")
                {
                    targetPos = circlePos[i];
                    lerpFactor = 7;
                }
                else
                {
                    targetPos = circlePos[i];
                    targetPos.x -= x * circles[i].GetComponent<RectTransform>().sizeDelta.x / 800;
                    targetPos.y -= y * circles[i].GetComponent<RectTransform>().sizeDelta.x / 800;
                }

                if (resetAnimPlaying & holdTransition) continue;

                newPos.x = circles[i].transform.position.x + (targetPos.x - circles[i].transform.position.x) / lerpFactor;
                newPos.y = circles[i].transform.position.y + (targetPos.y - circles[i].transform.position.y) / lerpFactor;
                circles[i].transform.position = new Vector3(newPos.x, newPos.y, 0);
            }
        }

        if (!playing & !resetAnimPlaying) return;

        //reset game animation handler
        if (resetAnimPlaying)
        {
            if (rocket.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name != "rocket_reset") return;
            Vector2 targetPosition = rocket_dummy.transform.position;
            targetPosition.x -= (targetPosition.x - rocket.transform.position.x) / 10f;
            targetPosition.y -= (targetPosition.y - rocket.transform.position.y) / 10f;
            rocket_dummy.transform.position = targetPosition;
            rocket_dummy.transform.rotation = rocket.transform.rotation;
            return;
            //lerp anim
        }

        //btnEvent handler
        if (btn_up & playing)
        {
            rb2D.AddForce(rb2D.transform.up * 1.25f);
        }
        if (btn_left & playing)
        {
            rb2D.rotation += 2.5f * Time.deltaTime * deltaTimeVelocity;
        }
        if (btn_right & playing)
        {
            rb2D.rotation -= 2.5f * Time.deltaTime * deltaTimeVelocity;
        }

        //success
        if (col_left & col_right & playing)
        {
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
            playing = false;
            Succeed();
        }

        //failed
        if (col_body & playing)
        {
            playing = false;
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
            // SetStatus(GameStatus.dead);
            Failed();
        }
    }

    public void ChangeBtnState(string idx, bool pressed)
    {
        if(pressed) playerRenderer.UnPauseAnim();
        else playerRenderer.PauseAnim();
        switch(idx)
        {
            case "left":
                if (!playing) return;
                thrustRightEmission.rateOverTime = pressed?20:0;
                btn_left = pressed;
                break;
            case "right":
                if (!playing) return;
                thrustLeftEmission.rateOverTime = pressed?20:0;
                btn_right = pressed;
                break;
            case "up":
                if (!playing) return;
                btn_up = pressed;
                SetThrustParticleFXEmission(pressed);
                if (pressed)
                {
                    if (isInitialLaunch)
                    {
                        print("INITIAL");
                        PlaySFX(sfx_launch);
                        shake.GetComponent<Animator>().enabled = false;
                        shake.transform.DOPunchPosition(new Vector3(3f, 3f, 0f), 2.5f, 7)
                            .OnComplete(() => {
                                shake.GetComponent<Animator>().enabled = true;
                                HideTutorial(2);
                            });
                        isInitialLaunch = false;
                    } else 
                        PlaySFX(sfx_middle);
                }
                break;
        }
        
        if(!pressed) KillAllSFX();
    }
    
    private void SetThrustParticleFXEmission(bool isOn)
    {
        thrustEmission.rateOverTime = isOn ? 60 : 0;
        thrustEmission2.rateOverTime = isOn ? 20 : 0;
        thrustLeftEmission.rateOverTime = 0;
        thrustRightEmission.rateOverTime = 0;
    }

    private void PlaySFX(AudioSource _audioSource)
    {
        if (DOTween.IsTweening(_audioSource)) DOTween.Kill(_audioSource);
        _audioSource.volume = PlayerPrefs.GetFloat("settings_sfx");
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.Play();
    }
    private void KillAllSFX()
    {
        KillSFX(sfx_launch);
        KillSFX(sfx_middle);
        KillSFX(sfx_small);
        KillSFX(sfx_small);
    }
    
    private void KillSFX(AudioSource _audioSource)
    {
        //sfx_launch, sfx_middle, sfx_small;
        if(_audioSource.volume != 0 && !DOTween.IsTweening(_audioSource))
            _audioSource.DOFade(0, 1f);
    }

    public void ChangeColliderState(string idx, bool collided)
    {
        switch (idx)
        {
            case "left":
                col_left = collided;
                break;
            case "right":
                col_right = collided;
                break;
            case "body":
                col_body = collided;
                break;
        }
    }

    void Succeed()
    {
        playerRenderer.PauseAnim();
        rocket_dummy.transform.position = rocket.transform.position;
        rocket_dummy.transform.rotation = rocket.transform.rotation;
        rocket_dummy.SetActive(true);
        // big_island.GetComponent<Animator>().SetTrigger("show");
        islandSizeCtrl.OpenIsland();
        clearText.DOFade(1, 1f)
            .OnComplete(ResetRocket);

        puff_l.transform.position = new Vector2(rocket.transform.position.x - 0.5f, puff_l.transform.position.y);
        puff_r.transform.position = new Vector2(rocket.transform.position.x + 0.5f, puff_r.transform.position.y);

        puff_l.GetComponent<Animator>().SetTrigger("puff_4");
        puff_r.GetComponent<Animator>().SetTrigger("puff_4");
        shake.GetComponent<Animator>().SetTrigger("up");
        stage_level.NextLevel();
        AudioManager.Instance.PlaySFXbyTag(SfxTag.rocket_clear);
        SetThrustParticleFXEmission(false);
    }

    void Failed()
    {
        playerRenderer.PauseAnim();
        puff_fail.GetComponent<Animator>().SetTrigger("puff_9");
        puff_fail.transform.position = failPosition;
        puff_fail.transform.rotation = rocket.transform.rotation;
        
        FXManager.Instance.CreateFX(FXType.rocketHit, failPosition);
        RemovceCircles();
        shake.GetComponent<Animator>().SetTrigger("large");
        SetThrustParticleFXEmission(false);
        //LoadStage(0);
        
        if(hasRevibed) ShowScore();
        else WatchAdsContinue.Instance.Init(Revibe, ShowScore, "Land_Revibe");
    }

    private void ShowScore()
    {
        stage_level.SetLevel(1);
        score = currentLevel;
        EndScoreCtrl.Instance.ShowScore(score, GameType.land);
        RocketCanvas.GetComponent<Animator>().SetTrigger("ending");
        hasRevibed = false;
    }

    private void Revibe()
    {
        hasRevibed = true;
        currentLevel -= 1;
        ResetRocket();
    }

    public void ResetRocket()
    {
        playerRenderer.PauseAnim();
        rocket.GetComponent<Animator>().enabled = true;
        rocket.GetComponent<Animator>().SetTrigger("reset");
        // big_island.GetComponent<Animator>().SetTrigger("hide");
        islandSizeCtrl.CloseIsland();
        clearText.DOFade(0, 1f);
        resetAnimPlaying = true;
        holdTransition = true;
        SetThrustParticleFXEmission(false);
        PlaySFX(sfx_small);
    }

    public void OpenNextStage()
    {
        print("open stage : " + (currentLevel + 1));
        holdTransition = false;
        RemovceCircles();
        LoadStage(currentLevel + 1);
    }

    public void RocketReady()
    {
        print("RocketReady");
        rb2D.constraints = RigidbodyConstraints2D.None;
        rocket.GetComponent<Animator>().enabled = false;
        resetAnimPlaying = false;

        playing = true;

        btn_up = false;
        btn_left = false;
        btn_right = false;

        col_body = false;
        col_left = false;
        col_right = false;
        SetThrustParticleFXEmission(false);

        if(stage_level != null) stage_level.PlayAnim();
        isInitialLaunch = true;
    }

    private void LoadStage(int idx)
    {
        print("load stage : " + idx);

        currentLevel = idx;
        LoadCircles(stageManager.stages[idx-1].GetComponent<Land_StageItemHolder>().items);
        stage_level.SetLevel(idx);
        
        if(idx > 1) HideTutorial(1);
        else
        {
            if(EndScoreCtrl.Instance.GetHighScore(GameType.land) <= 1 && EndScoreCtrl.Instance.GetHighScore(GameType.land) < 4) ShowTutorial();
        }
    }

    private void LoadCircles(List<GameObject> newCircles)
    {
        foreach(GameObject circle in newCircles)
        {
            GameObject newCircle = Instantiate(circle, circleCanvas.transform);
            circlePos.Add(newCircle.transform.position);
            newCircle.transform.position = new Vector3(-10 * Random.Range(1f,1.5f), newCircle.transform.position.y, 0);
            circles.Add(newCircle);

            if (newCircle.tag == "big_circle") stage_level = newCircle.GetComponent<Land_StageLevelAnim>();

            print("add");
        }
    }

    private void RemovceCircles(bool dontRemoveUI = false)
    {
        print("remove Circles");
        for(int i = 0; i < circles.Count; i++) {
            if (dontRemoveUI & circles[i].tag == "ui") continue;
            circlePos[i] = new Vector3(10f * Random.Range(1f, 1.5f), circlePos[i].y, 0);
        }
    }

    public void PlayAgain()
    {
        EndScoreCtrl.Instance.HideScore();
        RocketCanvas.GetComponent<Animator>().SetTrigger("retry");
        RemovceCircles();
        LoadStage(1);
        rocket.GetComponent<Animator>().enabled = true;
        rocket.GetComponent<Animator>().SetTrigger("begin");
        isInitialLaunch = true;
    }

    //public void ReturnToMenu()
    //{
    //    TransitionCanvas.GetComponent<transition_test>().canvas_A = RocketCanvas;
    //    TransitionCanvas.GetComponent<transition_test>().canvas_B = MainCanvas;
    //    TransitionCanvas.GetComponent<transition_test>().PreviouseMenuBtn = ReturnMenuBtn;
    //    TransitionCanvas.GetComponent<transition_test>().ReturnToMenu = true;
    //    TransitionCanvas.GetComponent<Animator>().SetTrigger("start");
    //}

    public void ClearGame()
    {
        playerRenderer.PauseAnim();
        print("clear game");
        rocket.transform.position = initialRocketPos;
        rocket.transform.rotation = initialRocketRot;
        RocketReady();
        StartGame();
        RemovceCircles();
        EndScoreCtrl.Instance.HideScore();
        isInitialLaunch = true;
        hasRevibed = false;
    }

    public void RestartGame()
    {
        StartGame();
    }

    public void StartGame()
    {
        print("game started");
        playing = true;
        LoadStage(1);
        stage_level.PlayAnim();
        score = 0;
        EndScoreCtrl.Instance.HideScore();
        isInitialLaunch = true;
        hasRevibed = false;
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
    
    [Button]
    public void SetPlayer(bool playAsPet, Pet pet = null)
    {
        playerPlaceHolder.SetActive(!playAsPet);
        playerRenderer.gameObject.SetActive(playAsPet);
        
        if (playAsPet)
        {
            playerRenderer.sprites = pet.GetShipAnim();
            playerRenderer.GetComponent<Image>().sprite = playerRenderer.sprites[0];

            playerRenderer.gameObject.transform.localRotation = pet.spriteRenderer.transform.localRotation;

            if (CustomPetPos.ContainsKey(pet.GetType()))
            {
                playerRenderer.gameObject.transform.localPosition = CustomPetPos[pet.GetType()];
            }
            else
            {
                playerRenderer.gameObject.transform.localPosition = pet.spriteRenderer.transform.localPosition;
            }
            playerRenderer.gameObject.transform.localScale = pet.spriteRenderer.transform.localScale;

            playerRenderer.interval = 0.9f / playerRenderer.sprites.Length;
        }
    }
    
    [SerializeField]
    Dictionary<PetType, Vector3> CustomPetPos;

}
