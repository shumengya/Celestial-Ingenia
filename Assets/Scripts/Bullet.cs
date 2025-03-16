using UnityEngine;

public class Bullet: MonoBehaviour
{
    // �ӵ��ƶ��ٶ�
    public float speed = 10f;
    // �ӵ������˺�
    public int damage = 10;
    // �����ʱ��
    public float maxLifetime = 2f;
    // ����ƶ�����
    public float maxDistance = 20f;
    // �ӵ��������飬0 ��ʾ��ң�1 ��ʾ����
    public int team = 0;
    // �Ƿ����������ʱ������
    public bool useLifetimeLimit = true;
    // �Ƿ���������ƶ���������
    public bool useDistanceLimit = true;
    // �ӵ���ײ���Ч��Ԥ����
    public GameObject impactEffect;

    private Vector2 startPosition;
    private float elapsedTime = 0f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // �ӵ��ƶ�
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        if (useLifetimeLimit)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= maxLifetime)
            {
                DestroyBullet();
            }
        }

        if (useDistanceLimit)
        {
            float distanceTraveled = Vector2.Distance(startPosition, transform.position);
            if (distanceTraveled >= maxDistance)
            {
                DestroyBullet();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // �����ײ����Ķ����ǩ
        int otherTeam = 0;


        if (other.CompareTag("Enemy"))
        {
            otherTeam = 1;
        }
        else if (other.CompareTag("Player"))
        {
            otherTeam = 0;
        }

        // ����ͬ�����ӵ�����˺�
        if (team != otherTeam)
        {
            // ������ײ������һ�� Health �ű�
            HealthBar targetHealth = other.GetComponent<HealthBar>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }

            // ������ײЧ��
            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
            }

            // �����ӵ�
            DestroyBullet();
        }
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}