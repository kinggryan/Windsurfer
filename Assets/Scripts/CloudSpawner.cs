using UnityEngine;
using System.Collections;

public class CloudSpawner : MonoBehaviour {

    [Header("Clouds")]
    public float minCloudSpawnTime = 5f;
    public float maxCloudSpawnTime = 10f;
    public float minCloudSpeed = 15;
    public float maxCloudSpeed = 25;
    public GameObject cloudPrefab;
    public int minCloudCount = 2;
    public int maxCloudCount = 5;

    [Header("Storms")]
    public float minStormSpawnTime = 30f;
    public float maxStormSpawnTime = 45f;
    public float minStormSpeed = 15;
    public float maxStormSpeed = 25;
    public GameObject stormPrefab;

    // Use this for initialization
    void Start()
    {
        for(int i = 0; i < minCloudCount; i++)
        {
            SpawnCloud();
        }

        StartCoroutine(SpawnCloudTimer(Random.Range(minCloudSpawnTime, maxCloudSpawnTime)));
        StartCoroutine(SpawnStormTimer(Random.Range(minStormSpawnTime, maxStormSpawnTime)));
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
        StartCoroutine(SpawnCloudTimer(Random.Range(minCloudSpawnTime, maxCloudSpawnTime)));
    }

    public IEnumerator SpawnStormTimer(float time)
    {
        yield return new WaitForSeconds(time);
        SpawnStorm();
        StartCoroutine(SpawnStormTimer(Random.Range(minStormSpawnTime, maxStormSpawnTime)));
    }

    void SpawnCloud()
    {
        // Choose a random location
        SpawnWorldObj(cloudPrefab, minCloudSpeed, maxCloudSpeed);
    }

    void SpawnStorm()
    {
        SpawnWorldObj(stormPrefab, minStormSpeed, maxStormSpeed);
    }

    void SpawnWorldObj(GameObject prefab,float minSpeed, float maxSpeed)
    {
        // Choose a random location
        Vector3 spawnLocation = Random.onUnitSphere;
        Vector3 rightAngleVector = spawnLocation == Vector3.up ? Vector3.right : Vector3.up;
        Vector3 movementDirection = Vector3.Cross(spawnLocation, rightAngleVector);
        Vector3 movementVector = Quaternion.AngleAxis(Random.Range(0f, 360f), spawnLocation) * movementDirection;

        GameObject spawnedObject = (GameObject)GameObject.Instantiate(prefab, spawnLocation * 50f, Quaternion.LookRotation(movementDirection, spawnLocation));
        SphereSurfaceSlider slider = spawnedObject.GetComponent<SphereSurfaceSlider>();
        slider.sphericalVelocity = Random.Range(minSpeed, maxSpeed) * movementVector;
    }
}
