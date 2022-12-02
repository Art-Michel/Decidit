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

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        PlaceHolderSoundManager.Instance.PlayHurt();
        Player.Instance.StartShake(0.3f, 0.3f);
        GameManager.Instance.StartSlowMo(0.1f, 1f);
    }
}
