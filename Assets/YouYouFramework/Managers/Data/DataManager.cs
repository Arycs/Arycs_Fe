using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YouYou
{
    /// <summary>
    /// 数据组件
    /// </summary>
    public class DataManager : ManagerBase, IDisposable
    {
        /// <summary>
        /// 系统数据
        /// </summary>
        public SysDataManager SysDataManager
        {
            get; private set;
        }

        /// <summary>
        /// 角色数据控制器
        /// </summary>
        public RoleDataManager RoleDataManager
        {
            get;
            private set;
        }

        /// <summary>
        /// 物品数据控制器
        /// </summary>
        public ItemDataManager ItemDataManager
        {
            get;
            private set;
        }


        public DataManager()
        {
            SysDataManager = new SysDataManager();
            RoleDataManager = new RoleDataManager();
            ItemDataManager = new ItemDataManager();
        }

        public void Dispose()
        {
            SysDataManager.Dispose();
            RoleDataManager.Dispose();
            ItemDataManager.Dispose();
        }

        public override void Init()
        {
            RoleDataManager.Init();
        }
    }
}
