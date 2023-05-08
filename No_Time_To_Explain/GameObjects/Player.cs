using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Player : GameObject
{
    private const float MOVE_TIME = 0.25f;
    private Sprite player;
    private int[] frameCountPerAnimation;
    private float time = 0;
    private float movementLength = 300;
    private const int PLAYER_TILING_X = 9;
    private const int PLAYER_TILING_Y = 5;
    private RenderWindow renderWindow;
    private int animationFrame;
    private float animationTime = 0;
    private float animationSpeed = 5;
    private AnimationType currentAnimation = AnimationType.Idle;
    private int spriteXOffset;
    private int spriteYOffset;
    private bool isMoving;
    private float moveTime = MOVE_TIME + 1;
    private Direction currDirection;
    private const float PLAYER_SCALING = 4f;

    public override void Draw(RenderWindow window)
    {
        window.Draw(player);   
    }

    public override void Initialize()
    {
        currDirection = Direction.Right;
        isMoving = false;

        frameCountPerAnimation = new int[5];
        frameCountPerAnimation[(int)AnimationType.Idle] = 6; //0
        frameCountPerAnimation[(int)AnimationType.Move] = 4; //1 | 8 is max, but only need half
        frameCountPerAnimation[(int)AnimationType.Death] = 9; //3

        player = new Sprite(AssetManager.Instance.Textures["player"]);
        
        player.TextureRect = new IntRect(
            0,
            0,
            (int)(player.Texture.Size.X / PLAYER_TILING_X),
            (int)(player.Texture.Size.Y / PLAYER_TILING_Y)
        );

        player.Origin = new Vector2f(player.Position.X + player.GetGlobalBounds().Width / 2, player.Position.Y + player.GetGlobalBounds().Height);
        player.Position = new Vector2f(0, 0);
        player.Scale *= PLAYER_SCALING;
    }

    public override void Update(float deltaTime)
    {
        animationTime += deltaTime * animationSpeed;   

        Input_AnimationHandling(deltaTime);
    }

    private void Input_AnimationHandling(float deltaTime)
    {
        moveTime += deltaTime;
        if(InputManager.Instance.GetKeyDown(Keyboard.Key.D) && moveTime > MOVE_TIME)
        {
            currentAnimation = AnimationType.Move;
            isMoving = true;
            moveTime = 0;
            currDirection = Direction.Right;
            player.Scale = new Vector2f(PLAYER_SCALING, PLAYER_SCALING);
        }
        else if(InputManager.Instance.GetKeyDown(Keyboard.Key.A) && moveTime > MOVE_TIME)
        {
            currentAnimation = AnimationType.Move;
            isMoving = true;
            moveTime = 0;
            currDirection = Direction.Left;
            player.Scale = new Vector2f(-PLAYER_SCALING, PLAYER_SCALING);
        }
        else if(InputManager.Instance.GetKeyDown(Keyboard.Key.W) && moveTime > MOVE_TIME)
        {
            currentAnimation = AnimationType.Move;
            isMoving = true;
            moveTime = 0;
            currDirection = Direction.Up;
        }
        else if(InputManager.Instance.GetKeyDown(Keyboard.Key.S) && moveTime > MOVE_TIME)
        {
            currentAnimation = AnimationType.Move;
            isMoving = true;
            moveTime = 0;
            currDirection = Direction.Down;
        }

        if(isMoving)
        {
            switch(currDirection)
            {
                case Direction.Right:
                    player.Position += new Vector2f(1, 0) * deltaTime * movementLength;
                break;
                case Direction.Left:
                    player.Position -= new Vector2f(1, 0) * deltaTime * movementLength;
                break;
                case Direction.Down:
                    player.Position += new Vector2f(0, 1) * deltaTime * movementLength;
                break;
                case Direction.Up:
                    player.Position -= new Vector2f(0, 1) * deltaTime * movementLength;
                break;
            }

            if(moveTime > MOVE_TIME)
            {
                isMoving = false;
                currentAnimation = AnimationType.Idle;
            }
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