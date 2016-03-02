using UnityEngine;
using System.Collections;

public class CloudSpawner : MonoBehaviour {

    public float minSpawnTime = 5f;
    public float maxSpawnTime = 10f;
    public float minCloudSpeed = 15;
    public float maxCloudSpeed = 25;
    public GameObject cloudPrefab;
    public int minCloudCount = 2;
    public int maxCloudCount = 5;

    // Use this for initialization
    void Start()
    {
        for(int i = 0; i < minCloudCount; i++)
        {
            SpawnCloud();
        }

        StartCoroutine(SpawnCloudTimer(Random.Range(minSpawnTime, maxSpawnTime)));
    }

    // Update is called once per frame
    void Update()
    {
        if(Object.FindObjectsOfType<Cloud>().Length < minCloudCount)
        {
            SpawnCloud();
        }
    }

    public IEnumerator SpawnCloudTimer(float time)
    {
        yield return new WaitForSeconds(time);
        if (Object.FindObjectsOfType<Cloud>().Length < maxCloudCount)
        {
            SpawnCloud();
        }
        StartCoroutine(SpawnCloudTimer(Random.Range(minSpawnTime, maxSpawnTime)));
    }

    void SpawnCloud()
    {
        // Choose a random location
        Vector3 spawnLocation = Random.onUnitSphere;
        Vector3 rightAngleVector = spawnLocation == Vector3.up ? Vector3.right : Vector3.up;
        Vector3 movementDirection = Vector3.Cross(spawnLocation, rightAngleVector);
        Vector3 movementVector = Quaternion.AngleAxis(Random.Range(0f, 360f), spawnLocation) * movementDirection;

        GameObject spawnedCloud = (GameObject)GameObject.Instantiate(cloudPrefab, spawnLocation * 50f, Quaternion.LookRotation(movementDirection, spawnLocation));
        SphereSurfaceSlider slider = spawnedCloud.GetComponent<SphereSurfaceSlider>();
        slider.sphericalVelocity = Random.Range(minCloudSpeed,maxCloudSpeed)*movementVector;
    }
}
