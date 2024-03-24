using System.Collections.Generic;
using UnityEngine;

public class BuildBlockHolderList : MonoBehaviour
{
    [SerializeField] private List<GameObject> buildBlocks;
    
    public IReadOnlyList<GameObject> GetBuildBlocks()
    {
        return buildBlocks.AsReadOnly();
    }

    public void AddBuildBlock(GameObject block)
    {
        if (block != null)
        {
            buildBlocks.Add(block);
        }
    }
    public void RemoveBuildBlock(GameObject block)
    {
        if (block != null)
        {
            buildBlocks.Remove(block);
        }
    }
}