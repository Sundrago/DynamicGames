using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Games.Jump
{
    public class GameManager : MonoBehaviour
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private SfxController sfxController;
        [SerializeField] private PetManager petManager;
        [SerializeField] private TouchInputController inputController;

        [Header("UI Components")] 
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI score_ui, highscore_ui;
        [SerializeField] private GameObject tutorial, tutorial_cursor, tutorial_text;

        [Header("Game Components")] 
        [SerializeField] private GameObject cylindar;
        [SerializeField] private GameObject player, footstepHolder;
        [SerializeField] private GameObject footstep_posA, footstep_posB, footstep_posC;
        [SerializeField] private GameObject step30, step60, step90, step180, step360;
        [SerializeField] private Transform palyerScaler;
        [SerializeField] private SpriteAnimator playerRenderer;
        [SerializeField] private GameObject playerPlaceHolder;
        [SerializeField] private ItemObject item_prefab;

        [Header("Game Play Status")]
        [SerializeField] private float footstepY_velcoity_max = 1f;
        [SerializeField] private float footstepY_velocity_min = 0.1f;
        [SerializeField] private float friction = 0.98f;
        private bool firstGame;
        private readonly List<GameObject> footsteps = new();
        private bool hasRevibed;
        private float height = -0.3f;
        private bool highFXShown;
        private float revibeTimer;

        private gameStatus status;
        private int totalStepCount;
        private float velocity;

        public int Score { get; set; }
        public int HighScore { get; private set; }

        private void Start()
        {
            title.color = new Color(1f, 1f, 1f, 0f);
            cylindar.transform.localEulerAngles = new Vector3(0f, 210f, 0f);
            tutorial.SetActive(false);
        }

        private void Update()
        {
            //Update Y Speed
            if (status == gameStatus.preGame)
            {
                if (player.transform.position.y > -0.25)
                {
                    tutorial_text.transform.DOScale(Vector3.zero, 0.5f);
                    tutorial_cursor.GetComponent<Image>().DOFade(0f, 0.5f)
                        .OnComplete(() => { tutorial.SetActive(false); });
                    SetStatus(gameStatus.playing);
                }

                UpdateFootsteps();
            }
            else if (status == gameStatus.playing)
            {
                var max = footstepY_velcoity_max * (Score + 80) / 80;
                var min = footstepY_velocity_min * (Score + 80) / 80;

                if (player.transform.position.y > -0.25f)
                {
                    if (velocity < max)
                        velocity += (max - velocity) * friction * Mathf.Abs(player.transform.position.y + 0.25f) / 2;
                }
                else
                {
                    if (velocity > min)
                        velocity += (min - velocity) * friction * Mathf.Abs(-player.transform.position.y - 0.25f) / 2;
                }

                Time.timeScale = 1 + 0.008f * Score;

                var updatePos = footstepHolder.transform.localPosition;
                updatePos.y -= velocity * Time.deltaTime;
                footstepHolder.transform.localPosition = updatePos;

                //Update Footstep Size
                UpdateFootsteps();

                //Check if game ends
                if (player.transform.position.y < -10f) SetStatus(gameStatus.dead);

                //ScoreCounter
                if (footsteps.Count > 0)
                    if (footsteps[0].transform.position.y <= footstep_posC.transform.position.y)
                    {
                        Destroy(footsteps[0]);
                        footsteps.RemoveAt(0);
                        if (footsteps.Count < 15) GenerateStageByDifficulty(0.05f, 0.15f, 0.7f);
                    }
            }
            else if (status == gameStatus.revibe)
            {
                if (Time.time > revibeTimer + 4f)
                {
                    SetStatus(gameStatus.playing);
                    player.GetComponent<Rigidbody>().isKinematic = false;
                }
            }
        }

        private void UpdateFootsteps()
        {
            foreach (var obj in footsteps)
                if (obj.transform.position.y >= footstep_posA.transform.position.y)
                {
                    obj.transform.localScale = new Vector3(0, 0, 0);
                }
                else if (obj.transform.position.y <= footstep_posB.transform.position.y)
                {
                    obj.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    var normal = 1 - (obj.transform.position.y - footstep_posB.transform.position.y) /
                        (footstep_posA.transform.position.y - footstep_posB.transform.position.y);
                    normal = EaseOutBack(normal);
                    obj.transform.localScale = new Vector3(normal, normal, normal);
                }
        }

        private void SetStatus(gameStatus _status)
        {
            if (status == _status) return;
            status = _status;

            switch (_status)
            {
                case gameStatus.preGame:
                    hasRevibed = false;
                    break;
                case gameStatus.playing:
                    break;
                case gameStatus.dead:
                    if (!hasRevibed) GameOver();
                    else ShowScore();
                    break;
                case gameStatus.revibe:
                    break;
            }
        }

        private void GameOver()
        {
            sfxController.PlaySfx(0);
            WatchAdsContinue.Instance.Init(Revibe, ShowScore, "Jump_Revibe");
        }

        private void ShowScore()
        {
            player.SetActive(false);
            Time.timeScale = 1f;
            EndScoreCtrl.Instance.ShowScore(Score, GameType.jump);
            SetupGame();
        }

        private void Revibe()
        {
            SetStatus(gameStatus.revibe);
            player.GetComponent<Rigidbody>().isKinematic = true;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player.transform.localPosition = new Vector3(0f, -5f, 1.25f);
            player.transform.DOLocalMove(new Vector3(0f, -1.5f, 1.25f), 1.5f)
                .SetEase(Ease.OutExpo);
            revibeTimer = Time.time;
            hasRevibed = true;
        }

        private float EaseOutBack(float normal)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;

            return 1 + c3 * Mathf.Pow(normal - 1, 3) + c1 * Mathf.Pow(normal - 1, 2);
        }

        public void GenerateStages()
        {
            //reset step list
            for (var i = footsteps.Count - 1; i >= 0; i--)
            {
                Destroy(footsteps[i]);
                footsteps.RemoveAt(i);
            }

            height = -0.6f;
            totalStepCount = 0;

            GenerateStage(360, 0, 0);
            if (firstGame) GenerateStage(0);
            GenerateStageByDifficulty(1f, 0f, 0f);
            GenerateStageByDifficulty(1f, 0f, 0f);
            GenerateStageByDifficulty(0.9f, 0.1f, 0f);
            GenerateStageByDifficulty(0.8f, 0.2f, 0f);
            GenerateStageByDifficulty(0.6f, 0.4f, 0f);
            GenerateStageByDifficulty(0.4f, 0.6f, 0f);
            GenerateStageByDifficulty(0.2f, 0.8f, 0f);
            GenerateStageByDifficulty(0.2f, 0.8f, 0f);
            GenerateStageByDifficulty(0.2f, 0.7f, 0.1f);
            GenerateStageByDifficulty(0.2f, 0.7f, 0.1f);
            GenerateStageByDifficulty(0f, 0.8f, 0.2f);
            GenerateStageByDifficulty(0f, 0.7f, 0.3f);
            GenerateStageByDifficulty(0f, 0.6f, 0.4f);
            GenerateStageByDifficulty(0f, 0.5f, 0.5f);
            GenerateStageByDifficulty(0f, 0.5f, 0.5f);
            GenerateStageByDifficulty(0f, 0.4f, 0.6f);
            GenerateStageByDifficulty(0f, 0.3f, 0.7f);
            GenerateStageByDifficulty(0f, 0.2f, 0.8f);
            GenerateStageByDifficulty(0f, 0.2f, 0.8f);
            GenerateStageByDifficulty(0f, 0.1f, 0.9f);
            GenerateStageByDifficulty(0f, 0.1f, 0.9f);
            GenerateStageByDifficulty(0f, 0f, 1f);
            GenerateStageByDifficulty(0f, 0f, 1f);
            GenerateStageByDifficulty(0f, 0f, 1f);
            GenerateStageByDifficulty(0f, 0f, 1f);
            GenerateStageByDifficulty(0f, 0f, 1f);
        }

        private void GenerateStageByDifficulty(float easy, float mid, float hard)
        {
            var rnd = Random.Range(0f, 1f);
            if (rnd <= easy)
            {
                var rndStage = Random.Range(0, 4);
                GenerateStage(rndStage);
            }
            else if (rnd < mid)
            {
                var rndStage = Random.Range(4, 11);
                GenerateStage(rndStage);
            }
            else
            {
                var rndStage = Random.Range(11, 17);
                GenerateStage(rndStage);
            }
        }

        private void GenerateStage(int idx)
        {
            var rnd = 0;
            switch (idx)
            {
                case 0:
                    //easy0
                    GenerateStage(90, 0, 0.15f);
                    GenerateStage(90, 45, 0.15f);
                    GenerateStage(90, 90, 0.15f);
                    GenerateStage(90, 135, 0.15f);
                    GenerateStage(90, 180, 0.15f);
                    GenerateStage(90, 225, 0.15f);
                    GenerateStage(90, 270, 0.15f);
                    GenerateStage(90, 315, 0.15f);
                    break;
                case 1:
                    //easy1
                    GenerateStage(180, 0, 0.2f);
                    GenerateStage(180, 90, 0.2f);
                    GenerateStage(180, 180, 0.2f);
                    GenerateStage(180, 270, 0.2f);
                    break;
                case 2:
                    //easy2
                    GenerateStage(180, 0, 0.2f);
                    GenerateStage(180, 270, 0.2f);
                    GenerateStage(180, 180, 0.2f);
                    GenerateStage(180, 90, 0.2f);
                    break;
                case 3:
                    //easy3
                    GenerateStage(180, 0, 0.2f);
                    GenerateStage(180, 180, 0.2f);
                    GenerateStage(180, 0, 0.2f);
                    GenerateStage(180, 180, 0.2f);
                    break;
                case 4:
                    //mid
                    GenerateStage(90, 0, 0.3f);
                    GenerateStage(90, 180, 0f);
                    GenerateStage(90, 90, 0.3f);
                    GenerateStage(90, 270, 0f);
                    GenerateStage(90, 0, 0.3f);
                    GenerateStage(90, 180, 0f);
                    GenerateStage(90, 90, 0.3f);
                    GenerateStage(90, 270, 0f);
                    break;
                case 5:
                    //mid2
                    GenerateStage(90, 0, 0.2f);
                    GenerateStage(90, 90, 0.2f);
                    GenerateStage(90, 180, 0.2f);
                    GenerateStage(90, 270, 0.2f);
                    break;
                case 6:
                    //mid3
                    GenerateStage(90, 0, 0.2f);
                    GenerateStage(90, 270, 0.2f);
                    GenerateStage(90, 180, 0.2f);
                    GenerateStage(90, 90, 0.2f);
                    break;
                case 7:
                    //mid4
                    GenerateStage(45, 0, 0.2f);
                    GenerateStage(45, 45, 0.2f);
                    GenerateStage(45, 90, 0.2f);
                    GenerateStage(45, 135, 0.2f);
                    GenerateStage(45, 180, 0.2f);
                    GenerateStage(45, 225, 0.2f);
                    GenerateStage(45, 270, 0.2f);
                    GenerateStage(45, 315, 0.2f);
                    break;
                case 8:
                    //mid-rnd
                    GenerateStage(60, 0, 0.3f);
                    GenerateStage(60, 120, 0f);
                    GenerateStage(60, 240, 0f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(180, rnd, 0.4f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(180, rnd, 0.4f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(180, rnd, 0.4f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(180, rnd, 0.4f);
                    break;
                case 9:
                    //mid-rnd2
                    GenerateStage(60, 0, 0.3f);
                    GenerateStage(60, 120, 0f);
                    GenerateStage(60, 240, 0f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(90, rnd, 0.25f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(90, rnd, 0.25f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(90, rnd, 0.25f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(90, rnd, 0.25f);
                    break;
                case 10:
                    //mid-rnd3
                    GenerateStage(60, 0, 0.3f);
                    GenerateStage(60, 120, 0f);
                    GenerateStage(60, 240, 0f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(45, rnd, 0.25f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(45, rnd, 0.25f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(45, rnd, 0.25f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(45, rnd, 0.25f);
                    break;
                case 11:
                    //hard1
                    GenerateStage(30, 0, 0.25f);
                    GenerateStage(30, 90, 0.25f);
                    GenerateStage(30, 180, 0.25f);
                    GenerateStage(30, 270, 0.25f);
                    break;
                case 12:
                    //hard2
                    GenerateStage(30, 0, 0.25f);
                    GenerateStage(30, 270, 0.25f);
                    GenerateStage(30, 180, 0.25f);
                    GenerateStage(30, 90, 0.25f);
                    break;
                case 13:
                    //hard3
                    GenerateStage(30, 0, 0.35f);
                    GenerateStage(30, 180, 0.35f);
                    GenerateStage(30, 0, 0.35f);
                    GenerateStage(30, 180, 0.35f);
                    break;
                case 14:
                    //hard4
                    GenerateStage(30, 90, 0.4f);
                    GenerateStage(30, 270, 0.4f);
                    GenerateStage(30, 90, 0.4f);
                    GenerateStage(30, 270, 0.4f);
                    break;
                case 15:
                    //hard-rnd
                    GenerateStage(60, 0, 0.3f);
                    GenerateStage(60, 120, 0f);
                    GenerateStage(60, 240, 0f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(180, rnd, 0.4f);
                    GenerateStage(90, rnd + 180, 0f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(180, rnd, 0.4f);
                    GenerateStage(90, rnd + 180, 0f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(180, rnd, 0.4f);
                    GenerateStage(90, rnd + 180, 0f);
                    break;
                case 16:
                    //hard5
                    GenerateStage(30, 0, 0.4f);
                    GenerateStage(30, 90, 0.4f);
                    GenerateStage(30, 180, 0.4f);
                    GenerateStage(30, 270, 0.4f);
                    break;
                case 17:
                    //hard-rnd2
                    rnd = Random.Range(0, 360);
                    GenerateStage(30, rnd, 0.25f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(30, rnd, 0.25f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(30, rnd, 0.25f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(30, rnd, 0.25f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(30, rnd, 0.25f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(30, rnd, 0.25f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(30, rnd, 0.25f);
                    rnd = Random.Range(0, 360);
                    GenerateStage(30, rnd, 0.25f);
                    break;
            }
        }

        public void GenerateStage(int type, int degree, float y)
        {
            GameObject nextStep;
            switch (type)
            {
                case 30:
                    nextStep = Instantiate(step30, footstepHolder.transform);
                    break;
                case 45:
                    nextStep = Instantiate(step60, footstepHolder.transform);
                    break;
                case 60:
                    nextStep = Instantiate(step60, footstepHolder.transform);
                    break;
                case 90:
                    nextStep = Instantiate(step90, footstepHolder.transform);
                    break;
                case 180:
                    nextStep = Instantiate(step180, footstepHolder.transform);
                    break;
                case 360:
                    nextStep = Instantiate(step360, footstepHolder.transform);
                    break;
                default:
                    nextStep = Instantiate(step60, footstepHolder.transform);
                    break;
            }

            height += y;
            nextStep.transform.localEulerAngles = new Vector3(0, degree, 0);
            nextStep.transform.localPosition = new Vector3(0, height, 0);

            nextStep.name = totalStepCount.ToString();
            totalStepCount += 1;

            footsteps.Add(nextStep);
            nextStep.SetActive(true);

            // Jump_Item item = Instantiate(item_prefab, nextStep.transform, true);
            // item.transform.position = Vector3.zero;
            // item.transform.localPosition = new Vector3(0.25f,0.1f,0.1f);
            // item.gameObject.SetActive(true);
        }

        public void UpdateScoreUI(int score)
        {
            score_ui.text = score.ToString();
            score_ui.transform.localScale = new Vector3(1, 1, 1);
            score_ui.transform.DOShakeScale(0.5f, 0.1f);

            HighScore = PlayerPrefs.GetInt("highscore_jump");

            if (score > HighScore)
            {
                if (!highFXShown && HighScore > 0)
                {
                    highFXShown = true;
                    EndScoreCtrl.Instance.ShowNewHighFX();
                }

                highscore_ui.transform.localScale = new Vector3(1, 1, 1);
                highscore_ui.transform.DOShakeScale(0.75f, 0.15f);
                HighScore = score;
            }

            highscore_ui.text = HighScore.ToString();
        }

        public void SetupGame()
        {
            //reset score
            DOVirtual.DelayedCall(0.2f, () => { GenerateStages(); });

            DOTween.Kill(footstepHolder.transform);
            DOTween.Kill(player.transform);

            footstepHolder.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            footstepHolder.transform.DOLocalMove(Vector3.zero, 3f)
                .SetEase(Ease.OutQuint);

            footstep_posA.transform.DOMoveY(-5f, 1.5f)
                .From().SetEase(Ease.Linear);
            footstep_posB.transform.DOMoveY(-5f, 1.5f)
                .From().SetEase(Ease.Linear)
                .SetDelay(0.5f);

            SetStatus(gameStatus.preGame);
            player.GetComponent<Rigidbody>().isKinematic = true;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player.transform.localPosition = new Vector3(0f, -5f, 1.25f);
            player.transform.DOLocalMove(new Vector3(0f, -1.5f, 1.25f), 2f)
                .SetEase(Ease.OutBack);

            if (firstGame)
            {
                cylindar.transform.localEulerAngles = new Vector3(0f, 210f, 0f);
                inputController.ResetRotation();
                title.gameObject.SetActive(true);

                if (DOTween.IsTweening(title)) DOTween.Kill(title);

                title.color = new Color(1f, 1f, 1f, 0f);
                title.DOFade(1f, 3f);
                title.DOFade(0f, 2f).SetDelay(3f).OnComplete(() => { title.gameObject.SetActive(false); });
            }
            else
            {
                cylindar.transform.DOLocalRotate(new Vector3(0f, -720, 0f), 50f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetRelative(true);
                inputController.gameObject.SetActive(false);
            }
        }

        public void StartGame()
        {
            player.SetActive(true);
            Time.timeScale = 1f;
            Score = 0;
            highFXShown = false;
            // jump_scrl.ResetRotation();
            UpdateScoreUI(Score);
            DOTween.Kill(cylindar.transform);
            player.GetComponent<Rigidbody>().isKinematic = false;
            EndScoreCtrl.Instance.HideScore();
            inputController.gameObject.SetActive(true);
            EndScoreCtrl.Instance.StartGame(GameType.jump);
        }

        public void ClearGame()
        {
            for (var i = footsteps.Count - 1; i >= 0; i--)
            {
                Destroy(footsteps[i]);
                footsteps.RemoveAt(i);
            }

            Score = 0;
            UpdateScoreUI(Score);
            gameObject.transform.localScale = new Vector3(1, 0, 1);
            player.GetComponent<Rigidbody>().isKinematic = true;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player.transform.localPosition = new Vector3(0f, -5f, 1.25f);
            EndScoreCtrl.Instance.HideScore();
            Time.timeScale = 1f;
        }

        public void BeginFirstGame()
        {
            player.SetActive(true);
            gameObject.transform.parent.gameObject.SetActive(true);
            tutorial.SetActive(false);
            firstGame = true;
            EndScoreCtrl.Instance.HideScore();
            gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), 1.5f)
                .OnComplete(() =>
                {
                    if (EndScoreCtrl.Instance.GetHighScore(GameType.jump) < 15)
                    {
                        tutorial_cursor.GetComponent<Image>().DOFade(0.5f, 0.5f);
                        tutorial_text.transform.DOScale(Vector3.one, 0.5f);
                        tutorial.SetActive(true);
                    }

                    TutorialManager.Instancee.JumpGameEntered();
                });

            SetupGame();
            DOVirtual.DelayedCall(3f, () => { StartGame(); });
            firstGame = false;
        }

        public void SetPlayer(bool playAsPet, Pet pet = null)
        {
            playerPlaceHolder.SetActive(!playAsPet);
            playerRenderer.gameObject.SetActive(playAsPet);

            if (playAsPet)
            {
                playerRenderer.sprites = pet.GetJumpAnim();
                playerRenderer.GetComponent<SpriteRenderer>().sprite = playerRenderer.sprites[0];

                playerRenderer.gameObject.transform.localRotation = pet.spriteRenderer.transform.localRotation;
                playerRenderer.gameObject.transform.localPosition = pet.spriteRenderer.transform.localPosition;
                playerRenderer.gameObject.transform.localScale = pet.spriteRenderer.transform.localScale;

                playerRenderer.interval = 0.9f / playerRenderer.sprites.Length;
            }
        }

        private enum gameStatus
        {
            preGame,
            playing,
            dead,
            revibe
        }
    }
}