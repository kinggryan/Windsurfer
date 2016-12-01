using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public void LevelComplete()
    {
		LevelUnlockManager.SharedInstance ().LevelCompleted (SceneManager.GetActiveScene ().name);
        Object.FindObjectOfType<PlayerCameraController>().LevelComplete(EndLevelAnimationComplete);
        Object.FindObjectOfType<ScreenFlash>().LevelCompleteFadeOut();
    }

    public void EndLevelAnimationComplete()
    {
		if(LevelUnlockManager.SharedInstance().IsGameComplete())
        {
            Object.FindObjectOfType<UIController>().GameComplete();
        }
        else
        {
			SceneManager.LoadScene(LevelUnlockManager.SharedInstance().GetLevelSceneNameAfter(SceneManager.GetActiveScene().name)); // "LevelSelect");
        }
    }
}
