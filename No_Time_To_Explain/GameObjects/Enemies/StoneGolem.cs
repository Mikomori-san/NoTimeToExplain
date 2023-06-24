/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

using SFML.Graphics;
using SFML.System;

public class StoneGolem : Enemy
{
    public StoneGolem(EnemyType enemyType, TextureName spriteName, RenderWindow window, Vector2i playerIndex, Room currentRoom) : base(enemyType, spriteName, window, playerIndex, currentRoom)
    {

    }

    protected override List<Vector2i> GetAttackTiles()
    {
        List<Vector2i> surroundingTiles = new List<Vector2i>();

        surroundingTiles.Add(new Vector2i(TileIndex.X, TileIndex.Y - 1));
        surroundingTiles.Add(new Vector2i(TileIndex.X, TileIndex.Y + 1));
        surroundingTiles.Add(new Vector2i(TileIndex.X - 1, TileIndex.Y));
        surroundingTiles.Add(new Vector2i(TileIndex.X + 1, TileIndex.Y));
        surroundingTiles.Add(new Vector2i(TileIndex.X + 1, TileIndex.Y + 1));
        surroundingTiles.Add(new Vector2i(TileIndex.X - 1, TileIndex.Y + 1));
        surroundingTiles.Add(new Vector2i(TileIndex.X - 1, TileIndex.Y - 1));
        surroundingTiles.Add(new Vector2i(TileIndex.X + 1, TileIndex.Y - 1));

        for(int y = 3; y <= 7; y += 2)
        {
            for(int x = -2; x <= 2; x++)
            {
                surroundingTiles.Add(new Vector2i(TileIndex.X + x, TileIndex.Y - y));
                surroundingTiles.Add(new Vector2i(TileIndex.X + x, TileIndex.Y + y));
            }
        }

        for(int x = 3; x <= 7; x += 2)
        {
            for(int y = -2; y <= 2; y++)
            {
                surroundingTiles.Add(new Vector2i(TileIndex.X + x, TileIndex.Y + y));
                surroundingTiles.Add(new Vector2i(TileIndex.X - x, TileIndex.Y + y));
            }
        }

        return surroundingTiles;
    }

}