using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
public class PlayerTrailEffectsManager : MonoBehaviour {

    public float maxSpeedCameraDistance = 30f;
    public float minSpeedCameraDistance = 20f;
    public float maxSpeedFOV = 60f;
    public float minSpeedFOV = 50f;

    public AnimationCurve rainEmissionCurve;

    public Color chargingTrailColor;
    public Renderer rainFromChargeIndicator;
    public ParticleSystem rainParticleSystem;
    public ParticleSystem chargeRainParticleSystem;
    public ParticleSystem boostRainParticleSystem;

    private TrailRenderer trailRenderer;
    private float rainMeter;

	// Use this for initialization
	void Start () {
        trailRenderer = GetComponent<TrailRenderer>();
        rainFromChargeIndicator.enabled = false;
        ParticleSystem.EmissionModule emission = rainParticleSystem.emission;
        emission.enabled = false;
        emission = chargeRainParticleSystem.emission;
        emission.enabled = false;
    }

    // Update is called once per frame
    void Update () {
        rainParticleSystem.transform.Rotate(transform.up,90f*Time.deltaTime);
	}

    public void StartCharging()
    {
        // rainFromChargeIndicator.enabled = true;
        if (rainMeter > 0)
        {
            ParticleSystem.EmissionModule emission = chargeRainParticleSystem.emission;
            emission.enabled = true;
        }
    }

    public void StopCharging()
    {
        //rainFromChargeIndicator.enabled = false;
        ParticleSystem.EmissionModule emission = chargeRainParticleSystem.emission;
        emission.enabled = false;
        if(rainMeter > 0)
        {
            boostRainParticleSystem.Play();
        }
    }

    public void StartBraking()
    {

    }

    public void StopBraking()
    {

    }

    public void UpdateMovement(Vector3 playerSphericalMotionVector,Vector3 playerLookDirection)
    {
   /*     float sign = Vector3.Angle(Vector3.Cross(playerSphericalMotionVector, transform.position), playerLookDirection) <= 90 ? -1 : 1;
        ParticleSystem.VelocityOverLifetimeModule vm = chargeRainParticleSystem.velocityOverLifetime;
        float min = Mathf.Min(sign * Vector3.Angle(playerSphericalMotionVector, playerLookDirection),0);
        float max = Mathf.Max(sign * Vector3.Angle(playerSphericalMotionVector, playerLookDirection), 0);
        vm.x = new ParticleSystem.MinMaxCurve(min, max); */
        //chargeRainParticleSystem.transform.localRotation = Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(-90, Vector3.forward);
    }

    public void UpdateSpeed(float speedToMaxSpeedRatio)
    {
       // Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition,new Vector3(Camera.main.transform.localPosition.x,Camera.main.transform.localPosition.y,minSpeedCameraDistance + (maxSpeedCameraDistance - minSpeedCameraDistance) * speedToMaxSpeedRatio),0.5f);
       // Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView,minSpeedFOV + (maxSpeedFOV - minSpeedFOV) * speedToMaxSpeedRatio,0.5f);
    }

    public void UpdateRainMeter(float rainMeter)
    {
        this.rainMeter = rainMeter;

        ParticleSystem.EmissionModule emission = rainParticleSystem.emission;

        if (rainMeter > 0)
        {
            emission.rate = new ParticleSystem.MinMaxCurve(rainEmissionCurve.Evaluate(rainMeter));
            emission.enabled = true;
        }
        else
        {
            emission.enabled = false;
        }
    }
}
