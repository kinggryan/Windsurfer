using UnityEngine;
using System.Collections;

public class DistantPlanetFader : MonoBehaviour {

    public AnimationCurve cameraDistanceToAlphaCurve;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float alpha = Vector3.Distance(transform.position, Camera.main.transform.position);
        Debug.Log(alpha);
        Color curColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = new Color(curColor.r, curColor.g, curColor.b, cameraDistanceToAlphaCurve.Evaluate(alpha));
        // TODO: Disable renderer most of the time
    }
}
