using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // 摄像机移动的速度
    public float moveSpeed = 5f;

    // 相机缩放的速度
    public float zoomSpeed = 5f;

    // 相机最小和最大的缩放范围（对于透视相机是视野，对于正交相机是正交大小）
    public float minZoom = 10f;
    public float maxZoom = 60f;

    private Camera mainCamera;

    void Start()
    {
        // 获取主相机组件
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        // 获取水平和垂直输入
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 根据输入计算移动方向
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f);

        // 根据移动方向和速度更新摄像机的位置
        transform.Translate(movement * moveSpeed * Time.deltaTime);

        // 获取鼠标滚轮的输入
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f)
        {
            if (mainCamera.orthographic)
            {
                // 正交相机的缩放处理
                mainCamera.orthographicSize -= scrollInput * zoomSpeed;
                // 限制正交大小在最小和最大缩放范围之间
                mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
            }
            else
            {
                // 透视相机的缩放处理
                mainCamera.fieldOfView -= scrollInput * zoomSpeed;
                // 限制视野在最小和最大缩放范围之间
                mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minZoom, maxZoom);
            }
        }
    }
}