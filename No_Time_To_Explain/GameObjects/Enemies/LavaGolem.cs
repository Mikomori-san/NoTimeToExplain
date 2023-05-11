using SFML.Graphics;
using SFML.System;

public class LavaGolem : Enemy
{
    public LavaGolem(Vector2f position, EnemyType enemyType, string spriteName) : base(position, enemyType, spriteName)
    {
        
    }

    protected override void EnemyAttack()
    {
        Console.WriteLine("Two");
    }
}