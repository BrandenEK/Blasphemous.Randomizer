using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class LaudesPlatformController : MonoBehaviour
{
	private void Awake()
	{
	}

	[Button(ButtonSizes.Small)]
	public void GetChildrenAnimators()
	{
		this.animators = base.GetComponentsInChildren<Animator>();
	}

	[Button(ButtonSizes.Small)]
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

	[Button(ButtonSizes.Small)]
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
