/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

using SFML.Audio;
using SFML.Graphics;

public class AssetManager
{
    private static AssetManager? instance;
    public readonly Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();
    public readonly Dictionary<string, SoundBuffer> Sounds = new Dictionary<string, SoundBuffer>();
    public readonly Dictionary<string, Music> Music = new Dictionary<string, Music>();
    public readonly Dictionary<string, Font> Fonts = new Dictionary<string, Font>();
    public static AssetManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AssetManager();
            }

            return instance;
        }
    }

    private AssetManager() { }

    public void LoadTexture(string name, string fileName)
    {
        Textures[name] = new Texture($"./Assets/Textures/{fileName}");
    }

    public void LoadSound(string name, string fileName)
    {
        Sounds[name] = new SoundBuffer($"./Assets/Sounds/{fileName}");
    }

    public void LoadMusic(string name, string fileName)
    {
        Music[name] = new Music($"./Assets/Music/{fileName}");
    }

    public void LoadFont(string name, string fileName)
    {
        Fonts[name] = new Font($"./Assets/Fonts/{fileName}");
    }
}