using SFML.Graphics;
using SFML.System;

public class EnemyHandler : GameObject
{
    public List<LavaGolem> lavaGolems;
    public override void Draw(RenderWindow window)
    {
        foreach(var lavaGolem in lavaGolems)
        {
            lavaGolem.Draw(window);
        }
    }

    public override void Initialize()
    {
        lavaGolems = new List<LavaGolem>();
        LavaGolem lavaGolem1 = new LavaGolem(new Vector2f(10, -10));
        lavaGolems.Add(lavaGolem1);
        foreach(var lavaGolem in lavaGolems)
        {
            lavaGolem.Initialize();
        }
    }

    public override void Update(float deltaTime)
    {
        foreach(var lavaGolem in lavaGolems)
        {
            lavaGolem.Update(deltaTime);
        }
        
    }
}