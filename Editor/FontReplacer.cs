using UnityEngine; 
using UnityEditor; 
using TMPro;

namespace TMPFontReplacer
{
    public class FontReplacer : EditorWindow  
    { 
        string searchDir = "";
        private DefaultAsset targetFolder;
        private TMP_FontAsset fontAsset; 
     
        [MenuItem("Tools/FontReplecer")] 
        public static void GetWindow() 
        { 
            EditorWindow.GetWindow(typeof(FontReplacer)).Show(); 
        }	 
     
        void OnGUI() 
        { 
            this.targetFolder = ( DefaultAsset )EditorGUILayout.ObjectField( "フォルダ", this.targetFolder, typeof( DefaultAsset ), false );
            if ( this.targetFolder != null ){
                string path = AssetDatabase.GetAssetOrScenePath( this.targetFolder );
                //フォルダ以外が選択されていたらリセット
                string[] folderList = path.Split( '/' );
                if (folderList[folderList.Length - 1].Contains("."))
                {
                    this.targetFolder = null;
                    searchDir = "";
                }
                else
                {
                    searchDir = path;
                }
    
            }
    
            this.fontAsset = EditorGUILayout.ObjectField("TMPFont", this.fontAsset, typeof(TMP_FontAsset), true) as TMP_FontAsset; 
            // フォントとディレクトリが空ならここで止める
            if (this.fontAsset == null || string.IsNullOrEmpty(searchDir)) 
            { 
                return; 
            } 
     
            if (GUILayout.Button("フォント入れ替え")) 
            { 
                ReplaceTMPFont (this.fontAsset); 
            } 
        } 
     
        private void ReplaceTMPFont(TMP_FontAsset font) 
        {
            string[] dir = new string[]{searchDir};
            string[] assets = AssetDatabase.FindAssets("", dir); 
     
            bool isSave = false; 
            for (int i = 0; i < assets.Length; i++) 
            { 
                //AssetのGUIDパスを取得
                string guid = assets [i]; 
                string guidPath = AssetDatabase.GUIDToAssetPath(guid); 
                //プログレスバーを表示
                EditorUtility.DisplayProgressBar(title, guidPath, (float)i / (float)assets.Length); 
     
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(guidPath); 
                if(go != null) 
                { 
                    //TMPProの配列を取得
                    TextMeshProUGUI[] textList = go.GetComponentsInChildren <TextMeshProUGUI>(true); 
                    for (int j = 0; j < textList.Length; j++) 
                    { 
                        TextMeshProUGUI text = textList[j]; 
                        if (text != null) 
                        {
                            //fontを入れ替え
                            text.font = font; 
                            EditorUtility.SetDirty(text); 
                        } 
                    } 
                    isSave = true; 
                } 
            } 
            if(isSave) 
            { 
                AssetDatabase.SaveAssets(); 
            } 
            EditorUtility.ClearProgressBar(); 
        } 
    }
}