using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // ������ƶ����ٶ�
    public float moveSpeed = 5f;

    // ������ŵ��ٶ�
    public float zoomSpeed = 5f;

    // �����С���������ŷ�Χ������͸���������Ұ���������������������С��
    public float minZoom = 10f;
    public float maxZoom = 60f;

    private Camera mainCamera;

    void Start()
    {
        // ��ȡ��������
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        // ��ȡˮƽ�ʹ�ֱ����
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // ������������ƶ�����
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f);

        // �����ƶ�������ٶȸ����������λ��
        transform.Translate(movement * moveSpeed * Time.deltaTime);

        // ��ȡ�����ֵ�����
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f)
        {
            if (mainCamera.orthographic)
            {
                // ������������Ŵ���
                mainCamera.orthographicSize -= scrollInput * zoomSpeed;
                // ����������С����С��������ŷ�Χ֮��
                mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
            }
            else
            {
                // ͸����������Ŵ���
                mainCamera.fieldOfView -= scrollInput * zoomSpeed;
                // ������Ұ����С��������ŷ�Χ֮��
                mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minZoom, maxZoom);
            }
        }
    }
}