/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

using SFML.Graphics;
using SFML.System;

public class BrokenStoneGolem : Enemy
{
    public BrokenStoneGolem(EnemyType enemyType, TextureName spriteName, RenderWindow window, Vector2i playerIndex, Room currentRoom) : base(enemyType, spriteName, window, playerIndex, currentRoom)
    {

    }

    protected override List<Vector2i> GetAttackTiles()
    {
        List<Vector2i> surroundingTiles = new List<Vector2i>();
        for(int i = 1; i <= 2; i++)
        {
            surroundingTiles.Add(new Vector2i(TileIndex.X, TileIndex.Y - i));
            surroundingTiles.Add(new Vector2i(TileIndex.X, TileIndex.Y + i));
            surroundingTiles.Add(new Vector2i(TileIndex.X - i, TileIndex.Y));
            surroundingTiles.Add(new Vector2i(TileIndex.X + i, TileIndex.Y));
            surroundingTiles.Add(new Vector2i(TileIndex.X + i, TileIndex.Y + i));
            surroundingTiles.Add(new Vector2i(TileIndex.X - i, TileIndex.Y + i));
            surroundingTiles.Add(new Vector2i(TileIndex.X - i, TileIndex.Y - i));
            surroundingTiles.Add(new Vector2i(TileIndex.X + i, TileIndex.Y - i));
        }
        
        return surroundingTiles;
    }

}