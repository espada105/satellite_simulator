using UnityEngine;

public class EarthManager : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public Color axisColor = Color.blue; // 자전축 시각화 색상
    public float axisLength = 1000f; // 자전축 길이

    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 23.5f);
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        
        // 자전축을 레이로 표시
        DrawRotationAxis();
    }

    void DrawRotationAxis()
    {
        // 지구의 중심에서 위쪽(+Y) 방향으로 자전축 표시
        Vector3 axisStart = transform.position;
        Vector3 axisEnd = axisStart + transform.up * axisLength;
        Vector3 axisEndNegative = axisStart - transform.up * axisLength;

        // 자전축 양방향을 시각화
        Debug.DrawRay(axisStart, axisEnd - axisStart, axisColor);
        Debug.DrawRay(axisStart, axisEndNegative - axisStart, axisColor);
    }
}
