using UnityEngine;
using UnityEngine.EventSystems;

public class StoneQuary : MonoBehaviour, IPointerClickHandler
{
    private HealthBar buildingHealthBar;
    public float damageAmount = 1f;
    private float oneSecondTimer = 0f;
    private float twoSecondTimer = 0f;
    private float threeSecondTimer = 0f;
    public string smyName = "��ʯ��";
    public string smyType = "����";
    public string smyDescription = "������ʯͷ��";

    // �����Ƿ���Ա�����Ŀ���
    public bool canBeClicked = true;
    public bool isSelected = false;

    void Start()
    {
        buildingHealthBar = GetComponentInChildren<HealthBar>();
    }

    void Update()
    {
        // �ۼӼ�ʱ��
        oneSecondTimer += Time.deltaTime;
        twoSecondTimer += Time.deltaTime;
        threeSecondTimer += Time.deltaTime;

        // �� 1 ���ʱ���ﵽ 1 ��ʱ
        if (oneSecondTimer >= 1f)
        {
            if (buildingHealthBar != null)
            {
                buildingHealthBar.TakeDamage(damageAmount);
            }
            oneSecondTimer = 0f;
        }

        // �� 2 ���ʱ���ﵽ 2 ��ʱ
        if (twoSecondTimer >= 2f)
        {
            // ÿ 2 ������һ��ʯͷ
            PlayerConfig.Instance.stoneNum += 1;
            twoSecondTimer = 0f;
        }

        // �� 3 ���ʱ���ﵽ 3 ��ʱ
        if (threeSecondTimer >= 3f)
        {
            // ���������� 3 ���ʱҪִ�е��߼�
            // Debug.Log("3 ���ʱ����");
            threeSecondTimer = 0f;
        }

        if (buildingHealthBar.IsDead())
        {
            Destroy(gameObject);
        }
    }

    // ʵ�� IPointerClickHandler �ӿڵķ���
    public void OnPointerClick(PointerEventData eventData)
    {
        if (canBeClicked)
        {
            Debug.Log("�ý�����ѡ��:" + eventData);
            isSelected = true;
        }
    }
}