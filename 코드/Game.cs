using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {
    public static int score = 0;
    
    LevelManager _levelManager;
    Graph        _graph;
    Player       _player;
    Score        _score;
    int          _level;
    
    void Start() {
        _graph = new Graph();
        
        // player 생성
        var player = Instantiate<GameObject>(Resources.Load<GameObject>("prefabs/normal/bird"));
        player.name = "player";
        player.transform.SetParent(transform);
        _player = player.GetComponent<Player>();

        // 레벨 관리자 생성
        var levelManager = new GameObject();
        levelManager.name = "LevelManager";
        levelManager.transform.SetParent(transform);
        _levelManager = levelManager.AddComponent<LevelManager>();
        _levelManager.graph = _graph;
        _levelManager.player = _player;

        // ui
        var ui = GameObject.Find("UI");

        // button
        int index = 0;
        foreach (var buttonName in new string[] { "forward", "left", "right", "back" }) {

            int i = index;
            ui.transform.Find(buttonName).GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!_graph.cur || !_graph.cur[i] || _player.state == Player.eState.moving) return;

                _player.Move(_graph.cur.position, _graph.cur[i].position);
                _graph.cur = _graph.cur[i];
            });
            index++;
        }

        // score
        _score = ui.transform.Find("score").GetComponent<Score>();
        _score.player = _player;

        // end ui

        // camera
        var camera = GameObject.Find("Camera/Main Camera");
        camera.AddComponent<CameraFollow>().player = _player;

        _level = 1;

        // 레벌 1 로드
        loadLevel(_level);
    }
	
	// Update is called once per frame
	void Update () {
        if (_player.state == Player.eState.dead)
            SceneManager.LoadScene("Gameover");
        if (_score.score == _graph.score)
        {
            Game.score += _score.score;
            loadLevel(_level);
        }
            
	}

    void loadLevel(int level)
    {
        bool isLoaded = _levelManager.LoadLevel(level.ToString());
        if (isLoaded)
        {
            _level++;
        }
        else
        {
            SceneManager.LoadScene("Result");
            _level = 1;
        }
    }

    
}
