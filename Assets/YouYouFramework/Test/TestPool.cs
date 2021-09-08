using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using YouYou;

public class TestPool : MonoBehaviour
{
    public Transform trans1;
    public Transform trans2;

    public Transform[] arr;

    public int index = 0; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
//        if (Input.GetKeyUp(KeyCode.A))
//        {
//            StringBuilder sbr = GameEntry.Pool.DequeueClassObject<StringBuilder>();
//            sbr.Length = 0;
//            sbr.Append("123");
//            
//            Debug.Log(sbr.ToString());
//            
//            GameEntry.Pool.EnqueueClassObject(sbr);
//        }
//
//        if (Input.GetKeyDown(KeyCode.S))
//        {
//            StringBuilder sbr = GameEntry.Pool.DequeueClassObject<StringBuilder>();
//            Debug.Log(sbr.ToString());
//        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            for (int i = 0; i < 20; i++)
            {
                GameEntry.Pool.GameObjectSpawn(1, (Transform instance) =>
                {
                    instance.transform.localPosition += new Vector3(0, 0, i * 2);
                    instance.gameObject.SetActive(true);
                    arr[i] = instance;
                });
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DeSpawn(1, arr[index]);
            index++;
        }
    }

//    private IEnumerator CreateObj()
//    {
//        for (int i = 0; i < 20; i++)
//        {
//            yield return  new WaitForSeconds(0.5f);
//            
//            GameEntry.Pool.GameObjectSpawn(1,trans1, (Transform instance) =>
//            {
//                instance.transform.localPosition += new Vector3(0, 0, i * 2);
//                instance.gameObject.SetActive(true);
//                StartCoroutine(DeSpawn(1, instance));
//            });
//            
//            GameEntry.Pool.GameObjectSpawn(2,trans2, (Transform instance) => 
//            { 
//                instance.transform.localPosition += new Vector3(0, 0, i * 5);
//                instance.gameObject.SetActive(true);
//                StartCoroutine(DeSpawn(2, instance));
//            });
//        }
//    }

    private void DeSpawn(byte poolId ,Transform instance)
    {
        GameEntry.Pool.GameObjectDespawn(poolId,instance);        
    }
}
