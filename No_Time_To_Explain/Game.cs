using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;


public class Game
{
    // Constants
    private const uint ORIGINAL_WIDTH = 1920;
    private const uint ORIGINAL_HEIGHT = 1080;
    public const int TILE_SIZE = 48;

    // Properties
    public RenderWindow Window { get; set; }
    private View view;
    private VideoMode mode;
    private float gameTime = 0;
    private Player player;
    private Hud hud;
    private EnemyHandler enemyHandler;
    private Room currentRoom;
    private Music? backgroundMusic;
    private Sound levelSwitch;
    private Vector2f ScalingFactor;
    private KillHandler killHandler;
    private float aspectRatio = 1;
    private Sprite backgroundSprite;
    private List<Room> randomRoomsToGenerate;
    private int maxRandomRoomsCounter = 0;
    private int currentCountOfRandomRooms = 0;
    private List<Music> backgroundTracks;
    public bool Retry { get; set; }
    public bool ReachedTeleporter { get; set; }

    //Rooms
    private Room startRoom;
    private Room randomRoom1;
    private Room randomRoom2;
    private Room randomRoom3;
    private Room randomRoom4;
    private Room randomRoom5;
    private Room randomRoom6;
    private Room randomRoom7;
    private Room randomRoom8;
    private Room randomRoom9;
    private Room randomRoom10;
    private Room teleporterRoom;

    public Game(RenderWindow window)
    {
        Window = window;
        
        view = new View(new Vector2f(), (Vector2f)Window.Size);
        Window.SetView(view);

        Window.Closed += OnWindowClosed;
        Window.Resized += OnResizeWindow;
    }

    public void Run()
    {
        Initialize();

        Clock clock = new Clock();

        while (Window.IsOpen)
        {
            float deltaTime = clock.Restart().AsSeconds();

            HandleEvents();

            Update(deltaTime);

            Draw();

            if(this.Retry)
            {
                Run();
            }

            if(this.ReachedTeleporter)
            {
                Run();
            }
        }
    }

    private void Update(float deltaTime)
    {
        gameTime += deltaTime;
        player.Update(deltaTime);
        
        if(!player.IsDead() && !player.reachedTeleporter)
        {
            enemyHandler.Update(deltaTime);
            killHandler.SearchForCollisions();
            RoomManagement();
        }
        else
        {
            killHandler.UpdateEnemies(new List<Enemy>());
            backgroundMusic.Stop();
        }
        hud.Update(deltaTime);
        
        if(hud.Retry)
        {
            this.Retry = true;
            player.DeathMusic.Stop();
        }

        if(hud.RemainingTime() <= 0 && !player.IsDead() && !player.reachedTeleporter)
        {
            killHandler.KillPlayer();
        }

        if(player.GameResetTeleporter)
        {
            ReachedTeleporter = true;
        }

        InputManager.Instance.Update(deltaTime);
    } 

    private void Draw()
    {
        Window.Clear();
        if(!player.IsDead() && !player.reachedTeleporter)
        {
            Window.Draw(backgroundSprite);
            currentRoom.Draw(Window);
            enemyHandler.Draw(Window);
        }
        player.Draw(Window);
        hud.Draw(Window);
        Window.Display();
    }

    private void HandleEvents()
    {
        Window.DispatchEvents();
    }

