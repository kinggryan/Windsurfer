using UnityEngine;
using System.Collections;

public delegate void LevelCompleteDelegate();

[RequireComponent(typeof(CameraShaker))]
public class PlayerCameraController : MonoBehaviour {

    private enum CameraState { StartingLevel, PlayingLevel, EndingLevel };
    public AnimationCurve startLevelCameraZoomCurve;
    public AnimationCurve endLevelCameraZoomCurve;

    private CameraState state;
    private float animationTime;
    private Vector3 startingLocalPosition;
    private LevelCompleteDelegate endLevelAnimationCompletionHandler;
    private CameraShaker cameraShaker;

	// Use this for initialization
	void Start () {
        state = CameraState.StartingLevel;
        animationTime = 0;
        startingLocalPosition = transform.localPosition;
        cameraShaker = GetComponent<CameraShaker>();
        cameraShaker.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	    switch(state)
        {
            case CameraState.StartingLevel:
                animationTime += Time.deltaTime;
                if (animationTime >= startLevelCameraZoomCurve.keys[startLevelCameraZoomCurve.keys.Length - 1].time)
                {
                    state = CameraState.PlayingLevel;
                    transform.localPosition = startingLocalPosition;

                    cameraShaker.enabled = true;
                    cameraShaker.UpdateOriginalRelativeCameraPosition(startingLocalPosition);
                }
                else
                {
                    transform.localPosition = startingLocalPosition + -1*Vector3.forward * startLevelCameraZoomCurve.Evaluate(animationTime);
                }
                break;
            case CameraState.EndingLevel:
                animationTime += Time.deltaTime;
                if (animationTime >= endLevelCameraZoomCurve.keys[startLevelCameraZoomCurve.keys.Length - 1].time)
                {
                    // Go to next level
                    endLevelAnimationCompletionHandler();
                }
//                else
//                {
                    transform.localPosition = startingLocalPosition + -1 * Vector3.forward * endLevelCameraZoomCurve.Evaluate(animationTime);
//                }
                break;
        }

		Debug.Log ("State " + state + " lp " + transform.localPosition);
	}

    public void LevelComplete(LevelCompleteDelegate completionHandler)
    {
        state = CameraState.EndingLevel;
        animationTime = 0;
        endLevelAnimationCompletionHandler = completionHandler;
		GetComponent<CameraShaker> ().StopShaking ();
        GetComponent<CameraShaker>().enabled = false;
    }
}
