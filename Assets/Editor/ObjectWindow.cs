using UnityEngine;
using UnityEditor;
using System.Collections;

class ObjectWindow : EditorWindow
{
    float _offset = 10;
    float _buttonSize = 100;

    string _stageNum;
    Vector2 _scrollPos;

    LevelManager _levelManager;

    void Awake()
    {
        var levelMgr = GameObject.Find("LevelManager");
        
        if(!levelMgr)
        {
            levelMgr = new GameObject();
            levelMgr.name = "LevelManager";
            levelMgr.AddComponent<LevelManager>();
        }

        _levelManager = levelMgr.GetComponent<LevelManager>();
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        _stageNum = EditorGUILayout.TextField("Stage Number", _stageNum);

        if (GUILayout.Button("save"))
        {
            if (_stageNum != "")
                _levelManager.SaveLevel(_stageNum + ".json");
        }

        if (GUILayout.Button("clear"))
        {
            if(_levelManager) DestroyImmediate(_levelManager.gameObject);
        }

        float width = position.width - _offset * 3;
        float height = position.height - _offset * 3;
        
        Texture[] textures = Resources.LoadAll<Texture>("textures/editor/");

        _scrollPos = GUI.BeginScrollView(new Rect(_offset, _offset + 100, width, height - 100), _scrollPos, new Rect(_offset, _offset, width, height), false, true);
        float startX = 0;
        float startY = 0;

        int x = 0;
        int y = 0;
        foreach(var texture in textures)
        {
            startX = _offset + _buttonSize * x;
            startY = _offset + _buttonSize * y;

            if (GUI.Button(new Rect(startX, startY, _buttonSize, _buttonSize), texture))
            {
                var parent = getParent(texture.name);
                var path = texture.name.Contains("box") ? "prefabs/box/": "prefabs/normal/";
                var prefab = Resources.Load<GameObject>(path + texture.name);
                var newObj = GameObject.Instantiate<GameObject>(prefab);
                var activeObj = Selection.activeGameObject;

                if (activeObj)
                {
                    newObj.transform.position = activeObj.transform.position +  new Vector3(0,1,0);
                }
                newObj.name = texture.name;                
                newObj.transform.SetParent(parent);
            }

            if (startX + _buttonSize * 2 > width)
            {
                x = 0;
                y++;
            }
            else x++;
        }
        GUI.EndScrollView();
    }

    LevelManager getLevelManager()
    {
        if (!_levelManager)
        {
            var levelMgr = GameObject.Find("LevelManager");

            if (!levelMgr)
            {
                levelMgr = new GameObject();
                levelMgr.name = "LevelManager";
            }
            
            _levelManager = levelMgr.AddComponent<LevelManager>();
        }

        return _levelManager;
    }

    Transform getParent(string prefabName)
    {
        var parent = _levelManager.transform.Find(prefabName);
        if (!parent)
        {
            var obj = new GameObject();
            parent = obj.transform;
            parent.name = prefabName;
            parent.transform.SetParent(_levelManager.transform);
        }

        return parent;
    }
}