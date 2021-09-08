using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public class TestSocket : MonoBehaviour
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
                Debug.Log("开始链接");
                GameEntry.Socket.ConnectToMainSocket("127.0.0.1",1304);
            }
            if (Input.GetKeyUp(KeyCode.B))
            {
                //C2GWS_RegClientProto proto = new C2GWS_RegClientProto();
                //proto.AccountId = 1;
                //GameEntry.Socket.SendMsg(proto);
            }
        }
    }
}