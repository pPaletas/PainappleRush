using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DisableDebugMenu : MonoBehaviour
{
    void Awake()
    {
        DebugManager.instance.enableRuntimeUI = false;
    }
}