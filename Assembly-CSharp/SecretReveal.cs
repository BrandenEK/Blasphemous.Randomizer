using System;
using Framework.Managers;

[Serializable]
public class SecretReveal
{
	public void Reveal()
	{
		if (this.UseMapIdInsteadOfCurrentMap)
		{
			if (this.RevealSecret && this.MapId != string.Empty && this.SecretId != string.Empty)
			{
				Core.NewMapManager.SetSecret(this.MapId, this.SecretId, true);
			}
		}
		else if (this.RevealSecret && this.SecretId != string.Empty)
		{
			Core.NewMapManager.SetSecret(this.SecretId, true);
		}
	}

	public bool RevealSecret;

	public string MapId = string.Empty;

	public string SecretId = string.Empty;

	public bool UseMapIdInsteadOfCurrentMap;
}
