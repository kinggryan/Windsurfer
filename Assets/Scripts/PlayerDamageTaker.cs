using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerDamageTaker : MonoBehaviour {

    public int damageTakenOnHit;
    public float invincibilityTime = 0.5f;

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
        if (!invincible)
        {
            health.TakeDamage(damageTakenOnHit);
            BecomeInvincible(invincibilityTime);
        }
    }

    public IEnumerator BecomeInvincible(float time)
    {
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
    }
}
