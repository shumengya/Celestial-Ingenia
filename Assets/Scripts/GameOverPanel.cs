using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public GameObject gameOverPanel; // ��Ϸ��������Panel

    void Start()
    {
        // ��Ϸ��ʼʱ���ص���
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOverPopup()
    {
        // ��ʾ��Ϸ��������
        gameOverPanel.SetActive(true);
    }
}