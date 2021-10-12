using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YouYou;

public class UIStartForm : UIFormBase
{
    public Button Btn_Start;

    public Button Btn_Load;

    public Button Btn_Exit;
    
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        Btn_Start.onClick.AddListener(OnStartGame);
        Btn_Load.onClick.AddListener(OnLoadGame);
        Btn_Exit.onClick.AddListener(OnExitGame);
    }

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
    }

    protected override void OnClose()
    {
        base.OnClose();
    }

    public void OnStartGame()
    {
        GameEntry.Procedure.ChangeState(ProcedureState.EnterGame);
    }

    public void OnLoadGame()
    {
        //TODO 读档 开始 游戏
    }

    public void OnExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    protected override void OnBeforDestroy()
    {
        base.OnBeforDestroy();
    }
}
