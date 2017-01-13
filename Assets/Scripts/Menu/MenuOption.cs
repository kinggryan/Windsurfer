using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum MenuOptionActionType
{
    Level,
	Credits,
	None
};

public class MenuOption : MonoBehaviour {

	public LoadLevelAnimator loadLevelAnimator;
    public MenuOptionActionType actionType = MenuOptionActionType.Level;
    public string loadLevelString;

    bool unlocked;
    public Renderer planetRenderer;
    public Color lockedPlanetColor;

	private static bool selectionsLocked;

	void Start() {
		selectionsLocked = false;

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

	public void BecomeUnhighlightedAction()
	{
		switch (actionType) {
		case MenuOptionActionType.Level:
			LevelPlanetRotator newPlanetRotator = GetComponent<LevelPlanetRotator> ();
			if (newPlanetRotator)
				newPlanetRotator.Deselect ();
			break;
		case MenuOptionActionType.Credits:
			Credits credits = GetComponent<Credits> ();
			if (credits) {
				credits.selected = false;
				credits.Reset ();
			}
			break;
		default:
			break;
		}
	}

	public void BecomeHighlightedAction()
	{
		switch (actionType) {
		case MenuOptionActionType.Level:
			LevelPlanetRotator newPlanetRotator = GetComponent<LevelPlanetRotator> ();
			if (newPlanetRotator)
				newPlanetRotator.Select ();
			break;
		case MenuOptionActionType.Credits:
			Credits credits = GetComponent<Credits> ();
			if (credits)
				credits.selected = true;
			break;
		default:
			break;
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
		if (!selectionsLocked) {
			selectionsLocked = true;
			loadLevelAnimator.LoadLevel (loadLevelString);
		}
    }

    public void Lock()
    {
        unlocked = false;
//        Debug.Log(loadLevelString + " is locked.");
    }

    public void Unlock()
    {
        unlocked = true;
//        Debug.Log(loadLevelString + " is unlocked.");
    }
}
