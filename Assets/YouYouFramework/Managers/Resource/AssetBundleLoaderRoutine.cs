using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 资源包加载器
    /// </summary>
    public class AssetBundleLoaderRoutine
    {
        /// <summary>
        /// 当前的资源包信息
        /// </summary>
        private AssetBundleInfoEntity m_CurrAssetBundleInfo;

        /// <summary>
        /// 资源包创建请求
        /// </summary>
        private AssetBundleCreateRequest m_CurrAssetBundleCreateRequest;
        
        /// <summary>
        /// 资源包创建请求更新
        /// </summary>
        public Action<float> OnAssetBundleCreateUpdate;

        /// <summary>
        /// 加载资源包完毕
        /// </summary>
        public Action<AssetBundle> OnLoadAssetBundleComplete;

        #region LoadAssetBundle 加载资源包

        public void LoadAssetBundle(string assetbundlePath)
        {
            m_CurrAssetBundleInfo = GameEntry.Resource.ResourceManager.GetAssetBundleInfo(assetbundlePath);
            byte[] buffer = GameEntry.Resource.ResourceManager.LocalAssetsManager.GetFileBuffer(assetbundlePath);
            if (buffer == null)
            {
                //如果可写区没有 那么就从只读区获取
                GameEntry.Resource.ResourceManager.StreamingAssetsManager.ReadAssetBundle(assetbundlePath,
                    (byte[] buff) =>
                    {
                        if (buff == null)
                        {
                            //如果只读区也没有,从CDN下载
                            Debug.LogError("资源包需要下载assetbundlePath = >" + assetbundlePath);
                            GameEntry.Download.BeginDownloadSingle(assetbundlePath,onComplete: (string fileUrl) =>
                            {
                                Debug.LogError("下载完毕fileUrl =>" + fileUrl);
                                buffer = GameEntry.Resource.ResourceManager.LocalAssetsManager.GetFileBuffer(fileUrl);
                                Debug.LogError("准备加载资源包=" + buffer);
                                LoadAssetBundleAsync(buffer);
                            });
                        }
                        else
                        {
                            LoadAssetBundleAsync(buff);
                        }
                    });
            }
            else
            {
                LoadAssetBundleAsync(buffer);
            }
        }

        /// <summary>
        /// 异步加载资源包
        /// </summary>
        /// <param name="buffer"></param>
        public void LoadAssetBundleAsync(byte[] buffer)
        {
            if (m_CurrAssetBundleInfo.IsEncrypt)
            {
                buffer = SecurityUtil.Xor(buffer);
            }

            m_CurrAssetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(buffer);
        }
        #endregion

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            m_CurrAssetBundleCreateRequest = null;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void OnUpdate()
        {
            UpdateAssetBundleCreateRequest();
        }

        #region UpdateAssetBundleCreateRequest 更新资源包请求
        /// <summary>
        /// 更新资源包请求
        /// </summary>
        private void UpdateAssetBundleCreateRequest()
        {
            if (m_CurrAssetBundleCreateRequest != null)
            {
                if (m_CurrAssetBundleCreateRequest.isDone)
                {
                    AssetBundle assetBundle = m_CurrAssetBundleCreateRequest.assetBundle;
                    if (assetBundle != null)
                    {
                        GameEntry.Log(LogCategory.Resource,"资源包=>{0} 加载完毕",m_CurrAssetBundleInfo.AssetBundleName);
                        Reset(); //一定 要早点Reset
                        if (OnLoadAssetBundleComplete != null)
                        {
                            OnLoadAssetBundleComplete(assetBundle);
                        }
                    }
                    else
                    {
                        GameEntry.Log(LogCategory.Resource,"资源包=>{0} 加载失败",m_CurrAssetBundleInfo.AssetBundleName);
                        Reset();

                        if (OnLoadAssetBundleComplete != null)
                        {
                            OnLoadAssetBundleComplete(null);
                        }
                    }
                }
                else
                {
                    //加载进度
                    if (OnAssetBundleCreateUpdate != null)
                    {
                        OnAssetBundleCreateUpdate(m_CurrAssetBundleCreateRequest.progress);
                    }
                }
            }
        }

        #endregion
    }
}