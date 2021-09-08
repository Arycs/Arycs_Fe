using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using YouYou;

[CreateAssetMenu]
public class AssetBundleSettings : ScriptableObject
{
    public enum CusBuildTarget
    {
        Windows,
        Android,
        IOS
    }

    [HorizontalGroup("Common", LabelWidth = 70)]
    [VerticalGroup("Common/Left")]
    [LabelText("资源版本号")]
    public string ResourceVersion = "1.0.1";

    [PropertySpace(10)]
    [VerticalGroup("Common/Left")]
    [LabelText("目标平台")]
    public CusBuildTarget CurrBuildTarget;

    public BuildTarget GetBuildTarget()
    {
        switch (CurrBuildTarget)
        {
            default:
            case CusBuildTarget.Windows:
                return BuildTarget.StandaloneWindows;
            case CusBuildTarget.Android:
                return BuildTarget.Android;
            case CusBuildTarget.IOS:
                return BuildTarget.iOS;
        }
    }

    [PropertySpace(10)]
    [VerticalGroup("Common/Left")]
    [LabelText("参数")]
    public BuildAssetBundleOptions Options;

    [VerticalGroup("Common/Right")]
    [Button(ButtonSizes.Medium)]
    [LabelText("更新版本号")]
    public void UpdateResourceVersion()
    {
        string version = ResourceVersion;
        string[] arr = version.Split('.');

        int shortVersion = 0;
        int.TryParse(arr[2], out shortVersion);
        version = string.Format("{0}.{1}.{2}", arr[0], arr[1], ++shortVersion);
        ResourceVersion = version;
    }

    [PropertySpace(5)]
    [VerticalGroup("Common/Right")]
    [Button(ButtonSizes.Medium)]
    [LabelText("清空资源包")]
    public void ClearAssetBundle()
    {
        if (Directory.Exists(Application.streamingAssetsPath + "/AssetBundles/"))
        {
            Directory.Delete(Application.streamingAssetsPath + "/AssetBundles/", true);
        }
        EditorUtility.DisplayDialog("", "清理完毕", "确定");
    }

    /// <summary>
    /// 要收集的资源包
    /// </summary>
    List<AssetBundleBuild> builds = new List<AssetBundleBuild>();

    [PropertySpace(5)]
    [VerticalGroup("Common/Right")]
    [Button(ButtonSizes.Medium)]
    [LabelText("打包")]
    public void BuildAssetBundle()
    {
        builds.Clear();
        int len = Datas.Length;
        for (int i = 0; i < len; i++)
        {
            AssetBundleData assetBundleData = Datas[i];
            int lenPath = assetBundleData.Path.Length;
            for (int j = 0; j < lenPath; j++)
            {
                //打包这个路径
                string path = assetBundleData.Path[j];
                BuildAssetBundleForPath(path, assetBundleData.Overall);
            }
        }

        if (!Directory.Exists(TempPath))
        {
            Directory.CreateDirectory(TempPath);
        }

        if (builds.Count == 0)
        {
            Debug.Log("未找到需要打包内容");
            return;
        }

        Debug.Log("builds count=" + builds.Count);

        BuildPipeline.BuildAssetBundles(TempPath, builds.ToArray(), Options, GetBuildTarget());

        Debug.Log("临时资源包打包完毕");

        CopyFile(TempPath);

        Debug.Log("拷贝到输出目录完毕");

        AssetBundleEncrypt();
        Debug.Log("资源包加密完成");

        CreateDependenciesFile();
        Debug.Log("生成依赖关系文件完毕");

        CreateVersionFile();
        Debug.Log("生成版本文件完毕");
    }

