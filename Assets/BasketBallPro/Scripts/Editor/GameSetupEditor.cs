namespace GameBench
{
    using UnityEngine;
    using UnityEditor;
#pragma warning disable CS0162 // Unreachable code detected
    [CustomEditor(typeof(GameSetup))]
    public class GameSetupEditor : Editor
    {
        private GameSetup instance;
        void OnEnable()
        {
            instance = (GameSetup)target;
        }
        public override void OnInspectorGUI()
        {
            CenterTitle("Game Setup Editor Menu");
            CustomEditorUI();
            FinishButtonGUI();
            if (GUI.changed)
            {
                GameSetup.DirtyEditor();
            }
        }
        public bool EnableAds
        {
            get
            {
                return instance.useAds;
            }

            set
            {
                bool prev_val = instance.useAds;
                if (prev_val == value)
                    return;
                instance.useAds = value;
                SetScriptingDefinedSymbols(GameSetup.ENABLE_ADS, value);
            }
        }

        public bool EnableFB
        {
            get
            {
                return instance.useFb;
            }

            set
            {
                bool prev_val = instance.useFb;
                if (prev_val == value)
                    return;
                instance.useFb = value;
                SetScriptingDefinedSymbols(GameSetup.ENABLE_FB_PLUGIN, value);
            }
        }
        public bool EnableIAP
        {
            get
            {
                return instance.useIAP;
            }

            set
            {
                bool prev_val = instance.useIAP;
                if (prev_val == value)
                    return;
                instance.useIAP = value;
                SetScriptingDefinedSymbols(GameSetup.ENABLE_IAP, value);
            }
        }

        void CustomEditorUI()
        {
#if !UNITY_IOS && !UNITY_ANDROID
            EditorGUILayout.Space();
            CenterTitle("Please select Android/iOS as build target to configure IAP, Facebook etc.");
            EditorGUILayout.Space();
            return;
#endif
            EnableAds = EditorGUILayout.BeginToggleGroup("Enable Ads", EnableAds);
            EditorGUILayout.EndToggleGroup();
#if ENABLE_ADS
            serializedObject.Update();
            if (instance.useAds)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();
                if (FoldOut("Unity Ads Configuration", false))
                {
                    EditorGUILayout.HelpBox("For Unity 5.5+ enable Unity Ads from the Services Window\nFor Older versions import Unity Ads Package from Asset Store", MessageType.Info);
                }
                EditorGUILayout.Space();

                if (FoldOut("Admob Configuration", false))
                {
                    instance.showAdBeforeGame = EditorGUILayout.Toggle("Show Ad Before Game", instance.showAdBeforeGame);
                    instance.showAdAfterGame = EditorGUILayout.Toggle("Show Ad After Game", instance.showAdAfterGame);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("interstitialAd"), new GUIContent("Interstitial Ad Ids"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("bannerAd"), new GUIContent("Banner Ad Ids"), true);

                }
                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
#endif


            EnableIAP = EditorGUILayout.BeginToggleGroup("Enable IAP", EnableIAP);
            EditorGUILayout.EndToggleGroup();
#if ENABLE_IAP
            EditorGUILayout.LabelField("Go to Windows>Services for setting Unity IAP");
#endif

            AddSpace(5);
            CenterTitle("More Optional Add-ons!");

            EnableFB = EditorGUILayout.BeginToggleGroup("Enable Facebook", EnableFB);
            EditorGUILayout.EndToggleGroup();
#if ENABLE_FACEBOOK
            EditorGUILayout.LabelField("Go to \"Facebook>Edit Settings\" from MenuBar to Configure Facebook");
#endif
        }
        void FinishButtonGUI()
        {
            DrawLine();
            CenterTitle("Basket Ball Pro Version 1.0");
            AddSpace(2);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Contact Us"))
            {
                Application.OpenURL("mailto:info.gamebench@gmail.com");
            }
            if (GUILayout.Button("Online Docs"))
            {
                Application.OpenURL("https://goo.gl/rLSzsp");
            }
            EditorGUILayout.EndHorizontal();
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
        public static bool FoldOut(string prefName, bool isBold = true)
        {
            bool defaultState = true;
            bool state = EditorPrefs.GetBool(prefName, defaultState);

            GUIStyle style = EditorStyles.foldout;
            FontStyle previousStyle = style.fontStyle;
            style.fontStyle = isBold ? FontStyle.Bold : FontStyle.Normal;
            bool newState = EditorGUILayout.Foldout(state, prefName, style);
            style.fontStyle = previousStyle;
            if (newState != state)
            {
                EditorPrefs.SetBool(prefName, newState);
            }
            return newState;
        }
        public static void AddSpace(int count)
        {
            for (int i = 0; i < count; i++)
            {
                EditorGUILayout.Space();
            }
        }
        public static void SetScriptingDefinedSymbols(string symbol, bool state)
        {
            SetScriptingDefinedSymbolsInternal(symbol, BuildTargetGroup.Android, state);
            SetScriptingDefinedSymbolsInternal(symbol, BuildTargetGroup.iOS, state);
        }
        static void SetScriptingDefinedSymbolsInternal(string symbol, BuildTargetGroup target, bool state)
        {
            var sNow = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
            sNow = sNow.Replace(symbol + ";", ""); sNow = sNow.Replace(symbol, "");
            if (state) sNow = symbol + ";" + sNow;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, sNow);
        }
    }
}