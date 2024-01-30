﻿using Blasphemous.ModdingAPI;
using Blasphemous.Randomizer.Zones;

namespace Blasphemous.Randomizer;

public class Randomizer : BlasMod
{
    public Randomizer() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    public DataHandler DataHandler { get; } = new();
    public ZoneHandler ZoneHandler { get; } = new();

    public RandomizerSettings CurrentSettings { get; private set; }

    protected override void OnInitialize()
    {
        LogError($"{ModInfo.MOD_NAME} has been initialized");

        DataHandler.Initialize();
    }
}
