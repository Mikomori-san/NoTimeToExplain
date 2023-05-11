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
    private LavaGolem lavaGolem1;
    private Room room1;
    private Hud hud;
    private EnemyHandler enemyHandler;
    private const uint ORIGINAL_WIDTH = 1280;
    private const uint ORIGINAL_HEIGHT = 720;
    private Room currentRoom;

    public Game()
    {
        mode = new VideoMode(ORIGINAL_WIDTH, ORIGINAL_HEIGHT);
        string title = "No Time To Explain";
        window = new RenderWindow(mode, title);

        view = new View(new Vector2f(), (Vector2f)window.Size);
        window.SetView(view);

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
        AssetManager.Instance.LoadFont("hud", "BrunoAce-Regular.ttf");
        
        player = new Player(window);
        player.Initialize();

        hud = new Hud(window);
        hud.Initialize();

        room1 = new Room("./Assets/Rooms/Room1.txt", 48);
        room1.Enemies = new List<Enemy>();
        
        for(int i = 1; i <= 5; i++)
        {
            room1.Enemies.Add(new LavaGolem(new Vector2f(10, i * -10), EnemyType.LavaGolem, "lavaGolem"));
        }
        
        currentRoom = room1; 

        enemyHandler = new EnemyHandler(currentRoom.Enemies); 
        enemyHandler.Initialize();                                          
    }

}