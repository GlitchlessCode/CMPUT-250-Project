using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject PowerOff;

    [Header("Buttons")]
    public Button StartButton;

    [Header("Audio")]
    public Audio ShutdownAudio;

    [Header("Events")]
    public AudioGameEvent AudioBus;

    // Start is called before the first frame update
    void Start()
    {
        PowerOff.SetActive(false);
        StartButton.onClick.AddListener(OnStartButton);
    }

    private void OnStartButton()
    {
        StartCoroutine(Shutdown());
    }

    private IEnumerator Shutdown()
    {
        yield return new WaitForSeconds(0.15f);
        if (ShutdownAudio.clip != null)
        {
            AudioBus?.Emit(ShutdownAudio);
        }
        PowerOff.SetActive(true);
        PowerOff.GetComponent<Animator>().SetTrigger("PlayPowerOff");
    }
}
