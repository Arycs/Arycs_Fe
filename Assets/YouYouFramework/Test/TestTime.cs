using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

public class TestTime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            //创建定时器
            TimeAction action = GameEntry.Time.CreateTimeAction();
            Debug.Log("创建了定时器1");
            action.Init(null, 1, 1, 2,
                () => { Debug.Log("定时器1开始运行"); },
                (int loop) => { Debug.Log("定时器1 运行中 剩余次数 = " + loop); },
                () => { Debug.Log("定时器1运行完毕"); }
            );
            action.Run();

//            Debug.Log("创建了定时器2");
//            TimeAction action2 = GameEntry.Time.CreateTimeAction();
//            action2.Init(4,1,10,() =>
//                {
//                    Debug.Log("定时器2开始运行");
//                }, 
//                (int loop) => {
//                    Debug.Log("定时器2 运行中 剩余次数 = " + loop);
//                },
//                () =>
//                {
//                    Debug.Log("定时器2运行完毕");
//                }).Run();
        }
    }
}
