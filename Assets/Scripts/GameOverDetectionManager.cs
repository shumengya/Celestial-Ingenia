using UnityEngine;
using System.Collections;

public class GameOverDetectionManager : MonoBehaviour
{
    // ����һ��ʱ�������������ü���ʱ��������������Ϊ1��
    public float detectionInterval = 1f;
    public Transform buildingParent;
    // �洢PlayerBase���������
    public string playerBaseName = "PlayerBase";

    void Start()
    {
        // ����һ��Э�̣���ʼ���ж�ʱ���
        StartCoroutine(CheckForGameOver());
    }

    IEnumerator CheckForGameOver()
    {
        while (true)
        {
            // ����IsPlayerBaseExists���������PlayerBase�����Ƿ����
            if (!IsPlayerBaseExists())
            {
                // ���PlayerBase���󲻴��ڣ�����GameOver����������Ϸ�����߼�
                GameOver();
                // ����ѭ����ֹͣ���
                break;
            }
            // �ȴ�ָ����ʱ�������ٴν��м��
            yield return new WaitForSeconds(detectionInterval);
        }
    }

    bool IsPlayerBaseExists()
    {
        // ����Bullets�������µ������Ӷ���
        for (int i = 0; i < buildingParent.childCount; i++)
        {
            // ��ȡ��ǰ�Ӷ���
            Transform child = buildingParent.GetChild(i);
            // ����Ӷ���������Ƿ���PlayerBase����ƥ��
            if (child.name == playerBaseName)
            {
                // ���ƥ�䣬˵��PlayerBase������ڣ�����true
                return true;
            }
        }
        // ��������������Ӷ���û���ҵ�ƥ������ƣ�˵��PlayerBase���󲻴��ڣ�����false
        return false;
    }

    void GameOver()
    {
        // ������ʵ����Ϸ�������߼���������ʾ��Ϸ����������������Ϸ���ݵ�
        Debug.Log("Game Over!");
    }
}