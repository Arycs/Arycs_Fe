using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TestAysnc : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.X))
        {
            //关键词
            Debug.Log("开始执行异步方法");
            //第一种方式,也是常用的方式
            //Task.Factory.StartNew(TestMethod);
            TestMethodAsync();
            
            Debug.Log("异步方法结束");
        }
    }

    /// <summary>
    /// 虽然标记为 异步方法,但是如果在其中调用同步方法也会卡住主线程
    /// </summary>
    private async void TestMethodAsync()
    {
        int result = await Test1();
        
        Debug.Log("方法结果为" + result);
    }

    public async Task<int> Test1()
    {
        int ret = 0;
        for (int i = 0; i < 100; i++)
        {
            ret += i;
            await Task.Delay(1);
        }

        return ret; 
    }

    private void TestMethod()
    {
        for (int i = 0; i < 20000; i++)
        {
            Debug.Log(i);
        }
    }
}
