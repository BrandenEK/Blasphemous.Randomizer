<div align="center">
  <!-- Logo by JimmyDiamonds -->
  <img src="../resources/data/Randomizer/logo.png">
</div>

---

<div align="center">
  <img src="https://img.shields.io/github/v/release/BrandenEK/Blasphemous.Randomizer?style=for-the-badge&color=2857AB">
  <img src="https://img.shields.io/github/last-commit/BrandenEK/Blasphemous.Randomizer?style=for-the-badge&color=AB2857">
  <img src="https://img.shields.io/github/downloads/BrandenEK/Blasphemous.Randomizer/total?style=for-the-badge&color=57AB28">
</div>

---

## Contributors

***- Programming and design -*** <br>
[@BrandenEK](https://github.com/BrandenEK), [TRPG0](https://github.com/TRPG0)

***- Artwork -*** <br>
[@JimmyDiamonds](https://github.com/JimmyDiamonds), [@rcvrdt](https://github.com/rcvrdt), [@ConanCimmerio](https://github.com/ConanCimmerio), [@LuceScarlet](https://github.com/LuceScarlet)

***- Translations -*** <br>
[@ConanCimmerio](https://github.com/ConanCimmerio), [@EltonZhang777](https://github.com/EltonZhang777), [@RocherBrockas](https://github.com/RocherBrockas)

***- Logic testing and improvements -*** <br>
[@Exempt-Medic](https://github.com/Exempt-Medic), [@LumineonRL](https://github.com/LumineonRL)

---

## Useful Info

- Only works on the most current game version (4.0.67)
- Press F8 to display the current seed (If you are stuck in dialogue use this to break out of it)
- Save & Quit can be used to instantly return to your last Prie Dieu, which can fix softlocks and makes the 'Return to Port' prayer useless
- Consult the spoiler generated in the game directory if you are stuck or believe the seed is unbeatable
- Do not load a vanilla game in the randomizer or vice versa

---

## Randomizer Settings

A full breakdown of all the available settings can be found [here](SETTINGS.md)

---

## Gameplay Differences

Tirso's questline
- Tirso's helpers will never die, so herbs can be given to him at any time

Gemino's questline
- Gemino will never freeze, so the thimble can be given to him at any time

Viridiana's questline
- Viridiana will never die, so she can be used for all 5 boss fights and will always give her item at the rooftops

Cleofas' questline
- There is no longer an option to choose to slay Socorro
- He will not jump off of the rooftops, even after talking to him without the cord

Crisanta's questline
- The scapular will not skip the Esdras fight; instead, it is required to open the door to the chapel
- Perpetva's item can be retrieved even after defeating Esdras
- Crisanta will always hold the 'Holy Wound of Abnegation'
- Crisanta does not have to beaten with the True Heart equipped in order to obtain the holy wound, it just has to be in your inventory when talking to her

Door rando
- A command has been added to respawn the player from the chosen starting location
- All doors from the rooftops elevator and above will remain the same so that the main goal is still to find all 3 masks
- Some other doors will always remain the same as they are in the original game
- The spawn points of a few doors have been modified
- Platforms have been added to the main bell room in Jondo
- The sword heart doesn't have to be unequipped for the Library bone puzzle
- The chalice will never unfill
- The Cistern shroud puzzle will be automatically completed if the shroud is equipped
- The Albero warp room can not be teleported to until activated, and the gate has been removed

---

## Seeds

In order to choose a specific seed that you want to play on, click on the seed text box and type the desired number on the settings menu shown before starting a new game.  Seeds can be in the range of 1 to 99,999,999.
<br><br>
In order to make it easier for multiple people to play with the same item generation (Such as for races or multiplayer), a sequence of 7 images is shown in the top right corner of the settings menu.  This is a unique identifier of the seed that also takes into account the other configuration settings and ensures that items will be placed exactly the same for all players.

---

## Autotracking

Beginning with Randomizer 2.0.9, autotracking for the poptracker pack has been disabled

---

## Available commands
- Press the 'backslash' key to open the debug console
- Type the desired command followed by the parameters all separated by a single space

| Command | Parameters | Description |
| ------- | ----------- | ------- |
| `randomizer help` | none | List all available commands |
| `randomizer respawn` | none | Respawns the player from the chosen starting location |

---

## Corpse hints

Each of the 34 corpses in the game can give a hint about the location of a random valuable item.  These include beads, prayers, sword hearts, relics, quest items, stat upgrades, and skills.  However, there are a few specific corpses that will always hint at the same location, so that some of the more inconvenient locations can possibly be avoided.
<br>
These are:
- Corpse in ossuary --> Isidora reward
- Corpse outside Sierpes --> Sierpes reward
- Corpse in Ferrous Tree --> Miriam reward
- Corpse in Echoes of Salt --> Chalice quest sword shrine
- Corpse outside wax puzzle room --> Wax puzzle chest
- Corpse outside Jocinero's room --> Jocinero's final reward
- Corpse at entrance of WhOTW --> White Lady tomb reward
- Corpse outside Albero church --> 50,000 tear donation reward

---

## Installation
This mod is available for download through the [Blasphemous Mod Installer](https://github.com/BrandenEK/Blasphemous.Modding.Installer)
- Required dependencies: Modding API, Credits Framework, Level Framework, Menu Framework, UI Framework, Cheat Console
