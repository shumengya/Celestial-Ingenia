using UnityEngine;

public class EnemySpawnerPoint : MonoBehaviour
{
    // ����Ԥ����
    public GameObject enemyPrefab;
    // ���ɼ��ʱ�䣨�룩
    public float spawnInterval = 1f;
    public Transform EnemysParent;
    // �ϴ����ɵ�ʱ��
    private float lastSpawnTime;

    private void Start()
    {
        // ��ʼ���ϴ�����ʱ��
        lastSpawnTime = Time.time;
    }

    private void Update()
    {
        // ����Ƿ�ﵽ���ɼ��ʱ��
        if (Time.time - lastSpawnTime >= spawnInterval)
        {
            // �������ɵ��˵ķ���
            SpawnEnemy();
            // �����ϴ�����ʱ��
            lastSpawnTime = Time.time;
        }
    }

    private void SpawnEnemy()
    {
        // �ڵ�ǰ��Ϸ���󣨵������ɵ㣩��λ�ú���ת��ʵ��������Ԥ����
        Instantiate(enemyPrefab, transform.position, transform.rotation,EnemysParent);
    }
}