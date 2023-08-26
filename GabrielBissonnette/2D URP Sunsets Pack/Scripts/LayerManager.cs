using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    [Header("Layer Color")]
    [Tooltip("The color of the layer")]
    public Color32 layerColor;

    [Header("Objects")]
    [Tooltip("Objects that will be affected by the color change")]
    public SpriteRenderer[] subjectToColorChange;

    public void ChangeColor()
    {
        if(subjectToColorChange != null && subjectToColorChange.Length >= 1)
        {
            foreach(var sprite in subjectToColorChange)
            {
                if (sprite != null)
                    sprite.color = layerColor;
            }
        }
    }
}
