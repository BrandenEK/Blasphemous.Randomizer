using System;
using System.Collections;
using FMODUnity;
using Framework.Managers;
using Framework.Util;
using UnityEngine;

namespace Framework.Audio
{
	public class FMODUtil
	{
		public void LerpParam(StudioEventEmitter emitter, string id, float value, float time)
		{
			Singleton<Core>.Instance.StartCoroutine(this.LerpParamAction(emitter, id, value, time));
		}

		private IEnumerator LerpParamAction(StudioEventEmitter emitter, string id, float value, float time)
		{
			for (float currentTime = 0f; currentTime <= time; currentTime += Time.deltaTime)
			{
				emitter.SetParameter(id, value);
				yield return new WaitForEndOfFrame();
			}
			yield break;
		}
	}
}
