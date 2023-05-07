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

    public Game()
    {
        mode = new VideoMode(1280, 720);
        string title = "Assignment 3 Kevin Raffetseder";
        window = new RenderWindow(mode, title);

        view = new View(new Vector2f(mode.Width/2, mode.Height/2), new Vector2f(mode.Width, mode.Height));
        window.SetView(view);

        window.Closed += OnWindowClosed;
        window.Resized += OnResizeWindow;
    }

    private void OnResizeWindow(object? sender, SizeEventArgs e)
    {
        view.Size = new Vector2f(e.Width, e.Height);
        window.SetView(view);

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
        InputManager.Instance.Update(deltaTime);
    }



    private void Draw()
    {
        window.Clear(Color.Blue);
        
        player.Draw(window);

        window.Display();
    }

    private void HandleEvents()
    {
        window.DispatchEvents();
    }

    private void Initialize()
    {
        AssetManager.Instance.LoadTexture("player", "Player/player.png");
        InputManager.Instance.Initialize(window);
        player = new Player(window);
        player.Initialize();
    }

}