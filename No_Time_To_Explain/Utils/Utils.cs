using SFML.Graphics;
using SFML.System;

public static class Utils
{
    public static float SqrMagnitude(this Vector2f input)
    {
        return (input.X * input.X + input.Y * input.Y);
    }

    public static float ToDegrees(this float angle)
    {
        return ((float)(angle * 180 / Math.PI));
    }

    public static float ToRadians(this float angle)
    {
        return ((float)(angle * Math.PI / 180));
    }

    public static Vector2f Normalize(this Vector2f source)
    {
        return new Vector2f(
            (float)(1 /Math.Sqrt(source.SqrMagnitude())* source.X), 
            (float)(1 /Math.Sqrt(source.SqrMagnitude())* source.Y)
        );
    }

    public static Vector2f RotateVector(Vector2f v, float angle)
    {
        return new Vector2f(
            (float)(v.X * Math.Cos(angle) - v.Y * Math.Sin(angle)),
            (float)(v.X * Math.Sin(angle) + v.Y * Math.Cos(angle))
        );
    }

    public static float AngleBetween(Vector2f v1, Vector2f v2)
    {
        return (float)Math.Atan2((v2.X * v1.Y) - (v2.Y * v1.X), (v2.X * v1.X) + (v2.Y * v1.Y));
    }

    public static float Distance(Vector2f a, Vector2f b)
    {
        return (float)Math.Sqrt(((a.X - b.X)*(a.X - b.X)) + ((a.Y - b.Y)*(a.Y - b.Y)));
    }

    public static Vector2f Lerp(Vector2f firstVector, Vector2f secondVector, float t)
    {
        t = Math.Clamp(t, 0.0f, 1.0f);
        return firstVector + (secondVector - firstVector) * t;
    }

    public static float Dot(Vector2f lhs, Vector2f rhs)
    {
        return (lhs.X * rhs.X + lhs.Y * rhs.Y);
    }

    internal static Vector2i ConvertToIndex(RenderWindow window, Vector2f position, Sprite sprite)
    {
        int a = (int)Math.Floor(sprite.Position.X / Game.TILE_SIZE + window.GetView().Size.X/Game.TILE_SIZE / 2);
        int b = (int)Math.Floor(sprite.Position.Y / Game.TILE_SIZE + window.GetView().Size.Y/Game.TILE_SIZE / 2);
        return new Vector2i(a, b);
    }

    internal static bool IsObstacle(Vector2i position, List<int[]> map)
    {
        int val = map[position.Y][position.X];
        Console.WriteLine("Value of tile: " + val);
        if(val >= 5 && val != Room.SPAWN_TILE_INDEX)
        {
            return true;
        }
                                                                                    
        return false;                                                               
    }

}