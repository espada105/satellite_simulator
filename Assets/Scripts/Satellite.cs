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
        Vector2[] result = new Vector2[objectsInRadar.Count];
        for(int i = 0; i < objectsInRadar.Count; i++)
            result[i] = CalculateDistance( objectsInRadar[i] );

        return result;
    }

    private Vector2 CalculateDistance(Transform target)
    {
        Vector3 relativePosition = target.position - transform.position;
        
        float maxDistance = collider.radius;
        float normalizedX = (relativePosition.x / maxDistance) * 125f;
        float normalizedZ = (relativePosition.z / maxDistance) * 125f;
        
        return new Vector2(normalizedX, normalizedZ);
    }
}