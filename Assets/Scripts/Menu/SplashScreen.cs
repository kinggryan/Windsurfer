using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour {

	UnityEngine.UI.Image fadeInOutImage;
	public AnimationCurve fadeInOutCurve;
	public float fadeInDuration;
	public float fadeOutDuration;

	public string menuSceneName;

	public bool fadeOutEnabled;

	private float fadeTime = 0;
	private bool fadingOut = false;

	void Start() {
		fadeInOutImage = GetComponent<UnityEngine.UI.Image> ();
	}

	// Update is called once per frame
	void Update () {
		if (fadingOut) {
			fadeTime -= Time.deltaTime;
			if (fadeTime <= 0) {
				SceneManager.LoadScene (menuSceneName);
			} else {
				Color originalColor = fadeInOutImage.color;
				originalColor.a = 1 - fadeInOutCurve.Evaluate (fadeTime / fadeOutDuration);
				fadeInOutImage.color = originalColor;
			}
		} else if (fadeTime < fadeInDuration) {
			fadeTime = Mathf.Min (fadeTime + Time.deltaTime, fadeInDuration);
			Color originalColor = fadeInOutImage.color;
			originalColor.a = 1 - fadeInOutCurve.Evaluate (fadeTime / fadeInDuration);
			fadeInOutImage.color = originalColor;
		} else if (fadeOutEnabled && Input.GetMouseButtonDown(0)) {
			fadingOut = true;
			fadeTime = fadeOutDuration;
		}
	}
}
