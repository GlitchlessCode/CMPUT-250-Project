using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicOnAwake : MonoBehaviour
{
    [Tooltip("Key from your AudioLibrary (e.g., 'menu_theme')")]
    public string musicKey = "menu_theme";
    public float fadeSeconds = 1.0f;
    public bool loop = true;

    // Ensures we only start once even if multiple scenes use this script
    private static bool _started;

    void Awake()
    {
        if (_started) return;
        _started = true;

        AudioManager.Instance?.PlayMusic(musicKey, fadeSeconds, loop);
    }
}

