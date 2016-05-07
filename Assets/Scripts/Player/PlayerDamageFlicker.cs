using UnityEngine;
using System.Collections;

public class PlayerDamageFlicker : MonoBehaviour {

    public Gradient flickerGradient;
    public AnimationCurve flickerDurationToDamageCurve;
    public AnimationCurve flickerFrequencyToDamageCurve;
    public Renderer playerRenderer;
    public Color defaultPlayerColor;
    public ParticleSystem damagedTrailRenderer;

    public float deathDuration = 4f;

    private bool damaged = false;
    private float damagePercent = 0f;
    private float damageAnimationPercent = 0f;
    private float currentFlickerDuration = 0f;
    private float currentFlickerFrequency = 0f;

	// Use this for initialization
	void Start () {
       // defaultPlayerColor = playerRenderer.material.color;
	}
	
	// Update is called once per frame
	void Update () {
	    if(damaged)
        {
            Color flickerOverlayColor = flickerGradient.Evaluate(damageAnimationPercent);
            float flickerOverlayPercent = flickerOverlayColor.a;
            flickerOverlayColor.a = 1;
            Color flickerColor = Color.Lerp(defaultPlayerColor,flickerOverlayColor,flickerOverlayPercent);
            playerRenderer.material.color = flickerColor;

            damageAnimationPercent += Time.deltaTime / currentFlickerDuration;
            damagePercent += Time.deltaTime / deathDuration;
        }
	}

    IEnumerator Flicker()
    {
        damageAnimationPercent = 0f;
        currentFlickerDuration = flickerDurationToDamageCurve.Evaluate(damagePercent);
        currentFlickerFrequency = flickerFrequencyToDamageCurve.Evaluate(damagePercent);

        yield return new WaitForSeconds(currentFlickerFrequency);
        if(damaged)
        {
            Debug.Log("Starting new flicker at time: " + Time.time);
            StartCoroutine(Flicker());
        }
    }

    public void TakeDamage()
    {
        if(!damaged)
        {
            damaged = true;
            damagePercent = 0f;
            damageAnimationPercent = 0f;
            StartCoroutine(Flicker());
            damagedTrailRenderer.Play();
        }
    }

    public void Heal()
    {
        damaged = false;
        playerRenderer.material.color = defaultPlayerColor;
        damagedTrailRenderer.Stop();
    }

    public void PlayerDied()
    {
        damaged = false;
        playerRenderer.material.color = defaultPlayerColor;
        damagedTrailRenderer.Stop();
    }
}
