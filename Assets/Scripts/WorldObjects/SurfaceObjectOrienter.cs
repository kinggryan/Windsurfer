using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SurfaceObjectOrienter))]
public class SurfaceObjectOrienterEditor : Editor
{

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
        orienter.groundDistance = EditorGUILayout.FloatField("Radius of World", orienter.groundDistance, newParams);
        orienter.surfaceSize = EditorGUILayout.FloatField("Size of Object on Surface", orienter.surfaceSize, newParams);

        orienter.hexObject = GUILayout.Toggle(orienter.hexObject, "Hex Object", newParams);
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
        float snapDegrees = 360 * orienter.surfaceSize / (2 * Mathf.PI * orienter.groundDistance);

        Quaternion zPositionQuaternion = Handles.Disc(orienter.transform.rotation, Vector3.zero, orienter.transform.forward, orienter.groundDistance+10f, true, 1f);
        if (Quaternion.Angle(zPositionQuaternion,orienter.transform.rotation) > snapDegrees)
        {
            Vector3 rotationAxis = orienter.transform.forward;
            Quaternion tentativeRotation = orienter.transform.rotation * Quaternion.AngleAxis(snapDegrees, rotationAxis);
            if (Quaternion.Angle(zPositionQuaternion, tentativeRotation) > Quaternion.Angle(zPositionQuaternion, orienter.transform.rotation))
                orienter.transform.RotateAround(Vector3.zero, rotationAxis, -snapDegrees);
            else
                orienter.transform.RotateAround(Vector3.zero, rotationAxis, snapDegrees);
        }

        // If Not a hex object, do this for the local x axis. Otherwise, do it for the two hex angles
        if (!orienter.hexObject)
        {
            Quaternion xPositionQuaternion = Handles.Disc(orienter.transform.rotation, Vector3.zero, orienter.transform.right, orienter.groundDistance + 10f, true, 1f);
            if (Quaternion.Angle(xPositionQuaternion, orienter.transform.rotation) > snapDegrees)
            {
                Vector3 rotationAxis = orienter.transform.right;
                Quaternion tentativeRotation = orienter.transform.rotation * Quaternion.AngleAxis(snapDegrees, rotationAxis);
                if (Quaternion.Angle(xPositionQuaternion, tentativeRotation) > Quaternion.Angle(xPositionQuaternion, orienter.transform.rotation))
                    orienter.transform.RotateAround(Vector3.zero, rotationAxis, -snapDegrees);
                else
                    orienter.transform.RotateAround(Vector3.zero, rotationAxis, snapDegrees);
            }
        }
        else
        {
            // Do a hex rotation
            Vector3 firstHexAngle = Quaternion.AngleAxis(60f, orienter.transform.up) * orienter.transform.forward;
            Quaternion firstHexRotation = Handles.Disc(orienter.transform.rotation, Vector3.zero, firstHexAngle, orienter.groundDistance + 10f, true, 1f);
            if (Quaternion.Angle(firstHexRotation, orienter.transform.rotation) > snapDegrees)
            {
                Vector3 rotationAxis = firstHexAngle;
                Quaternion tentativeRotation = orienter.transform.rotation * Quaternion.AngleAxis(snapDegrees, rotationAxis);
                if (Quaternion.Angle(firstHexRotation, tentativeRotation) > Quaternion.Angle(firstHexRotation, orienter.transform.rotation))
                    orienter.transform.RotateAround(Vector3.zero, rotationAxis, -snapDegrees);
                else
                    orienter.transform.RotateAround(Vector3.zero, rotationAxis, snapDegrees);
            }

            // Do other hex rotation
            Vector3 secondHexAngle = Quaternion.AngleAxis(120f, orienter.transform.up) * orienter.transform.forward;
            Quaternion secondHexRotation = Handles.Disc(orienter.transform.rotation, Vector3.zero, secondHexAngle, orienter.groundDistance + 10f, true, 1f);
            if (Quaternion.Angle(secondHexRotation, orienter.transform.rotation) > snapDegrees)
            {
                Vector3 rotationAxis = secondHexAngle;
                Quaternion tentativeRotation = orienter.transform.rotation * Quaternion.AngleAxis(snapDegrees, rotationAxis);
                if (Quaternion.Angle(secondHexRotation, tentativeRotation) > Quaternion.Angle(secondHexRotation, orienter.transform.rotation))
                    orienter.transform.RotateAround(Vector3.zero, rotationAxis, -snapDegrees);
                else
                    orienter.transform.RotateAround(Vector3.zero, rotationAxis, snapDegrees);
            }
        }
    }
} 

public class SurfaceObjectOrienter : MonoBehaviour {

    public float surfaceSize = 15f;
    public float groundDistance = 75f;
    public bool hexObject;

    // Use this for initialization
    void Start () {
        transform.rotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.position.normalized), transform.position.normalized);
        transform.position = groundDistance * transform.position.normalized;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