    private void Initialize()
    {
        InputManager.Instance.Initialize(Window);
        AssetManager.Instance.LoadTexture("player", "Player/player.png");
        AssetManager.Instance.LoadTexture("map", "Tiles/Tileset.png");
        AssetManager.Instance.LoadTexture("lavaGolem", "Enemy/LavaGolem/LavaGolemSpriteSheet.png");
        AssetManager.Instance.LoadTexture("stoneGolem", "Enemy/StoneGolem/StoneGolemSpriteSheet.png");
        AssetManager.Instance.LoadTexture("brokenStoneGolem", "Enemy/BrokenStoneGolem/BrokenStoneGolemSpriteSheet.png");
        AssetManager.Instance.LoadTexture("baseStoneGolem", "Enemy/BaseStoneGolem/BaseStoneGolemSpriteSheet.png");
        AssetManager.Instance.LoadTexture("background", "background.png");
        AssetManager.Instance.LoadMusic("background1", "YourOwnPersonalHell.ogg");
        AssetManager.Instance.LoadMusic("background2", "DemonSlayer.ogg");
        AssetManager.Instance.LoadSound("levelSwitch", "levelSwitch.wav");
        
        Retry = false;

        currentCountOfRandomRooms = 0;

        backgroundSprite = new Sprite(AssetManager.Instance.Textures["background"]);
        backgroundSprite.Position = new Vector2f(-Window.GetView().Size.X / 2, -Window.GetView().Size.Y / 2);
        backgroundSprite.Scale *= 3;

        backgroundTracks = new List<Music>();
        backgroundTracks.Add(AssetManager.Instance.Music["background1"]);
        backgroundTracks.Add(AssetManager.Instance.Music["background2"]);
        Random randomTracks = new Random();
        backgroundMusic = backgroundTracks[randomTracks.Next(0, backgroundTracks.Count)];
        backgroundMusic.Volume *= 0.1f;


        levelSwitch = new Sound(Utils.TrimSound(AssetManager.Instance.Sounds["levelSwitch"], 1.5f));
        levelSwitch.Volume *= 0.7f;

        if(!ReachedTeleporter)
        {
            hud = new Hud(Window);
        }
        hud.Initialize();

        ReachedTeleporter = false;

        Random ran = new Random();
        maxRandomRoomsCounter = ran.Next(0, 4); //create between 0 and 4 random rooms between the spawn and teleporter room
        Console.WriteLine("Max Count of Rooms " + maxRandomRoomsCounter);

        RoomInitializing();
        
        RoomHandler.Instance.SetFirstRoom(startRoom);
        currentRoom = RoomHandler.Instance.GetCurrentRoom();

        player = new Player(Window, hud);
        player.SetCurrentRoom(currentRoom);
        player.Initialize();
        EnemySetter();

        enemyHandler = new EnemyHandler(currentRoom.Enemies, currentRoom.EnemySpawnTiles, currentRoom.TileSize); 
        enemyHandler.Initialize();                            

        killHandler = new KillHandler(player, currentRoom.Enemies, hud);

        backgroundMusic.Play();
        backgroundMusic.Loop = true;
    }

    private void RoomManagement()
    {
        if(currentRoom.NextRoomTile != null)
        {
            if(!TurnHandler.Instance.IsPlayerTurn() && 
                player.tileIndex == Utils.ConvertToIndex(Window, currentRoom.NextRoomTile) &&
                !player.GetTurnLock()
            )
            {
                if(RoomHandler.Instance.GetNextRooms().Count > 0)
                {
                    RoomHandler.Instance.NextRoom();
                }
                else if(currentCountOfRandomRooms < maxRandomRoomsCounter)
                {
                    Console.WriteLine("Current Count of Random Rooms" + currentCountOfRandomRooms);
                    Random random = new Random();
                    int randomRoom = random.Next(0, randomRoomsToGenerate.Count);

                    while(randomRoomsToGenerate[randomRoom].Name == currentRoom.Name)
                    {
                        randomRoom = random.Next(0, randomRoomsToGenerate.Count);
                    }

                    RoomHandler.Instance.NextRoom(randomRoomsToGenerate[randomRoom]);
                    hud.Add30Seconds();
                    currentCountOfRandomRooms++;
                }
                else
                {
                    RoomHandler.Instance.NextRoom(teleporterRoom);
                    hud.Add30Seconds();
                }
                currentRoom = RoomHandler.Instance.GetCurrentRoom();
                EnemySetter();
                player.SetCurrentRoom(currentRoom);
                player.SpawnPlayerFromNextRoomTile();
                enemyHandler.UpdateEnemies(currentRoom.Enemies, currentRoom.EnemySpawnTiles);
                killHandler.UpdateEnemies(currentRoom.Enemies);
                player.SetTurnLock();
                levelSwitch.Play();
            }
        }
        
        if(currentRoom.PreviousRoomTile != null)
        {
            if(!TurnHandler.Instance.IsPlayerTurn()  && 
                player.tileIndex == Utils.ConvertToIndex(Window, currentRoom.PreviousRoomTile) &&
                !player.GetTurnLock()
            )
            {
                RoomHandler.Instance.PreviousRoom();
                currentRoom = RoomHandler.Instance.GetCurrentRoom();
                EnemySetter();
                player.SetCurrentRoom(currentRoom);
                player.SpawnPlayerFromPreviousRoomTile();
                enemyHandler.UpdateEnemies(currentRoom.Enemies, currentRoom.EnemySpawnTiles);
                killHandler.UpdateEnemies(currentRoom.Enemies);
                player.SetTurnLock();
                levelSwitch.Play();
            }
        }

        if(currentRoom.TeleporterTile != null)
        {
            if(player.tileIndex == Utils.ConvertToIndex(Window, currentRoom.TeleporterTile))
            {
                hud.ReachedTeleporter();
                player.ReachedTeleporter();
            }
        }
    }

