using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

[CreateAssetMenu]
public class ParamsSettings : ScriptableObject
{
    [BoxGroup("InitUrl")]
    public string WebAccountUrl;

    [BoxGroup("InitUrl")]
    public string TestWebAccountUrl;

    [BoxGroup("InitUrl")]
    public bool IsTest;

    [BoxGroup("GeneralParams")]
    [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
    [HideLabel]
    public GeneralParamData[] GeneralParams;

    [BoxGroup("GradeParams")]
    [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
    [HideLabel]
    public GradeParamData[] GradeParams;

    /// <summary>
    /// 常规参数
    /// </summary>
    [Serializable]
    public class GeneralParamData
    {
        /// <summary>
        /// 参数Key
        /// </summary>
        [TableColumnWidth(160, Resizable = false)]
        public string Key;

        /// <summary>
        /// 参数值
        /// </summary>
        public int Value;
    }

    /// <summary>
    /// 设备等级
    /// </summary>
    public enum DeviceGrade
    {
        Low = 0,
        Midle = 1,
        High = 2
    }

    private int m_LenGradeParams = 0;
    /// <summary>
    /// 根据Key和设备等级获取参数
    /// </summary>
    /// <param name="http_Retry"></param>
    /// <param name="currDeviceGrade"></param>
    /// <returns></returns>
    public int GetGradeParamData(string key, DeviceGrade grade)
    {
        m_LenGradeParams = GradeParams.Length;
        for (int i = 0; i < m_LenGradeParams; i++)
        {
            GradeParamData gradeParamData = GradeParams[i];
            if (gradeParamData.Key.Equals(key,StringComparison.CurrentCultureIgnoreCase))
            {
                return gradeParamData.GetValueByGrade(grade);
            }
        }
        GameEntry.LogError("GetGradeParamData Fail key= {0}", key);
        return 0;
    }

    /// <summary>
    /// 等级参数数据
    /// </summary>
    [Serializable]
    public class GradeParamData
    {
        /// <summary>
        /// 参数Key
        /// </summary>
        [TableColumnWidth(160, Resizable = false)]
        public string Key;

        public int LowValue;

        public int MidleValue;

        public int HighValue;

        public int GetValueByGrade(DeviceGrade grade)
        {
            switch (grade)
            {
                default:
                case DeviceGrade.Low:
                    return LowValue;
                case DeviceGrade.Midle:
                    return MidleValue;
                case DeviceGrade.High:
                    return HighValue;
            }
        }
    }
}
