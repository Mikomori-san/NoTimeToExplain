using SFML.Graphics;
using SFML.System;

public class Room
{
    private Texture tileset;
    private List<Sprite> tiles;
    private List<int[]> map;
    private int tileSize;
    private List<Enemy> enemies;
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

    public Room(string pathToRoomFile, int tileSize)
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
    }

    public void Draw(RenderWindow window)
    {
        for(int y = 0; y < map.Count; y++)
        {
            for(int x = 0; x < map[y].Length; x++)
            {
                int tileIndex = map[y][x];
                if(tileIndex != -1)
                {
                    Sprite tile = tiles[tileIndex];
                    tile.Position = new Vector2f(-window.GetView().Size.X / 2 + x * tileSize, -window.GetView().Size.Y / 2 + y * tileSize);
                    window.Draw(tile);
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
}