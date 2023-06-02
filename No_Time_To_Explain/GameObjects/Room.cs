using SFML.Graphics;
using SFML.System;

public class Room
{
    // Constants
    public const int SPAWN_TILE_INDEX = 5;
    public const int ENEMY_SPAWN_TILE_INDEX = 6;
    public const int TELEPORTER_TILE_INDEX = 7;
    public const int NEXT_ROOM_INDEX = 8;
    public const int PREVIOUS_ROOM_INDEX = 9;

    // Properties
    public string Name { get; set; }
    private Texture tileset;
    private List<Sprite> tiles;
    public List<int[]> Map { get; private set; }
    public Sprite SpawnTile { get; private set; }
    public List<Sprite> EnemySpawnTiles { get; } = new List<Sprite>();
    public Sprite TeleporterTile { get; private set; }
    public Sprite NextRoomTile { get; private set; }
    public Sprite PreviousRoomTile { get; private set; }
    public List<Enemy> Enemies { get; set; } = new List<Enemy>();
    private Sprite tile;
    private Color originalColor;
    public bool hasTeleporter;
    public bool hasSpawnTile;
    public readonly int TileSize;
    
    public Room(string name, string pathToRoomFile, int tileSize, RenderWindow window, RoomFeatures features)
    {
        Name = name;

        hasSpawnTile = features.HasFlag(RoomFeatures.HasSpawnTile);
        hasTeleporter = features.HasFlag(RoomFeatures.HasTeleporter);
        bool hasNextRoom = features.HasFlag(RoomFeatures.HasNextRoom);
        bool hasPreviousRoom = features.HasFlag(RoomFeatures.HasPreviousRoom);

        tileset = new Texture(AssetManager.Instance.Textures["map"]);
        LoadMap(pathToRoomFile);

        this.TileSize = tileSize;

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
        for(int y = 0; y < Map.Count; y++)
        {
            for(int x = 0; x < Map[y].Length; x++)
            {
                int tileIndex = Map[y][x];
                if(tileIndex == ENEMY_SPAWN_TILE_INDEX)
                {
                    Sprite enemySpawnTile = new Sprite(tiles[tileIndex]);
                    enemySpawnTile.Position = new Vector2f(-window.GetView().Size.X / 2 + x * TileSize, -window.GetView().Size.Y / 2 + y * TileSize);
                    EnemySpawnTiles.Add(enemySpawnTile);
                }
            }
        }
    }

    public void Draw(RenderWindow window)
    {
        for(int y = 0; y < Map.Count; y++)
        {
            for(int x = 0; x < Map[y].Length; x++)
            {
                int tileIndex = Map[y][x];
                tile = tiles[tileIndex];
                originalColor = tile.Color;

                ColorEnemyPatterns(x, y);

                if(tileIndex != -1)
                {
                    tile.Position = new Vector2f(-window.GetView().Size.X / 2 + x * TileSize, -window.GetView().Size.Y / 2 + y * TileSize);
                    window.Draw(tile); 
                    tile.Color = originalColor;
                }        
            }
        }
    }

    private void ColorEnemyPatterns(int x, int y)
    {
        foreach(var enemy in Enemies)
        {
            if(enemy.ReadiedAttack || enemy.IsHighlighted)
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
        Map = new List<int[]>();

        using (StreamReader file = new StreamReader(pathToRoomFile))
        {
            while(!file.EndOfStream)
            {
                string line = file.ReadLine();
                int[] row = Array.ConvertAll(line.Split('.'), int.Parse);
                Map.Add(row);
            }
        }
    }

    private void FindSpawnTile(RenderWindow window)
    {   
        for(int y = 0; y < Map.Count; y++)
        {
            for(int x = 0; x < Map[y].Length; x++)
            {
                int tileIndex = Map[y][x];
                if(tileIndex == SPAWN_TILE_INDEX)
                {
                    SpawnTile = tiles[tileIndex];
                    SpawnTile.Position = new Vector2f(-window.GetView().Size.X / 2 + x * TileSize, -window.GetView().Size.Y / 2 + y * TileSize);
                    return;
                }
            }
        }
    }

    private void FindTeleporterTile(RenderWindow window)
    {
        
        for(int y = 0; y < Map.Count; y++)
        {
            for(int x = 0; x < Map[y].Length; x++)
            {
                int tileIndex = Map[y][x];
                if(tileIndex == TELEPORTER_TILE_INDEX)
                {
                    TeleporterTile = tiles[tileIndex];
                    TeleporterTile.Position = new Vector2f(-window.GetView().Size.X / 2 + x * TileSize, -window.GetView().Size.Y / 2 + y * TileSize);
                    return;
                }       
            }
        }
    }

    private void FindNextRoomTile(RenderWindow window)
    {
        
        for(int y = 0; y < Map.Count; y++)
        {
            for(int x = 0; x < Map[y].Length; x++)
            {
                int tileIndex = Map[y][x];
                if(tileIndex == NEXT_ROOM_INDEX)
                {
                    NextRoomTile = tiles[tileIndex];
                    NextRoomTile.Position = new Vector2f(-window.GetView().Size.X / 2 + x * TileSize, -window.GetView().Size.Y / 2 + y * TileSize);
                    return;
                }       
            }
        }
    }

    private void FindPreviousRoomTile(RenderWindow window)
    {
        
        for(int y = 0; y < Map.Count; y++)
        {
            for(int x = 0; x < Map[y].Length; x++)
            {
                int tileIndex = Map[y][x];
                if(tileIndex == PREVIOUS_ROOM_INDEX)
                {
                    PreviousRoomTile = tiles[tileIndex];
                    PreviousRoomTile.Position = new Vector2f(-window.GetView().Size.X / 2 + x * TileSize, -window.GetView().Size.Y / 2 + y * TileSize);
                    return;
                }       
            }
        }
    }
}