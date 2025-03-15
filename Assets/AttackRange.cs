using UnityEngine;

public class AttackRange : MonoBehaviour
{
    public float attackRange = 5f;
    public Color rangeColor = new Color(1f, 0f, 0f, 0.2f);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = rangeColor;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}