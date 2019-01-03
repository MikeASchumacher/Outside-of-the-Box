using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UpdateableData), true)]
public class UpdateableDataEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //reference to updateable data
        UpdateableData data = (UpdateableData)target;

        if (GUILayout.Button("Update")) {
            data.NotifyOfUpdate();
            EditorUtility.SetDirty(target);
        }
    }
}
