using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour {

    public Color desertColor;
    public Color forestFadeColor;
    public Color forestCompleteColor;

    public float fullRainTime;

    public float groundDistance = 51.5f;

    public UnityEngine.UI.Text victoryText;

    public PlantGrowthAnimator growthAnimator;

    private float rainAmount = 0f;
    private bool forest = false;
    new private Renderer renderer;

    // Public Methods

    public void RainedOnAtPoint(Vector3 rainPoint, float rainMultiplier)
    {
        RainedOnAmount(Time.deltaTime / fullRainTime * rainMultiplier);
    }

    public void RainedOnBurst(float amountFull)
    {
        RainedOnAmount(amountFull * fullRainTime);
    }

    // Private Methods

    // Use this for initialization
    void Start () {
        renderer = GetComponent<Renderer>();
        renderer.material.color = rainAmount * forestFadeColor + (1 - rainAmount) * desertColor;

        transform.rotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.position.normalized), transform.position.normalized);
        transform.position = groundDistance*transform.position.normalized;

        Object.FindObjectOfType<GroundsRemainingController>().InitGround();
    }

    void RainedOnAmount(float amountRainedOn)
    {
        if (!forest)
        {
            rainAmount += amountRainedOn;
            if (rainAmount >= 1.0)
            {
                forest = true;
                renderer.material.color = forestCompleteColor;

                Object.FindObjectOfType<GroundsRemainingController>().GroundRemoved();
                if(growthAnimator)
                    growthAnimator.Grow();
            }
            else
            {
                renderer.material.color = rainAmount * forestFadeColor + (1 - rainAmount) * desertColor;
            }
        }
    }
}
