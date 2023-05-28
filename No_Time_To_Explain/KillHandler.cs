using SFML.Audio;
using SFML.System;

public class KillHandler
{
    private Player player;
    private List<Enemy> enemies;
    private Hud hud;
    private Sound killSound;
    public KillHandler(Player player, List<Enemy> enemies, Hud hud)
    {
        this.player = player;
        this.enemies = enemies;
        this.hud = hud;
        killSound = new Sound(AssetManager.Instance.Sounds["kill"]);
        killSound.Volume *= 0.5f;
        killSound.PlayingOffset = Time.FromSeconds(0.5f);
    }

    public void SearchForCollisions()
    {
        if(TurnHandler.Instance.IsPlayerTurn())
        {
            //Player Turn
            foreach(var enemy in enemies)
            {
                if(player.tileIndex == enemy.tileIndex && enemy.soulHarvested == false)
                {
                    hud.AddSoul();
                    enemy.RespawnEnemy();
                    killSound.Play();
                    killSound.PlayingOffset = Time.FromSeconds(0.5f);
                    
                    if(enemy.tileIndex == player.tileIndex)
                    {
                        player.SpawnPlayerFromNextRoomTile();
                        killSound.Play();
                        killSound.PlayingOffset = Time.FromSeconds(0.5f);
                    }
                }
            } 
        } 
        else
        {
            //Enemy Turn
            foreach(var enemy in enemies)
            {
                if(enemy.tileIndex == player.tileIndex)
                {
                    player.SpawnPlayerFromNextRoomTile();
                    killSound.Play();
                    killSound.PlayingOffset = Time.FromSeconds(0.5f);
                }
            }
        }
    }

    public void UpdateEnemies(List<Enemy> enemies)
    {
        this.enemies = enemies;
    }
}