using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private int shopSceneBuildIndex;

    private void Awake() {
        Application.targetFrameRate = 90;
    }

    public void Play() {
        SceneManager.LoadScene(shopSceneBuildIndex);
    }
}
