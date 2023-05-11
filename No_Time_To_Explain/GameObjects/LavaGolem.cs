using SFML.Graphics;
using SFML.System;

public class LavaGolem : GameObject
{
    private Sprite lavaGolem;
    private const int LAVAGOLEM_TILING_X = 4;
    private const int LAVAGOLEM_TILING_Y = 3;
    private int animationFrame;
    private float animationTime = 0;
    private float animationSpeed = 5;
    private LavaGolemAnimationType currentAnimation = LavaGolemAnimationType.Idle;
    private int spriteXOffset;
    private int spriteYOffset;
    private Direction currDirection = Direction.Right;
    private const float LAVAGOLEM_SCALING = 1.5f;
    private float generalTime = 0;
    private float movementLength = 191.5f;
    private Vector2f pos;
    private bool alreadyIdle = true;

    public LavaGolem(Vector2f position)
    {
        pos = position;
    }

    public override void Draw(RenderWindow window)
    {
        window.Draw(lavaGolem);
    }

    public override void Initialize()
    {
        lavaGolem = new Sprite(AssetManager.Instance.Textures["lavaGolem"]);
        lavaGolem.Position = new Vector2f(0, 0);
        
        currDirection = Direction.Right;

        lavaGolem.TextureRect = new IntRect(
            0,
            0,
            (int)(lavaGolem.Texture.Size.X / LAVAGOLEM_TILING_X),
            (int)(lavaGolem.Texture.Size.Y / LAVAGOLEM_TILING_Y)
        );

        lavaGolem.Origin = new Vector2f(lavaGolem.GetGlobalBounds().Left + lavaGolem.GetGlobalBounds().Width / 2, lavaGolem.GetGlobalBounds().Top + lavaGolem.GetGlobalBounds().Height / 2);
        lavaGolem.Position = pos;
        lavaGolem.Scale *= LAVAGOLEM_SCALING;
    }

    public override void Update(float deltaTime)
    {
        animationTime += deltaTime * animationSpeed;
        Input_AnimationHandling(deltaTime);
    }

    private void Input_AnimationHandling(float deltaTime)
    {
        if(TurnHandler.Instance.IsPlayerTurn())
        {
            if(!alreadyIdle)
            {
                SwitchDirection();
                currentAnimation = LavaGolemAnimationType.Idle;
                if(currDirection == Direction.Left)
                {
                    lavaGolem.Scale = new Vector2f(-LAVAGOLEM_SCALING, LAVAGOLEM_SCALING);
                }
                else
                {
                    lavaGolem.Scale = new Vector2f(LAVAGOLEM_SCALING, LAVAGOLEM_SCALING);
                }
                alreadyIdle = true;
            }
            generalTime = 0;
            //if already idle then do nothing
        }
        else
        {
            //if it's the enemy turn, move
            if(generalTime < 0.25f)
            {
                generalTime += deltaTime;
                currentAnimation = LavaGolemAnimationType.Move;
                if(currDirection == Direction.Right)
                {
                    lavaGolem.Position += new Vector2f(1, 0) * movementLength * deltaTime;
                }
                else
                {
                    lavaGolem.Position -= new Vector2f(1, 0) * movementLength * deltaTime;
                }   
            }
            else
            {
                //if movement is done, set the turn to player again and be able to get idled next frame
                TurnHandler.Instance.PlayerTurn();
                alreadyIdle = false;
            }
            
        }

        animationFrame = (int)(animationTime % LAVAGOLEM_TILING_X);

        spriteXOffset = animationFrame * lavaGolem.TextureRect.Width;
        spriteYOffset = (int)currentAnimation * lavaGolem.TextureRect.Height;

        lavaGolem.TextureRect = new IntRect(
            spriteXOffset,
            spriteYOffset,
            lavaGolem.TextureRect.Width,
            lavaGolem.TextureRect.Height
        );
    }

    private void SwitchDirection()
    {
        if(currDirection == Direction.Right)
        {
            currDirection = Direction.Left;
        }
        else
        {
            currDirection = Direction.Right;
        }
    }
}