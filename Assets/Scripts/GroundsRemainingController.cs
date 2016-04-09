using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundsRemainingController : MonoBehaviour {

    public float percentGroundsNeeded = 1.0f;

    public UnityEngine.UI.Text groundsLeftText;

    //  public Color allDesertSkyColor;
    //  public Color levelCompleteColor;
    public AnimationCurve atmosphereSizeCurve;
    public Gradient atmosphereGradient;
    public Material planetAtmosphereMat;

    public AudioSource growthAudioSource;
    public float growthAudioSourceMaxVolumeTime;
    public float growthAudioSourceSilenceTime;
    public float growthAudioSourceDecayTime;

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
            growthAudioSource.volume = Mathf.MoveTowards(growthAudioSource.volume, targetGrowthAudioSourceVolume, Time.deltaTime / growthAudioSourceMaxVolumeTime);
        else
            growthAudioSource.volume = Mathf.MoveTowards(growthAudioSource.volume, 0, Time.deltaTime / growthAudioSourceSilenceTime);

        rainSoundDecayDelay -= Time.deltaTime;
    }

    public void GroundRainedOn() {
        rainSoundDecayDelay = growthAudioSourceDecayTime;
    }

    public void GroundRemoved()
    {
        if(--groundsNeeded <= 0)
        {
            GetComponent<LevelManager>().LevelComplete();
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
