using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button startGameBtn;
    public Button exitGamebtn;

    void Start()
    {
        // 为开始游戏按钮添加点击事件
        startGameBtn.onClick.AddListener(StartGame);
        // 为退出游戏按钮添加点击事件
        exitGamebtn.onClick.AddListener(ExitGame);
    }

    void Update()
    {
        
    }

    // 开始游戏的方法
    void StartGame()
    {
        Debug.Log("游戏开始！");
        // 这里可以添加实际的游戏开始逻辑，例如加载游戏场景
        SceneManager.LoadScene("Background Introduction");
    }

    // 退出游戏的方法
    void ExitGame()
    {
        // 在编辑器中停止运行
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // 在发布版本中退出应用程序
        Application.Quit();
        #endif
    }
}    