using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField, BoxGroup("UI References")]
    private GameObject hud;
    [SerializeField, FoldoutGroup("UI References/Pause Screen")]
    private GameObject pauseScreen;
    [SerializeField, FoldoutGroup("UI References/Pause Screen")]
    private RectTransform pauseSidebar;
    [SerializeField, BoxGroup("UI References")]
    private GameObject gameOverScreen;
    [SerializeField, FoldoutGroup("UI References/Level Over Screen")]
    private GameObject levelOverScreen;
    [SerializeField, FoldoutGroup("UI References/Level Over Screen")]
    private RectTransform levelOverSidebar;
    [SerializeField, BoxGroup("UI References")]
    private Image[] gunSlots;
    [SerializeField]
    private Vector2 gunSlotMaxDimensions;
    [SerializeField, BoxGroup("Scenes")]
    private int shopSceneBuildIndex;
    [SerializeField, BoxGroup("Scenes")]
    private int mainMenuBuildIndex;
    [SerializeField, BoxGroup("Configuration")]
    private float sidepanelAnimDuration = 0.8f;

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
            else {
                gunSlots[i].sprite = progressManager.Data.EquippedGuns[i].image;
                gunSlots[i].SetNativeSize();
                Vector2 factor = new Vector2(gunSlotMaxDimensions.x / gunSlots[i].rectTransform.sizeDelta.x,
                                             gunSlotMaxDimensions.y / gunSlots[i].rectTransform.sizeDelta.y);
                gunSlots[i].rectTransform.sizeDelta *= Mathf.Min(factor.x, factor.y);
            }
        }
    }

    public void Pause() {
        _isPaused = !_isPaused;
        if (_isPaused) {
            hud.SetActive(!_isPaused);
            pauseScreen.SetActive(_isPaused);
            pauseSidebar.DOAnchorPos(new Vector2(-500f, 0f), sidepanelAnimDuration);
            OnPause?.Invoke(_isPaused);
        }
        else
            pauseSidebar.DOAnchorPos(new Vector2(500f, 0f), sidepanelAnimDuration).onComplete += () => {
                                                                                                     OnPause
                                                                                                         ?.Invoke(_isPaused);
                                                                                                     hud
                                                                                                         .SetActive(!_isPaused);
                                                                                                     pauseScreen
                                                                                                         .SetActive(_isPaused);
                                                                                                 };
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel() {
        ReferenceManager.Inst.ProgressManager.Save();
        SceneManager.LoadScene(shopSceneBuildIndex);
    }

    public void MainMenu() {
        SceneManager.LoadScene(mainMenuBuildIndex);
    }

    private void GameOver() {
        hud.SetActive(false);
        gameOverScreen.SetActive(true);
    }

    private void LevelOver() {
        levelOverScreen.SetActive(true);
        levelOverSidebar.DOAnchorPos(new Vector2(-500f, 0f), sidepanelAnimDuration);
    }

    private void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus && !_isPaused) {
            Pause();
        }
    }
}