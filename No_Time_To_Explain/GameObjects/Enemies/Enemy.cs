using SFML.Graphics;
using SFML.System;

public class Enemy : GameObject
{
    protected Sprite? sprite;
    protected const int ENEMY_TILING_X = 6;
    protected const int ENEMY_TILING_Y = 3;
    protected int[] frameCountPerAnimation;
    protected int animationFrame;
    protected float animationTime = 0;
    protected float animationSpeed = 5;
    protected EnemyAnimationType currentAnimation = EnemyAnimationType.Idle;
    protected int spriteXOffset;
    protected int spriteYOffset;
    protected Direction currDirection = Direction.Right;
    protected const float ENEMY_SCALING = 1.5f;
    protected float generalTime = 0;
    protected float movementLength = 191.5f;
    protected Vector2f pos;
    protected bool alreadyIdle = true;
    protected EnemyType enemyType;
    protected string spriteName;
    public bool hasTurn = false;

    public Enemy(Vector2f position, EnemyType enemyType, string spriteName)
    {
        this.enemyType = enemyType;
        this.spriteName = spriteName;
        pos = position;
    }

    public override void Draw(RenderWindow window)
    {
        window.Draw(sprite);
    }

    public override void Initialize()
    {
        sprite = new Sprite(AssetManager.Instance.Textures[spriteName]);
        sprite.Position = new Vector2f(0, 0);
        
        currDirection = Direction.Right;

        frameCountPerAnimation = new int[3];
        frameCountPerAnimation[(int)EnemyAnimationType.Idle] = 4; 
        frameCountPerAnimation[(int)EnemyAnimationType.Move] = 6; 
        frameCountPerAnimation[(int)EnemyAnimationType.Death] = 6;

        sprite.TextureRect = new IntRect(
            0,
            0,
            (int)(sprite.Texture.Size.X / ENEMY_TILING_X),
            (int)(sprite.Texture.Size.Y / ENEMY_TILING_Y)
        );

        sprite.Origin = new Vector2f(sprite.GetGlobalBounds().Left + sprite.GetGlobalBounds().Width / 2, sprite.GetGlobalBounds().Top + sprite.GetGlobalBounds().Height / 2);
        sprite.Position = pos;
        sprite.Scale *= ENEMY_SCALING;
    }

    public override void Update(float deltaTime)
    {
        animationTime += deltaTime * animationSpeed;
        Input_AnimationHandling(deltaTime);
    }

    protected void Input_AnimationHandling(float deltaTime)
    {
        if(TurnHandler.Instance.IsPlayerTurn())
        {
            if(!alreadyIdle)
            {
                SwitchDirection();
                currentAnimation = EnemyAnimationType.Idle;
                if(currDirection == Direction.Left)
                {
                    sprite.Scale = new Vector2f(-ENEMY_SCALING, ENEMY_SCALING);
                }
                else
                {
                    sprite.Scale = new Vector2f(ENEMY_SCALING, ENEMY_SCALING);
                }
                alreadyIdle = true;
            }
            generalTime = 0;
            //if already idle then do nothing
        }
        else
        {
            //if it's the enemy turn, move
            EnemyMovement(deltaTime);
            EnemyAttack();
        }

        animationFrame = (int)(animationTime % frameCountPerAnimation[(int)currentAnimation]);

        spriteXOffset = animationFrame * sprite.TextureRect.Width;
        spriteYOffset = (int)currentAnimation * sprite.TextureRect.Height;

        sprite.TextureRect = new IntRect(
            spriteXOffset,
            spriteYOffset,
            sprite.TextureRect.Width,
            sprite.TextureRect.Height
        );
    }

    protected virtual void EnemyAttack()
    {
        Console.WriteLine("One");
    }

    protected void EnemyMovement(float deltaTime)
    {
        if(generalTime < 0.25f)
        {
            generalTime += deltaTime;
            currentAnimation = EnemyAnimationType.Move;
            if(currDirection == Direction.Right)
            {
                sprite.Position += new Vector2f(1, 0) * movementLength * deltaTime;
            }
            else
            {
                sprite.Position -= new Vector2f(1, 0) * movementLength * deltaTime;
            }  
            hasTurn = true; 
        }
        else
        {
            //if movement is done, set the turn to player again and be able to get idled next frame
            hasTurn = false;
            alreadyIdle = false;
        }
    }

    protected void SwitchDirection()
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