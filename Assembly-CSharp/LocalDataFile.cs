using System;
using System.IO;
using System.Text;
using Framework.Managers;
using FullSerializer;
using Tools;
using UnityEngine;

public class LocalDataFile
{
	public LocalDataFile(ILocalData data)
	{
		this.Data = data;
		this.LoadData();
	}

	public bool SaveData()
	{
		string saveGameFile = this.GetSaveGameFile();
		Debug.Log("* Saving file " + saveGameFile);
		fsData fsData;
		fsResult fsResult = this.serializer.TrySerialize<ILocalData>(this.Data, ref fsData);
		if (fsResult.Failed)
		{
			Debug.LogError("** Saving file error: " + fsResult.FormattedMessages);
			return false;
		}
		string s = fsJsonPrinter.CompressedJson(fsData);
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		string encryptedData = Convert.ToBase64String(bytes);
		FileTools.SaveSecure(saveGameFile, encryptedData);
		return true;
	}

	public bool LoadData()
	{
		string saveGameFile = this.GetSaveGameFile();
		Debug.Log("* Loading file " + saveGameFile);
		bool flag = true;
		string text = string.Empty;
		this.Data.Clean();
		try
		{
			string s = File.ReadAllText(saveGameFile);
			byte[] bytes = Convert.FromBase64String(s);
			text = Encoding.UTF8.GetString(bytes);
		}
		catch (Exception)
		{
			flag = false;
		}
		if (flag)
		{
			fsData fsData;
			fsResult fsResult = fsJsonParser.Parse(text, ref fsData);
			if (fsResult.Failed)
			{
				Debug.LogError("** Loading file parsing error: " + fsResult.FormattedMessages);
				flag = false;
			}
			else
			{
				try
				{
					fsResult = this.serializer.TryDeserialize<ILocalData>(fsData, ref this.Data);
				}
				catch (Exception ex)
				{
					Debug.LogError("** Loading file deserialization exception: " + ex.Message);
					flag = false;
				}
				finally
				{
					if (fsResult.Failed)
					{
						Debug.LogError("** Loading file deserialization error: " + fsResult.FormattedMessages);
						flag = false;
					}
				}
			}
		}
		return flag;
	}

	private string GetSaveGameFile()
	{
		return PersistentManager.GetPathAppSettings(this.Data.GetFileName());
	}

	private fsSerializer serializer = new fsSerializer();

	public ILocalData Data;
}
