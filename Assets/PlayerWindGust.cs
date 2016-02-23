using UnityEngine;
using System.Collections;

public class PlayerWindGust : MonoBehaviour {

    private SphereSurfaceSlider slider;
    private float remainingLifespan;

	// Use this for initialization
	void Start () {
        slider = GetComponent<SphereSurfaceSlider>();
	}

    void Update()
    {
        remainingLifespan -= Time.deltaTime;
        if(remainingLifespan <= 0)
        {
            GameObject.Destroy(gameObject);
        }
    }
	
	// Update is called once per frame
	public void LaunchWithSphericalVelocityAndLifespan(Vector3 sphericalVelocity,float lifespan)
    {
        GetComponent<SphereSurfaceSlider>().sphericalVelocity = sphericalVelocity;
        remainingLifespan = lifespan;
    }

    void OnCollisionEnter(Collision collision)
    {
        Puck puck = collision.collider.GetComponent<Puck>();
        SphereSurfaceSlider otherSlider = collision.collider.GetComponent<SphereSurfaceSlider>();

        if(otherSlider && puck)
        {
            otherSlider.HitByStormWithSphericalVelocity(slider.sphericalVelocity);
            GameObject.Destroy(gameObject);
        }
    }

    public void HitStorm(Puck puck)
    {
        SphereSurfaceSlider otherSlider = puck.GetComponent<SphereSurfaceSlider>();

        if (otherSlider && puck)
        {
            otherSlider.HitByStormWithSphericalVelocity(0.5f*slider.sphericalVelocity);
           // otherSlider.HitByStormAtPosition(transform.position);
            GameObject.Destroy(gameObject);
        }
    }
}
