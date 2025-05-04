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

   

    public void SpawnEnemy(int additionalEnemies = 0)
    {
        // 随机生成 2 - 4 个敌人
        int totalEnemies = Random.Range(0, 2) + additionalEnemies;
        if (totalEnemies >= 5)
        {
            totalEnemies = 5; // 限制最多生成 5 个敌人
        }

        for (int i = 0; i < totalEnemies; i++)
        {
            Instantiate(enemyPrefab, transform.position, transform.rotation, EnemysParent);
        }
    }
}