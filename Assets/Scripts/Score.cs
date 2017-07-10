using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

    internal Player player { get; set; }
    internal int score { get { return int.Parse(_text.text); } }

    UnityEngine.UI.Text _text;

    void Awake()
    {
        _text = transform.Find("text").GetComponent<UnityEngine.UI.Text>();
    }

	void Update ()
    {
        if (!player) return;

        _text.text = player.score.ToString();
	}
}
