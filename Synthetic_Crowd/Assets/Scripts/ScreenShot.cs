using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

 public class ScreenShot : MonoBehaviour {   
    public static string IndexedFilename(string stub, string extension){
        int index = 0;
        string filename = null;
        do{
            index++;
            filename = string.Format("{0}{1}.{2}", stub, index, extension);
        } while(File.Exists(filename));
        return filename;
    }

    public static void Save(string path, string content){
        File.AppendAllText(path, content);
    }

    public static void TakeScreenshot(Camera cam){
        int captureWidth = (int)(cam.pixelWidth*2.25);
        int captureHeight = (int)(cam.pixelHeight*2.25);
        // Take screenshot
        RenderTexture rt = new RenderTexture(captureWidth, captureHeight, 24);
        cam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        cam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // Get updated filename and save screenshot
        byte[] bytes = screenShot.EncodeToJPG(); 
        string stub = Application.dataPath+"/Savings/Screenshots/IMG_";
        string filename = IndexedFilename(stub,"jpg");
        System.IO.File.WriteAllBytes(filename, bytes);

        Debug.Log(string.Format("{0} took screenshot", cam.name));
        
        // Save head positions
        int size = SyntheticCrowdGenerator.headPositions.Count;
        string stb = Application.dataPath+"/Savings/Positions/IMG_";
        string fn = IndexedFilename(stb,"txt");

        for(int i=0; i<size; i++){
            Vector3 pos = cam.WorldToViewportPoint(SyntheticCrowdGenerator.headPositions[i]);
            if( (pos.x >= 0 && pos.x <= 1) && (pos.y >= 0 && pos.y <= 1) && (pos.z > 0) ){
                Save(fn, 
                    ((int)(pos.x*captureWidth)).ToString()+" "+
                    ((int)(pos.y*captureHeight)).ToString()+"\n");
            }
        }
        Debug.Log("(Saved head positions)");
    }
}
