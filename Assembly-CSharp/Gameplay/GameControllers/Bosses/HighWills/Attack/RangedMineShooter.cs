using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Framework.Managers;
using Framework.Pooling;
using Gameplay.GameControllers.Bosses.PontiffHusk;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.HighWills.Attack
{
	public class RangedMineShooter : MonoBehaviour
	{
		private void Start()
		{
			this.SpriteRenderer = base.GetComponent<SpriteRenderer>();
			this.SpriteRenderer.DOFade(0f, 0f);
		}

		private void Update()
		{
			if (this.CheckIfAllMinesGotDestroyed())
			{
				this.LaunchCrisantaAttack();
			}
			if (!this.shootingMines)
			{
				return;
			}
			this.timePassed += Time.deltaTime;
			float portion = this.timePassed / this.MovementTime;
			this.ShootMineIfNeeded(portion);
			this.UpdatePosition(portion);
		}

		private bool CheckIfAllMinesGotDestroyed()
		{
			bool flag = this.mines.Count == this.MinesData.Count;
			flag = (flag && !this.crisantaAttacked);
			bool result;
			if (flag)
			{
				result = !this.mines.Exists((RangedMine x) => !x.GotDestroyed);
			}
			else
			{
				result = false;
			}
			return result;
		}

		private void LaunchCrisantaAttack()
		{
			Core.Logic.ScreenFreeze.Freeze(this.ParadinhaTimeScale, this.ParadinhaDuration, 0f, null);
			this.mines.Clear();
			PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.CrisantaSimpleVFX, this.PontiffHuskBoss.gameObject.transform.position, Quaternion.identity, true, 1);
			PoolObject componentInChildren = objectInstance.GameObject.GetComponentInChildren<PoolObject>(true);
			componentInChildren.gameObject.SetActive(true);
			componentInChildren.OnObjectReuse();
			this.crisantaAttacked = true;
			base.StartCoroutine(this.WaitAndDamageHW());
		}

		private IEnumerator WaitAndDamageHW()
		{
			yield return new WaitForSeconds(0.5f);
			yield break;
		}

		private void ShootMineIfNeeded(float portion)
		{
			int num = this.lastMineIndex + 1;
			if (this.MinesData[num].MovementPortionToSpawn < portion)
			{
				this.lastMineIndex++;
				PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.MinesData[num].MinePrefab, base.transform.position, Quaternion.Euler(0f, 0f, -90f), true, this.MinesData.Count * 3);
				RangedMine component = objectInstance.GameObject.GetComponent<RangedMine>();
				this.mines.Add(component);
				RangedMine priorMine = null;
				foreach (RangedMine rangedMine in this.mines)
				{
					rangedMine.SetPriorMine(priorMine);
					priorMine = rangedMine;
				}
				if (num == this.MinesData.Count - 1)
				{
					this.shootingMines = false;
					base.StartCoroutine(this.WaitAndFade());
				}
			}
		}

		private IEnumerator WaitAndFade()
		{
			yield return new WaitForSeconds(this.TimeToFadeAfterFinalMine);
			this.SpriteRenderer.DOFade(0f, this.FadeTime);
			yield break;
		}

		private void UpdatePosition(float portion)
		{
			float num = this.VerticalMovementWhileShooting.Evaluate(portion);
			float d = num * this.EndPointRelativeHeight;
			float d2 = this.timePassed * 3f;
			base.transform.position = this.startPos + Vector3.up * d + Vector3.right * d2;
		}

		public void StartShootingMines(Vector3 startPos)
		{
			this.startPos = startPos;
			base.transform.position = startPos;
			this.SpriteRenderer.DOFade(1f, 0.1f).OnComplete(new TweenCallback(this.ShootMines));
		}

		private void ShootMines()
		{
			this.shootingMines = true;
			this.crisantaAttacked = false;
			this.lastMineIndex = -1;
			this.timePassed = 0f;
			this.mines.Clear();
		}

		[HideInInspector]
		public PontiffHuskBoss PontiffHuskBoss;

		public GameObject CrisantaSimpleVFX;

		public SpriteRenderer SpriteRenderer;

		public List<RangedMineShooter.MineData> MinesData;

		public AnimationCurve VerticalMovementWhileShooting;

		public float MovementTime;

		public float EndPointRelativeHeight;

		public float TimeToFadeAfterFinalMine = 0.25f;

		public float FadeTime = 0.1f;

		public float ParadinhaTimeScale = 0.05f;

		public float ParadinhaDuration = 0.4f;

		private Vector3 startPos;

		private bool shootingMines;

		private float timePassed;

		private int lastMineIndex;

		private List<RangedMine> mines = new List<RangedMine>();

		private bool crisantaAttacked;

		[Serializable]
		public struct MineData
		{
			public GameObject MinePrefab;

			public float MovementPortionToSpawn;
		}
	}
}
