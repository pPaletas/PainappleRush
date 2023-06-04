using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInfo : MonoBehaviour
{
    public static SceneInfo sceneInfo;

    private EntityInfo _plr;
    private Transform _plrCharacter;
    private Transform _plrRoot;

    public EntityInfo Plr { get => _plr; }
    public Transform PlrCharacter { get => _plrCharacter; }
    public Transform PlrRoot { get => _plrRoot; }

    private void Awake()
    {
        if (sceneInfo == null)
            sceneInfo = this;

        _plr = GameObject.FindAnyObjectByType<PlayerInput>().GetComponent<EntityInfo>();
        _plrCharacter = _plr.transform.Find("Character");

        _plrRoot = _plrCharacter.Find("Pivot/PhysicBody/Armature/Root/Spine");
    }
}