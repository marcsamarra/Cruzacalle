using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cruzacalle.Modelo
{
    class Vehiculo
    {
        public int Id { get; set; }
        public Texture2D Textura { get; set; }
        public Carril Carril { get; set; }
        public int Distancia { get; set; }

        public Vehiculo(int id, Texture2D textura, Carril carril, int distancia)
        {
            this.Id = id;
            this.Textura = textura;
            this.Carril = carril;
            this.Distancia = distancia;

        }

        public Vehiculo()
        {
        }

        public Vector2 getposicion()
        {
            return new Vector2(Distancia, Carril.PosicionY);
        }
    }
}
