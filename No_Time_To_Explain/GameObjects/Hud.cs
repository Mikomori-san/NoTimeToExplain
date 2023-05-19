using SFML.Graphics;
using SFML.System;

public class Hud : GameObject
{
    private RenderWindow renderWindow;
    private Font font;
    private Text time;
    private Text souls;
    private const int MAX_TIME = 120;
    private float remainingTime = 0;
    private int currentSouls = 0;

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
        window.Draw(time);
        window.Draw(souls);
    }

    public override void Initialize()
    {
        font = AssetManager.Instance.Fonts["hud"];
        time = new Text($"Time's ticking: {MAX_TIME - (int)remainingTime}", font, 12);
        time.FillColor = Color.White;
        time.Position = new Vector2f(renderWindow.Size.X / 2 - (time.GetGlobalBounds().Left + time.GetGlobalBounds().Width + 10), -renderWindow.Size.Y / 2 + 10);
        souls = new Text($"Souls harvested: {currentSouls}", font, 12);
        souls.FillColor = Color.White;
        souls.Position = new Vector2f(-renderWindow.Size.X / 2 + 10, -renderWindow.Size.Y / 2 + 10);
    }

    public override void Update(float deltaTime)
    {
        remainingTime += deltaTime;
        
        if(MAX_TIME - (int)remainingTime >= 0)
            time.DisplayedString = $"Time's ticking: {MAX_TIME - (int)remainingTime}";
    }

    public int RemainingTime()
    {
        return MAX_TIME - (int)remainingTime;
    }

    public void AddSoul()
    {
        currentSouls++;
        souls.DisplayedString = $"Souls harvested: {currentSouls}";
    }

}