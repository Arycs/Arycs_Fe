using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{

    /// <summary>
    /// 系统数据管理器
    /// </summary>
    public class SysDataManager
    {
        /// <summary>
        /// 当前服务器时间
        /// </summary>
        public long CurrServerTime
        {
            get
            {
                if (CurrChannelConfig == null)
                {
                    return (long)Time.unscaledTime;
                }
                else
                {
                    return CurrChannelConfig.ServerTime + (long)Time.unscaledTime;
                }
            }
        }

        /// <summary>
        /// 当前的渠道设置
        /// </summary>
        public ChannelConfigEntity CurrChannelConfig
        {
            get; private set;
        }

        public SysDataManager()
        {
            CurrChannelConfig = new ChannelConfigEntity();
        }


        public void Clear()
        {

        }

        public void Dispose()
        {

        }
        /// <summary>
        /// 根据系统码获取提示内容
        /// </summary>
        /// <returns></returns>
        public string GetSysCodeContent(int sysCode)
        {
            Sys_CodeEntity sys_CodeEntity = GameEntry.DataTable.Sys_CodeDBModel.Get(sysCode);
            if (sys_CodeEntity != null)
            {
                return GameEntry.Localization.GetString(sys_CodeEntity.Key);
            }
            return string.Empty;
           
        }

    }
}
