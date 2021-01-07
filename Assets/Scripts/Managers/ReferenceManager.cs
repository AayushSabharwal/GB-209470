using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Inst;
    [SerializeField]
    private Health playerHealth;
    [SerializeField]
    private PlayerShooter playerShooter;
    [SerializeField]
    private AudioSource sfxAudio;
    [SerializeField]
    private MapGenerator mapGenerator;
    [SerializeField]
    private ObjectPooler objectPooler;
    [SerializeField]
    private DropManager dropManager;
    [SerializeField]
    private SharedDataManager sharedDataManager;
    [SerializeField]
    private EnemySpawner enemySpawner;
    [SerializeField]
    private CurrencyManager currencyManager;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private ProgressManager progressManager;
    [SerializeField]
    private ShopManager shopManager;
    [SerializeField]
    private EquipGunDialog equipGunDialog;
    [SerializeField]
    private InfoDialog infoDialog;

    public Health PlayerHealth => playerHealth;
    public PlayerShooter PlayerShooter => playerShooter;
    public AudioSource SfxAudio => sfxAudio;
    public MapGenerator MapGenerator => mapGenerator;
    public ObjectPooler ObjectPooler => objectPooler;
    public DropManager DropManager => dropManager;
    public SharedDataManager SharedDataManager => sharedDataManager;
    public EnemySpawner EnemySpawner => enemySpawner;
    public CurrencyManager CurrencyManager => currencyManager;
    public UIManager UIManager => uiManager;
    public ProgressManager ProgressManager => progressManager;
    public ShopManager ShopManager => shopManager;
    public EquipGunDialog EquipGunDialog => equipGunDialog;
    public InfoDialog InfoDialog => infoDialog;

    private void Awake() {
        if (Inst == null)
            Inst = this;
        else if (Inst != this) {
            Destroy(gameObject);
            return;
        }
    }
}