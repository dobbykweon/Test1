#if UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using System.IO;

public class BluetoothPostProcessBuild
{
	[PostProcessBuild]
	public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
	{
		if (buildTarget == BuildTarget.iOS)
		{
			// Get plist
			string plistPath = pathToBuiltProject + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));

			// Get root
			PlistElementDict rootDict = plist.root;

			rootDict.SetString("NSBluetoothPeripheralUsageDescription", "Activate the function in order to connect with the smart toothbrush.");
			rootDict.SetString("NSPhotoLibraryUsageDescription", "Activate the function in order to check saved photos.");
			rootDict.SetString("NSPhotoLibraryAddUsageDescription", "Activate the function in order to save selfies.");

			// Write to file
			File.WriteAllText(plistPath, plist.WriteToString());
		}
	}
}
#endif
