using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hud;
    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private GameObject gameOverScreen;
    [SerializeField]
    private GameObject levelOverScreen;

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

    private void GameOver() {
        hud.SetActive(false);
        gameOverScreen.SetActive(true);
    }

    private void LevelOver() {
        hud.SetActive(false);
        levelOverScreen.SetActive(true);
    }
}
