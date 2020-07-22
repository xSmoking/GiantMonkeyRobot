using LitJson;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GridBuilder : MonoBehaviour
{
    [SerializeField] private string filename;

    [Header("GUI References")]
    [SerializeField] private Text gridTitle;
    [SerializeField] private GameObject grid;

    [Header("Prefabs")]
    [SerializeField] private GameObject gridRowPrefab;
    [SerializeField] private GameObject textPrefab;

    void Start()
    {
        LoadGrid();
    }

    public void LoadGrid()
    {
        ClearGrid();

        // Create file path
        string path = Application.streamingAssetsPath + "/" + filename;

        // If input file exists
        if (File.Exists(path))
        {
            // Read text and remove all spaces and line breaks
            string jsonString = File.ReadAllText(path);
            jsonString = jsonString.Trim().Replace(" ", "").Replace("\r", "").Replace("\n", "");
            jsonString = RemoveTrailingComma(jsonString);

            // Deserialize Json
            var deserializedObject = JsonMapper.ToObject(jsonString);

            #region Set Grid Title
            gridTitle.text = deserializedObject["Title"].ToString();
            #endregion

            #region Build Grid Header
            GameObject headerGO = Instantiate(gridRowPrefab, grid.GetComponent<RectTransform>());
            // Foreach string in array
            foreach(var columnHeader in deserializedObject["ColumnHeaders"])
            {
                // Instantiate header td and set its text
                GameObject header = Instantiate(textPrefab, headerGO.GetComponent<RectTransform>());
                header.GetComponent<Text>().text = columnHeader.ToString();
                header.GetComponent<Text>().fontStyle = FontStyle.Bold;
            }
            #endregion

            #region Build Grid Body
            // Foreach Data
            for (int i = 0; i < deserializedObject["Data"].Count; i++)
            {
                // Instantiate row
                GameObject rowGo = Instantiate(gridRowPrefab, grid.GetComponent<RectTransform>());

                // For each information (accordingly to header)
                foreach (var columnHeader in deserializedObject["ColumnHeaders"])
                {
                    // instantiate in row and set its text
                    GameObject text = Instantiate(textPrefab, rowGo.GetComponent<RectTransform>());
                    if(deserializedObject["Data"][i].ContainsKey(columnHeader.ToString()))
                        text.GetComponent<Text>().text = deserializedObject["Data"][i][columnHeader.ToString()].ToString();
                }
            }
            #endregion
        }
        // If input file not exists
        else
        {
            // Print file not found
            GameObject.Find("Title").GetComponent<Text>().text = "File not found";
        }
    }

    private void ClearGrid()
    {
        // Foreach rect transform in grid
        foreach (RectTransform child in grid.GetComponent<RectTransform>())
        {
            Destroy(child.gameObject);
        }
    }

    private string RemoveTrailingComma(string jsonString)
    {
        string tmp = "";

        // For each char in string
        for(int i = 0; i < jsonString.Length; i++)
        {
            // If char is a reagular character
            // or is a comma, but next char isn't } or ]
            // copy char
            if(jsonString[i] != ',' || (jsonString[i] == ',' && jsonString[i + 1] != '}' && jsonString[i + 1] != ']'))
            {
                tmp += jsonString[i];
            }
        }

        // Return new string
        return tmp;
    }
}