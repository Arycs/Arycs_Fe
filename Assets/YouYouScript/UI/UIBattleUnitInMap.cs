using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Models;
using UnityEngine;
using YouYou;

public class UIBattleUnitInMap : MonoBehaviour
{
    private SubUIBattleUnitInMap m_LeftUnit;
    private SubUIBattleUnitInMap m_RightUnit;
    
    public struct RoleArg
    {
        public RoleType roleType;
        public int id;
        public ulong guid;
    }

    private Role GetRole(RoleArg arg)
    {
        Role role = null;
        if (arg.roleType == RoleType.Unique)
        {
            UniqueRole r;
            GameEntry.Data.RoleDataManager.m_UniqueRoles.TryGetValue(arg.id, out r);
            role = r;
        }
        else
        {
            FollowingRole r; 
            GameEntry.Data.RoleDataManager.m_FollowingRoles.TryGetValue(arg.guid, out r);
            role = r;
        }

        return role;
    }

    private string GetRoleName(Role role)
    {
        if (role.roleType == RoleType.Unique)
        {
            return role.character.info.CharacterName;
        }
        else
        {
            return role.cls.info.ClassName;
        }
    }

    public void OnOpen(params object[] args)
    {
        m_LeftUnit.maskImage.fillAmount = 0f;
        m_RightUnit.maskImage.fillAmount = 0f;
    }

    public void OnOpenBattleWindow(RoleArg left, RoleArg right, float time)
    {
        Role leftRole = GetRole(left);
        Role rightRole = GetRole(right);

        m_LeftUnit.SetName(GetRoleName(leftRole));
        m_RightUnit.SetName(GetRoleName(rightRole));

        m_LeftUnit.SetSliderHpMinMaxValue(0, leftRole.maxHp);
        m_RightUnit.SetSliderHpMinMaxValue(0, rightRole.maxHp);

        m_LeftUnit.SetSliderHpValue(leftRole.hp);
        m_LeftUnit.SetSliderHpValue(rightRole.hp);

        if (time <=0  )
        {
            m_LeftUnit.maskImage.fillAmount = 1f;
            m_LeftUnit.maskImage.fillAmount = 1f;
        }
        else
        {
            StartCoroutine(OpeningWindow(time));
        }

    }
    private IEnumerator OpeningWindow(float time)
    {
        float t = 0f;
        float value = 0f;
        while (value != 1f)
        {
            t += Time.deltaTime;
            value = t / time;
            if (value > 1f)
            {
                value = 1f;
            }
            m_LeftUnit.maskImage.fillAmount = value;
            m_RightUnit.maskImage.fillAmount = value;
            yield return null;
        }
    }

    public void UpdateHp(int leftHp, int rightHp, float time)
    {
        m_LeftUnit.UpdateHpAsync(leftHp,time);
        m_RightUnit.UpdateHpAsync(rightHp,time);
    }
}
