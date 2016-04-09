using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

    public int maxHealth;
    public int currentHealth;
    public UnityEngine.UI.Slider healthBar;
    public UnityEngine.UI.Text[] componentsToEnableOnHitGround;
    public ParticleSystem deadTrailParticleEffect;
    public GameObject deathParticleEffect;
    public float deathShakeIntensity;

    public AnimationCurve fallSpeedOnDeathCurve;
    private bool playerLost = false;
    private bool playerHitGround;
    private Vector3 fallingSphericalMovementVector;
    private float deadTime;

	// Use this for initialization
	void Start () {
        currentHealth = maxHealth;
        playerHitGround = false;
	}
	
	public void TakeDamage(int damageTaken)
    {
        if (playerLost)
            return;

        // Shake camera
        Vector3 shakeDirection = Vector3.up;
        shakeDirection.z = 0;
        shakeDirection.Normalize();
        Camera.main.GetComponent<CameraShaker>().ShakeInDirectionWithIntensity(shakeDirection, deathShakeIntensity);

        currentHealth -= damageTaken;
        if(currentHealth <= 0)
        {
            // Lose
            LevelDifficultyManager.LevelLost();
            playerLost = true;
            deadTrailParticleEffect.Play();
            GetComponent<PlayerDamageTaker>().enabled = false;
            fallingSphericalMovementVector = GetComponent<PlayerMouseController>().sphericalMovementVector;
            GetComponent<PlayerMouseController>().enabled = false;
            deadTime = 0;
        }

        healthBar.value = 1.0f * currentHealth / maxHealth;
    }

    void Update()
    {
        if(playerLost)
        {
          //  if (!playerHitGround)
         //  {
                transform.RotateAround(Vector3.zero, fallingSphericalMovementVector, fallingSphericalMovementVector.magnitude * Time.deltaTime);
                transform.position = transform.position.normalized * Mathf.Max(0, transform.position.magnitude - fallSpeedOnDeathCurve.Evaluate(deadTime) * Time.deltaTime);
          //  }
            deadTime += Time.deltaTime;
            if(Input.GetButtonDown("Boost"))
                Application.LoadLevel(Application.loadedLevel);
        }
    }

   void OnTriggerEnter(Collider trigger)
    {
        if(playerLost && trigger.GetComponent<Planet>())
        {
            GameObject.Instantiate(deathParticleEffect, transform.position, Quaternion.LookRotation(-1*transform.forward,transform.up));    
            playerHitGround = true;
            Camera.main.transform.parent = null;
            // Shake camera
            Vector3 shakeDirection = Vector3.up;
            shakeDirection.z = 0;
            shakeDirection.Normalize();
            Camera.main.GetComponent<CameraShaker>().ShakeInDirectionWithIntensity(shakeDirection, deathShakeIntensity);
            foreach (MonoBehaviour sc in componentsToEnableOnHitGround)
            {
                sc.enabled = true;
            }
        }
    }
}
