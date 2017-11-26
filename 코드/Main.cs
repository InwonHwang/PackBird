using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {

    private GameObject _uiSetting;

    void Awake()
    {
        _uiSetting = transform.Find("setting").gameObject;
    }

    void Start ()
    {
        transform.Find("main/button_start").GetComponent<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("Game");
        });

        transform.Find("main/button_setting").GetComponent<Button>().onClick.AddListener(() => {
            _uiSetting.SetActive(true);
        });

        transform.Find("setting/button_exit").GetComponent<Button>().onClick.AddListener(() => {
            Application.Quit();
        });

        transform.Find("setting/button_ok").GetComponent<Button>().onClick.AddListener(() => {
            _uiSetting.SetActive(false);
        });

        transform.Find("setting/button_music/button").GetComponent<Button>().onClick.AddListener(() => {
            GameObject maker = transform.Find("setting/button_music/checkmark").gameObject;
            AudioSource mainSound = transform.GetComponent<AudioSource>();

            if(maker.activeSelf)
            {
                maker.SetActive(false);
                mainSound.mute = true;
            }
            else
            {
                maker.SetActive(true);
                mainSound.mute = false;
            }
        });
        
    }
}
