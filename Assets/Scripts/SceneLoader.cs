using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // ����һ�����������ڼ���ָ�����Ƶĳ���
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ʾ����������Ϊ "NextScene" �ĳ���
    public void LoadMainGame()
    {
        LoadScene("MainGame");
    }

    // �˳���Ϸ�ķ���
    public void QuitGame()
    {
#if UNITY_EDITOR
        // �� Unity �༭���У�ʹ�� UnityEditor.EditorApplication.isPlaying ��ֹͣ����ģʽ
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // �ڴ�������Ϸ�У�ʹ�� Application.Quit �����˳���Ϸ
        Application.Quit();
#endif
    }
}