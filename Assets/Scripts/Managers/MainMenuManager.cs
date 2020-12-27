using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private int shopSceneBuildIndex;
    
    public void Play() {
        SceneManager.LoadScene(shopSceneBuildIndex);
    }
}
