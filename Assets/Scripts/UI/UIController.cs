using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

	public UnityEngine.UI.Text[] victoryText;

    public GameObject nextLevelButton;
    public GameObject quitButton;

	private bool gameComplete = false;
	private float gameCompleteTimer = 0;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (gameComplete) {
			gameCompleteTimer += Time.deltaTime;

			if(gameCompleteTimer >= 3f && (MobileInput.GetTouchDown() || Input.GetButtonDown ("Boost")))
				SceneManager.LoadScene ("Menu");
		}
	}

    public void NextLevel()
    {
        if (LevelDifficultyManager.GameComplete()) {
            GameComplete();
        }
        else
            Application.LoadLevel(0);
    }

    public void GameComplete()
    {
//        victoryText.text = "You Win!";
//        victoryText.enabled = true;
		foreach (UnityEngine.UI.Text vtext in victoryText)
			vtext.enabled = true;

		gameComplete = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LevelComplete()
    {
        LevelDifficultyManager.LevelComplete();

		foreach (UnityEngine.UI.Text vtext in victoryText)
			vtext.enabled = true;

        GameObject[] buttonsToEnableOnVictory = { quitButton };
        if(!LevelDifficultyManager.GameComplete())
        {
            buttonsToEnableOnVictory = new GameObject[2] { quitButton, nextLevelButton };
        }
        else
        {
//            victoryText.text = "You won the game!";
        }

        foreach (GameObject obj in buttonsToEnableOnVictory)
        {
            List<MonoBehaviour> components = new List<MonoBehaviour>();
            components.AddRange(obj.GetComponentsInChildren<UnityEngine.UI.Text>());
            components.AddRange(obj.GetComponents<UnityEngine.UI.Image>());
            components.AddRange(obj.GetComponents<UnityEngine.UI.Button>());

            foreach (MonoBehaviour mb in components)
            {
                mb.enabled = true;
            }
        }
    }
}
