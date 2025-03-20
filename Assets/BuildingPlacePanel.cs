using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacePanel : MonoBehaviour
{
    public Image PlayerBase;
    public Image TimberMill;
    public Image StoneQuarry;

    public GameObject playerBasePrefab;
    public GameObject timberMillPrefab;
    public GameObject stoneQuarryPrefab;

    private GameObject currentBuildingToPlace;
    private bool isPlacingBuilding = false;

    void Start()
    {
        // Ϊÿ�� Image ��ӵ���¼�������
        PlayerBase.GetComponent<Button>().onClick.AddListener(() => StartPlacingBuilding(playerBasePrefab));
        TimberMill.GetComponent<Button>().onClick.AddListener(() => StartPlacingBuilding(timberMillPrefab));
        StoneQuarry.GetComponent<Button>().onClick.AddListener(() => StartPlacingBuilding(stoneQuarryPrefab));
    }

    void Update()
    {
        if (isPlacingBuilding)
        {
            // ��ȡ���λ�ò�ת��Ϊ��������
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            // �������������Ԥ���������߼������紴��һ����͸���Ľ���ģ�͸������

            if (Input.GetMouseButtonDown(0))
            {
                // ���������������ý���
                Instantiate(currentBuildingToPlace, mousePosition, Quaternion.identity);
                StopPlacingBuilding();
            }
        }
    }

    private void StartPlacingBuilding(GameObject buildingPrefab)
    {
        // ���ص�ǰ���
        gameObject.SetActive(false);

        // ��¼Ҫ���õĽ�������
        currentBuildingToPlace = buildingPrefab;
        isPlacingBuilding = true;
    }

    private void StopPlacingBuilding()
    {
        // ��ʾ���
        gameObject.SetActive(true);

        // ����״̬
        currentBuildingToPlace = null;
        isPlacingBuilding = false;
    }
}