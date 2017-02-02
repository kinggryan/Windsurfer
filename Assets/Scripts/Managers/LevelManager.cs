using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public void LevelComplete()
    {
		LevelUnlockManager.SharedInstance ().LevelCompleted (SceneManager.GetActiveScene ().name);
        Object.FindObjectOfType<PlayerCameraController>().LevelComplete(EndLevelAnimationComplete);
		if(!(LevelUnlockManager.SharedInstance().IsGameComplete() && LevelUnlockManager.SharedInstance().GetLevelSceneNameAfter(SceneManager.GetActiveScene().name) == null)) 
			Object.FindObjectOfType<ScreenFlash>().LevelCompleteFadeOut();
    }

    public void EndLevelAnimationComplete()
    {
		if(LevelUnlockManager.SharedInstance().IsGameComplete() && LevelUnlockManager.SharedInstance().GetLevelSceneNameAfter(SceneManager.GetActiveScene().name) == null)
        {
            Object.FindObjectOfType<UIController>().GameComplete();
        }
        else
        {
			SceneManager.LoadScene(LevelUnlockManager.SharedInstance().GetLevelSceneNameAfter(SceneManager.GetActiveScene().name)); // "LevelSelect");
        }
    }
}
