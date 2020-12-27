using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class ReferenceManager : SerializedMonoBehaviour
{
    public static ReferenceManager Inst;

    [OdinSerialize, ShowInInspector, FoldoutGroup("In Game")]
    public Health PlayerHealth { get; private set; }
    [OdinSerialize, ShowInInspector, FoldoutGroup("In Game")]
    public PlayerShooter PlayerShooter { get; private set; }
    [OdinSerialize, ShowInInspector, FoldoutGroup("In Game")]
    public MapGenerator MapGenerator { get; private set; }
    [OdinSerialize, ShowInInspector, FoldoutGroup("In Game")]
    public ObjectPooler ObjectPooler { get; private set; }
    [OdinSerialize, ShowInInspector, FoldoutGroup("In Game")]
    public DropManager DropManager { get; private set; }
    [OdinSerialize, ShowInInspector, FoldoutGroup("In Game")]
    public SharedDataManager SharedDataManager { get; private set; }
    [OdinSerialize, ShowInInspector, FoldoutGroup("In Game")]
    public EnemySpawner EnemySpawner { get; private set; }
    [OdinSerialize, ShowInInspector]
    public CurrencyManager CurrencyManager { get; private set; }
    [OdinSerialize, ShowInInspector, FoldoutGroup("In Game")]
    public UIManager UIManager { get; private set; }
    [OdinSerialize, ShowInInspector]
    public ProgressManager ProgressManager { get; private set; }
    [OdinSerialize, ShowInInspector, FoldoutGroup("Shop Screen")]
    public ShopManager ShopManager { get; private set; }
    [OdinSerialize, ShowInInspector, FoldoutGroup("Shop Screen")]
    public EquipGunDialog EquipGunDialog { get; private set; }
    [OdinSerialize, ShowInInspector, FoldoutGroup("Shop Screen")]
    public InfoDialog InfoDialog { get; private set; }
    
    private void Awake() {
        if (Inst == null)
            Inst = this;
        else if (Inst != this) {
            Destroy(gameObject);
            return;
        }
    }
}
