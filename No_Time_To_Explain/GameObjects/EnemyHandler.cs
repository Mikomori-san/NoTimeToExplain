using SFML.Graphics;
using SFML.System;

public class EnemyHandler : GameObject
{
    private List<Enemy> enemies;
    private List<Sprite> enemySpawn;
    private int tileSize;

    public EnemyHandler(List<Enemy> enemies, List<Sprite> enemySpawn, int tileSize)
    {
        this.enemySpawn = enemySpawn;
        this.enemies = enemies;
        this.tileSize = tileSize;
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
        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].SpriteInitializing(enemySpawn[i].Position + new Vector2f(tileSize / 2, 3*tileSize / 4));
            enemies[i].Initialize();
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
}