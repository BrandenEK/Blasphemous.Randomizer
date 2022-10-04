using System;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.BellGhost
{
	[RequireComponent(typeof(Collider2D))]
	public class BellGhostChasingArea : MonoBehaviour
	{
		public bool ChasingPlayer { get; private set; }

		private void Awake()
		{
		}

		private void Start()
		{
			this.ChasingPlayer = false;
		}

		private void Update()
		{
			if (this.playerEnterInChasingArea)
			{
				this.deltaAlertTime += Time.deltaTime;
				if (this.deltaAlertTime >= this.alertTime && !this.ChasingPlayer)
				{
					this.ChasingPlayer = true;
				}
			}
			else
			{
				this.deltaAlertTime = 0f;
				this.ChasingPlayer = false;
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if ((this.playerLayer.value & 1 << collision.gameObject.layer) > 0 && !this.playerEnterInChasingArea)
			{
				this.playerEnterInChasingArea = true;
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if ((this.playerLayer.value & 1 << collision.gameObject.layer) > 0 && this.playerEnterInChasingArea)
			{
				this.playerEnterInChasingArea = !this.playerEnterInChasingArea;
			}
		}

		public float alertTime = 2f;

		private float deltaAlertTime;

		private bool playerEnterInChasingArea;

		public LayerMask playerLayer;
	}
}
