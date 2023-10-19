# No Time To Explain

Normally, it's "Good versus Evil," but here, the tables have turned. In No Time To Explain, you play as a small yet powerful time demon who has never misused or even used his powers. However, God and Lucifer fear you so much that they banish you to the deepest levels of Lucifer's castle in Hell and attempt to steal your powers. They only succeed partially, which results in bizarre time distortions, and you don't have much time left to live.

A terrible anger ignites within you, fueled by confusion and hatred. This is the moment you turn "evil." Nothing else is on your mind but revenge, so you embark on a rampage in Hell. It's up to you to devour as many souls as possible and create chaos and fear in Hell before your time runs out. Time's ticking…

## Movement
The playing field consists of different tiles. There are two types of tiles: paths that you can move on and obstacles that block your way.
Move using WASD in one-tile increments in each direction.
- **W = Up**
- **A = Down**
- **S = Left**
- **D = Right**
<br>
What's special here: Every time you move, the enemies also move. Movement is divided into turns. If you manage to jump onto an enemy during your turn, you'll defeat them.

## Enemies
There are four different types of enemies:
- **Stone Golem**
- **Broken Stone Golem**
- **Base Stone Golem**
- **Lava Golem**
<br>
Each enemy has its attack pattern. When you're in an enemy's attack range, the enemy prepares to strike. If you don't manage to move out of the range, the enemy jumps at you, resulting in a game over. If you do succeed in moving out of range, the enemy jumps to the tile where you last stood.

## Levels
A level consists of several rooms. There are a total of 12 different room presets.
The first room is always the Spawn Room, which contains the blue pentagram where you spawn. The last room is always the Teleportation Room, which has a portal leading to the next level.
Between these two rooms, 0 to 4 rooms from the remaining 10 rooms are generated.
To move to the next room, you must jump onto the left-facing Stairs Tile. Once done, the next room is generated. To return to the previous room, jump onto the right-facing Stairs Tile to regenerate it.

## Objective / Score
The goal of the game is to reach the Teleportation Room from the Spawn Room to progress to the next level. During the level, you need to kill enemies to collect their souls. Collected souls are displayed in the top left corner of the screen.
At the beginning of each level, a countdown appears in the top right corner, counting down from 120 seconds. If the timer reaches 0, the player dies. Each newly discovered room adds 30 seconds to the timer. Upon loading the next level, collected souls and remaining time are converted into the actual game score.
The score is calculated as follows: SCORE = SCORE + SOULS x 10 + REMAINING_TIME x 3. 
This means if a player dies in the first level without reaching the portal, they receive no game score. For example, if a player has collected 30 souls in the second level and then dies, these souls aren't included in the game score. Game score is only incremented after completing a level.
