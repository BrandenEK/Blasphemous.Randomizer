using System;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;

public class ShowHideUIItem : MonoBehaviour
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event ShowHideUIItem.ShowHideEvent OnShown;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event ShowHideUIItem.ShowHideEvent OnHidden;

	public virtual void Show()
	{
		if (this.canvasGroup != null)
		{
			this.IsShowing = true;
			this.canvasGroup.DOFade(1f, this.showSeconds).OnComplete(delegate
			{
				this.DoOnShown();
			});
		}
	}

	public virtual void Hide()
	{
		if (this.canvasGroup != null)
		{
			this.IsShowing = false;
			this.canvasGroup.DOFade(0f, this.hideSeconds).OnComplete(delegate
			{
				this.DoOnHidden();
			});
		}
	}

	private void DoOnShown()
	{
		if (this.OnShown != null)
		{
			this.OnShown(this);
		}
	}

	private void DoOnHidden()
	{
		this.IsShowing = false;
		if (this.OnHidden != null)
		{
			this.OnHidden(this);
		}
	}

	public CanvasGroup canvasGroup;

	public float showSeconds = 0.5f;

	public float hideSeconds = 0.5f;

	public bool IsShowing;

	public delegate void ShowHideEvent(ShowHideUIItem item);
}
