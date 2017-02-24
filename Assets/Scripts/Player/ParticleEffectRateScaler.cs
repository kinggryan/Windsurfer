using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleEffectRateScaler : MonoBehaviour {

	private float multiplierScalar;
	private ParticleSystem system;
	private ParticleSystem.EmissionModule emission;

	// Use this for initialization
	void Start () {
		system = GetComponent<ParticleSystem> ();
		emission = system.emission;
		multiplierScalar = emission.rateOverTimeMultiplier;
	}

	public void SetEmissionRateMultiplier(float multiplier) {
		emission.rateOverTimeMultiplier = multiplierScalar*multiplier;
	}
}
