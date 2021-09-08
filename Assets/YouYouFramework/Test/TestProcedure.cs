using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

public class TestProcedure : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    private IEnumerator Test1(VarInt a)
    {
        //保留
        a.Retain();
        yield return new WaitForSeconds(5);
        Debug.Log("a = " + a.Value);
        Debug.Log("在协程中释放");
        //释放
        a.Release();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
//
//            List<ChapterEntity> lst = dr.GetList();
//            int len = lst.Count;
//            for (int i = 0; i < len; i++)
//            {
//                ChapterEntity entity = lst[i];
//                Debug.Log("Id" + entity.Id + "Name : " + entity.ChapterName);
//            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            GameEntry.Procedure.ChangeState(ProcedureState.Preload);
        }
    }
}