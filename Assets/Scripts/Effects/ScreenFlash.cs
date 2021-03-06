﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class ScreenFlash : MonoBehaviour {

    public Gradient forestCompleteGradient;
    public float forestCompleteFlashTime = 0.3f;
    public Gradient levelCompleteGradient;
    public float levelCompleteDuration;
    public Gradient levelStartGradient;
    public float levelStartDuration;

    private Gradient currentGradient;
    private float currentFlashDuration;
    private float currentFlashTime;
    private UnityEngine.UI.Image screenOverlay;

	// Use this for initialization
	void Start () {
        screenOverlay = GetComponent<UnityEngine.UI.Image>();
        currentGradient = null;
        LevelStart();
	}
	
	// Update is called once per frame
	void Update () {
	    if(currentGradient != null)
        {
            screenOverlay.color = currentGradient.Evaluate(currentFlashTime / currentFlashDuration);
            currentFlashTime += Time.deltaTime;
            if(currentFlashTime >= currentFlashDuration)
            {
                currentFlashTime = currentFlashDuration;
                currentGradient = null;
            }
        }
	}

    void FlashScreen(Gradient flashGradient,float flashDuration)
    {
        currentFlashTime = 0f;
        currentFlashDuration = flashDuration;
        currentGradient = flashGradient;
    }

    public void ForestComplete()
    {
    //    FlashScreen(forestCompleteGradient, forestCompleteFlashTime);
    }

    public void LevelCompleteFadeOut()
    {
        FlashScreen(levelCompleteGradient, levelCompleteDuration);
    }

    public void LevelStart()
    {
        FlashScreen(levelStartGradient, levelStartDuration);
    }
}
