using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]

public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.red;

        if (fov.offsetTransform == null)
            fov.offsetTransform = fov.transform;

        Handles.DrawWireArc(fov.offsetTransform.position, Vector3.up, Vector3.forward, 360, fov.viewRaduis);

        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

        Handles.DrawLine(fov.offsetTransform.position, fov.offsetTransform.position + viewAngleA * fov.viewRaduis);
        Handles.DrawLine(fov.offsetTransform.position, fov.offsetTransform.position + viewAngleB * fov.viewRaduis);

        foreach (Transform visibleTarget in fov.visibleTargets)
            Handles.DrawLine(fov.offsetTransform.position, visibleTarget.position);
    }
}
