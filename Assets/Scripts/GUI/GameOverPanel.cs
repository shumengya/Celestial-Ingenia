using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    public GameObject gameOverPanel; // ��Ϸ��������Panel
    public Button BackToMainSceneBtn;
    public Button PlayAgainBtn; // ���齫��������Ϊ QuitGameBtn ��ֱ��

    void Start()
    {
        // ��Ϸ��ʼʱ���ص���
        gameOverPanel.SetActive(false);

        // �󶨰�ť����¼�
        if (BackToMainSceneBtn != null)
        {
            BackToMainSceneBtn.onClick.AddListener(LoadMainMenuScene);
        }
        else
        {
            Debug.LogError("BackToMainSceneBtn δ��ֵ��");
        }

        if (PlayAgainBtn != null)
        {
            // �޸İ�ť�����¼�Ϊ�˳���Ϸ
            PlayAgainBtn.onClick.AddListener(QuitGame);
            // �����޸İ�ť���֣�������UI�ı������ڴ���ӣ�
            // PlayAgainBtn.GetComponentInChildren<Text>().text = "�˳���Ϸ";
        }
        else
        {
            Debug.LogError("PlayAgainBtn δ��ֵ��");
        }
    }

    public void ShowGameOverPopup()
    {
        // ��ʾ��Ϸ��������
        gameOverPanel.SetActive(true);
    }

    // ɾ��ԭ���� RestartGame ����
    // private void RestartGame() { ... }

    // �����˳���Ϸ����
    private void QuitGame()
    {
        Debug.Log("�˳���Ϸ");

        // �ڱ༭����ֹͣ���У������ڲ��ԣ�
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // �ڴ����İ汾���˳�
#endif
    }

    // �������˵�����
    private void LoadMainMenuScene()
    {
        Debug.Log("�������˵�");
        // ������Ϊ "MainMenu" �ĳ���
        SceneManager.LoadScene("MainMenu");
    }
}