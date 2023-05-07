using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Player : GameObject
{
    private Sprite player;
    private int[] frameCountPerAnimation;
    private float time = 0;
    private float movementSpeed = 30;
    private const int PLAYER_TILING_X = 9;
    private const int PLAYER_TILING_Y = 3;
    private RenderWindow renderWindow;
    private int animationFrame;
    private float animationTime = 0;
    private float animationSpeed = 10;
    private AnimationType currentAnimation = AnimationType.Idle;
    private int spriteXOffset;
    private int spriteYOffset;

    public override void Draw(RenderWindow window)
    {
        window.Draw(player);   
    }

    public override void Initialize()
    {
        frameCountPerAnimation = new int[3];
        frameCountPerAnimation[(int)AnimationType.Idle] = 6;
        frameCountPerAnimation[(int)AnimationType.Move] = 8;
        frameCountPerAnimation[(int)AnimationType.Death] = 9;

        player = new Sprite(AssetManager.Instance.Textures["player"]);
        player.TextureRect = new IntRect(
            0,
            0,
            (int)(player.Texture.Size.X / PLAYER_TILING_X),
            (int)(player.Texture.Size.Y / PLAYER_TILING_Y)
        );
        player.Origin = new Vector2f(player.Position.X + player.GetGlobalBounds().Width / 2, player.Position.Y + player.GetGlobalBounds().Height);
        player.Position = new Vector2f(renderWindow.Size.X / 2, renderWindow.Size.Y / 2);
        player.Scale *= 2f;
    }

    public override void Update(float deltaTime)
    {
        animationTime += deltaTime * animationSpeed;   

        Input_AnimationHandling(deltaTime);
    }

    private void Input_AnimationHandling(float deltaTime)
    {
        if (InputManager.Instance.GetKeyPressed(Keyboard.Key.D))
        {
            currentAnimation = AnimationType.Move;
            //MOVEMENT
        }
        else if(InputManager.Instance.GetKeyPressed(Keyboard.Key.A))
        {
            currentAnimation = AnimationType.Move; // MIRROR MOVEMENT
            //MOVEMENT
        }
        else if(InputManager.Instance.GetKeyPressed(Keyboard.Key.W))
        {
            currentAnimation = AnimationType.Move;
            //MOVEMENT
        }
        else if(InputManager.Instance.GetKeyPressed(Keyboard.Key.S))
        {
            currentAnimation = AnimationType.Move;
            //MOVEMENT
        }

        if(InputManager.Instance.GetKeyUp(Keyboard.Key.D))
        {
            currentAnimation = AnimationType.Idle;
        }
        else if(InputManager.Instance.GetKeyUp(Keyboard.Key.A))
        {
            currentAnimation = AnimationType.Idle;
        }
        else if(InputManager.Instance.GetKeyUp(Keyboard.Key.W))
        {
            currentAnimation = AnimationType.Idle;
        }
        else if(InputManager.Instance.GetKeyUp(Keyboard.Key.S))
        {
            currentAnimation = AnimationType.Idle;
        }

        animationFrame = (int)(animationTime % frameCountPerAnimation[(int)currentAnimation]);
        
        spriteXOffset = animationFrame * player.TextureRect.Width;
        spriteYOffset = (int)currentAnimation * player.TextureRect.Height;
            
        player.TextureRect = new IntRect(
            spriteXOffset,
            spriteYOffset,
            player.TextureRect.Width,
            player.TextureRect.Height
        );   
    }
    public Player(RenderWindow renderWindow)
    {
        this.renderWindow = renderWindow;
    }
}