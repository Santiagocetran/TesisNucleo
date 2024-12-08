#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class CanvasScalerUpdater : EditorWindow
{
    private Vector2 referenceResolution = new Vector2(1920, 1080);
    private float matchWidthOrHeight = 0.5f;

    [MenuItem("Tools/Update All Canvas Scalers")]
    public static void ShowWindow()
    {
        GetWindow<CanvasScalerUpdater>("Canvas Scaler Updater");
    }

    private void OnGUI()
    {
        GUILayout.Label("Canvas Scaler Settings", EditorStyles.boldLabel);

        referenceResolution = EditorGUILayout.Vector2Field("Reference Resolution", referenceResolution);
        matchWidthOrHeight = EditorGUILayout.Slider("Match Width/Height", matchWidthOrHeight, 0f, 1f);

        if (GUILayout.Button("Update All Canvas Scalers"))
        {
            UpdateAllCanvasScalers();
        }
    }

    private void UpdateAllCanvasScalers()
    {
        CanvasScaler[] scalers = FindObjectsOfType<CanvasScaler>();
        Undo.RecordObjects(scalers, "Update Canvas Scalers");

        foreach (CanvasScaler scaler in scalers)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = matchWidthOrHeight;
            EditorUtility.SetDirty(scaler);
        }

        Debug.Log($"Updated {scalers.Length} Canvas Scalers");
    }
}
#endif