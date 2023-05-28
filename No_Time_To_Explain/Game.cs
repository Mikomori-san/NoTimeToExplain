using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;


public class Game
{
    private RenderWindow window;
    private View view;
    private VideoMode mode;
    private float gameTime = 0;
    private Player player;
    private Room room1;
    private Room room2;
    private Room room3;
    private Hud hud;
    private EnemyHandler enemyHandler;
    private const uint ORIGINAL_WIDTH = 1920; //1280 
    private const uint ORIGINAL_HEIGHT = 1080; //720
    private Room currentRoom;
    private Music? backgroundMusic;
    private Sound levelSwitch;
    private Vector2f ScalingFactor;
    private KillHandler killHandler;
    public const int TILE_SIZE = 48;
    private float aspectRatio = 1;
    private Sprite backgroundSprite;
    private List<Room> randomRoomsToGenerate;

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
        }
    }

    private void Update(float deltaTime)
    {
        gameTime += deltaTime;
        player.Update(deltaTime);
        enemyHandler.Update(deltaTime);
        hud.Update(deltaTime);
        killHandler.SearchForCollisions();
        RoomManagement();
        InputManager.Instance.Update(deltaTime);
    } 

    private void Draw()
    {
        window.Clear();
        window.Draw(backgroundSprite);
        currentRoom.Draw(window);
        player.Draw(window);
        enemyHandler.Draw(window);
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
        AssetManager.Instance.LoadMusic("background", "Afterlife.ogg");
        AssetManager.Instance.LoadSound("obstacleHit", "obstacleHit.wav");
        AssetManager.Instance.LoadSound("woosh", "woosh.wav");
        AssetManager.Instance.LoadSound("levelSwitch", "levelSwitch.wav");
        AssetManager.Instance.LoadSound("kill", "kill.wav");
        
        backgroundSprite = new Sprite(AssetManager.Instance.Textures["background"]);
        backgroundSprite.Position = new Vector2f(-window.GetView().Size.X / 2, -window.GetView().Size.Y / 2);
        backgroundSprite.Scale *= 3;

        backgroundMusic = AssetManager.Instance.Music["background"];
        backgroundMusic.Volume *= 0.1f;
        levelSwitch = new Sound(Utils.TrimSound(AssetManager.Instance.Sounds["levelSwitch"], 1.5f));
        levelSwitch.Volume *= 0.7f;

        hud = new Hud(window);
        hud.Initialize();

        room1 = new Room("Room 1", "./Assets/Rooms/Room1.txt", TILE_SIZE, window, true, false, true, false); //spawn Room
        room2 = new Room("Room 2", "./Assets/Rooms/Room2.txt", TILE_SIZE, window, false, false, true, true); //middle Room

        randomRoomsToGenerate = new List<Room>();
        randomRoomsToGenerate.Add(room2);
        
        RoomHandler.Instance.SetFirstRoom(room1);
        currentRoom = RoomHandler.Instance.GetCurrentRoom();

        player = new Player(window, hud);
        player.SetCurrentRoom(room1);
        player.Initialize();
        EnemySetter();

        enemyHandler = new EnemyHandler(currentRoom.Enemies, currentRoom.EnemySpawnTiles, currentRoom.TileSize); 
        enemyHandler.Initialize();                            

        killHandler = new KillHandler(player, currentRoom.Enemies, hud);

        backgroundMusic.Play();
        backgroundMusic.Loop = true;
    }

    private void SetRoom1Enemies()
    {
        room1.Enemies = new List<Enemy>();
        room1.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", window, player.tileIndex, currentRoom));
        room1.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", window, player.tileIndex, currentRoom));
    }

    private void SetRoom2Enemies()
    {
        room2.Enemies = new List<Enemy>();
        room2.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", window, player.tileIndex, currentRoom));
        room2.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", window, player.tileIndex, currentRoom));
        room2.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", window, player.tileIndex, currentRoom));
        room2.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", window, player.tileIndex, currentRoom));
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
                else
                {
                    Random random = new Random();
                    RoomHandler.Instance.NextRoom(randomRoomsToGenerate[random.Next(0, randomRoomsToGenerate.Count)]);
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
    }

    private void EnemySetter()
    {
        if(currentRoom.Name == room1.Name)
        {
            SetRoom1Enemies();
        } else if(currentRoom.Name == room2.Name)
        {
            SetRoom2Enemies();
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