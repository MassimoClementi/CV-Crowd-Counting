using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

 public class ScreenShot : MonoBehaviour {
    //public int resWidth = 2550; 
    //public int resHeight = 3300;
    
    string IndexedFilename(string stub, string extension){
        int index = 0;
        string filename = null;
        do{
            index++;
            filename = string.Format("{0}{1}.{2}", stub, index, extension);
        } while(File.Exists(filename));
        return filename;
    }

    void Save(string path, string content){
        File.AppendAllText(path, content);
    }
  
    void LateUpdate() {
        int resWidth = GetComponent<Camera>().pixelWidth;
        int resHeight = GetComponent<Camera>().pixelHeight;

        if (Input.GetKeyDown("k")) {
            // Take screenshot
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            GetComponent<Camera>().targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            GetComponent<Camera>().Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            GetComponent<Camera>().targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);

            // Get updated filename and save
            byte[] bytes = screenShot.EncodeToPNG();
            //byte[] bytes = screenShot.EncodeToJPG(); 
            string stub = Application.dataPath+"/Savings/Screenshots/IMG_";
            string filename = IndexedFilename(stub,"png"); //"png" or "jpg"
            System.IO.File.WriteAllBytes(filename, bytes);

            Debug.Log(string.Format("Took screenshot to: {0}", filename));

            // Save head positions
            int size = SyntheticCrowdGenerator.headPositions.Count;
            string stb = Application.dataPath+"/Savings/Positions/IMG_";
            string fn = IndexedFilename(stb,"txt");

            for(int i=0; i<size; i++){
                Vector3 pos = GetComponent<Camera>().WorldToViewportPoint(SyntheticCrowdGenerator.headPositions[i]);
                if( (pos.x >= 0 && pos.x <= 1) && (pos.y >= 0 && pos.y <= 1) && (pos.z > 0) ){
                    Save(fn, 
                        ((int)(pos.x*resWidth)).ToString()+" "+
                        ((int)(pos.y*resHeight)).ToString()+"\n");
                }
            }
            Debug.Log("Saved head positions");
        }
    }
}
