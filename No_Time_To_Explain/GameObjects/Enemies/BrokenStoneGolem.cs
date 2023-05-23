using SFML.Graphics;
using SFML.System;

public class BrokenStoneGolem : Enemy
{
    public BrokenStoneGolem(EnemyType enemyType, string spriteName, RenderWindow window, Vector2i playerIndex, Room currentRoom) : base(enemyType, spriteName, window, playerIndex, currentRoom)
    {

    }

    protected override List<Vector2i> GetAttackTiles()
    {
        List<Vector2i> surroundingTiles = new List<Vector2i>();

        surroundingTiles.Add(new Vector2i(tileIndex.X, tileIndex.Y - 1));
        surroundingTiles.Add(new Vector2i(tileIndex.X, tileIndex.Y + 1));
        surroundingTiles.Add(new Vector2i(tileIndex.X - 1, tileIndex.Y));
        surroundingTiles.Add(new Vector2i(tileIndex.X + 1, tileIndex.Y));
        surroundingTiles.Add(new Vector2i(tileIndex.X + 1, tileIndex.Y + 1));
        surroundingTiles.Add(new Vector2i(tileIndex.X - 1, tileIndex.Y + 1));
        surroundingTiles.Add(new Vector2i(tileIndex.X - 1, tileIndex.Y - 1));
        surroundingTiles.Add(new Vector2i(tileIndex.X + 1, tileIndex.Y - 1));

        return surroundingTiles;
    }

}