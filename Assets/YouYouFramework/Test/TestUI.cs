using System.Collections;
using System.Collections.Generic;
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
            GameEntry.UI.OpenUIForm(UIFormId.UI_Reg);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameEntry.UI.OpenUIForm(UIFormId.UI_Login);
        }
    }
}
