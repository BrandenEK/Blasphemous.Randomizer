using System;
using System.Collections.Generic;
using Framework.FrameworkCore.Attributes;
using Framework.Inventory;
using Framework.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.UIGameLogic
{
	public class PlayerFlask : MonoBehaviour
	{
		private void Start()
		{
			int num = 0;
			Transform transform;
			do
			{
				transform = base.transform.Find("Flask" + num.ToString());
				if (transform)
				{
					this.flasks.Add(transform.GetComponent<Image>());
				}
				num++;
			}
			while (transform != null);
			this.RefreshFlask();
		}

		private void Update()
		{
			this.RefreshFlask();
		}

		private void RefreshFlask()
		{
			if (Core.Logic == null || Core.Logic.Penitent == null)
			{
				return;
			}
			Flask flask = Core.Logic.Penitent.Stats.Flask;
			int num = (int)(Core.Logic.Penitent.Stats.FlaskHealth.PermanetBonus / Core.Logic.Penitent.Stats.FlaskHealthUpgrade);
			if (num > this.flasksEmpty.Count)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"PlayerFlask::RefreshFlask: You have upgraded the flasks more than ",
					this.flasksEmpty.Count,
					" times and we don't have sprites for those upgraded flasks! Current flask upgrade level: ",
					num
				}));
				num = this.flasksEmpty.Count;
			}
			if (this.swordHeart06 == null)
			{
				this.swordHeart06 = Core.InventoryManager.GetSword("HE06");
			}
			if (this.swordHeart06 != null && this.swordHeart06.IsEquiped)
			{
				for (int i = 0; i < this.flasks.Count; i++)
				{
					this.flasks[i].gameObject.SetActive(false);
				}
				flask.Current = 0f;
			}
			else
			{
				if (this.currentFlaskNumber == flask.Final && this.currentFlaskFull == flask.Current && this.currentFlaskLevel == (float)num && this.flasks[0].gameObject.activeInHierarchy && this.currentFlaskIsFervour == Core.PenitenceManager.UseFervourFlasks)
				{
					return;
				}
				this.currentFlaskIsFervour = Core.PenitenceManager.UseFervourFlasks;
				this.currentFlaskNumber = flask.Final;
				this.currentFlaskFull = flask.Current;
				this.currentFlaskLevel = (float)num;
				for (int j = 0; j < this.flasks.Count; j++)
				{
					if ((float)j < this.currentFlaskFull)
					{
						if (Core.PenitenceManager.UseFervourFlasks)
						{
							this.flasks[j].sprite = this.flasksFullFervour[num];
						}
						else
						{
							this.flasks[j].sprite = this.flasksFull[num];
						}
						this.flasks[j].gameObject.SetActive(true);
					}
					else if ((float)j < this.currentFlaskNumber)
					{
						this.flasks[j].sprite = this.flasksEmpty[num];
						this.flasks[j].gameObject.SetActive(true);
					}
					else
					{
						this.flasks[j].gameObject.SetActive(false);
					}
				}
			}
		}

		[SerializeField]
		public List<Sprite> flasksFull;

		[SerializeField]
		public List<Sprite> flasksEmpty;

		[SerializeField]
		public List<Sprite> flasksFullFervour;

		private List<Image> flasks = new List<Image>();

		private float currentFlaskNumber = -1f;

		private float currentFlaskFull = -1f;

		private float currentFlaskLevel = -1f;

		private bool currentFlaskIsFervour;

		private Sword swordHeart06;
	}
}
