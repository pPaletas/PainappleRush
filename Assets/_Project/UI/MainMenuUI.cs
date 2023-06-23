using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    private bool GetInputDown()
    {
        bool touchedScreen = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        touchedScreen = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#else
        touchedScreen = Input.GetMouseButtonDown(0);
#endif

        return touchedScreen;
    }

    private IEnumerator ChangeToNextScene()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("Level1");
    }

    private void Update()
    {
        if (GetInputDown())
        {
            StartCoroutine(ChangeToNextScene());
        }
    }
}