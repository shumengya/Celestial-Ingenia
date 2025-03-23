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
        // 确保 bulletNumText 已正确赋值
        if (bulletNumText == null)
        {
            bulletNumText = GetComponent<Text>();
        }
    }

    void Update()
    {
        // 统计玩家子弹数量
        if (PlayerBullets != null)
        {
            playerBulletNum = PlayerBullets.transform.childCount;
        }

        // 统计敌人子弹数量
        if (EnemyBullets != null)
        {
            enemyBulletNum = EnemyBullets.transform.childCount;
        }

        // 更新文本显示
        if (bulletNumText != null)
        {
            bulletNumText.text = "子弹数量:"+ (enemyBulletNum + playerBulletNum).ToString();
        }
    }
}