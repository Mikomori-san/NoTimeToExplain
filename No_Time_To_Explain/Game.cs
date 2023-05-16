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
    private const uint ORIGINAL_WIDTH = 1280;
    private const uint ORIGINAL_HEIGHT = 720;
    private Room currentRoom;
    private Music? backgroundMusic;
    private Vector2f ScalingFactor;
    public const int TILE_SIZE = 48;

    public Game()
    {
        mode = new VideoMode(ORIGINAL_WIDTH, ORIGINAL_HEIGHT);
        string title = "No Time To Explain";
        window = new RenderWindow(mode, title);

        view = new View(new Vector2f(), (Vector2f)window.Size);
        window.SetView(view);
        window.SetFramerateLimit(144);

        window.Closed += OnWindowClosed;
        window.Resized += OnResizeWindow;
    }

    private void OnResizeWindow(object? sender, SizeEventArgs e)
    {
        view.Size = new Vector2f(e.Width, e.Height);
        window.SetView(view);
        hud.UpdateWindow(window);
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
        
        backgroundMusic = AssetManager.Instance.Music["background"];

        hud = new Hud(window);
        hud.Initialize();

        room1 = new Room("./Assets/Rooms/Room1.txt", TILE_SIZE);
        room1.Enemies = new List<Enemy>();
        
        room1.Enemies.Add(new LavaGolem(new Vector2f(10, 0), EnemyType.LavaGolem, "lavaGolem", window));
        room1.Enemies.Add(new StoneGolem(new Vector2f(10, 48), EnemyType.StoneGolem, "stoneGolem", window));
        room1.Enemies.Add(new BrokenStoneGolem(new Vector2f(10, 78), EnemyType.BrokenStoneGolem, "brokenStoneGolem", window));
        room1.Enemies.Add(new BaseStoneGolem(new Vector2f(10, 126), EnemyType.BaseStoneGolem, "baseStoneGolem", window));
        
        currentRoom = room1; 

        player = new Player(window);
        player.Initialize();
        player.SetCurrentRoom(room1);

        enemyHandler = new EnemyHandler(currentRoom.Enemies); 
        enemyHandler.Initialize();                            

        //backgroundMusic.Play();
    }
}