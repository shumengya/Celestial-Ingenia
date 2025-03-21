using UnityEngine;
using System.Collections;

public class GameOverDetectionManager : MonoBehaviour
{
    // ����һ��ʱ�������������ü���ʱ��������������Ϊ1��
    public float detectionInterval = 1f;
    public Transform buildingParent;
    // �洢PlayerBase���������
    public string playerBaseName = "PlayerBase";
    public string playerBaseName2 = "PlayerBase(Clone)";
    public GameOverPanel gameOverPanelScript; // ���� GameOverPanel �ű�

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
            if (child.name == playerBaseName || child.name == playerBaseName2)
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
        Debug.Log("��Ϸ����!");
        // ���� GameOverPanel �ű��� ShowGameOverPopup ����
        if (gameOverPanelScript != null)
        {
            Debug.Log("���õ���");
            gameOverPanelScript.ShowGameOverPopup();

            PlayerConfig.Instance.woodNum = 0;
            PlayerConfig.Instance.stoneNum = 0;
            PlayerConfig.Instance.ironNum = 0;
            PlayerConfig.Instance.copperNum = 0;
            PlayerConfig.Instance.playerName = "����ѿ";
        }

        // ��ͣ��Ϸ
        Time.timeScale = 0f;
    }
}