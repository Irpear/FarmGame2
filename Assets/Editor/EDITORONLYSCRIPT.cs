#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ResetPlayerPrefsOnPlay
{
    static ResetPlayerPrefsOnPlay()
    {
        // Wordt aangeroepen wanneer Play in Editor start
        EditorApplication.playModeStateChanged += ResetPrefsOnPlay;
    }

    static void ResetPrefsOnPlay(PlayModeStateChange state)
    {
        // ExitingEditMode = vlak vóór Play, dus voordat er een Awake draait
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            PlayerPrefs.DeleteAll();
            SaveManager.DeleteSave();
            Debug.Log("PlayerPrefs + save.json reset for testing!");
        }
    }
}
#endif
