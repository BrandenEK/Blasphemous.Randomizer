using System;
using UnityEngine;

namespace Framework.Map
{
	[Serializable]
	public class ZoneKey
	{
		public ZoneKey()
		{
			this._district = string.Empty;
			this._zone = string.Empty;
		}

		public ZoneKey(string district, string zone, string scene)
		{
			this._district = district;
			this._zone = zone;
			this._scene = scene;
		}

		public ZoneKey(ZoneKey other)
		{
			this._district = other.District;
			this._zone = other.Zone;
			this._scene = other.Scene;
		}

		public string District
		{
			get
			{
				return this._district;
			}
			set
			{
				this._district = value;
			}
		}

		public string Zone
		{
			get
			{
				return this._zone;
			}
			set
			{
				this._zone = value;
			}
		}

		public string Scene
		{
			get
			{
				return this._scene;
			}
			set
			{
				this._scene = value;
			}
		}

		public int GetSceneInt()
		{
			int result = 0;
			if (this.Scene != null && this.Scene != string.Empty)
			{
				int.TryParse(this.Scene.Substring(1), out result);
			}
			return result;
		}

		public string GetKey()
		{
			return string.Concat(new string[]
			{
				this._district,
				"_",
				this._zone,
				"_",
				this._scene
			});
		}

		public string GetLevelName()
		{
			return this._district + this._zone + this._scene;
		}

		public string GetLocalizationKey()
		{
			return this._district + "_" + this._zone;
		}

		public override int GetHashCode()
		{
			string key = this.GetKey();
			return key.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ZoneKey);
		}

		public bool Equals(ZoneKey obj)
		{
			return obj != null && obj.Zone == this.Zone && obj.District == this.District && obj.Scene == this.Scene;
		}

		public bool IsEmpty()
		{
			return this.District == string.Empty || this.Zone == string.Empty || this.Scene == string.Empty;
		}

		[SerializeField]
		private string _district;

		[SerializeField]
		private string _zone;

		[SerializeField]
		private string _scene;
	}
}
