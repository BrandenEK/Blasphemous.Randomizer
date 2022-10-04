using System;
using System.Collections;
using FMODUnity;
using Framework.Managers;
using UnityEngine;

namespace Tools.Audio
{
	public class CreateFxOnEnable : MonoBehaviour
	{
		private void Awake()
		{
			if (this.toInstantiate != null && this.n > 0)
			{
				PoolManager.Instance.CreatePool(this.toInstantiate, this.n);
			}
		}

		private void OnEnable()
		{
			base.StartCoroutine(this.DelayedInstantiation());
		}

		private IEnumerator DelayedInstantiation()
		{
			yield return new WaitForSeconds(this.delay);
			this.InstantiateStuff();
			yield break;
		}

		private void InstantiateStuff()
		{
			if (this.fxOneshotSound != string.Empty)
			{
				Core.Audio.PlayOneShot(this.fxOneshotSound, default(Vector3));
			}
			if (this.toInstantiate != null)
			{
				GameObject gameObject = PoolManager.Instance.ReuseObject(this.toInstantiate, base.transform.position + this.offset, base.transform.rotation, false, 1).GameObject;
				gameObject.transform.localScale = base.transform.localScale;
			}
		}

		public int n;

		public GameObject toInstantiate;

		public float delay = 0.1f;

		public Vector2 offset;

		[EventRef]
		public string fxOneshotSound;
	}
}
