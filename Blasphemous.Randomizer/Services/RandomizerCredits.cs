using Blasphemous.Framework.Credits;
using Blasphemous.Framework.Credits.Editors;

namespace Blasphemous.Randomizer.Services;

/// <summary>
/// Displays all contributors to the Randomizer on the credits screen
/// </summary>
public class RandomizerCredits : ModCredits
{
    /// <summary>
    /// Displays all contributors to the Randomizer on the credits screen
    /// </summary>
    protected override void OnDisplay(ICreditsEditor editor)
    {
        Main.Randomizer.Log("Display credits");
    }
}
