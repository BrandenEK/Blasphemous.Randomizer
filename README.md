<div align="center">
  <!-- Logo by JimmyDiamonds -->
  <img src="resources/data/Randomizer/logo.png">
</div>

---

<div align="center">
  <img src="https://img.shields.io/github/v/release/BrandenEK/Blasphemous-Randomizer?style=for-the-badge">
  <img src="https://img.shields.io/github/last-commit/BrandenEK/Blasphemous-Randomizer?color=important&style=for-the-badge">
  <img src="https://img.shields.io/github/downloads/BrandenEK/Blasphemous-Randomizer/total?color=success&style=for-the-badge">
</div>

---

## Contributors

A very special thank you to everyone who has helped with the randomizer

***- Programming and design -*** <br>
[@BrandenEK](https://github.com/BrandenEK)

***- Translations -*** <br>
[@ConanCimmerio](https://github.com/ConanCimmerio), [@EltonZhang777](https://github.com/EltonZhang777), [@RocherBrockas](https://github.com/RocherBrockas)

***- Images and UI -*** <br>
[@JimmyDiamonds](https://github.com/JimmyDiamonds), [@rcvrdt](https://github.com/rcvrdt), [@ConanCimmerio](https://github.com/ConanCimmerio), [@LuceScarlet](https://github.com/LuceScarlet)

***- Logic testing and improvements -*** <br>
[@Exempt-Medic](https://github.com/Exempt-Medic), [@LumineonRL](https://github.com/LumineonRL)

---

## Useful Info

- Only works on the most current game version (4.0.67)
- Press 'Numpad 6' to display the current seed (If you are stuck in dialogue use this to break out of it)
- Save & Quit can be used to instantly return to your last Prie Dieu, which can fix softlocks and makes the 'Return to Port' prayer useless
- Consult the spoiler generated in the game directory if you are stuck or believe the seed is unbeatable
- Do not load a vanilla game in the randomizer or vice versa

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

## Randomizer Settings

  <table>
    <tr>
      <td align="center"> Name </td>
      <td align="center" colspan="3"> Explanation </td>
      <td align="center"> Default </td>
    </tr>
    <tr>
      <td align="center"> Logic Difficulty </td>
      <td align="center" colspan="3"> Determines what skips and techniques may be required in the logic <br>
        (See below for a more detailed breakdown) </td>
      <td align="center"> Normal </td>
    </tr>
    <tr>
      <td align="center"> Starting Location </td>
      <td align="center" colspan="3"> Which area of the game you will start from </td>
      <td align="center"> Brotherhood </td>
    </tr>
    <tr>
      <td align="center"> Teleportation </td>
      <td align="center" colspan="3"> Should prie dieus be upgraded to maximum level from the beginning </td>
      <td align="center"> True </td>
    </tr>
    <tr>
      <td align="center"> Allow Hints </td>
      <td align="center" colspan="3"> Should the corpses give vague information about the location of progression items <br>
        (See below for specific corpse hints) </td>
      <td align="center"> True </td>
    </tr>
    <tr>
      <td align="center"> Allow Penitence </td>
      <td align="center" colspan="3"> Should a penitence be selectable from the Brotherhood statue </td>
      <td align="center"> False </td>
    </tr>
    <tr>
      <td align="center"> Door Shuffle </td>
      <td align="center"> Disabled: <br>
        Room transitions will always lead to their original destinations </td>
      <td align="center"> Simple: <br>
        Only room transitions that lead to a different region will be shuffled with each other </td>
      <td align="center"> Full: <br>
        All room transtitions will be shuffled with each other </td>
      <td align="center"> Disabled </td>
    </tr>
    <tr>
      <td align="center"> Enemy Shuffle </td>
      <td align="center"> Disabled: <br>
        Enemies will remain in their original places </td>
      <td align="center"> Simple: <br>
        Enemies will be placed randomly, with each enemy appearing the same number of times as in the original game </td>
      <td align="center"> Full: <br>
        Enemies will be placed randomly, with each enemy appearing any number of times </td>
      <td align="center"> Disabled </td>
    </tr>
    <tr>
      <td align="center"> Maintain Class </td>
      <td align="center" colspan="3"> Enemies are constrained to their original group, so flying enemies only replace flying enemies etc. <br>
        (Only takes effect with enemy shuffle on) </td>
      <td align="center"> True </td>
    </tr>
    <tr>
      <td align="center"> Area scaling </td>
      <td align="center" colspan="3"> Enemy health and damage is scaled up/down based on their location <br>
        (Only takes effect with enemy shuffle on) </td>
      <td align="center"> True </td>
    </tr>
    <tr>
      <td align="center"> Shuffle Reliquaries </td>
      <td align="center" colspan="3"> Shuffles the three reliquaries into the item pool </td>
      <td align="center"> True </td>
    </tr>
    <tr>
      <td align="center"> Shuffle Dash </td>
      <td align="center" colspan="3"> Shuffles the dash ability into the item pool <br>
        (Only available with specific starting locations or full door shuffle) </td>
      <td align="center"> False </td>
    </tr>
    <tr>
      <td align="center"> Shuffle Wall Climb </td>
      <td align="center" colspan="3"> Shuffles the wall climb ability into the item pool <br>
        (Only available with specific starting locations or full door shuffle) </td>
      <td align="center"> False </td>
    </tr>
    <tr>
      <td align="center"> Shuffle Spike Boots </td>
      <td align="center" colspan="3"> Shuffles the Boots of Pleading into the item pool <br>
        (Requires the mod to be installed) </td>
      <td align="center"> False </td>
    </tr>
    <tr>
      <td align="center"> Shuffle Double Jump </td>
      <td align="center" colspan="3"> Shuffles the Purified Hand of the Nun into the item pool <br>
        (Requires the mod to be installed) </td>
      <td align="center"> False </td>
    </tr>
    <tr>
      <td align="center"> Shuffle Sword Skills </td>
      <td align="center" colspan="3"> Shuffles the sword skills into the item pool </td>
      <td align="center"> True </td>
    </tr>
    <tr>
      <td align="center"> Shuffle Thorns </td>
      <td align="center" colspan="3"> Shuffles the 8 thorns into the item pool </td>
      <td align="center"> True </td>
    </tr>
    <tr>
      <td align="center"> Junk Inconvenient Locations </td>
      <td align="center" colspan="3"> Forces a junk item at inconvenient locations such as Miriam </td>
      <td align="center"> True </td>
    </tr>
    <tr>
      <td align="center"> Start with Wheel </td>
      <td align="center" colspan="3"> The starting gift will be the Young Mason's Wheel </td>
      <td align="center"> False </td>
    </tr>
  </table>

---

## Logic difficulty

<table>
    <tr>
      <td align="center"> Easy </td>
      <td>
        <ul>
          <li>No skips or glitches will be necessary</li>
          <li>Only the expected method of reaching items will be considered in logic</li>
          <li>Bosses require an extra 10% strength to be in logic</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td align="center"> Normal </td>
      <td>
        <ul>
          <li>Using dawn heart skips & mid-air stalls may be necessary</li>
          <li>Tiento may be required to access items in poison clouds without Silvered Lung</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td align="center"> Hard </td>
      <td>
        <ul>
          <li>Mourning and Havoc skip may be required</li>
          <li>Slash Upwarp skips, Dive Laser skips, and using Tirana to break switches are considered in logic</li>
          <li>Some items in poison clouds are considered in logic without Tiento or Silvered Lung</li>
          <li>Bosses require 10% less strength to be in logic</li>
        </ul>
      </td>
    </tr>
</table>

---

## Seeds

In order to choose a specific seed that you want to play on, click on the seed text box and type the desired number on the settings menu shown before starting a new game.  Seeds can be in the range of 1 to 99,999,999.
<br><br>
In order to make it easier for multiple people to play with the same item generation (Such as for races or multiplayer), a sequence of 7 images is shown in the top right corner of the settings menu.  This is a unique identifier of the seed that also takes into account the other configuration settings and ensures that items will be placed exactly the same for all players.

---

## Autotracking

Beginning in Randomizer 2.0.9, autotracking for the poptracker pack has been disabled

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
