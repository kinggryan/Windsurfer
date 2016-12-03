using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class LevelUnlockManager {

	// MARK: Types
	[System.Serializable]
	public struct LevelInfo 
	{
		public string name;
		public string sceneName;
		public bool unlocked;
		public bool completed;
	}

	// MARK: Private Properties
	private static LevelUnlockManager sharedInstance;
	private LevelInfo[] allLevelInfo;

	// MARK: Private methods
	private LevelUnlockManager() {
		// If this file exists, load the level info
		Debug.Log("File location: " + Application.persistentDataPath + "/saveData.gd");
		if (File.Exists (Application.persistentDataPath + "/saveData.gd")) {
			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/saveData.gd", FileMode.Open);
			allLevelInfo = (LevelInfo[])formatter.Deserialize (file);
			file.Close ();
		} else {
			// TODO: Use some sort of xml or other type of format to init this data. Shouldn't have to do it in source.
			allLevelInfo = new LevelInfo[8];

			for (int i = 0; i < 8; i++) {
				allLevelInfo [i].name = "World " + (i+1);
				allLevelInfo [i].sceneName = "Level1-" + (i+1);
				allLevelInfo [i].unlocked = false;
				allLevelInfo [i].completed = false;
			}

			allLevelInfo [0].unlocked = true;
		}
	}

	// MARK: Static Methods
	public static LevelUnlockManager SharedInstance() {
		if (sharedInstance == null)
			sharedInstance = new LevelUnlockManager ();

		return sharedInstance;
	}

	public void LevelCompleted(string sceneName) {
		int levelNumber = GetLevelNumberFromSceneName (sceneName);

		allLevelInfo [levelNumber].completed = true;
		if (levelNumber + 1 < allLevelInfo.Length)
			allLevelInfo [levelNumber + 1].unlocked = true;

		Save ();
	}

	public bool IsGameComplete() {
		foreach (LevelInfo levelInfo in allLevelInfo) {
			if (!levelInfo.completed)
				return false;
		}
		return true;
	}

	public bool IsLevelUnlocked(string sceneName) {
		int levelNumber = GetLevelNumberFromSceneName (sceneName);
		return allLevelInfo [levelNumber].unlocked;
	}

	public string GetLevelSceneNameAfter(string sceneName) {
		int levelNumber = GetLevelNumberFromSceneName (sceneName);
		if (levelNumber + 1 < allLevelInfo.Length)
			return allLevelInfo [levelNumber + 1].sceneName;
		else
			return null;
	}

	private int GetLevelNumberFromSceneName(string sceneName) {
		int levelNumber = 0;
		for (int i = 0; i < allLevelInfo.Length; i++) {
			if (allLevelInfo [i].sceneName == sceneName) {
				levelNumber = i;
				break;
			}
		}
		return levelNumber;
	}

	private void Save()
	{
		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream fileStream = File.Create (Application.persistentDataPath + "/saveData.gd");
		formatter.Serialize (fileStream, allLevelInfo);
		fileStream.Close ();
	}
}
