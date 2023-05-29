using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Hud : GameObject
{
    private RenderWindow renderWindow;
    private Font font;
    private Font deathFont;
    private Text time;
    private Text souls;
    private Text deathText;
    private Text scoreText;
    private Sprite retryButton;
    private Sprite leaveButton;

    private const int MAX_TIME = 120;
    private float remainingTime = 0;
    private int currentSouls = 0;
    private bool playerDeath = false;
    public bool Retry { get; private set; } = false;

    public int CurrentSouls
    {
        get 
        { 
            return currentSouls; 
        }
    }

    public Hud(RenderWindow renderWindow)
    {
        this.renderWindow = renderWindow;
    }

    public override void Draw(RenderWindow window)
    {
        if (!playerDeath)
        {
            window.Draw(time);
            window.Draw(souls);
        }
        else
        {
            window.Draw(deathText);
            window.Draw(scoreText);
            window.Draw(retryButton);
            window.Draw(leaveButton);
        }
    }

    public override void Initialize()
    {
        font = AssetManager.Instance.Fonts["hud"];
        deathFont = AssetManager.Instance.Fonts["death"];

        time = new Text($"Time's ticking: {MAX_TIME - (int)remainingTime}", font, 12);
        time.FillColor = Color.White;
        time.Position = new Vector2f(
            renderWindow.Size.X / 2 - (time.GetGlobalBounds().Left + time.GetGlobalBounds().Width + 10),
            -renderWindow.Size.Y / 2 + 10
        );

        souls = new Text($"Souls harvested: {currentSouls}", font, 12);
        souls.FillColor = Color.White;
        souls.Position = new Vector2f(-renderWindow.Size.X / 2 + 10, -renderWindow.Size.Y / 2 + 10);

        deathText = new Text("YOUR SOUL HAS VANQUISHED FROM EXISTENCE", deathFont, 30);
        deathText.FillColor = Color.Red;
        deathText.Origin = new Vector2f(
            deathText.GetGlobalBounds().Left + deathText.GetGlobalBounds().Width / 2,
            deathText.GetGlobalBounds().Top + deathText.GetGlobalBounds().Height / 2
        );
        deathText.Position = new Vector2f(0, -100);

        scoreText = new Text($"Your Score: {currentSouls}", font, 20);
        scoreText.FillColor = Color.Red;
        scoreText.Origin = new Vector2f(
            scoreText.GetGlobalBounds().Left + scoreText.GetGlobalBounds().Width / 2,
            scoreText.GetGlobalBounds().Top + scoreText.GetGlobalBounds().Height / 2
        );
        scoreText.Position = new Vector2f(0, 200);

        retryButton = new Sprite(AssetManager.Instance.Textures["startbutton"]);
        retryButton.Origin = new Vector2f(
            retryButton.GetGlobalBounds().Left + retryButton.GetGlobalBounds().Width / 2,
            retryButton.GetGlobalBounds().Top + retryButton.GetGlobalBounds().Height / 2
        );
        retryButton.Position = new Vector2f(0, 300);
        retryButton.Scale *= 0.01f;

        leaveButton = new Sprite(AssetManager.Instance.Textures["startbutton"]);
        leaveButton.Origin = new Vector2f(
            leaveButton.GetGlobalBounds().Left + leaveButton.GetGlobalBounds().Width / 2,
            leaveButton.GetGlobalBounds().Top + leaveButton.GetGlobalBounds().Height / 2
        );
        leaveButton.Position = new Vector2f(0, 350);
        leaveButton.Scale *= 0.01f;

        renderWindow.MouseButtonPressed += OnMouseButtonPressed;        
    }

    public override void Update(float deltaTime)
    {
        remainingTime += deltaTime;

        if (MAX_TIME - (int)remainingTime >= 0)
        {
            time.DisplayedString = $"Time's ticking: {MAX_TIME - (int)remainingTime}";
        }
    }

    internal int RemainingTime()
    {
        return MAX_TIME - (int)remainingTime;
    }

    internal void AddSoul()
    {
        currentSouls++;
        souls.DisplayedString = $"Souls harvested: {currentSouls}";
    }

    internal void DisplayDeathText()
    {
        playerDeath = true;
        scoreText.DisplayedString = $"Your Score: {currentSouls}";
    }

    internal void Add30Seconds()
    {
        remainingTime -= 30;
    }

    private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
    {
        if(playerDeath)
        {
            if (retryButton.GetGlobalBounds().Contains(e.X - renderWindow.Size.X / 2, e.Y - renderWindow.Size.Y / 2))
            {
                Retry = true;
            }
            if(leaveButton.GetGlobalBounds().Contains(e.X - renderWindow.Size.X / 2, e.Y - renderWindow.Size.Y / 2))
            {
                renderWindow.Close();
            }
        }
    }
}
