using Blasphemous.Framework.Credits;
using Blasphemous.Framework.Credits.Editors;
using System.Collections.Generic;

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
        editor.AddImage(Main.Randomizer.data.Logo);
        editor.AddBlank();

        AddSingleSection(editor, "Programming and design", PROGRAMMING);
        AddDoubleSection(editor, "Translations", TRANSLATIONS);
        AddSingleSection(editor, "Images and UI", IMAGES);
        AddSingleSection(editor, "Logic testing and improvements", LOGIC);

        editor.AddDivider();
        editor.AddBlank();
    }

    private void AddSingleSection(ICreditsEditor editor, string title, string[] section)
    {
        editor.AddHeader(title);
        foreach (string person in section)
            editor.AddSingle(person);
        editor.AddBlank();
    }

    private void AddDoubleSection(ICreditsEditor editor, string title, Dictionary<string, string> section)
    {
        editor.AddHeader(title);
        foreach (var person in section)
            editor.AddDouble(person.Key, person.Value);
        editor.AddBlank();
    }

    private readonly string[] PROGRAMMING =
    {
        "Damocles"
    };

    private readonly Dictionary<string, string> TRANSLATIONS = new()
    {
        { "Ro", "Spanish" },
        { "Newbie Elton", "Chinese" },
        { "Rocher", "French" }
    };

    private readonly string[] IMAGES =
    {
        "Jimmy Diamonds",
        "Raider",
        "Ro",
        "LuceScarlet"
    };

    private readonly string[] LOGIC =
    {
        "Exempt Medic",
        "Lumineon"
    };
}
