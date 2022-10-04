using System;
using System.Collections;
using FMODUnity;
using UnityEngine;

namespace Framework.Audio
{
	[RequireComponent(typeof(StudioEventEmitter))]
	public class LayeredAudio : MonoBehaviour
	{
		private void Start()
		{
			this.emitter = base.GetComponent<StudioEventEmitter>();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!this.active)
			{
				base.StartCoroutine(this.Actions());
				this.active = true;
			}
		}

		private IEnumerator Actions()
		{
			this.emitter.Play();
			yield return new WaitForSeconds(10f);
			this.SetIntensity(1f);
			this.text.text = "INTENSITY LEVEL: 1";
			yield return new WaitForSeconds(10f);
			this.SetIntensity(2f);
			this.text.text = "INTENSITY LEVEL: 2";
			yield return new WaitForSeconds(10f);
			this.SetIntensity(3f);
			this.text.text = "INTENSITY LEVEL: 3";
			yield return new WaitForSeconds(10f);
			this.SetIntensity(4f);
			this.text.text = "INTENSITY LEVEL: 4";
			yield return new WaitForSeconds(10f);
			this.SetIntensity(5f);
			this.text.text = "INTENSITY LEVEL: 5";
			yield return new WaitForSeconds(10f);
			this.SetIntensity(6f);
			this.text.text = "INTENSITY LEVEL: 6";
			yield return new WaitForSeconds(10f);
			this.SetIntensity(7f);
			this.text.text = "INTENSITY LEVEL: 7";
			yield return new WaitForSeconds(10f);
			this.SetIntensity(8f);
			this.text.text = "INTENSITY LEVEL: 8";
			yield return new WaitForSeconds(10f);
			this.SetEnding();
			this.text.text = "ENDING PHASE";
			yield break;
		}

		public void SetIntensity(float intensity)
		{
			this.emitter.SetParameter("Intensity", intensity);
		}

		public void SetEnding()
		{
			this.emitter.SetParameter("Ending", 1f);
		}

		private bool active;

		private StudioEventEmitter emitter;

		public TextMesh text;
	}
}
