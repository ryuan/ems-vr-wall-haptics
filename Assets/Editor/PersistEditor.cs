using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PersistBehaviour), true)]
public class PersistEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PersistBehaviour myScript = (PersistBehaviour)target;

        if (GUILayout.Button("Save Values"))
        {
            myScript.SaveValues();
        }

        if (GUILayout.Button("(Re-)Load Values"))
        {
            myScript.LoadValues();
        }
    }
}