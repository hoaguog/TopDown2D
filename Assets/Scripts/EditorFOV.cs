using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class EditorFOV : Editor
{

    void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;

        Handles.DrawWireArc(fow.transform.position, Vector3.back, Vector3.right, 360, fow.viewRadius);


        Vector2 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector2 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + (Vector3)viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + (Vector3)viewAngleB * fow.viewRadius);

        Handles.color = Color.red;

        //foreach (Transform visibleTarget in fow.visibleTargets)
        //{
        //    Handles.DrawLine(fow.transform.position, visibleTarget.position);
        //}
    }


}
