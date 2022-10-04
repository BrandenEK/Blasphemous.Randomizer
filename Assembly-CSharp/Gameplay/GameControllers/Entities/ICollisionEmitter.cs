using System;
using Framework.Util;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public interface ICollisionEmitter
	{
		event EventHandler<Collider2DParam> OnEnter;

		void OnTriggerEnter2DNotify(Collider2D c);

		event EventHandler<Collider2DParam> OnStay;

		void OnTriggerStay2DNotify(Collider2D c);

		event EventHandler<Collider2DParam> OnExit;

		void OnTriggerExit2DNotify(Collider2D c);
	}
}
