using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using System.IO;

[OPS.Obfuscator.Attribute.DoNotRenameAttribute]
public class PostBuildStep
{
    // Set the IDFA request description:
    [OPS.Obfuscator.Attribute.DoNotRenameAttribute]
    const string k_TrackingDescription = "We will use your data to provide a better and personalized ad experience.";

    [OPS.Obfuscator.Attribute.DoNotRenameAttribute]
    const string k_LocationDescription = "Your location will provide you with the best and personalized advertising selection for you.";

    [PostProcessBuild]
    [OPS.Obfuscator.Attribute.DoNotRenameAttribute]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToXcode)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            AddPListValues(pathToXcode);
        }
    }

    // Implement a function to read and write values to the plist file:
    [OPS.Obfuscator.Attribute.DoNotRenameAttribute]
    static void AddPListValues(string pathToXcode)
    {
        // Retrieve the plist file from the Xcode project directory:
        string plistPath = pathToXcode + "/Info.plist";
        PlistDocument plistObj = new PlistDocument();


        // Read the values from the plist file:
        plistObj.ReadFromString(File.ReadAllText(plistPath));

        // Set values from the root object:
        PlistElementDict plistRoot = plistObj.root;

        // Set the description key-value in the plist:
        plistRoot.SetString("NSUserTrackingUsageDescription", k_TrackingDescription);
        plistRoot.SetString("NSLocationWhenInUseUsageDescription", k_LocationDescription);

        // Save changes to the plist:
        File.WriteAllText(plistPath, plistObj.WriteToString());
    }
}
#endif