using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;


public class Game
{
    public RenderWindow window;
    private View view;
    private VideoMode mode;
    private float gameTime = 0;
    private Player player;
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
    private Hud hud;
    private EnemyHandler enemyHandler;
    private const uint ORIGINAL_WIDTH = 1920;
    private const uint ORIGINAL_HEIGHT = 1080;
    private Room currentRoom;
    private Music? backgroundMusic;
    private Sound levelSwitch;
    private Vector2f ScalingFactor;
    private KillHandler killHandler;
    public const int TILE_SIZE = 48;
    private float aspectRatio = 1;
    private Sprite backgroundSprite;
    private List<Room> randomRoomsToGenerate;
    private int maxRandomRoomsCounter = 0;
    private int currentCountOfRandomRooms = 0;
    public bool Retry;
    public bool ReachedTeleporter = false;

    public Game()
    {
        mode = new VideoMode(ORIGINAL_WIDTH, ORIGINAL_HEIGHT);
        string title = "No Time To Explain";
        window = new RenderWindow(mode, title, Styles.Fullscreen); //Styles.Fullscreen
        

        view = new View(new Vector2f(), (Vector2f)window.Size);
        window.SetView(view);
        window.SetFramerateLimit(144);

        window.Closed += OnWindowClosed;
        window.Resized += OnResizeWindow;
    }

    public void Run()
    {
        Initialize();

        Clock clock = new Clock();

        while (window.IsOpen)
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

        if(hud.RemainingTime() <= 0 && !player.IsDead())
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
        window.Clear();
        if(!player.IsDead() && !player.reachedTeleporter)
        {
            window.Draw(backgroundSprite);
            currentRoom.Draw(window);
            enemyHandler.Draw(window);
        }
        player.Draw(window);
        hud.Draw(window);
        window.Display();
    }

    private void HandleEvents()
    {
        window.DispatchEvents();
    }

    private void Initialize()
    {
        InputManager.Instance.Initialize(window);
        AssetManager.Instance.LoadTexture("player", "Player/player.png");
        AssetManager.Instance.LoadTexture("map", "Tiles/Tileset.png");
        AssetManager.Instance.LoadTexture("lavaGolem", "Enemy/LavaGolem/LavaGolemSpriteSheet.png");
        AssetManager.Instance.LoadTexture("stoneGolem", "Enemy/StoneGolem/StoneGolemSpriteSheet.png");
        AssetManager.Instance.LoadTexture("brokenStoneGolem", "Enemy/BrokenStoneGolem/BrokenStoneGolemSpriteSheet.png");
        AssetManager.Instance.LoadTexture("baseStoneGolem", "Enemy/BaseStoneGolem/BaseStoneGolemSpriteSheet.png");
        AssetManager.Instance.LoadTexture("background", "background.png");
        AssetManager.Instance.LoadFont("hud", "BrunoAce-Regular.ttf");
        AssetManager.Instance.LoadFont("death", "NightmarePills-BV2w.ttf");
        AssetManager.Instance.LoadMusic("background", "YourOwnPersonalHell.ogg");
        AssetManager.Instance.LoadSound("levelSwitch", "levelSwitch.wav");
        AssetManager.Instance.LoadSound("kill", "kill.wav");
        
        Retry = false;

        backgroundSprite = new Sprite(AssetManager.Instance.Textures["background"]);
        backgroundSprite.Position = new Vector2f(-window.GetView().Size.X / 2, -window.GetView().Size.Y / 2);
        backgroundSprite.Scale *= 3;

        backgroundMusic = AssetManager.Instance.Music["background"];
        backgroundMusic.Volume *= 0.1f;
        levelSwitch = new Sound(Utils.TrimSound(AssetManager.Instance.Sounds["levelSwitch"], 1.5f));
        levelSwitch.Volume *= 0.7f;

        if(!ReachedTeleporter)
        {
            hud = new Hud(window);
        }
        hud.Initialize();

        ReachedTeleporter = false;

        Random ran = new Random();
        maxRandomRoomsCounter = ran.Next(0, 0); //create between 0 and 4 random rooms, 2 are always created, first spawn and last teleporter 
        Console.WriteLine("Max Count of Rooms " + maxRandomRoomsCounter);

        RoomInitializing();
        
        RoomHandler.Instance.SetFirstRoom(startRoom);
        currentRoom = RoomHandler.Instance.GetCurrentRoom();

        player = new Player(window, hud);
        player.SetCurrentRoom(currentRoom);
        player.Initialize();
        EnemySetter();

        enemyHandler = new EnemyHandler(currentRoom.Enemies, currentRoom.EnemySpawnTiles, currentRoom.TileSize); 
        enemyHandler.Initialize();                            

        killHandler = new KillHandler(player, currentRoom.Enemies, hud);

        backgroundMusic.Play();
        backgroundMusic.Loop = true;
    }

