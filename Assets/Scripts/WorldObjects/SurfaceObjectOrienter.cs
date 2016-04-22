using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SurfaceObjectOrienter))]
public class SurfaceObjectOrienterEditor : Editor
{
    private bool hexObject;

    override public void OnInspectorGUI()
    {
        var orienter = target as SurfaceObjectOrienter;
        GUILayoutOption[] newParams = new GUILayoutOption[0];
        if(GUILayout.Button("Reset Rotation", newParams))
        {
            var rotation = Quaternion.LookRotation(Vector3.Cross(orienter.transform.position.normalized, Vector3.down), orienter.transform.position.normalized);
            orienter.transform.rotation = rotation;
        }
        if (GUILayout.Button("Reset Distance", newParams))
        {
            orienter.transform.position = orienter.groundDistance*orienter.transform.position.normalized;
        }
        orienter.groundDistance = EditorGUILayout.FloatField(orienter.groundDistance, newParams);

        hexObject = GUILayout.Toggle(hexObject, "Hex Object", newParams);
    }

    void OnSceneGUI()
    {
        var orienter = target as SurfaceObjectOrienter;
        var arrowRotation = Quaternion.LookRotation(orienter.transform.position.normalized);
        orienter.transform.position = Handles.Slider(orienter.transform.position, orienter.transform.position.normalized);
        orienter.groundDistance = orienter.transform.position.magnitude;
        Handles.DrawLine(Vector3.zero, orienter.transform.position);

        orienter.transform.rotation = Handles.Disc(orienter.transform.rotation, orienter.transform.position, orienter.transform.position.normalized, 25f, true, 1f);

        // Draw the spherical position handles
        Quaternion zPositionQuaternion = Handles.Disc(orienter.transform.rotation, Vector3.zero, orienter.transform.forward, orienter.groundDistance+10f, true, 1f);
        Quaternion inverseOfOriginalRotation = Quaternion.Inverse(orienter.transform.rotation);
        orienter.transform.position = inverseOfOriginalRotation * orienter.transform.position;
        orienter.transform.position = zPositionQuaternion * orienter.transform.position;
        orienter.transform.rotation = zPositionQuaternion;

        // If Not a hex object, do this for the local x axis. Otherwise, do it for the two hex angles
        if (!hexObject)
        {
            Quaternion xPositionQuaternion = Handles.Disc(orienter.transform.rotation, Vector3.zero, orienter.transform.right, orienter.groundDistance + 10f, true, 1f);
            inverseOfOriginalRotation = Quaternion.Inverse(orienter.transform.rotation);
            orienter.transform.position = inverseOfOriginalRotation * orienter.transform.position;
            orienter.transform.position = xPositionQuaternion * orienter.transform.position;
            orienter.transform.rotation = xPositionQuaternion;
        }
        else
        {
            // Do a hex rotation
            Vector3 firstHexAngle = Quaternion.AngleAxis(60f, orienter.transform.up) * orienter.transform.forward;
            Quaternion firstHexRotation = Handles.Disc(orienter.transform.rotation, Vector3.zero, firstHexAngle, orienter.groundDistance + 10f, true, 1f);
            inverseOfOriginalRotation = Quaternion.Inverse(orienter.transform.rotation);
            orienter.transform.position = inverseOfOriginalRotation * orienter.transform.position;
            orienter.transform.position = firstHexRotation * orienter.transform.position;
            orienter.transform.rotation = firstHexRotation;

            // Do other hex rotation
            Vector3 secondHexAngle = Quaternion.AngleAxis(120f, orienter.transform.up) * orienter.transform.forward;
            Quaternion secondHexRotation = Handles.Disc(orienter.transform.rotation, Vector3.zero, secondHexAngle, orienter.groundDistance + 10f, true, 1f);
            inverseOfOriginalRotation = Quaternion.Inverse(orienter.transform.rotation);
            orienter.transform.position = inverseOfOriginalRotation * orienter.transform.position;
            orienter.transform.position = secondHexRotation * orienter.transform.position;
            orienter.transform.rotation = secondHexRotation;
        }
    }
} 

public class SurfaceObjectOrienter : MonoBehaviour {

    public float groundDistance = 75f;

	// Use this for initialization
	void Start () {
        transform.rotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.position.normalized), transform.position.normalized);
        transform.position = groundDistance * transform.position.normalized;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
