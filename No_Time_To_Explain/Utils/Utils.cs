using SFML.Audio;
using SFML.Graphics;
using SFML.System;

public static class Utils
{
    public const int OBSTACLE_TILE_INDEX = 10;

    public static Vector2f Lerp(Vector2f firstVector, Vector2f secondVector, float t)
    {
        t = Math.Clamp(t, 0.0f, 1.0f);
        return firstVector + (secondVector - firstVector) * t;
    }

    internal static Vector2i ConvertToIndex(RenderWindow window, Sprite sprite)
    {
        int a = (int)Math.Floor(sprite.Position.X / Game.TILE_SIZE + window.GetView().Size.X/Game.TILE_SIZE / 2);
        int b = (int)Math.Floor(sprite.Position.Y / Game.TILE_SIZE + window.GetView().Size.Y/Game.TILE_SIZE / 2);
        return new Vector2i(a, b);
    }

    internal static Vector2f ConvertToPosition(RenderWindow window, Vector2i tileIndex)
    {
        float x = (tileIndex.X - window.GetView().Size.X / Game.TILE_SIZE / 2) * Game.TILE_SIZE;
        float y = (tileIndex.Y - window.GetView().Size.Y / Game.TILE_SIZE / 2) * Game.TILE_SIZE;
        return new Vector2f(x + 24, y + 34);
    }

    internal static bool IsObstacle(Vector2i position, List<int[]> map)
    {
        int val = map[position.Y][position.X];
        if(val >= OBSTACLE_TILE_INDEX)
        {
            return true;
        }
                                                                                    
        return false;                                                               
    }

    internal static SoundBuffer TrimSound(SoundBuffer originalSound, float durationInSeconds)
    {
        int sampleCount = (int)(originalSound.SampleRate * durationInSeconds);
        uint channelCount = originalSound.ChannelCount;

        int sampleCountToKeep = Math.Min(sampleCount, originalSound.Samples.Length);

        short[] trimmedSamples = new short[sampleCountToKeep * channelCount];

        Array.Copy(originalSound.Samples, trimmedSamples, sampleCountToKeep * channelCount);

        SoundBuffer trimmedSound = new SoundBuffer(trimmedSamples, channelCount, originalSound.SampleRate);

        return trimmedSound;
    }

}