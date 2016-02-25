using UnityEngine;
using System.Collections;

public class GoalSpawner : MonoBehaviour {

    public float minSpawnTime = 4.0f;
    public float maxSpawnTime = 9.0f;
    public GameObject objToSpawn;
    public float spawnRadius = 101f;

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnObjs());
	}

    IEnumerator SpawnObjs()
    {
        Vector3 spawnPos = Random.onUnitSphere * spawnRadius;
        GameObject.Instantiate(objToSpawn, spawnPos,Quaternion.LookRotation(Vector3.Cross(spawnPos,Vector3.up),spawnPos));
        yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
        StartCoroutine(SpawnObjs());
    }

    // Update is called once per frame
    void Update () {
	
	}
}
