/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class UI_Handler : GameObject
{
    // Constants
    private const int MAX_TIME = 120;
    private const float DISPLAY_SOULS_THRESHOLD = 1f;
    private const int SOULS_SCORE_MODIFIER = 10;
    private const int TIME_SCORE_MODIFIER = 3;
    private const string HUD_FONT_NAME = "BrunoAce-Regular.ttf";
    private const string DEATH_FONT_NAME = "NightmarePills-BV2w.ttf";
    private const string RETRY_TEXTURE_NAME = "retryButton.png";
    private const string LEAVE_TEXTURE_NAME = "leaveButton.png";
    private const float RETRYBUTTON_SCALE_MULTIPLIER = 0.25f;
    private const float LEAVEBUTTON_SCALE_MULTIPLIER = 0.01f;
    private const int SMALL_FONT_SIZE = 12;
    private const int MIDDLE_FONT_SIZE = 20;
    private const int BIG_FONT_SIZE = 30;

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

    public UI_Handler(RenderWindow renderWindow)
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
        AssetManager.Instance.LoadFont(FontName.Hud, HUD_FONT_NAME);
        AssetManager.Instance.LoadFont(FontName.Death, DEATH_FONT_NAME);
        AssetManager.Instance.LoadTexture(TextureName.Retry, RETRY_TEXTURE_NAME);
        AssetManager.Instance.LoadTexture(TextureName.Leave, LEAVE_TEXTURE_NAME);

        font = AssetManager.Instance.Fonts[FontName.Hud];
        deathFont = AssetManager.Instance.Fonts[FontName.Death];

        currentSouls = 0;

        time = TextCreator($"Time's ticking: {MAX_TIME - (int)remainingTime}", font, SMALL_FONT_SIZE, Color.White, new Vector2f(renderWindow.Size.X / 2 - 75.5f, -renderWindow.Size.Y / 2 + 15));
        souls = TextCreator($"Souls harvested: {currentSouls}", font, SMALL_FONT_SIZE, Color.White, new Vector2f(-renderWindow.Size.X / 2 + 85, -renderWindow.Size.Y / 2 + 15));
        deathText = TextCreator("YOUR SOUL HAS VANQUISHED FROM EXISTENCE", deathFont, BIG_FONT_SIZE, Color.Red, new Vector2f(0, -100));
        soulsDeathText = TextCreator($"Your Souls this level: {currentSouls}", font, MIDDLE_FONT_SIZE, Color.Red, new Vector2f(0, 200));
        scoreText = TextCreator($"Your Score: 0", font, MIDDLE_FONT_SIZE, Color.Magenta, new Vector2f(0, 250));
        nextLevelTimeText = TextCreator($"+Time", font, MIDDLE_FONT_SIZE, Color.Magenta, new Vector2f(scoreText.GetGlobalBounds().Left + scoreText.GetGlobalBounds().Width + 100, 250));
        nextLevelSoulsText = TextCreator($"+Souls", font, MIDDLE_FONT_SIZE, Color.Magenta, new Vector2f(scoreText.GetGlobalBounds().Left + scoreText.GetGlobalBounds().Width + 100, 250));

        retryButton = ButtonCreator(AssetManager.Instance.Textures[TextureName.Retry], new Vector2f(0, 300), RETRYBUTTON_SCALE_MULTIPLIER);
        leaveButton = ButtonCreator(AssetManager.Instance.Textures[TextureName.Leave], new Vector2f(0, 350), LEAVEBUTTON_SCALE_MULTIPLIER);

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

    private Text TextCreator(string textString, Font usedFont, uint charSize, Color fillColor, Vector2f pos)
    {
        Text text = new Text(textString, usedFont, charSize);
        text.FillColor = fillColor;
        text.Origin = new Vector2f(
            text.GetGlobalBounds().Left + text.GetGlobalBounds().Width / 2,
            text.GetGlobalBounds().Top + text.GetGlobalBounds().Height / 2
        );
        text.Position = pos;
        return text;
    }

    private Sprite ButtonCreator(Texture texture, Vector2f pos, float scaleMultiplier)
    {
        Sprite button = new Sprite(texture);
        button.Origin = new Vector2f(
            button.GetGlobalBounds().Left + button.GetGlobalBounds().Width / 2,
            button.GetGlobalBounds().Top + button.GetGlobalBounds().Height / 2
        );
        button.Position = pos;
        button.Scale *= scaleMultiplier;

        return button;
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