    private void RoomInitializing()
    {
        startRoom = new Room("Spawn Room", "./Assets/Rooms/SpawnRoom.txt", TILE_SIZE, Window, true, false, true, false); //spawn Room
        randomRoom1 = new Room("Random Room 1", "./Assets/Rooms/RandomRoom1.txt", TILE_SIZE, Window, false, false, true, true);
        randomRoom2 = new Room("Random Room 2", "./Assets/Rooms/RandomRoom2.txt", TILE_SIZE, Window, false, false, true, true);
        randomRoom3 = new Room("Random Room 3", "./Assets/Rooms/RandomRoom3.txt", TILE_SIZE, Window, false, false, true, true);
        randomRoom4 = new Room("Random Room 4", "./Assets/Rooms/RandomRoom4.txt", TILE_SIZE, Window, false, false, true, true);
        randomRoom5 = new Room("Random Room 5", "./Assets/Rooms/RandomRoom5.txt", TILE_SIZE, Window, false, false, true, true);
        randomRoom6 = new Room("Random Room 6", "./Assets/Rooms/RandomRoom6.txt", TILE_SIZE, Window, false, false, true, true);
        randomRoom7 = new Room("Random Room 7", "./Assets/Rooms/RandomRoom7.txt", TILE_SIZE, Window, false, false, true, true);
        randomRoom8 = new Room("Random Room 8", "./Assets/Rooms/RandomRoom8.txt", TILE_SIZE, Window, false, false, true, true);
        randomRoom9 = new Room("Random Room 9", "./Assets/Rooms/RandomRoom9.txt", TILE_SIZE, Window, false, false, true, true);
        randomRoom10 = new Room("Random Room 10", "./Assets/Rooms/RandomRoom10.txt", TILE_SIZE, Window, false, false, true, true);
        teleporterRoom = new Room("Teleporter Room", "./Assets/Rooms/TeleporterRoom.txt", TILE_SIZE, Window, false, true, false, true); //teleporter Room

        randomRoomsToGenerate = new List<Room>();
        randomRoomsToGenerate.Add(randomRoom1);
        randomRoomsToGenerate.Add(randomRoom2);
        randomRoomsToGenerate.Add(randomRoom3);
        randomRoomsToGenerate.Add(randomRoom4);
        randomRoomsToGenerate.Add(randomRoom5);
        randomRoomsToGenerate.Add(randomRoom6);
        randomRoomsToGenerate.Add(randomRoom7);
        randomRoomsToGenerate.Add(randomRoom8);
        randomRoomsToGenerate.Add(randomRoom9);
        randomRoomsToGenerate.Add(randomRoom10);
    }

    private void EnemySetter()
    {
        switch(currentRoom.Name)
        {
            case "Spawn Room":
                SetSpawnRoomEnemies();
                break;
            case "Teleporter Room":
                SetTeleporterRoomEnemies();
                break;
            case "Random Room 1":
                SetRandomRoom1Enemies();
                break;
            case "Random Room 2":
                SetRandomRoom2Enemies();
                break;
            case "Random Room 3":
                SetRandomRoom3Enemies();
                break;
            case "Random Room 4":
                SetRandomRoom4Enemies();
                break;
            case "Random Room 5":
                SetRandomRoom5Enemies();
                break;
            case "Random Room 6":
                SetRandomRoom6Enemies();
                break;
            case "Random Room 7":
                SetRandomRoom7Enemies();
                break;
            case "Random Room 8":
                SetRandomRoom8Enemies();
                break;
            case "Random Room 9":
                SetRandomRoom9Enemies();
                break;
            case "Random Room 10":
                SetRandomRoom10Enemies();
                break;
        }
    }

