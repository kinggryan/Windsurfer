using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerDamageTaker : MonoBehaviour {

    public int damageTakenOnHit;
    public float invincibilityTime = 0.5f;

    public float hitMountainShakeIntensity = 0.5f;

    public AudioSource hitSound;

    private PlayerHealth health;
    private bool invincible = false;

	// Use this for initialization
	void Start () {
        health = GetComponent<PlayerHealth>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<PlayerObstacle>() && !invincible)
        {
            health.TakeDamage(damageTakenOnHit);
            StartCoroutine(BecomeInvincible(invincibilityTime));

            // Shake camera
            Vector3 shakeDirection = Camera.main.transform.InverseTransformDirection(transform.position - collider.transform.position);
            shakeDirection.z = 0;
            shakeDirection.Normalize();
            Camera.main.GetComponent<CameraShaker>().ShakeInDirectionWithIntensity(shakeDirection, hitMountainShakeIntensity);

            hitSound.Play();
        }
    }

    public IEnumerator BecomeInvincible(float time)
    {
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
    }
}
