using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RocketPhysics : MonoBehaviour
{
    [SerializeField] StageManager stageManager;
    [SerializeField] StageLevelIndicator stage_level;
    [SerializeField] EndScoreCtrl endScore;
    [SerializeField] GameObject rocket, fire, rocket_dummy, big_island, puff_l, puff_r, puff_fail, circleCanvas;
    [SerializeField] GameObject RocketCanvas, MainCanvas, TransitionCanvas, ReturnMenuBtn, shake;

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

    void Start()
    {
        initialRocketRot = rocket.transform.rotation;
        initialRocketPos = rocket.transform.position;
        Application.targetFrameRate = 60;
        rb2D = rocket.GetComponent<Rigidbody2D>();
        rocket.GetComponent<Animator>().SetTrigger("hide");
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
            print("success");
        }

        //failed
        if (col_body & playing)
        {
            playing = false;
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
            Failed();
            print("fail");
        }
    }

    public void ChangeBtnState(string idx, bool pressed)
    {
        switch(idx)
        {
            case "left":
                if (!playing) return;
                btn_left = pressed;
                break;
            case "right":
                if (!playing) return;
                btn_right = pressed;
                break;
            case "up":
                if (!playing) return;
                btn_up = pressed;
                fire.SetActive(pressed);
                break;
        }
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
        rocket_dummy.transform.position = rocket.transform.position;
        rocket_dummy.transform.rotation = rocket.transform.rotation;
        rocket_dummy.SetActive(true);
        big_island.GetComponent<Animator>().SetTrigger("show");

        puff_l.transform.position = new Vector2(rocket.transform.position.x - 0.5f, puff_l.transform.position.y);
        puff_r.transform.position = new Vector2(rocket.transform.position.x + 0.5f, puff_r.transform.position.y);

        puff_l.GetComponent<Animator>().SetTrigger("puff_4");
        puff_r.GetComponent<Animator>().SetTrigger("puff_4");
        shake.GetComponent<Animator>().SetTrigger("up");
        stage_level.NextLevel();
    }

    void Failed()
    {
        puff_fail.GetComponent<Animator>().SetTrigger("puff_9");
        print(failPosition.x);
        puff_fail.transform.position = failPosition;
        puff_fail.transform.rotation = rocket.transform.rotation;

        stage_level.SetLevel(1);

        RemovceCircles();
        score = currentLevel;

        endScore.ShowScore(score, GameType.land);

        shake.GetComponent<Animator>().SetTrigger("large");
        RocketCanvas.GetComponent<Animator>().SetTrigger("ending");
        //LoadStage(0);

    }

    public void ResetRocket()
    {
        print("ResetRocket");
        rocket.GetComponent<Animator>().enabled = true;
        rocket.GetComponent<Animator>().SetTrigger("reset");
        big_island.GetComponent<Animator>().SetTrigger("hide");
        resetAnimPlaying = true;
        holdTransition = true;
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

        if(stage_level != null) stage_level.PlayAnim();
    }

    private void LoadStage(int idx)
    {
        print("load stage : " + idx);

        currentLevel = idx;
        LoadCircles(stageManager.stages[idx-1].GetComponent<stage_holder>().items);
        stage_level.SetLevel(idx);
    }

    private void LoadCircles(List<GameObject> newCircles)
    {
        foreach(GameObject circle in newCircles)
        {
            GameObject newCircle = Instantiate(circle, circleCanvas.transform);
            circlePos.Add(newCircle.transform.position);
            newCircle.transform.position = new Vector3(-10 * Random.Range(1f,1.5f), newCircle.transform.position.y, 0);
            circles.Add(newCircle);

            if (newCircle.tag == "big_circle") stage_level = newCircle.GetComponent<StageLevelIndicator>();

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
        endScore.HideScore();
        RocketCanvas.GetComponent<Animator>().SetTrigger("retry");
        RemovceCircles();
        LoadStage(1);
        rocket.GetComponent<Animator>().enabled = true;
        rocket.GetComponent<Animator>().SetTrigger("begin");
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
        print("clear game");
        rocket.transform.position = initialRocketPos;
        rocket.transform.rotation = initialRocketRot;
        RocketReady();
        StartGame();
        RemovceCircles();
        endScore.gameObject.SetActive(false);
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
        endScore.gameObject.SetActive(false);
    }
}
