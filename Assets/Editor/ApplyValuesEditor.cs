using MuscleDeck;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ApplyValues), true)]
public class ApplyValuesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ApplyValues myScript = (ApplyValues)target;

        if (GUILayout.Button("Apply Values"))
        {
            myScript.ApplyValuesToHandlers();
        }
    }
}