    private void RoomInitializing()
    {
        startRoom = new Room("Spawn Room", "./Assets/Rooms/SpawnRoom.txt", TILE_SIZE, window, true, false, true, false); //spawn Room
        randomRoom1 = new Room("Random Room 1", "./Assets/Rooms/RandomRoom1.txt", TILE_SIZE, window, false, false, true, true);
        teleporterRoom = new Room("Teleporter Room", "./Assets/Rooms/TeleporterRoom.txt", TILE_SIZE, window, false, true, false, true); //teleporter Room
        randomRoom2 = new Room("Random Room 2", "./Assets/Rooms/RandomRoom2.txt", TILE_SIZE, window, false, false, true, true);

        randomRoomsToGenerate = new List<Room>();
        randomRoomsToGenerate.Add(randomRoom1);
        randomRoomsToGenerate.Add(randomRoom2);
    }

    private void SetSpawnRoomEnemies()
    {
        startRoom.Enemies = new List<Enemy>();
        startRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", window, player.tileIndex, currentRoom));
        startRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", window, player.tileIndex, currentRoom));
    }

    private void SetRandomRoom1Enemies()
    {
        randomRoom1.Enemies = new List<Enemy>();
        randomRoom1.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", window, player.tileIndex, currentRoom));
        randomRoom1.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", window, player.tileIndex, currentRoom));
        randomRoom1.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", window, player.tileIndex, currentRoom));
        randomRoom1.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", window, player.tileIndex, currentRoom));
    }

    private void SetTeleporterRoomEnemies()
    {
        teleporterRoom.Enemies = new List<Enemy>();
        teleporterRoom.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", window, player.tileIndex, currentRoom));
        teleporterRoom.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", window, player.tileIndex, currentRoom));
        teleporterRoom.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", window, player.tileIndex, currentRoom));
        teleporterRoom.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", window, player.tileIndex, currentRoom));
    }

    private void SetRandomRoom2Enemies()
    {
        randomRoom2.Enemies = new List<Enemy>();
        randomRoom2.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", window, player.tileIndex, currentRoom));
        randomRoom2.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", window, player.tileIndex, currentRoom));
        randomRoom2.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", window, player.tileIndex, currentRoom));
    }

    private void RoomManagement()
    {
        if(currentRoom.NextRoomTile != null)
        {
            if(!TurnHandler.Instance.IsPlayerTurn() && 
                player.tileIndex == Utils.ConvertToIndex(window, currentRoom.NextRoomTile.Position, currentRoom.NextRoomTile) &&
                !player.GetTurnLock())
            {
                if(RoomHandler.Instance.GetNextRooms().Count > 0)
                {
                    RoomHandler.Instance.NextRoom();
                }
                else if(currentCountOfRandomRooms != maxRandomRoomsCounter)
                {
                    Random random = new Random();
                    RoomHandler.Instance.NextRoom(randomRoomsToGenerate[random.Next(0, randomRoomsToGenerate.Count)]);
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
                player.tileIndex == Utils.ConvertToIndex(window, currentRoom.PreviousRoomTile.Position, currentRoom.PreviousRoomTile) &&
                !player.GetTurnLock())
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
            if(player.tileIndex == Utils.ConvertToIndex(window, currentRoom.TeleporterTile.Position, currentRoom.TeleporterTile))
            {
                hud.ReachedTeleporter();
                player.ReachedTeleporter();
            }
        }
    }

    private void EnemySetter()
    {
        if(currentRoom.Name == startRoom.Name)
        {
            SetSpawnRoomEnemies();
        } 
        else if(currentRoom.Name == randomRoom1.Name)
        {
            SetRandomRoom1Enemies();
        }
        else if(currentRoom.Name == randomRoom2.Name)
        {
            SetRandomRoom2Enemies();
        }
        else if(currentRoom.Name == teleporterRoom.Name)
        {
            SetTeleporterRoomEnemies();
        }
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

        window.Size = new Vector2u((uint)newWidth, (uint)newHeight);

    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        window.Close();
    }
}