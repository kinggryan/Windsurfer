﻿using UnityEngine;
using System.Collections;

public class PlantGrowthAnimator : MonoBehaviour {

    public AnimationCurve xzScaleCurve;
    public AnimationCurve yScaleCurve;
    public float scaleMultiplier;

    private bool animating = false;
    private float animationTime = 0f;

	// Use this for initialization
	void Start () {
        transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
	    if(animating)
        {
            animationTime += Time.deltaTime;
            float xzScale = xzScaleCurve.Evaluate(animationTime);
            float yScale = yScaleCurve.Evaluate(animationTime);
            transform.localScale = scaleMultiplier*(new Vector3(xzScale, yScale, xzScale));
            if(animationTime > xzScaleCurve.keys[xzScaleCurve.keys.Length-1].time && animationTime > yScaleCurve.keys[yScaleCurve.keys.Length - 1].time)
            {
                animating = false;
                GameObject.Destroy(this);
            }
        }
	}

    public void Grow()
    {
        animating = true;
    }
}
