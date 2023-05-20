using SFML.Graphics;
using SFML.System;

public class BaseStoneGolem : Enemy
{
    public BaseStoneGolem(EnemyType enemyType, string spriteName, RenderWindow window, Vector2i playerIndex, Room currentRoom) : base(enemyType, spriteName, window, playerIndex, currentRoom)
    {

    }
}