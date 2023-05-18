using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Player : GameObject
{
    private const float MOVE_TIME = 0.25f;
    private Sprite player;
    private int[] frameCountPerAnimation;
    private float movementLength = 185f;
    private const int PLAYER_TILING_X = 9;
    private const int PLAYER_TILING_Y = 5;
    private RenderWindow renderWindow;
    private int animationFrame;
    private float animationTime = 0;
    private float animationSpeed = 5;
    private PlayerAnimationType currentAnimation = PlayerAnimationType.Idle;
    private int spriteXOffset;
    private int spriteYOffset;
    private bool isMoving;
    private float generalTime = MOVE_TIME + 1; //Has to be greater than MOVE_TIME, because we want to be able to move as soon as we load in
    private Direction currDirection;
    private const float PLAYER_SCALING = 2f;
    public Vector2i tileIndex;
    private Room currentRoom;
    private Vector2f targetPos;
    private Sound obstacleHit;

    public override void Draw(RenderWindow window)
    {
        window.Draw(player);   
    }

    public override void Initialize()
    {
        currDirection = Direction.Right;
        isMoving = false;
        
        obstacleHit = new Sound(AssetManager.Instance.Sounds["obstacleHit"]);
        obstacleHit.Pitch = 2.5f;
        obstacleHit.Volume = 5;
        frameCountPerAnimation = new int[5];
        frameCountPerAnimation[(int)PlayerAnimationType.Idle] = 6; //0
        frameCountPerAnimation[(int)PlayerAnimationType.Move] = 4; //1
        frameCountPerAnimation[(int)PlayerAnimationType.Death] = 9; //3

        player = new Sprite(AssetManager.Instance.Textures["player"]);
        
        player.TextureRect = new IntRect(
            0,
            0,
            (int)(player.Texture.Size.X / PLAYER_TILING_X),
            (int)(player.Texture.Size.Y / PLAYER_TILING_Y)
        );

        player.Origin = new Vector2f(player.GetGlobalBounds().Left + player.GetGlobalBounds().Width / 2 + 3, player.GetGlobalBounds().Top + player.GetGlobalBounds().Height - 4);
        player.Position = currentRoom.SpawnTile.Position + new Vector2f(currentRoom.TileSize / 2, currentRoom.TileSize / 2);
        player.Scale *= PLAYER_SCALING;
        tileIndex = Utils.ConvertToIndex(renderWindow, player.Position, player);
    }

    public override void Update(float deltaTime)
    {
        animationTime += deltaTime * animationSpeed;   

        Input_Handling(deltaTime);
    }

    private void Input_Handling(float deltaTime)
    {
        generalTime += deltaTime;
        //Console.WriteLine("Current Position:" + player.Position);
            
        //Console.WriteLine("Current Index:" + tileIndex);
        if (TurnHandler.Instance.IsPlayerTurn())
        {
            if (InputManager.Instance.GetKeyDown(Keyboard.Key.D) && generalTime > MOVE_TIME)
            {
                if(tileIndex.X + 1 < currentRoom.Map[tileIndex.Y].Length)
                {
                    if (Utils.IsObstacle(tileIndex + new Vector2i(1, 0), currentRoom.Map))
                    {
                        obstacleHit.Play();
                        Console.WriteLine("Is Obstacle: Yes");
                        Movement_AnimationHandling(deltaTime);
                    }
                    else
                    {
                        Console.WriteLine("Is Obstalce: No");
                        currentAnimation = PlayerAnimationType.Move;
                        isMoving = true;
                        generalTime = 0;
                        currDirection = Direction.Right;
                        player.Scale = new Vector2f(PLAYER_SCALING, PLAYER_SCALING);
                    }
                }
                else
                {
                    obstacleHit.Play();
                }
                
            }
            else if (InputManager.Instance.GetKeyDown(Keyboard.Key.A) && generalTime > MOVE_TIME)
            {
                if(tileIndex.X - 1 >= 0)
                {
                    if (Utils.IsObstacle(tileIndex - new Vector2i(1, 0), currentRoom.Map))
                    {
                        obstacleHit.Play();
                        Console.WriteLine("Is Obstacle: Yes");
                        Movement_AnimationHandling(deltaTime);
                    }
                    else
                    {
                        Console.WriteLine("Is Obstalce: No");
                        currentAnimation = PlayerAnimationType.Move;
                        isMoving = true;
                        generalTime = 0;
                        currDirection = Direction.Left;
                        player.Scale = new Vector2f(-PLAYER_SCALING, PLAYER_SCALING);
                    }
                }
                else
                {
                    obstacleHit.Play();
                }
                
            }
            else if (InputManager.Instance.GetKeyDown(Keyboard.Key.W) && generalTime > MOVE_TIME)
            {
                if(tileIndex.Y - 1 >= 0)
                {
                    if (Utils.IsObstacle(tileIndex - new Vector2i(0, 1), currentRoom.Map))
                    {
                        obstacleHit.Play();
                        Console.WriteLine("Is Obstacle: Yes");
                        Movement_AnimationHandling(deltaTime);
                    }
                    else
                    {
                        Console.WriteLine("Is Obstalce: No");
                        currentAnimation = PlayerAnimationType.Move;
                        isMoving = true;
                        generalTime = 0;
                        currDirection = Direction.Up;
                    }
                }
                else
                {
                    obstacleHit.Play();
                }
                
            }
            else if (InputManager.Instance.GetKeyDown(Keyboard.Key.S) && generalTime > MOVE_TIME)
            {
                if(tileIndex.Y + 1 < currentRoom.Map.Count)
                {
                    if (Utils.IsObstacle(tileIndex + new Vector2i(0, 1), currentRoom.Map))
                    {
                        obstacleHit.Play();
                        Console.WriteLine("Is Obstacle: Yes");
                        Movement_AnimationHandling(deltaTime);
                    }
                    else
                    {
                        Console.WriteLine("Is Obstalce: No");
                        currentAnimation = PlayerAnimationType.Move;
                        isMoving = true;
                        generalTime = 0;
                        currDirection = Direction.Down;
                    }
                }
                else
                {
                    obstacleHit.Play();
                }
            }
            Movement_AnimationHandling(deltaTime);
        }
    }


    private void Movement_AnimationHandling(float deltaTime)
    {
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
            if(generalTime > MOVE_TIME)
            {
                isMoving = false;
                currentAnimation = PlayerAnimationType.Idle;
                tileIndex = Utils.ConvertToIndex(renderWindow, player.Position, player);
                TurnHandler.Instance.EnemyTurn();
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

    public void UpdateWindow(RenderWindow window, float scalingFactorX, float scalingFactorY)
    {
        renderWindow = window;
    }

    public void SetCurrentRoom(Room room)
    {
        currentRoom = room;
    }
}