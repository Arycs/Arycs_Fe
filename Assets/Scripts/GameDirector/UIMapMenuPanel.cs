using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using DR.Book.SRPG_Dev.UI;
using UnityEngine;

public class UIMapMenuPanel : MonoBehaviour
{
    private SubUIButtonLayoutGroup m_ButtonLayoutGroup;

    private Action<MenuTextID> m_OnItemClickAction;

    private void Button_onClick(MenuTextID menuTextID)
    {
        //TODO 关闭其他界面
        if (m_OnItemClickAction != null)
        {
            m_OnItemClickAction(menuTextID);
        }
    }

}