    private void CreateVersionFile()
    {
        string path = OutPath;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string strVersionFilePath = path + "/VersionFile.txt"; //版本文件路径

        //如果版本文件存在 则删除
        IOUtil.DeleteFile(strVersionFilePath);

        StringBuilder sbContent = new StringBuilder();

        DirectoryInfo directory = new DirectoryInfo(path);

        //拿到文件夹下所有文件
        FileInfo[] arrFiles = directory.GetFiles("*", SearchOption.AllDirectories);
        sbContent.AppendLine(ResourceVersion);

        for (int i = 0; i < arrFiles.Length; i++)
        {
            FileInfo file = arrFiles[i];

            if (file.Extension == ".manifest")
            {
                continue;
            }

            string fullName = file.FullName; //全名 包含路径扩展名

            //相对路径
            string name = fullName.Substring(fullName.IndexOf(CurrBuildTarget.ToString()) +
                                             CurrBuildTarget.ToString().Length + 1);

            string md5 = EncryptUtil.GetFileMD5(fullName); //文件的MD5
            if (md5 == null)
                continue;

            string size = file.Length.ToString(); //文件大小

            bool isFirstData = false; //是否初始数据
            bool isEncrypt = false; //是否加密
            bool isBreak = false;

            for (int j = 0; j < Datas.Length; j++)
            {
                foreach (string xmlPath in Datas[j].Path)
                {
                    string tempPath = xmlPath;

                    name = name.Replace("\\", "/");
                    if (name.IndexOf(tempPath, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        isFirstData = Datas[j].IsFirstData;
                        isEncrypt = Datas[j].IsEncrypt;
                        isBreak = true;
                        break;
                    }
                }

                if (isBreak)
                    break;
            }

            //本地数据表 和 lua脚本 是初始数据
            if (name.IndexOf("DataTable") != -1 || name.IndexOf("xLuaLogic") != -1)
            {
                isFirstData = true;
            }

            string strLine = string.Format("{0}|{1}|{2}|{3}|{4}", name, md5, size, isFirstData ? 1 : 0,
                isEncrypt ? 1 : 0);
            sbContent.AppendLine(strLine);
        }

        IOUtil.CreateTextFile(strVersionFilePath, sbContent.ToString());

        MMO_MemoryStream ms = new MMO_MemoryStream();
        string str = sbContent.ToString().Trim();
        string[] arr = str.Split('\n');
        int len = arr.Length;
        ms.WriteInt(len);
        for (int i = 0; i < len; i++)
        {
            if (i == 0)
            {
                ms.WriteUTF8String(arr[i]);
            }
            else
            {
                string[] arrInner = arr[i].Split('|');
                ms.WriteUTF8String(arrInner[0]);
                ms.WriteUTF8String(arrInner[1]);
                ms.WriteULong(ulong.Parse(arrInner[2]));
                ms.WriteByte(byte.Parse(arrInner[3]));
                ms.WriteByte(byte.Parse(arrInner[4]));
            }
        }

        string filePath = path + "/VersionFile.bytes"; //版本文件路径
        byte[] buffer = ms.ToArray();
        buffer = ZlibHelper.CompressBytes(buffer);
        FileStream fs = new FileStream(filePath, FileMode.Create);
        fs.Write(buffer, 0, buffer.Length);
        fs.Close();
        fs.Dispose();
        Debug.Log("创建版本文件成功");
    }

    #region CreateDependenciesFile 创建依赖文件
    /// <summary>
    /// 创建依赖文件
    /// </summary>
    private void CreateDependenciesFile()
    {
        //临时列表
        List<AssetEntity> tempList = new List<AssetEntity>();

        int len = Datas.Length;
        //循环设置文件夹包括子文件里边的项
        for (int i = 0; i < len; i++)
        {
            AssetBundleData assetBundleData = Datas[i]; //取到一个节点

            for (int j = 0; j < assetBundleData.Path.Length; j++)
            {
                string path = Application.dataPath + "/" + assetBundleData.Path[j];
                //Debug.LogError("path=" + path);
                CollectFileInfo(tempList, path);
            }
        }

        //资源列表
        List<AssetEntity> assetList = new List<AssetEntity>();
        len = tempList.Count;

        for (int i = 0; i < len; i++)
        {
            AssetEntity entity = tempList[i];

            AssetEntity newEntity = new AssetEntity();
            newEntity.Category = entity.Category;
            newEntity.AssetName = entity.AssetFullName.Substring(entity.AssetFullName.LastIndexOf("/") + 1);
            newEntity.AssetName = newEntity.AssetName.Substring(0, newEntity.AssetName.LastIndexOf("."));
            newEntity.AssetFullName = entity.AssetFullName;
            newEntity.AssetBundleName = entity.AssetBundleName;
            assetList.Add(newEntity);

            newEntity.DependsAssetList = new List<AssetDependsEntity>();

            string[] arr = AssetDatabase.GetDependencies(entity.AssetFullName, true);
            foreach (string str in arr)
            {
                if (!str.Equals(newEntity.AssetFullName, StringComparison.CurrentCultureIgnoreCase) &&
                    GetIsAsset(tempList, str))
                {
                    AssetDependsEntity assetDepends = new AssetDependsEntity();
                    assetDepends.Category = GetAssetCategory(str);
                    assetDepends.AssetFullName = str;

                    //把依赖资源 加入到依赖资源列表
                    newEntity.DependsAssetList.Add(assetDepends);
                }
            }
        }

        //生成一个Json文件
        string targetPath = OutPath;
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        string strJsonFilePath = targetPath + "/AssetInfo.json"; //版本文件路径
        IOUtil.CreateTextFile(strJsonFilePath, LitJson.JsonMapper.ToJson(assetList));
        Debug.LogError("生成AssetInfo.json完毕");

        MMO_MemoryStream ms = new MMO_MemoryStream();

        //生成二进制文件
        len = assetList.Count;
        ms.WriteInt(len);
        for (int i = 0; i < len; i++)
        {
            AssetEntity entity = assetList[i];
            ms.WriteByte((byte)entity.Category);
            ms.WriteUTF8String(entity.AssetFullName);
            ms.WriteUTF8String(entity.AssetBundleName);

            if (entity.DependsAssetList != null)
            {
                //添加依赖资源
                int depLen = entity.DependsAssetList.Count;
                ms.WriteInt(depLen);
                for (int j = 0; j < depLen; j++)
                {
                    AssetDependsEntity assetDepends = entity.DependsAssetList[j];
                    ms.WriteByte((byte)assetDepends.Category);
                    ms.WriteUTF8String(assetDepends.AssetFullName);
                }
            }
            else
            {
                ms.WriteInt(0);
            }
        }

        string filePath = targetPath + "/AssetInfo.bytes"; //版本文件路径
        byte[] buffer = ms.ToArray();
        buffer = ZlibHelper.CompressBytes(buffer);
        FileStream fs = new FileStream(filePath, FileMode.Create);
        fs.Write(buffer, 0, buffer.Length);
        fs.Close();
        fs.Dispose();
        Debug.LogError("生成AssetBundleInfo.bytes 完成");
    }

    /// <summary>
    /// 判断某个资源是否存在于资源列表
    /// </summary>
    /// <param name="tempLst"></param>
    /// <param name="assetFullName"></param>
    /// <returns></returns>
    private bool GetIsAsset(List<AssetEntity> tempLst, string assetFullName)
    {
        int len = tempLst.Count;
        for (int i = 0; i < len; i++)
        {
            AssetEntity entity = tempLst[i];
            if (entity.AssetFullName.Equals(assetFullName, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 收集文件信息
    /// </summary>
    /// <param name="tempLst"></param>
    /// <param name="folderPath"></param>
    private void CollectFileInfo(List<AssetEntity> tempLst, string folderPath)
    {
        DirectoryInfo directory = new DirectoryInfo(folderPath);

        //拿到文件夹下所有文件
        FileInfo[] arrFiles = directory.GetFiles("*", SearchOption.AllDirectories);

        for (int i = 0; i < arrFiles.Length; i++)
        {
            FileInfo file = arrFiles[i];
            if (file.Extension == ".meta")
                continue;
            if (file.FullName.IndexOf(".idea") != -1)
                continue;

            //绝对路径
            string filePath = file.FullName;
            //Debug.LogError("filePath==" + filePath);
            int index = filePath.IndexOf("Assets\\", StringComparison.CurrentCultureIgnoreCase);

            //路径
            string newPath = filePath.Substring(index);

            AssetEntity entity = new AssetEntity();
            entity.AssetFullName = newPath.Replace("\\", "/");
            entity.Category = GetAssetCategory(newPath.Replace(file.Name, "")); // 去掉文件名,只保留路径
            entity.AssetBundleName = (GetAssetBundleName(entity.AssetFullName) + ".assetbundle").ToLower();
            tempLst.Add(entity);

        }
    }

    /// <summary>
    /// 获取资源分类
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private AssetCategory GetAssetCategory(string filePath)
    {
        AssetCategory category = AssetCategory.Audio;

        if (filePath.IndexOf("Audio") != -1)
        {
            category = AssetCategory.Audio;
        }
        else if (filePath.IndexOf("CusShaders") != -1)
        {
            category = AssetCategory.CusShaders;
        }
        else if (filePath.IndexOf("DataTable") != -1)
        {
            category = AssetCategory.DataTable;
        }
        else if (filePath.IndexOf("Effect") != -1)
        {
            category = AssetCategory.EffectSources;
        }
        else if (filePath.IndexOf("Role") != -1)
        {
            category = AssetCategory.RolePrefab;
        }
        else if (filePath.IndexOf("RoleSources") != -1)
        {
            category = AssetCategory.RoleSources;
        }
        else if (filePath.IndexOf("Scenes") != -1)
        {
            category = AssetCategory.Scenes;
        }
        else if (filePath.IndexOf("UIFont") != -1)
        {
            category = AssetCategory.UIFont;
        }
        else if (filePath.IndexOf("UIPrefab") != -1)
        {
            category = AssetCategory.UIPrefab;
        }
        else if (filePath.IndexOf("UIRes") != -1)
        {
            category = AssetCategory.UIRes;
        }
        else if (filePath.IndexOf("xLuaLogic") != -1)
        {
            category = AssetCategory.xLuaLogic;
        }

        return category;
    }

    private string GetAssetBundleName(string assetFullName)
    {
        int len = Datas.Length;
        //循环设置文件夹包括子文件里边的项
        for (int i = 0; i < len; i++)
        {
            AssetBundleData assetBundleData = Datas[i];
            for (int j = 0; j < assetBundleData.Path.Length; j++)
            {
                if (assetFullName.IndexOf(assetBundleData.Path[j], StringComparison.CurrentCultureIgnoreCase) > -1)
                {
                    if (assetBundleData.Overall)
                    {
                        //文件夹是个整包 则返回这个特文件夹名字
                        return assetBundleData.Path[j].ToLower();
                    }
                    else
                    {
                        //零散资源
                        return assetFullName.Substring(0, assetFullName.LastIndexOf('.')).ToLower().Replace("assets/", "");
                    }
                }
            }
        }
        return null;
    }
    #endregion

    #region AssetBundleEncrypt 资源包加密
    private void AssetBundleEncrypt()
    {
        int len = Datas.Length;
        for (int i = 0; i < len; i++)
        {
            AssetBundleData assetBundleData = Datas[i];
            if (assetBundleData.IsEncrypt)
            {
                //如果需要加密
                for (int j = 0; j < assetBundleData.Path.Length; j++)
                {
                    string path = OutPath + "/" + assetBundleData.Path[j];
                    if (assetBundleData.Overall)
                    {
                        //不是遍历文件夹打包 说明这个路径就是一个包
                        path = path + ".assetbundle";
                        AssetBundleEncryptFile(path);
                    }
                    else
                    {
                        AssetBundleEncryptFolder(path);
                    }
                }
            }
        }
    }

    private void AssetBundleEncryptFolder(string folderPath, bool isDelete = false)
    {
        DirectoryInfo directory = new DirectoryInfo(folderPath);
        //拿到文件夹下所有文件
        FileInfo[] arrFiles = directory.GetFiles("*", SearchOption.AllDirectories);

        foreach (FileInfo file in arrFiles)
        {
            AssetBundleEncryptFile(file.FullName, isDelete);
        }
    }

    private void AssetBundleEncryptFile(string filePath, bool isDelete = false)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        byte[] buffer = null;

        using (FileStream fs = new FileStream(filePath, FileMode.Open))
        {
            buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
        }

        buffer = SecurityUtil.Xor(buffer);
        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            fs.Write(buffer, 0, buffer.Length);
            fs.Flush();
        }
    }
    #endregion

    #region CopyFile 拷贝文件到正式目录
    /// <summary>
    /// 拷贝文件到正式目录
    /// </summary>
    /// <param name="oldPath"></param>
    private void CopyFile(string oldPath)
    {
        if (Directory.Exists(OutPath))
        {
            Directory.Delete(OutPath, true);
        }

        IOUtil.CopyDirectory(oldPath, OutPath);
        DirectoryInfo directory = new DirectoryInfo(OutPath);

        //拿到文件夹下所有文件
        FileInfo[] arrFiles = directory.GetFiles("*.y", SearchOption.AllDirectories);
        int len = arrFiles.Length;
        for (int i = 0; i < len; i++)
        {
            FileInfo file = arrFiles[i];
            File.Move(file.FullName, file.FullName.Replace(".ab.y", ".assetbundle"));
        }

    }
    #endregion

    #region BuildAssetBundleForPath 根据路径打包资源
    /// <summary>
    /// 根据路径打包资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="overall"></param>
    private void BuildAssetBundleForPath(string path, bool overall)
    {
        // Application.dataPath => 当前项目的Assets文件夹
        string fullPath = Application.dataPath + "/" + path;
        //Debug.Log("fullPath" + fullPath);
        //1. 拿到文件夹下所有文件
        DirectoryInfo directory = new DirectoryInfo(fullPath);

        //拿到文件夹下所有文件
        FileInfo[] arrFiles = directory.GetFiles("*", SearchOption.AllDirectories);

        //Debug.LogError("arrFile=" + arrFile.Length);
        if (overall)
        {
            //打成一个资源包
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = path + ".ab";
            build.assetBundleVariant = "y";
            string[] arr = GetValidateFiles(arrFiles);
            build.assetNames = arr;
            builds.Add(build);
        }
        else
        {
            //每个文件打成一个包
            string[] arr = GetValidateFiles(arrFiles);
            for (int i = 0; i < arr.Length; i++)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = arr[i].Substring(0, arr[i].LastIndexOf(".")).Replace("Assets/", "") + ".ab";
                build.assetBundleVariant = "y";
                build.assetNames = new string[] { arr[i] };

                //Debug.LogError("assetBundleName==" + build.assetBundleName);
                builds.Add(build);
            }
        }
    }
    #endregion

    /// <summary>
    /// 检查合法文件
    /// </summary>
    /// <param name="arrFiles"></param>
    /// <returns></returns>
    private string[] GetValidateFiles(FileInfo[] arrFiles)
    {
        List<string> lst = new List<string>();

        int len = arrFiles.Length;
        for (int i = 0; i < len; i++)
        {
            FileInfo file = arrFiles[i];
            if (!file.Extension.Equals(".meta", StringComparison.CurrentCultureIgnoreCase))
            {
                lst.Add("Assets" + file.FullName.Replace("\\", "/").Replace(Application.dataPath, ""));
            }
        }
        return lst.ToArray();
    }


    #region TempPath OutPath
    /// <summary>
    /// 临时目录
    /// </summary>
    public string TempPath
    {
        get
        {
            return Application.dataPath + "/../" + AssetBundleSavePath + "/" + ResourceVersion + "_Temp/" + CurrBuildTarget;
        }
    }

    public string OutPath
    {
        get
        {
            return TempPath.Replace("_Temp", "");
        }
    }
    #endregion

    [LabelText("资源包保存路径")]
    [FolderPath]
    /// <summary>
    /// 资源包保存路径
    /// </summary>
    public string AssetBundleSavePath;

    [LabelText("勾选进行编辑")]
    public bool IsCanEditor;

    [EnableIf("IsCanEditor")]
    [BoxGroup("AssetBundleSettings")]
    public AssetBundleData[] Datas;

    [Serializable]
    public class AssetBundleData
    {
        [LabelText("名称")]
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;

        [LabelText("文件夹为一个资源")]
        /// <summary>
        /// 文件夹为一个资源
        /// </summary>
        public bool Overall;

        [LabelText("是否为初始资源")]
        /// <summary>
        /// 是否为初始资源
        /// </summary>
        public bool IsFirstData;

        [LabelText("是否加密")]
        /// <summary>
        /// 是否加密
        /// </summary>
        public bool IsEncrypt;

        [FolderPath(ParentFolder = "Asset")]
        /// <summary>
        /// 路径
        /// </summary>
        public string[] Path;

    }
}
