using UnityEngine;

public class HealthBarRotationLock : MonoBehaviour
{
    // ������ת�ķ�ʽ����ѡ���̶��ǶȻ������������
    public bool lockRotation = true;
    public bool faceCamera = false; // ��3D��Ϸ��Ҫ
    private Quaternion originalRotation;

    private void Start()
    {
        // ��¼��ʼ��ת
        originalRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (lockRotation)
        {
            // ǿ������Ѫ��������ת
            if (faceCamera)
            {
                // 3D��Ϸ����Ѫ��ʼ�����������
                transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            }
            else
            {
                // ���ֳ�ʼ��ת
                transform.rotation = originalRotation;
            }
        }
    }
}