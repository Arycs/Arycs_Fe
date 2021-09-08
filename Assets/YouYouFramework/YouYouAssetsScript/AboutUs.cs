using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AboutUs : ScriptableObject
{
    [BoxGroup("AboutUs")]
    [HorizontalGroup("AboutUs/Split", 80)]
    [VerticalGroup("AboutUs/Split/Left")]
    [HideLabel,PreviewField(80,ObjectFieldAlignment.Center)]
    public Texture Icon;

    [HorizontalGroup("AboutUs/Split",LabelWidth = 70)]
    [VerticalGroup("AboutUs/Split/Right")]
    [DisplayAsString]
    [LabelText("框架名称")]
    [GUIColor(2,6,6,1)]
    public string Name = "YouYouFramework";

    //[PropertySpace] 添加一行空格
    [VerticalGroup("AboutUs/Split/Right")]
    [DisplayAsString]
    [LabelText("版本号")]
    [GUIColor(2, 6, 6, 1)]
    public string Version = "1.1";

    [VerticalGroup("AboutUs/Split/Right")]
    [DisplayAsString]
    [LabelText("作者")]
    [GUIColor(2, 6, 6, 1)]
    public string Author = "Arycs";

    [VerticalGroup("AboutUs/Split/Right")]
    [DisplayAsString]
    [LabelText("联系方式")]
    [GUIColor(2, 6, 6, 1)]
    public string Contact = "QQ : 786859650";


    [BoxGroup("Models")]
    [Title("宏设置",bold:false)]
    [HideLabel]
    [DisplayAsString]
    public string MacroSettings = "宏设置，能够方便进行全局设置";

    [PropertySpace(10)]
    [BoxGroup("Models")]
    [Title("参数设置", bold: false)]
    [HideLabel]
    [DisplayAsString]
    public string ParamsSetting = "全局参数";

}
