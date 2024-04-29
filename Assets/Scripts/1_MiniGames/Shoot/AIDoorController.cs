using System.Threading.Tasks;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    public class AIDoorController
    {
        private GameManager gameManager;
        [SerializeField] private Animator doorLeftAnimator;
        [SerializeField] private Animator doorRightAnimator;

        public AIDoorController(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public async Task SpawnOnLeft(int count)
        {
            if (gameManager.state != GameManager.ShootGameState.Playing) return;
            doorLeftAnimator.SetTrigger("open");
            await Task.Delay(400);
            for (var i = 0; i < count; i++)
            {
                gameManager.enemyManager.SpawnOnIsland(180, -1.5f, 0f);
                await Task.Delay(1000);
            }

            doorLeftAnimator.SetTrigger("close");
        }

        public async Task SpawnOnRight(int count)
        {
            if (gameManager.state != GameManager.ShootGameState.Playing) return;
            doorRightAnimator.SetTrigger("open");
            await Task.Delay(400);
            for (var i = 0; i < count; i++)
            {
                gameManager.enemyManager.SpawnOnIsland(0, 1.5f, 0f);
                await Task.Delay(1000);
            }

            doorRightAnimator.SetTrigger("close");
        }

        public void SetIslandAnimation(IslandState state, GameManager gameManager)
        {
            switch (state)
            {
                case IslandState.Open:
                    gameManager.IslandSizeController.OpenIsland();
                    break;
                case IslandState.Close:
                    gameManager.IslandSizeController.CloseIsland();
                    break;
            }
        }
    }
}