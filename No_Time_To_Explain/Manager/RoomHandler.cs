/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

using SFML.Graphics;

public class RoomHandler
{
    private const int COUNT_OF_MIDDLE_ROOM_LEVELS = 10;
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
        startRoom = new Room(RoomName.SpawnRoom, "./Assets/Rooms/SpawnRoom.txt", Game.TILE_SIZE, window, RoomFeatures.HasSpawnTile | RoomFeatures.HasNextRoom); //spawn Room
        teleporterRoom = new Room(RoomName.TeleporterRoom, "./Assets/Rooms/TeleporterRoom.txt", Game.TILE_SIZE, window, RoomFeatures.HasTeleporter | RoomFeatures.HasPreviousRoom); //teleporter Room
        
        for(int i = 1; i <= COUNT_OF_MIDDLE_ROOM_LEVELS; i++)
        {
            availableRandomRooms.Add(new Room((RoomName)Enum.GetValues(typeof(RoomName)).GetValue(i), $"./Assets/Rooms/RandomRoom{i}.txt", Game.TILE_SIZE, window, RoomFeatures.HasNextRoom | RoomFeatures.HasPreviousRoom));
        }
    }

    public void EnemySetter(Player player, RenderWindow window)
    {
        List<EnemyType> enemies = new();
        switch(currentRoom.Name)
        {
            case RoomName.SpawnRoom:
                SetSpawnRoomEnemies(player, window);
                break;
            case RoomName.TeleporterRoom:
                SetTeleporterRoomEnemies(player, window);
                break;
            case RoomName.RandomRoom1:
                enemies = new List<EnemyType>(){EnemyType.LavaGolem, EnemyType.StoneGolem, EnemyType.BaseStoneGolem, EnemyType.BrokenStoneGolem};
                break;
            case RoomName.RandomRoom2:
                enemies = new List<EnemyType>(){EnemyType.BaseStoneGolem, EnemyType.BrokenStoneGolem, EnemyType.StoneGolem, EnemyType.BrokenStoneGolem, EnemyType.StoneGolem, EnemyType.BrokenStoneGolem};
                break;
            case RoomName.RandomRoom3:
                enemies = new List<EnemyType>(){EnemyType.LavaGolem, EnemyType.StoneGolem, EnemyType.BaseStoneGolem, EnemyType.BrokenStoneGolem, EnemyType.LavaGolem, EnemyType.StoneGolem, EnemyType.BaseStoneGolem, EnemyType.BrokenStoneGolem};
                break;
            case RoomName.RandomRoom4:
                enemies = new List<EnemyType>(){EnemyType.LavaGolem, EnemyType.LavaGolem, EnemyType.BrokenStoneGolem, EnemyType.BrokenStoneGolem};
                break;
            case RoomName.RandomRoom5:
                enemies = new List<EnemyType>(){EnemyType.BrokenStoneGolem, EnemyType.BrokenStoneGolem, EnemyType.BaseStoneGolem};
                break;
            case RoomName.RandomRoom6:
                enemies = new List<EnemyType>(){EnemyType.BaseStoneGolem, EnemyType.BaseStoneGolem, EnemyType.BaseStoneGolem, EnemyType.BaseStoneGolem};
                break;
            case RoomName.RandomRoom7:
                enemies = new List<EnemyType>(){EnemyType.BaseStoneGolem, EnemyType.BaseStoneGolem, EnemyType.LavaGolem, EnemyType.LavaGolem};
                break;
            case RoomName.RandomRoom8:
                enemies = new List<EnemyType>(){EnemyType.BaseStoneGolem, EnemyType.BaseStoneGolem, EnemyType.StoneGolem, EnemyType.StoneGolem};
                break;
            case RoomName.RandomRoom9:
                enemies = new List<EnemyType>(){EnemyType.BaseStoneGolem, EnemyType.BaseStoneGolem, EnemyType.StoneGolem, EnemyType.BrokenStoneGolem};
                break;
            case RoomName.RandomRoom10:
                enemies = new List<EnemyType>(){EnemyType.LavaGolem, EnemyType.LavaGolem, EnemyType.StoneGolem, EnemyType.StoneGolem};
                break;
        }

        if(currentRoom.Name != RoomName.SpawnRoom && currentRoom.Name != RoomName.TeleporterRoom)
        {
            SetRandomRoomEnemies(enemies, player, window);
        }
    }

    private void SetRandomRoomEnemies(List<EnemyType> enemies, Player player, RenderWindow window)
    {
        currentRoom.Enemies = new List<Enemy>();
        foreach(var enemy in enemies)
        {
            switch(enemy)
            {
                case EnemyType.LavaGolem:
                    currentRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, TextureName.LavaGolem, window, player.TileIndex, currentRoom));
                    break;
                case EnemyType.BaseStoneGolem:
                    currentRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, TextureName.BaseStoneGolem, window, player.TileIndex, currentRoom));
                    break;
                case EnemyType.BrokenStoneGolem:
                    currentRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, TextureName.BrokenStoneGolem, window, player.TileIndex, currentRoom));
                    break;
                case EnemyType.StoneGolem:
                    currentRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, TextureName.StoneGolem, window, player.TileIndex, currentRoom));
                    break;
            }
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
}