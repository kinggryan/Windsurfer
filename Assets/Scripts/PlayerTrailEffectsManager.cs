using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
public class PlayerTrailEffectsManager : MonoBehaviour {

    public Color chargingTrailColor;
    public Renderer rainFromChargeIndicator;

    private TrailRenderer trailRenderer;

	// Use this for initialization
	void Start () {
        trailRenderer = GetComponent<TrailRenderer>();
        rainFromChargeIndicator.enabled = false;
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void StartCharging()
    {
        rainFromChargeIndicator.enabled = true;
    }

    public void StopCharging()
    {
        rainFromChargeIndicator.enabled = false;
    }

    public void StartBraking()
    {

    }

    public void StopBraking()
    {

    }
}
