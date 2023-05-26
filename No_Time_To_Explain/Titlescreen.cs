using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Titlescreen
{
    private RenderWindow window;
    private Sprite background;
    private Sprite startButtonSprite;
    private bool startButtonPressed;
    private Music devilTrigger;

    public Titlescreen()
    {
        VideoMode mode = new VideoMode(1920, 1080);
        string title = "No Time To Explain";
        window = new RenderWindow(mode, title, Styles.Fullscreen); //Styles.Fullscreen
        AssetManager.Instance.LoadTexture("startbutton", "startButton.png");
        AssetManager.Instance.LoadTexture("titleScreenBackground", "title.jpg");
        AssetManager.Instance.LoadMusic("devilTrigger", "DevilTrigger.ogg");
        startButtonPressed = false;

        devilTrigger = AssetManager.Instance.Music["devilTrigger"];
        devilTrigger.Volume *= 0.1f;

        startButtonSprite = new Sprite(new Texture(AssetManager.Instance.Textures["startbutton"]));
        startButtonSprite.Origin = new Vector2f(startButtonSprite.GetGlobalBounds().Left + startButtonSprite.GetGlobalBounds().Width / 2, startButtonSprite.GetGlobalBounds().Top + startButtonSprite.GetGlobalBounds().Height / 2);
        startButtonSprite.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2);
        startButtonSprite.Scale *= 0.05f;

        background = new Sprite(new Texture(AssetManager.Instance.Textures["titleScreenBackground"]));
        
        window.MouseButtonPressed += OnMouseButtonPressed;
    }

    public void Run()
    {
        devilTrigger.Play();
        window.SetFramerateLimit(144);
        while (window.IsOpen && !startButtonPressed)
        {
            window.DispatchEvents();

            window.Clear(Color.Black);

            window.Draw(background);
            window.Draw(startButtonSprite);

            window.Display();
        }

        // Starte das Spiel, wenn der Startknopf gedrückt wurde
        if (startButtonPressed)
        {
            devilTrigger.Stop();
            Game game = new Game();
            game.Run();
        }
    }

    private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
    {
        // Überprüfe, ob der Mausklick innerhalb des Startknopfs liegt
        if (startButtonSprite.GetGlobalBounds().Contains(e.X, e.Y))
        {
            startButtonPressed = true;
        }
    }
}
