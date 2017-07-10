using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cat : Serializable
{

    internal enum eState { idle, dead, moving }
    internal eState state { get; private set; }
    internal Player player { get; set; }
    internal Graph graph { get; set; }

    Coroutine _move;
    Coroutine _fsm;
    Animator _animator;
    float _cooltime;

    void Awake()
    {
        _animator = transform.GetComponent<Animator>();
        _cooltime = 1.0f;
    }

    void OnEnable()
    {
        state = eState.idle;
        _fsm = StartCoroutine(processFSM());
    }

    
    IEnumerator processFSM()
    {
        while(true)
        {
            Coroutine temp = null;
            if (player && player.transform.position.y - transform.position.y < 0.6f)
                temp = StartCoroutine(trackPlayer());
            else
                temp = StartCoroutine(idle());
            
            yield return new WaitForSeconds(_cooltime);

            if (temp != null) StopCoroutine(temp);
        }
    }

    IEnumerator trackPlayer()
    {
        if (graph == null) yield break;

        Vector3 start = new Vector3(Mathf.RoundToInt(player.transform.position.x),
                                    Mathf.RoundToInt(player.transform.position.y),
                                    Mathf.RoundToInt(player.transform.position.z));
        Vector3 end = new Vector3(Mathf.RoundToInt(transform.position.x),
                                  Mathf.RoundToInt(transform.position.y),
                                  Mathf.RoundToInt(transform.position.z));

        List<Vector3> path = graph.GetPath(start, end);

        for (int i = 1; i < path.Count; ++i)
        {
            Vector3 pos = path[i];
            yield return _move = StartCoroutine(move(transform.position, pos));
        }
    }

    IEnumerator idle()
    {
        if (graph == null) yield break;

        int randomInt = Mathf.RoundToInt(Random.Range(0, 4));

        Vector3 pos = new Vector3(Mathf.RoundToInt(transform.position.x),
                                    Mathf.RoundToInt(transform.position.y - 1.0f),
                                    Mathf.RoundToInt(transform.position.z));
        Box box = graph.Find(pos);

        if (box && box[randomInt] && box[randomInt].position.y - box.position.y < 0.1f)
        {
            Move(transform.position, box[randomInt].position);
        }
    }

    internal void Move(Vector3 cur, Vector3 next)
    {
        if (_move != null) StopCoroutine(_move);

        _move = StartCoroutine(move(cur, next));
    }

    internal void Clear()
    {
        if (_move != null)
        {
            StopCoroutine(_move);
            _animator.enabled = false;
            state = eState.idle;
        }
    }

    IEnumerator move(Vector3 cur, Vector3 next)
    {
        state = eState.moving;

        if (_animator.enabled == false) _animator.enabled = true;

        Vector3 direction = next;
        direction.y = transform.position.y;
        transform.LookAt(direction);

        _animator.Play("WalkFront", -1, 0);

        yield return new WaitForSeconds(1.0f);

        transform.position = next;
        state = eState.idle;
    }

    void OnCollisionEnter(Collision collision)
    {
        var name = collision.gameObject.name;

        switch (name)
        {
            case "falling_rock":

                if (Rock.eState.falling == collision.gameObject.GetComponent<Rock>().state)
                {
                    if (_fsm != null) StopCoroutine(_fsm);
                    gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }
    }
}
