using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
public class PlayerTrailEffectsManager : MonoBehaviour {

    public Color chargingTrailColor;

    private TrailRenderer trailRenderer;

	// Use this for initialization
	void Start () {
        trailRenderer = GetComponent<TrailRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartCharging()
    {
        
    }

    public void StopCharging()
    {

    }

    public void StartBraking()
    {

    }

    public void StopBraking()
    {

    }
}
