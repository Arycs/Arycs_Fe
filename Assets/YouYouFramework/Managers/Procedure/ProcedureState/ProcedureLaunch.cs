using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 开始流程
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("OnEnter ProcedureLaunch");

            //访问账号服务器
            string url = GameEntry.Http.RealWebAccountUrl + "/init";

            //从对象池中获取字典,用来存储数据,防止是之前的数据,对字典进行Clear
            Dictionary<string, object> dic = GameEntry.Pool.DequeueClassObject<Dictionary<string, object>>();
            dic.Clear();

            GameEntry.Data.SysDataManager.CurrChannelConfig.ChannelId = 0;
            GameEntry.Data.SysDataManager.CurrChannelConfig.InnerVersion = 1001;

            dic["ChannelId"] = GameEntry.Data.SysDataManager.CurrChannelConfig.ChannelId;
            dic["InnerVersion"] = GameEntry.Data.SysDataManager.CurrChannelConfig.InnerVersion;
            GameEntry.Http.SendData(url, OnWebAccountInit, true, false, dic);
        }

        /// <summary>
        /// HTTP请求回来的数据,服务器没写好,先注释.
        /// </summary>
        /// <param name="args"></param>
        private void OnWebAccountInit(HttpCallBackArgs args)
        {
            if (!args.HasError)
            {
                RetValue retValue = LitJson.JsonMapper.ToObject<RetValue>(args.Value);
                if (!retValue.HasError)
                {
                    LitJson.JsonData data = LitJson.JsonMapper.ToObject(args.Value);
                    LitJson.JsonData config = LitJson.JsonMapper.ToObject(data["Value"].ToString());
                    GameEntry.Data.SysDataManager.CurrChannelConfig.ServerTime =
                        long.Parse(config["ServerTime"].ToString());
                    GameEntry.Data.SysDataManager.CurrChannelConfig.SourceVersion = config["SourceVersion"].ToString();
                    GameEntry.Data.SysDataManager.CurrChannelConfig.SourceUrl = config["SourceUrl"].ToString();
                    GameEntry.Data.SysDataManager.CurrChannelConfig.RechargeUrl = config["RechargeUrl"].ToString();
                    GameEntry.Data.SysDataManager.CurrChannelConfig.TDAppId = config["TDAppId"].ToString();
                    bool.TryParse(config["IsOpenTD"].ToString(),
                        out GameEntry.Data.SysDataManager.CurrChannelConfig.IsOpenTD);

                    Debug.Log("RealSourceUrl" + GameEntry.Data.SysDataManager.CurrChannelConfig.RealSourceUrl);
                    //连接上服务器, 切换检查版本更新流程
                    GameEntry.Procedure.ChangeState(ProcedureState.CheckVersion);
                }
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            //Debug.Log("OnUpdate ProcedureLaunch");
        }

        public override void OnLeave()
        {
            base.OnLeave();
            Debug.Log("OnLeave ProcedureLaunch");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }

}
