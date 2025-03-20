using UnityEngine;
using UnityEngine.EventSystems;

public class TimberMill : MonoBehaviour, IPointerClickHandler
{
    private HealthBar buildingHealthBar;
    public float damageAmount = 1f;
    private float timer = 0f;
    public string smyName = "��ľ��";
    public string smyType = "����";
    public string smyDescription = "��������ľ��";

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
        timer += Time.deltaTime;

        // ����ʱ���ﵽ 1 ��ʱ
        if (timer >= 1f)
        {
            if (buildingHealthBar != null)
            {
                buildingHealthBar.TakeDamage(damageAmount);
            }
            //ÿ������һ��ľ��
            PlayerConfig.Instance.woodNum += 1;
            timer = 0f;
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