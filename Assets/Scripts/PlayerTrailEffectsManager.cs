using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
public class PlayerTrailEffectsManager : MonoBehaviour {

    public Color chargingTrailColor;
    public Renderer rainFromChargeIndicator;
    public ParticleSystem chargeRainParticleSystem;

    private TrailRenderer trailRenderer;

	// Use this for initialization
	void Start () {
        trailRenderer = GetComponent<TrailRenderer>();
        rainFromChargeIndicator.enabled = false;
        ParticleSystem.EmissionModule emission = chargeRainParticleSystem.emission;
        emission.enabled = false;
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void StartCharging()
    {
       // rainFromChargeIndicator.enabled = true;
        ParticleSystem.EmissionModule emission = chargeRainParticleSystem.emission;
        emission.enabled = true;
    }

    public void StopCharging()
    {
        rainFromChargeIndicator.enabled = false;
        ParticleSystem.EmissionModule emission = chargeRainParticleSystem.emission;
        emission.enabled = false;
    }

    public void StartBraking()
    {

    }

    public void StopBraking()
    {

    }
}
