/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Player : GameObject
{
    // Constants
    private const float MOVE_TIME = 0.25f;
    private const int PLAYER_TILING_X = 9;
    private const int PLAYER_TILING_Y = 5;
    private const float PLAYER_SCALING = 2f;
    private const float DEATH_ANIMATION_DELAY = 1;
    private const int LAST_DEATH_FRAME = 8;
    private const float TIME_UNTIL_NEXT_SPAWN = 3;
    private const int NEXT_LEVEL_ANIMATION_SPEED_MODIFIER = 6;
    private const string OBSTACLEHIT_SOUND_NAME = "obstacleHit.wav";
    private const string WOOSH_SOUND_NAME = "woosh.wav";
    private const string DEMONLAUGH_SOUND_NAME = "demonLaugh.wav";
    private const string DEATH_MUSIC_NAME = "deathMusic.ogg";

    // Dependencies
    private RenderWindow renderWindow;
    private UI_Handler hud;

    // Sprites and animations
    private Sprite player;
    private int[] frameCountPerAnimation;
    private int animationFrame;
    private float animationTime = 0;
    private float animationSpeed = 5;
    private PlayerAnimationType currentAnimation = PlayerAnimationType.Idle;
    private int spriteXOffset;
    private int spriteYOffset;

    // Movement and positioning
    private float movementLength = 185f;
    private bool isMoving;
    private float generalTime = MOVE_TIME + 1; //has to be greater than MOVE_TIME, because we wanna move as soon as we load in
    private Direction currDirection;
    public Vector2i TileIndex { get; private set; }
    private Room currentRoom;

    // Sounds
    private Sound obstacleHit;
    private Sound woosh;
    private Sound demonLaugh;
    public Music DeathMusic { get; private set; }

    // State variables
    private bool turnLock = false;
    private bool isDead = false;
    private float deathTimer = 0;
    private bool stopDeathTimer = false;
    private float deathAnimationDelayTimer = 0;
    private bool hasLaughed = false;
    private bool deathMusicPlaying = false;
    public bool ReachedTeleporter { get; private set; }
    private float teleporterTimer;
    public bool GameResetTeleporter { get; private set; }

    public Player(RenderWindow renderWindow, UI_Handler hud)
    {
        this.renderWindow = renderWindow;
        this.hud = hud;
    }

    public override void Draw(RenderWindow window)
    {
        window.Draw(player);   
    }

    public override void Initialize()
    {
        currDirection = Direction.Right;
        isMoving = false;
        
        deathTimer = 0;
        teleporterTimer = 0;

        AssetManager.Instance.LoadSound(SoundName.ObstacleHit, OBSTACLEHIT_SOUND_NAME);
        obstacleHit = new Sound(AssetManager.Instance.Sounds[SoundName.ObstacleHit]);
        obstacleHit.Pitch = 2.5f;
        obstacleHit.Volume = 5;

        AssetManager.Instance.LoadSound(SoundName.Woosh, WOOSH_SOUND_NAME);
        woosh = new Sound(AssetManager.Instance.Sounds[SoundName.Woosh]);
        woosh.Volume = 10;

        AssetManager.Instance.LoadSound(SoundName.DemonLaugh, DEMONLAUGH_SOUND_NAME);
        demonLaugh = new Sound(AssetManager.Instance.Sounds[SoundName.DemonLaugh]);
        demonLaugh.Volume = 5;

        AssetManager.Instance.LoadMusic(MusicName.DeathMusic, DEATH_MUSIC_NAME);
        DeathMusic = AssetManager.Instance.Music[MusicName.DeathMusic];
        DeathMusic.Volume *= 0.1f;
        DeathMusic.Loop = true;

        frameCountPerAnimation = new int[5];
        frameCountPerAnimation[(int)PlayerAnimationType.Idle] = 6; //0
        frameCountPerAnimation[(int)PlayerAnimationType.Move] = 4; //1
        frameCountPerAnimation[(int)PlayerAnimationType.Death] = 9; //3

        player = new Sprite(AssetManager.Instance.Textures[TextureName.Player]);
        
        player.TextureRect = new IntRect(
            0,
            0,
            (int)(player.Texture.Size.X / PLAYER_TILING_X),
            (int)(player.Texture.Size.Y / PLAYER_TILING_Y)
        );

        player.Origin = new Vector2f(player.GetGlobalBounds().Left + player.GetGlobalBounds().Width / 2 + 3, player.GetGlobalBounds().Top + player.GetGlobalBounds().Height - 4);
        player.Position = currentRoom.SpawnTile.Position + new Vector2f(currentRoom.TileSize / 2, currentRoom.TileSize / 2);
        player.Scale *= PLAYER_SCALING;
        TileIndex = Utils.ConvertToIndex(renderWindow, player);
    }

    public override void Update(float deltaTime)
    {
        animationTime += deltaTime * animationSpeed;   
        if(!isDead && !ReachedTeleporter)
        {
            Input_Handling(deltaTime);
        }
        else if(isDead)
        {
            Death_Handling(deltaTime);
        }
        else if(ReachedTeleporter)
        {
            Teleporter_Handling(deltaTime);
        }
    }

    private void Teleporter_Handling(float deltaTime)
    {
        teleporterTimer += deltaTime * NEXT_LEVEL_ANIMATION_SPEED_MODIFIER;

        if(teleporterTimer >= TIME_UNTIL_NEXT_SPAWN * NEXT_LEVEL_ANIMATION_SPEED_MODIFIER && hud.FinishedScoreAdding)
        {
            GameResetTeleporter = true;
            hud.Reset();
        }

        player.Position = new Vector2f(0, 100);
        player.Scale = new Vector2f(5, 5);

        frameCountPerAnimation[(int)PlayerAnimationType.Move] = 8;
        currentAnimation = PlayerAnimationType.Move;

        animationFrame = (int)(teleporterTimer % frameCountPerAnimation[(int)currentAnimation]);
        
        spriteXOffset = animationFrame * player.TextureRect.Width;
        spriteYOffset = (int)currentAnimation * player.TextureRect.Height;
            
        player.TextureRect = new IntRect(
            spriteXOffset,
            spriteYOffset,
            player.TextureRect.Width,
            player.TextureRect.Height
        );
    }

    private void Death_Handling(float deltaTime)
    {
        if(!hasLaughed)
        {
            demonLaugh.Play();
            hasLaughed = true;
        }

        if(deathAnimationDelayTimer < DEATH_ANIMATION_DELAY)
        {
            deathAnimationDelayTimer += deltaTime;
        }
        else if(!stopDeathTimer && deathAnimationDelayTimer >= DEATH_ANIMATION_DELAY)
        {
            deathTimer += deltaTime * 6;
        }

        player.Position = new Vector2f(0, 100);
        player.Scale = new Vector2f(5, 5);
        
        currentAnimation = PlayerAnimationType.Death;

        animationFrame = (int)(deathTimer % frameCountPerAnimation[(int)currentAnimation]);
        
        spriteXOffset = animationFrame * player.TextureRect.Width;
        spriteYOffset = (int)currentAnimation * player.TextureRect.Height;
            
        player.TextureRect = new IntRect(
            spriteXOffset,
            spriteYOffset,
            player.TextureRect.Width,
            player.TextureRect.Height
        );

        if(animationFrame == LAST_DEATH_FRAME && !stopDeathTimer)
        {
            hud.DisplayDeathText();
            stopDeathTimer = true;

            if(!deathMusicPlaying)
            {
                DeathMusic.Play();
                deathMusicPlaying = true;
            }
            
        }
    }

    private void Input_Handling(float deltaTime)
    {
        generalTime += deltaTime;
        if (TurnHandler.Instance.IsPlayerTurn())
        {
            turnLock = false;
            if (InputManager.Instance.GetKeyDown(Keyboard.Key.D) && generalTime > MOVE_TIME)
            {
                if(TileIndex.X + 1 < currentRoom.Map[TileIndex.Y].Length)
                {
                    if (Utils.IsObstacle(TileIndex + new Vector2i(1, 0), currentRoom.Map))
                    {
                        obstacleHit.Play();
                        AnimationHandling();
                    }
                    else
                    {
                        woosh.Play();
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
                if(TileIndex.X - 1 >= 0)
                {
                    if (Utils.IsObstacle(TileIndex - new Vector2i(1, 0), currentRoom.Map))
                    {
                        obstacleHit.Play();
                        AnimationHandling();
                    }
                    else
                    {
                        woosh.Play();
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
                if(TileIndex.Y - 1 >= 0)
                {
                    if (Utils.IsObstacle(TileIndex - new Vector2i(0, 1), currentRoom.Map))
                    {
                        obstacleHit.Play();
                        AnimationHandling();
                    }
                    else
                    {
                        woosh.Play();
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
                if(TileIndex.Y + 1 < currentRoom.Map.Count)
                {
                    if (Utils.IsObstacle(TileIndex + new Vector2i(0, 1), currentRoom.Map))
                    {
                        obstacleHit.Play();
                        AnimationHandling();
                    }
                    else
                    {
                        woosh.Play();
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
            MovementHandling(deltaTime);
        }
    }

    private void MovementHandling(float deltaTime)
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
            
            TileIndex = Utils.ConvertToIndex(renderWindow, player);
            
            if(generalTime > MOVE_TIME)
            {
                isMoving = false;
                currentAnimation = PlayerAnimationType.Idle;
                foreach(var enemy in currentRoom.Enemies)
                {
                    enemy.UpdatePlayerIndex(TileIndex);
                }
                TurnHandler.Instance.EnemyTurn();
            }
        }

        AnimationHandling();
    }

    private void AnimationHandling()
    {
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

    public void SetCurrentRoom(Room room)
    {
        currentRoom = room;
    }

    public void SpawnPlayerFromPreviousRoomTile()
    {
        player.Position = currentRoom.NextRoomTile.Position + new Vector2f(currentRoom.TileSize / 2, currentRoom.TileSize / 2);
        TileIndex = Utils.ConvertToIndex(renderWindow, player);
    }

    public void SpawnPlayerFromNextRoomTile()
    {
        if(currentRoom.hasSpawnTile)
        {
            player.Position = currentRoom.SpawnTile.Position + new Vector2f(currentRoom.TileSize / 2, currentRoom.TileSize / 2);
        }
        else
        {
            player.Position = currentRoom.PreviousRoomTile.Position + new Vector2f(currentRoom.TileSize / 2, currentRoom.TileSize / 2);
        }

        TileIndex = Utils.ConvertToIndex(renderWindow, player);
    }

    public void SetTurnLock()
    {
        turnLock = true;
    }

    public bool GetTurnLock()
    {
        return turnLock;
    }

    public void KillPlayer()
    {
        isDead = true;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void HasReachedTeleporter()
    {
        ReachedTeleporter = true;
    }
}