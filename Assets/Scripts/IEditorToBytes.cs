using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEditorToBytes
{
   byte[] ToBytes();

    void CreateByteFile();
    
    bool CheckFile();

    void CreateTextFile();

    void CreateCSFile();

    void CreateDBModel();

    void CreateEntity();
}
