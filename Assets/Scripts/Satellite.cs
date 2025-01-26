using UnityEngine;
using System.Collections.Generic;

public class Satellite : MonoBehaviour
{
    public static Satellite instance;
    public Transform EarthManager;
    public float orbitSpeed = 5f;
    public float orbitRadius = 500f;
    public float orbitTilt = 23.5f;

    private float angle = 0f;

    public List<Transform> objectsInRadar;
    private SphereCollider collider;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        collider = GetComponent<SphereCollider>();
        objectsInRadar = new List<Transform>();
    }

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

    void OnTriggerEnter( Collider other )
    {
        objectsInRadar.Add( other.transform );
    }

    void OnTriggerExit( Collider other )
    {
        if(objectsInRadar.Contains( other.transform ))
            objectsInRadar.Remove( other.transform );
    }

    public Vector2[] GetDistances()
    {
        List<Vector2> validDistances = new List<Vector2>();
        
        foreach(Transform target in objectsInRadar)
        {
            Vector2? distance = CalculateDistance(target);
            if (distance.HasValue)
                validDistances.Add(distance.Value);
        }

        return validDistances.ToArray();
    }

    private Vector2? CalculateDistance(Transform target)
    {
        Vector3 relativePosition = target.position - transform.position;
        
        float normalizedX = Mathf.Clamp(relativePosition.x / collider.radius, -1f, 1f) * 125f;
        float normalizedZ = Mathf.Clamp(relativePosition.z / collider.radius, -1f, 1f) * 125f;
        
        Vector3 normalized = new Vector3(normalizedX, 0, normalizedZ);
        
        return new Vector2(normalizedX, normalizedZ);
    }
}