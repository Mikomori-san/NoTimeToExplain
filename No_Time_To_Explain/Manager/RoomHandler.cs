public class RoomHandler
{
    private static RoomHandler? instance;
    private Room currentRoom;
    private Stack<Room> previousRooms = new Stack<Room>();
    private Stack<Room> nextRooms = new Stack<Room>();
    public static RoomHandler Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new();
            }

            return instance;
        }
    }

    public void SetFirstRoom(Room room)
    {
        currentRoom = room;
    }

    public void NextRoom(Room room)
    {
        previousRooms.Push(currentRoom);
        currentRoom = room;
    }

    public void NextRoom()
    {
        previousRooms.Push(currentRoom);
        currentRoom = nextRooms.Pop();
    }

    public void PreviousRoom()
    {
        nextRooms.Push(currentRoom);
        currentRoom = previousRooms.Pop();
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }
    public Stack<Room> GetNextRooms()
    {
        return nextRooms;
    }
}