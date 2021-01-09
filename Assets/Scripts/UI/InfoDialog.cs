using TMPro;
using UnityEngine;

public class InfoDialog : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI titleShadow;
    [SerializeField]
    private TextMeshProUGUI content;

    public void Show(string titleText, string contentText) {
        title.text = titleText;
        titleShadow.text = title.text;
        content.text = contentText;
        gameObject.SetActive(true);
    }

    public void Finish() {
        gameObject.SetActive(false);
    }
}