using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    private Transform _parent;
    private Vector3 _offset;

    private void Awake()
    {
        _parent = transform.parent;
        _offset = transform.localPosition;

        transform.SetParent(null);
    }

    private void LateUpdate()
    {
        // Establece la posición siempre arriba, para evitar que rote cuando salga volando
        if (_parent != null) transform.position = _parent.position + _offset;
    }
}