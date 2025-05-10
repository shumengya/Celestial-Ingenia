
using UnityEngine;
using UnityEngine.UI;

public class WaveCountdown : MonoBehaviour
{
    // 每波之间的倒计时时间（秒）
    public float countdownTime = 10f;
    // 显示倒计时的文本组件
    public Text countdownText;
    // 当前波数
    public int currentWave = 1;

    public EnemySpawnerPoint[] enemySpawnerPoints;

    private float currentTime;

    void Start()
    {
        currentTime = countdownTime;
        UpdateCountdownText();
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateCountdownText();
            if (currentTime <= 0)
            {
                // 倒计时结束，触发新一波
                TriggerNewWave();
            }
        }
    }

    void UpdateCountdownText()
    {
        if (countdownText != null)
        {
            countdownText.text = "下一波剩余时间: " + Mathf.CeilToInt(currentTime).ToString() + " 秒";
        }
    }

    void TriggerNewWave()
    {
        // 重置倒计时
        currentTime = countdownTime;
        UpdateCountdownText();

        // 增加波数
        currentWave++;
        countdownTime += 1f; 

        // 这里可以添加新一波敌人生成等逻辑
        Debug.Log("第 " + currentWave + " 波开始！");
        // 遍历所有敌人生成点，生成敌人
        foreach (var enemySpawnerPoint in enemySpawnerPoints)
        {
            enemySpawnerPoint.SpawnEnemy(currentWave - 1); // 传递当前波数作为额外敌人数量
        }
    }
}    