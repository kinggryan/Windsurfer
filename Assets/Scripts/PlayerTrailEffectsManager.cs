using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
public class PlayerTrailEffectsManager : MonoBehaviour {

    public float maxSpeedCameraDistance = 30f;
    public float minSpeedCameraDistance = 20f;
    public float maxSpeedFOV = 60f;
    public float minSpeedFOV = 50f;

    public Color chargingTrailColor;
    public Renderer rainFromChargeIndicator;
    public ParticleSystem chargeRainParticleSystem;

    private TrailRenderer trailRenderer;

	// Use this for initialization
	void Start () {
        trailRenderer = GetComponent<TrailRenderer>();
        rainFromChargeIndicator.enabled = false;
  //      ParticleSystem.EmissionModule emission = chargeRainParticleSystem.emission;
  //      emission.enabled = false;
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void StartCharging()
    {
       // rainFromChargeIndicator.enabled = true;
      //  ParticleSystem.EmissionModule emission = chargeRainParticleSystem.emission;
//        emission.enabled = true;
    }

    public void StopCharging()
    {
        //rainFromChargeIndicator.enabled = false;
   //     ParticleSystem.EmissionModule emission = chargeRainParticleSystem.emission;
        //emission.enabled = false;
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
}
