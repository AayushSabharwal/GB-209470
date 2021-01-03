using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private int shopSceneBuildIndex;
    [SerializeField]
    private ScriptableObjectReferenceCache refCache;
    
    private void Awake() {
        Application.targetFrameRate = 90;
    }

    private void Start() {
        refCache.Initialize();
    }

    public void Play() {
        SceneManager.LoadSceneAsync(shopSceneBuildIndex);
    }
}
