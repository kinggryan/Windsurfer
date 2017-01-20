using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexRefilledUIEffect : MonoBehaviour {

	public RectTransform rectTransform;
	public AnimationCurve fadeoutCurve;
	public AnimationCurve scaleCurve;

	private float animationTime = 0;
	private float originalRectTransformWidth;
	private float originalRectTransformHeight;

	// Use this for initialization
	void Start () {
		originalRectTransformWidth = rectTransform.rect.width;
		originalRectTransformHeight = rectTransform.rect.height;
	}
	
	// Update is called once per frame
	void Update () {
		animationTime += Time.deltaTime;
		if (animationTime > 1) {
			GameObject.Destroy (gameObject);
		} 
		else {
			rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, scaleCurve.Evaluate (animationTime)*originalRectTransformWidth);
			rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, scaleCurve.Evaluate (animationTime)*originalRectTransformWidth);
		}
	}
}
