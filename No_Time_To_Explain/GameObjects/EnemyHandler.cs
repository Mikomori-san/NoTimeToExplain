using SFML.Graphics;
using SFML.System;

public class EnemyHandler : GameObject
{
    private List<Enemy> enemies;

    public EnemyHandler(List<Enemy> enemies)
    {
        this.enemies = enemies;
    }

    public override void Draw(RenderWindow window)
    {
        foreach(var enemy in enemies)
        {
            enemy.Draw(window);
        }
    }

    public override void Initialize()
    {
        foreach(var enemy in enemies)
        {
            enemy.Initialize();
        }
    }

    public override void Update(float deltaTime)
    {
        foreach(var enemy in enemies)
        {
            enemy.Update(deltaTime);
        }
        
        if(!enemies.Last().hasTurn)
        {
            TurnHandler.Instance.PlayerTurn();
        }
        
    }

    public List<Enemy> GetEnemies()
    {
        return enemies;
    }
}