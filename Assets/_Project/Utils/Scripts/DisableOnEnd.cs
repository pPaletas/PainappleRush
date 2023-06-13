using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnEnd : MonoBehaviour
{
    public void DisableGO()
    {
        gameObject.SetActive(false);
    }
}