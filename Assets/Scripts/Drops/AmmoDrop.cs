public class AmmoDrop : Drop
{
    private PlayerShooter _playerShooter;

    protected override void Awake() {
        base.Awake();
        _playerShooter = ReferenceManager.Inst.Player.GetComponent<PlayerShooter>();
    }

    protected override void OnPickup() {
        _playerShooter.AddAmmo((data as AmmoDropData).type, data.value);
    }
}