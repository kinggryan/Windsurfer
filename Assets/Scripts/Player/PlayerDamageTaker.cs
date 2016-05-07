﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerDamageFlicker))]
public class PlayerDamageTaker : MonoBehaviour {

    public int damageTakenOnHit;
    public float invincibilityTime = 0.5f;

    public float hitMountainShakeIntensity = 0.5f;
    public GameObject hitObstacleParticleEffect;

    public AudioSource hitSound;

    private PlayerHealth health;
    private PlayerMouseController controller;
    private PlayerDamageFlicker flickerer;
    private bool invincible = false;

	// Use this for initialization
	void Start () {
        health = GetComponent<PlayerHealth>();
        controller = GetComponent<PlayerMouseController>();
        flickerer = GetComponent<PlayerDamageFlicker>();
    }
	
	void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<PlayerObstacle>() && !invincible)
        {
            TakeDamage(collider.transform.position);
        }
    }

    public void RainRanOut()
    {
        health.TakeDamage(1);
    }

    public void TakeDamage(Vector3 damageOrigin)
    {
        //   health.TakeDamage(damageTakenOnHit);
        controller.TakeDamageAndLoseRain();
        flickerer.TakeDamage();
        StartCoroutine(BecomeInvincible(invincibilityTime));

        // Shake camera
        Vector3 shakeDirection = Camera.main.transform.InverseTransformDirection(transform.position - damageOrigin);
        shakeDirection.z = 0;
        shakeDirection.Normalize();
        Camera.main.GetComponent<CameraShaker>().ShakeInDirectionWithIntensity(shakeDirection, hitMountainShakeIntensity);

        // Show particle effect
        GameObject.Instantiate(hitObstacleParticleEffect, transform.position, Quaternion.LookRotation(-1 * transform.forward, transform.up));

        hitSound.Play();
    }

    public IEnumerator BecomeInvincible(float time)
    {
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
    }
}
