using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBase : MonoBehaviour, IPointerClickHandler
{
    private HealthBar playerHealthBar;
    public float damageAmount = 1f;
    private float timer = 0f;

    // �����Ƿ���Ա�����Ŀ���
    public bool canBeClicked = true;
    public bool isSelected = false;

    void Start()
    {
        playerHealthBar = GetComponentInChildren<HealthBar>();
    }

    void Update()
    {
        // �ۼӼ�ʱ��
        timer += Time.deltaTime;

        // ����ʱ���ﵽ 1 ��ʱ
        if (timer >= 1f)
        {
            if (playerHealthBar != null)
            {
                playerHealthBar.TakeDamage(damageAmount);
            }
            timer = 0f;
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