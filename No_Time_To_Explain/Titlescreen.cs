/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Titlescreen : IDisposable
{
    //Constants
    private const string STARTBUTTON_TEXTURE_NAME = "/TitleScreen/startButton.png";
    private const string TITLESCREENBACKGROUND_TEXTURE_NAME = "/TitleScreen/titleBackground3.png";
    private const string TITLEBACKGROUND_MUSIC_NAME = "TheSacrifice.ogg";

    //Assets
    private Sprite background;
    private Sprite startButtonSprite;
    private Music titleBackgroundMusic;

    private RenderWindow window;
    private bool startButtonPressed;

    public Titlescreen()
    {
        VideoMode mode = new VideoMode(1920, 1080);
        string title = "No Time To Explain";
        window = new RenderWindow(mode, title, Styles.Fullscreen);
        AssetManager.Instance.LoadTexture(TextureName.StartButton, STARTBUTTON_TEXTURE_NAME);
        AssetManager.Instance.LoadTexture(TextureName.TitlescreenBackground, TITLESCREENBACKGROUND_TEXTURE_NAME);
        AssetManager.Instance.LoadMusic(MusicName.TitleBackground, TITLEBACKGROUND_MUSIC_NAME);
        startButtonPressed = false;

        titleBackgroundMusic = AssetManager.Instance.Music[MusicName.TitleBackground];
        titleBackgroundMusic.Volume *= 0.1f;

        startButtonSprite = new Sprite(new Texture(AssetManager.Instance.Textures[TextureName.StartButton]));
        startButtonSprite.Origin = new Vector2f(
            startButtonSprite.GetGlobalBounds().Left + startButtonSprite.GetGlobalBounds().Width / 2,
            startButtonSprite.GetGlobalBounds().Top + startButtonSprite.GetGlobalBounds().Height / 2
        );

        startButtonSprite.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 + 100);
        startButtonSprite.Scale *= 0.05f;

        background = new Sprite(new Texture(AssetManager.Instance.Textures[TextureName.TitlescreenBackground]));
        background.Scale *= 1.325f;

        window.MouseButtonPressed += OnMouseButtonPressed;
    }

    public void Run()
    {
        titleBackgroundMusic.Play();
        window.SetFramerateLimit(144);
        while (window.IsOpen && !startButtonPressed)
        {
            window.DispatchEvents();

            window.Clear(Color.Black);

            window.Draw(background);
            window.Draw(startButtonSprite);

            window.Display();
        }

        if (startButtonPressed)
        {
            RunGame();
        }
    }

    private void RunGame()
    {
        titleBackgroundMusic.Stop();
        Game game = new Game(window);
        game.Run();
    }

    private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
    {
        if (startButtonSprite.GetGlobalBounds().Contains(e.X, e.Y))
        {
            startButtonPressed = true;
        }
    }

    public void Dispose()
    {
        window.Dispose();
        startButtonSprite.Dispose();
        background.Dispose();
        titleBackgroundMusic.Dispose();
    }
}