using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace SpaceEngineer.GameObjects.Ship
{
    public class BreakableComponent : ShipComponent
    {
        public override IShapeF Bounds { get; }
        public override void Cancel()
        {
            // TODO - Add cancel action when player moves away
        }

        public BreakableComponent(Sprite sprite, Vector2 startingPosition)
        {
            _sprite = sprite;
            _position = startingPosition;

            // Translates position into Transform and then converts that to a RectangleF; 
            // Used to get correct sprite bounds for collision detection
            Transform2 transform = new Transform2(startingPosition);
            RectangleF rectangle = _sprite.GetBoundingRectangle(transform);

            Bounds = rectangle;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, _position);
            spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Blue);
        }

        public override void Interact()
        {
            Console.WriteLine("Interacting...");
            // TODO = Add interaction logic
        }

        public override void Update(GameTime gameTime)
        {
            // TODO - Add fix logic
        }
    }
}
