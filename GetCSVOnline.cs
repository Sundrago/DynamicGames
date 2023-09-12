using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;
using UnityEngine.Networking;

//https://data.sundragon.net/dynamic_games_data.csv
public class GetCSVOnline : MonoBehaviour
{
    // private string filePath = "Assets/Resources/data.csv";
    private string dataURL = "https://data.sundragon.net/dynamic_games_data.csv";

    void Start()
    {
        // File.Create(filePath);
        StartCoroutine(DownLoadGet(dataURL));
    }
    
    IEnumerator DownLoadGet(string URL)
    {
        UnityWebRequest request = UnityWebRequest.Get(URL);

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            string data = request.downloadHandler.text;
            string[] rows = data.Split('\n');
            Dictionary<string, string> CSVData = new Dictionary<string, string>();
            
            foreach(string row in rows)
            {
                string[] cols = row.Split(',');
                if(cols[0] == "") continue;
                CSVData.Add(cols[0], cols[1]);
                print(cols[0] + " : " + cols[1]);
            }
        }
    }

    // class CSVData
    // {
    //     public string name;
    //     public string value;
    //
    //     CSVData(string _name, string _value)
    //     {
    //         name = _name;
    //         value = _value;
    //     }
    // }
}
