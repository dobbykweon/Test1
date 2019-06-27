using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class PostBuild  {
    [PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget target, string pathToBuiltProject) {
		if (target == BuildTarget.iOS) {

			//Disable metal for the remote display sample.  Metal is not supported due to shaders used in the source Unity sample.
			PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, false);
			GraphicsDeviceType[] apis = {GraphicsDeviceType.OpenGLES2};
			PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, apis);

		} else if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64) {
			string path = pathToBuiltProject.Substring(0, pathToBuiltProject.IndexOf(".exe")) + "_Data/Plugins/model";
			FileUtil.CopyFileOrDirectory("Assets/ULSFaceTracker/Plugins/model", path);
		}
    }
}
