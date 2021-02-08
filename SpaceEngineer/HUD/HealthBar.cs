using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using SpaceEngineer.GUI;

namespace SpaceEngineer.HUD
{
    public class HealthBar
    {
        private ProgressBar _hpBar;

        public HealthBar(Sprite _barSprite, Sprite _borderSprite, Vector2 position, Vector2 scale)
        {
            _hpBar = new ProgressBar(_barSprite, position, scale, _borderSprite);
        }

        /// <summary>
        /// Updates current HP value
        /// </summary>
        /// <param name="value">percentage Range: (0f-1f)</param>
        public void UpdateHp(float value)
        {
            _hpBar.UpdateProgress(value);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _hpBar.Draw(spriteBatch);
        }
    }
}
