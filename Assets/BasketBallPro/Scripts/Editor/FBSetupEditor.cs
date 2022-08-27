namespace GameBench
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;

    [CustomEditor(typeof(FBSetup))]
    public class FBSetupEditor : Editor
    {
        private FBSetup instance;
        public override void OnInspectorGUI()
        {
            instance = (FBSetup)target;
            EditorGUILayout.Space();
            CenterTitle("Settings For Facebook");
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            instance.InviteFriendsCount = EditorGUILayout.IntField("Total Friends To Invite", instance.InviteFriendsCount);
            EditorGUILayout.Space();
            instance.shareDialogTitle = EditorGUILayout.TextField("Share Dialog Title", instance.shareDialogTitle);
            instance.shareDialogMsg = EditorGUILayout.TextField("Share Dialog Message", instance.shareDialogMsg);
            instance.fbShareURI = EditorGUILayout.TextField("URL For FB Share", instance.fbShareURI);
            instance.fbSharePicURI = EditorGUILayout.TextField("Pic URL For FB Share", instance.fbSharePicURI);

            EditorGUILayout.Space();
            instance.inviteDialogTitle = EditorGUILayout.TextField("Invite Dialog Title", instance.inviteDialogTitle);
            instance.inviteDialogMsg = EditorGUILayout.TextField("Invite Dialog Message", instance.shareDialogMsg);
            
            FBSetup.DirtyEditor();
        }

        public static void CenterTitle(string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(text, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public static void DrawLine()
        {
            EditorGUI.indentLevel--;
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
            EditorGUI.indentLevel++;
        }
    }
}