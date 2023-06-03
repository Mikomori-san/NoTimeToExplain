/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Enemy : GameObject, IDisposable
{
    // Sprites
    protected Sprite sprite;
    private RenderWindow window;
    protected Vector2f SpawnPosition;
    protected Color originalColor;

    // Constants
    protected const int ENEMY_TILING_X = 6;
    protected const int ENEMY_TILING_Y = 4;
    protected const float ENEMY_SCALING = 1.5f;
    protected const float SOUL_HARVEST_COOLDOWN = 5f;
    protected const int MAX_TILES_SEARCHED = 60;
    private const float MOVEMENT_TIME = 0.25f;
    private const int MAX_TIME = 120;
    private const float DISPLAY_SOULS_THRESHOLD = 1f;
    private const int SOULS_SCORE_MODIFIER = 10;
    private const int TIME_SCORE_MODIFIER = 3;

    // Animation
    protected int[] frameCountPerAnimation;
    protected int animationFrame;
    protected float animationTime = 0;
    protected float animationSpeed = 5;
    protected EnemyAnimationType currentAnimation = EnemyAnimationType.Idle;
    protected int spriteXOffset;
    protected int spriteYOffset;
    protected Direction currDirection = Direction.Right;
    protected float generalTime = 0;
    protected bool alreadyIdle = true;
    protected bool isAttacking = false;
    protected bool endOfTurnLock = true;
    protected bool checkForPlayer = true;
    protected bool posUpdated = false;
    protected bool blockedMovement = false;

    // Enemy Type
    protected EnemyType enemyType;
    protected string spriteName;
    protected bool pathFound = false;
    protected Vector2i? blockedEnemyTileIndex = null;
    protected Vector2i? lockedAttackTile = null;
    protected Vector2i goalTile = new();

    // Gameplay
    public bool HasTurn { get; private set; } = false;
    public Vector2i TileIndex { get; private set; }
    public bool SoulHarvested { get; private set; } = false;
    protected float soulHarvestCooldownTimer = 0;
    protected Vector2i playerIndex;
    protected Room currentRoom;
    protected BreadthFirstSearch bds;
    public List<Vector2i> AttackPatternTiles {get; private set; } = new();
    public bool ReadiedAttack { get; private set; } = false;
    public bool IsHighlighted { get; private set; } = false;


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


        TileIndex = Utils.ConvertToIndex(window, sprite);
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
        generalTime += deltaTime;
        soulHarvestCooldownTimer += deltaTime;
        if(soulHarvestCooldownTimer >= SOUL_HARVEST_COOLDOWN)
        {
                SoulHarvested = false;
        }
        
        if(SoulHarvested)
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
                AttackPatternTiles = GetAttackTiles();
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

                if(!pathFound && playerIndex != TileIndex && !ReadiedAttack && !isAttacking)
                {
                    BFSPathfinding();
                    foreach(var enemy in currentRoom.Enemies)
                    {
                        switch(currDirection)
                        {
                            case Direction.Right:
                                if(enemy.TileIndex == TileIndex + new Vector2i(1, 0))
                                {
                                    blockedEnemyTileIndex = enemy.TileIndex;
                                    BFSPathfinding();
                                    blockedEnemyTileIndex = null;
                                }
                                break;
                            case Direction.Left:
                                if(enemy.TileIndex == TileIndex - new Vector2i(1, 0))
                                {
                                    blockedEnemyTileIndex = enemy.TileIndex;
                                    BFSPathfinding();
                                    blockedEnemyTileIndex = null;
                                }
                                break;
                            case Direction.Down:
                                if(enemy.TileIndex == TileIndex + new Vector2i(0, 1))
                                {
                                    blockedEnemyTileIndex = enemy.TileIndex;
                                    BFSPathfinding();
                                    blockedEnemyTileIndex = null;
                                }
                                break;
                            case Direction.Up:
                                if(enemy.TileIndex == TileIndex - new Vector2i(0, 1))
                                {
                                    blockedEnemyTileIndex = enemy.TileIndex;
                                    BFSPathfinding();
                                    blockedEnemyTileIndex = null;
                                }
                                break;
                        }
                    }
                }

                if(!posUpdated && !ReadiedAttack)
                {
                    TileIndex = goalTile;
                    posUpdated = true;
                }

                if(!ReadiedAttack)
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

        AttackPatternTiles = GetAttackTiles();

        foreach (var tile in AttackPatternTiles)
        {
            if (playerIndex == tile)
            {
                if (ReadiedAttack)
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

        if (ReadiedAttack)
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
        ReadiedAttack = false;

        if(pos.Value.X > TileIndex.X)
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
        ReadiedAttack = true;
    }

    protected void EnemyMovement(float deltaTime)
    {
        if(generalTime < MOVEMENT_TIME)
        {
            currentAnimation = EnemyAnimationType.Move;
            DirectionMovement(deltaTime);
            
            HasTurn = true; 
        }
        else
        {
            //if movement is done, set the turn to player again and be able to get idled next frame
            FinishedFunction();
        }
    }

    private void FinishedFunction()
    {
        HasTurn = false;
        alreadyIdle = false;
        pathFound = false;
        isAttacking = false;
        ReadiedAttack = false;
        lockedAttackTile = null;
        endOfTurnLock = true;
        blockedMovement = false;
        AttackPatternTiles = GetAttackTiles();
        checkForPlayer = true;
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
        TileIndex = Utils.ConvertToIndex(window, sprite);
        SoulHarvested = true;
        soulHarvestCooldownTimer = 0;
        ReadiedAttack = false;
        goalTile = TileIndex;
        endOfTurnLock = true;
        blockedMovement = true;
    }

    protected void BFSPathfinding()
    {
        bds = new BreadthFirstSearch(currentRoom.Map[0].Length, currentRoom.Map.Count, currentRoom, blockedEnemyTileIndex, MAX_TILES_SEARCHED);
        List<Vector2i> tilesInWay = bds.FindPath(TileIndex, playerIndex);
        if(tilesInWay.Count == 0)
        {
            currDirection = Direction.None;
        }
        else
        {
            if(tilesInWay[0].X > TileIndex.X)
            {
                currDirection = Direction.Right;
            }
            else if(tilesInWay[0].X < TileIndex.X)
            {
                currDirection = Direction.Left;
            }
            else if(tilesInWay[0].Y > TileIndex.Y)
            {
                currDirection = Direction.Down;
            }
            else if(tilesInWay[0].Y < TileIndex.Y)
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
            goalTile = TileIndex;
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

    public new void Dispose()
    {
        window.MouseButtonPressed -= OnMouseButtonPressed; //in a nutshell: unsubscribe when Enemy is getting disposed of
    }
}