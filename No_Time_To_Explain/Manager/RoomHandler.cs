/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

using SFML.Graphics;

public class RoomHandler
{
    private static RoomHandler? instance;
    private Room currentRoom;
    private Stack<Room> previousRooms = new Stack<Room>();
    private Stack<Room> nextRooms = new Stack<Room>();
    private List<Room> availableRandomRooms = new List<Room>();
    public Room startRoom { get; private set; }
    public Room teleporterRoom { get; private set; }
    public static RoomHandler Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new();
            }

            return instance;
        }
    }

    public void SetFirstRoom()
    {
        currentRoom = startRoom;
    }

    public void NextRoom(Room room)
    {
        previousRooms.Push(currentRoom);
        currentRoom = room;
    }

    public void NextRoom()
    {
        previousRooms.Push(currentRoom);
        currentRoom = nextRooms.Pop();
    }

    public void NextTeleporterRoom()
    {
        previousRooms.Push(currentRoom);
        currentRoom = teleporterRoom;
    }

    public void PreviousRoom()
    {
        nextRooms.Push(currentRoom);
        currentRoom = previousRooms.Pop();
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }
    public Stack<Room> GetNextRooms()
    {
        return nextRooms;
    }

    public List<Room> GenerateRandomRooms(int count)
    {
        Random random = new Random();
        List<Room> randomRooms = new List<Room>();

        for (int i = 0; i < count; i++)
        {
            int randomIndex = random.Next(0, availableRandomRooms.Count);

            randomRooms.Add(availableRandomRooms[randomIndex]);

            availableRandomRooms.RemoveAt(randomIndex);
        }

        return randomRooms;
    }
    
    public void SetRooms(RenderWindow window)
    {
        startRoom = new Room("Spawn Room", "./Assets/Rooms/SpawnRoom.txt", Game.TILE_SIZE, window, RoomFeatures.HasSpawnTile | RoomFeatures.HasNextRoom); //spawn Room
        teleporterRoom = new Room("Teleporter Room", "./Assets/Rooms/TeleporterRoom.txt", Game.TILE_SIZE, window, RoomFeatures.HasTeleporter | RoomFeatures.HasPreviousRoom); //teleporter Room
        availableRandomRooms.Add(new Room("Random Room 1", "./Assets/Rooms/RandomRoom1.txt", Game.TILE_SIZE, window, RoomFeatures.HasNextRoom | RoomFeatures.HasPreviousRoom));
        availableRandomRooms.Add(new Room("Random Room 2", "./Assets/Rooms/RandomRoom2.txt", Game.TILE_SIZE, window, RoomFeatures.HasNextRoom | RoomFeatures.HasPreviousRoom));
        availableRandomRooms.Add(new Room("Random Room 3", "./Assets/Rooms/RandomRoom3.txt", Game.TILE_SIZE, window, RoomFeatures.HasNextRoom | RoomFeatures.HasPreviousRoom));
        availableRandomRooms.Add(new Room("Random Room 4", "./Assets/Rooms/RandomRoom4.txt", Game.TILE_SIZE, window, RoomFeatures.HasNextRoom | RoomFeatures.HasPreviousRoom));
        availableRandomRooms.Add(new Room("Random Room 5", "./Assets/Rooms/RandomRoom5.txt", Game.TILE_SIZE, window, RoomFeatures.HasNextRoom | RoomFeatures.HasPreviousRoom));
        availableRandomRooms.Add(new Room("Random Room 6", "./Assets/Rooms/RandomRoom6.txt", Game.TILE_SIZE, window, RoomFeatures.HasNextRoom | RoomFeatures.HasPreviousRoom));
        availableRandomRooms.Add(new Room("Random Room 7", "./Assets/Rooms/RandomRoom7.txt", Game.TILE_SIZE, window, RoomFeatures.HasNextRoom | RoomFeatures.HasPreviousRoom));
        availableRandomRooms.Add(new Room("Random Room 8", "./Assets/Rooms/RandomRoom8.txt", Game.TILE_SIZE, window, RoomFeatures.HasNextRoom | RoomFeatures.HasPreviousRoom));
        availableRandomRooms.Add(new Room("Random Room 9", "./Assets/Rooms/RandomRoom9.txt", Game.TILE_SIZE, window, RoomFeatures.HasNextRoom | RoomFeatures.HasPreviousRoom));
        availableRandomRooms.Add(new Room("Random Room 10", "./Assets/Rooms/RandomRoom10.txt", Game.TILE_SIZE, window, RoomFeatures.HasNextRoom | RoomFeatures.HasPreviousRoom));     
    }

    public void EnemySetter(Player player, RenderWindow window)
    {
        switch(currentRoom.Name)
        {
            case "Spawn Room":
                SetSpawnRoomEnemies(player, window);
                break;
            case "Teleporter Room":
                SetTeleporterRoomEnemies(player, window);
                break;
            case "Random Room 1":
                SetRandomRoom1Enemies(player, window);
                break;
            case "Random Room 2":
                SetRandomRoom2Enemies(player, window);
                break;
            case "Random Room 3":
                SetRandomRoom3Enemies(player, window);
                break;
            case "Random Room 4":
                SetRandomRoom4Enemies(player, window);
                break;
            case "Random Room 5":
                SetRandomRoom5Enemies(player, window);
                break;
            case "Random Room 6":
                SetRandomRoom6Enemies(player, window);
                break;
            case "Random Room 7":
                SetRandomRoom7Enemies(player, window);
                break;
            case "Random Room 8":
                SetRandomRoom8Enemies(player, window);
                break;
            case "Random Room 9":
                SetRandomRoom9Enemies(player, window);
                break;
            case "Random Room 10":
                SetRandomRoom10Enemies(player, window);
                break;
        }
    }

    private void SetSpawnRoomEnemies(Player player, RenderWindow window)
    {
        startRoom.Enemies = new List<Enemy>();
        startRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, TextureName.LavaGolem, window, player.TileIndex, currentRoom));
        startRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, TextureName.StoneGolem, window, player.TileIndex, currentRoom));
    }

    private void SetTeleporterRoomEnemies(Player player, RenderWindow window)
    {
        teleporterRoom.Enemies = new List<Enemy>();
        teleporterRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, TextureName.LavaGolem, window, player.TileIndex, currentRoom));
        teleporterRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, TextureName.StoneGolem, window, player.TileIndex, currentRoom));
        teleporterRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
        teleporterRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, TextureName.BrokenStoneGolem, window, player.TileIndex, currentRoom));
    }

    private void SetRandomRoom1Enemies(Player player, RenderWindow window)
    {
        currentRoom.Enemies = new List<Enemy>();
        currentRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, TextureName.LavaGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, TextureName.StoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, TextureName.BrokenStoneGolem, window, player.TileIndex, currentRoom));
    }

    private void SetRandomRoom2Enemies(Player player, RenderWindow window)
    {
        currentRoom.Enemies = new List<Enemy>();
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, TextureName.BrokenStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, TextureName.StoneGolem, window, player.TileIndex, currentRoom));  
        currentRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, TextureName.BrokenStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, TextureName.StoneGolem, window, player.TileIndex, currentRoom));  
        currentRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, TextureName.BrokenStoneGolem, window, player.TileIndex, currentRoom));
    }

    private void SetRandomRoom3Enemies(Player player, RenderWindow window)
    {
        currentRoom.Enemies = new List<Enemy>();
        currentRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, TextureName.LavaGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, TextureName.StoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, TextureName.BrokenStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, TextureName.LavaGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, TextureName.StoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, TextureName.BrokenStoneGolem, window, player.TileIndex, currentRoom));
    }

    private void SetRandomRoom4Enemies(Player player, RenderWindow window)
    {
        currentRoom.Enemies = new List<Enemy>();
        currentRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, TextureName.LavaGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, TextureName.LavaGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, TextureName.BrokenStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, TextureName.BrokenStoneGolem, window, player.TileIndex, currentRoom));
    }

    private void SetRandomRoom5Enemies(Player player, RenderWindow window)
    {
        currentRoom.Enemies = new List<Enemy>();
        currentRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, TextureName.BrokenStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, TextureName.BrokenStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
    }

    private void SetRandomRoom6Enemies(Player player, RenderWindow window)
    {
        currentRoom.Enemies = new List<Enemy>();
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
    }

    private void SetRandomRoom7Enemies(Player player, RenderWindow window)
    {
        currentRoom.Enemies = new List<Enemy>();
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, TextureName.LavaGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, TextureName.LavaGolem, window, player.TileIndex, currentRoom));
    }

    private void SetRandomRoom8Enemies(Player player, RenderWindow window)
    {
        currentRoom.Enemies = new List<Enemy>();
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, TextureName.StoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, TextureName.StoneGolem, window, player.TileIndex, currentRoom));
    }

    private void SetRandomRoom9Enemies(Player player, RenderWindow window)
    {
        currentRoom.Enemies = new List<Enemy>();
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, TextureName.StoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, TextureName.BrokenStoneGolem, window, player.TileIndex, currentRoom));
    }

    private void SetRandomRoom10Enemies(Player player, RenderWindow window)
    {
        currentRoom.Enemies = new List<Enemy>();
        currentRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, TextureName.LavaGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, TextureName.LavaGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, TextureName.StoneGolem, window, player.TileIndex, currentRoom));
        currentRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, TextureName.StoneGolem, window, player.TileIndex, currentRoom));
    }
}