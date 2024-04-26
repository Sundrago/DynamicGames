using System;
using DynamicGames.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DynamicGames.MiniGames.Jump
{
    public class StageDifficulty
    {
        //Represent probabilities
        public int Easy, Medium, Hard;
    }
    
    /// <summary>
    ///     Generate and manage stages in Jump Game.
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private GameManager gameManager;

        [Header("Stage Elements")] 
        [SerializeField] private TextAsset stageMapJson;
        [SerializeField] private GameObject step30, step60, step90, step180, step360;
        [SerializeField] private Transform footstepHolder;

        private StageDifficulty[] stageMap;
        private float height;
        private int totalStepCount;

        [Button]
        private void LoadData()
        {
            stageMap = Converter.DeserializeJSONToArray<StageDifficulty>(stageMapJson.text);
        }

        public void GenerateStages(bool isFirstStage)
        {
            gameManager.DestroyFootsteps();
            height = -0.6f;
            totalStepCount = 0;

            GenerateStage(360, 0, 0);
            if (isFirstStage) GenerateStageEasy(0);
            foreach (var difficulty in stageMap)
                GenerateStageByDifficulty(difficulty.Easy, difficulty.Medium, difficulty.Hard);
        }

        public void GenerateStageByDifficulty(float easy, float mid, float hard)
        {
            var rnd = Random.Range(0f, 1f);
            if (rnd <= easy)
                GenerateStageEasy();
            else if (rnd < mid)
                GenerateStageMid();
            else
                GenerateStageHard();
        }

        private void GenerateStageEasy(int idx = -1)
        {
            if (idx == -1) idx = Random.Range(0, 4);
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
            }
        }

        private void GenerateStageMid(int idx = -1)
        {
            if (idx == -1) idx = Random.Range(0, 7);
            int rnd;
            switch (idx)
            {
                case 0:
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
                case 1:
                    //mid2
                    GenerateStage(90, 0, 0.2f);
                    GenerateStage(90, 90, 0.2f);
                    GenerateStage(90, 180, 0.2f);
                    GenerateStage(90, 270, 0.2f);
                    break;
                case 2:
                    //mid3
                    GenerateStage(90, 0, 0.2f);
                    GenerateStage(90, 270, 0.2f);
                    GenerateStage(90, 180, 0.2f);
                    GenerateStage(90, 90, 0.2f);
                    break;
                case 3:
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
                case 4:
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
                case 5:
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
                case 6:
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
            }
        }

        private void GenerateStageHard(int idx = -1)
        {
            if (idx == -1) idx = Random.Range(0, 6);
            int rnd;
            switch (idx)
            {
                case 0:
                    //hard1
                    GenerateStage(30, 0, 0.25f);
                    GenerateStage(30, 90, 0.25f);
                    GenerateStage(30, 180, 0.25f);
                    GenerateStage(30, 270, 0.25f);
                    break;
                case 1:
                    //hard2
                    GenerateStage(30, 0, 0.25f);
                    GenerateStage(30, 270, 0.25f);
                    GenerateStage(30, 180, 0.25f);
                    GenerateStage(30, 90, 0.25f);
                    break;
                case 2:
                    //hard3
                    GenerateStage(30, 0, 0.35f);
                    GenerateStage(30, 180, 0.35f);
                    GenerateStage(30, 0, 0.35f);
                    GenerateStage(30, 180, 0.35f);
                    break;
                case 3:
                    //hard4
                    GenerateStage(30, 90, 0.4f);
                    GenerateStage(30, 270, 0.4f);
                    GenerateStage(30, 90, 0.4f);
                    GenerateStage(30, 270, 0.4f);
                    break;
                case 4:
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
                case 5:
                    //hard5
                    GenerateStage(30, 0, 0.4f);
                    GenerateStage(30, 90, 0.4f);
                    GenerateStage(30, 180, 0.4f);
                    GenerateStage(30, 270, 0.4f);
                    break;
                case 6:
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

            gameManager.AddFootstepObject(nextStep);
            nextStep.SetActive(true);
        }
    }
}