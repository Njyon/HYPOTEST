using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Ultra
{
	public class Hope
	{
		UnityEngine.Object obj;
		System.Threading.Timer timer;
		static Hope instance;
		public static Hope Instance
		{
			get {
				if (instance == null) instance = new Hope(); 
				return instance; 
			}
		}

		public Hope()
		{ }

		public async System.Threading.Tasks.Task SwitchAsset(UnityEngine.Object asset)
		{
			obj = asset;
			Selection.activeObject = null;
			await System.Threading.Tasks.Task.Delay(10);
			Selection.activeObject = obj;
		}
	}
}
#endif

