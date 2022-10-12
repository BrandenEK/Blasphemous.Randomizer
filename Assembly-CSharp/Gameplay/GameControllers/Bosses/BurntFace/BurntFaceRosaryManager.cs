using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using BezierSplines;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.BurntFace.Rosary;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BurntFace
{
	public class BurntFaceRosaryManager : MonoBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<BurntFaceRosaryManager> OnPatternEnded;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnAllBeadsDestroyedEvent;

		private void Awake()
		{
			this.InstantiateBeads();
		}

		private void Start()
		{
			this.ConfigureRosary();
			this._lastSpeed = this.speed;
			this._lastRadius = this.radiusOffset;
			this._smoothChangeCounter = this.smoothTime;
			this.SetPatternFromDatabase("EMPTY");
		}

		private void InstantiateBeads()
		{
			for (int i = 0; i < this.maxBeads; i++)
			{
				UnityEngine.Object.Instantiate<GameObject>(this.rosaryBeadPrefab, base.transform);
			}
		}

		private void ConfigureRosary()
		{
			this.beads = new List<BurntFaceRosaryBead>(base.GetComponentsInChildren<BurntFaceRosaryBead>());
			float num = 1f / (float)this.beads.Count;
			for (int i = 0; i < this.beads.Count; i++)
			{
				Vector3 vector = this.beads[i].transform.position - this.center.position;
				this.beads[i].transform.position = this.spline.GetPoint(num * (float)i) + vector.normalized * this.radiusOffset;
				this.beads[i].Init(this);
			}
		}

		public void OnBeadDestroyed(BurntFaceRosaryBead b)
		{
			if (this.AllBeadsDestroyed())
			{
				this.OnAllBeadsDestroyed();
			}
		}

		private void OnAllBeadsDestroyed()
		{
			if (this.OnAllBeadsDestroyedEvent != null)
			{
				this.OnAllBeadsDestroyedEvent();
			}
		}

		public bool AllBeadsDestroyed()
		{
			bool flag = true;
			foreach (BurntFaceRosaryBead burntFaceRosaryBead in this.beads)
			{
				flag = (flag && burntFaceRosaryBead.IsDestroyed());
			}
			return flag;
		}

		public void Clear()
		{
			this.EndPattern();
		}

		public void SetPatternFromDatabase(string id)
		{
			BurntFaceRosaryPattern pattern = this.patternDatabase.GetPattern(id);
			this._currentPattern = pattern.ID;
			this._targetSpeed = pattern.maxSpeed;
			this._targetRadius = pattern.radiusOffset;
			this._smoothChangeCounter = 0f;
			this._currentTimer = pattern.activeTime;
			this.SetPattern(pattern);
		}

		private void SetPattern(BurntFaceRosaryPattern pattern)
		{
			foreach (BurntFaceRosaryBead burntFaceRosaryBead in this.beads)
			{
				burntFaceRosaryBead.SetPattern(pattern);
			}
		}

		public void HideBeads()
		{
			foreach (BurntFaceRosaryBead burntFaceRosaryBead in this.beads)
			{
				burntFaceRosaryBead.Hide();
			}
		}

		public void ShowBeads()
		{
			foreach (BurntFaceRosaryBead burntFaceRosaryBead in this.beads)
			{
				burntFaceRosaryBead.Show();
			}
		}

		public void Recharge()
		{
			this.EndPattern();
			base.StartCoroutine(this.RechargeCoroutine());
		}

		private IEnumerator RechargeCoroutine()
		{
			this.isRecharging = true;
			int i = 0;
			while (i < this.beads.Count)
			{
				PoolManager.Instance.ReuseObject(this.rechargeFX, this.beads[i].GetPosition(), Quaternion.identity, false, 1);
				this.beads[i].Regenerate();
				i++;
				yield return new WaitForSeconds(1f);
			}
			this.isRecharging = false;
			yield break;
		}

		public void DestroyAllBeads()
		{
			foreach (BurntFaceRosaryBead burntFaceRosaryBead in this.beads)
			{
				burntFaceRosaryBead.ForceDestroy();
			}
		}

		public void RegenerateAllBeads()
		{
			foreach (BurntFaceRosaryBead burntFaceRosaryBead in this.beads)
			{
				burntFaceRosaryBead.Regenerate();
			}
		}

		private void UpdateSmoothChange()
		{
			if (this._smoothChangeCounter < this.smoothTime)
			{
				this._smoothChangeCounter += Time.deltaTime;
				if (this._smoothChangeCounter > this.smoothTime)
				{
					this._smoothChangeCounter = 1f;
					this._lastSpeed = this._targetSpeed;
					this._lastRadius = this._targetRadius;
				}
				float t = this.smoothCurve.Evaluate(this._smoothChangeCounter / this.smoothTime);
				this.radiusOffset = Mathf.Lerp(this._lastRadius, this._targetRadius, t);
				this.speed = Mathf.Lerp(this._lastSpeed, this._targetSpeed, t);
			}
		}

		private void UpdateBeads()
		{
			this.UpdateSmoothChange();
			this.updateCounter = (this.updateCounter + Time.deltaTime * this.speed) % 1f;
			float num = 1f / (float)this.beads.Count;
			for (int i = 0; i < this.beads.Count; i++)
			{
				float num2 = (num * (float)i + this.updateCounter) % 1f;
				if (num2 < 0f)
				{
					num2 = 1f + num2;
				}
				if (i == 0)
				{
					this.debugLastV = num2;
				}
				BurntFaceRosaryBead burntFaceRosaryBead = this.beads[i];
				Vector3 v = burntFaceRosaryBead.transform.position - this.center.position;
				burntFaceRosaryBead.transform.position = this.spline.GetPoint(num2) + v.normalized * this.radiusOffset;
				burntFaceRosaryBead.SetLaserParentRotation(v);
				float num3 = Mathf.Atan2(v.y, v.x) * 57.29578f;
				if (num3 < 0f)
				{
					num3 += 360f;
				}
				burntFaceRosaryBead.UpdateAngle(num3);
			}
		}

		private void UpdateActiveCounter()
		{
			if (this._currentPattern == "EMPTY")
			{
				return;
			}
			if (this._currentTimer > 0f)
			{
				this._currentTimer -= Time.deltaTime;
			}
			else if (this._currentTimer < 0f)
			{
				this._currentTimer = 0f;
				this.EndPattern();
			}
		}

		private void EndPattern()
		{
			this.SetPatternFromDatabase("EMPTY");
			if (this.OnPatternEnded != null)
			{
				this.OnPatternEnded(this);
			}
		}

		private void Update()
		{
			this.UpdateBeads();
			this.UpdateActiveCounter();
		}

		public float speed;

		private float updateCounter;

		public List<BurntFaceRosaryBead> beads;

		public BezierSpline spline;

		public Transform center;

		public BurntFaceRosaryScriptablePattern patternDatabase;

		public GameObject rosaryBeadPrefab;

		public float radiusOffset;

		public int maxBeads = 8;

		public BurntFace owner;

		public float smoothTime = 1.5f;

		public AnimationCurve smoothCurve;

		public GameObject rechargeFX;

		private float _currentTimer;

		private string _currentPattern;

		private float _targetSpeed;

		private float _targetRadius;

		private float _lastRadius;

		private float _lastSpeed;

		private float _smoothChangeCounter;

		private const string DEACTIVATED_ROSARY_PATTERN = "EMPTY";

		public bool isRecharging;

		[FoldoutGroup("Debug", 0)]
		public float debugLastV;
	}
}
