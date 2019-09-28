using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Error : MonoBehaviour
{
    private const string errorScene = "Error";
    private static string nextSceneName;
    private static string errorMessage;

    public Text text;

    public static void ShowError(string sceneName, string message)
    {
        nextSceneName = sceneName;
        errorMessage = message;
        SceneManager.LoadScene(errorScene, LoadSceneMode.Single);
    }

    private void Awake()
    {
        text.text = errorMessage;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (SceneManager.GetSceneByName(nextSceneName) != null)
                SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
            else
                Debug.Log("next scene not defined");
        }
    }
}
