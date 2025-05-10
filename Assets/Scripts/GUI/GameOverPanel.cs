using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    public GameObject gameOverPanel; // 游戏结束弹窗Panel
    public Button BackToMainSceneBtn;
    public Button QuitGameBtn; 
    public CanvasGroup canvasGroup;

    void Start()
    {
        // 游戏开始时隐藏弹窗
        HidePanel();

        // 绑定按钮点击事件
        if (BackToMainSceneBtn != null)
        {
            BackToMainSceneBtn.onClick.AddListener(LoadMainMenuScene);
        }

        if (QuitGameBtn != null)
        {
            // 修改按钮监听事件为退出游戏
            QuitGameBtn.onClick.AddListener(QuitGame);
            // 建议修改按钮文字（如果需改UI文本，可在此添加）
            // QuitGameBtn.GetComponentInChildren<Text>().text = "退出游戏";
        }
    }

    public void ShowGameOverPopup()
    {
        // 显示游戏结束弹窗
        ShowPanel();
    }


    // 新增退出游戏方法
    private void QuitGame()
    {
        Debug.Log("退出游戏");

        // 在编辑器中停止运行（仅用于测试）
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // 在打包后的版本中退出
#endif
    }

    // 返回主菜单场景
    private void LoadMainMenuScene()
    {
        Debug.Log("返回主菜单");
        // 加载名为 "MainMenu" 的场景
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f; // 确保时间缩放恢复正常
    }

    public void ShowPanel()
    {
        Time.timeScale = 0;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HidePanel()
    {
        Time.timeScale = 1;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}