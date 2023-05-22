using SFML.Graphics;
using SFML.System;

public class Enemy : GameObject
{
    protected Sprite sprite;
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
    protected bool alreadyIdle = true;
    protected EnemyType enemyType;
    protected string spriteName;
    public bool hasTurn = false;
    public Vector2i tileIndex;
    public RenderWindow window;
    protected Vector2f SpawnPosition;
    public bool soulHarvested = false;
    protected float soulHarvestCooldownTimer = 0;
    protected const float SOUL_HARVEST_COOLDOWN = 0.5f;
    protected Vector2i playerIndex;
    protected Room currentRoom;
    protected int tilesToGoal = 0;
    protected BreadthFirstSearch bds;
    protected bool pathFound = false;
    protected Vector2i? blockedEnemyTileIndex = null;
    protected bool posUpdated = false;

    public Enemy(EnemyType enemyType, string spriteName, RenderWindow window, Vector2i playerIndex, Room currentRoom)
    {
        this.window = window;
        this.enemyType = enemyType;
        this.spriteName = spriteName;
        this.playerIndex = playerIndex;
        this.currentRoom = currentRoom;
    }

    public override void Draw(RenderWindow window)
    {
        window.Draw(sprite);
    }

    public override void Initialize()
    {
        tileIndex = Utils.ConvertToIndex(window, sprite.Position, sprite);
        currDirection = Direction.Right;
        frameCountPerAnimation = new int[3];
        frameCountPerAnimation[(int)EnemyAnimationType.Idle] = 4; 
        frameCountPerAnimation[(int)EnemyAnimationType.Move] = 6; 
        frameCountPerAnimation[(int)EnemyAnimationType.Death] = 6;

    }

    public override void Update(float deltaTime)
    {
        animationTime += deltaTime * animationSpeed;
        soulHarvestCooldownTimer += deltaTime;
        if(soulHarvestCooldownTimer >= SOUL_HARVEST_COOLDOWN)
        {
            soulHarvested = false;
        }
        Input_AnimationHandling(deltaTime);
    }

    protected void Input_AnimationHandling(float deltaTime)
    {
        
        if(TurnHandler.Instance.IsPlayerTurn())
        {
            if(!alreadyIdle)
            {
                currentAnimation = EnemyAnimationType.Idle;
                alreadyIdle = true;
            }
            generalTime = 0;
            posUpdated = false;
            //if already idle then do nothing
        }
        else
        {
            if(!pathFound && playerIndex != tileIndex)
            {
                BFSPathfinding(); 
            }
            //Update the tile to the next tile
            if(!posUpdated)
            {
                tileIndex = Utils.ConvertToIndex(window, sprite.Position, sprite);
                switch(currDirection)
                {
                    case Direction.Right:
                        tileIndex += new Vector2i(1, 0);
                        break;
                    case Direction.Left:
                        tileIndex -= new Vector2i(1, 0);
                        break;
                    case Direction.Down:
                        tileIndex += new Vector2i(0, 1);
                        break;
                    case Direction.Up:
                        tileIndex -= new Vector2i(0, 1);
                        break;
                }
                posUpdated = true;
            }

            //if it's the enemy turn, move
            EnemyMovement(deltaTime);
            
            //EnemyAttack();
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
                sprite.Scale = new Vector2f(ENEMY_SCALING, ENEMY_SCALING);
                sprite.Position += new Vector2f(1, 0) * movementLength * deltaTime;
            }
            else if(currDirection == Direction.Left)
            {
                sprite.Scale = new Vector2f(-ENEMY_SCALING, ENEMY_SCALING);
                sprite.Position -= new Vector2f(1, 0) * movementLength * deltaTime;
            } 
            else if(currDirection == Direction.Down)
            {
                sprite.Position += new Vector2f(0, 1) * movementLength * deltaTime;
            } 
            else if(currDirection == Direction.Up)
            {
                sprite.Position -= new Vector2f(0, 1) * movementLength * deltaTime;
            } 
            else if(currDirection == Direction.None)
            {
                //Do nothing
                currentAnimation = EnemyAnimationType.Idle;
            }
            
            hasTurn = true; 
        }
        else
        {
            //if movement is done, set the turn to player again and be able to get idled next frame
            hasTurn = false;
            alreadyIdle = false;
            pathFound = false;
        }
    }
    public void SpriteInitializing(Vector2f position)
    {
        SpawnPosition = position;
        sprite = new Sprite(AssetManager.Instance.Textures[spriteName]);
        
        sprite.TextureRect = new IntRect(
            0,
            0,
            (int)(sprite.Texture.Size.X / ENEMY_TILING_X),
            (int)(sprite.Texture.Size.Y / ENEMY_TILING_Y)
        );

        sprite.Origin = new Vector2f(sprite.GetGlobalBounds().Left + sprite.GetGlobalBounds().Width / 2 + 2, sprite.GetGlobalBounds().Top + sprite.GetGlobalBounds().Height + 5);
        sprite.Position = SpawnPosition;
        sprite.Scale *= ENEMY_SCALING;
    }

    public void RespawnEnemy()
    {
        sprite.Position = SpawnPosition;
        tileIndex = Utils.ConvertToIndex(window, sprite.Position, sprite);
        soulHarvested = true;
        soulHarvestCooldownTimer = 0;

    }

    protected void BFSPathfinding()
    {
        bds = new BreadthFirstSearch(currentRoom.Map[0].Length, currentRoom.Map.Count, currentRoom, blockedEnemyTileIndex);
        List<Vector2i> tilesInWay = bds.FindPath(tileIndex, playerIndex);
        if(tilesInWay.Count == 0)
        {
            currDirection = Direction.None;
        }
        else
        {
            if(tilesInWay[0].X > tileIndex.X)
            {
                currDirection = Direction.Right;
            }
            else if(tilesInWay[0].X < tileIndex.X)
            {
                currDirection = Direction.Left;
            }
            else if(tilesInWay[0].Y > tileIndex.Y)
            {
                currDirection = Direction.Down;
            }
            else if(tilesInWay[0].Y < tileIndex.Y)
            {
                currDirection = Direction.Up;
            }
        }

        foreach(var enemy in currentRoom.Enemies)
        {
            switch(currDirection)
            {
                case Direction.Right:
                    if(enemy.tileIndex == tileIndex + new Vector2i(1, 0))
                    {
                        blockedEnemyTileIndex = enemy.tileIndex;
                        BFSPathfinding();
                        blockedEnemyTileIndex = null;
                    }
                    break;
                case Direction.Left:
                    if(enemy.tileIndex == tileIndex - new Vector2i(1, 0))
                    {
                        blockedEnemyTileIndex = enemy.tileIndex;
                        BFSPathfinding();
                        blockedEnemyTileIndex = null;
                    }
                    break;
                case Direction.Down:
                    if(enemy.tileIndex == tileIndex + new Vector2i(0, 1))
                    {
                        blockedEnemyTileIndex = enemy.tileIndex;
                        BFSPathfinding();
                        blockedEnemyTileIndex = null;
                    }
                    break;
                case Direction.Up:
                    if(enemy.tileIndex == tileIndex - new Vector2i(0, 1))
                    {
                        blockedEnemyTileIndex = enemy.tileIndex;
                        BFSPathfinding();
                        blockedEnemyTileIndex = null;
                    }
                    break;
            }
        }
        pathFound = true;
    }

    public void UpdatePlayerIndex(Vector2i playerIndex)
    {
        this.playerIndex = playerIndex;
    }
}