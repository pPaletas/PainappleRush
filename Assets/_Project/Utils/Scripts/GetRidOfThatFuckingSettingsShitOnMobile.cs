using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GetRidOfThatFuckingSettingsShitOnMobile : MonoBehaviour
{
    private void Awake()
    {
        DebugManager.instance.enableRuntimeUI = false;
    }
}