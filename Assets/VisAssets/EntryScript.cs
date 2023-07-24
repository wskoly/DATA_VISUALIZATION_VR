using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;

public enum ChartType{
    BarChart,
    PieChart
}
public class EntryScript : MonoBehaviour
{
    public GameObject menuObj;
    public GameObject backObj;
    public GameObject barChart;
    public GameObject pieChart;
    public TMP_Text columnTxt;
    public TMP_Text chartTxt;
    public static List<string> columns;
    GameObject chartObj;
    private string URL = "http://127.0.0.1:8000/get_columns";

    public static ChartType selectedChart = ChartType.BarChart;
    public static int selectedColumnIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("GetData");
        chartTxt.text = ChartType.BarChart.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChartPrevNextOnClick(){
        if(selectedChart == ChartType.BarChart){
            selectedChart = ChartType.PieChart;
        }else{
            selectedChart = ChartType.BarChart;
        }
        chartTxt.text = selectedChart.ToString();
    }

    public void ColumnPrevOnClick(){
        if(selectedColumnIndex == 0){
            selectedColumnIndex = columns.Count-1;
        }else{
            selectedColumnIndex--;
        }
        columnTxt.text =  columns[selectedColumnIndex];
    }

    public void ColumnNextOnClick(){
        selectedColumnIndex = (++selectedColumnIndex)%columns.Count;
        Debug.Log(selectedColumnIndex);
        columnTxt.text =  columns[selectedColumnIndex];
    }

    public void OnTheGo(){
        if(selectedChart == ChartType.BarChart){
            chartObj = Instantiate(barChart);
        }else{
            chartObj = Instantiate(pieChart);
        }
        menuObj.SetActive(false);
        backObj.SetActive(true);
    }

    public void OnBack(){
        menuObj.SetActive(true);
        backObj.SetActive(false);
        Destroy(chartObj);
    }

    IEnumerator GetData(){
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.URL))
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
                    columns = JsonConvert.DeserializeObject<List<string>>(response);
                    columnTxt.text =  columns[0];
                    break;
            }
        }
    }
}
