using SFML.Graphics;
using SFML.System;

public class LavaGolem : Enemy
{
    public LavaGolem(EnemyType enemyType, string spriteName, RenderWindow window) : base(enemyType, spriteName, window)
    {
        
    }

    protected override void EnemyAttack()
    {
        Console.WriteLine("Two");
    }
}