/*
MultiMediaTechnology / FH Salzburg
MultiMediaProject 1
Author: Kevin Raffetseder
*/

using SFML.Graphics;

public abstract class GameObject : Transformable
{
    public abstract void Initialize();

    public abstract void Update(float deltaTime);

    public abstract void Draw(RenderWindow window);
}