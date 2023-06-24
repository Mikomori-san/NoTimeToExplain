/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

using SFML.Audio;
using SFML.System;

public class KillHandler
{
    private const string KILL_SOUND_NAME = "kill.wav";
    private Player player;
    private List<Enemy> enemies;
    private UI_Handler hud;
    private Sound killSound;
    public KillHandler(Player player, List<Enemy> enemies, UI_Handler hud)
    {
        this.player = player;
        this.enemies = enemies;
        this.hud = hud;
        
        AssetManager.Instance.LoadSound(SoundName.Kill, KILL_SOUND_NAME);
        killSound = new Sound(AssetManager.Instance.Sounds[SoundName.Kill]);
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
                if(player.TileIndex == enemy.TileIndex)
                {
                    if(enemy.SoulHarvested == false)
                    {
                        hud.AddSoul();
                    }
                    
                    enemy.RespawnEnemy();
                    killSound.Play();
                    killSound.PlayingOffset = Time.FromSeconds(0.5f); //if player stands on enemy respawn tile while enemy respawns, player dies
                    
                    if(enemy.TileIndex == player.TileIndex)
                    {
                        KillPlayer();
                    }
                }
            } 
        } 
        else
        {
            //Enemy Turn
            foreach(var enemy in enemies)
            {
                if(enemy.TileIndex == player.TileIndex)
                {
                    KillPlayer();
                }
            }
        }
    }

    internal void KillPlayer()
    {
        player.KillPlayer();
        killSound.Play();
        killSound.PlayingOffset = Time.FromSeconds(0.5f);
    }

    public void UpdateEnemies(List<Enemy> enemies)
    {
        this.enemies = enemies;
    }
}