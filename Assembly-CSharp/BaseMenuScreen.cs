using System;
using UnityEngine;

public class BaseMenuScreen : MonoBehaviour
{
	public virtual void Open()
	{
	}

	public virtual void Close()
	{
	}

	protected virtual void OnOpen()
	{
	}

	protected virtual void OnClose()
	{
	}
}
