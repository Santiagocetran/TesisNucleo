using UnityEngine;
using TMPro;

// Add this new component to manage the hold UI
public class HoldUI : MonoBehaviour
{
    public static HoldUI Instance { get; private set; }

    [SerializeField] private Canvas holdCanvas;
    [SerializeField] private TextMeshProUGUI holdText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Ensure UI starts hidden
        if (holdCanvas != null)
        {
            holdCanvas.enabled = false;
        }
    }

    public void ShowHoldUI(bool show)
    {
        if (holdCanvas != null)
        {
            holdCanvas.enabled = show;
        }
    }
}