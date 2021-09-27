using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubUIBattleUnitInMap : MonoBehaviour
{
    public Text txtHpText;
    public Slider sliderHp;
    public Text nameText;
    public Image maskImage { get; set; }

    public void UpdateHp(int hp)
    {
        SetSliderHpValue(hp);
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void UpdateHpAsync(int hp, float time)
    {
        if (txtHpText == null || sliderHp == null)
        {
            return;
        }

        if (time <= 0f)
        {
            SetSliderHpValue(hp);
            return;
        }

        StartCoroutine(ChangingHp(hp, time));
    }

    public void SetSliderHpValue(float hp)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator ChangingHp(int hp, float time)
    {
        float old = sliderHp.value;
        float offset = (float) hp - sliderHp.value;

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            if (t>time)
            {
                t = time;
            }

            float cur = old + offset * (t / time);
            SetSliderHpValue(cur);
            yield return null;
        }
    }

    public void SetSliderHpMinMaxValue(int i, int rightRoleMAXHp)
    {
        throw new System.NotImplementedException();
    }
}
