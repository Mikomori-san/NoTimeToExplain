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
    private Hud hud;
    private EnemyHandler enemyHandler;
    private const uint ORIGINAL_WIDTH = 1280; //1920
    private const uint ORIGINAL_HEIGHT = 720; //1080
    private Room currentRoom;
    private Music? backgroundMusic;
    private Vector2f ScalingFactor;
    private KillHandler killHandler;
    public const int TILE_SIZE = 48;
    private float aspectRatio = 1;

    public Game()
    {
        mode = new VideoMode(ORIGINAL_WIDTH, ORIGINAL_HEIGHT);
        string title = "No Time To Explain";
        window = new RenderWindow(mode, title); //Styles.Fullscreen
        

        view = new View(new Vector2f(), (Vector2f)window.Size);
        window.SetView(view);
        window.SetFramerateLimit(144);

        window.Closed += OnWindowClosed;
        window.Resized += OnResizeWindow;
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
        InputManager.Instance.Update(deltaTime);
    }

    private void Draw()
    {
        window.Clear(Color.Blue);
        
        room1.Draw(window);
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
        AssetManager.Instance.LoadFont("hud", "BrunoAce-Regular.ttf");
        AssetManager.Instance.LoadMusic("background", "backgroundMusic1.ogg");
        AssetManager.Instance.LoadSound("obstacleHit", "ObstacleHit.wav");
        
        backgroundMusic = AssetManager.Instance.Music["background"];
        backgroundMusic.Volume *= 0.1f;
        hud = new Hud(window);
        hud.Initialize();

        room1 = new Room("./Assets/Rooms/Room1.txt", TILE_SIZE, window);
        currentRoom = room1;
        room1.Enemies = new List<Enemy>();

        player = new Player(window, hud);
        player.SetCurrentRoom(room1);
        player.Initialize();

        room1.Enemies.Add(new LavaGolem(EnemyType.LavaGolem, "lavaGolem", window, player.tileIndex, currentRoom));
        room1.Enemies.Add(new StoneGolem(EnemyType.StoneGolem, "stoneGolem", window, player.tileIndex, currentRoom));
        room1.Enemies.Add(new BrokenStoneGolem(EnemyType.BrokenStoneGolem, "brokenStoneGolem", window, player.tileIndex, currentRoom));
        room1.Enemies.Add(new BaseStoneGolem(EnemyType.BaseStoneGolem, "baseStoneGolem", window, player.tileIndex, currentRoom)); 

        enemyHandler = new EnemyHandler(currentRoom.Enemies, currentRoom.EnemySpawnTiles, currentRoom.TileSize); 
        enemyHandler.Initialize();                            


        killHandler = new KillHandler(player, currentRoom.Enemies, hud);

        backgroundMusic.Play();
        backgroundMusic.Loop = true;
    }
}