using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorOffset : MonoBehaviour
{
	private void Awake()
	{
		this._animator = base.GetComponent<Animator>();
		this._animator.speed = 1f + Random.Range(this.minOffset, this.maxOffset);
	}

	private Animator _animator;

	public float minOffset = -0.2f;

	public float maxOffset = 0.1f;
}
