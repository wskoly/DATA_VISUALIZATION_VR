using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ChartAndGraph;
using Newtonsoft.Json;

public class PieValuegen : MonoBehaviour
{
    public ChartDynamicMaterial[] mat;
    private string URL = "http://127.0.0.1:8000/get_pie_plots/";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("GetData");
        PieChart pieChart = GetComponent<PieChart>();
        if (pieChart != null)
        {
            pieChart.DataSource.Clear();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetData(){
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{this.URL}{EntryScript.columns[EntryScript.selectedColumnIndex]}"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    string response = webRequest.downloadHandler.text;
                    // Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                    Dictionary<string, double> data = JsonConvert.DeserializeObject<Dictionary<string, double>>(response);
                    SetData(data);
                        
                    break;
            }
        }
    }

    void SetData(Dictionary<string, double> data){
        PieChart pieChart = GetComponent<PieChart>();
        if (pieChart != null)
        {
            var matIndex = 0;
            foreach(var item in data)
                        {
                            pieChart.DataSource.AddCategory(item.Key, mat[matIndex%mat.Length],1,1,0);
                            pieChart.DataSource.SetValue(item.Key, item.Value);
                            matIndex++;
                        }
            PieAnimation pieAnimation = GetComponent<PieAnimation>();
            pieAnimation.Animate();
        }
    }
}
