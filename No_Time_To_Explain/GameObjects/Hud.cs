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
    private Text soulsText;
    private Text scoreText;
    private Sprite retryButton;
    private Sprite leaveButton;

    private const int MAX_TIME = 120;
    private float remainingTime = 0;
    private int currentSouls = 0;
    private bool playerDeath = false;
    public bool Retry { get; private set; } = false;
    private int score = 0;
    private bool reachedTeleporter = false;
    private int scoreCount = 0;
    public bool FinishedScoreAdding = false;

    public int CurrentSouls
    {
        get 
        { 
            return currentSouls; 
        }
        set
        {
            currentSouls = value;
        }
    }

    public Hud(RenderWindow renderWindow)
    {
        this.renderWindow = renderWindow;
    }

    public override void Draw(RenderWindow window)
    {
        if (!playerDeath && !reachedTeleporter)
        {
            window.Draw(time);
            window.Draw(souls);
        }
        else if(playerDeath)
        {
            window.Draw(deathText);
            window.Draw(soulsText);
            window.Draw(retryButton);
            window.Draw(leaveButton);
            window.Draw(scoreText);
        } else if(reachedTeleporter)
        {
            window.Draw(scoreText);
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

        currentSouls = 0;

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

        soulsText = new Text($"Your Souls this level: {currentSouls}", font, 20);
        soulsText.FillColor = Color.Red;
        soulsText.Origin = new Vector2f(
            soulsText.GetGlobalBounds().Left + soulsText.GetGlobalBounds().Width / 2,
            soulsText.GetGlobalBounds().Top + soulsText.GetGlobalBounds().Height / 2
        );
        soulsText.Position = new Vector2f(0, 200);

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

        scoreText = new Text($"Your Score: 0", font, 20);
        scoreText.FillColor = Color.Blue;
        scoreText.Origin = new Vector2f(
            scoreText.GetGlobalBounds().Left + scoreText.GetGlobalBounds().Width / 2,
            scoreText.GetGlobalBounds().Top + scoreText.GetGlobalBounds().Height / 2
        );
        scoreText.Position = new Vector2f(0, 250);

        renderWindow.MouseButtonPressed += OnMouseButtonPressed;        
    }

    public override void Update(float deltaTime)
    {
        remainingTime += deltaTime;

        if (MAX_TIME - (int)remainingTime >= 0)
        {
            time.DisplayedString = $"Time's ticking: {MAX_TIME - (int)remainingTime}";
        }
        
        if(playerDeath)
        {
            scoreText.DisplayedString = $"Your Score: {score}";
        }
        else
        {
            scoreText.DisplayedString = $"Your Score: {scoreCount}";
        }

        if(reachedTeleporter)
        {
            ScoreUpCountAnimation();
        }
        else
        {
            scoreCount = 0;
        }
    }

    private void ScoreUpCountAnimation()
    {
        if(scoreCount < score)
        {
            scoreCount++;
        }
        else
        {
            FinishedScoreAdding = true;
        }
        Console.WriteLine($"Your Score: {scoreCount}");
        scoreText.DisplayedString = $"Your Score: {scoreCount}";
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
        soulsText.DisplayedString = $"Your Souls this level: {currentSouls}";
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

    public void ReachedTeleporter()
    {
        reachedTeleporter = true;
        score += currentSouls * 5;
    }

    public void Reset()
    {
        reachedTeleporter = false;
        playerDeath = false;
        currentSouls = 0;
        remainingTime = 0;
        FinishedScoreAdding = false;
    }
}
