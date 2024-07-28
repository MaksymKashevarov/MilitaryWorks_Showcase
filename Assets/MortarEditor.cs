using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Mortar))]
public class MortarEditor : Editor
{
    private bool reloadSwitch = false;
    private bool launchSwitch = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Mortar mortar = (Mortar)target;

        GUILayout.Space(10);

        // Azimuth input
        GUILayout.Label("Azimuth", EditorStyles.boldLabel);
        mortar.azimuth = EditorGUILayout.FloatField(mortar.azimuth);

        // Angle input
        GUILayout.Label("Angle", EditorStyles.boldLabel);
        mortar.angle = EditorGUILayout.FloatField(mortar.angle);

        GUILayout.Space(10);

        // Reload switch
        reloadSwitch = EditorGUILayout.Toggle("Reload", reloadSwitch);
        if (reloadSwitch)
        {
            mortar.Reload();
            reloadSwitch = false;
        }

        // Launch switch
        mortar.launchShell = EditorGUILayout.Toggle("Launch", mortar.launchShell);
    }
}
