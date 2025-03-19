using UnityEngine;
using UnityEngine.UI;

public class Enemy_Num_Text : MonoBehaviour
{
    public string enemysParentName = "Enemys"; // Enemys 对象的名称
    public Text enemyNumText; // 用于显示敌人数量的 Text 组件

    void Start()
    {
        // 确保 enemyNumText 已正确赋值
        if (enemyNumText == null)
        {
            enemyNumText = GetComponent<Text>();
        }
    }

    void Update()
    {
        // 查找 Enemys 对象
        GameObject enemysParent = GameObject.Find(enemysParentName);
        if (enemysParent != null)
        {
            // 获取 Enemys 对象下的子对象数量
            int enemyCount = enemysParent.transform.childCount;
            // 更新文本显示
            enemyNumText.text = "敌人数量: " + enemyCount.ToString();
        }
        else
        {
            // 如果找不到 Enemys 对象，显示错误信息
            enemyNumText.text = "未找到 Enemys 对象";
        }
    }
}