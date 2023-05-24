using SFML.Graphics;
using SFML.System;

public class LavaGolem : Enemy
{
    public LavaGolem(EnemyType enemyType, string spriteName, RenderWindow window, Vector2i playerIndex, Room currentRoom) : base(enemyType, spriteName, window, playerIndex, currentRoom)
    {
        
    }

    protected override List<Vector2i> GetAttackTiles()
    {
        List<Vector2i> surroundingTiles = new List<Vector2i>();
        for(int i = 0; i < 5; i++)
        {
            surroundingTiles.Add(new Vector2i(tileIndex.X, tileIndex.Y - i));
            surroundingTiles.Add(new Vector2i(tileIndex.X, tileIndex.Y + i));
            surroundingTiles.Add(new Vector2i(tileIndex.X + i, tileIndex.Y));
            surroundingTiles.Add(new Vector2i(tileIndex.X - i, tileIndex.Y));
        }

        return surroundingTiles;
    }

}