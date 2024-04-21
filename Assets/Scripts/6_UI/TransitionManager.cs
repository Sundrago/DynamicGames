using DynamicGames.MiniGames.Land;
using UnityEngine;

namespace DynamicGames.UI
{
    /// <summary>
    /// Manages the transition between different UI canvases and performs related operations.
    /// </summary>
    public class TransitionManager : MonoBehaviour
    {
        public static TransitionManager Instance;
        public GameObject canvas_A, canvas_B, canvas_transition, rocket, build, jump, shoot;
        public bool ReturnToMenu;
        public bool OnTransition;

        [SerializeField] private GameObject adj;

        private void Awake()
        {
            Instance = this;
        }

        public void StartTransition()
        {
            if (OnTransition) return;
            gameObject.GetComponent<Animator>().SetTrigger("play");
            OnTransition = true;
        }

        public void OnTransitionPoint()
        {
            adj.gameObject.SetActive(true);
            if (ReturnToMenu)
            {
                if (rocket.activeInHierarchy) rocket.GetComponent<GameManager>().ClearGame();
                if (build.activeInHierarchy) build.GetComponent<MiniGames.Build.GameManager>().ClearGame();
                if (jump.activeInHierarchy) jump.GetComponent<MiniGames.Jump.GameManager>().ClearGame();
                if (shoot.activeInHierarchy) shoot.GetComponent<MiniGames.Shoot.GameManager>().ClearGame();

                canvas_A.SetActive(false);
                canvas_B.SetActive(true);
                ReturnToMenu = false;
                OnTransition = false;

                if (canvas_B.GetComponent<MainPage.MainPage>() != null)
                    canvas_B.GetComponent<MainPage.MainPage>().ReturnToMainPage();

                return;
            }

            canvas_transition.GetComponent<Animator>().Play("transition_out");
        }

        public void HideCanvasA()
        {
            OnTransition = false;
            canvas_B.SetActive(true);
            if (canvas_B.GetComponent<Animator>() != null) canvas_B.GetComponent<Animator>().SetTrigger("play");
            canvas_A.SetActive(false);
        }
    }
}