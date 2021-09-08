using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public class ConstDefine
    {
        /// <summary>
        /// 版本文件名称
        /// </summary>
        public const string VersionFileName = "VersionFile.bytes";

        /// <summary>
        /// 资源版本号
        /// </summary>
        public const string ResourceVersion = "ResourceVersion";

        /// <summary>
        /// 资源信息文件名称
        /// </summary>
        public const string AssetInfoName = "AssetInfo.bytes";

        /// <summary>
        /// lua脚本资源包路径
        /// </summary>
        public const string XLuaAssetBundlePath = "download/xlualogic.assetbundle";

        /// <summary>
        /// 自定义Shader资源包路径
        /// </summary>
        public const string CusShaderAssetBundlePath = "download/cusshaders.assetbundle";

        /// <summary>
        /// 声音资源包路径
        /// </summary>
        public const string AudioAssetBundlePath = "download/audio.assetbundle";

        /// <summary>
        /// 日志预设路径
        /// </summary>
        public const string ReporterPath = "download/reporter.assetbundle";

        /// <summary>
        /// 点击按钮声音
        /// </summary>
        public const int Audio_ButtonClick = 201;

        /// <summary>
        /// UI关闭声音
        /// </summary>
        public const int Audio_UIClose = 202;

        /// <summary>
        /// Http重试
        /// </summary>
        public const string Http_Retry = "Http_Retry";

        /// <summary>
        /// 下载重试次数
        /// </summary>
        public const string Download_Retry = "Download_Retry";

        /// <summary>
        /// 下载器数量
        /// </summary>
        public const string Download_RoutineCount = "Download_RoutineCount";

        /// <summary>
        /// 
        /// </summary>
        public const string Download_FlushSize = "Download_FlushSize";

        /// <summary>
        /// 类对象池的释放间隔
        /// </summary>
        public const string Pool_ReleaseClassObjectInterval = "Pool_ReleaseClassObjectInterval";

        /// <summary>
        /// AssetBundle池的释放间隔
        /// </summary>
        public const string Pool_ReleaseAssetBundleInterval = "Pool_ReleaseAssetBundleInterval";

        /// <summary>
        /// Asset池的释放间隔
        /// </summary>
        public const string Pool_ReleaseAssetInterval = "Pool_ReleaseAssetInterval";

        /// <summary>
        /// UI池中最大数量
        /// </summary>
        public const string UI_PoolMaxCount = "UI_PoolMaxCount";

        /// <summary>
        /// UI过期时间
        /// </summary>
        public const string UI_Exprie = "UI_Exprie";

        /// <summary>
        /// UI清理间隔
        /// </summary>
        public const string UI_ClearInterval = "UI_ClearInterval";
    }                       
}                           