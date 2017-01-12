using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MobileInput {

	static float swipePixPerSecond = 10;

	public static bool GetTouchDown()
	{
		if (Input.touchCount == 0)
			return false;

		return Input.touches [0].phase == TouchPhase.Began;
	}

	public static bool GetTouchUp()
	{
		if (Input.touchCount == 0)
			return false;

		return Input.touches [0].phase == TouchPhase.Ended;
	}

	public static bool GetSwipedLeft()
	{
		if (Input.touchCount == 0)
			return false;

		return Input.touches [0].deltaPosition.x / Input.touches [0].deltaTime < -swipePixPerSecond;
	}

	public static bool GetSwipedRight()
	{
		if (Input.touchCount == 0)
			return false;

		return Input.touches [0].deltaPosition.x / Input.touches [0].deltaTime > swipePixPerSecond;
	}

	public static bool GetTouched()
	{
		return Input.touchCount > 0;
	}
}
