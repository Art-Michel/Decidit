using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    [SerializeField] Image _hpCursor;
    [SerializeField] RectTransform _hpBarStart;
    [SerializeField] RectTransform _hpBarEnd;

    protected override void DisplayHealth()
    {
        base.DisplayHealth();
        if (_hpCursor)
            _hpCursor.rectTransform.anchoredPosition = Vector2.Lerp(_hpBarStart.anchoredPosition,_hpBarEnd.anchoredPosition, _hpUi.fillAmount);
    }
}
