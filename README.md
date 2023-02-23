# Blasphemous Randomizer

## Table of Contents

- [Links](https://github.com/BrandenEK/Blasphemous-Randomizer#links)
- [Installation](https://github.com/BrandenEK/Blasphemous-Randomizer#installation)
- [Randomizer info](https://github.com/BrandenEK/Blasphemous-Randomizer#randomizer-info)
  - [Important notes](https://github.com/BrandenEK/Blasphemous-Randomizer#important-notes)
  - [Lesser know logic](https://github.com/BrandenEK/Blasphemous-Randomizer#lesser-known-logic)
  - [Questline changes](https://github.com/BrandenEK/Blasphemous-Randomizer#questline-changes)
  - [Corpse hints](https://github.com/BrandenEK/Blasphemous-Randomizer#corpse-hints)
- [Changelog](https://github.com/BrandenEK/Blasphemous-Randomizer#changelog)

---

## Links

- Progress board: [Trello](https://trello.com/b/FJ42w6X1/blasphemous-randomizer)
- Discord: [Blasphemous General Server](https://discord.gg/Blasphemous)
- Map Tracker: [Tracker by Sassyvania](https://github.com/sassyvania/Blasphemous-Randomizer-Maptracker)
- Multiworld: [Archipelago by TRPG0](https://github.com/BrandenEK/Blasphemous-Multiworld)

---

## Installation

1. Download the latest release of the Modding API from https://github.com/BrandenEK/Blasphemous-Modding-API/releases
2. Follow the instructions there on how to install the api
3. Download the latest release of the Randomizer from the [Releases](https://github.com/BrandenEK/Blasphemous-Randomizer/releases) page
4. Extract the contents of the BlasphemousRandomizer.zip file into the "Modding" folder

---

## Randomizer info

### Important notes

- Only works on the most current game version (4.0.67)
- Press 'Numpad 6' to display the current seed
- Consult the spoiler generated in the game directory if you are stuck or believe the seed is unbeatable
- Do not load a vanilla game in the randomizer or vice versa
- Do not load an outdated randomized game in a newer version of the randomizer - seed generation is different

### Lesser known logic

- Many gaps can be jumped across by using either dawn heart or young mason's wheel
- Many cherubs can be collected with the more uncommon prayers
- Lvdovico's reward for meeting Cleofas doesn't require the marks - Just talking to him
- Laudes only requires the 4 golden verses to access - She can be fought before any of the other Amanecidas
- Holes in the ground can be opened with either dive or charge attack or using any prayer on top of them
- There are three ways to reach the second half of the map
  1. Collect 3 holy wounds and defeat Esdras
  2. Use dawn heart & blood attack to jump across the gap in Mourning and Havoc
  3. Use Tirana to release the ladder in Mourning and Havoc

### Questline changes

- Tirso
  - Tirso's helpers will never die, so herbs can be given to him at any time
- Gemino
  - Gemino will never freeze, so the thimble can be given to him at any time
- Viridiana
  - Viridiana will never die, so she can be used for all 5 boss fights and will always give her item at the rooftops
- Redento
  - No changes
- Cleofas
  - There is no longer an option to choose to slay Socorro
  - He will not jump off of the rooftops, even after talking to him without the cord
- Crisanta
  - The scapular will not skip the Esdras fight; instead, it is required to open the door to the chapel
  - Perpetva's item can be retrieved even after defeating Esdras
  - Crisanta will always hold the 'Holy Wound of Abnegation'
  - Crisanta does not have to beaten with the True Heart equipped in order to obtain the holy wound, it just has to be in your inventory when talking to her

### Corpse hints

Each of the 34 corpses in the game can give a hint about the location of a random valuable item.  These include beads, prayers, sword hearts, relics, quest items, stat upgrades, and skills.  However, there are a few specific corpses that will always hint at the same location, so that some of the more inconvenient locations can possibly be avoided.
<br>
These are:
- Corpse in ossuary --> Isidora reward
- Corpse outside Sierpes --> Sierpes reward
- Corpse in Ferrous Tree --> Miriam reward
- Corpse in Echoes of Salt --> Chalice quest sword shrine
- Corpse underneath WaHP lift puzzle --> WaHP lift puzzle item
- Corpse outside wax puzzle room --> Wax puzzle chest
- Corpse outside Jocinero's room --> Jocinero's final reward
- Corpse at entrance of WhOTW --> White Lady tomb reward
- Corpse outside Albero church --> 50,000 tear donation reward

---

## Changelog

### v1.4.0 (Modding API)
- Fixed melted coins bug
- Reworked rando to be compatible with the Modding API

### v1.3.0 (Sword Skills)
- Shuffle the 15 sword skills
- Golden verses are random
- Changed enemy rando types to (Disabled, Shuffled, Random)

### v1.2.0 (More Randomization)
- Multiworld support
- Show save file validity on select screen
- Cleofas & Crisanta quests are random
- Option to start with wheel
- Option to shuffle reliquaries
- In game settings menu

### v1.1.0 (Enemizer Improvements)
- Full enemy randomization
- Add flying class of enemies
- Remove weak class of enemies
- Fix glitched enemy spawn positions
- Misc. enemy improvements
	- Fix flagellant hitbox
	- Fix librarian animation
	- Fix wall enemy attack
	- Fix Tizona softlock
	- Fix inconsistent crashes
	- Randomize Flying Patrollers & Melted Ladies
	
### v1.0.0 (New Format)
- Entire new mod format
- Shroud is not required for corpse hints

### v0.5.0 (Corpse Hints)
- Improved loading/randomization times
- Can unequip true heart
- Viridiana never dies
- Corpses give useful hints

### v0.4.0 (Progressive Items)
- Added tear requirements to logic
- Fixed duplicate items spawning
- Gemino never dies
- Modified hitboxes of large enemies
- Shops display the randomized items
- Progressive items
- More items are randomized (Shops, Candles, Thorns)

### v0.3.0 (Enemy Randomization)
- Basic enemy randomization
- Scale enemy stats based on area
- Display error when loading invalid save file
- More items are randomized (Holy wounds, Masks, Herbs)
- Tirso never dies

### v0.2.0 (Tear Rewards)
- Added tear rewards to the item pool
- Added custom images for life/fervour/sword upgrades & cherubs
- Generates spoiler log
- Changed behaviour of the Ossuary
- Enabled console commands

### v0.1.0 (Base Release)
- Initial randomization algorithm
