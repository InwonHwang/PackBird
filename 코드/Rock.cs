using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {

    internal enum eState { falling, unknown }

    internal eState state { get; private set; }

    void OnEnable()
    {
        state = eState.falling;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.name.Contains("player"))
            state = eState.unknown;
        StartCoroutine(delaySetActive(false, 2));
    }

    IEnumerator delaySetActive(bool flag, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        gameObject.SetActive(flag);
    }
}
