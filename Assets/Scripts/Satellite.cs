using UnityEngine;

public class Satellite : MonoBehaviour
{
    public Transform EarthManager;
    public float orbitSpeed = 5f;
    public float orbitRadius = 500f;
    public float orbitTilt = 23.5f;

    private float angle = 0f;

    void Update()
    {
        angle += orbitSpeed * Time.deltaTime;

        Quaternion tiltRotation = Quaternion.Euler(orbitTilt, 0f, 0f);
        Vector3 orbitPosition = tiltRotation * new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad)) * orbitRadius;

        transform.position = EarthManager.position + orbitPosition;

        Vector3 directionToEarth = (EarthManager.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToEarth, Vector3.up);

        transform.rotation = targetRotation * Quaternion.Euler(90f, 0f, 0f);
        
    }
}
