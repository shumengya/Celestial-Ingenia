using UnityEngine;

public class EnemySpawnerPoint : MonoBehaviour
{
    // 敌人预制体
    public GameObject enemyPrefab;
    // 生成间隔时间（秒）
    public float spawnInterval = 1f;
    public Transform EnemysParent;
    // 上次生成的时间
    private float lastSpawnTime;

    private void Start()
    {
        // 初始化上次生成时间
        lastSpawnTime = Time.time;
    }

    private void Update()
    {
        // 检查是否达到生成间隔时间
        if (Time.time - lastSpawnTime >= spawnInterval)
        {
            // 调用生成敌人的方法
            SpawnEnemy();
            // 更新上次生成时间
            lastSpawnTime = Time.time;
        }
    }

    private void SpawnEnemy()
    {
        // 在当前游戏对象（敌人生成点）的位置和旋转下实例化敌人预制体
        Instantiate(enemyPrefab, transform.position, transform.rotation,EnemysParent);
    }
}