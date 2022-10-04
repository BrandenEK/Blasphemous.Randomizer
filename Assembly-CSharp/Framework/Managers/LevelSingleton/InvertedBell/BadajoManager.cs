using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Managers.LevelSingleton.InvertedBell
{
	public class BadajoManager : MonoBehaviour
	{
		private void Awake()
		{
			this.mat = base.GetComponent<MeshRenderer>().material;
		}

		private void Start()
		{
		}

		private void CheckFlags()
		{
			if (Core.Events.GetFlag("BELL_PUZZLE1_ACTIVATED"))
			{
				Debug.Log(" PUZZLE 1 ACTIVATED");
				foreach (GameObject gameObject in this.leftChainStuff)
				{
					gameObject.SetActive(false);
				}
			}
			if (Core.Events.GetFlag("BELL_PUZZLE2_ACTIVATED"))
			{
				Debug.Log(" PUZZLE 2 ACTIVATED");
				foreach (GameObject gameObject2 in this.rightChainStuff)
				{
					gameObject2.SetActive(false);
				}
			}
		}

		public void PlayFeedback()
		{
		}

		public void PlayReactionRight()
		{
			this.Animator.Play("REACTION_RIGHT");
		}

		public void PlayReactionLeft()
		{
			this.Animator.Play("REACTION_LEFT");
		}

		public void PlayBreak()
		{
			this.Animator.Play("BREAK");
			base.StartCoroutine(this.LerpMatValue(3f, new Action(this.Callback)));
		}

		private void Callback()
		{
			Core.Logic.CameraManager.ProCamera2DShake.Shake(5f, new Vector3(2f, 2f, 0f), 100, 0.25f, 0f, default(Vector3), 0.06f, true);
		}

		private IEnumerator LerpMatValue(float maxSeconds, Action callback)
		{
			float counter = 0f;
			while (counter < maxSeconds)
			{
				float v = Mathf.Lerp(1f, 0.3f, counter / maxSeconds);
				this.mat.SetFloat("_Multiply", v);
				counter += Time.deltaTime;
				yield return null;
			}
			callback();
			yield break;
		}

		public Animator Animator;

		private Material mat;

		public List<GameObject> leftChainStuff;

		public List<GameObject> rightChainStuff;
	}
}
