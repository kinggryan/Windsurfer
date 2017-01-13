using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour {

	public float scrollHeight;
	public float autoscrollStartDelay;
	public float autoscrollSpeed;
	public bool selected;

	private float scrollOffset;
	private float autoScrollStartTime;

	// Update is called once per frame
	void Update () {
		if (selected) {
			autoScrollStartTime += Time.deltaTime;
			if (autoScrollStartTime > autoscrollStartDelay) {
				float newOffset = Mathf.Min(scrollHeight,scrollOffset + autoscrollSpeed * Time.deltaTime);
				transform.position += (newOffset - scrollOffset)*Vector3.up;
				scrollOffset = newOffset;
			}
		}
	}

	public void Reset() {
		autoScrollStartTime = 0;
		transform.position -= scrollOffset*Vector3.up;
		scrollOffset = 0;
	}
}