    private void SetSpawnRoomEnemies()
    {
        startRoom.Enemies = new List<Enemy>();
        startRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", Window, player.tileIndex, currentRoom));
        startRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", Window, player.tileIndex, currentRoom));
    }

    private void SetTeleporterRoomEnemies()
    {
        teleporterRoom.Enemies = new List<Enemy>();
        teleporterRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", Window, player.tileIndex, currentRoom));
        teleporterRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", Window, player.tileIndex, currentRoom));
        teleporterRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
        teleporterRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", Window, player.tileIndex, currentRoom));
    }

    private void SetRandomRoom1Enemies()
    {
        randomRoom1.Enemies = new List<Enemy>();
        randomRoom1.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", Window, player.tileIndex, currentRoom));
        randomRoom1.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom1.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom1.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", Window, player.tileIndex, currentRoom));
    }

    private void SetRandomRoom2Enemies()
    {
        randomRoom2.Enemies = new List<Enemy>();
        randomRoom2.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom2.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom2.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", Window, player.tileIndex, currentRoom));  
        randomRoom2.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom2.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", Window, player.tileIndex, currentRoom));  
        randomRoom2.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", Window, player.tileIndex, currentRoom));
    }

    private void SetRandomRoom3Enemies()
    {
        randomRoom3.Enemies = new List<Enemy>();
        randomRoom3.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", Window, player.tileIndex, currentRoom));
        randomRoom3.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom3.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom3.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom3.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", Window, player.tileIndex, currentRoom));
        randomRoom3.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom3.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom3.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", Window, player.tileIndex, currentRoom));
    }

    private void SetRandomRoom4Enemies()
    {
        randomRoom4.Enemies = new List<Enemy>();
        randomRoom4.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", Window, player.tileIndex, currentRoom));
        randomRoom4.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", Window, player.tileIndex, currentRoom));
        randomRoom4.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom4.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", Window, player.tileIndex, currentRoom));
    }

    private void SetRandomRoom5Enemies()
    {
        randomRoom5.Enemies = new List<Enemy>();
        randomRoom5.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom5.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom5.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
    }

    private void SetRandomRoom6Enemies()
    {
        randomRoom6.Enemies = new List<Enemy>();
        randomRoom6.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom6.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom6.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom6.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
    }

    private void SetRandomRoom7Enemies()
    {
        randomRoom7.Enemies = new List<Enemy>();
        randomRoom7.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom7.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom7.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", Window, player.tileIndex, currentRoom));
        randomRoom7.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", Window, player.tileIndex, currentRoom));
    }

    private void SetRandomRoom8Enemies()
    {
        randomRoom8.Enemies = new List<Enemy>();
        randomRoom8.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom8.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom8.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom8.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", Window, player.tileIndex, currentRoom));
    }

    private void SetRandomRoom9Enemies()
    {
        randomRoom9.Enemies = new List<Enemy>();
        randomRoom9.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom9.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom9.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom9.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", Window, player.tileIndex, currentRoom));
    }

    private void SetRandomRoom10Enemies()
    {
        randomRoom10.Enemies = new List<Enemy>();
        randomRoom10.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", Window, player.tileIndex, currentRoom));
        randomRoom10.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", Window, player.tileIndex, currentRoom));
        randomRoom10.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", Window, player.tileIndex, currentRoom));
        randomRoom10.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", Window, player.tileIndex, currentRoom));
    }

    private void OnResizeWindow(object? sender, SizeEventArgs e)
    {
        aspectRatio = (float)ORIGINAL_WIDTH / ORIGINAL_HEIGHT;

        int newWidth = (int)(e.Height * aspectRatio);
        int newHeight = (int)e.Height;

        if (newWidth > e.Width)
        {
            newWidth = (int)e.Width;
            newHeight = (int)(newWidth / aspectRatio);
        }

        Window.Size = new Vector2u((uint)newWidth, (uint)newHeight);

    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        Window.Close();
    }
}