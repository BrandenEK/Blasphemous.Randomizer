using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Map
{
	public class MapRenderer
	{
		public MapRenderer(MapRendererConfig config, Transform parent, string name)
		{
			this.Config = config;
			string text = "RootRenderer_" + name;
			this.Root = (RectTransform)parent.Find(text);
			if (this.Root == null)
			{
				this.Root = this.CreateRectTranform(parent, text);
				this.Root.localRotation = Quaternion.identity;
				this.Root.localScale = Vector3.one;
				this.Root.localPosition = Vector3.zero;
			}
			Sprite sprite = this.Config.Sprites.Values.First<Sprite>();
			this.CellSizeX = sprite.rect.width;
			this.CellSizeY = sprite.rect.height;
			this.spriteTransforms.Clear();
			this.cellsUI.Clear();
		}

		public MapRendererConfig Config { get; private set; }

		public RectTransform Root { get; private set; }

		public CellKey GetCenterCell()
		{
			return new CellKey(Mathf.RoundToInt(-this.Root.localPosition.x / this.CellSizeX), Mathf.RoundToInt(-this.Root.localPosition.y / this.CellSizeY));
		}

		public void SetCenterCell(CellKey cellKey)
		{
			if (cellKey != null)
			{
				this.Root.localPosition = this.GetPosition(cellKey) * -1f;
			}
		}

		public void MoveCenter(Vector3 movement)
		{
			CellKey cellKey = new CellKey(Mathf.RoundToInt(-(this.Root.localPosition.x + movement.x) / this.CellSizeX), Mathf.RoundToInt(-(this.Root.localPosition.y + movement.y) / this.CellSizeY));
			if (cellKey.X < this.Config.minCell.x || cellKey.X > this.Config.maxCell.x)
			{
				movement.x = 0f;
			}
			if (cellKey.Y < this.Config.minCell.y || cellKey.Y > this.Config.maxCell.y)
			{
				movement.y = 0f;
			}
			this.Root.localPosition += movement;
		}

		public Vector3 Center
		{
			get
			{
				return this.Root.localPosition;
			}
			set
			{
				this.MoveCenterTo(value);
			}
		}

		public void MoveCenterTo(Vector3 position)
		{
			CellKey cellKey = new CellKey(Mathf.RoundToInt(-position.x / this.CellSizeX), Mathf.RoundToInt(-position.y / this.CellSizeY));
			if (cellKey.X >= this.Config.minCell.x && cellKey.X <= this.Config.maxCell.x && cellKey.Y >= this.Config.minCell.y && cellKey.Y <= this.Config.maxCell.y)
			{
				this.Root.localPosition = position;
			}
		}

		public void Render(List<CellData> revealedCells, List<CellKey> secrets, Dictionary<CellKey, List<MapData.MarkType>> marks, CellKey playerCell)
		{
			Dictionary<CellKey, Image> dictionary = new Dictionary<CellKey, Image>();
			foreach (CellData cellData in revealedCells)
			{
				MapRenderer.SpriteTransform sprite = this.GetSprite(cellData.Walls, cellData.Doors);
				if (sprite.BaseSprite == null)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"Map renderer sprite null KEY:",
						sprite.Key,
						" CELL:",
						cellData.CellKey.X.ToString(),
						"_",
						cellData.CellKey.Y.ToString(),
						"  ZONE:",
						cellData.ZoneId.GetKey()
					}));
				}
				else
				{
					Image image;
					if (this.CachedImages.ContainsKey(cellData.CellKey) && !secrets.Contains(cellData.CellKey))
					{
						image = this.CachedImages[cellData.CellKey];
						this.CachedImages.Remove(cellData.CellKey);
					}
					else
					{
						RectTransform rectTransform = this.CreateRectTranform(this.Root, cellData.CellKey.X.ToString() + "_" + cellData.CellKey.Y.ToString());
						rectTransform.localRotation = Quaternion.identity;
						rectTransform.localScale = new Vector3((!sprite.FlipX) ? 1f : -1f, (!sprite.FlipY) ? 1f : -1f, 1f);
						rectTransform.localPosition = this.GetPosition(cellData.CellKey);
						rectTransform.sizeDelta = new Vector2(sprite.BaseSprite.rect.width, sprite.BaseSprite.rect.height);
						image = rectTransform.gameObject.AddComponent<Image>();
					}
					dictionary[cellData.CellKey] = image;
					image.sprite = sprite.BaseSprite;
					image.material = this.Config.SpriteMaterial;
					if (cellData.CellKey.Equals(playerCell))
					{
						image.color = this.Config.PlayerBackgoundColor;
					}
					else
					{
						image.color = this.Config.GetZoneColor(cellData.ZoneId);
					}
					MapRenderer.UIData uidata = new MapRenderer.UIData();
					uidata.originalColor = image.color;
					uidata.image = image;
					this.cellsUI[cellData.CellKey] = uidata;
				}
			}
			foreach (KeyValuePair<CellKey, Image> keyValuePair in this.CachedImages)
			{
				Object.Destroy(keyValuePair.Value.gameObject);
			}
			this.CachedImages = new Dictionary<CellKey, Image>(dictionary);
			if (this.markRoot != null)
			{
				Object.Destroy(this.markRoot.gameObject);
			}
			this.markRoot = this.CreateRectTranform(this.Root, "Marks");
			this.markRoot.localRotation = Quaternion.identity;
			this.markRoot.localScale = Vector3.one;
			this.markRoot.localPosition = Vector3.zero;
			this.markRoot.gameObject.SetActive(true);
			this.UpdateMarks(marks);
		}

		public void UpdateMarks(Dictionary<CellKey, List<MapData.MarkType>> marks)
		{
			IEnumerator enumerator = this.markRoot.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					Object.Destroy(transform.gameObject);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			foreach (KeyValuePair<CellKey, List<MapData.MarkType>> keyValuePair in marks)
			{
				Sprite sprite = null;
				int num = 0;
				foreach (MapData.MarkType key in keyValuePair.Value)
				{
					if (this.Config.Marks.TryGetValue(key, out sprite) && sprite != null)
					{
						RectTransform rectTransform = this.CreateRectTranform(this.markRoot, string.Concat(new string[]
						{
							keyValuePair.Key.X.ToString(),
							"_",
							keyValuePair.Key.Y.ToString(),
							"_",
							num.ToString()
						}));
						rectTransform.localRotation = Quaternion.identity;
						rectTransform.localScale = Vector3.one;
						rectTransform.localPosition = this.GetPosition(keyValuePair.Key);
						rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
						Image image = rectTransform.gameObject.AddComponent<Image>();
						image.sprite = sprite;
					}
					num++;
				}
			}
		}

		public void ToogleMarks()
		{
			this.markRoot.gameObject.SetActive(!this.markRoot.gameObject.activeSelf);
		}

		public void SetVisibleMarks(bool visible)
		{
			this.markRoot.gameObject.SetActive(visible);
		}

		public void SetSelected(List<CellKey> cells, bool selected)
		{
			foreach (CellKey key in cells)
			{
				if (this.cellsUI.ContainsKey(key))
				{
					if (selected)
					{
						this.cellsUI[key].image.material = this.Config.SpriteMaterialSelected;
					}
					else
					{
						this.cellsUI[key].image.material = this.Config.SpriteMaterial;
					}
				}
			}
		}

		public void ResetSelection()
		{
			foreach (MapRenderer.UIData uidata in this.cellsUI.Values)
			{
				uidata.image.material = this.Config.SpriteMaterial;
			}
		}

		public Vector2 GetPosition(CellKey cellKey)
		{
			return new Vector2(this.CellSizeX * (float)cellKey.X, this.CellSizeY * (float)cellKey.Y);
		}

		private RectTransform CreateRectTranform(Transform parent, string name)
		{
			GameObject gameObject = new GameObject(name, new Type[]
			{
				typeof(RectTransform)
			});
			gameObject.transform.SetParent(parent);
			return (RectTransform)gameObject.transform;
		}

		private MapRenderer.SpriteTransform GetSprite(bool[] walls, bool[] doors)
		{
			MapRenderer.SpriteTransform spriteTransform = new MapRenderer.SpriteTransform();
			string text = string.Empty;
			foreach (EditorMapCellData.CellSide cellSide in MapRendererConfig.spriteKeyOrder)
			{
				string str = "_";
				int num = (int)cellSide;
				if (doors[num])
				{
					str = "D";
				}
				else if (walls[num])
				{
					str = "W";
				}
				text += str;
			}
			spriteTransform.Key = text;
			if (this.Config.Sprites.ContainsKey(text))
			{
				spriteTransform.BaseSprite = this.Config.Sprites[text];
			}
			else if (this.spriteTransforms.ContainsKey(text))
			{
				spriteTransform = this.spriteTransforms[text];
			}
			else
			{
				string key = text.Substring(2, 1) + text.Substring(1, 1) + text.Substring(0, 1) + text.Substring(3, 1);
				if (this.Config.Sprites.ContainsKey(key))
				{
					spriteTransform.FlipX = true;
					spriteTransform.BaseSprite = this.Config.Sprites[key];
					this.spriteTransforms[key] = spriteTransform;
				}
				else
				{
					string key2 = text.Substring(0, 1) + text.Substring(3, 1) + text.Substring(2, 1) + text.Substring(1, 1);
					if (this.Config.Sprites.ContainsKey(key2))
					{
						spriteTransform.FlipY = true;
						spriteTransform.BaseSprite = this.Config.Sprites[key2];
						this.spriteTransforms[key2] = spriteTransform;
					}
					else
					{
						string key3 = text.Substring(2, 1) + text.Substring(3, 1) + text.Substring(0, 1) + text.Substring(1, 1);
						if (this.Config.Sprites.ContainsKey(key3))
						{
							spriteTransform.FlipY = true;
							spriteTransform.FlipX = true;
							spriteTransform.BaseSprite = this.Config.Sprites[key3];
							this.spriteTransforms[key3] = spriteTransform;
						}
						else
						{
							Debug.LogError("Renderer config: Can find sprite for cell " + text);
						}
					}
				}
			}
			return spriteTransform;
		}

		private const string RootName = "RootRenderer";

		private float CellSizeX;

		private float CellSizeY;

		private RectTransform markRoot;

		private Dictionary<string, MapRenderer.SpriteTransform> spriteTransforms = new Dictionary<string, MapRenderer.SpriteTransform>();

		private Dictionary<CellKey, Image> CachedImages = new Dictionary<CellKey, Image>();

		private Dictionary<CellKey, MapRenderer.UIData> cellsUI = new Dictionary<CellKey, MapRenderer.UIData>();

		private class SpriteTransform
		{
			public Sprite BaseSprite;

			public bool FlipX;

			public bool FlipY;

			public string Key = string.Empty;
		}

		private class UIData
		{
			public Image image;

			public Color originalColor;
		}
	}
}
