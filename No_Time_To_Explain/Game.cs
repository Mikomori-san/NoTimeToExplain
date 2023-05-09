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
    private const uint ORIGINAL_WIDTH = 1280;
    private const uint ORIGINAL_HEIGHT = 720;

    public Game()
    {
        mode = new VideoMode(ORIGINAL_WIDTH, ORIGINAL_HEIGHT);
        string title = "Assignment 3 Kevin Raffetseder";
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
        hud.Update(deltaTime);
        InputManager.Instance.Update(deltaTime);
    }

    private void Draw()
    {
        window.Clear(Color.Blue);
        
        room1.Draw(window);
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
        AssetManager.Instance.LoadFont("hud", "BrunoAce-Regular.ttf");

        player = new Player(window);
        player.Initialize();

        hud = new Hud(window);
        hud.Initialize();

        room1 = new Room("./Assets/Rooms/Room1.txt", 48);
    }

}