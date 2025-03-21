using UnityEngine;
using UnityEngine.EventSystems;

public class IronFactory : MonoBehaviour, IPointerClickHandler
{
    private HealthBar IronFactorHealthBar;
    public float damageAmount = -1f;
    private float oneTimer = 0f;
    public string smyName = "����";
    public string smyType = "����";
    public string smyDescription = "�������Ŀ�";

    // �����Ƿ���Ա�����Ŀ���
    public bool canBeClicked = true;
    public bool isSelected = false;

    void Start()
    {
        IronFactorHealthBar = GetComponentInChildren<HealthBar>();
    }

    void Update()
    {
        // һ���ʱ��
        oneTimer += Time.deltaTime;
        // ����ʱ���ﵽ 1 ��ʱ
        if (oneTimer >= 1f)
        {
            if (IronFactorHealthBar != null)
            {
                IronFactorHealthBar.TakeDamage(damageAmount);
                PlayerConfig.Instance.ironNum++;
                
            }
            oneTimer = 0f;
        }


        if (IronFactorHealthBar.IsDead())
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
