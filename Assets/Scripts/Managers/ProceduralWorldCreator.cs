using UnityEngine;
using System.Collections;

public class ProceduralWorldCreator : MonoBehaviour {

    [Header("Tile Attributes")]
    public GameObject groundPrefab;
    public float sphereRadius;
    public float groundRadius;

    [Header("Continent Attributes")]
    public int minTilesPerContinent;
    public int maxTilesPerContinent;

    [Header("World Attributes")]
    public int minNumContinents;
    public int maxNumContinents;

	// Use this for initialization
	void Start () {
        int numConts = Random.Range(minNumContinents, maxNumContinents);
	    for(int i = 0; i < numConts; i++)
        {
            GenerateContinent(Random.onUnitSphere);
        }
	}

    void GenerateContinent(Vector3 continentDirection)
    {
        int numTiles = Random.Range(minTilesPerContinent, maxTilesPerContinent);
        int startingNumTiles = numTiles;
        Vector3 spawnDirection = continentDirection;

        while(numTiles > 0)
        {
            // The ground will take care of its own orientation and ensuring its distance is appropriate
            if (numTiles == startingNumTiles) {
                Quaternion lookQuat = Quaternion.LookRotation(Vector3.Cross(Vector3.right, spawnDirection), spawnDirection);
                GameObject.Instantiate(groundPrefab, continentDirection * sphereRadius, lookQuat);
                numTiles--;
            }
            else
            {
                numTiles = GenerateContinentHexagonLayer(continentDirection, 1, numTiles);
            }
        }
    }

    // TODO; Make this method actually work for a layer other than layer 1.
    int GenerateContinentHexagonLayer(Vector3 continentDirection,int layerNumber,int numTiles)
    {
        // Each layer has a tiles per side equal to the layer number, where layer 0 is the tile at the very center
        Vector3 directionAtRightAngle = continentDirection == Vector3.up ? Vector3.right : Vector3.up;
        Vector3 rotationalAxis = Vector3.Cross(continentDirection, directionAtRightAngle);

        // Calculate how many degrees are in a tile's surface distance (approximately)
        float angularDistanceOfTile = (2 * groundRadius / (2 * Mathf.PI * sphereRadius)) * 360;
        Vector3 spawnDirection = Quaternion.AngleAxis(layerNumber*angularDistanceOfTile, rotationalAxis) * continentDirection;

        int tilesToSpawn = Mathf.Min(layerNumber * 6, numTiles);
        for(int i = 0; i < tilesToSpawn; i++)
        {
            Quaternion lookQuat = Quaternion.LookRotation(Vector3.Cross(Vector3.right, spawnDirection), spawnDirection);
            GameObject.Instantiate(groundPrefab, spawnDirection, lookQuat);
            spawnDirection = Quaternion.AngleAxis(60f, continentDirection) * spawnDirection;
        }

        return numTiles-tilesToSpawn;
    }
}
