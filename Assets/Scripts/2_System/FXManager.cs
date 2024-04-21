using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace DynamicGames.System
{
    public enum FXType
    {
        Bomb,
        SmallExplosion,
        ItemHit,
        ShadowMissile,
        ShadowBomb,
        Empty,
        BulletB1,
        BulletB2,
        BulletC1,
        BulletC2,
        BulletD1,
        BulletD2,
        BulletE1,
        BulletE2,
        Shield,
        ShieldPop,
        Blackhole,
        DeadExplosion,
        Spin,
        TierUp,
        TierDown,
        RocketHit
    }
    
    /// <summary>
    /// Manages the creation and destruction of particle effects (FX).
    /// </summary>
    public class FXManager : MonoBehaviour
    {
        [SerializeField] public List<FXData> FXDatas;
        private readonly List<ObjectPool<FXController>> FX_Pools = new();
     
        public static FXManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            foreach (var fxdata in FXDatas)
            {
                var defaultCapacity = 3;
                var maxCapacity = 3;
                var new_pool = new ObjectPool<FXController>(() =>
                    {
                        var fx = Instantiate(fxdata.prefab);
                        fx.transform.SetParent(gameObject.transform);
                        return fx;
                    }, fx => { fx.gameObject.SetActive(true); }, fx => { fx.gameObject.SetActive(false); },
                    fx => { Destroy(fx.gameObject); }, true, defaultCapacity, maxCapacity);

                FX_Pools.Add(new_pool);
            }
        }

        public GameObject CreateFX(FXType fXType, Transform target)
        {
            var pool = GetObjectPoolByFxType(fXType);

            if (pool == null)
            {
                Debug.Log("FX data not found : " + fXType);
                return null;
            }

            var fx = pool.Get();
            fx.InitAndPlayFX(target.transform.position, fXType);

            return fx.gameObject;
        }

        public GameObject CreateFX(FXType fXType, Vector3 target)
        {
            var pool = GetObjectPoolByFxType(fXType);

            if (pool == null)
            {
                Debug.Log("FX data not found : " + fXType);
                return null;
            }

            var fx = pool.Get();
            fx.InitAndPlayFX(target, fXType);

            return fx.gameObject;
        }

        public void KillFX(FXController fxController)
        {
            GetObjectPoolByFxType(fxController.GetFXType()).Release(fxController);
        }

        private ObjectPool<FXController> GetObjectPoolByFxType(FXType type)
        {
            var idx = -1;
            for (var i = 0; i < FXDatas.Count; i++)
                if (FXDatas[i].type == type)
                {
                    idx = i;
                    break;
                }

            if (idx != -1) return FX_Pools[idx];
            return null;
        }
    }
    
    [Serializable]
    public class FXData
    {
        public FXType type;
        public FXController prefab;
    }
}