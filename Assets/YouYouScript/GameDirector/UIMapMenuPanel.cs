using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using DR.Book.SRPG_Dev.UI;
using UnityEngine;
using UnityEngine.UI;
using YouYou;

public class UIMapMenuPanel : MonoBehaviour
{
    private SubUIButtonLayoutGroup m_ButtonLayoutGroup;

    private Action<MenuTextID> m_OnItemClickAction;

    private readonly Dictionary<MenuTextID, SubUIButtonLayoutGroup.ItemOption> m_MenuOptions =
        new Dictionary<MenuTextID, SubUIButtonLayoutGroup.ItemOption>();

    private readonly Dictionary<string, Action> m_ButtonClickActions = new Dictionary<string, Action>();

    public void OnOpen(params object[] args)
    {
        m_ButtonLayoutGroup.Display(false);
    }
    
    /// <summary>
    /// 打开菜单
    /// </summary>
    /// <param name="textIds"></param>
    /// <param name="onItemClick"></param>
    /// <param name="left"></param>
    public void OpenMenu(IEnumerable<MenuTextID> textIds, Action<MenuTextID> onItemClick)
    {
        if (onItemClick == null)
        {
            Debug.LogError("OpenMapMenu error: `action` is null.");
            //TODO 关闭其他 界面
            return;
        }

        m_OnItemClickAction = onItemClick;
        m_ButtonLayoutGroup.itemOptions.Clear();

        // 设置需要打开的按钮
        if (textIds != null)
        {
            foreach (MenuTextID item in textIds)
            {
                m_ButtonLayoutGroup.itemOptions.Add(m_MenuOptions[item]);
            }
        }

        // 没有按钮显示，直接关闭
        if (m_ButtonLayoutGroup.itemOptions.Count == 0)
        {
            //TODO 关闭其他 界面
            onItemClick(MenuTextID.Close);
            return;
        }

        // 打开菜单
        m_ButtonLayoutGroup.Display(true);
    }

    private void Button_onClick(MenuTextID menuTextID)
    {
        //TODO 关闭其他界面
        if (m_OnItemClickAction != null)
        {
            m_OnItemClickAction(menuTextID);
        }
    }

    /// <summary>
    /// 初始化按钮，
    /// </summary>
    /// <param name="textId"></param>
    /// <param name="action"></param>
    private void InitButtonOption(MenuTextID textId, Action action)
    {
        GameEntry.DataTable.Sys_LanguageDBModel.LanguageDic.TryGetValue((int)textId,out string menuName);
        if (string.IsNullOrEmpty(menuName))
        {
            Debug.LogError("语言包 ID " + (int)textId + "错误");
        }
        SubUIButtonLayoutGroup.ItemOption option = new SubUIButtonLayoutGroup.ItemOption(menuName,"");
        m_MenuOptions[textId] = option;
        m_ButtonClickActions[menuName] = action;
    }
    
    /// <summary>
    /// 初始化所有按钮
    /// </summary>
    private void InitButtonOptions()
    {
        m_MenuOptions.Clear();
        m_ButtonClickActions.Clear();

        InitButtonOption(MenuTextID.Close, ButtonClose_onClick);

        InitButtonOption(MenuTextID.Unit, ButtonUnit_onClick);
        InitButtonOption(MenuTextID.Item, ButtonItem_onClick);
        InitButtonOption(MenuTextID.Data, ButtonData_onClick);
        InitButtonOption(MenuTextID.Skill, ButtonSkill_onClick);
        InitButtonOption(MenuTextID.Config, ButtonConfig_onClick);
        InitButtonOption(MenuTextID.Save, ButtonSave_onClick);
        InitButtonOption(MenuTextID.TurnEnd, ButtonTurnEnd_onClick);

        InitButtonOption(MenuTextID.Move, ButtonMove_onClick);
        InitButtonOption(MenuTextID.Holding, ButtonHolding_onClick);
        InitButtonOption(MenuTextID.Talk, ButtonTalk_onClick);
        InitButtonOption(MenuTextID.Attack, ButtonAttack_onClick);
        InitButtonOption(MenuTextID.Status, ButtonStatus_onClick);
    }

    #region Button Click
    private void ButtonClose_onClick()
    {
        Button_onClick(MenuTextID.Close);
    }    
    private void ButtonUnit_onClick()
    {
        Button_onClick(MenuTextID.Unit);
    }    
    private void ButtonItem_onClick()
    {
        Button_onClick(MenuTextID.Item);
    }    
    private void ButtonData_onClick()
    {
        Button_onClick(MenuTextID.Data);
    }    
    private void ButtonSkill_onClick()
    {
        Button_onClick(MenuTextID.Skill);
    }    
    private void ButtonConfig_onClick()
    {
        Button_onClick(MenuTextID.Config);
    }    
    private void ButtonSave_onClick()
    {
        Button_onClick(MenuTextID.Save);
    }    
    private void ButtonTurnEnd_onClick()
    {
        Button_onClick(MenuTextID.TurnEnd);
    }    
    private void ButtonMove_onClick()
    {
        Button_onClick(MenuTextID.Move);
    }    
    private void ButtonHolding_onClick()
    {
        Button_onClick(MenuTextID.Holding);
    }    
    private void ButtonTalk_onClick()
    {
        Button_onClick(MenuTextID.Talk);
    }    
    private void ButtonAttack_onClick()
    {
        Button_onClick(MenuTextID.Attack);
    }
    private void ButtonStatus_onClick()
    {
        Button_onClick(MenuTextID.Status);
    }
    #endregion
    
    protected IEnumerator OnLoadingView()
    {
        InitButtonOptions();
        m_ButtonLayoutGroup = FindObjectOfType<SubUIButtonLayoutGroup>();
        m_ButtonLayoutGroup.onItemClick.AddListener(ButtonLayoutGroup_onItemClick);

        yield break;
    }

    private void ButtonLayoutGroup_onItemClick(GameObject buttonGameObject, int index, string message)
    {
        Text text = buttonGameObject.GetComponentInChildren<Text>();
        Action action;
        if (!m_ButtonClickActions.TryGetValue(text.text, out action))
        {
            Debug.LogError("Button Click Action was not found.");
            Button_onClick(MenuTextID.Close);
            return;
        }
        action();
    }
    
}
