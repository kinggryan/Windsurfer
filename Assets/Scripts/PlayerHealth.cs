using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

    public int maxHealth;
    public int currentHealth;
    public UnityEngine.UI.Slider healthBar;

	// Use this for initialization
	void Start () {
        currentHealth = maxHealth;
	}
	
	public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;
        if(currentHealth <= 0)
        {
            // Lose
            Application.LoadLevel(Application.loadedLevel);
        }

        healthBar.value = 1.0f * currentHealth / maxHealth;
    }
}
