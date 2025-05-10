using UnityEngine;
using System.Collections;

public class GameOverDetectionManager : MonoBehaviour
{
    // 定义一个时间间隔，用于设置检测的时间间隔，这里设置为1秒
    public float detectionInterval = 1f;
    public Transform buildingParent;
    // 存储PlayerBase对象的名称
    public string playerBaseName = "PlayerBase";
    public string playerBaseName2 = "PlayerBase(Clone)";
    public GameOverPanel gameOverPanelScript; // 引用 GameOverPanel 脚本

    void Start()
    {
        // 启动一个协程，开始进行定时检测
        StartCoroutine(CheckForGameOver());
    }

    IEnumerator CheckForGameOver()
    {
        while (true)
        {
            // 调用IsPlayerBaseExists方法，检查PlayerBase对象是否存在
            if (!IsPlayerBaseExists())
            {
                // 如果PlayerBase对象不存在，调用GameOver方法触发游戏结束逻辑
                GameOver();
                // 跳出循环，停止检测
                break;
            }
            // 等待指定的时间间隔后再次进行检测
            yield return new WaitForSeconds(detectionInterval);
        }
    }

    bool IsPlayerBaseExists()
    {
        for (int i = 0; i < buildingParent.childCount; i++)
        {
            Transform child = buildingParent.GetChild(i);
            if (child.name == playerBaseName || child.name == playerBaseName2)
            {
                return true;
            }
        }
        return false;
    }

    void GameOver()
    {
        // 在这里实现游戏结束的逻辑，例如显示游戏结束弹窗、保存游戏数据等
        Debug.Log("游戏结束!");
        // 调用 GameOverPanel 脚本的 ShowGameOverPopup 函数
        if (gameOverPanelScript != null)
        {
            Debug.Log("调用弹窗");
            gameOverPanelScript.ShowGameOverPopup();
        }
    }
}