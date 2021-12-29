using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CrystalCluster)), CanEditMultipleObjects]
public class CrystalClusterEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        CrystalCluster myTarget = (CrystalCluster)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Regenerate Cluster"))
        {
            myTarget.RegenerateCluster();
        }

        if(GUILayout.Button("Save As Asset"))
        {
            myTarget.SaveAsAsset();
        }
      
    }
}