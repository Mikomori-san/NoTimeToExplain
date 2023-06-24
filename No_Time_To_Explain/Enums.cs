/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

public enum Direction
{
    Right = 0,
    Down = 1,
    Left = 2,
    Up = 3,
    None = 4,
    RightUp = 5,
    RightDown = 6,
    LeftUp = 7,
    LeftDown = 8
}

public enum EnemyAnimationType
{
    Idle = 0,
    Move = 1,
    Death = 2,
    AttackReady = 3
}

public enum EnemyType
{
    LavaGolem = 0,
    StoneGolem = 1,
    BrokenStoneGolem = 2,
    BaseStoneGolem = 3
}

public enum PlayerAnimationType
{
    Idle = 0,
    Move = 1,
    Death = 3 //2   
}

[Flags]
public enum RoomFeatures
{
    HasSpawnTile = 1,
    HasTeleporter = 2,
    HasNextRoom = 4,
    HasPreviousRoom = 8
}

public enum TextureName
{
    Player,
    Map,
    LavaGolem,
    StoneGolem,
    BrokenStoneGolem,
    BaseStoneGolem,
    Background,
    Retry,
    Leave,
    StartButton,
    TitlescreenBackground
}

public enum MusicName
{
    Background1,
    Background2,
    DeathMusic,
    TitleBackground
}

public enum SoundName
{
    LevelSwitch,
    ObstacleHit,
    Woosh,
    DemonLaugh,
    Kill
}

public enum FontName
{
    Hud,
    Death
}