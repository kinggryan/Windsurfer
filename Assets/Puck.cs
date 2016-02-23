using UnityEngine;
using System.Collections;

public class Puck : MonoBehaviour {

    private SphereSurfaceSlider slider;

	// Use this for initialization
	void Start () {
        slider = GetComponent<SphereSurfaceSlider>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Goal>())
        {
            GameObject.Destroy(collider.gameObject);
            // TODO: Get score
        }
        if(collider.GetComponent<PlayerWindGust>())
        {
            PlayerWindGust gust = collider.GetComponent<PlayerWindGust>();
            gust.HitStorm(this);
        }

        if (collider.GetComponent<Goal>() == null && collider.transform != transform)
        {
            slider.SetSphericalVelocityWithLinearVelocity(slider.sphericalVelocity.magnitude * (transform.position - collider.transform.position).normalized);
            slider.StartCoroutine(slider.FreezeScreen(0.05f));
        }
    }
}
