using UnityEngine;
using System.Collections;

[System.Serializable]
public struct LevelParameters
{
    public int minNumContinents;
    public int maxNumContinents;
    public float minMountainSpawnRate;
    public float maxMountainSpawnRate;
    public int minNumClouds;
    public int maxNumClouds;
}

[RequireComponent(typeof(ProceduralWorldCreator))]
[RequireComponent(typeof(CloudSpawner))]
[RequireComponent(typeof(MountainSpawner))]
public class LevelDifficultyManager : MonoBehaviour {

    public LevelParameters[] levels;
    static int levelNumber = 0;
    static int numLevels;

	// Use this for initialization
	void Start () {
        ProceduralWorldCreator worldCreator = GetComponent<ProceduralWorldCreator>();
        CloudSpawner cloudSpawner = GetComponent<CloudSpawner>();
        MountainSpawner spawner = GetComponent<MountainSpawner>();

        LevelParameters level = levels[levelNumber];
        worldCreator.minNumContinents = level.minNumContinents;
        worldCreator.maxNumContinents = level.maxNumContinents;
        cloudSpawner.minCloudCount = level.minNumClouds;
        cloudSpawner.maxCloudCount = level.maxNumClouds;
        spawner.minSpawnTime = level.minMountainSpawnRate;
        spawner.maxSpawnTime = level.maxMountainSpawnRate;

        numLevels = 3; //levels.Length;
	}

	// Update is called once per frame
	static public bool LevelComplete () {
        levelNumber++;
        return GameComplete();
	}

    static public void LevelLost()
    {
        levelNumber = 0;
    }

    static public bool GameComplete()
    {
        return levelNumber >= numLevels;
    }
}
