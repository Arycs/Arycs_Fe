using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;
public class TestEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameEntry.Event.CommonEvent.AddEventListener(CommonEventId.RegComplete, OnRegComplete);
        GameEntry.Event.CommonEvent.AddEventListener(CommonEventId.RegComplete, OnTestComplete);
    }

    private void OnRegComplete(object userdata)
    {
        Debug.Log("注册成功 :" + userdata);
    }

    private void OnTestComplete(object userdata)
    {
        Debug.Log("测试 : " + userdata);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            GameEntry.Event.CommonEvent.Dispatch(CommonEventId.RegComplete,123);
        }
    }

    private void OnDestroy()
    {
        //Debug.Log("TestEvent OnDestroy");
        GameEntry.Event.CommonEvent.RemoveEventListener(CommonEventId.RegComplete,OnRegComplete);
    }
}
