using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Enemy : GameObject
{
    protected Sprite sprite;
    protected const int ENEMY_TILING_X = 6;
    protected const int ENEMY_TILING_Y = 4;
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
    protected const float SOUL_HARVEST_COOLDOWN = 5f;
    protected Vector2i playerIndex;
    protected Room currentRoom;
    protected int tilesToGoal = 0;
    protected BreadthFirstSearch bds;
    protected bool pathFound = false;
    protected Vector2i? blockedEnemyTileIndex = null;
    protected bool posUpdated = false;
    protected const int MAX_TILES_SEARCHED = 40;
    protected List<Vector2i> attackPattern = new();
    protected Vector2i? lockedAttackTile = null;
    public bool readiedAttack = false;
    protected bool isAttacking = false;
    protected bool endOfTurnLock = true;
    protected bool checkForPlayer = true;
    protected Vector2i goalTile = new();
    public bool IsHighlighted = false;
    private bool blockedMovement = false;
    private Color originalColor;
    private const float MOVEMENT_TIME = 0.25f;
    public List<Vector2i> AttackPatternTiles
    {
        get
        {
            return attackPattern;
        }
    }

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
        window.MouseButtonPressed += OnMouseButtonPressed;


        tileIndex = Utils.ConvertToIndex(window, sprite.Position, sprite);
        currDirection = Direction.Right;
        frameCountPerAnimation = new int[4];
        frameCountPerAnimation[(int)EnemyAnimationType.Idle] = 4; 
        frameCountPerAnimation[(int)EnemyAnimationType.Move] = 6; 
        frameCountPerAnimation[(int)EnemyAnimationType.Death] = 6;
        frameCountPerAnimation[(int)EnemyAnimationType.AttackReady] = 4;
    }

    public override void Update(float deltaTime)
    {
        animationTime += deltaTime * animationSpeed;
        soulHarvestCooldownTimer += deltaTime;
        if(soulHarvestCooldownTimer >= SOUL_HARVEST_COOLDOWN)
        {
                soulHarvested = false;
        }
        
        if(soulHarvested)
        {
            sprite.Color = new Color(255, 255, 255, 128);
        }
        else
        {
            sprite.Color = originalColor;
        }

        Input_AnimationHandling(deltaTime);

    }

    protected void Input_AnimationHandling(float deltaTime)
    {
        
        if(TurnHandler.Instance.IsPlayerTurn())
        {
            if(endOfTurnLock)
            {
                attackPattern = GetAttackTiles();
                endOfTurnLock = false;
            }

            if(!alreadyIdle)
            {
                currentAnimation = EnemyAnimationType.Idle;
                alreadyIdle = true;
            }
            generalTime = 0;
            posUpdated = false;
            checkForPlayer = true;
        }
        else if(!endOfTurnLock) //f.e. if enemy is readying an attack, he should not move
        {
            if(!blockedMovement) //if enemy just respawned, he should not be able to move immediatly again
            {
                if(!isAttacking && checkForPlayer)
                {
                    CheckPlayerInAttackRange(); //Check if player is in Attack Range 1 time, then wait until its enemy turn again to check again
                    
                    checkForPlayer = false;
                }

                if(!pathFound && playerIndex != tileIndex && !readiedAttack && !isAttacking)
                {
                    BFSPathfinding();
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
                }

                if(!posUpdated && !readiedAttack)
                {
                    tileIndex = goalTile;
                    posUpdated = true;
                }

                if(!readiedAttack)
                {
                    EnemyMovement(deltaTime);
                }
                else if(isAttacking)
                {
                    DirectionMovement(deltaTime);
                }
            }
            else
            {
                FinishedFunction();
            }

        }    
        AnimationHandling();
    }

    private void AnimationHandling()
    {
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

    protected void CheckPlayerInAttackRange()
    {

        attackPattern = GetAttackTiles();

        foreach (var tile in attackPattern)
        {
            if (playerIndex == tile)
            {
                if (readiedAttack)
                {
                    Attack(tile);
                    return;
                }
                else
                {
                    ReadyAttack(tile);
                    endOfTurnLock = true;
                    return;
                }
            }
        }

        if (readiedAttack)
        {
            Attack(null);
        }
    }



    private void Attack(Vector2i? tile)
    {
        Vector2i? pos = new();

        if(tile == null)
        {
            pos = lockedAttackTile;
        }
        else
        {
            pos = tile;
        }
        posUpdated = false;
        isAttacking = true;
        readiedAttack = false;

        if(pos.Value.X > tileIndex.X)
        {
            currDirection = Direction.Right;
        }
        else
        {
            currDirection = Direction.Left;
        }
        goalTile = new Vector2i(pos.Value.X, pos.Value.Y);
    }

    protected virtual List<Vector2i> GetAttackTiles()
    {
        return new List<Vector2i>();
    }

    protected void ReadyAttack(Vector2i markedTile)
    {
        lockedAttackTile = markedTile;
        currentAnimation = EnemyAnimationType.AttackReady;
        currDirection = Direction.None;
        posUpdated = true;
        pathFound = true;
        readiedAttack = true;
    }

    protected void EnemyMovement(float deltaTime)
    {
        if(generalTime < MOVEMENT_TIME)
        {
            generalTime += deltaTime;
            currentAnimation = EnemyAnimationType.Move;
            DirectionMovement(deltaTime);
            
            hasTurn = true; 
        }
        else
        {
            //if movement is done, set the turn to player again and be able to get idled next frame
            FinishedFunction();
        }
    }

    private void FinishedFunction()
    {
        hasTurn = false;
        alreadyIdle = false;
        pathFound = false;
        isAttacking = false;
        readiedAttack = false;
        lockedAttackTile = null;
        endOfTurnLock = true;
        blockedMovement = false;
        attackPattern = GetAttackTiles();
    }

    private void DirectionMovement(float deltaTime)
    {
        switch (currDirection)
        {
            case Direction.Right:
                sprite.Scale = new Vector2f(ENEMY_SCALING, ENEMY_SCALING);      
                break;

            case Direction.Left:
                sprite.Scale = new Vector2f(-ENEMY_SCALING, ENEMY_SCALING);
                break;

            case Direction.None:
                // Do nothing
                currentAnimation = EnemyAnimationType.Idle;
                break;

            case Direction.RightDown:
                sprite.Scale = new Vector2f(ENEMY_SCALING, ENEMY_SCALING);
                break;

            case Direction.RightUp:
                sprite.Scale = new Vector2f(ENEMY_SCALING, ENEMY_SCALING);
                break;

            case Direction.LeftDown:
                sprite.Scale = new Vector2f(-ENEMY_SCALING, ENEMY_SCALING);
                break;

            case Direction.LeftUp:
                sprite.Scale = new Vector2f(-ENEMY_SCALING, ENEMY_SCALING);
                break;
        }

        float t = generalTime / MOVEMENT_TIME;
        sprite.Position = Utils.Lerp(sprite.Position, Utils.ConvertToPosition(window, goalTile), t);
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
        originalColor = sprite.Color;
    }

    public void RespawnEnemy()
    {
        sprite.Position = SpawnPosition;
        tileIndex = Utils.ConvertToIndex(window, sprite.Position, sprite);
        soulHarvested = true;
        soulHarvestCooldownTimer = 0;
        readiedAttack = false;
        goalTile = tileIndex;
        endOfTurnLock = true;
        blockedMovement = true;
    }

    protected void BFSPathfinding()
    {
        bds = new BreadthFirstSearch(currentRoom.Map[0].Length, currentRoom.Map.Count, currentRoom, blockedEnemyTileIndex, MAX_TILES_SEARCHED);
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

        if(tilesInWay.Count > 0)
        {
            goalTile = tilesInWay[0];
        }
        else
        {
            goalTile = tileIndex;
        }
        pathFound = true;

    }

    public void UpdatePlayerIndex(Vector2i playerIndex)
    {
        this.playerIndex = playerIndex;
    }

    private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
    {
        if (e.Button == Mouse.Button.Left)
        {
            Vector2i mousePosition = (Vector2i)window.MapPixelToCoords(Mouse.GetPosition(window));
            FloatRect spriteBounds = sprite.GetGlobalBounds();

            if (spriteBounds.Contains(mousePosition.X, mousePosition.Y))
            {
                IsHighlighted = true;
            }
            else
            {
                IsHighlighted = false;
            }
        }
    }
}