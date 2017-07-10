using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {
    
	void Start () {
        transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(() => {
            Application.Quit();
            Game.score = 0;
        });
        
        transform.Find("ReplayButton").GetComponent<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("Game");
            Game.score = 0;
        });
    }
}
