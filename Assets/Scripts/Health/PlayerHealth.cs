using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    [SerializeField] Image _HpCursor;

    protected override void DisplayHealth()
    {
        base.DisplayHealth();
        if (_HpCursor)
        {
            float x = 0; //TODO calculer ça
            _HpCursor.rectTransform.position = new Vector3(x , _hpUi.rectTransform.position.y, 0);
        }
    }
}
