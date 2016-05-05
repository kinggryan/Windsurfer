using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public void LevelComplete()
    {
        LevelDifficultyManager.LevelComplete();
        Object.FindObjectOfType<PlayerCameraController>().LevelComplete(EndLevelAnimationComplete);
        Object.FindObjectOfType<ScreenFlash>().LevelCompleteFadeOut();
    }

    public void EndLevelAnimationComplete()
    {
        if(LevelDifficultyManager.GameComplete())
        {
            Object.FindObjectOfType<UIController>().GameComplete();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // "LevelSelect");
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
