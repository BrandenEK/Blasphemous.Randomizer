using System;
using System.Timers;

namespace Framework.FrameworkCore.Attributes.Logic
{
	public class FinalBonus : BaseAttribute
	{
		public FinalBonus(float time, float baseValue, float baseMultiplier = 1f) : base(baseValue, baseMultiplier)
		{
			this._timer = new Timer((double)time);
			this._timer.Elapsed += this.TimerOnElapsed;
		}

		public void StartTimer(Attribute parentAttribute)
		{
			this._parentAttribute = parentAttribute;
			this._timer.Start();
		}

		private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			this._timer.Stop();
			this._parentAttribute.RemoveFinalBonus(this);
		}

		private Timer _timer;

		private Attribute _parentAttribute;
	}
}
