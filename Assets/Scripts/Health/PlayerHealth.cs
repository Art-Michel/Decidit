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

    public override void TakeDamage(int amount = 10)
    {
        base.TakeDamage();
        PlaceHolderSoundManager.Instance.PlayHurt();
        Player.Instance.StartShake(0.3f, 0.3f);
        GameManager.Instance.StartSlowMo(0.1f, 1f);
    }

    [Button]
    private void ProbRegen(int amount = 10)
    {
        if (_hp < _probHp)
        {
            _hp = Mathf.Clamp(_hp + amount, 0, _probHp);
            DisplayHealth();
            PlaceHolderSoundManager.Instance.PlayRegen();
            //StartProbHealth(); //uncomment if we want to reset prob timer upon regen
        }
    }
}
