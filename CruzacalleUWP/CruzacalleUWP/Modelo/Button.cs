using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Cruzacalle
{
    class Button
    {
        private Rectangle _location;
        private Texture2D _texture;
        private int _lastTouchId;
        private bool _pressed;
        
        public Button(Texture2D texture, int posX, int posY)
        {
            _texture = texture;
            _location = new Rectangle(posX, posY, texture.Width, texture.Height);
        }

        public bool Pressed(Vector3 scalingFactor, ref TouchCollection touches)
        {
            foreach (var touch in touches)
            {
                if (touch.Id == _lastTouchId)
                    continue;

                if (touch.State != TouchLocationState.Pressed)
                    continue;

                var px = touch.Position.X / scalingFactor.X;
                var py = touch.Position.Y / scalingFactor.Y;

                if (_location.Contains(new Vector2(px, py)))
                {
                    _lastTouchId = touch.Id;
                    _pressed = true;
                    return true;
                }
            }

            _pressed = false;
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _location, _pressed ? Color.DarkGray : Color.White);
        }

    }
}
