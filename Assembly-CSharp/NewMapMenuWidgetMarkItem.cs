using System;
using Framework.Map;
using UnityEngine;
using UnityEngine.UI;

public class NewMapMenuWidgetMarkItem : MonoBehaviour
{
	public MapData.MarkType MarkId { get; private set; }

	public void SetInitialData(MapData.MarkType id, Sprite sprite, bool selected)
	{
		this.Sprite = base.transform.Find(this.IconChildName).GetComponent<Image>();
		this.Selection = base.transform.Find(this.SelectionChildName).gameObject;
		this.SelectionImage = this.Selection.GetComponent<Image>();
		RectTransform rectTransform = (RectTransform)this.Sprite.transform;
		rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
		this.Sprite.sprite = sprite;
		this.SetSelected(selected);
		this.MarkId = id;
	}

	public void SetSelected(bool selected)
	{
		this.Selection.SetActive(selected);
	}

	public void SetDisabled(bool disabled)
	{
		this.SelectionImage.color = ((!disabled) ? Color.white : Color.gray);
		this.Sprite.color = ((!disabled) ? Color.white : Color.gray);
	}

	public string IconChildName = "Icon";

	public string SelectionChildName = "Selection";

	private Image Sprite;

	private GameObject Selection;

	private Image SelectionImage;
}
