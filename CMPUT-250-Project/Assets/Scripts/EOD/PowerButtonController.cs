using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerButtonController : MonoBehaviour
{
    [SerializeField] private string eodSceneName = "EOD";

    public void OnPowerPressed()
    {
        SceneManager.LoadScene(eodSceneName, LoadSceneMode.Single);
    }
}

