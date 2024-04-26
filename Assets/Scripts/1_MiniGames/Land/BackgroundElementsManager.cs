using System.Collections.Generic;
using UnityEngine;

namespace DynamicGames.MiniGames.Land
{
    /// <summary>
    ///     Manages the background elements in the Land game.
    /// </summary>
    public class BackgroundElementsManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Transform circleCanvas;
        private readonly List<Vector2> circlePositions = new();
        private readonly List<GameObject> circles = new();

        public void ProcessCircles(Transform rocket)
        {
            if (circles.Count <= 0) return;
            var x = rocket.localPosition.x / Screen.width;
            var y = rocket.localPosition.y / Screen.height;

            DestroyCirclesOutOfBounds();
            MoveCircles(x, y);
        }

        private void DestroyCirclesOutOfBounds()
        {
            for (var i = circles.Count - 1; i > 0; i--)
                if (circles[i].transform.position.x > 8f)
                    if (circlePositions[i].x > 8f)
                    {
                        Destroy(circles[i]);
                        circles.Remove(circles[i]);
                        circlePositions.Remove(circlePositions[i]);
                    }
        }

        private void MoveCircles(float x, float y)
        {
            for (var i = 0; i < circles.Count; i++)
            {
                Vector2 targetPos, newPos;
                var lerpFactor = 20;

                if (circles[i].CompareTag("ui"))
                {
                    targetPos = circlePositions[i];
                    lerpFactor = 7;
                }
                else
                {
                    targetPos = circlePositions[i];
                    targetPos.x -= x * circles[i].GetComponent<RectTransform>().sizeDelta.x / 800;
                    targetPos.y -= y * circles[i].GetComponent<RectTransform>().sizeDelta.x / 800;
                }

                if (gameManager.ResetAnimPlaying & gameManager.HoldTransition) continue;

                newPos.x = circles[i].transform.position.x +
                           (targetPos.x - circles[i].transform.position.x) / lerpFactor;
                newPos.y = circles[i].transform.position.y +
                           (targetPos.y - circles[i].transform.position.y) / lerpFactor;
                circles[i].transform.position = new Vector3(newPos.x, newPos.y, 0);
            }
        }

        public void LoadCircles(List<GameObject> newCircles)
        {
            foreach (var circle in newCircles)
            {
                var newCircle = Instantiate(circle, circleCanvas);
                circlePositions.Add(newCircle.transform.position);
                newCircle.transform.position =
                    new Vector3(-10 * Random.Range(1f, 1.5f), newCircle.transform.position.y, 0);
                circles.Add(newCircle);

                if (newCircle.tag == "big_circle")
                    gameManager.stageLevelAnimator = newCircle.GetComponent<StageLevelAnimator>();
            }
        }

        public void RemoveCircles(bool dontRemoveUI = false)
        {
            for (var i = 0; i < circles.Count; i++)
            {
                if (dontRemoveUI & (circles[i].tag == "ui")) continue;
                circlePositions[i] = new Vector3(10f * Random.Range(1f, 1.5f), circlePositions[i].y, 0);
            }
        }
    }
}