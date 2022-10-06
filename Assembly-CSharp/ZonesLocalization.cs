using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;

public class ZonesLocalization : MonoBehaviour
{
	[Button(0)]
	public void GenerateLocalization()
	{
		List<Transform> list = (from gObj in base.gameObject.scene.GetRootGameObjects()
		select gObj.transform).ToList<Transform>();
		string pattern = "D(?<ID>[0-9][0-9]) - (?<name>[a-zA-Z0-9_ ()]*)";
		string pattern2 = "Z(?<ID>[0-9][0-9]) - (?<name>[a-zA-Z0-9_ ()]*)";
		Regex regex = new Regex(pattern);
		Regex regex2 = new Regex(pattern2);
		foreach (Transform transform in list)
		{
			Match match = regex.Match(transform.name);
			if (match.Success)
			{
				string value = match.Groups["ID"].Value;
				string value2 = match.Groups["name"].Value;
				this.CreateTermIfNeeded("Map/D" + value, value2);
				IEnumerator enumerator2 = transform.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						object obj = enumerator2.Current;
						Transform transform2 = (Transform)obj;
						Match match2 = regex2.Match(transform2.name);
						if (match2.Success)
						{
							string value3 = match2.Groups["ID"].Value;
							string value4 = match2.Groups["name"].Value;
							this.CreateTermIfNeeded("Map/D" + value + "_Z" + value3, value4);
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator2 as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
		}
	}

	private void CreateTermIfNeeded(string key, string cad)
	{
		TermData termData = this.source.GetTermData(key, false);
		if (termData == null)
		{
			termData = this.source.AddTerm(key);
		}
		for (int i = 0; i < this.source.GetLanguages(true).Count<string>(); i++)
		{
			termData.Languages[i] = cad;
		}
	}

	public LanguageSource source;
}
