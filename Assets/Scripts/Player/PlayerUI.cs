﻿using UnityEngine;
using System.Collections;

public class PlayerUI : MonoBehaviour {

    public UnityEngine.UI.Slider rainMeterSlider;
	public UnityEngine.UI.Image rainTimerImage;
	public Animator rainTimerAnimator;
    public float rainMeterSliderMaxDelta = 1.0f;

	public GameObject hexFlickerPrefab;

    private float rainMeter = 0;

	private float previousRainTimerAmount = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateRainMeter(float rainMeter)
    {
        this.rainMeter = rainMeter;
        rainMeterSlider.value = Mathf.MoveTowards(rainMeterSlider.value, this.rainMeter, rainMeterSliderMaxDelta * Time.deltaTime);
    }

	public void UpdateRainTimer(float timePercent)
	{
//		rainTimerAnimator.SetBool ("hurt", timePercent < 1.0);
//		Debug.Log (timePercent < 1.0);
		rainTimerAnimator.SetFloat("timerPercent",timePercent);

		if (timePercent > previousRainTimerAmount) {
			// Spawn effect that matches grounds filled effect
			GameObject flickerObj = GameObject.Instantiate(hexFlickerPrefab,rainTimerImage.transform.position,rainTimerImage.transform.rotation);
			flickerObj.transform.parent = rainTimerImage.transform.parent;
		}
		rainTimerImage.fillAmount = timePercent;
		previousRainTimerAmount = timePercent;
	}
}
