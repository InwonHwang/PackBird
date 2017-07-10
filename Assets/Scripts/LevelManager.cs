using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;

public class LevelManager : MonoBehaviour {

    internal Graph graph { get; set; }
    internal Player player { get; set; }

    Dictionary<string, ObjectPool> _pool;
    
    Coroutine _throwRock;

    void Awake()
    {
        _pool = new Dictionary<string, ObjectPool>();

        createPool(Resources.Load<GameObject>("prefabs/box/box_base_0"));
        createPool(Resources.Load<GameObject>("prefabs/box/box_base_1"));
        createPool(Resources.Load<GameObject>("prefabs/box/box_worm_0"));
        createPool(Resources.Load<GameObject>("prefabs/box/box_worm_1"));
        createPool(Resources.Load<GameObject>("prefabs/normal/nest"));
        createPool(Resources.Load<GameObject>("prefabs/normal/falling_rock"));
        createPool(Resources.Load<GameObject>("prefabs/normal/cat"));
    }
    internal bool LoadLevel(string fileName)
    {
        if (_throwRock != null) StopCoroutine(_throwRock);

        graph.Clear();
        TextAsset ta = Resources.Load<TextAsset>("Level/" + fileName);

        if (!ta) return false;

        JSONArray jsonArr = JSON.Parse(ta.text).AsArray;

        foreach(var pool in _pool)
        {
            pool.Value.Clear();
        }

        for (int i = 0; i < jsonArr.Count; ++i)
        {
            _pool[jsonArr[i][0]["name"]].count = jsonArr[i].Count;

            var objs = _pool[jsonArr[i][0]["name"]].GetObjects(transform);
            for (int j = 0; j < objs.Count; ++j)
            {
                JSONObject jsonObj = jsonArr[i][j].AsObject;
                objs[j].GetComponent<Serializable>().Deserialize(jsonObj);

                if (objs[j].GetComponent<Box>())
                {
                    graph.AddBox(objs[j]);
                }
                else if (objs[j].GetComponent<Cat>())
                {
                    objs[j].GetComponent<Cat>().graph = graph;
                    objs[j].GetComponent<Cat>().player = player;
                }
                else
                {
                    graph.Check(objs[j]);
                }
            }
        }

        graph.BuildGraph();
        _throwRock = StartCoroutine(throwRock(3, 3));

        graph.cur = graph.start;
        player.Clear();
        player.transform.position = graph.start.position;

        return true;
    }

    public void SaveLevel(string fileName)
    {
        int childCount = transform.childCount;

        JSONArray rootArr = new JSONArray();
        JSONObject jsonObj;

        for(int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);

            if (child.childCount == 0) continue;

            JSONArray jsonArr = new JSONArray();
            for (int j = 0; j < child.childCount; ++j)
            {
                Serializable serializable = child.GetChild(j).GetComponent<Serializable>();
                jsonObj = serializable.Serialize();
                jsonArr.Add(jsonObj);
            }

            rootArr.Add(jsonArr);
        }

        File.WriteAllText(Application.dataPath + "/Resources/Level/" + fileName, rootArr.ToString(1));
    }

    ObjectPool createPool(GameObject prefab)
    {
        var pool = new ObjectPool();
        pool.prefab = prefab;
        _pool.Add(pool.prefab.name, pool);

        return pool;
    }

    IEnumerator throwRock(float repeatTime, int count)
    {
        _pool["falling_rock"].count = count * 2;
        List<GameObject> rocks = _pool["falling_rock"].GetObjects(transform);

        foreach (var rock in rocks)        
            rock.SetActive(false);
        

        while (true)
        {
            foreach(var rock in rocks)
            {
                int x = (int)Random.Range(graph.minX, graph.maxX + 1);
                int z = (int)Random.Range(graph.minZ, graph.maxZ + 1);
                int y = (int)graph.GetBoxHeight(x, z);
                rock.SetActive(true);
                rock.transform.position = new Vector3(x, y + 20, z);
                rock.GetComponent<Rigidbody>().velocity = Vector3.zero;

                yield return new WaitForSeconds(repeatTime);
            }
        }
    }
}
