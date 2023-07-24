using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ChartAndGraph;
using Newtonsoft.Json;

public class BarValuegen : MonoBehaviour
{
    public ChartDynamicMaterial[] mat;
    private string URL = "http://127.0.0.1:8000/get_bar_plots/";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("GetData");
        BarChart barChart = GetComponent<BarChart>();
        if (barChart != null)
        {
            barChart.DataSource.ClearCategories();
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
        BarChart barChart = GetComponent<BarChart>();
        if (barChart != null)
        {
            var matIndex = 0;
            foreach(var item in data)
                        {
                            barChart.DataSource.AddCategory(item.Key, mat[matIndex%mat.Length]);
                            barChart.DataSource.SetValue(item.Key, "All", item.Value);
                            matIndex++;
                        }
            barChart.DataSource.AutomaticMaxValue = true;
            BarAnimation barAnimation = GetComponent<BarAnimation>();
            barAnimation.Animate();
        }
    }
}
