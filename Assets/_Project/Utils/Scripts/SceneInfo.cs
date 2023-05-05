using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInfo : MonoBehaviour
{
    public static SceneInfo sceneInfo;

    private EntityInfo _plr;
    private Transform _plrCharacter;

    public EntityInfo Plr { get => _plr; }
    public Transform PlrCharacter { get => _plrCharacter; }

    private void Awake()
    {
        if (sceneInfo == null)
            sceneInfo = this;

        _plr = GameObject.FindAnyObjectByType<PlayerInput>().GetComponent<EntityInfo>();
        _plrCharacter = _plr.transform.Find("Character");
    }
}