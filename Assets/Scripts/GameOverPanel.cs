using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public GameObject gameOverPanel; // 游戏结束弹窗Panel

    void Start()
    {
        // 游戏开始时隐藏弹窗
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOverPopup()
    {
        // 显示游戏结束弹窗
        gameOverPanel.SetActive(true);
    }
}