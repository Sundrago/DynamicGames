using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandPositionAdj : MonoBehaviour
{
    [SerializeField] GameObject[] Adjs;
    [SerializeField] GameObject guide;
    // Start is called before the first frame update
    void Start()
    {
        if(UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX)
        {
            foreach(GameObject adj in Adjs)
            {
                adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            }
        } else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXR)
        {
            foreach (GameObject adj in Adjs)
            {
                adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            }
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXS)
        {
            foreach (GameObject adj in Adjs)
            {
                adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            }
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXSMax)
        {
            foreach (GameObject adj in Adjs)
            {
                adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            }
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone11)
        {
            foreach (GameObject adj in Adjs)
            {
                adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            }
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone11Pro)
        {
            foreach (GameObject adj in Adjs)
            {
                adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            }
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone11ProMax)
        {
            // foreach (GameObject adj in Adjs)
            // {
            //     adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            // }
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone12)
        {
            foreach (GameObject adj in Adjs)
            {
                adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            }
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone12Mini)
        {
            foreach (GameObject adj in Adjs)
            {
                adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            }
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone12Pro)
        {
            foreach (GameObject adj in Adjs)
            {
                adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            }
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone12ProMax)
        {
            foreach (GameObject adj in Adjs)
            {
                adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            }
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone13)
        {
            foreach (GameObject adj in Adjs)
            {
                adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            }
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone13Mini)
        {
            foreach (GameObject adj in Adjs)
            {
                adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            }
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone13Pro)
        {
            foreach (GameObject adj in Adjs)
            {
                adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            }
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone13ProMax)
        {
            foreach (GameObject adj in Adjs)
            {
                adj.GetComponent<RectTransform>().localPosition = guide.GetComponent<RectTransform>().localPosition;
            }
        }
    }
}
