using SFML.Graphics;
using SFML.System;

public class LavaGolem : Enemy
{
    public LavaGolem(Vector2f position, EnemyType enemyType, string spriteName, RenderWindow window) : base(position, enemyType, spriteName, window)
    {
        
    }

    protected override void EnemyAttack()
    {
        Console.WriteLine("Two");
    }
}