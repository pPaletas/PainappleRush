using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    private Transform _parent;
    private Vector3 _offset;

    private void Start()
    {
        _parent = transform.parent;
        _offset = transform.localPosition;

        transform.SetParent(null);
    }

    private void Update()
    {
        // Establece la posición siempre arriba, para evitar que rote cuando salga volando
        transform.position = _parent.position + _offset;
    }
}