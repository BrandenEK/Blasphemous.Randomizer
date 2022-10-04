using System;
using PygmyMonkey.AdvancedBuilder;
using UnityEngine;

public class Demo1 : MonoBehaviour
{
	private void Start()
	{
		if (AppParameters.Get.platformType.Equals("Android"))
		{
			Debug.Log("Android platform");
		}
		else if (AppParameters.Get.platformType.Equals("iOS"))
		{
			Debug.Log("iOS platform");
		}
		string releaseType = AppParameters.Get.releaseType;
		if (releaseType != null)
		{
			if (!(releaseType == "Dev"))
			{
				if (!(releaseType == "Beta"))
				{
					if (!(releaseType == "Release"))
					{
					}
				}
			}
		}
	}

	private void OnGUI()
	{
		int num = 5;
		int num2 = 5;
		int num3 = 200;
		int num4 = 22;
		GUI.Label(new Rect((float)num, (float)num2, (float)num3, (float)num4), "Release type:");
		GUI.Label(new Rect((float)(num + num3), (float)num2, (float)num3, (float)num4), AppParameters.Get.releaseType);
		num2 += num4;
		GUI.Label(new Rect((float)num, (float)num2, (float)num3, (float)num4), "Platform type:");
		GUI.Label(new Rect((float)(num + num3), (float)num2, (float)num3, (float)num4), AppParameters.Get.platformType);
		num2 += num4;
		GUI.Label(new Rect((float)num, (float)num2, (float)num3, (float)num4), "Distribution platform:");
		GUI.Label(new Rect((float)(num + num3), (float)num2, (float)num3, (float)num4), AppParameters.Get.distributionPlatform);
		num2 += num4;
		GUI.Label(new Rect((float)num, (float)num2, (float)num3, (float)num4), "Platform architecture:");
		GUI.Label(new Rect((float)(num + num3), (float)num2, (float)num3, (float)num4), AppParameters.Get.platformArchitecture);
		num2 += num4;
		GUI.Label(new Rect((float)num, (float)num2, (float)num3, (float)num4), "Texture compression:");
		GUI.Label(new Rect((float)(num + num3), (float)num2, (float)num3, (float)num4), AppParameters.Get.textureCompression);
		num2 += num4;
		GUI.Label(new Rect((float)num, (float)num2, (float)num3, (float)num4), "Product name:");
		GUI.Label(new Rect((float)(num + num3), (float)num2, (float)num3, (float)num4), AppParameters.Get.productName);
		num2 += num4;
		GUI.Label(new Rect((float)num, (float)num2, (float)num3, (float)num4), "Bundle identifier:");
		GUI.Label(new Rect((float)(num + num3), (float)num2, (float)num3, (float)num4), AppParameters.Get.bundleIdentifier);
		num2 += num4;
		GUI.Label(new Rect((float)num, (float)num2, (float)num3, (float)num4), "Bundle version:");
		GUI.Label(new Rect((float)(num + num3), (float)num2, (float)num3, (float)num4), AppParameters.Get.bundleVersion);
		if (AppParameters.Get.platformType.Equals("Android"))
		{
			num2 += num4;
			GUI.Label(new Rect((float)num, (float)num2, (float)num3, (float)num4), "Texture Compression:");
			GUI.Label(new Rect((float)(num + num3), (float)num2, (float)num3, (float)num4), AppParameters.Get.textureCompression);
		}
		if (AppParameters.Get.platformType.Equals("Windows"))
		{
			num2 += num4;
			GUI.Label(new Rect((float)num, (float)num2, (float)num3, (float)num4), "Platform Architecture:");
			GUI.Label(new Rect((float)(num + num3), (float)num2, (float)num3, (float)num4), AppParameters.Get.platformArchitecture);
		}
		num2 += num4;
		GUI.Label(new Rect((float)num, (float)num2, (float)num3, (float)num4), "Platform Windows (from define)");
	}
}
