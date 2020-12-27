using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField, BoxGroup("UI References")]
    private GameObject hud;
    [SerializeField, BoxGroup("UI References")]
    private GameObject pauseScreen;
    [SerializeField, BoxGroup("UI References")]
    private GameObject gameOverScreen;
    [SerializeField, BoxGroup("UI References")]
    private GameObject levelOverScreen;
    [SerializeField, BoxGroup("UI References")]
    private Image[] gunSlots;
    [SerializeField, BoxGroup("Scenes")]
    private int shopSceneBuildIndex;
    
    public delegate void OnPauseDelegate(bool isPaused);

    public event OnPauseDelegate OnPause;
    private bool _isPaused;

    private void Awake() {
        _isPaused = false;
    }

    private void Start() {
        hud.SetActive(true);
        gameOverScreen.SetActive(false);
        levelOverScreen.SetActive(false);
        pauseScreen.SetActive(false);

        ReferenceManager.Inst.PlayerHealth.OnDeath += (_, __) => GameOver();
        ReferenceManager.Inst.EnemySpawner.OnLevelEnd += LevelOver;
        ProgressManager progressManager = ReferenceManager.Inst.ProgressManager;
        for (int i = 0; i < 3; i++) {
            if (i >= progressManager.Data.EquippedGuns.Length || progressManager.Data.EquippedGuns[i] == null)
                gunSlots[i].color = new Color(0f, 0f, 0f, 0f);
            else
                gunSlots[i].sprite = progressManager.Data.EquippedGuns[i].image;
        }
    }

    public void Pause() {
        _isPaused = !_isPaused;
        hud.SetActive(!_isPaused);
        pauseScreen.SetActive(_isPaused);
        OnPause?.Invoke(_isPaused);
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel() {
        SceneManager.LoadScene(shopSceneBuildIndex);
    }

    private void GameOver() {
        hud.SetActive(false);
        gameOverScreen.SetActive(true);
    }

    private void LevelOver() {
        hud.SetActive(false);
        levelOverScreen.SetActive(true);
    }
}
