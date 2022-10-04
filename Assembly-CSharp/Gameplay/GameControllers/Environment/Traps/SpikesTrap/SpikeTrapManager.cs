using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.SpikesTrap
{
	public class SpikeTrapManager : MonoBehaviour
	{
		private void Start()
		{
			this.OrderSpikesTraps();
			this.isRise = false;
			this.spikesCicleCounter = 0;
		}

		private void Update()
		{
			this.deltaDelayTime += Time.deltaTime;
			switch (this.riseMode)
			{
			case SpikeTrapManager.RiseMode.Carrousel:
				this.runCarrousel();
				break;
			case SpikeTrapManager.RiseMode.Alternate:
				this.runAlternate();
				break;
			case SpikeTrapManager.RiseMode.Simultaneous:
				this.runSimultaneous();
				break;
			}
		}

		public void OrderSpikesTraps()
		{
			if (this.spikeTraps.Length > 0)
			{
				if (this.horizontalOrder)
				{
					this.spikeTraps = (from singleSpikeTrap in this.spikeTraps
					orderby singleSpikeTrap.transform.position.x
					select singleSpikeTrap).ToArray<SpikeTrap>();
				}
				else
				{
					this.spikeTraps = (from singleSpikeTrap in this.spikeTraps
					orderby singleSpikeTrap.transform.position.y
					select singleSpikeTrap).ToArray<SpikeTrap>();
				}
			}
		}

		private IEnumerator Rise(float delayTime, bool rise = true)
		{
			for (int i = 0; i < this.spikeTraps.Length; i++)
			{
				this.spikeTraps[i].RiseSpikes(rise);
				yield return new WaitForSeconds(delayTime);
			}
			if (this.riseMode == SpikeTrapManager.RiseMode.Carrousel)
			{
				this.deltaDelayTime = 0f;
				this.outSpread = !this.outSpread;
			}
			this.isRise = rise;
			yield break;
		}

		private IEnumerator RiseOdd(float delayTime, bool rise = true)
		{
			this.isRise = rise;
			this.spikesCicleCounter++;
			int spikesTurn = this.spikesCicleCounter % 4;
			for (int i = 0; i < this.spikeTraps.Length; i++)
			{
				switch (spikesTurn)
				{
				case 0:
					if (i % 2 != 0)
					{
						this.spikeTraps[i].RiseSpikes(true);
					}
					break;
				case 1:
					if (i % 2 != 0)
					{
						this.spikeTraps[i].RiseSpikes(false);
					}
					break;
				case 2:
					if (i % 2 == 0)
					{
						this.spikeTraps[i].RiseSpikes(true);
					}
					break;
				case 3:
					if (i % 2 == 0)
					{
						this.spikeTraps[i].RiseSpikes(false);
					}
					break;
				}
				yield return new WaitForSeconds(delayTime);
			}
			yield break;
		}

		protected void runCarrousel()
		{
			if (this.deltaDelayTime >= this.delayTime && !this.outSpread)
			{
				this.outSpread = true;
				base.StartCoroutine(this.Rise(this.interval, !this.isRise));
			}
		}

		protected void runAlternate()
		{
			if (this.deltaDelayTime >= this.delayTime)
			{
				this.deltaDelayTime = 0f;
				base.StartCoroutine(this.RiseOdd(0f, !this.isRise));
			}
		}

		protected void runSimultaneous()
		{
			if (this.deltaDelayTime >= this.delayTime)
			{
				this.deltaDelayTime = 0f;
				base.StartCoroutine(this.Rise(0f, !this.isRise));
			}
		}

		public bool horizontalOrder;

		public bool spikeTrapEnabled;

		public SpikeTrap[] spikeTraps;

		public SpikeTrapManager.RiseMode riseMode;

		public float delayTime;

		protected float deltaDelayTime;

		protected bool isRise;

		public float interval = 0.15f;

		protected bool outSpread;

		protected bool oddRising;

		protected int spikesCicleCounter;

		public enum RiseMode
		{
			Carrousel,
			Alternate,
			Simultaneous
		}
	}
}
