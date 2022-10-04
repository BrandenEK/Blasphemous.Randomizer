using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core.Surrogates;
using DG.Tweening.Plugins.Options;
using FMODUnity;
using Framework.Managers;
using Framework.Map;
using Rewired;
using Sirenix.OdinInspector;
using Tools.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class NewMapMenuWidget : MonoBehaviour
	{
		public NewMapMenuWidget()
		{
			this.UseSpeedInTeleport = true;
			this.TeleportSpeed = 400f;
			this.TeleportTime = 0.4f;
			this.TeleportEase = Ease.InOutQuad;
			this.TeleportMovementEpsilon = 0.2f;
			this.TeleportSoundFound = "event:/SFX/UI/ChangeTab";
			this.TeleportSoundNotFound = "event:/SFX/UI/ChangeSelection";
			this.AnglesToCheckNear = new List<float>
			{
				30f,
				40f,
				60f
			};
			this.Cells = new List<CellData>();
			this.CellsDict = new Dictionary<CellKey, CellData>();
			this.KeysByZone = new Dictionary<ZoneKey, List<CellKey>>();
			this.MapRenderers = new List<MapRenderer>();
			this.markItems = new List<NewMapMenuWidgetMarkItem>();
			this.TeleportTarget = new CellKey(0, 0);
			this.TeleportStart = Vector2.zero;
		}

		public PauseWidget.MapModes CurrentMapMode { get; private set; }

		public void Initialize()
		{
			this.TeleportTransform = (RectTransform)this.CursorTeleportElement.transform;
			this.TeleportStart = this.TeleportTransform.sizeDelta;
			Player player = ReInput.players.GetPlayer(0);
			if (player != null)
			{
				this.Rewired = player;
			}
			this.Cells = Core.NewMapManager.GetAllRevealedCells();
			List<CellKey> allRevealSecretsCells = Core.NewMapManager.GetAllRevealSecretsCells();
			this.CellsDict.Clear();
			this.KeysByZone.Clear();
			Dictionary<CellKey, List<MapData.MarkType>> allMarks = Core.NewMapManager.GetAllMarks();
			foreach (CellData cellData in this.Cells)
			{
				this.CellsDict[cellData.CellKey] = cellData;
				if (!this.KeysByZone.ContainsKey(cellData.ZoneId))
				{
					this.KeysByZone[cellData.ZoneId] = new List<CellKey>();
				}
				this.KeysByZone[cellData.ZoneId].Add(cellData.CellKey);
			}
			int num = 0;
			if (Core.NewMapManager.GetTotalCells() != 0)
			{
				num = (from cell in this.Cells
				where !cell.IgnoredForMapPercentage
				select cell).ToList<CellData>().Count * 100 / Core.NewMapManager.GetTotalCells();
			}
			this.PercentText.text = num.ToString() + "%";
			this.CherubsText.text = CherubCaptorPersistentObject.CountRescuedCherubs() + "/" + 38;
			if (this.itemsText == null)
			{
				RectTransform rectTransform = UnityEngine.Object.Instantiate<GameObject>(this.CherubsText.gameObject, base.transform).transform as RectTransform;
				rectTransform.anchoredPosition = new Vector2(45f, -60f);
				this.itemsText = rectTransform.GetComponentInChildren<Text>();
				this.itemsText.alignment = TextAnchor.MiddleLeft;
			}
			this.itemsText.text = string.Concat(new object[]
			{
				"Items collected: ",
				Core.Randomizer.itemsCollected,
				"/",
				Core.Randomizer.totalItems
			});
			if (this.MapRenderers.Count == 0)
			{
				int num2 = 0;
				using (List<MapRendererConfig>.Enumerator enumerator2 = this.RendererConfigs.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						MapRendererConfig config = enumerator2.Current;
						MapRenderer mapRenderer = new MapRenderer(config, this.MapContent, num2.ToString());
						mapRenderer.Render(this.Cells, allRevealSecretsCells, allMarks, Core.NewMapManager.GetPlayerCell());
						mapRenderer.Root.gameObject.SetActive(false);
						this.MapRenderers.Add(mapRenderer);
						num2++;
					}
					goto IL_32E;
				}
			}
			foreach (MapRenderer mapRenderer2 in this.MapRenderers)
			{
				mapRenderer2.Render(this.Cells, allRevealSecretsCells, allMarks, Core.NewMapManager.GetPlayerCell());
				mapRenderer2.Root.gameObject.SetActive(false);
			}
			IL_32E:
			this.CurrentRendererIndex = 0;
			this.CurrentRenderer = null;
			this.markItems.Clear();
			this.currentMarkItem = -1;
			this.markTemplateElement.SetActive(false);
			this.scrollRect = this.selectMarkRoot.GetComponentInChildren<ScrollRect>();
			this.markUISize = ((RectTransform)this.markTemplateElement.transform).rect.width + 2f;
			float width = this.scrollRect.viewport.rect.width;
			this.visibleMarksInScroll = Mathf.FloorToInt(width / this.markUISize);
		}

		public void OnShow(PauseWidget.MapModes mapMode)
		{
			Core.NewMapManager.RevealCellInCurrentPlayerPosition();
			this.CurrentMapMode = mapMode;
			this.CurrentRendererIndex = 0;
			this.UpdateZoomControl();
			this.IsEnableMarkSelector = false;
			this.mapEnabled = Core.NewMapManager.CanShowMapInCurrentZone();
			this.UpdateCurrentRenderer();
			this.MapControlsRootNormal.SetActive(this.mapEnabled && this.CurrentMapMode == PauseWidget.MapModes.SHOW);
			this.MapControlsRootTeleport.SetActive(this.mapEnabled && this.CurrentMapMode == PauseWidget.MapModes.TELEPORT);
			this.MapControlsNotMap.SetActive(!this.mapEnabled);
			this.teleportNormalButton.SetActive(false);
			this.teleportDisableButton.SetActive(true);
			this.markCurrentCooldownTime = 0f;
		}

		public void ZoomOut()
		{
			if (!this.mapEnabled)
			{
				return;
			}
			if (this.CurrentRendererIndex < this.MapRenderers.Count - 1)
			{
				this.CurrentRendererIndex++;
				this.UpdateCurrentRenderer();
				this.CurrentRenderer.UpdateMarks(Core.NewMapManager.GetAllMarks());
				this.UpdateZoomControl();
			}
		}

		public void ZoomIn()
		{
			if (!this.mapEnabled)
			{
				return;
			}
			if (this.CurrentRendererIndex > 0)
			{
				this.CurrentRendererIndex--;
				this.UpdateCurrentRenderer();
				this.CurrentRenderer.UpdateMarks(Core.NewMapManager.GetAllMarks());
				this.UpdateZoomControl();
			}
		}

		public void CenterView()
		{
			this.CurrentRenderer.ResetSelection();
			this.CurrentCell = null;
			this.CurrentKey = Core.NewMapManager.GetPlayerCell();
			if (this.CurrentRenderer.Config.CenterAtPlayer)
			{
				this.CurrentRenderer.SetCenterCell(this.CurrentKey);
			}
			else
			{
				this.CurrentRenderer.SetCenterCell(new CellKey(this.CurrentRenderer.Config.centerCell.x, this.CurrentRenderer.Config.centerCell.y));
			}
			this.UpdateCellData();
		}

		public void UITabLeft()
		{
			if (!this.IsEnableMarkSelector || this.currentMarkItem == -1)
			{
				return;
			}
			if (this.currentMarkItem > 0)
			{
				this.markItems[this.currentMarkItem].SetSelected(false);
				this.currentMarkItem--;
				this.markItems[this.currentMarkItem].SetSelected(true);
				this.UpdateScrollToCurrent();
			}
		}

		public void UITabRight()
		{
			if (!this.IsEnableMarkSelector || this.currentMarkItem == -1)
			{
				return;
			}
			if (this.currentMarkItem < this.markItems.Count - 1)
			{
				this.markItems[this.currentMarkItem].SetSelected(false);
				this.currentMarkItem++;
				this.markItems[this.currentMarkItem].SetSelected(true);
				this.UpdateScrollToCurrent();
			}
		}

		public void ToogleMarks()
		{
			if (!this.mapEnabled || this.CurrentRenderer == null || !this.CurrentRenderer.Config.UseMarks)
			{
				return;
			}
			this.IsEnableMarkSelector = !this.IsEnableMarkSelector;
			this.UpdateElementsByMode();
		}

		public void SubmitPressed()
		{
			PauseWidget.MapModes currentMapMode = this.CurrentMapMode;
			if (currentMapMode != PauseWidget.MapModes.SHOW)
			{
				if (currentMapMode == PauseWidget.MapModes.TELEPORT)
				{
					this.TeleportPressed();
					return;
				}
			}
			else
			{
				this.MarkPressed();
			}
		}

		public void MarkPressed()
		{
			if (!this.mapEnabled || this.CurrentRenderer == null || !this.CurrentRenderer.Config.UseMarks || !this.IsEnableMarkSelector)
			{
				return;
			}
			if (this.CurrentMapMode == PauseWidget.MapModes.TELEPORT || !this.ParentWidget.IsActive() || this.ParentWidget.IsFading)
			{
				return;
			}
			if (this.markCurrentCooldownTime < 0.5f)
			{
				return;
			}
			if (this.canAddMark)
			{
				Core.NewMapManager.AddMarkOnCell(this.CurrentRenderer.GetCenterCell(), this.markItems[this.currentMarkItem].MarkId);
			}
			else if (this.canRemoveMark)
			{
				Core.NewMapManager.RemoveMarkOnCell(this.CurrentRenderer.GetCenterCell());
			}
			this.CurrentRenderer.UpdateMarks(Core.NewMapManager.GetAllMarks());
		}

		public void TeleportPressed()
		{
			if (!this.mapEnabled || this.CurrentRenderer == null || this.CurrentMapMode != PauseWidget.MapModes.TELEPORT)
			{
				return;
			}
			CellData teleportCell = this.GetTeleportCell();
			if (teleportCell == null)
			{
				return;
			}
			UIController.instance.HidePauseMenu();
			Core.SpawnManager.SpawnFromTeleportOnPrieDieu(teleportCell.ZoneId.GetLevelName(), true);
		}

		public bool GoBack()
		{
			return false;
		}

		private void UpdateCurrentRenderer()
		{
			if (this.CurrentRenderer != null)
			{
				this.CurrentRenderer.ResetSelection();
				this.CurrentRenderer.Root.gameObject.SetActive(false);
			}
			if (this.mapEnabled)
			{
				this.CurrentRenderer = this.MapRenderers[this.CurrentRendererIndex];
				this.CurrentRenderer.Root.gameObject.SetActive(true);
				this.CursorElement.SetActive(this.CurrentRenderer.Config.UseMarks && this.CurrentMapMode != PauseWidget.MapModes.TELEPORT);
				this.CursorTeleportElement.SetActive(this.CurrentRenderer.Config.UseMarks && this.CurrentMapMode == PauseWidget.MapModes.TELEPORT);
				this.CenterView();
			}
			else
			{
				this.CurrentRenderer = null;
				this.CurrentKey = null;
				this.CursorElement.SetActive(false);
				this.CursorTeleportElement.SetActive(false);
			}
			this.noMapText.SetActive(!this.mapEnabled);
			this.UpdateCellData();
			this.IsEnableMarkSelector = (this.CurrentRenderer != null && this.CurrentRenderer.Config.UseMarks && this.CurrentMapMode == PauseWidget.MapModes.SHOW);
			this.currentMarkItem = -1;
			this.UpdateElementsByMode();
		}

		private void UpdateElementsByMode()
		{
			if (!this.mapEnabled)
			{
				this.IsEnableMarkSelector = false;
				if (this.CurrentRenderer != null)
				{
					this.CurrentRenderer.SetVisibleMarks(false);
				}
			}
			else
			{
				this.CurrentRenderer.SetVisibleMarks(this.CurrentRenderer.Config.UseMarks && (this.CurrentMapMode == PauseWidget.MapModes.TELEPORT || this.IsEnableMarkSelector));
				if (this.CurrentRenderer.Config.UseMarks)
				{
					IEnumerator enumerator = this.markTemplateElement.transform.parent.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							Transform transform = (Transform)obj;
							if (transform != this.markTemplateElement.transform)
							{
								UnityEngine.Object.Destroy(transform.gameObject);
							}
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
					this.markItems.Clear();
					foreach (KeyValuePair<MapData.MarkType, Sprite> keyValuePair in this.CurrentRenderer.Config.Marks)
					{
						if (!MapData.MarkPrivate.Contains(keyValuePair.Key))
						{
							GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.markTemplateElement, this.markTemplateElement.transform.parent);
							gameObject.name = keyValuePair.Key.ToString();
							NewMapMenuWidgetMarkItem component = gameObject.GetComponent<NewMapMenuWidgetMarkItem>();
							component.SetInitialData(keyValuePair.Key, keyValuePair.Value, false);
							gameObject.SetActive(true);
							this.markItems.Add(component);
						}
					}
					if (this.markItems.Count > 0)
					{
						if (this.currentMarkItem < 0 || this.currentMarkItem >= this.markItems.Count)
						{
							this.markItems[0].SetSelected(true);
							this.currentMarkItem = 0;
						}
						else
						{
							this.markItems[this.currentMarkItem].SetSelected(true);
						}
						this.UpdateScrollToCurrent();
					}
				}
			}
			this.UpdateMarkButtons();
		}

		private void UpdateMarkButtons()
		{
			this.addMarkNormalButton.SetActive(this.IsEnableMarkSelector && this.canAddMark);
			this.removeMarkButton.SetActive(this.IsEnableMarkSelector && this.canRemoveMark);
			this.addMarkDisableButton.SetActive(!this.IsEnableMarkSelector || (!this.canAddMark && !this.canRemoveMark));
			foreach (NewMapMenuWidgetMarkItem newMapMenuWidgetMarkItem in this.markItems)
			{
				newMapMenuWidgetMarkItem.SetDisabled(!this.IsEnableMarkSelector);
			}
			this.MarkFrameImage.color = (this.IsEnableMarkSelector ? Color.white : Color.gray);
		}

		private void Update()
		{
			if (this.CurrentRenderer == null || !this.CurrentRenderer.Config || !this.ParentWidget.IsActive())
			{
				return;
			}
			if (this.Rewired != null && this.CurrentMapMode == PauseWidget.MapModes.SHOW)
			{
				float axisRaw = this.Rewired.GetAxisRaw(48);
				float axisRaw2 = this.Rewired.GetAxisRaw(49);
				if (Mathf.Abs(axisRaw) >= 0.08f || Mathf.Abs(axisRaw2) >= 0.08f)
				{
					Vector2 vector = new Vector2(axisRaw, axisRaw2);
					vector *= Time.unscaledDeltaTime * this.CurrentRenderer.Config.MovementSpeed * -1f;
					this.CurrentRenderer.MoveCenter(vector);
				}
			}
			if (this.CurrentRenderer.Config.CenterAtPlayer)
			{
				CellKey centerCell = this.CurrentRenderer.GetCenterCell();
				if (centerCell != this.CurrentKey)
				{
					this.CurrentKey = centerCell;
					this.UpdateCellData();
				}
			}
			if (this.CurrentRenderer.Config.UseMarks && this.IsEnableMarkSelector)
			{
				this.canAddMark = false;
				CellKey centerCell2 = this.CurrentRenderer.GetCenterCell();
				this.canRemoveMark = Core.NewMapManager.IsMarkOnCell(centerCell2);
				if (!this.canRemoveMark)
				{
					this.canAddMark = Core.NewMapManager.CanAddMark(centerCell2);
				}
				if (this.markCurrentCooldownTime < 0.5f)
				{
					this.markCurrentCooldownTime += Time.unscaledDeltaTime;
				}
				this.UpdateMarkButtons();
			}
			if (this.CurrentMapMode == PauseWidget.MapModes.SHOW)
			{
				this.ControlZoom();
				return;
			}
			this.ControlTeleportMovement();
			bool flag = this.GetTeleportCell() != null;
			this.teleportNormalButton.SetActive(flag);
			this.teleportDisableButton.SetActive(!flag);
		}

		private CellData GetTeleportCell()
		{
			CellData result = null;
			CellKey centerCell = this.CurrentRenderer.GetCenterCell();
			if (!centerCell.Equals(Core.NewMapManager.GetPlayerCell()) && this.CellsDict.ContainsKey(centerCell) && this.CellsDict[centerCell].Type == EditorMapCellData.CellType.PrieDieu)
			{
				result = this.CellsDict[centerCell];
			}
			return result;
		}

		private void ControlZoom()
		{
			if (this.Rewired == null)
			{
				return;
			}
			float axisRaw = this.Rewired.GetAxisRaw(20);
			int num = this.CurrentRendererIndex;
			float num2 = 0.2f;
			if (axisRaw > num2 && this.CurrentRendererIndex != 0)
			{
				num = 0;
			}
			else if (axisRaw < -num2 && this.CurrentRendererIndex == 0)
			{
				num = 1;
			}
			if (num != this.CurrentRendererIndex)
			{
				this.CurrentRendererIndex = num;
				this.UpdateCurrentRenderer();
				this.CurrentRenderer.UpdateMarks(Core.NewMapManager.GetAllMarks());
				this.UpdateZoomControl();
			}
		}

		private void ControlTeleportMovement()
		{
			if (this.teleportMoving && !this.AllowChangeInTween)
			{
				return;
			}
			this.AnglesToCheckNear.Sort();
			List<CellKey> list = new List<CellKey>();
			List<float> list2 = new List<float>();
			for (int i = 0; i < this.AnglesToCheckNear.Count; i++)
			{
				list.Add(new CellKey(0, 0));
				list2.Add(0f);
			}
			float axisRaw = this.Rewired.GetAxisRaw(48);
			float axisRaw2 = this.Rewired.GetAxisRaw(49);
			Vector2 to = new Vector2(axisRaw, axisRaw2);
			if (to.magnitude >= this.TeleportMovementEpsilon)
			{
				to.Normalize();
				CellKey centerCell = this.CurrentRenderer.GetCenterCell();
				Vector2Int vector = centerCell.GetVector2();
				foreach (KeyValuePair<CellKey, CellData> keyValuePair in this.CellsDict)
				{
					if (!centerCell.Equals(keyValuePair.Key) && keyValuePair.Value.Type == EditorMapCellData.CellType.PrieDieu)
					{
						Vector2Int v = keyValuePair.Key.GetVector2() - vector;
						float num = Vector2.Angle(v, to);
						float num2 = (float)v.sqrMagnitude;
						for (int j = 0; j < this.AnglesToCheckNear.Count; j++)
						{
							if (num <= this.AnglesToCheckNear[j] && (list2[j] == 0f || num2 < list2[j]))
							{
								list2[j] = num2;
								list[j] = keyValuePair.Key;
							}
						}
					}
				}
				string text = this.TeleportSoundNotFound;
				for (int k = 0; k < this.AnglesToCheckNear.Count; k++)
				{
					if (list2[k] != 0f)
					{
						text = this.TeleportSoundFound;
						this.MoveTeleport(list[k]);
						break;
					}
				}
				if (text != string.Empty)
				{
					if (text == this.TeleportSoundNotFound)
					{
						if (!this.canPlayTeleportNotFoundSound)
						{
							return;
						}
						this.canPlayTeleportNotFoundSound = false;
					}
					Core.Audio.PlayOneShot(text, default(Vector3));
					return;
				}
			}
			else
			{
				this.canPlayTeleportNotFoundSound = true;
			}
		}

		private void MoveTeleport(CellKey target)
		{
			if (this.teleportMoving)
			{
				if (!this.AllowChangeInTween || this.TeleportTarget.Equals(target))
				{
					return;
				}
				if (this.TeleportTween != null)
				{
					this.TeleportTween.Kill(false);
				}
			}
			this.EndCursor();
			this.teleportMoving = true;
			this.TeleportTarget = target;
			Vector2 vector = -this.CurrentRenderer.GetPosition(target);
			float num = this.TeleportTime;
			if (this.UseSpeedInTeleport)
			{
				Vector2 b = this.CurrentRenderer.Center;
				num = (vector - b).magnitude / this.TeleportSpeed;
			}
			DOTween.defaultTimeScaleIndependent = true;
			this.TeleportTween = DOTween.To(() => this.CurrentRenderer.Center, delegate(Vector3Wrapper x)
			{
				this.CurrentRenderer.Center = x;
			}, new Vector3(vector.x, vector.y, 0f), num).OnComplete(new TweenCallback(this.EndMoveTeleport)).SetEase(this.TeleportEase);
			this.TeleportCursorTween = DOTween.To(() => this.TeleportTransform.sizeDelta, delegate(Vector2Wrapper x)
			{
				this.TeleportTransform.sizeDelta = x;
			}, this.TeleportStart * 2f, num / 2f).SetLoops(2, LoopType.Yoyo);
		}

		private void EndMoveTeleport()
		{
			this.teleportMoving = false;
		}

		private void EndCursor()
		{
			if (this.TeleportCursorTween != null && this.TeleportCursorTween.IsActive())
			{
				this.TeleportCursorTween.Kill(false);
			}
			this.TeleportTransform.sizeDelta = this.TeleportStart;
		}

		private void UpdateZoomControl()
		{
			this.zoomIconInput.axisCheck = ((this.CurrentRendererIndex != 0) ? InputIcon.AxisCheck.Positive : InputIcon.AxisCheck.Negative);
			this.zoomIconInput.RefreshIcon();
		}

		private void UpdateCellData()
		{
			CellData currentCell = this.CurrentCell;
			if (this.CurrentKey == null || !this.CellsDict.TryGetValue(this.CurrentKey, out this.CurrentCell))
			{
				this.CurrentCell = null;
			}
			if (currentCell != null && (this.CurrentCell == null || currentCell.ZoneId != this.CurrentCell.ZoneId))
			{
				this.SetSelection(currentCell.ZoneId, false);
			}
			if (this.CurrentCell != null && (currentCell == null || currentCell.ZoneId != this.CurrentCell.ZoneId))
			{
				this.SetSelection(this.CurrentCell.ZoneId, true);
			}
			if (this.CurrentCell == null)
			{
				this.DistrictText.text = string.Empty;
				this.ZoneText.text = string.Empty;
				return;
			}
			this.DistrictText.text = Core.NewMapManager.GetDistrictName(this.CurrentCell.ZoneId.District).ToUpper();
			this.ZoneText.text = Core.NewMapManager.GetZoneName(this.CurrentCell.ZoneId);
		}

		private void UpdateScrollToCurrent()
		{
			Canvas.ForceUpdateCanvases();
			float num = (float)(Mathf.FloorToInt((float)(this.currentMarkItem / this.visibleMarksInScroll)) * this.visibleMarksInScroll) * this.markUISize;
			this.scrollRect.content.anchoredPosition = new Vector2(-num, this.scrollRect.content.anchoredPosition.y);
		}

		private void SetSelection(ZoneKey zone, bool value)
		{
			if (this.KeysByZone.ContainsKey(zone) && this.CurrentRenderer != null)
			{
				this.CurrentRenderer.SetSelected(this.KeysByZone[zone], value);
			}
		}

		[BoxGroup("Elements", true, false, 0)]
		public Text DistrictText;

		[BoxGroup("Elements", true, false, 0)]
		public BasicUIBlockingWidget ParentWidget;

		[BoxGroup("Elements", true, false, 0)]
		public Text ZoneText;

		[BoxGroup("Elements", true, false, 0)]
		public Text PercentText;

		[BoxGroup("Elements", true, false, 0)]
		public Text CherubsText;

		[BoxGroup("Elements", true, false, 0)]
		public Transform MapContent;

		[BoxGroup("Elements", true, false, 0)]
		public GameObject CursorElement;

		[BoxGroup("Elements", true, false, 0)]
		public GameObject CursorTeleportElement;

		[BoxGroup("Config", true, false, 0)]
		public List<MapRendererConfig> RendererConfigs;

		[BoxGroup("Elements", true, false, 0)]
		public GameObject MapControlsRootNormal;

		[BoxGroup("Elements", true, false, 0)]
		public GameObject MapControlsRootTeleport;

		[BoxGroup("Elements", true, false, 0)]
		public GameObject MapControlsNotMap;

		[BoxGroup("Elements", true, false, 0)]
		public GameObject selectMarkRoot;

		[BoxGroup("Elements", true, false, 0)]
		public Image MarkFrameImage;

		[BoxGroup("Elements", true, false, 0)]
		public GameObject markTemplateElement;

		[BoxGroup("Elements", true, false, 0)]
		public GameObject noMapText;

		[BoxGroup("Elements", true, false, 0)]
		public InputIcon zoomIconInput;

		[BoxGroup("Mark Buttons", true, false, 0)]
		public GameObject addMarkNormalButton;

		[BoxGroup("Mark Buttons", true, false, 0)]
		public GameObject addMarkDisableButton;

		[BoxGroup("Mark Buttons", true, false, 0)]
		public GameObject removeMarkButton;

		[BoxGroup("Teleport Buttons", true, false, 0)]
		public GameObject teleportNormalButton;

		[BoxGroup("Teleport Buttons", true, false, 0)]
		public GameObject teleportDisableButton;

		[BoxGroup("Config Teleport", true, false, 0)]
		public bool UseSpeedInTeleport;

		[BoxGroup("Config Teleport", true, false, 0)]
		[ShowIf("UseSpeedInTeleport", true)]
		public float TeleportSpeed;

		[HideIf("UseSpeedInTeleport", true)]
		[BoxGroup("Config Teleport", true, false, 0)]
		public float TeleportTime;

		[BoxGroup("Config Teleport", true, false, 0)]
		public bool AllowChangeInTween;

		[BoxGroup("Config Teleport", true, false, 0)]
		public Ease TeleportEase;

		[BoxGroup("Config Teleport", true, false, 0)]
		public float TeleportMovementEpsilon;

		[BoxGroup("Config Teleport", true, false, 0)]
		[EventRef]
		public string TeleportSoundFound;

		[BoxGroup("Config Teleport", true, false, 0)]
		[EventRef]
		public string TeleportSoundNotFound;

		[BoxGroup("Config Teleport", true, false, 0)]
		public List<float> AnglesToCheckNear;

		private List<CellData> Cells;

		private Dictionary<CellKey, CellData> CellsDict;

		private Dictionary<ZoneKey, List<CellKey>> KeysByZone;

		private List<MapRenderer> MapRenderers;

		private int CurrentRendererIndex;

		private MapRenderer CurrentRenderer;

		private Player Rewired;

		private ScrollRect scrollRect;

		private bool canAddMark;

		private bool canRemoveMark;

		private CellKey CurrentKey;

		private CellData CurrentCell;

		private bool IsEnableMarkSelector;

		private List<NewMapMenuWidgetMarkItem> markItems;

		private int currentMarkItem;

		private const float MovementEpsilon = 0.08f;

		private int visibleMarksInScroll;

		private float markUISize;

		private bool mapEnabled;

		private bool teleportMoving;

		private CellKey TeleportTarget;

		private Tweener TeleportTween;

		private Tweener TeleportCursorTween;

		private RectTransform TeleportTransform;

		private Vector2 TeleportStart;

		private bool canPlayTeleportNotFoundSound;

		private float markCurrentCooldownTime;

		private const float MARK_MAX_COOLDOWN_TIME = 0.5f;

		private Text itemsText;
	}
}
