using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum MenuOptionActionType
{
    Level
};

public class MenuOption : MonoBehaviour {

    public MenuOptionActionType actionType = MenuOptionActionType.Level;
    public string loadLevelString;

    bool unlocked;
    public Renderer planetRenderer;
    public Color lockedPlanetColor;

	void Start() {
		if (LevelUnlockManager.SharedInstance ().IsLevelUnlocked (loadLevelString))
			Unlock ();
		else
			Lock ();
	}

	// Update is called once per frame
	void Update () {
	    // Fade towards correct color
        if(planetRenderer)
        {
            foreach(Material mat in planetRenderer.materials)
                mat.color = Color.Lerp(planetRenderer.sharedMaterial.color, unlocked ? Color.white : lockedPlanetColor, Time.deltaTime / 0.1f);
        }
	}

    public void PerformAction()
    {
        switch(actionType)
        {
            case MenuOptionActionType.Level:
                LoadLevel();
                break;
            default:
                break;
        }
    }

    void LoadLevel()
    {
        SceneManager.LoadSceneAsync(loadLevelString);
    }

    public void Lock()
    {
        unlocked = false;
    }

    public void Unlock()
    {
        unlocked = true;
    }
}
