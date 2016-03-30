using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundsRemainingController : MonoBehaviour {

    public float percentGroundsNeeded = 1.0f;

    public UnityEngine.UI.Text victoryText;
    public UnityEngine.UI.Text groundsLeftText;
    public GameObject[] buttonsToEnableOnVictory;

    //  public Color allDesertSkyColor;
    //  public Color levelCompleteColor;
    public AnimationCurve atmosphereSizeCurve;
    public Gradient atmosphereGradient;
    public Material planetAtmosphereMat;

    public AudioSource growthAudioSource;

    private int totalGrounds;
    private int groundsNeeded;

    private float targetGrowthAudioSourceVolume;
    private float rainSoundDecayDelay = 0f;

    // Use this for initialization
    void Start() {
        targetGrowthAudioSourceVolume = growthAudioSource.volume;
        growthAudioSource.volume = 0;
    }

    // Update is called once per frame
    void Update() {
        if (rainSoundDecayDelay > 0)
            growthAudioSource.volume = Mathf.MoveTowards(growthAudioSource.volume, targetGrowthAudioSourceVolume, 5.0f * Time.deltaTime);
        else
            growthAudioSource.volume = Mathf.MoveTowards(growthAudioSource.volume, 0, 5.0f * Time.deltaTime);

        rainSoundDecayDelay -= Time.deltaTime;
    }

    public void GroundRainedOn() {
        rainSoundDecayDelay = 0.25f;
    }

    public void GroundRemoved()
    {
        if(--groundsNeeded <= 0)
        {
            victoryText.enabled = true;
            foreach(GameObject obj in buttonsToEnableOnVictory)
            {
                List<MonoBehaviour> components = new List<MonoBehaviour>();
                components.AddRange(obj.GetComponentsInChildren<UnityEngine.UI.Text>());
                components.AddRange(obj.GetComponents<UnityEngine.UI.Image>());
                components.AddRange(obj.GetComponents<UnityEngine.UI.Button>());

                foreach (MonoBehaviour mb in components)
                {
                    mb.enabled = true;
                }
            }
        }

        groundsLeftText.text = "" + groundsNeeded;
        float percentComplete = 1 - 1.0f * groundsNeeded / Mathf.FloorToInt(totalGrounds * percentGroundsNeeded);
        //  Camera.main.backgroundColor = percentComplete * levelCompleteColor + (1 - percentComplete) * allDesertSkyColor;
        planetAtmosphereMat.SetColor("_AtmoColor", atmosphereGradient.Evaluate(percentComplete));
        planetAtmosphereMat.SetFloat("_Size", atmosphereSizeCurve.Evaluate(percentComplete));

        ScreenFlash sFlash = Object.FindObjectOfType<ScreenFlash>();
        if (sFlash)
            sFlash.ForestComplete();
    }

    public void InitGround()
    {
        totalGrounds++;
        groundsNeeded = Mathf.FloorToInt(totalGrounds * percentGroundsNeeded);
        groundsLeftText.text = "" + groundsNeeded;
        //    Camera.main.backgroundColor = allDesertSkyColor;
        planetAtmosphereMat.SetColor("_AtmoColor", atmosphereGradient.Evaluate(0));
        planetAtmosphereMat.SetFloat("_Size", atmosphereSizeCurve.Evaluate(0));
    }
}
