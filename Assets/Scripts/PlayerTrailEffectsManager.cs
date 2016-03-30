﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(AudioSource))]
public class PlayerTrailEffectsManager : MonoBehaviour {

    public Renderer playerRenderer;

    [Header("Camera Speed Parameters")]
    public float maxSpeedCameraDistance = 30f;
    public float minSpeedCameraDistance = 20f;
    public float maxSpeedFOV = 60f;
    public float minSpeedFOV = 50f;

    [Header("Animations")]
    public GameObject playerModel;
    public float playerMaxTurnRate;
    public float maxTurnTiltAngle = 30f;
    private Vector3 previousSphericalMovementVector;
    private float previousTurnRate;

    [Header("Rain Effects")]
    public AnimationCurve rainEmissionCurve;
    public float chargeRainEmissionRate;

    public Color chargingTrailColor;
    public ParticleSystem rainParticleSystem;
    public ParticleSystem chargeRainParticleSystem;
    public ParticleSystem boostRainParticleSystem;

    [Header("Audio")]
    public AudioSource rainOverWaterAudio;
    public AudioSource boostAudio;

    public float rainOverWaterAudioMaxVolume = 0.5f;

    private TrailRenderer trailRenderer;
    private float rainMeter;
    private bool charging = false;
    private float chargeRainMultiplier;

    public float rainOverWaterAudioMaxTurnTargetVolume;
    public float rainOverWaterAudioMinTurnTargetVolume;

    [Header("UI")]
    public UnityEngine.UI.Image rainLossDamageOverlay;
    private float rainLossDamageOverlayMaxOpacity;

    [Header("Boost Flicker")]
    public Color boostFlickerColor;
    public float boostFlickerDuration;
    public float boostFlickerPeriod;

	// Use this for initialization
	void Start () {
        trailRenderer = GetComponent<TrailRenderer>();
        ParticleSystem.EmissionModule emission = chargeRainParticleSystem.emission;
        emission.enabled = false;
        rainOverWaterAudio.volume = 0f;
        rainLossDamageOverlayMaxOpacity = rainLossDamageOverlay.color.a;
    }

    // Update is called once per frame
    void Update () {
        rainParticleSystem.transform.Rotate(transform.up,90f*Time.deltaTime);
	}

    public void StartCharging()
    {
        charging = true; 
        if (rainMeter > 0)
        {
            ParticleSystem.EmissionModule emission = chargeRainParticleSystem.emission;
            emission.enabled = true;
        }
    }

    public void StopCharging()
    {
        charging = false;
        ParticleSystem.EmissionModule emission = chargeRainParticleSystem.emission;
        emission.enabled = false;
        if(rainMeter > 0)
        {
            boostRainParticleSystem.Play();
            boostAudio.Play();
        }
    }

    public void StartBraking()
    {

    }

    public void StopBraking()
    {

    }

    public void UpdateMovement(Vector3 playerSphericalMotionVector,Vector3 playerLookDirection,float playerMaxTurnDirection)
    {
        chargeRainMultiplier = Vector3.Angle(playerSphericalMotionVector, playerLookDirection) / playerMaxTurnDirection;

        float sign = Vector3.Angle(Vector3.Cross(playerSphericalMotionVector, transform.position), previousSphericalMovementVector) <= 90 ? -1 : 1;
        float turnRate = Vector3.Angle(playerSphericalMotionVector, previousSphericalMovementVector) / Time.deltaTime;

        // Debug.Log(Vector3.Angle(playerSphericalMotionVector, previousSphericalMovementVector));
        //Debug.Log(playerSphericalMotionVector);
        float visibleTurnRate = Mathf.Lerp(previousTurnRate, turnRate, 0.5f);
        visibleTurnRate = Mathf.MoveTowards(visibleTurnRate, 0, 15);
        visibleTurnRate *= 1.5f;
       // Debug.Log(visibleTurnRate);
      // Debug.Log((playerSphericalMotionVector - previousSphericalMovementVector).magnitude);

        Vector3 newEulerAngles = playerModel.transform.localEulerAngles;
        newEulerAngles.y = 90 + sign * visibleTurnRate / playerMaxTurnRate * maxTurnTiltAngle;
        playerModel.transform.localEulerAngles = newEulerAngles;

        previousSphericalMovementVector = playerSphericalMotionVector;
        previousTurnRate = turnRate;
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
            if (charging)
            {
                emission = chargeRainParticleSystem.emission;
                if (!emission.enabled)
                {
                    emission.enabled = true;
                }
                emission.rate = new ParticleSystem.MinMaxCurve(chargeRainMultiplier * chargeRainEmissionRate);

                var rainVolume = chargeRainMultiplier * (rainOverWaterAudioMaxTurnTargetVolume - rainOverWaterAudioMinTurnTargetVolume) + rainOverWaterAudioMinTurnTargetVolume;
                rainOverWaterAudio.volume = Mathf.MoveTowards(rainOverWaterAudio.volume, rainVolume, Time.deltaTime*5);
            }
            else
            {
                rainOverWaterAudio.volume = Mathf.MoveTowards(rainOverWaterAudio.volume, 0, Time.deltaTime*5);
            }
        }
        else
        {
            emission.enabled = false;
            if (charging && chargeRainParticleSystem.emission.enabled)
            {
                emission = chargeRainParticleSystem.emission;
                emission.enabled = false;
            }
            rainOverWaterAudio.volume = Mathf.MoveTowards(rainOverWaterAudio.volume, 0, Time.deltaTime * 5);
        }
    }

    public void BoostReady()
    {
        StartCoroutine(FlickerBoostColor(boostFlickerDuration));
    //    Debug.Log("Boost ready" + Time.time);
    }

    public IEnumerator FlickerBoostColor(float totalTimeLeft)
    {
        yield return new WaitForSeconds(boostFlickerPeriod);

        Color currentColor = playerRenderer.material.color;
        if(currentColor == boostFlickerColor || totalTimeLeft <= 0)
        {
            playerRenderer.material.color = Color.white;
        }
        else
        {
            playerRenderer.material.color = Color.white; //boostFlickerColor;
        }

        if(totalTimeLeft > 0)
            StartCoroutine(FlickerBoostColor(totalTimeLeft - boostFlickerPeriod));
    }

    public void SetPlayerRainDamagePercent(float rainDamageTimerPercent)
    {
        // 1.0 is taking damage, 0 is not at all.
        Color color = rainLossDamageOverlay.color;
        color.a = rainDamageTimerPercent * rainLossDamageOverlayMaxOpacity;
        rainLossDamageOverlay.color = color;
    }
}
