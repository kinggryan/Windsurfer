using UnityEngine;
using System.Collections;

public class MountainSpawner : MonoBehaviour {

    public float minSpawnTime = 5f;
    public float maxSpawnTime = 10f;
    public GameObject mountainPrefab;
    public float mountainStartHeight = 50f;
    public float finalMountainHeight = 75f;
    public float mountainRiseTime = 2f;
    public float mountainRadius = 15f;

    private GameObject currentlySpawningMountain;

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnMountain(0));
	}
	
	// Update is called once per frame
	void Update () {
	    if(currentlySpawningMountain)
        {
            float mountainDistance = currentlySpawningMountain.transform.position.magnitude;
            mountainDistance += Time.deltaTime * (finalMountainHeight - mountainStartHeight) / mountainRiseTime;
            if (mountainDistance >= finalMountainHeight)
            {
                mountainDistance = finalMountainHeight;
                currentlySpawningMountain.transform.position = mountainDistance * currentlySpawningMountain.transform.position.normalized;
                currentlySpawningMountain = null;
                StartCoroutine(SpawnMountain(Random.Range(minSpawnTime, maxSpawnTime)));
            }
            else
            {
                currentlySpawningMountain.transform.position = mountainDistance * currentlySpawningMountain.transform.position.normalized;
            }
        }
	}

    public IEnumerator SpawnMountain(float time)
    {
        yield return new WaitForSeconds(time);
        Vector3 spawnDirection = Random.onUnitSphere;
        while(!MountainCanSpawnInDirection(spawnDirection))
        {
            spawnDirection = Random.onUnitSphere;
        }

        Vector3 spawnPosition = mountainStartHeight * spawnDirection;
        Quaternion spawnRotation = Quaternion.LookRotation(Vector3.Cross(spawnPosition, Vector3.up), spawnPosition.normalized);
        currentlySpawningMountain = (GameObject)GameObject.Instantiate(mountainPrefab, spawnPosition, spawnRotation);
     //   SurfaceObjectOrienter orienter = currentlySpawningMountain.GetComponent<SurfaceObjectOrienter>();
     //   if(orienter)
      //  {
       //     Object.Destroy(orienter);
       // }
    }

    bool MountainCanSpawnInDirection(Vector3 direction)
    {
        return !Physics.CheckCapsule(Vector3.zero, finalMountainHeight*direction, mountainRadius);
    } 
}
