using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundsRemainingController : MonoBehaviour {

    public float percentGroundsNeeded = 1.0f;

    public UnityEngine.UI.Text victoryText;
    public UnityEngine.UI.Text groundsLeftText;
    public GameObject[] buttonsToEnableOnVictory;

    public Color allDesertSkyColor;
    public Color levelCompleteColor;

    private int totalGrounds;
    private int groundsNeeded;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GroundRemoved()
    {
        if(--groundsNeeded <= 0)
        {
            victoryText.enabled = true;
            foreach(GameObject obj in buttonsToEnableOnVictory)
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

        groundsLeftText.text = "" + groundsNeeded;
        float percentComplete = 1 - 1.0f * groundsNeeded / Mathf.FloorToInt(totalGrounds * percentGroundsNeeded);
        Camera.main.backgroundColor = percentComplete * levelCompleteColor + (1 - percentComplete) * allDesertSkyColor;

        ScreenFlash sFlash = Object.FindObjectOfType<ScreenFlash>();
        if (sFlash)
            sFlash.ForestComplete();
    }

    public void InitGround()
    {
        totalGrounds++;
        groundsNeeded = Mathf.FloorToInt(totalGrounds * percentGroundsNeeded);
        groundsLeftText.text = "" + groundsNeeded;
        Camera.main.backgroundColor = allDesertSkyColor;
    }
}
