using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PreloadAsset", menuName = "Create preload reference holder asset")]
public class PreloadReferenceScriptableAsset : ScriptableObject
{
	public List<GameObject> enemyPrefabs;

	public List<Texture> spritesheets;
}
