using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class SkillCommand : ConsoleCommand
	{
		public override bool HasLowerParameters()
		{
			return false;
		}

		public override void Execute(string command, string[] parameters)
		{
			List<string> paramList;
			string subcommand = base.GetSubcommand(parameters, out paramList);
			if (command != null)
			{
				if (command == "skill")
				{
					this.ParseSkill(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"skill"
			};
		}

		private void ParseSkill(string command, List<string> paramList)
		{
			string text = "skill " + command;
			switch (command)
			{
			case "help":
				if (base.ValidateParams(text, 0, paramList))
				{
					base.Console.Write("Available SKILL commands:");
					base.Console.Write("skill list: List all skills and the current status");
					base.Console.Write("skill lock SKILLNAME: Locks the skill SKILLNAME");
					base.Console.Write("skill lockall: Locks all the skills");
					base.Console.Write("skill unlock SKILLNAME: Unlocks the skill SKILLNAME");
					base.Console.Write("skill unlockall: Unlocks all skills");
					base.Console.Write("skill showui: Show the unlock UI");
				}
				return;
			case "list":
				if (base.ValidateParams(text, 0, paramList))
				{
					base.Console.Write("*** All skills:");
					for (int i = 0; i <= Core.SkillManager.GetMaxSkillsTier(); i++)
					{
						List<UnlockableSkill> skillsByTier = Core.SkillManager.GetSkillsByTier(i);
						if (skillsByTier.Count > 0)
						{
							base.Console.Write("Tier " + i.ToString());
							foreach (UnlockableSkill unlockableSkill in skillsByTier)
							{
								string text2 = string.Concat(new string[]
								{
									"  ",
									unlockableSkill.id,
									"  -- ",
									unlockableSkill.caption,
									" ("
								});
								text2 += ((!unlockableSkill.unlocked) ? "Locked" : "Unlocked");
								text2 += ")";
								base.Console.Write(text2);
							}
						}
					}
				}
				return;
			case "unlock":
				if (base.ValidateParams(text, 1, paramList))
				{
					base.WriteCommandResult(text, Core.SkillManager.UnlockSkill(paramList[0].ToUpper(), true));
				}
				return;
			case "unlockall":
				if (base.ValidateParams(text, 0, paramList))
				{
					for (int j = 0; j <= Core.SkillManager.GetMaxSkillsTier(); j++)
					{
						List<UnlockableSkill> skillsByTier2 = Core.SkillManager.GetSkillsByTier(j);
						if (skillsByTier2.Count > 0)
						{
							foreach (UnlockableSkill unlockableSkill2 in skillsByTier2)
							{
								if (!unlockableSkill2.unlocked)
								{
									Core.SkillManager.UnlockSkill(unlockableSkill2.id.ToUpper(), true);
								}
							}
						}
					}
					base.Console.Write("All skills unlocked");
				}
				return;
			case "lock":
				if (base.ValidateParams(text, 1, paramList))
				{
					base.WriteCommandResult(text, Core.SkillManager.LockSkill(paramList[0].ToUpper()));
				}
				return;
			case "lockall":
				if (base.ValidateParams(text, 0, paramList))
				{
					for (int k = 0; k <= Core.SkillManager.GetMaxSkillsTier(); k++)
					{
						List<UnlockableSkill> skillsByTier3 = Core.SkillManager.GetSkillsByTier(k);
						if (skillsByTier3.Count > 0)
						{
							foreach (UnlockableSkill unlockableSkill3 in skillsByTier3)
							{
								if (!unlockableSkill3.unlocked)
								{
									Core.SkillManager.LockSkill(unlockableSkill3.id.ToUpper());
								}
							}
						}
					}
				}
				return;
			case "showui":
				if (base.ValidateParams(text, 0, paramList))
				{
					UIController.instance.ShowUnlockSKill();
				}
				return;
			}
			base.Console.Write("Command " + text + " is unknow, use skill help");
		}
	}
}
