using System;
using Steamworks;
using UnityEngine;

namespace Framework.Achievements
{
	public class SteamAchievementsHelper : IAchievementsHelper
	{
		public SteamAchievementsHelper()
		{
			this.steamInit();
		}

		private void steamInit()
		{
			if (SteamManager.Initialized)
			{
				this.m_GameID = new CGameID(SteamUtils.GetAppID());
				this.m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(new Callback<UserStatsReceived_t>.DispatchDelegate(this.OnUserStatsReceived));
				this.m_UserStatsStored = Callback<UserStatsStored_t>.Create(new Callback<UserStatsStored_t>.DispatchDelegate(this.OnUserStatsStored));
				this.m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(new Callback<UserAchievementStored_t>.DispatchDelegate(this.OnAchievementStored));
				this.steamInitialized = true;
				if (!SteamUserStats.RequestCurrentStats())
				{
					Debug.LogError("RequestCurrentStats returned false!");
				}
			}
			else
			{
				Debug.LogError("SteamManager not Initialized!");
			}
		}

		public void SetAchievementProgress(string Id, float value)
		{
			if (this.isOnline)
			{
				if (this.m_bStatsValid)
				{
					try
					{
						if (value >= 100f)
						{
							SteamUserStats.SetAchievement(Id);
							if (!SteamUserStats.StoreStats())
							{
								Debug.LogError("SetAchievementProgress: we couldn't store the stats!");
							}
						}
					}
					catch (Exception ex)
					{
						Debug.LogError(ex.Message);
					}
				}
				else
				{
					Debug.LogError("SetAchievementProgress: stats aren't valid!");
				}
			}
			else
			{
				Debug.LogError("SetAchievementProgress: we are not online!");
			}
		}

		public void GetAchievementProgress(string Id, GetAchievementOperationEvent evt)
		{
			if (this.isOnline)
			{
				if (this.m_bStatsValid)
				{
					try
					{
						bool flag = false;
						SteamUserStats.GetAchievement(Id, ref flag);
						if (flag)
						{
							evt(Id, 100f);
						}
						else
						{
							evt(Id, 0f);
						}
					}
					catch (Exception ex)
					{
						Debug.LogError(ex.Message);
					}
				}
				else
				{
					Debug.LogError("GetAchievementProgress: stats aren't valid!");
				}
			}
			else
			{
				Debug.LogError("GetAchievementProgress: we are not online!");
			}
		}

		private void OnAchievementStored(UserAchievementStored_t pCallback)
		{
			if ((ulong)this.m_GameID == pCallback.m_nGameID)
			{
				if (pCallback.m_nMaxProgress == 0U)
				{
					Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
				}
				else
				{
					Debug.Log(string.Concat(new object[]
					{
						"Achievement '",
						pCallback.m_rgchAchievementName,
						"' progress callback, (",
						pCallback.m_nCurProgress,
						",",
						pCallback.m_nMaxProgress,
						")"
					}));
				}
			}
		}

		private void OnUserStatsReceived(UserStatsReceived_t pCallback)
		{
			if (!SteamManager.Initialized)
			{
				return;
			}
			if ((ulong)this.m_GameID == pCallback.m_nGameID)
			{
				if (pCallback.m_eResult == 1)
				{
					Debug.Log("Received stats and achievements from Steam");
					this.m_bStatsValid = true;
					this.isOnline = true;
				}
				else
				{
					Debug.Log("RequestStats - failed, " + pCallback.m_eResult);
				}
			}
		}

		private void OnUserStatsStored(UserStatsStored_t pCallback)
		{
			if ((ulong)this.m_GameID == pCallback.m_nGameID)
			{
				if (pCallback.m_eResult == 1)
				{
					Debug.Log("StoreStats - success");
				}
				else if (pCallback.m_eResult == 8)
				{
					Debug.Log("StoreStats - some failed to validate");
					UserStatsReceived_t pCallback2 = default(UserStatsReceived_t);
					pCallback2.m_eResult = 1;
					pCallback2.m_nGameID = (ulong)this.m_GameID;
					this.OnUserStatsReceived(pCallback2);
				}
				else
				{
					Debug.Log("StoreStats - failed, " + pCallback.m_eResult);
				}
			}
		}

		private bool m_bStoreStats;

		private bool m_bRequestedStats;

		private bool m_bStatsValid;

		private CGameID m_GameID;

		private Callback<UserStatsReceived_t> m_UserStatsReceived;

		private Callback<UserStatsStored_t> m_UserStatsStored;

		private Callback<UserAchievementStored_t> m_UserAchievementStored;

		private bool steamInitialized;

		private bool isOnline;
	}
}
