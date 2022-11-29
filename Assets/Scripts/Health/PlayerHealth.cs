using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class PlayerHealth : Health
{
    [SerializeField] Image _hpCursor;
    [SerializeField] RectTransform _hpBarStart;
    [SerializeField] RectTransform _hpBarEnd;

    protected override void DisplayHealth()
    {
        base.DisplayHealth();
        if (_hpCursor)
            _hpCursor.rectTransform.anchoredPosition = Vector2.Lerp(_hpBarStart.anchoredPosition, _hpBarEnd.anchoredPosition, _hpUi.fillAmount);
    }

    [Button]
    private void ProbRegen(int amount = 10)
    {
        _hp = Mathf.Clamp(_hp + amount, 0, _probHp);
        DisplayHealth();
        //StartProbHealth();
    }
}
