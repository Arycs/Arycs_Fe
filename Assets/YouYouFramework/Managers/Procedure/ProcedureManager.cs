using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YouYou
{
    /// <summary>
    /// 流程状态
    /// </summary>
    public enum ProcedureState
    {
        Launch = 0,
        CheckVersion = 1,
        Preload = 2,
        ChangeScene = 3,
        LogOn = 4,
        SelectRole = 5,
        EnterGame = 6,
        WorldMap = 7,
        GameLevel = 8
    }

    /// <summary>
    /// 流程管理器
    /// </summary>
    public class ProcedureManager : ManagerBase,IDisposable
    {
        /// <summary>
        /// 流程状态机
        /// </summary>
        private Fsm<ProcedureManager> m_CurrFsm;

        /// <summary>
        /// 当前流程状态机
        /// </summary>
        public Fsm<ProcedureManager> CurrFsm
        {
            get { return m_CurrFsm; }
        }

        /// <summary>
        /// 当前流程的枚举
        /// </summary>
        public ProcedureState CurProcedureState
        {
            get { return (ProcedureState) m_CurrFsm.CurrStateType; }
        }

        /// <summary>
        /// 当前的流程
        /// </summary>
        public FsmState<ProcedureManager> CurrProcedure
        {
            get { return m_CurrFsm.GetState(m_CurrFsm.CurrStateType); }
        }

        public ProcedureManager()
        {
            
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init()
        {
            FsmState<ProcedureManager>[] states = new FsmState<ProcedureManager>[9];
            states[0] = new ProcedureLaunch();
            states[1] = new ProcedureCheckVersion();
            states[2] = new ProcedurePreload();
            states[3] = new ProcedureChangeScene();
            states[4] = new ProcedureLogOn();
            states[5] = new ProcedureSelectRole();
            states[6] = new ProcedureEnterGame();
            states[7] = new ProcedureWorldMap();
            states[8] = new ProcedureGameLevel();
            
            m_CurrFsm = GameEntry.Fsm.Create(this,states);
        }
        
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(ProcedureState state)
        {
            m_CurrFsm.ChangeState((sbyte)state);
        }


        public void OnUpdate()
        {
            m_CurrFsm.OnUpdate();
        }

        public void Dispose()
        {
            
        }

        /// <summary>
        /// 取得参数
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public TData GetData<TData>(string key)
        {
            return CurrFsm.GetData<TData>(key);
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="TData">泛型类型</typeparam>
        public void SetData<TData>(string key, TData value)
        {
            CurrFsm.SetData<TData>(key, value);
        }

        /*
         * *  一一 ： 剑魂40 剑魔30 奶妈10 酱油 酱油 酱油
         *  别云 ： 剑宗26 剑魂17 红眼19 奶妈9.5 奶爸8.5 巨龙
         *  TY  ： 刃影40 奶萝10 巨龙 酱油 酱油 酱油
         *  傲然 ： 井盖20 死灵20 光兵15 奶妈9.0 巨龙 巨龙
         *  剑心 ： 剑魂20 刃影15 巨龙
         *  阿睿 :  瞎子20 冰洁15 关羽15 奶爸8.5 巨龙 奶妈7.0
         *  矢崎 ： 剑帝40 剑魔30 蓝拳25 剑魂20 奶爸10 奶妈9.0
         *  野鸡 ： 漫游40 奶妈8.0 剑帝25 巨龙 巨龙
         *  小朵 ： 剑宗20 龙神20 元素15 奶萝8.0 奶妈7.0 酱油
         *  阿哲 ： 剑帝20 红眼20 奶妈8.5 巨龙 巨龙 巨龙
         * 
         *  剑心 ： 剑魂20 刃影15 巨龙
         * 
         *  波数      1队主C        1队酱油        1队巨龙        1队奶         2队主C        2队副C        2队巨龙        2队奶         酱油X1    
         *   1      矢崎剑帝40      一一酱油       野鸡巨龙       傲然奶妈       别云剑宗       小朵龙神       阿哲巨龙       阿睿奶妈       TY 酱油
         *   2      一一剑魂40      小朵酱油       傲然巨龙       TY 奶萝       矢崎蓝拳       别云剑魂       野鸡巨龙       阿哲奶妈       阿睿冰洁                
         *   3      野鸡漫游40      一一酱油       TY 巨龙       矢崎奶妈       傲然井盖       阿睿瞎子       阿哲巨龙       别云奶爸       小朵奶妈                               
         *   4      TY 刃影40      一一酱油       野鸡巨龙       别云奶妈       小朵剑宗       矢崎剑魂       傲然光兵       阿睿奶爸       阿哲红眼/阿哲巨龙                  
         *   5      一一剑魔30      TY 酱油       傲然巨龙       矢崎奶爸       阿哲剑帝       别云红眼       阿睿巨龙       野鸡奶妈       小朵元素                                
         *   6      矢崎剑魔25      TY 酱油       阿哲巨龙       一一奶妈       傲然死灵       野鸡剑帝       别云巨龙       小朵奶萝       阿睿关羽
         *                                     (野鸡巨龙)                                (阿哲红眼)
         */     
    }
}
