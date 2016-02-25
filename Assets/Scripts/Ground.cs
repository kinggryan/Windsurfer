﻿using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour {

    public Color desertColor;
    public Color forestFadeColor;
    public Color forestCompleteColor;

    public float fullRainTime;

    private float rainAmount = 0f;
    private bool forest = false;
    new private Renderer renderer;

	// Use this for initialization
	void Start () {
        renderer = GetComponent<Renderer>();
        renderer.material.color = rainAmount * forestFadeColor + (1 - rainAmount) * desertColor;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void RainedOnAtPoint(Vector3 rainPoint)
    {
        if (!forest) {
            rainAmount += Time.deltaTime * fullRainTime;
            if (rainAmount >= 1.0)
            {
                forest = true;
                renderer.material.color = forestCompleteColor;
            }
            else
            {
                renderer.material.color = rainAmount * forestFadeColor + (1 - rainAmount) * desertColor;
            }
        }
    }
}
