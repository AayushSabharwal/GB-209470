using UnityEngine;

public class EquipGunDialog : MonoBehaviour
{
    private GunData _active;

    private void Start() {
        gameObject.SetActive(false);
    }

    public void Equip(GunData gun) {
        gameObject.SetActive(true);
        _active = gun;
    }

    public void SlotButton(int index) {
        ReferenceManager.Inst.ShopManager.EquipSlot(_active, index);
        gameObject.SetActive(false);
    }
}
