using SFML.Graphics;
using SFML.System;

public class Room
{
    private Texture tileset;
    private List<Sprite> tiles;
    private List<int[]> map;
    private int tileSize;
    private Sprite spawnTile;
    private List<Sprite> enemySpawnTiles = new();
    private List<Enemy> enemies;
    public const int SPAWN_TILE_INDEX = 05;
    public const int ENEMY_SPAWN_TILE_INDEX = 06;
    private Sprite tile;
    private Color originalColor;

    public List<Sprite> EnemySpawnTiles
    {
        get
        {
            return enemySpawnTiles;
        }
    }
    public int TileSize
    {
        get
        {
            return tileSize;
        }
    }
    public List<Enemy> Enemies
    {
        get
        {
            return enemies;
        }
        set
        {
            enemies = value;
        }
    }
    public List<int[]> Map
    {
        get
        {
            return map;
        }
    }
    public Sprite SpawnTile
    {
        get
        {
            return spawnTile;
        }
    }
    public List<Sprite> Tiles
    {
        get
        {
            return tiles;
        }

        set
        {
            tiles = value;
        }
    }
    public Room(string pathToRoomFile, int tileSize, RenderWindow window)
    {
        tileset = new Texture(AssetManager.Instance.Textures["map"]);
        LoadMap(pathToRoomFile);

        this.tileSize = tileSize;

        tiles = new List<Sprite>();
        for(int y = 0; y < tileset.Size.Y; y += tileSize)
        {
            for(int x = 0; x < tileset.Size.X; x += tileSize)
            {
                tiles.Add(new Sprite(tileset, new IntRect(x, y, tileSize, tileSize)));
            }
        }

        FindSpawnTile(window);
    }

    public void Draw(RenderWindow window)
    {
        for(int y = 0; y < map.Count; y++)
        {
            for(int x = 0; x < map[y].Length; x++)
            {
                int tileIndex = map[y][x];
                tile = tiles[tileIndex];
                originalColor = tile.Color;

                ColorEnemyPatterns(x, y);

                if(tileIndex != -1)
                {
                    tile.Position = new Vector2f(-window.GetView().Size.X / 2 + x * tileSize, -window.GetView().Size.Y / 2 + y * tileSize);
                    window.Draw(tile); 
                    tile.Color = originalColor;
                }        
            }
        }
    }

    private void ColorEnemyPatterns(int x, int y)
    {
        foreach(var enemy in enemies)
        {
            if(enemy.readiedAttack)
            {
                foreach(var attackTileIndex in enemy.AttackPatternTiles)
                {
                    if(new Vector2i(x, y) == attackTileIndex)
                    {
                        tile.Color = Color.Red;
                        return;
                    }                
                }
            }
        }
    }

    private void LoadMap(string pathToRoomFile)
    {
        map = new List<int[]>();
        StreamReader file = new StreamReader(pathToRoomFile);
        while(!file.EndOfStream)
        {
            string line = file.ReadLine();
            int[] row = Array.ConvertAll(line.Split('.'), int.Parse);
            map.Add(row);
        }
    }

    private void FindSpawnTile(RenderWindow window)
    {
        
        for(int y = 0; y < map.Count; y++)
        {
            for(int x = 0; x < map[y].Length; x++)
            {
                int tileIndex = map[y][x];
                if(tileIndex == SPAWN_TILE_INDEX)
                {
                    spawnTile = tiles[tileIndex];
                    spawnTile.Position = new Vector2f(-window.GetView().Size.X / 2 + x * tileSize, -window.GetView().Size.Y / 2 + y * tileSize);
                }
                if(tileIndex == ENEMY_SPAWN_TILE_INDEX)
                {
                    Sprite enemySpawnTile = new Sprite(tiles[tileIndex]);
                    enemySpawnTile.Position = new Vector2f(-window.GetView().Size.X / 2 + x * tileSize, -window.GetView().Size.Y / 2 + y * tileSize);
                    Console.WriteLine("Tile Pos: " + enemySpawnTile.Position);
                    enemySpawnTiles.Add(enemySpawnTile);
                }              
            }
        }
    }
}