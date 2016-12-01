using UnityEngine;
using System.Collections;

public class LevelUnlockManager {

	// MARK: Types
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
		// TODO: Use some sort of xml or other type of format to init this data. Shouldn't have to do it in source.
		allLevelInfo = new LevelInfo[8];

		for (int i = 0; i < 8; i++) {
			allLevelInfo [i].name = "World " + i;
			allLevelInfo [i].sceneName = "Level1-" + i;
			allLevelInfo [i].unlocked = false;
			allLevelInfo [i].completed = false;
		}

		allLevelInfo [0].unlocked = true;
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
	}

	public bool IsGameComplete() {
		foreach (LevelInfo levelInfo in allLevelInfo) {
			if (!levelInfo.completed)
				return false;
		}
		return true;
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
}
