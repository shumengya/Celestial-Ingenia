using UnityEngine;
using UnityEngine.UI;

public class Bullet_Num_Text : MonoBehaviour
{
    public GameObject PlayerBullets;
    public GameObject EnemyBullets;
    public Text bulletNumText;

    private int playerBulletNum = 0;
    private int enemyBulletNum = 0;

    void Start()
    {
        // ȷ�� bulletNumText ����ȷ��ֵ
        if (bulletNumText == null)
        {
            bulletNumText = GetComponent<Text>();
        }
    }

    void Update()
    {
        // ͳ������ӵ�����
        if (PlayerBullets != null)
        {
            playerBulletNum = PlayerBullets.transform.childCount;
        }

        // ͳ�Ƶ����ӵ�����
        if (EnemyBullets != null)
        {
            enemyBulletNum = EnemyBullets.transform.childCount;
        }

        // �����ı���ʾ
        if (bulletNumText != null)
        {
            bulletNumText.text = "�ӵ�����:"+ (enemyBulletNum + playerBulletNum).ToString();
        }
    }
}