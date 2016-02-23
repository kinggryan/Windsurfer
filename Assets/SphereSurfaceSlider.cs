using UnityEngine;
using System.Collections;

public class SphereSurfaceSlider : MonoBehaviour {

    public Vector3 sphericalVelocity;   //!< The spherical velocity should always be a vector perpendicular to the spherical loction of the object, for now.

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(Vector3.zero, sphericalVelocity, sphericalVelocity.magnitude * Time.deltaTime);
	}

    // Hits the object with a force vector in world space.
    void HitWithLinearForce(Vector3 force)
    {
        // Convert the rotational velocity to worldspace TODO: Support tilted movement in linear movement speed.
        Vector3 linearMovementDirection = Vector3.Cross(-1 * transform.position.normalized, sphericalVelocity).normalized;
        float linearMovementSpeed = 2 * Mathf.PI * transform.position.magnitude / (sphericalVelocity.magnitude * Mathf.Deg2Rad);

        // Get the new linear force
        Vector3 newLinearMovement = force + linearMovementSpeed * linearMovementDirection;

        SetSphericalVelocityWithLinearVelocity(newLinearMovement);
    }

    public void SetSphericalVelocityWithLinearVelocity(Vector3 linearVelocity)
    {
        // Calculate this force in spherical space
        Vector3 newRotationAxis = Vector3.Cross(linearVelocity.normalized, -1 * transform.position.normalized);
        float sphericalSpeed = linearVelocity.magnitude * Mathf.Rad2Deg / (2 * Mathf.PI * transform.position.magnitude);
        sphericalVelocity = sphericalSpeed * newRotationAxis.normalized;
    }

    public void HitByStormWithSphericalVelocity(Vector3 stormSphericalVelocity)
    {
        HitWithLinearForce(Vector3.Cross(transform.position.normalized, stormSphericalVelocity.normalized));
        StartCoroutine(FreezeScreen(0.05f));
    }

    public void HitByStormAtPosition(Vector3 stormPosition)
    {
        float storedMagnitude = sphericalVelocity.magnitude;
        float newMagnitude = 2 * Mathf.PI * transform.position.magnitude / (sphericalVelocity.magnitude * Mathf.Deg2Rad);
        HitWithLinearForce(newMagnitude * (transform.position - stormPosition).normalized);
        sphericalVelocity = storedMagnitude * sphericalVelocity.normalized;
    }

    public IEnumerator FreezeScreen(float time)
    {
        float slowedTimescale = 0.01f;
        Time.timeScale = slowedTimescale;
        yield return new WaitForSeconds(time*slowedTimescale);
        Time.timeScale = 1.0f;
    }
}
