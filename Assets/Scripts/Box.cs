using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class Box : Serializable {

    internal enum eDirection{ forward, left, right, back, unknown };

    internal bool isTraversable { get; set; }
    internal bool isStart { get; set; }
    internal Vector3 position { get { return transform.position + _offset; } }
    internal Box this[int index]
    {
        get
        {
            if (index < 0 || index >= _neighbours.Length)
                return null;

            return _neighbours[index];
        }
    }

    Box[]       _neighbours;
    float       _boundary;
    Vector3     _offset;
    GameObject  _worm;

    void Awake() {
        _neighbours = new Box[4] { null, null, null, null };        
        _boundary = Mathf.Sqrt(2);
        var wormTransform = transform.Find("worm");
        if (wormTransform) _worm = wormTransform.gameObject;


        _offset = new Vector3(0, 1, 0);
        isTraversable = false;
        isStart = false;

        Vector3 bounds = GetComponent<Renderer>().bounds.extents;

        transform.localScale /= (bounds.x * 2.0f);
    }

    void OnEnable()
    {
        if (_worm) _worm.SetActive(true);    
    }

    internal void AddNeighbour(Box box)
    {
        eDirection eDirection = geteDirection(box.position);
        switch (eDirection)
        {
            case eDirection.forward:
                _neighbours[0] = box;
                break;
            case eDirection.left:
                _neighbours[1] = box;
                break;
            case eDirection.right:
                _neighbours[2] = box;
                break;
            case eDirection.back:
                _neighbours[3] = box;
                break;
        }
    }

    internal bool IsNeighbour(Box box)
    {
        if (!box.isTraversable || box.gameObject == gameObject) return false;

        Vector3 vec = transform.position - box.transform.position;
        float dist = Vector3.Magnitude(vec);

        if (dist <= _boundary)
        {
            if (dist == _boundary && vec.y == 0)
                return false;

            return true;
        }

        return false;
    }

    internal override JSONObject Serialize()
    {
        JSONObject rootObj = base.Serialize();

        return rootObj;
    }

    internal override void Deserialize(JSONObject jsonObj)
    {
        base.Deserialize(jsonObj);
        
        _neighbours[0] = null;
        _neighbours[1] = null;
        _neighbours[2] = null;
        _neighbours[3] = null;
    }

    eDirection geteDirection(Vector3 pos)
    {
        Vector3 r = position - pos;
        if (r.z == -1) return eDirection.forward;
        if (r.z == 1) return eDirection.back;
        if (r.x == 1) return eDirection.left;
        if (r.x == -1) return eDirection.right;

        return eDirection.unknown;
    }
}
