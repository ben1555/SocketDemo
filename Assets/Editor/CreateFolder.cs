/********************************************************************************
** Auth： 黎建斌
** Date： 2016-05-31 14:54
** Desc： 类功能描述
** Ver ： V1.0.0
*********************************************************************************/



namespace StoryProcess
{
    using UnityEditor;
    using System.IO;

    public class CreateFolder : Editor{

        //private static char pathSymbol = Path.DirectorySeparatorChar;
        private static readonly string assest = "Assets" + Path.DirectorySeparatorChar;
        private static readonly string[] folders = new string[] {
                        
            assest + "Test",
            assest + "Audios",
            assest + "Scenes",
            assest + "Scripts",
            assest + "Prefabs",
            assest + "Plugins",
            assest + "Textures",
            assest + "JsonFiles",
            assest + "Resources",
            assest + "Extensions",
            assest + "StreamingAssets"
        };


        [MenuItem("Assets/Create All Folder")]
        private static void CreateAllFolder() {
            for (int i = 0; i < folders.Length; i++) {
                if (Directory.Exists(folders[i])) continue;
                Directory.CreateDirectory(folders[i]);
            }
            AssetDatabase.Refresh();
        }

    }

}


