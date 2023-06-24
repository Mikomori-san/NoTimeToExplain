/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Hud : GameObject
{
    // Constants
    private const int MAX_TIME = 120;
    private const float DISPLAY_SOULS_THRESHOLD = 1f;
    private const int SOULS_SCORE_MODIFIER = 10;
    private const int TIME_SCORE_MODIFIER = 3;

    private RenderWindow renderWindow;

    public event Action RetryButtonPressed;

    // Fonts
    private Font font;
    private Font deathFont;

    // Texts
    private Text time;
    private Text souls;
    private Text deathText;
    private Text soulsDeathText;
    private Text nextLevelTimeText;
    private Text nextLevelSoulsText;
    private Text scoreText;

    // Buttons
    private Sprite retryButton;
    private Sprite leaveButton;

    // Game state variables
    private float remainingTime = 0;
    private int currentSouls = 0;
    public bool playerDeath{get; set;} = false;
    private bool reachedTeleporter = false;
    private bool displayTimeNextLevel = false;
    private bool displaySoulsNextLevel = false;
    public bool FinishedScoreAdding {get; private set; } = false;
    private float displayTimer = 0;
    private int score = 0;
    private int scoreCount = 0;

    public Hud(RenderWindow renderWindow)
    {
        this.renderWindow = renderWindow;
    }

    public override void Draw(RenderWindow window)
{
    if (!playerDeath && !reachedTeleporter)
    {
        DrawGameplayHUD(window);
    }
    else if (playerDeath)
    {
        DrawDeathScreen(window);
    }
    else if (reachedTeleporter)
    {
        DrawTeleporterScreen(window);
    }
}

    private void DrawGameplayHUD(RenderWindow window)
    {
        window.Draw(time);
        window.Draw(souls);
    }

    private void DrawDeathScreen(RenderWindow window)
    {
        window.Draw(deathText);
        window.Draw(soulsDeathText);
        window.Draw(retryButton);
        window.Draw(leaveButton);
        window.Draw(scoreText);
    }

    private void DrawTeleporterScreen(RenderWindow window)
    {
        window.Draw(scoreText);
        if (displayTimeNextLevel)
        {
            window.Draw(nextLevelTimeText);
        }
        else if (displaySoulsNextLevel)
        {
            window.Draw(nextLevelSoulsText);
        }
    }

    public override void Initialize()
    {
        AssetManager.Instance.LoadFont("hud", "BrunoAce-Regular.ttf");
        AssetManager.Instance.LoadFont("death", "NightmarePills-BV2w.ttf");
        AssetManager.Instance.LoadTexture("retry", "retryButton.png");
        AssetManager.Instance.LoadTexture("leave", "leaveButton.png");

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

        soulsDeathText = new Text($"Your Souls this level: {currentSouls}", font, 20);
        soulsDeathText.FillColor = Color.Red;
        soulsDeathText.Origin = new Vector2f(
            soulsDeathText.GetGlobalBounds().Left + soulsDeathText.GetGlobalBounds().Width / 2,
            soulsDeathText.GetGlobalBounds().Top + soulsDeathText.GetGlobalBounds().Height / 2
        );
        soulsDeathText.Position = new Vector2f(0, 200);

        retryButton = new Sprite(AssetManager.Instance.Textures["retry"]);
        retryButton.Origin = new Vector2f(
            retryButton.GetGlobalBounds().Left + retryButton.GetGlobalBounds().Width / 2,
            retryButton.GetGlobalBounds().Top + retryButton.GetGlobalBounds().Height / 2
        );
        retryButton.Position = new Vector2f(0, 300);
        retryButton.Scale *= 0.25f;

        leaveButton = new Sprite(AssetManager.Instance.Textures["leave"]);
        leaveButton.Origin = new Vector2f(
            leaveButton.GetGlobalBounds().Left + leaveButton.GetGlobalBounds().Width / 2,
            leaveButton.GetGlobalBounds().Top + leaveButton.GetGlobalBounds().Height / 2
        );
        leaveButton.Position = new Vector2f(0, 350);
        leaveButton.Scale *= 0.01f;

        scoreText = new Text($"Your Score: 0", font, 20);
        scoreText.FillColor = Color.Magenta;
        scoreText.Origin = new Vector2f(
            scoreText.GetGlobalBounds().Left + scoreText.GetGlobalBounds().Width / 2,
            scoreText.GetGlobalBounds().Top + scoreText.GetGlobalBounds().Height / 2
        );
        scoreText.Position = new Vector2f(0, 250);

        nextLevelTimeText = new Text($"+Time", font, 20);
        nextLevelTimeText.FillColor = Color.Magenta;
        nextLevelTimeText.Origin = new Vector2f(
            nextLevelTimeText.GetGlobalBounds().Left + nextLevelTimeText.GetGlobalBounds().Width / 2,
            nextLevelTimeText.GetGlobalBounds().Top + nextLevelTimeText.GetGlobalBounds().Height / 2
        );
        nextLevelTimeText.Position = new Vector2f(scoreText.GetGlobalBounds().Left + scoreText.GetGlobalBounds().Width + 100, 250);

        nextLevelSoulsText = new Text($"+Souls", font, 20);
        nextLevelSoulsText.FillColor = Color.Magenta;
        nextLevelSoulsText.Origin = new Vector2f(
            nextLevelSoulsText.GetGlobalBounds().Left + nextLevelSoulsText.GetGlobalBounds().Width / 2,
            nextLevelSoulsText.GetGlobalBounds().Top + nextLevelSoulsText.GetGlobalBounds().Height / 2
        );
        nextLevelSoulsText.Position = new Vector2f(scoreText.GetGlobalBounds().Left + scoreText.GetGlobalBounds().Width + 100, 250);

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
            ScoreUpCountAnimation(deltaTime);
        }
        else
        {
            scoreCount = score;
        }
    }

    private void ScoreUpCountAnimation(float deltaTime)
    {
        if(scoreCount < score)
        {
            if(scoreCount < score - currentSouls * SOULS_SCORE_MODIFIER) //while the scoreCount is lower than the score with timeScore, add up and display time
            {
                scoreCount++;
                displayTimeNextLevel = true;
            }
            else //then, take away time, and start countdown for soulsDisplay
            {
                displayTimeNextLevel = false;
                displayTimer += deltaTime;
                
                if(displayTimer >= DISPLAY_SOULS_THRESHOLD)
                {
                    displaySoulsNextLevel = true;
                    scoreCount++;
                }
            }
        }
        else
        {
            displayTimer = 0;
            displaySoulsNextLevel = false;
            FinishedScoreAdding = true;
        }
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
        soulsDeathText.DisplayedString = $"Your Souls this level: {currentSouls}";
    }

    internal void Add30Seconds()
    {
        remainingTime -= 30;
    }

    private void OnRetryButtonPressed()
    {
        RetryButtonPressed?.Invoke();
    }

    private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
    {
        if(playerDeath)
        {
            if (retryButton.GetGlobalBounds().Contains(e.X - renderWindow.Size.X / 2, e.Y - renderWindow.Size.Y / 2))
            {
                OnRetryButtonPressed();
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
        score += currentSouls * SOULS_SCORE_MODIFIER;
        score += (MAX_TIME - (int)remainingTime) * TIME_SCORE_MODIFIER;
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
