using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startGameBtn;
    public Button gamePlayIntroBtn;
    public Button exitGamebtn;
    public SceneTransition transitionPrefab; // 在 Inspector 中绑定预制体
    public GamePlayIntroPanel gamePlayIntroPanel;
    void Start()
    {

        // 为开始游戏按钮添加点击事件
        startGameBtn.onClick.AddListener(StartGame);
        gamePlayIntroBtn.onClick.AddListener(ShowGamePlayIntro);
        // 为退出游戏按钮添加点击事件
        exitGamebtn.onClick.AddListener(ExitGame);
    }

    // 开始游戏的方法
    void StartGame()
    {
        Debug.Log("游戏开始！");
        // 实例化过渡预制体（作为 GUI Canvas 的子节点）
        SceneTransition transition = Instantiate(transitionPrefab, GameObject.Find("GUI").transform);// 指定父节点为已有 Canvas);
        transition.FadeToScene("BackgroundIntroduction"); // 触发过渡动画
        //SceneManager.LoadScene("BackgroundIntroduction");
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

    void ShowGamePlayIntro()
    {
        gamePlayIntroPanel.ShowPanel();
    }
}    