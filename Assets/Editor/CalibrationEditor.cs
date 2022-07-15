using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Calibration), true)]
public class EmsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Calibration myScript = (Calibration)target;

        if (GUILayout.Button("Send Pulse"))
        {
            myScript.sendPulse();
        }
    }
}