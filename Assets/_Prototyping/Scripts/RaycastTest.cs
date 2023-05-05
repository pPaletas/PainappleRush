using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTest : MonoBehaviour
{
    [SerializeField] private LayerMask _limbLayermask;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, _limbLayermask))
            {
                hit.rigidbody.isKinematic = false;
                hit.rigidbody.useGravity = true;
                hit.rigidbody.AddForceAtPosition(Camera.main.transform.forward * 1000f, hit.point, ForceMode.Impulse);
            }
        }
    }
}