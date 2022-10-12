using System;
using System.Collections;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PietyMonster.Attack
{
	public class PietyRootsManager : MonoBehaviour
	{
		public PietyMonster PietyMonster { get; set; }

		public BoxCollider2D Collider { get; set; }

		public GameObject Target { get; set; }

		public float RootDamage { get; set; }

		private void Start()
		{
			this.Collider = base.GetComponent<BoxCollider2D>();
			if (this.PietyRootPrefab == null)
			{
				Debug.LogError("A Piety Monster's root prefab is needed");
			}
		}

		private void Update()
		{
			if (this.PietyMonster == null)
			{
				this.PietyMonster = UnityEngine.Object.FindObjectOfType<PietyMonster>();
			}
		}

		public void EnableNearestRoots()
		{
			float missingRatio = this.PietyMonster.Stats.Life.MissingRatio;
			int rootsAmount = ((double)missingRatio < 0.5) ? 3 : 2;
			base.StartCoroutine(this.SpawnFollowingRoots(rootsAmount));
		}

		public void EnableDominoRoots()
		{
			float missingRatio = this.PietyMonster.Stats.Life.MissingRatio;
			int rootsAmount = (missingRatio < 0.5f) ? 5 : 3;
			base.StartCoroutine(this.SpawnLeftDominoRoots(rootsAmount));
			base.StartCoroutine(this.SpawnRighDominoRoots(rootsAmount));
		}

		private IEnumerator SpawnFollowingRoots(int rootsAmount)
		{
			float yPos = this.Collider.bounds.min.y;
			for (int i = 0; i < rootsAmount; i++)
			{
				float xPos = this.GetTargetXPosition();
				Vector2 rootPosition = new Vector2(xPos, yPos);
				if (!this.RootIsTooCloseToLastFollowingRoot(rootPosition))
				{
					if (!this.RootIsTooCloseToBoss(rootPosition))
					{
						this.GetRoot(rootPosition);
						this._lastRootPositions.Add(rootPosition);
					}
				}
				else
				{
					Vector3 nearestRootToLastRoot = this.GetNearestRootToLastRoot(rootPosition);
					if (!this.RootIsTooCloseToBoss(nearestRootToLastRoot))
					{
						this.GetRoot(nearestRootToLastRoot);
						this._lastRootPositions.Add(nearestRootToLastRoot);
					}
				}
				yield return new WaitForSeconds(0.5f);
			}
			this._lastRootPositions.Clear();
			yield break;
		}

		public float GetTargetXPosition()
		{
			if (this.Target == null)
			{
				return base.transform.position.x;
			}
			float num = Mathf.Floor(this.Target.transform.position.x * 32f) / 32f;
			if (num >= this.Collider.bounds.max.x)
			{
				num = this.Collider.bounds.max.x - 0.5f;
			}
			else if (num <= this.Collider.bounds.min.x)
			{
				num = this.Collider.bounds.min.x + 0.5f;
			}
			return num;
		}

		private IEnumerator SpawnRighDominoRoots(int rootsAmount)
		{
			float startXPos = this.GetDominoRightStartPos();
			float yPos = this.Collider.bounds.min.y;
			float rightXBoundary = this.Collider.bounds.max.x;
			for (int i = 0; i < rootsAmount; i++)
			{
				if (startXPos + (float)i > rightXBoundary)
				{
					break;
				}
				Vector2 rootPos = new Vector2(startXPos + (float)i * this.DominoRootsOffset, yPos);
				this.GetRoot(rootPos);
				yield return new WaitForSeconds(0.1f);
			}
			yield break;
		}

		private IEnumerator SpawnLeftDominoRoots(int rootsAmount)
		{
			float startXPos = this.GetDominoLeftStartPos();
			float yPos = this.Collider.bounds.min.y;
			float leftXBoundary = this.Collider.bounds.min.x;
			WaitForSeconds delay = new WaitForSeconds(0.1f);
			for (int i = 0; i < rootsAmount; i++)
			{
				if (startXPos - (float)i < leftXBoundary)
				{
					break;
				}
				Vector2 rootPos = new Vector2(startXPos - (float)i * this.DominoRootsOffset, yPos);
				this.GetRoot(rootPos);
				yield return delay;
			}
			yield break;
		}

		private bool RootIsTooCloseToBoss(Vector2 rootPos)
		{
			bool result = false;
			Vector3 position = this.PietyMonster.transform.position;
			if (this.PietyMonster.Status.Orientation == EntityOrientation.Left)
			{
				if (rootPos.x < position.x && Vector2.Distance(position, rootPos) < this.MinSpawnRootDistance)
				{
					result = true;
				}
			}
			else if (rootPos.x > position.x && Vector2.Distance(position, rootPos) < this.MinSpawnRootDistance)
			{
				result = true;
			}
			return result;
		}

		private bool RootIsTooCloseToLastFollowingRoot(Vector2 rootPos)
		{
			bool result = false;
			if (this._lastRootPositions.Count > 0)
			{
				Vector2 b = this._lastRootPositions[this._lastRootPositions.Count - 1];
				float num = Vector2.Distance(rootPos, b);
				result = (num < this.MinDistanceBetweenFollowingRoots);
			}
			return result;
		}

		private Vector3 GetNearestRootToLastRoot(Vector2 targetPos)
		{
			Vector2 v = targetPos;
			Vector2 vector = this._lastRootPositions[this._lastRootPositions.Count - 1];
			if (vector.x >= targetPos.x)
			{
				float x = (vector.x - this.MinDistanceBetweenFollowingRoots <= this.Collider.bounds.min.x) ? (vector.x + this.MinDistanceBetweenFollowingRoots) : (vector.x - this.MinDistanceBetweenFollowingRoots);
				v = new Vector2(x, vector.y);
			}
			else
			{
				float x2 = (vector.x + this.MinDistanceBetweenFollowingRoots <= this.Collider.bounds.max.x) ? (vector.x + this.MinDistanceBetweenFollowingRoots) : (vector.x - this.MinDistanceBetweenFollowingRoots);
				v = new Vector2(x2, vector.y);
			}
			return v;
		}

		public float GetDominoRightStartPos()
		{
			return this.PietyMonster.PietyBehaviour.SmashAttack.PietySmash.AttackAreas[0].WeaponCollider.bounds.max.x;
		}

		public float GetDominoLeftStartPos()
		{
			return this.PietyMonster.PietyBehaviour.SmashAttack.PietySmash.AttackAreas[0].WeaponCollider.bounds.min.x;
		}

		public void GetRoot(Vector2 rootPosition)
		{
			if (this._pietyRoots.Count > 0)
			{
				GameObject gameObject = this._pietyRoots[this._pietyRoots.Count - 1];
				this._pietyRoots.Remove(gameObject);
				gameObject.SetActive(true);
				gameObject.transform.position = rootPosition;
			}
			else
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.PietyRootPrefab, rootPosition, Quaternion.identity);
				PietyRoot component = gameObject2.GetComponent<PietyRoot>();
				AttackArea component2 = gameObject2.GetComponent<AttackArea>();
				component2.Entity = this.PietyMonster;
				component.Manager = this;
			}
		}

		public void StoreRoot(GameObject pietyRoot)
		{
			if (!this._pietyRoots.Contains(pietyRoot))
			{
				this._pietyRoots.Add(pietyRoot);
			}
		}

		private void OnTriggerEnter2D(Collider2D target)
		{
			if ((this.TargetLayer.value & 1 << target.gameObject.layer) > 0)
			{
				this.Target = target.gameObject;
			}
		}

		public void EnablePietyRoot(PietyRoot pietyRoot)
		{
			if (!pietyRoot.gameObject.activeSelf)
			{
				pietyRoot.gameObject.SetActive(true);
			}
		}

		public void DisablePietyRoot(PietyRoot pietyRoot)
		{
			if (pietyRoot.gameObject.activeSelf)
			{
				pietyRoot.gameObject.SetActive(false);
			}
		}

		private List<GameObject> _pietyRoots = new List<GameObject>();

		public LayerMask TargetLayer;

		public GameObject PietyRootPrefab;

		public float MinSpawnRootDistance = 3f;

		public float MinDistanceBetweenFollowingRoots = 2f;

		private List<Vector2> _lastRootPositions = new List<Vector2>();

		[Range(0f, 2f)]
		public float DominoRootsOffset = 1.5f;
	}
}
