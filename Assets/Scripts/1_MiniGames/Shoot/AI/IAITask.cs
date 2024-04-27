using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;

namespace DynamicGames.MiniGames.Shoot
{
    [Serializable]
    public enum AITaskType
    {
        Delay,
        SpawnItem,
        SetIslandAnimation,
        SetFaceAnimation,
        SpawnEnemiesAtRandomPos,
        SpawnEnemyInSpiral,
        SpawnEnemyOnIsland,
        CreateMeteor
    }

    public interface IAITask
    {
        public abstract AITaskType AITaskType { get; }
    }

    public class Delay : IAITask
    {
        public AITaskType AITaskType => AITaskType.Delay;
        [field: SerializeField, HorizontalGroup] public int Duration { get; private set; }
        [field: SerializeField, HorizontalGroup] public int Proper { get; private set; }

        public Delay()
        {
            Duration = 1000;
        }

        public Delay(int duration)
        {
            Duration = duration;
        }
    }

    public class SpawnItem : IAITask
    {
        public AITaskType AITaskType => AITaskType.SpawnItem;
        [field: SerializeField, HorizontalGroup] public int Amount { get; }
        [field: SerializeField, HorizontalGroup] public int Delay { get; }

        public SpawnItem()
        {
            Amount = 1;
            Delay = 0;
        }

        public SpawnItem(int amount, int delay)
        {
            Amount = amount;
            Delay = delay;
        }
    }

    [Serializable]
    public enum IslandState
    {
        Open,
        Close
    }

    public class SetIslandAnimation : IAITask
    {
        public AITaskType AITaskType => AITaskType.SetIslandAnimation;
        [field: SerializeField] public IslandState IslandState { get; private set; }

        public SetIslandAnimation()
        {
            IslandState = IslandState.Close;
        }

        public SetIslandAnimation(IslandState islandState)
        {
            IslandState = islandState;
        }
    }

    [Serializable]
    public enum FaceState
    {
        Idle,
        TurnRed,
        Angry01,
    }
    public class SetFaceAnimation : IAITask
    {
        public AITaskType AITaskType => AITaskType.SetFaceAnimation;
        [field: SerializeField] public FaceState FaceState { get; private set; }

        public SetFaceAnimation()
        {
            FaceState = FaceState.Idle;
        }

        public SetFaceAnimation(FaceState faceState)
        {
            FaceState = faceState;
        }
    }

    public class SpawnEnemiesAtRandomPos : IAITask
    {
        public AITaskType AITaskType => AITaskType.SpawnEnemiesAtRandomPos;

        [field: SerializeField, HorizontalGroup] public int Amount { get; private set; }
        [field: SerializeField, HorizontalGroup] public int Delay { get; private set; }

        public SpawnEnemiesAtRandomPos()
        {
            Amount = 3;
            Delay = 100;
        }

        public SpawnEnemiesAtRandomPos(int amount, int delay)
        {
            Amount = amount;
            Delay = delay;
        }
    }

    public class SpawnEnemyInSpiral : IAITask
    {
        public AITaskType AITaskType => AITaskType.SpawnEnemyInSpiral;

        [field: SerializeField, HorizontalGroup] public float RadiusMin { get; private set; }
        [field: SerializeField, HorizontalGroup] public float RadiusMax { get; private set; }
        [field: SerializeField, HorizontalGroup] public int Count { get; private set; }
        [field: SerializeField, HorizontalGroup] public float MaxAngle { get; private set; }
        [field: SerializeField, HorizontalGroup] public int Delay { get; private set; }
        [field: SerializeField, HorizontalGroup] public float PrewarmDuration { get; private set; }


        public SpawnEnemyInSpiral()
        {
            RadiusMin = 0.5f;
            RadiusMax = 1.6f;
            Count = 20;
            MaxAngle = 1.3f;
            Delay = 30;
            PrewarmDuration = 0.75f;
        }

        public SpawnEnemyInSpiral(float radiusMin, float radiusMax, int count, float maxAngle, int delay,
            float prewarmDuration)
        {
            RadiusMin = radiusMin;
            RadiusMax = radiusMax;
            Count = count;
            MaxAngle = maxAngle;
            Delay = delay;
            PrewarmDuration = prewarmDuration;
        }
    }

    public enum SpawnEnemyOnIslandDirection
    {
        Left,
        Right
    }
    public class SpawnEnemyOnIsland : IAITask
    {
        public AITaskType AITaskType => AITaskType.SpawnEnemyOnIsland;
        [field: SerializeField, HorizontalGroup] public SpawnEnemyOnIslandDirection Direction { get; private set; }
        [field: SerializeField, HorizontalGroup] public int Amount { get; private set; }

        public SpawnEnemyOnIsland()
        {
            Direction = SpawnEnemyOnIslandDirection.Left;
            Amount = 3;
        }
        public SpawnEnemyOnIsland(SpawnEnemyOnIslandDirection direction, int amount)
        {
            Direction = direction;
            Amount = amount;
        }
    }

    public class CreateMeteor : IAITask
    {
        public AITaskType AITaskType => AITaskType.CreateMeteor;

        [field: SerializeField, HorizontalGroup] public int Amount { get; private set; }
        [field: SerializeField, HorizontalGroup] public int Delay { get; private set; }

        CreateMeteor()
        {
            Amount = 3;
            Delay = 100;
        }
        public CreateMeteor(int amount, int delay)
        {
            Amount = amount;
            Delay = delay;
        }
    }
}