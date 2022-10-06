using System;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class FlagObject
{
	public FlagObject()
	{
		ValueDropdownList<PersistentManager.PercentageType> valueDropdownList = new ValueDropdownList<PersistentManager.PercentageType>();
		valueDropdownList.Add("BossDefeated_1", PersistentManager.PercentageType.BossDefeated_1);
		valueDropdownList.Add("BossDefeated_2", PersistentManager.PercentageType.BossDefeated_2);
		valueDropdownList.Add("Upgraded", PersistentManager.PercentageType.Upgraded);
		valueDropdownList.Add("EndingA", PersistentManager.PercentageType.EndingA);
		valueDropdownList.Add("BossDefeated_NgPlus", PersistentManager.PercentageType.BossDefeated_NgPlus);
		this.FLagsPercentages = valueDropdownList;
		base..ctor();
	}

	public FlagObject(FlagObject other)
	{
		ValueDropdownList<PersistentManager.PercentageType> valueDropdownList = new ValueDropdownList<PersistentManager.PercentageType>();
		valueDropdownList.Add("BossDefeated_1", PersistentManager.PercentageType.BossDefeated_1);
		valueDropdownList.Add("BossDefeated_2", PersistentManager.PercentageType.BossDefeated_2);
		valueDropdownList.Add("Upgraded", PersistentManager.PercentageType.Upgraded);
		valueDropdownList.Add("EndingA", PersistentManager.PercentageType.EndingA);
		valueDropdownList.Add("BossDefeated_NgPlus", PersistentManager.PercentageType.BossDefeated_NgPlus);
		this.FLagsPercentages = valueDropdownList;
		base..ctor();
		this.id = other.id;
		this.shortDescription = other.shortDescription;
		this.description = other.description;
		this.value = other.value;
		this.preserveInNewGamePlus = other.preserveInNewGamePlus;
		this.addToPercentage = other.addToPercentage;
		this.percentageType = other.percentageType;
	}

	public void OnIdChanged(string value)
	{
		this.id = value.Replace(' ', '_').ToUpper();
	}

	[OnValueChanged("OnIdChanged", false)]
	public string id = string.Empty;

	public string shortDescription = string.Empty;

	[TextArea(3, 10)]
	public string description = string.Empty;

	public bool value;

	public bool preserveInNewGamePlus;

	[HideInInspector]
	public string sourceList = string.Empty;

	public bool addToPercentage;

	private ValueDropdownList<PersistentManager.PercentageType> FLagsPercentages;

	[ShowIf("addToPercentage", true)]
	[ValueDropdown("FLagsPercentages")]
	public PersistentManager.PercentageType percentageType;
}
