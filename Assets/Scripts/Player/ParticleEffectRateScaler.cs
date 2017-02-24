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
		StartCoroutine(StartPlayingInSeconds(1.5f));
//		emission.rateOverTimeMultiplier = 0;
//		system.Play ();
	}

	public void SetEmissionRateMultiplier(float multiplier) {
//		if (!system.isPlaying)
//			system.Play ();
		emission.rateOverTimeMultiplier = multiplierScalar*multiplier;
	}

	IEnumerator StartPlayingInSeconds(float seconds) {
		yield return new WaitForSeconds (seconds);

		system.Play ();
	}
}
