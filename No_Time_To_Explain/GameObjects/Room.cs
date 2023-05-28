using SFML.Graphics;
using SFML.System;

public class Room
{
    public string Name;
    private Texture tileset;
    private List<Sprite> tiles;
    private List<int[]> map;
    private int tileSize;
    private Sprite spawnTile;
    private List<Sprite> enemySpawnTiles = new();
    private Sprite teleporterTile;
    private Sprite nextRoomTile;
    private Sprite previousRoomTile;
    private List<Enemy> enemies = new List<Enemy>();
    public const int SPAWN_TILE_INDEX = 05;
    public const int ENEMY_SPAWN_TILE_INDEX = 06;
    public const int TELEPORTER_TILE_INDEX = 07;
    public const int NEXT_ROOM_INDEX = 08;
    public const int PREVIOUS_ROOM_INDEX = 09;
    private Sprite tile;
    private Color originalColor;
    public bool hasTeleporter;
    public bool hasSpawnTile;
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
    public Sprite TeleporterTile
    {
        get
        {
            return teleporterTile;
        }
    }
    public Sprite PreviousRoomTile
    {
        get
        {
            return previousRoomTile;
        }
    }
    public Sprite NextRoomTile
    {
        get
        {
            return nextRoomTile;
        }
    }
    
    public Room(string name, string pathToRoomFile, int tileSize, RenderWindow window, bool hasSpawnTile, bool hasTeleporter, bool hasNextRoom, bool hasPreviousRoom)
    {
        Name = name;
        this.hasSpawnTile = hasSpawnTile;
        this.hasTeleporter = hasTeleporter;

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

        FindEnemySpawnTiles(window);

        if(hasSpawnTile)
        {
            FindSpawnTile(window);
        }

        if(hasTeleporter)
        {
            FindTeleporterTile(window);
        }

        if(hasNextRoom)
        {
            FindNextRoomTile(window);
        }

        if(hasPreviousRoom)
        {
            FindPreviousRoomTile(window);
        }
    }

    private void FindEnemySpawnTiles(RenderWindow window)
    {
        for(int y = 0; y < map.Count; y++)
        {
            for(int x = 0; x < map[y].Length; x++)
            {
                int tileIndex = map[y][x];
                if(tileIndex == ENEMY_SPAWN_TILE_INDEX)
                {
                    Sprite enemySpawnTile = new Sprite(tiles[tileIndex]);
                    enemySpawnTile.Position = new Vector2f(-window.GetView().Size.X / 2 + x * tileSize, -window.GetView().Size.Y / 2 + y * tileSize);
                    enemySpawnTiles.Add(enemySpawnTile);
                }
            }
        }
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
            if(enemy.readiedAttack || enemy.IsHighlighted)
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
                    return;
                }
            }
        }
    }

    private void FindTeleporterTile(RenderWindow window)
    {
        
        for(int y = 0; y < map.Count; y++)
        {
            for(int x = 0; x < map[y].Length; x++)
            {
                int tileIndex = map[y][x];
                if(tileIndex == TELEPORTER_TILE_INDEX)
                {
                    teleporterTile = tiles[tileIndex];
                    teleporterTile.Position = new Vector2f(-window.GetView().Size.X / 2 + x * tileSize, -window.GetView().Size.Y / 2 + y * tileSize);
                    return;
                }       
            }
        }
    }

    private void FindNextRoomTile(RenderWindow window)
    {
        
        for(int y = 0; y < map.Count; y++)
        {
            for(int x = 0; x < map[y].Length; x++)
            {
                int tileIndex = map[y][x];
                if(tileIndex == NEXT_ROOM_INDEX)
                {
                    nextRoomTile = tiles[tileIndex];
                    nextRoomTile.Position = new Vector2f(-window.GetView().Size.X / 2 + x * tileSize, -window.GetView().Size.Y / 2 + y * tileSize);
                    return;
                }       
            }
        }
    }

    private void FindPreviousRoomTile(RenderWindow window)
    {
        
        for(int y = 0; y < map.Count; y++)
        {
            for(int x = 0; x < map[y].Length; x++)
            {
                int tileIndex = map[y][x];
                if(tileIndex == PREVIOUS_ROOM_INDEX)
                {
                    previousRoomTile = tiles[tileIndex];
                    previousRoomTile.Position = new Vector2f(-window.GetView().Size.X / 2 + x * tileSize, -window.GetView().Size.Y / 2 + y * tileSize);
                    return;
                }       
            }
        }
    }
}