using UnityEngine;

public class SatelliteLocation : MonoBehaviour
{
    public Transform EarthManager; // 지구 중심 Transform
    public Vector3 NorthPole; // 북극 좌표
    public float orbitSpeed = 5f; // 공전 속도
    public float orbitRadius = 500f; // 궤도 반경
    public float orbitTilt = 23.5f; // 궤도의 경사각

    private float angle = 0f;
    private Vector3 poleAxis;
    private Vector3 normalizedPoleAxis;

    void Start()
    {
        // 북극-지구 중심 축 계산
        poleAxis = NorthPole - EarthManager.position;
        normalizedPoleAxis = poleAxis.normalized;

        // 초기 각도 설정
        angle = Random.Range(0f, 360f);
    }

    void Update()
    {
        // 공전 각도 업데이트
        angle += orbitSpeed * Time.deltaTime;

        // 궤도 위치 계산
        Quaternion tiltRotation = Quaternion.Euler(orbitTilt, 0f, 0f);
        Vector3 orbitPosition = tiltRotation * new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad)) * orbitRadius;

        // 위성의 위치 갱신
        transform.position = EarthManager.position + orbitPosition;

        // 위성을 지구 중심으로 회전
        Vector3 directionToEarth = (EarthManager.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToEarth, Vector3.up);
        transform.rotation = targetRotation * Quaternion.Euler(90f, 0f, 0f);

        // 위도와 경도 계산
        Vector2 latLong = CalculateLatitudeLongitude();

        // 위도와 경도를 콘솔에 출력
        Debug.Log($"Latitude: {latLong.x:F6}, Longitude: {latLong.y:F6}");
    }

    private Vector2 CalculateLatitudeLongitude()
    {
        // 위성과 지구 중심 간의 상대 위치
        Vector3 relativePosition = transform.position - EarthManager.position;

        // 위도 계산 (북극 축 기준)
        float dot = Vector3.Dot(relativePosition.normalized, normalizedPoleAxis);
        float latitude = Mathf.Asin(dot) * Mathf.Rad2Deg;

        // 경도 계산 (북극 축과 수직인 평면 기준)
        Vector3 relativeXZ = relativePosition - Vector3.Project(relativePosition, normalizedPoleAxis); // X-Z 평면 투영
        float longitude = Mathf.Atan2(relativeXZ.z, relativeXZ.x) * Mathf.Rad2Deg;

        // 경도를 -180° ~ 180° 범위로 변환
        if (longitude > 180f)
            longitude -= 360f;
        else if (longitude < -180f)
            longitude += 360f;

        return new Vector2(latitude, longitude);
    }
}
