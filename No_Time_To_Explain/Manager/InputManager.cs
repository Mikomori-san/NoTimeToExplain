using SFML.Graphics;
using SFML.Window;

public class InputManager
{
    private static InputManager? instance;

    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InputManager();
            }

            return instance;
        }
    }

    private Dictionary<Keyboard.Key, bool> pressedKeys = new Dictionary<Keyboard.Key, bool>();
    private Dictionary<Keyboard.Key, bool> keysUpThisFrame = new Dictionary<Keyboard.Key, bool>();
    private Dictionary<Keyboard.Key, bool> keysDownThisFrame = new Dictionary<Keyboard.Key, bool>();

    private InputManager() { }

    public void Initialize(RenderWindow window)
    {
        window.KeyPressed += OnKeyPressed;
        window.KeyReleased += OnKeyUp;

        pressedKeys[Keyboard.Key.Space] = false;
        pressedKeys[Keyboard.Key.A] = false;
        pressedKeys[Keyboard.Key.W] = false;
        pressedKeys[Keyboard.Key.S] = false;
        pressedKeys[Keyboard.Key.D] = false;
        pressedKeys[Keyboard.Key.Num1] = false;
        pressedKeys[Keyboard.Key.Num2] = false;

        foreach (var key in pressedKeys.Keys)
        {
            keysUpThisFrame[key] = false;
            keysDownThisFrame[key] = false;
        }
    }

    public void Update(float deltaTime)
    {
        foreach (var keyState in keysUpThisFrame)
        {
            keysUpThisFrame[keyState.Key] = false;
        }
        foreach (var keyState in keysDownThisFrame)
        {
            keysDownThisFrame[keyState.Key] = false;
        }
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (pressedKeys.ContainsKey(e.Code))
        {
            if (pressedKeys[e.Code])
            {
                keysUpThisFrame[e.Code] = true;
                pressedKeys[e.Code] = false;
            }
        }
    }

    private void OnKeyPressed(object? sender, KeyEventArgs e)
    {
        if (pressedKeys.ContainsKey(e.Code))
        {
            if (!pressedKeys[e.Code])
            {
                keysDownThisFrame[e.Code] = true;
                pressedKeys[e.Code] = true;
            }
        }
    }

    public bool GetKeyPressed(Keyboard.Key key)
    {
        return pressedKeys[key];
    }

    public bool GetKeyDown(Keyboard.Key key)
    {
        return keysDownThisFrame[key];
    }

    public bool GetKeyUp(Keyboard.Key key)
    {
        return keysUpThisFrame[key];
    }
}