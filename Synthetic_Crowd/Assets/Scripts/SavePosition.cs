using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class SavePosition
{
    public static void Save(string path, string content){
        File.AppendAllText(Application.dataPath+"/"+path, content);
    }
}
