using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {

    public float rainDrainedOnPlayerHit = 0.34f;
    public float minScale = 12;

    private float rainMeter = 1.0f;
    private float startingScale;
    private bool canHitPlayer = true;
    private float invulnerableTime = 0.5f;

	// Use this for initialization
	void Start () {
        startingScale = transform.localScale.x;
    }
	
    public bool HitPlayer()
    {
        if(!canHitPlayer)
        {
            return false;
        }

        rainMeter -= rainDrainedOnPlayerHit;
        if(rainMeter <= 0)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            transform.localScale = new Vector3((startingScale - minScale) * rainMeter + minScale, transform.localScale.y, (startingScale - minScale) * rainMeter + minScale);
        }

        canHitPlayer = false;
        StartCoroutine(BecomeHittable(invulnerableTime));

        return true;
    }

    public IEnumerator BecomeHittable(float time)
    {
        yield return new WaitForSeconds(time);
        canHitPlayer = true;
    }
}
