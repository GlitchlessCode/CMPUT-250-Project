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

    // Start is called before the first frame update
    void Start()
    {
        PowerOff.SetActive(false);
        StartButton.onClick.AddListener(OnStartButton);
    }

    private void OnStartButton()
    {
        PowerOff.SetActive(true);
        PowerOff.GetComponent<Animator>().SetTrigger("PlayPowerOff");       
    }
}
