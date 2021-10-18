using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;
using YouYou;

public class TestUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameEntry.UI.OpenUIForm(UIFormId.UI_Talk);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            MapEventInfo info = new MapEventInfo();
            GameEntry.Resource.ResourceLoaderManager.LoadMainAsset(AssetCategory.MapEventInfo,"Assets/Download/MapConfig/Map01.asset",(
                Resources =>
                {
                    info = Resources.Target as MapEventInfo;
                }));

            MapEventInfo temp = info;
        }
    }
}
