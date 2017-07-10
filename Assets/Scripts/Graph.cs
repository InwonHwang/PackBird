using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class Graph
{
    class Node
    {
        public float heuristic;
        public float goal;

        public Box value;
        public Node parent;
    }

    internal Box cur { get; set; }
    internal Box start { get; private set; }
    internal int score { get; private set; }
    internal int minX { get; private set; }
    internal int maxX { get; private set; }
    internal int minZ { get; private set; }
    internal int maxZ { get; private set; }
    internal int minY { get; private set; }
    internal int maxY { get; private set; }

    List<Box> _boxes = new List<Box>();

    internal void BuildGraph()
    {
        linkNeighbour();
        SetScoreToClear();
    }
    
    internal void AddBox(GameObject boxObj)
    {
        _boxes.Add(boxObj.GetComponent<Box>());
        boxObj.GetComponent<Box>().isTraversable = true;

        int x = (int)boxObj.transform.position.x;
        int y = (int)boxObj.transform.position.y;
        int z = (int)boxObj.transform.position.z;

        minX = minX > x ? x : minX;
        maxX = maxX < x ? x : maxX;
        minY = minY > y ? y : minY;
        maxY = maxY < y ? y : maxY;
        minZ = minZ > z ? z : minZ;
        maxZ = maxZ < z ? z : maxZ;

        Check(boxObj);
    }

    internal void Check(GameObject obj)
    {
        Box box = Find(obj.transform.position + new Vector3(0, -1.0f, 0));
        
        if (!box) return;

        if (obj.name == "nest")
        {
            box.isStart = true;
            start = box;
        }            
        else
        {
            box.isTraversable = false;
            _boxes.Remove(box);
        }
    }

    internal float GetBoxHeight(float x, float z)
    {
        foreach (var box in _boxes)
        {
            if (box.transform.position.x == x ||
                box.transform.position.z == z)
                return box.transform.position.y;
        }

        return 0;
    }

    internal void Clear()
    {
        _boxes.Clear();
    }

    internal Box Find(Vector3 position)
    {
        foreach (var box in _boxes)
        {
            if (box.transform.position == position)            
                return box;
            
        }

        return null;
    }

    internal List<Vector3> GetPath(Vector3 start, Vector3 end)
    {
        PriorityQueue<Node> pq = new PriorityQueue<Node>((Node a, Node b) => {
            return ((a.goal + a.heuristic) > (b.goal + b.heuristic)) ? true : false;
        });
        List<Box> visit = new List<Box>();

        Node startNode = new Node { heuristic = Vector3.Distance(start, end),
            goal = 0,
            value = Find(start + new Vector3(0, -1.0f, 0)),
            parent = null
        };
        Node cur = null;
        Node target = null;

        pq.Push(startNode);
        visit.Add(startNode.value);
        
        while (!pq.Empty())
        {
            cur = pq.Top();
            pq.Pop();
            
            if (cur.value && cur.value.position == end) target = cur;

            for(int i = 0; i < 4; ++i)
            {
                if (cur.value == null) break;

                if (cur.value[i] && !visit.Contains(cur.value[i]) &&
                    cur.value[i].position.y == cur.value.position.y)
                {
                    Node child = new Node
                    {
                        heuristic = Vector3.Distance(cur.value[i].position, end),
                        goal = cur.goal + Vector3.Distance(cur.value[i].position, cur.value.position),
                        value = cur.value[i],
                        parent = cur
                    };

                    pq.Push(child);
                    visit.Add(child.value);
                }
            }
        }

        List<Vector3> path = new List<Vector3>();

        Node temp = target;
        
        while(temp != null)
        {
            path.Add(temp.value.position);
            temp = temp.parent;
        }
        
        return path;
    }

    void linkNeighbour()
    {
        Box b1, b2;
        for (int i = 0; i < _boxes.Count - 1; ++i)
        {
            b1 = _boxes[i];

            for (int j = i + 1; j < _boxes.Count; ++j)
            {
                b2 = _boxes[j];

                if (!b1.IsNeighbour(b2)) continue;

                b1.AddNeighbour(b2);
                b2.AddNeighbour(b1);
            }
        }
    }

    void SetScoreToClear()
    {
        score = 0;
        for (int i = 0; i < _boxes.Count; ++i)
        {
            if (_boxes[i].name.Contains("worm"))
                score++;
        }
    }
}
