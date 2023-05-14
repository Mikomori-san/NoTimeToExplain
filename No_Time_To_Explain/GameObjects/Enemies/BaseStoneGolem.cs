using SFML.Graphics;
using SFML.System;

public class BaseStoneGolem : Enemy
{
    public BaseStoneGolem(Vector2f position, EnemyType enemyType, string spriteName, RenderWindow window) : base(position, enemyType, spriteName, window)
    {

    }
}