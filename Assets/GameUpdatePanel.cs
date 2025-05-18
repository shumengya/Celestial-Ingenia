using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class GameUpdatePanel : MonoBehaviour
{
    public Button quitButton;
    public Button refreshButton;
    public Button downloadButton;
    public Text updateText;
    public CanvasGroup canvasGroup;


    private string gameid = "tiangong";
    private string baseUrl = "http://192.168.1.112:5000/";



    void Start()
    {
        HidePanel();
        quitButton.onClick.AddListener(OnQuitButtonClick);
        refreshButton.onClick.AddListener(CheckForLatestUpdate);
        downloadButton.onClick.AddListener(DownloadUpdate);
        CheckForLatestUpdate();
        updateText.text = "正在检查更新...";
    }


    void OnQuitButtonClick()
    {
        HidePanel();
    }

    public void ShowPanel()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HidePanel()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    //http://192.168.1.112:5000/api/game/tiangong/latest
    //http://192.168.1.112:5000/api/game/tiangong/updates

    public void CheckForLatestUpdate()
    {
        StartCoroutine(GetLatestUpdate());
    }

    IEnumerator GetLatestUpdate()
    {
        string url = $"{baseUrl}api/game/{gameid}/updates";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("游戏最新更新内容：\n"+jsonResponse);
                
                UpdateResponse updateResponse = JsonUtility.FromJson<UpdateResponse>(jsonResponse);
                
                string formattedText = "";
                formattedText += $"<color=#666666>---------------------------------------------------------------------------------------------------------</color>\n";
                if (updateResponse != null && updateResponse.updates != null)
                {
                    foreach (UpdateInfo update in updateResponse.updates)
                    {

                        formattedText += $"<size=100><color=#FFCC00><b>{update.title}</b></color></size>\n";
                        formattedText += "<color=#AADDFF>内容: </color>"+$"<color=#FFFFFF>{update.content}</color>\n";
                        formattedText += $"<color=#777777><i>时间: {update.timestamp}</i></color>\n";
                        formattedText += $"<color=#00CC66><b>版本: {update.version}</b></color>\n";
                        formattedText += $"<color=#666666>---------------------------------------------------------------------------------------------------------</color>\n";
                    }
                }
                
                updateText.text = formattedText;
            }
            else
            {
                Debug.LogError($"错误: {webRequest.error}");
            }
        }
    }




    public void DownloadUpdate()
    {
    }
    

}

[Serializable]
public class UpdateResponse
{
    public List<UpdateInfo> updates;
}

[Serializable]
public class UpdateInfo
{
    public string content;
    public string game_id;
    public string game_name;
    public int id;
    public string timestamp;
    public string title;
    public string version;
}
