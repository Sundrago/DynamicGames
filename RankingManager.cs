using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RankingManager : MonoBehaviour
{
    [TableList]
    public List<RankSprite> rankSprites;
    [SerializeField, TableList]
    private List<IconBadge> iconBadges;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    private void AddObjs()
    {
        foreach (Ranks rank in Enum.GetValues(typeof(Ranks)))
        {
            RankSprite rankSprite = new RankSprite();
            rankSprite.rank = rank;
            rankSprites.Add(rankSprite);
        }

        foreach (GameType type in Enum.GetValues((typeof(GameType))))
        {
            IconBadge iconBadge = new IconBadge();
            iconBadge.gameType = type;
            iconBadges.Add(iconBadge);
        }
    }

    [Serializable]
    public class RankSprite
    {
        public Ranks rank;
        public Sprite Sprite;
    }
    
    [Serializable]
    private class IconBadge
    {
        public GameType gameType;
        public GameObject obj;
    }
}

public enum Ranks { bronze0, bronze1, bronze2, silver0, silver1, silver2, gold0, gold1, gold2, diamond0, diamond1, diamond2 }