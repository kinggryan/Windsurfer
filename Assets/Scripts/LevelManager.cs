using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    public void LevelComplete()
    {
        LevelDifficultyManager.LevelComplete();
        Object.FindObjectOfType<PlayerCameraController>().LevelComplete();
    }

    public void EndLevelAnimationComplete()
    {
        if(LevelDifficultyManager.GameComplete())
        {
            Object.FindObjectOfType<UIController>().GameComplete();
        }
        else
        {
            Application.LoadLevel(Application.loadedLevel);
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
