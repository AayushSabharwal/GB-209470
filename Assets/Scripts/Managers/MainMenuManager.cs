using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private int shopSceneBuildIndex;
    [SerializeField]
    private int levelSceneBuildIndex;
    [SerializeField]
    private Button newButton;
    [SerializeField]
    private Button continueButton;
    [SerializeField]
    private TextMeshProUGUI continueButtonText;
    [SerializeField]
    private Button shopButton;
    [SerializeField]
    private TextMeshProUGUI shopButtonText;
    [SerializeField]
    private Color emphasisColour;
    [SerializeField]
    private Color baseColour;
    [SerializeField]
    private Color textDisabledColour;
    [SerializeField]
    private ScriptableObjectReferenceCache refCache;
    
    private void Awake() {
        Application.targetFrameRate = 90;
    }

    private void Start() {
        refCache.Initialize();
        if (File.Exists(Application.persistentDataPath + "/savegame.sgm")) {
            newButton.image.color = baseColour;
            continueButton.image.color = emphasisColour;
            continueButton.interactable = true;
            shopButton.interactable = true;
        }
        else {
            newButton.image.color = emphasisColour;
            continueButton.image.color = baseColour;
            continueButton.interactable = false;
            shopButton.interactable = false;
            continueButtonText.color = textDisabledColour;
            shopButtonText.color = textDisabledColour;
        }
    }

    public void NewGame() {
        if (File.Exists(Application.persistentDataPath + "/savegame.sgm")) {
            File.Delete(Application.persistentDataPath + "/savegame.sgm");
        }
        Play();
    }

    public void Play() {
        SceneManager.LoadSceneAsync(levelSceneBuildIndex);
    }

    public void Shop() {
        SceneManager.LoadSceneAsync(shopSceneBuildIndex);
    }
}
