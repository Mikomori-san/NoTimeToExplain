

# Projektordnerstruktur

Das Projekt ist in folgende Ordner unterteilt:

## Assets

Der Ordner "Assets" enthält alle wichtigen Assets für das Spiel. Die Struktur innerhalb des Ordners ist wie folgt:

-   Fonts: Enthält Schriftarten für das Spiel.
-   Music: Beinhaltet Musikdateien für den Soundtrack des Spiels.
-   Rooms: Enthält TXT-Dateien für die einzelnen Räume.
-   Sounds: Beinhaltet Soundeffektdateien.
-   Textures: Enthält Texturen für Sprites und Hintergrundbilder.

## Enums

Der Ordner "Enums" enthält alle globalen Enums und Flags, die im Spiel benötigt werden.

## GameObjects

Der Ordner "GameObjects" enthält verschiedene Klassen und Komponenten des Spiels:

-   Enemies: Unterordner für die einzelnen Gegner und deren spezifische Implementierungen.
-   HUD.cs: Verantwortlich für das UI und das Punktesystem des Spiels.
-   Player.cs: Beinhaltet alle erforderlichen Komponenten für den Spieler-Charakter.
-   Room.cs: Erstellt und zeichnet die einzelnen Räume.
-   GameObject.cs: Die Basisklasse, von der alle Spielobjekte erben.
-   EnemyHandler.cs: Verwaltet die Gegner während eines Raums.

### Enemies

Beinhaltet alle Gegner und die Game.cs Klasse, die die eigentliche Logik beinhaltet und von der die Gegner erben. Die Gegner an sich haben nur deren Attack Patterns implementiert.
 
- BaseStoneGolem.cs
- BrokenStoneGolem.cs
- Enemy.cs
- LavaGolem.cs
- StoneGolem.cs

## Manager

Der Ordner "Manager" enthält verschiedene Manager-Klassen, die als Singleton implementiert sind:

-   AssetManager: Laden der benötigten Assets.
-   InputHandler: Verarbeitet die Tastatureingaben.
-   RoomHandler: Verwaltet und initialisiert die Räume am Anfang eines Levels.
-   TurnHandler: Verwaltet die Reihenfolge der Züge zwischen Spieler und Gegnern.

## Utils

Der Ordner "Utils" enthält die Klasse "Utils.cs", die für Koordinatenumwandlungen und Berechnungen des Spielers und der Gegner zuständig ist.

## BreadthFirstSearch.cs

Die Klasse "BreadthFirstSearch.cs" implementiert das Pathfinding für die Gegner. Sie verwendet eine Breitensuche, um den kürzesten Pfad eines Gegners zum Spieler zu berechnen. Mithilfe von Queues werden jeweils die Nachbartiles nach dem Spieler durchsucht. Dabei wird jeder besuchte Nachbar markiert, um ihn nicht nochmal zu besuchen. Es wird eine Maximalanzahl von Tiles angegeben, die der Algorithmus durchsuchen darf. Wenn der Spieler nicht gefunden wird, gibt die Methode eine Liste mit 0 Tiles zurück, und die Gegner bewegen sich nicht.

## Game.cs

Die Klasse "Game.cs" kümmert sich um den Game Loop und ruft die entsprechenden Initialize-, Update- und Draw-Methoden auf. Sie initialisiert den Spieler, die Räume, das HUD, den EnemyHandler und den KillHandler. Beim Betreten eines neuen Levels werden bestimmte Elemente hier neu geladen/initialisiert.

## Titlescreen.cs

Die Klasse "Titlescreen.cs" repräsentiert den Titelbildschirm des Spiels. Diese enthält einen Button, der das "Game.cs" aufruft und das eigentliche Spiel lädt.