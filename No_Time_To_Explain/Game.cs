/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;


public class Game
{
    // Constants
    private const uint ORIGINAL_WIDTH = 1920;
    private const uint ORIGINAL_HEIGHT = 1080;
    public const int TILE_SIZE = 48;
    private const string PLAYER_TEXTURE_NAME = "Player/player.png";
    private const string MAP_TEXTURE_NAME = "Tiles/Tileset.png";
    private const string LAVAGOLEM_TEXTURE_NAME = "Enemy/LavaGolem/LavaGolemSpriteSheet.png";
    private const string STONEGOLEM_TEXTURE_NAME = "Enemy/StoneGolem/StoneGolemSpriteSheet.png";
    private const string BROKENSTONEGOLEM_TEXTURE_NAME = "Enemy/BrokenStoneGolem/BrokenStoneGolemSpriteSheet.png";
    private const string BASESTONEGOLEM_TEXTURE_NAME = "Enemy/BaseStoneGolem/BaseStoneGolemSpriteSheet.png";
    private const string BACKGROUND_TEXTURE_NAME = "background.png";
    private const string BACKGROUND1_MUSIC_NAME = "YourOwnPersonalHell.ogg";
    private const string BACKGROUND2_MUSIC_NAME = "DemonSlayer.ogg";
    private const string LEVELSWITCH_SOUND_NAME = "levelSwitch.wav";

    // Properties
    public RenderWindow Window { get; private set; }
    private View view;
    private float gameTime = 0;
    private Player player;
    private Hud hud;
    private EnemyHandler enemyHandler;
    private Room currentRoom;
    private Music? backgroundMusic;
    private Sound levelSwitch;
    private KillHandler killHandler;
    private float aspectRatio;
    private Sprite backgroundSprite;
    private List<Room> randomRoomsToGenerate;
    private int maxRandomRoomsCounter = 0;
    private int currentCountOfRandomRooms = 0;
    private List<Music> backgroundTracks;
    public bool Retry { get; private set; }
    public bool ReachedTeleporter { get; private set; }

    public Game(RenderWindow window)
    {
        Window = window;
        
        view = new View(new Vector2f(), (Vector2f)Window.Size);
        Window.SetView(view);

        Window.Closed += OnWindowClosed;
        Window.Resized += OnResizeWindow;
    }

    public void Run()
    {
        Initialize();
        AddEvents();

        Clock clock = new Clock();

        while (Window.IsOpen)
        {
            float deltaTime = clock.Restart().AsSeconds();

            HandleEvents();

            Update(deltaTime);

            Draw();

            if (Retry || ReachedTeleporter)
            {
                RemoveEvents();
                Initialize();
                AddEvents();
            }
        }
    }

    private void Update(float deltaTime)
    {
        gameTime += deltaTime;
        player.Update(deltaTime);
        
        if(!player.IsDead() && !player.ReachedTeleporter) //basically if the game runs normally
        {
            enemyHandler.Update(deltaTime);
            killHandler.SearchForCollisions();
            RoomManagement();
        }
        else //if the player is dead or reached the teleporter, delete the enemies and stop the backgroundMUsic
        {
            killHandler.UpdateEnemies(new List<Enemy>());
            backgroundMusic.Stop();
        }

        hud.Update(deltaTime);

        if(hud.RemainingTime() <= 0 && !player.IsDead() && !player.ReachedTeleporter) //if the time runs out but the player is not already dead OR in teleporter sequence
        {
            killHandler.KillPlayer();
        }

        if(player.GameResetTeleporter) //if player reached the teleporter
        {
            ReachedTeleporter = true;
        }

        InputManager.Instance.Update(deltaTime);
    } 

    private void Draw()
    {
        Window.Clear();
        if(!player.IsDead() && !player.ReachedTeleporter) //only draw the player and the hud when the player is dead or in teleporter
        {
            Window.Draw(backgroundSprite);
            currentRoom.Draw(Window);
            enemyHandler.Draw(Window);
        }
        player.Draw(Window);
        hud.Draw(Window);
        Window.Display();
    }

    private void HandleEvents()
    {
        Window.DispatchEvents();
    }

