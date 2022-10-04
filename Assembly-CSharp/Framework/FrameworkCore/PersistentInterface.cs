using System;
using Framework.Managers;

namespace Framework.FrameworkCore
{
	public interface PersistentInterface
	{
		string GetPersistenID();

		PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave);

		void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath);

		void ResetPersistence();

		int GetOrder();
	}
}
