using System;
using System.Collections.Generic;
using System.Diagnostics;
using Framework.Util;
using Sirenix.Utilities;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	[RequireComponent(typeof(Collider2D))]
	public class TriggerSensor : MonoBehaviour, ICollisionEmitter
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event TriggerSensor.ColliderTriggerEvent OnColliderEnter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event TriggerSensor.ColliderTriggerEvent OnColliderExit;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event TriggerSensor.EntityTriggerEvent OnEntityEnter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event TriggerSensor.EntityTriggerEvent OnEntityExit;

		private void Awake()
		{
			this.SensorCollider2D = base.GetComponent<Collider2D>();
		}

		public Collider2D SensorCollider2D { get; private set; }

		private void Reset()
		{
			base.GetComponent<Collider2D>().isTrigger = true;
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			if (col.CompareTag("Sensor"))
			{
				return;
			}
			if ((this.sensorLayerDetection.value & 1 << col.gameObject.layer) > 0)
			{
				if (!this.targetTag.IsNullOrWhitespace() && !col.CompareTag(this.targetTag))
				{
					return;
				}
				this.objectList.Add(col.gameObject);
				if (this.OnColliderEnter != null)
				{
					this.OnColliderEnter(col);
				}
				Entity componentInParent = col.GetComponentInParent<Entity>();
				if ((componentInParent != null && col.CompareTag("Penitent")) || col.CompareTag("NPC"))
				{
					this.entityList.Add(componentInParent);
					if (this.OnEntityEnter != null)
					{
						this.OnEntityEnter(componentInParent);
					}
					if (col.CompareTag("Penitent"))
					{
						this.PlayerInside = true;
					}
				}
			}
		}

		private void OnTriggerExit2D(Collider2D col)
		{
			if (col.CompareTag("Sensor"))
			{
				return;
			}
			if ((this.sensorLayerDetection.value & 1 << col.gameObject.layer) > 0)
			{
				this.OnTriggerExit2DNotify(col);
				if (!this.targetTag.IsNullOrWhitespace() && !col.CompareTag(this.targetTag))
				{
					return;
				}
				this.objectList.Remove(col.gameObject);
				if (this.OnColliderExit != null)
				{
					this.OnColliderExit(col);
				}
				Entity componentInParent = col.GetComponentInParent<Entity>();
				if ((componentInParent != null && col.CompareTag("Penitent")) || col.CompareTag("NPC"))
				{
					this.entityList.Remove(componentInParent);
					if (this.OnEntityExit != null)
					{
						this.OnEntityExit(componentInParent);
					}
					if (col.CompareTag("Penitent"))
					{
						this.PlayerInside = false;
					}
				}
			}
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if ((this.sensorLayerDetection.value & 1 << other.gameObject.layer) > 0)
			{
				this.OnTriggerStay2DNotify(other);
			}
		}

		public bool PlayerInside { get; private set; }

		public bool ObjectsInside
		{
			get
			{
				return this.objectList.Count > 0;
			}
		}

		public bool EntitiesInside
		{
			get
			{
				return this.entityList.Count > 0;
			}
		}

		public List<GameObject> GetObjectsInside
		{
			get
			{
				return this.objectList;
			}
		}

		public List<Entity> GetEntitiesInside
		{
			get
			{
				return this.entityList;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<Collider2DParam> OnEnter;

		public void OnTriggerEnter2DNotify(Collider2D c)
		{
			if (this.OnEnter != null)
			{
				this.OnEnter(this, new Collider2DParam
				{
					Collider2DArg = c
				});
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<Collider2DParam> OnStay;

		public void OnTriggerStay2DNotify(Collider2D c)
		{
			if (this.OnStay != null)
			{
				this.OnStay(this, new Collider2DParam
				{
					Collider2DArg = c
				});
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<Collider2DParam> OnExit;

		public void OnTriggerExit2DNotify(Collider2D c)
		{
			if (this.OnExit != null)
			{
				this.OnExit(this, new Collider2DParam
				{
					Collider2DArg = c
				});
			}
		}

		public LayerMask sensorLayerDetection;

		public string targetTag;

		private List<GameObject> objectList = new List<GameObject>();

		private List<Entity> entityList = new List<Entity>();

		public delegate void ColliderTriggerEvent(Collider2D objectCollider);

		public delegate void EntityTriggerEvent(Entity entity);
	}
}
