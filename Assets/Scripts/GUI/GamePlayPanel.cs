using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{
   public GameObject GamePlayPanel; 
    public Button BackToGamePlayPanelBtn;
    public Button BackToMainMenuBtn;
    void Start()
    {
          GamePlayPanel.SetActive(false);
          BackToGamePlayPanelBtn.onClick.AddListener(ShowPanel);
          BackToMainMenuBtn.onClick.AddListener(HidePanel);
    }

       void ShowPanel()
    {
        // 显示 Panel
        GamePlayPanel.SetActive(true);
    }
       void HidePanel()
       {
         GamePlayPanel.SetActive(false);
       }
}
