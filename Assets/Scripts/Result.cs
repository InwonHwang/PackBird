using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.Find("OKButton").GetComponent<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("Main");
            Game.score = 0;
        });

        transform.Find("Score").GetComponent<Text>().text = Game.score.ToString();

        transform.Find("ReplayButton").GetComponent<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("Game");
            Game.score = 0;
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
