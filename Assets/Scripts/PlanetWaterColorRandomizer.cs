using UnityEngine;
using System.Collections;

public class PlanetWaterColorRandomizer : MonoBehaviour {

    public Material planetWaterMaterial;
    public Gradient planetWaterGradient;

	// Use this for initialization
	void Start () {
        planetWaterMaterial.color = planetWaterGradient.Evaluate(Random.value);
	}
}
