using UnityEngine;
using System.Collections;

public class SatelliteLocation : MonoBehaviour
{
    public Transform earthTransform;        // 지구 오브젝트 Transform
    public Transform jejuReferencePoint;    // 제주도 위치 오브젝트 Transform
    public Vector2 jejuLatitudeLongitude = new Vector2(33.4996f, 126.5312f); // 제주도 위도 경도
    public Camera cameraToUse;              // 특정 카메라를 지정
    public Color rayColor = Color.red;      // 레이 시각화 색상
    public Color hitPointColor = Color.green; // 충돌 지점 색상
    public float hitPointSize = 0.5f;       // 충돌 지점 Gizmo 크기

    private Vector3 lastHitPoint;           // 마지막으로 충돌한 지점
    private bool hitDetected = false;       // 충돌 여부

    private UIManager ui;
    private GMSManager gms;

    private void Start()
    {
        ui = FindObjectOfType<UIManager>();
        gms = FindObjectOfType<GMSManager>();
        StartCoroutine(UpdateCoordinateRoutine());
    }

    private IEnumerator UpdateCoordinateRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        
        while (true)
        {
            if (hitDetected)
            {
                float earthRadius = Vector3.Distance(earthTransform.position, jejuReferencePoint.position);
                float surfaceDist = CalculateSurfaceDistance(jejuReferencePoint.position, lastHitPoint, earthRadius);
                Vector2 predictCoord = PredictLatLong(jejuLatitudeLongitude.x, jejuLatitudeLongitude.y, surfaceDist);
                ui.UpdateCoordinate(predictCoord);
                gms.latitude = predictCoord.x;
                gms.longitude = predictCoord.y;
            }
            yield return wait;
        }
    }

    void Update()
    {
        // 카메라의 forward 방향에서 레이 캐스트 발사
        RaycastHit hit;
        Ray ray = new Ray(cameraToUse.transform.position, cameraToUse.transform.forward); // 카메라의 forward 방향으로 레이 발사

        if (Physics.Raycast(ray, out hit))
        {
            // 레이가 지구의 표면에 충돌한 지점
            lastHitPoint = hit.point;
            hitDetected = true;

            // 충돌 지점 색상 시각화
            Debug.DrawLine(ray.origin, hit.point, rayColor);
            Debug.DrawLine(jejuReferencePoint.position, hit.point, Color.blue);
            // 구 표면상에서의 거리 계산
            float earthRadius = Vector3.Distance(earthTransform.position, jejuReferencePoint.position);
            float surfaceDist = CalculateSurfaceDistance(jejuReferencePoint.position, hit.point, earthRadius);
        
            Vector2 predictCoord =  PredictLatLong(jejuLatitudeLongitude.x, jejuLatitudeLongitude.y, surfaceDist);
        }
        else
        {
            hitDetected = false;
        }
    }

    // 구 표면상의 거리를 계산하는 함수
    private float CalculateSurfaceDistance(Vector3 b, Vector3 c, float radius)
    {
        // 두 점의 위치 벡터를 정규화 (반지름이 1인 단위 벡터로 변환)
        Vector3 normalizedB = b.normalized;
        Vector3 normalizedC = c.normalized;

        // 두 벡터의 내적을 통해 cos(theta) 계산
        float cosTheta = Vector3.Dot(normalizedB, normalizedC);

        // cos(theta) 값을 -1과 1 사이로 제한 (부동소수점 오차 방지)
        cosTheta = Mathf.Clamp(cosTheta, -1f, 1f);

        // 중심각 theta 계산 (라디안 단위)
        float theta = Mathf.Acos(cosTheta);

        // 구 표면상의 거리 계산 (d = r * theta)
        float surfaceDistance = radius * theta;

        return surfaceDistance;
    }

    private Vector2 PredictLatLong(float latB, float lonB, float d)
    {
        // 거리를 중심각으로 변환 (라디안 단위)
        float earthRadius = Vector3.Distance(earthTransform.position, jejuReferencePoint.position);
        float theta = d / earthRadius;

        // 방위각 계산
        float azimuth = CalculateAzimuth(jejuReferencePoint.position, lastHitPoint);

        // 점 B의 위도와 경도를 라디안으로 변환
        float latBRad = latB * Mathf.Deg2Rad;
        float lonBRad = lonB * Mathf.Deg2Rad;
        float azimuthRad = azimuth * Mathf.Deg2Rad;

        // 점 C의 위도 계산
        float latCRad = Mathf.Asin(Mathf.Sin(latBRad) * Mathf.Cos(theta) +
                           Mathf.Cos(latBRad) * Mathf.Sin(theta) * Mathf.Cos(azimuthRad));

        // 점 C의 경도 계산
        float lonCRad = lonBRad + Mathf.Atan2(Mathf.Sin(azimuthRad) * Mathf.Sin(theta) * Mathf.Cos(latBRad),
                                              Mathf.Cos(theta) - Mathf.Sin(latBRad) * Mathf.Sin(latCRad));

        // 라디안을 도 단위로 변환
        float latC = latCRad * Mathf.Rad2Deg;
        float lonC = lonCRad * Mathf.Rad2Deg;
        lonC = NormalizeLongitude(lonC);

        return new Vector2(latC, lonC);
    }

    private float CalculateAzimuth(Vector3 pointB, Vector3 pointC)
    {
        // 지구 중심을 기준으로 한 상대 벡터
        Vector3 centerToB = (pointB - earthTransform.position).normalized;
        Vector3 centerToC = (pointC - earthTransform.position).normalized;

        // 북쪽 방향 벡터 (지구의 up 벡터)
        Vector3 north = earthTransform.up;

        // pointB에서의 접평면 상의 벡터들
        Vector3 tangentNorth = Vector3.ProjectOnPlane(north, centerToB).normalized;
        Vector3 toPointC = Vector3.ProjectOnPlane(centerToC - centerToB, centerToB).normalized;

        // 방위각 계산
        float angle = Vector3.SignedAngle(tangentNorth, toPointC, centerToB);
        
        // 각도를 0-360도 범위로 변환
        if (angle < 0)
            angle += 360f;

        return angle;
    }

    private float NormalizeLongitude(float longitude)
    {
        while (longitude > 180f)
        {
            longitude -= 360f;
        }
        while (longitude < -180f)
        {
            longitude += 360f;
        }
        return longitude;
    }

    // 충돌 지점에 Gizmo 표시
    void OnDrawGizmos()
    {
        if (hitDetected)
        {
            Gizmos.color = hitPointColor;
            Gizmos.DrawSphere(lastHitPoint, hitPointSize);
        }
    }
}
