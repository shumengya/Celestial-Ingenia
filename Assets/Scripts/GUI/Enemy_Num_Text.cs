using UnityEngine;
using UnityEngine.UI;

public class Enemy_Num_Text : MonoBehaviour
{
    public string enemysParentName = "Enemys"; // Enemys ���������
    public Text enemyNumText; // ������ʾ���������� Text ���

    void Start()
    {
        // ȷ�� enemyNumText ����ȷ��ֵ
        if (enemyNumText == null)
        {
            enemyNumText = GetComponent<Text>();
        }
    }

    void Update()
    {
        // ���� Enemys ����
        GameObject enemysParent = GameObject.Find(enemysParentName);
        if (enemysParent != null)
        {
            // ��ȡ Enemys �����µ��Ӷ�������
            int enemyCount = enemysParent.transform.childCount;
            // �����ı���ʾ
            enemyNumText.text = "��������: " + enemyCount.ToString();
        }
        else
        {
            // ����Ҳ��� Enemys ������ʾ������Ϣ
            enemyNumText.text = "δ�ҵ� Enemys ����";
        }
    }
}