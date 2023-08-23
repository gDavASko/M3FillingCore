using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelResult : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resultScore = null;
    [SerializeField] private Button _buttonNextLevel = null;

    private System.Action OnApplyCallback = null;

    public void ShowWithParams(int score, System.Action OnApplyCallback)
    {
        _resultScore.text = score.ToString();
        this.OnApplyCallback = OnApplyCallback;
        gameObject.SetActive(true);
    }

    private void Awake()
    {
        _buttonNextLevel.onClick.AddListener(OnNextClick);
    }

    private void OnNextClick()
    {
        OnApplyCallback?.Invoke();
        OnApplyCallback = null;

        gameObject.SetActive(false);
    }
}