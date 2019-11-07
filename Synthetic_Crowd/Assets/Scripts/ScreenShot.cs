using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class ScreenShot : MonoBehaviour {
    //public int resWidth = 2550; 
    //public int resHeight = 3300;
 
    public static string ScreenShotName(int width, int height) {
        return string.Format("{0}/Savings/Screenshots/screen_{1}x{2}_{3}.png", 
                            Application.dataPath, 
                            width, height, 
                            System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
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
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);

            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);

            Debug.Log(string.Format("Took screenshot to: {0}", filename));

            // Save head positions
            int size = SyntheticCrowdGenerator.headPositions.Count;
            string fn = "Savings/Positions/pos_"+System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")+".txt";
            
            for(int i=0; i<size; i++){
                Vector3 pos = GetComponent<Camera>().WorldToViewportPoint(SyntheticCrowdGenerator.headPositions[i]);
                if( (pos.x >= 0 && pos.x <= 1) && (pos.y >= 0 && pos.y <= 1) && (pos.z > 0) ){
                    SavePosition.Save(fn, 
                                    ((int)(pos.x*resWidth)).ToString()+" "+
                                    ((int)(pos.y*resHeight)).ToString()+"\n");
                }
            }
            Debug.Log("Saved head positions");
        }
    }
}
