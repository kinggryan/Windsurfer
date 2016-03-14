using UnityEngine;
using System.Collections;

public class ScaleAnimator : MonoBehaviour {

    private bool animatingScale = true;
    private Vector3 targetLocalScale;
    private float scalingAnimationSpeed = 25f;
    private float rotationRate = 45f;
    private bool destroyOnScaleAnimationCompletion = false;

    // Use this for initialization
    void Start () {
        targetLocalScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {       
        if (animatingScale) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetLocalScale, scalingAnimationSpeed * Time.deltaTime);
            if (transform.localScale == targetLocalScale) {
                animatingScale = false;
                if (destroyOnScaleAnimationCompletion)
                {
                    GameObject.Destroy(gameObject);
                }
            }
        }
    }

    public void AnimateToScale(Vector3 newScale,bool destroyOnComplete)
    {
        targetLocalScale = newScale;
        destroyOnScaleAnimationCompletion = destroyOnComplete;
        animatingScale = true;
    }
}
