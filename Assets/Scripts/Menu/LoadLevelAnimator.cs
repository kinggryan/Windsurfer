using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevelAnimator : MonoBehaviour {

	public UnityEngine.UI.Image fadeInImage;
	public AnimationCurve loadLevelAnimationCurve;
	public float loadLevelAnimationDuration;

	private float loadLevelAnimationTime;
	private bool animating;
	private string sceneToLoad;

	void Start() {
		animating = false;
	}

	// Update is called once per frame
	void Update () {
		if (animating) {
			loadLevelAnimationTime += Time.deltaTime;
			Color color = fadeInImage.color;
			color.a = loadLevelAnimationCurve.Evaluate(loadLevelAnimationTime / loadLevelAnimationDuration);
			fadeInImage.color = color;
			if (loadLevelAnimationTime >= loadLevelAnimationDuration) {
				FinishLoadingLevel ();
			}
		}
	}

	public void LoadLevel(string sceneName) {
		animating = true;
		sceneToLoad = sceneName;
	}

	private void FinishLoadingLevel() {
		animating = false;
		SceneManager.LoadSceneAsync(sceneToLoad);
	}
}
