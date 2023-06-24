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
    public readonly Dictionary<TextureName, Texture> Textures = new();
    public readonly Dictionary<SoundName, SoundBuffer> Sounds = new();
    public readonly Dictionary<MusicName, Music> Music = new();
    public readonly Dictionary<FontName, Font> Fonts = new();

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

    public void LoadTexture(TextureName name, string fileName)
    {
        Textures[name] = new Texture($"./Assets/Textures/{fileName}");
    }

    public void LoadSound(SoundName name, string fileName)
    {
        Sounds[name] = new SoundBuffer($"./Assets/Sounds/{fileName}");
    }

    public void LoadMusic(MusicName name, string fileName)
    {
        Music[name] = new Music($"./Assets/Music/{fileName}");
    }

    public void LoadFont(FontName name, string fileName)
    {
        Fonts[name] = new Font($"./Assets/Fonts/{fileName}");
    }
}