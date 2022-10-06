using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.Gizmos;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint.Attack
{
	public class BejeweledSmashHandManager : MonoBehaviour
	{
		public bool HandsUp { get; set; }

		public bool IsBusy { get; set; }

		private void Start()
		{
			BejeweledSmashHand.OnHandDown = (Core.SimpleEvent)Delegate.Combine(BejeweledSmashHand.OnHandDown, new Core.SimpleEvent(this.OnHandDown));
		}

		private void OnDestroy()
		{
			BejeweledSmashHand.OnHandDown = (Core.SimpleEvent)Delegate.Remove(BejeweledSmashHand.OnHandDown, new Core.SimpleEvent(this.OnHandDown));
		}

		private void OnHandDown()
		{
			int num = this.SmashHands.Count(delegate(BejeweledSmashHand x)
			{
				bool flag = false;
				x.IsRaised = flag;
				return flag;
			});
			if (this.SmashHands.Length >= num)
			{
				this.HandsUp = false;
			}
		}

		public Vector3 GetRandomSpawnPoint()
		{
			if (this._pickedUpPositions.Count >= this.HandsSpawnPoint.Length)
			{
				this._pickedUpPositions.Clear();
			}
			int num;
			do
			{
				num = Random.Range(0, this.HandsSpawnPoint.Length);
			}
			while (this._pickedUpPositions.Contains(num));
			this._pickedUpPositions.Add(num);
			return this.HandsSpawnPoint[num].transform.position;
		}

		public void LineAttack(Vector2 origin, Vector2 dir)
		{
			base.StartCoroutine(this.LineAttackCoroutine(origin, dir));
		}

		private IEnumerator LineAttackCoroutine(Vector2 origin, Vector2 dir)
		{
			this.IsBusy = true;
			this.HandsUp = true;
			float offset = 2.5f;
			float delay = 0.4f;
			for (int i = 0; i < this.SmashHands.Length; i++)
			{
				BejeweledSmashHand smashHand = this.SmashHands[i];
				smashHand.transform.position = new Vector2(origin.x, smashHand.transform.position.y) + dir.normalized * offset * (float)i;
				smashHand.AttackAppearing();
				smashHand.IsRaised = true;
				yield return new WaitForSeconds(delay);
			}
			this.IsBusy = false;
			this._pickedUpPositions.Clear();
			yield break;
		}

		public void SmashAttack()
		{
			base.StartCoroutine(this.SmashAttackCoroutine());
		}

		private IEnumerator SmashAttackCoroutine()
		{
			this.HandsUp = true;
			this.IsBusy = true;
			foreach (BejeweledSmashHand smashHand in this.SmashHands)
			{
				smashHand.transform.position = new Vector2(this.GetRandomSpawnPoint().x, smashHand.transform.position.y);
				smashHand.AttackAppearing();
				smashHand.IsRaised = true;
				yield return new WaitForSeconds(1f);
			}
			this.IsBusy = false;
			this._pickedUpPositions.Clear();
			yield break;
		}

		public BejeweledSmashHand[] SmashHands;

		public RootMotionDriver[] HandsSpawnPoint;

		private readonly List<int> _pickedUpPositions = new List<int>();
	}
}
