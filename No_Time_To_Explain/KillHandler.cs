public class KillHandler
{
    private Player player;
    private List<Enemy> enemies;
    private Hud hud;
    public KillHandler(Player player, List<Enemy> enemies, Hud hud)
    {
        this.player = player;
        this.enemies = enemies;
        this.hud = hud;
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
                    player.RespawnPlayer();
                }
            }
        }
    }

    public void UpdateEnemies(List<Enemy> enemies)
    {
        this.enemies = enemies;
    }
}