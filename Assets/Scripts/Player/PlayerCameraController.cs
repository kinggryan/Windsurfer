using UnityEngine;
using System.Collections;

public class PlayerCameraController : MonoBehaviour {

    private enum CameraState { StartingLevel, PlayingLevel, EndingLevel };
    public AnimationCurve startLevelCameraZoomCurve;
    public AnimationCurve endLevelCameraZoomCurve;

    private CameraState state;
    private float animationTime;
    private Vector3 startingLocalPosition;

	// Use this for initialization
	void Start () {
        state = CameraState.StartingLevel;
        animationTime = 0;
        startingLocalPosition = transform.localPosition;
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
                    transform.localPosition = transform.localPosition;
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
                    Object.FindObjectOfType<LevelManager>().EndLevelAnimationComplete();
                }
                else
                {
                    transform.localPosition = startingLocalPosition + -1 * Vector3.forward * endLevelCameraZoomCurve.Evaluate(animationTime);
                }
                break;
        }
	}

    public void LevelComplete()
    {
        state = CameraState.EndingLevel;
        animationTime = 0;
    }
}
