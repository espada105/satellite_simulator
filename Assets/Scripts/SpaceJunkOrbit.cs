using UnityEngine;

public class SpaceJunkOrbit : MonoBehaviour
{
    public Transform earth;
    public float orbitSpeed = 5f; // 공전 속도
    public float orbitTilt = 23.5f; // 궤도의 경사각

    private float orbitRadius; // 궤도 반경 (초기화 시 현재 거리로 고정)
    private float angle = 0f; // 현재 공전 각도

    void Start()
    {
        earth = FindObjectOfType<EarthManager>().GetComponent<Transform>();
        
        orbitRadius = Vector3.Distance(transform.position, earth.position);

        angle = Random.Range(0f, 360f);
    }

    void Update()
    {
        angle += orbitSpeed * Time.deltaTime;

        Quaternion tiltRotation = Quaternion.Euler(orbitTilt, 0f, 0f);
        Vector3 orbitPosition = tiltRotation * new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad)) * orbitRadius;

        transform.position = earth.position + orbitPosition;

        Vector3 directionToEarth = (earth.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToEarth, Vector3.up);

        transform.rotation = targetRotation * Quaternion.Euler(90f, 0f, 0f);
    }
}