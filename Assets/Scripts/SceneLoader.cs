using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // 定义一个方法，用于加载指定名称的场景
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 示例：加载名为 "NextScene" 的场景
    public void LoadMainGame()
    {
        LoadScene("MainGame");
    }

    // 退出游戏的方法
    public void QuitGame()
    {
#if UNITY_EDITOR
        // 在 Unity 编辑器中，使用 UnityEditor.EditorApplication.isPlaying 来停止播放模式
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 在打包后的游戏中，使用 Application.Quit 方法退出游戏
        Application.Quit();
#endif
    }
}