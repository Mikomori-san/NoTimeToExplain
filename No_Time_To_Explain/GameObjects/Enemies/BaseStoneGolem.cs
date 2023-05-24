using SFML.Graphics;
using SFML.System;

public class BaseStoneGolem : Enemy
{
    public BaseStoneGolem(EnemyType enemyType, string spriteName, RenderWindow window, Vector2i playerIndex, Room currentRoom) : base(enemyType, spriteName, window, playerIndex, currentRoom)
    {

    }

    protected override List<Vector2i> GetAttackTiles()
    {
        List<Vector2i> surroundingTiles = new List<Vector2i>();
        for(int a = 0; a < 2; a++)
        {
            for(int y = 2; y <= 4; y += 2)
            {
                for(int x = -1; x <= 1; x++)
                {
                    surroundingTiles.Add(new Vector2i(tileIndex.X + x, tileIndex.Y - y));
                    surroundingTiles.Add(new Vector2i(tileIndex.X + x, tileIndex.Y + y));
                }
            }
        }

        for(int a = 0; a < 2; a++)
        {
            for(int x = 2; x <= 4; x += 2)
            {
                for(int y = -1; y <= 1; y++)
                {
                    surroundingTiles.Add(new Vector2i(tileIndex.X - x, tileIndex.Y + y));
                    surroundingTiles.Add(new Vector2i(tileIndex.X + x, tileIndex.Y + y));
                }
            }
        }
        
        return surroundingTiles;
    }
}