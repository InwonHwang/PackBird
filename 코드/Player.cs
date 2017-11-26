using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    internal enum eState { idle, dead, moving }    
    internal eState state { get; private set; }
    internal int score { get { return _score; } }

    const int MAX_WORM = 5;

    Coroutine _move;
    Animator _animator;
    int _worm;
    int _score;

    void Awake()
    {
        _animator = transform.GetComponent<Animator>();
        _worm = 0;
        _score = 0;
    }

    void OnEnable()
    {
        state = eState.idle;
    }

    internal void Move(Vector3 cur, Vector3 next)
    {
        _move = StartCoroutine(move(cur, next));
    }

    internal void Clear()
    {
        _score = 0;
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

        if(_animator.enabled == false) _animator.enabled = true;

        Vector3 direction = next;
        direction.y = transform.position.y;
        transform.LookAt(direction);

        float y = next.y - cur.y;

        if (y > 0)
            _animator.Play("JumpUp", -1, 0);        
        else if (y < 0)        
            _animator.Play("JumpDown", -1, 0);        
        else
            _animator.Play("JumpFront", -1, 0);

        yield return new WaitForSeconds(1.0f);

        transform.position = new Vector3(Mathf.RoundToInt(next.x),
                                        Mathf.RoundToInt(next.y),
                                        Mathf.RoundToInt(next.z));
        state = eState.idle;
    }

    void OnTriggerEnter(Collider other)
    {
        var name = other.name;

        switch(name)
        {
            case "worm":
                if (_worm < MAX_WORM) _worm++;
                other.gameObject.SetActive(false);
                break;
            case "nest":
                _score += _worm;
                _worm = 0;
                break;
            case "falling_rock":
                break;
            default:
                break;
        }
    }
    

    void OnCollisionEnter(Collision collision)
    {
        var name = collision.gameObject.name;
        
        switch (name)
        {
            case "falling_rock":
                if (Rock.eState.falling == collision.gameObject.GetComponent<Rock>().state)
                    state = eState.dead;
                break;
            case "cat":
                state = eState.dead;
                break;
            default:
                break;
        }
    }
}