    private void Initialize()
    {
        InputManager.Instance.Initialize(Window);
        AssetManager.Instance.LoadTexture(TextureName.Player, PLAYER_TEXTURE_NAME);
        AssetManager.Instance.LoadTexture(TextureName.Map, MAP_TEXTURE_NAME);
        AssetManager.Instance.LoadTexture(TextureName.LavaGolem, LAVAGOLEM_TEXTURE_NAME);
        AssetManager.Instance.LoadTexture(TextureName.StoneGolem, STONEGOLEM_TEXTURE_NAME);
        AssetManager.Instance.LoadTexture(TextureName.BrokenStoneGolem, BROKENSTONEGOLEM_TEXTURE_NAME);
        AssetManager.Instance.LoadTexture(TextureName.BaseStoneGolem, BASESTONEGOLEM_TEXTURE_NAME);
        AssetManager.Instance.LoadTexture(TextureName.Background, BACKGROUND_TEXTURE_NAME);
        AssetManager.Instance.LoadMusic(MusicName.Background1, BACKGROUND1_MUSIC_NAME);
        AssetManager.Instance.LoadMusic(MusicName.Background2, BACKGROUND2_MUSIC_NAME);
        AssetManager.Instance.LoadSound(SoundName.LevelSwitch, LEVELSWITCH_SOUND_NAME);
        
        Retry = false; // set retry to false if Initialize was called because of Retry

        currentCountOfRandomRooms = 0;

        backgroundSprite = new Sprite(AssetManager.Instance.Textures[TextureName.Background]);
        backgroundSprite.Position = new Vector2f(-Window.GetView().Size.X / 2, -Window.GetView().Size.Y / 2);
        backgroundSprite.Scale *= 3;

        backgroundTracks = new List<Music>();
        backgroundTracks.Add(AssetManager.Instance.Music[MusicName.Background1]);
        backgroundTracks.Add(AssetManager.Instance.Music[MusicName.Background2]);
        Random randomTracks = new Random();
        backgroundMusic = backgroundTracks[randomTracks.Next(0, backgroundTracks.Count)];
        backgroundMusic.Volume *= 0.1f;


        levelSwitch = new Sound(Utils.TrimSound(AssetManager.Instance.Sounds[SoundName.LevelSwitch], 1.5f));
        levelSwitch.Volume *= 0.7f;

        if(!ReachedTeleporter) //if the player reached the teleporter, do not set the hud new because it will delete the score
        {
            hud = new Hud(Window);
        }
        hud.Initialize();

        ReachedTeleporter = false;

        Random ran = new Random();
        maxRandomRoomsCounter = ran.Next(0, 4); //create between 0 and 4 random rooms between the spawn and teleporter room

        RoomHandler.Instance.SetRooms(Window);

        randomRoomsToGenerate = RoomHandler.Instance.GenerateRandomRooms(maxRandomRoomsCounter);
        RoomHandler.Instance.SetFirstRoom();
        
        currentRoom = RoomHandler.Instance.GetCurrentRoom();

        player = new Player(Window, hud);
        player.SetCurrentRoom(currentRoom);
        player.Initialize();
        RoomHandler.Instance.EnemySetter(player, Window);

        enemyHandler = new EnemyHandler(currentRoom.Enemies, currentRoom.EnemySpawnTiles, currentRoom.TileSize); 
        enemyHandler.Initialize();                            

        killHandler = new KillHandler(player, currentRoom.Enemies, hud);

        backgroundMusic.Play();
        backgroundMusic.Loop = true;
    }

    private void AddEvents()
    {
        hud.RetryButtonPressed += RetryButtonPressedHandler;
    }

    private void RemoveEvents()
    {
        hud.RetryButtonPressed -= RetryButtonPressedHandler;
    }

    private void RetryButtonPressedHandler()
    {
        Retry = true;
        player.DeathMusic.Stop();
    }

    private void RoomManagement()
    {
        if(currentRoom.NextRoomTile != null)
        {
            if(!TurnHandler.Instance.IsPlayerTurn() && 
                player.TileIndex == Utils.ConvertToIndex(Window, currentRoom.NextRoomTile) &&
                !player.GetTurnLock()
            )
            {
                if(RoomHandler.Instance.GetNextRooms().Count > 0)
                {
                    RoomHandler.Instance.NextRoom();
                }
                else if(currentCountOfRandomRooms < maxRandomRoomsCounter)
                {
                    Random random = new Random();
                    int randomRoom = random.Next(0, randomRoomsToGenerate.Count);

                    while(randomRoomsToGenerate[randomRoom].Name == currentRoom.Name)
                    {
                        randomRoom = random.Next(0, randomRoomsToGenerate.Count);
                    }

                    RoomHandler.Instance.NextRoom(randomRoomsToGenerate[randomRoom]);
                    hud.Add30Seconds();
                    currentCountOfRandomRooms++;
                }
                else
                {
                    RoomHandler.Instance.NextTeleporterRoom();
                    hud.Add30Seconds();
                }
                currentRoom = RoomHandler.Instance.GetCurrentRoom();
                RoomHandler.Instance.EnemySetter(player, Window);
                player.SetCurrentRoom(currentRoom);
                player.SpawnPlayerFromNextRoomTile();
                enemyHandler.UpdateEnemies(currentRoom.Enemies, currentRoom.EnemySpawnTiles);
                killHandler.UpdateEnemies(currentRoom.Enemies);
                player.SetTurnLock();
                levelSwitch.Play();
            }
        }
        
        if(currentRoom.PreviousRoomTile != null)
        {
            if(!TurnHandler.Instance.IsPlayerTurn()  && 
                player.TileIndex == Utils.ConvertToIndex(Window, currentRoom.PreviousRoomTile) &&
                !player.GetTurnLock()
            )
            {
                RoomHandler.Instance.PreviousRoom();
                currentRoom = RoomHandler.Instance.GetCurrentRoom();
                RoomHandler.Instance.EnemySetter(player, Window);
                player.SetCurrentRoom(currentRoom);
                player.SpawnPlayerFromPreviousRoomTile();
                enemyHandler.UpdateEnemies(currentRoom.Enemies, currentRoom.EnemySpawnTiles);
                killHandler.UpdateEnemies(currentRoom.Enemies);
                player.SetTurnLock();
                levelSwitch.Play();
            }
        }

        if(currentRoom.TeleporterTile != null)
        {
            if(player.TileIndex == Utils.ConvertToIndex(Window, currentRoom.TeleporterTile))
            {
                hud.ReachedTeleporter();
                player.HasReachedTeleporter();
            }
        }
    }

    

    private void OnResizeWindow(object? sender, SizeEventArgs e)
    {
        aspectRatio = (float)ORIGINAL_WIDTH / ORIGINAL_HEIGHT;

        int newWidth = (int)(e.Height * aspectRatio);
        int newHeight = (int)e.Height;

        if (newWidth > e.Width)
        {
            newWidth = (int)e.Width;
            newHeight = (int)(newWidth / aspectRatio);
        }

        Window.Size = new Vector2u((uint)newWidth, (uint)newHeight);

    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        Window.Close();
    }
}