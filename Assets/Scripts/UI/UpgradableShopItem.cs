using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradableShopItem : MonoBehaviour
{
    [SerializeField, InlineEditor]
    private UpgradableShopItemData item;
    [SerializeField]
    private Button upgradeButton;
    [SerializeField]
    private TextMeshProUGUI costText;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Vector2 imageMaxDimensions;
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private AudioClip cancelAudio;
    [SerializeField]
    private AudioClip pressAudio;

    private AudioSource _audioSource;
    private InfoDialog _infoDialog;

    private void Awake() {
        _infoDialog = ReferenceManager.Inst.InfoDialog;
        _audioSource = ReferenceManager.Inst.SfxAudio;
    }

    private void Start() {
        image.sprite = item.sprite;
        image.SetNativeSize();
        Vector2 factor = new Vector2(imageMaxDimensions.x / image.rectTransform.sizeDelta.x, imageMaxDimensions.y / image.rectTransform.sizeDelta.y);
        image.rectTransform.sizeDelta *= Mathf.Min(factor.x, factor.y);
        
        titleText.text = item.itemName;
        UpdateUI();
    }

    private void UpdateUI() {
        if (item.IsUpgradable) {
            costText.text = item.effectiveness[item.Level + 1].cost.ToString();
        }
        else {
            costText.gameObject.SetActive(false);
            upgradeButton.gameObject.SetActive(false);
        }
            
    }

    public void Upgrade() {
        if (!item.Upgrade()) {
            _audioSource.PlayOneShot(cancelAudio);
            return;
        }

        _audioSource.PlayOneShot(pressAudio);
        UpdateUI();
    }

    public void Info() {
        _infoDialog.Show(item.itemName, item.GetDescription());
    }
}