using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

namespace YouYou
{
    /// <summary>
    /// Http发送数据的回调委托
    /// </summary>
    /// <param name="args"></param>
    public delegate void HttpSendDataCallBack(HttpCallBackArgs args);

    /// <summary>
    /// Http访问器
    /// </summary>
    public class HttpRoutine
    {
        #region 属性

        /// <summary>
        /// Http请求回调
        /// </summary>
        private HttpSendDataCallBack m_CallBack;

        /// <summary>
        /// Http请求回调数据
        /// </summary>
        private HttpCallBackArgs m_CallBackArgs;

        /// <summary>
        /// 是否繁忙
        /// </summary>
        public bool IsBusy
        {
            get; private set;
        }

        /// <summary>
        /// 当前重试次数
        /// </summary>
        private int m_CurrRetry = 0;

        /// <summary>
        /// Url
        /// </summary>
        private string m_Url;

        /// <summary>
        /// Post
        /// </summary>
        private bool m_IsPost = false;

        /// <summary>
        /// 是否获取data数据
        /// </summary>
        private bool m_IsGetData = false;

        /// <summary>
        /// 发送的数据
        /// </summary>
        private Dictionary<string, object> m_Dic;
        #endregion

        public HttpRoutine()
        {
            m_CallBackArgs = new HttpCallBackArgs();
            m_Dic = new Dictionary<string, object>();
        }

        #region SendData 发送web数据
        /// <summary>
        /// 发送web数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isPost">是否是Post请求</param>
        /// <param name="dic">Post需要处理的数据</param>
        public void SendData(string url, HttpSendDataCallBack callBack, bool isPost = false, bool isGetData = false,
            Dictionary<string, object> dic = null)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            m_Url = url;
            m_CallBack = callBack;
            m_IsPost = isPost;
            m_IsGetData = isGetData;
            m_Dic = dic;
            SendData();
        }

        public void SendData()
        {
            if (!m_IsPost)
            {
                GetUrl(m_Url);
            }
            else
            {
                //web加密
                if (m_Dic != null)
                {
                    //客户端标识符
                    m_Dic["deviceIdentifier"] = DeviceUtil.DeviceIdentifier;

                    //设备型号
                    m_Dic["deviceModel"] = DeviceUtil.DeviceModel;

                    long t = GameEntry.Data.SysDataManager.CurrServerTime;
                    //签名
                    m_Dic["sign"] = EncryptUtil.Md5(string.Format("{0}:{1}", t, DeviceUtil.DeviceIdentifier));

                    //时间戳
                    m_Dic["t"] = t;
                }

                string json = string.Empty;
                if (m_Dic != null)
                {
                    json = JsonMapper.ToJson(m_Dic);
#if DEBUG_LOG_PROTO
                    Debug.Log("<color=#ffa200>发送消息:</color><color=#fffb80>" + m_Url + "</color>");
                    Debug.Log("<color=#ffdeb3>==>>" + json + "</color>");
#endif
                    GameEntry.Pool.EnqueueClassObject(m_Dic);
                }

                PostUrl(m_Url, json);
            }
        }

        #endregion

        #region GetUrl Get请求,从url中请求数据
        /// <summary>
        /// Get请求,从url中请求数据
        /// </summary>
        /// <param name="url">地址</param>
        private void GetUrl(string url)
        {
            UnityWebRequest data = UnityWebRequest.Get(url);
            GameEntry.Instance.StartCoroutine(Request(data));
        }

        #endregion

        #region PostUrl Post请求,向url提交要被处理的数据
        /// <summary>
        /// Post请求,向url提交要被处理的数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="json">要处理的数据</param>
        private void PostUrl(string url, string json)
        {
            //定义一个表单
            WWWForm form = new WWWForm();
            form.AddField("json", json);

            UnityWebRequest data = UnityWebRequest.Post(url, form);

            GameEntry.Instance.StartCoroutine(Request(data));
        }

        #endregion

        #region Request 请求服务器
        /// <summary>
        /// 请求服务器
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerator Request(UnityWebRequest data)
        {
            //发送请求
            yield return data.SendWebRequest();

            // WebRequest有错误, 回调参数记录错误
            if (data.isNetworkError || data.isHttpError)
            {
                //报错了， 进行重试
                m_CurrRetry++;
                if (m_CurrRetry <= GameEntry.Http.Retry)
                {
                    GameEntry.Log(LogCategory.Proto, "<color=#eaff>请求URL：</color><color=#00ff9c> {0}失败 当前重试次数{1}</color>", m_Url, m_CurrRetry);
                    SendData();
                    yield break;
                }

                IsBusy = false;

                if (m_CallBack != null)
                {
                    m_CallBackArgs.HasError = true;
                    m_CallBackArgs.Value = data.error;

                    if (!m_IsGetData)
                    {
                        GameEntry.Log(LogCategory.Proto, "<color=#00eaff>接收消息:</color><color=#00ff9c>" + data.url + "</color>");
                        GameEntry.Log(LogCategory.Proto, "<color=#c5e1dc>==>>" + JsonUtility.ToJson(m_CallBackArgs) + "</color>");
                    }
                    m_CallBack(m_CallBackArgs);
                }
            }
            else
            {
                IsBusy = false;

                if (m_CallBack != null)
                {
                    m_CallBackArgs.HasError = false;
                    m_CallBackArgs.Value = data.downloadHandler.text;
                    if (!m_IsGetData)
                    {
                        GameEntry.Log(LogCategory.Proto, "<color=#00eaff>接收消息:</color><color=#00ff9c>" + data.url + "</color>");
                        GameEntry.Log(LogCategory.Proto, "<color=#c5e1dc>==>>" + JsonUtility.ToJson(m_CallBackArgs) + "</color>");
                    }

                    m_CallBackArgs.Data = data.downloadHandler.data;
                    m_CallBack(m_CallBackArgs);
                }
            }

            m_CurrRetry = 0;
            m_Url = null;
            if (m_Dic != null)
            {
                m_Dic.Clear();
                m_Dic = null;
            }
            m_CallBackArgs.Data = null;
            data.Dispose();
            data = null;

            //Debug.Log("把Http访问器对象回池");
            GameEntry.Pool.EnqueueClassObject(this);
        }

        #endregion
    }
}