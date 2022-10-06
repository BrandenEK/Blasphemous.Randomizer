using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class LaudesPlatformController : MonoBehaviour
{
	private void Awake()
	{
	}

	[Button(0)]
	public void GetChildrenAnimators()
	{
		this.animators = base.GetComponentsInChildren<Animator>();
	}

	[Button(0)]
	public void ShowAllPlatforms()
	{
		if (this.animators != null)
		{
			foreach (Animator animator in this.animators)
			{
				animator.SetBool("HIDDEN", false);
			}
		}
	}

	[Button(0)]
	public void HideAllPlatforms()
	{
		if (this.animators != null)
		{
			foreach (Animator animator in this.animators)
			{
				animator.SetBool("HIDDEN", true);
			}
		}
	}

	public Animator[] animators;
}
