using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerDamageTaker : MonoBehaviour {

    public int damageTakenOnHit;
    public float invincibilityTime = 0.5f;

    public float hitMountainShakeIntensity = 0.5f;

    public AudioSource hitSound;

    private PlayerHealth health;
    private PlayerMouseController controller;
    private bool invincible = false;

	// Use this for initialization
	void Start () {
        health = GetComponent<PlayerHealth>();
        controller = GetComponent<PlayerMouseController>();

    }
	
	// Update is called once per frame
	void Update () {
	
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
        StartCoroutine(BecomeInvincible(invincibilityTime));

        // Shake camera
        Vector3 shakeDirection = Camera.main.transform.InverseTransformDirection(transform.position - damageOrigin);
        shakeDirection.z = 0;
        shakeDirection.Normalize();
        Camera.main.GetComponent<CameraShaker>().ShakeInDirectionWithIntensity(shakeDirection, hitMountainShakeIntensity);

        hitSound.Play();
    }

    public IEnumerator BecomeInvincible(float time)
    {
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
    }
}
