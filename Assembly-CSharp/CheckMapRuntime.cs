using System;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class CheckMapRuntime : MonoBehaviour
{
	[HideInEditorMode]
	[Button(ButtonSizes.Small)]
	private void TestMap()
	{
		Core.Persistence.DEBUG_SaveAllData(this.slot, this.mapReveals, this.elementPerScene);
	}

	public int slot = 4;

	public int mapReveals = 10;

	public int elementPerScene = 10;
}
