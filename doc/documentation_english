# Project Folder Structure

The project is divided into the following folders:

## Assets

The "Assets" folder contains all essential assets for the game. The structure within the folder is as follows:

- **Fonts:** Contains fonts for the game.
- **Music:** Holds music files for the game's soundtrack.
- **Rooms:** Contains TXT files for individual rooms.
- **Sounds:** Includes sound effect files.
- **Textures:** Contains textures for sprites and background images.

## Enums

The "Enums" folder contains all global enums and flags needed in the game.

## GameObjects

The "GameObjects" folder contains various classes and components of the game:

- **Enemies:** Subfolder for individual enemies and their specific implementations.
- **HUD.cs:** Responsible for the game's UI and scoring system.
- **Player.cs:** Contains all necessary components for the player character.
- **Room.cs:** Creates and draws individual rooms.
- **GameObject.cs:** The base class from which all game objects inherit.
- **EnemyHandler.cs:** Manages enemies during a room.

### Enemies

Contains all enemies and the `Game.cs` class, which contains the actual logic that enemies inherit. The enemies themselves only implement their attack patterns.

- BaseStoneGolem.cs
- BrokenStoneGolem.cs
- Enemy.cs
- LavaGolem.cs
- StoneGolem.cs

## Manager

The "Manager" folder contains various manager classes implemented as singletons:

- **AssetManager:** Loads required assets.
- **InputHandler:** Processes keyboard inputs.
- **RoomHandler:** Manages and initializes rooms at the start of a level.
- **TurnHandler:** Manages the turn order between players and enemies.

## Utils

The "Utils" folder contains the `Utils.cs` class responsible for player and enemy coordinate conversions and calculations.

## BreadthFirstSearch.cs

The `BreadthFirstSearch.cs` class implements pathfinding for enemies. It uses breadth-first search to calculate the shortest path from an enemy to the player. It searches neighboring tiles using queues, marking each visited neighbor to avoid revisiting. A maximum number of tiles that the algorithm can search is specified. If the player is not found, the method returns a list of 0 tiles, and the enemies do not move.

## Game.cs

The `Game.cs` class manages the game loop and calls the corresponding `Initialize`, `Update`, and `Draw` methods. It initializes the player, rooms, HUD, EnemyHandler, and KillHandler. Certain elements are reloaded/initialized here when entering a new level.

## Titlescreen.cs

The `Titlescreen.cs` class represents the game's title screen. It includes a button that calls `Game.cs`, loading the actual game.
