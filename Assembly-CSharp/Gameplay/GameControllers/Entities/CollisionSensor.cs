using System;
using System.Collections.Generic;
using System.Diagnostics;
using Framework.Managers;
using Tools.Level.Interactables;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	[RequireComponent(typeof(Collider2D))]
	public class CollisionSensor : MonoBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event CollisionSensor.CollisionEvent SensorTriggerEnter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event CollisionSensor.CollisionEvent SensorTriggerExit;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event CollisionSensor.CollisionEvent SensorTriggerStay;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event CollisionSensor.CollisionEvent OnColliderEnter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event CollisionSensor.CollisionEvent OnColliderExit;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event CollisionSensor.EntityCollisionEvent OnEntityEnter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event CollisionSensor.EntityCollisionEvent OnEntityExit;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnPenitentEnter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnPenitentExit;

		private void Awake()
		{
			this.SensorCollider2D = base.GetComponent<Collider2D>();
			if (base.GetComponentInParent<Door>() != null)
			{
				BoxCollider2D boxCollider2D = (BoxCollider2D)this.SensorCollider2D;
				if (boxCollider2D.size.y > boxCollider2D.size.x)
				{
					boxCollider2D.size = new Vector2((boxCollider2D.size.x >= 0.75f) ? boxCollider2D.size.x : 0.75f, 4f);
				}
			}
		}

		public Collider2D SensorCollider2D { get; private set; }

		private void OnTriggerEnter2D(Collider2D col)
		{
			if ((this.sensorLayerDetection.value & 1 << col.gameObject.layer) > 0)
			{
				this.objectList.Add(col.gameObject);
				if (this.SensorTriggerEnter != null && !col.CompareTag("Sensor"))
				{
					this.SensorTriggerEnter(col);
				}
				if (this.OnPenitentEnter != null && col.CompareTag("Penitent"))
				{
					this.OnPenitentEnter();
				}
				Entity componentInParent = col.GetComponentInParent<Entity>();
				if (componentInParent != null && !col.CompareTag("Sensor"))
				{
					this.entityList.Add(componentInParent);
				}
				if (componentInParent != null && this.OnEntityEnter != null && !col.CompareTag("Sensor"))
				{
					this.OnEntityEnter(componentInParent);
				}
			}
		}

		private void OnTriggerExit2D(Collider2D col)
		{
			if ((this.sensorLayerDetection.value & 1 << col.gameObject.layer) > 0)
			{
				this.objectList.Remove(col.gameObject);
				if (this.SensorTriggerExit != null)
				{
					this.SensorTriggerExit(col);
				}
				if (this.OnPenitentExit != null && col.CompareTag("Penitent"))
				{
					this.OnPenitentExit();
				}
				Entity componentInParent = col.GetComponentInParent<Entity>();
				if (componentInParent != null)
				{
					this.entityList.Remove(componentInParent);
				}
				if (componentInParent != null && this.OnEntityExit != null)
				{
					this.OnEntityExit(componentInParent);
				}
			}
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if ((this.sensorLayerDetection.value & 1 << other.gameObject.layer) > 0 && this.SensorTriggerStay != null)
			{
				this.SensorTriggerStay(other);
			}
		}

		public bool IsColliding()
		{
			return this.objectList.Count > 0;
		}

		public Entity[] GetTouchedEntities()
		{
			return this.entityList.ToArray();
		}

		private void OnDisable()
		{
			this.objectList.Clear();
			this.entityList.Clear();
		}

		public LayerMask sensorLayerDetection = 32;

		private List<GameObject> objectList = new List<GameObject>();

		private List<Entity> entityList = new List<Entity>();

		public delegate void CollisionEvent(Collider2D objectCollider);

		public delegate void EntityCollisionEvent(Entity entity);
	}
}
