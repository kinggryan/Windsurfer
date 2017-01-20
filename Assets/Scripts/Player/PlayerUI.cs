using UnityEngine;
using System.Collections;

public class PlayerUI : MonoBehaviour {

    public UnityEngine.UI.Slider rainMeterSlider;
	public UnityEngine.UI.Image rainTimerImage;
    public float rainMeterSliderMaxDelta = 1.0f;
    private float rainMeter = 0;

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
		rainTimerImage.fillAmount = timePercent;
	}
}
