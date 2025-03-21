using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    public GameObject gameOverPanel; // 游戏结束弹窗Panel
    public Button BackToMainSceneBtn;
    public Button PlayAgainBtn; // 建议将变量名改为 QuitGameBtn 更直观

    void Start()
    {
        // 游戏开始时隐藏弹窗
        gameOverPanel.SetActive(false);

        // 绑定按钮点击事件
        if (BackToMainSceneBtn != null)
        {
            BackToMainSceneBtn.onClick.AddListener(LoadMainMenuScene);
        }
        else
        {
            Debug.LogError("BackToMainSceneBtn 未赋值！");
        }

        if (PlayAgainBtn != null)
        {
            // 修改按钮监听事件为退出游戏
            PlayAgainBtn.onClick.AddListener(QuitGame);
            // 建议修改按钮文字（如果需改UI文本，可在此添加）
            // PlayAgainBtn.GetComponentInChildren<Text>().text = "退出游戏";
        }
        else
        {
            Debug.LogError("PlayAgainBtn 未赋值！");
        }
    }

    public void ShowGameOverPopup()
    {
        // 显示游戏结束弹窗
        gameOverPanel.SetActive(true);
    }

    // 删除原来的 RestartGame 方法
    // private void RestartGame() { ... }

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
    }
}