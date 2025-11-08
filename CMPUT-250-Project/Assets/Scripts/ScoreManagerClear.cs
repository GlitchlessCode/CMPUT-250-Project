using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManagerClear : MonoBehaviour
{
    void Start()
    {
        foreach (ScoreManager manager in FindObjectsOfType<ScoreManager>())
        {
            Destroy(manager.gameObject);
        }
        Destroy(gameObject);
    }
}
