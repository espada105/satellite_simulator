using UnityEngine;

public class SpaceJunk : MonoBehaviour
{
    public GameObject objectPrefab;
    public Transform center; // 중심점 (지구기준임/)
    public float minSpawnRadius; // 지구로부터 최소 생성 반경 
    public float maxSpawnRadius; // 지구로부터 최대 생성 반경
    public int objectCount; // 생성할 오브젝트 수
    public float minOrbitSpeed; // 최소 공전 속도
    public float maxOrbitSpeed; // 최대 공전 속도

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        for (int i = 0; i < objectCount; i++)
        {
            // 랜덤 방향과 반경을 사용하여 위치 계산
            Vector3 randomDirection = Random.onUnitSphere; // 단위 구에서 랜덤 방향
            Vector3 spawnPosition = center.position + randomDirection * Random.Range(minSpawnRadius, maxSpawnRadius);

            // 오브젝트 생성
            GameObject debris = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

            Satellite satellite = debris.GetComponent<Satellite>();
            if (satellite != null)
            {
                satellite.EarthManager = center; // 중심점 설정
                satellite.orbitSpeed = Random.Range(minOrbitSpeed, maxOrbitSpeed); // 랜덤 속도
                satellite.orbitTilt = Random.Range(15f, 30f); // 궤도 경사각
            }
        }
    }
}