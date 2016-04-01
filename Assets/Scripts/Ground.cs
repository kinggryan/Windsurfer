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

    public new Renderer renderer;

    private float rainAmount = 0f;
    private bool forest = false;
    

    // Public Methods

    public void RainedOnAtPoint(Vector3 rainPoint, float rainMultiplier)
    {
        RainedOnAmount(Time.deltaTime / fullRainTime * rainMultiplier);
    }

    public void RainedOnBurst(float amountFull)
    {
        Debug.Log("Rained on burst " + amountFull);     
        RainedOnAmount(amountFull);
    }

    // Private Methods

    // Use this for initialization
    void Start () {
       // renderer = GetComponent<Renderer>();
       // renderer.material.SetFloat("_ForestAmount", rainAmount); // = rainAmount * forestFadeColor + (1 - rainAmount) * desertColor;

        transform.rotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.position.normalized), transform.position.normalized);
        transform.position = groundDistance*transform.position.normalized;

        Object.FindObjectOfType<GroundsRemainingController>().InitGround();
    }

    void RainedOnAmount(float amountRainedOn)
    {
        if (!forest)
        {
            Object.FindObjectOfType<GroundsRemainingController>().GroundRainedOn();
            rainAmount += amountRainedOn;
            if (rainAmount >= 1.0)
            {
                rainAmount = 1;
                forest = true;
                renderer.material.SetFloat("_ForestAmount", rainAmount);

                Object.FindObjectOfType<GroundsRemainingController>().GroundRemoved();
                if (growthAnimator)
                    growthAnimator.Grow();

                Object.FindObjectOfType<PlayerMouseController>().ForestCreated();
            }
            else
            {
                renderer.material.SetFloat("_ForestAmount", rainAmount);
            }
        }
    }
}
