using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void NextLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
