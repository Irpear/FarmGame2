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
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("PlayerPrefs reset for testing!");
        }
    }
}
#endif
