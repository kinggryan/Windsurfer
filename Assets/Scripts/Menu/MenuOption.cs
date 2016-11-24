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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
}
