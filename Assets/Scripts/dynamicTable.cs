using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.IO;


public class dynamicTable : MonoBehaviour
{
    public GameObject RowPrefab;
    public GameObject BigRowPrefab;
    public GameObject TextPrefab;
    public GameObject BoldTextPrefab;
    public string JsonFileName = "JsonChallenge.json";

    public static dynamicTable instance;
    //private var loadedData;
    private bool isReady = false;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //DontDestroyOnLoad(gameObject);

        LoadTextFile(JsonFileName);

    }

    void OnGUI()
    {
        if (GUILayout.Button("Reload File",GUILayout.Width(120), GUILayout.Height(80) )) {
            Debug.Log("Reload Button Pressed");
            clearDynamicTable();
            LoadTextFile(JsonFileName);
        }
    }

    public void LoadTextFile(string FileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath + "/", FileName);
        Debug.Log("File Path: " + filePath);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath); //unused alternative: TextAsset loadedData = Resources.Load<TextAsset>(filePath);  then string loadedData.text;

            if (!string.IsNullOrEmpty(dataAsJson))
            {

                //added "results : " for parsing with SimpleJSON plugin
                var loadedData = JSON.Parse(" \"results\" : " + dataAsJson);
                Debug.Log("Data Parsed");

                int count = loadedData.Count;
                if (count != 3)
                {
                    Debug.Log("Unexpected Json Format, does not follow the challenge rules");
                }

                string title = loadedData["Title"];
                int colCount = loadedData["ColumnHeaders"].Count;
                string columnHeaders = loadedData["ColumnHeaders"].ToString();
                int rowCount = loadedData["Data"].Count;
                string rows = loadedData["Data"].ToString();

                Debug.Log("Title: " + title);
                Debug.Log("N° of columns: " + colCount);
                Debug.Log("Columns: " + columnHeaders);
                Debug.Log("N° of rows: " + rowCount);
                
            
                //text stored in arrays
                string[] titleArray = { title };
                string[] colNamesArray = new string[colCount];
                string[] rowArray = new string[colCount];

                //Instantiate data in table dynamically.
                fillDynamicTable(titleArray, 1, true);           

                for (int i = 0; i < colCount; i++)
                {
                    colNamesArray[i] = loadedData["ColumnHeaders"][i]; 
                }
                fillDynamicTable(colNamesArray, colCount, true);

                //data rows
                for (int j = 0; j < rowCount; j++)
                {
                    Debug.Log(loadedData["Data"][j]);
                    for (int i = 0; i < colCount; i++)
                    {
                        string aux = colNamesArray[i];
                        rowArray[i] = loadedData["Data"][j][aux];                     
                    }
                    fillDynamicTable(rowArray, colCount, false);
                }
                
            }
            else
            { Debug.LogError("Data is empty, check JSON File and its name in the editor");
            }     
        }
        else
        {   Debug.LogError("Cannot find file named " + FileName);
        }

        isReady = true;
    }


    public bool GetIsReady()
    {
        return isReady;
    }

    void clearDynamicTable()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    void fillDynamicTable(string[] textIn, int colCount, bool isBold)
    {
        GameObject actualRow;
        GameObject textOut;
        //Instantiate row
        if (!isBold){
            actualRow = Instantiate(RowPrefab, gameObject.transform);
        }
        else{
            actualRow = Instantiate(BigRowPrefab, gameObject.transform);
        }

        //Instantiate text inside row layout
        for (int i = 0; i < colCount; i++)
        {
            if (!isBold)
            {
                textOut = Instantiate(TextPrefab, actualRow.transform);
            }
            else
            {
                textOut = Instantiate(BoldTextPrefab, actualRow.transform);
            }

            textOut.GetComponent<Text>().text = textIn[i];            
            
        }
        return;
    }

    







}
