using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    internal Player player { get; set; }

    Vector3 offset = new Vector3(10, 15, -10);

    void Update()
    {
        if (!player) return;

        Vector3 targetPos = player.transform.position + offset;
        Vector3 newPos = Vector3.Lerp(gameObject.transform.position, targetPos, Time.deltaTime);
        transform.position = newPos;
    }
}
