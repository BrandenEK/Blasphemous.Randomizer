using System;
using Gameplay.GameControllers.Entities;
using UnityEngine;
using UnityEngine.UI;

public class DisplayDamage : MonoBehaviour
{
	private void Awake()
	{
		this.entity.OnDamageTaken += this.Entity_OnDamaged;
		this.text = base.GetComponent<Text>();
	}

	private void Entity_OnDamaged(float dmg)
	{
		if (Time.time > this.lastHitTime + this.resetTime)
		{
			this.accumDamage = 0f;
			this.totalHits = 0;
		}
		this.totalHits++;
		this.accumDamage += dmg;
		this.text.text = dmg.ToString("0000");
		Text text = this.text;
		text.text += string.Format("\n<color=red>{0:0000} ({1})</color>", this.accumDamage, this.totalHits);
		this.lastHitTime = Time.time;
	}

	public Entity entity;

	public Text text;

	public float resetTime = 2f;

	private float accumDamage;

	private float lastHitTime;

	private int totalHits;
}
