/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

public class TurnHandler
{
    private static TurnHandler? instance;
    private bool isPlayerTurn = true;
    public static TurnHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TurnHandler();
            }

            return instance;
        }
    }

    public void PlayerTurn()
    {
        isPlayerTurn = true;
    }

    public void EnemyTurn()
    {
        isPlayerTurn = false;
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
}