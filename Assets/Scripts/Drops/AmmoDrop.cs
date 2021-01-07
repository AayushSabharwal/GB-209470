public class AmmoDrop : Drop
{
    private AmmoDropData _data;
    private PlayerShooter _playerShooter;

    protected override void Awake() {
        base.Awake();
        _playerShooter = ReferenceManager.Inst.PlayerShooter;
    }

    protected override void OnEnable() {
        base.OnEnable();
        _data = data as AmmoDropData;
    }

    protected override void OnPickup() {
        base.OnPickup();
        _playerShooter.AddAmmo(_data.type, data.value);
    }
}