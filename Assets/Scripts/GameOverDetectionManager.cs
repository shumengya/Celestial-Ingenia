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
        // 遍历Bullets父对象下的所有子对象
        for (int i = 0; i < buildingParent.childCount; i++)
        {
            // 获取当前子对象
            Transform child = buildingParent.GetChild(i);
            // 检查子对象的名称是否与PlayerBase名称匹配
            if (child.name == playerBaseName || child.name == playerBaseName2)
            {
                // 如果匹配，说明PlayerBase对象存在，返回true
                return true;
            }
        }
        // 如果遍历完所有子对象都没有找到匹配的名称，说明PlayerBase对象不存在，返回false
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

            PlayerConfig.Instance.woodNum = 0;
            PlayerConfig.Instance.stoneNum = 0;
            PlayerConfig.Instance.ironNum = 0;
            PlayerConfig.Instance.copperNum = 0;
            PlayerConfig.Instance.playerName = "树萌芽";
        }

        // 暂停游戏
        Time.timeScale = 0f;
    }
}