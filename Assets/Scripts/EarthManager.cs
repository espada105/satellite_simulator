using UnityEngine;

public class EarthManager : MonoBehaviour
{
    public float rotationSpeed = 5f;
    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 23.5f);
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
