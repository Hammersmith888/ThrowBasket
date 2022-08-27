namespace GameBench
{
    using System.IO;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;
    public class FBSetup : ScriptableObject
    {
        public string shareDialogTitle = "Amazing Example",
            shareDialogMsg = "This is a Superb Awesome Game! Check this Out.",
            inviteDialogTitle = "Amazing Example",
            inviteDialogMsg = "Let's Play this Great Fun Game!";
        public string fbShareURI = "http://u3d.as/aRQ",
                fbSharePicURI = "http://i.imgur.com/fPs7tnx.png";

        int _inviteFriendsCount = 100;

        public int InviteFriendsCount
        {
            set { _inviteFriendsCount = Mathf.Clamp(value, 1, 5000); }
            get { return _inviteFriendsCount; }
        }

        const string assetDataPath = "Assets/BasketBallPro/Prefabs/Resources/";
        const string assetName = "FBSetup";
        const string assetExt = ".asset";
        private static FBSetup instance;
        public static FBSetup Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load(assetName) as FBSetup;
                    if (instance == null)
                    {
                        instance = CreateInstance<FBSetup>();
#if UNITY_EDITOR
                        if (!Directory.Exists(assetDataPath))
                        {
                            Directory.CreateDirectory(assetDataPath);
                        }
                        string fullPath = assetDataPath + assetName + assetExt;
                        AssetDatabase.CreateAsset(instance, fullPath);
                        AssetDatabase.SaveAssets();
#endif
                    }
                }
                return instance;
            }
        }
        public static void DirtyEditor()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(Instance);
#endif
        }
#if UNITY_EDITOR
        [MenuItem("Tools/**FBManager**")]
        public static void Edit()
        {
            Selection.activeObject = Instance;
        }
#endif
    }
}