using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class DebugUIPanel : MonoBehaviour
{
    public Text bulletNumText;
    public Text enemyNumText;
    public Text buildingNumText;
    public Text FPSText;

    public GameObject PlayerBullets;
    public GameObject EnemyBullets;
    public GameObject EnemyNum;
    public GameObject BuildingNum;

    public float updateTime = 0.5f;

    private int playerBulletNum = 0;
    private int enemyBulletNum = 0;
    private int enemyNum = 0;
    private int buildingNum = 0;

    void Start()
    {

    }

    void Update()
    {
        updateTime -= Time.deltaTime;
        if (updateTime <= 0)
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

            if (EnemyNum != null)
            {
                enemyNum = EnemyNum.transform.childCount;
            }

            if (BuildingNum != null)
            {
                buildingNum = BuildingNum.transform.childCount;
            }

            // 更新文本显示
            if (bulletNumText != null)
            {
                bulletNumText.text = "子弹数量:"+ (enemyBulletNum + playerBulletNum).ToString();
            }
            if (enemyNumText != null)
            {
                enemyNumText.text = "敌人数量:" + enemyNum.ToString();
            }
            if (buildingNumText != null)
            {
                buildingNumText.text = "建筑数量:" + buildingNum.ToString();
            }
            if (FPSText != null)
            {
                FPSText.text = "FPS:" + ((int)(1f / Time.smoothDeltaTime)).ToString();
            }

            updateTime = 0.5f;
        }

    }
}
