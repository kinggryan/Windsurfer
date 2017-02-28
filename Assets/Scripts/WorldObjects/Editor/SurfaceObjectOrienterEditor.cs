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
        if (GUILayout.Button("Reset Rotation", newParams))
        {
            var rotation = Quaternion.LookRotation(Vector3.Cross(orienter.transform.position.normalized, Vector3.down), orienter.transform.position.normalized);
            orienter.transform.rotation = rotation;
        }
        if (GUILayout.Button("Reset Distance", newParams))
        {
            orienter.transform.position = orienter.groundDistance * orienter.transform.position.normalized;
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

		Quaternion axisRotation = Handles.Disc(orienter.transform.rotation, orienter.transform.position, orienter.transform.position.normalized, 25f, true, 1f);
		if(axisRotation != orienter.transform.rotation) {
			Undo.RecordObject (orienter.transform, "Orienter Rotation");
			orienter.transform.rotation = axisRotation;
		}

        // Draw the spherical position handles
        Quaternion zPositionQuaternion = Handles.Disc(orienter.transform.rotation, Vector3.zero, orienter.transform.forward, orienter.groundDistance + 10f, true, 1f);
        if (zPositionQuaternion != orienter.transform.rotation)
        {
            Undo.RecordObject(orienter.transform, "Orienter Rotation");
            Quaternion inverseOfOriginalRotation = Quaternion.Inverse(orienter.transform.rotation);
            orienter.transform.position = inverseOfOriginalRotation * orienter.transform.position;
            orienter.transform.position = zPositionQuaternion * orienter.transform.position;
            orienter.transform.rotation = zPositionQuaternion;
        }

        // If Not a hex object, do this for the local x axis. Otherwise, do it for the two hex angles
        if (!orienter.hexObject)
        {
            Quaternion xPositionQuaternion = Handles.Disc(orienter.transform.rotation, Vector3.zero, orienter.transform.right, orienter.groundDistance + 10f, true, 1f);
            if (xPositionQuaternion != orienter.transform.rotation)
            {
                Undo.RecordObject(orienter.transform, "Orienter Rotation");
                Quaternion inverseOfOriginalRotation = Quaternion.Inverse(orienter.transform.rotation);
                orienter.transform.position = inverseOfOriginalRotation * orienter.transform.position;
                orienter.transform.position = xPositionQuaternion * orienter.transform.position;
                orienter.transform.rotation = xPositionQuaternion;
            }
        }
        else
        {
            // Do a hex rotation
            Vector3 firstHexAngle = Quaternion.AngleAxis(60f, orienter.transform.up) * orienter.transform.forward;
            Quaternion firstHexRotation = Handles.Disc(orienter.transform.rotation, Vector3.zero, firstHexAngle, orienter.groundDistance + 10f, true, 1f);
            if (firstHexRotation != orienter.transform.rotation)
            {
                Undo.RecordObject(orienter.transform, "Orienter Rotation");
                Quaternion inverseOfOriginalRotation = Quaternion.Inverse(orienter.transform.rotation);
                orienter.transform.position = inverseOfOriginalRotation * orienter.transform.position;
                orienter.transform.position = firstHexRotation * orienter.transform.position;
                orienter.transform.rotation = firstHexRotation;
            }

            // Do other hex rotation
            Vector3 secondHexAngle = Quaternion.AngleAxis(120f, orienter.transform.up) * orienter.transform.forward;
            Quaternion secondHexRotation = Handles.Disc(orienter.transform.rotation, Vector3.zero, secondHexAngle, orienter.groundDistance + 10f, true, 1f);
            if (secondHexRotation != orienter.transform.rotation)
            {
                Undo.RecordObject(orienter.transform, "Orienter Rotation");
                Quaternion inverseOfOriginalRotation = Quaternion.Inverse(orienter.transform.rotation);
                orienter.transform.position = inverseOfOriginalRotation * orienter.transform.position;
                orienter.transform.position = secondHexRotation * orienter.transform.position;
                orienter.transform.rotation = secondHexRotation;
            }
        }
    }
}

