using DG.Tweening;
using DynamicGames.System;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    public class ItemHandler
    {
        private GameManager gameManager;
        [SerializeField] private FXManager fXManager;

        public ItemHandler(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void CreateMetheor()
        {
            if (gameManager.state != GameManager.ShootGameState.Playing) return;
            var path = new Vector3[3];
            path[0] = gameManager.island.transform.position;
            path[2] = gameManager.player.transform.position;
            path[0].z = path[2].z;

            var ydiff = Mathf.Abs(gameManager.player.transform.position.y - gameManager.island.transform.position.y);
            var xdiff = map(ydiff, 0, 5, 0.2f, 1.5f);
            path[1] = Vector3.Lerp(path[0], path[2], 0.5f);
            path[1].x = gameManager.player.position.x < 0 ? -xdiff : xdiff;

            var metheor = fXManager.CreateFX(FXType.ShadowMissile, path[0]);
            metheor.transform.DOPath(path, Random.Range(1.8f, 2.2f), PathType.CatmullRom, PathMode.TopDown2D, 1,
                    Color.red)
                .SetEase(Ease.OutQuart)
                .OnComplete(() =>
                {
                    //fXManager.KillFX(metheor.GetComponent<FX>());
                    fXManager.CreateFX(FXType.ShadowBomb, metheor.transform);
                });

            float map(float s, float a1, float a2, float b1, float b2)
            {
                return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
            }
        }

        public void GetShield()
        {
            if (gameManager.shield != null) return;
            gameManager.shield = fXManager.CreateFX(FXType.Shield, gameManager.player.transform).GetComponent<FXController>();
            gameManager.shield.gameObject.transform.SetParent(gameManager.player.transform, true);
            gameManager.shield.transform.localPosition = Vector3.zero;
        }

        public void DestroyShield()
        {
            if (gameManager.shield == null) return;
            gameManager.UIManager.itemInformationUIShield.HideUI();
            fXManager.CreateFX(FXType.ShieldPop, gameManager.shield.gameObject.transform);
            fXManager.CreateFX(FXType.Bomb, gameManager.shield.gameObject.transform);
            fXManager.KillFX(gameManager.shield);
            gameManager.audioManager.PlaySfxByTag(SfxTag.ShieldPop);
            gameManager.shield = null;
        }
    }
}