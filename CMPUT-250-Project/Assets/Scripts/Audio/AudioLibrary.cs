using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoundDef
{
    public string key;
    public AudioClip clip;
    [Range(0f, 1f)] public float defaultVolume = 1f;
    [Range(0.5f, 2f)] public float defaultPitch = 1f;
    [Range(0f, 1f)] public float spatialBlend = 0f; // 0 = 2D, 1 = 3D
}

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Audio/Library")]
public class AudioLibrary : ScriptableObject
{
    public List<SoundDef> sounds = new List<SoundDef>();

    private Dictionary<string, SoundDef> _map;

    public bool TryGet(string key, out SoundDef def)
    {
        if (_map == null)
        {
            _map = new Dictionary<string, SoundDef>();
            foreach (var s in sounds)
                if (!string.IsNullOrEmpty(s.key))
                    _map[s.key] = s;
        }
        return _map.TryGetValue(key, out def);
    }
}
