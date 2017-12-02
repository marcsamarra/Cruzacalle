using Cruzacalle.Modelo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;

namespace Cruzacalle
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D jugador;
        Vector2 posicion;

        Vehiculo[] vehiculos;
        Carril[] carriles;

        int nivel = 0;

        Texture2D carblue;
        Texture2D carred;
        Texture2D decorado;

        Texture2D btnUpTexture;
        Texture2D btnDownTexture;
        Texture2D btnLeftTexture;
        Texture2D btnRightTexture;

        FaseJuego faseJuego;

        int puntuacion;

        SpriteFont gameFont;

        //Resolution Independence
        Vector2 virtualScreen;
        Vector3 ScalingFactor;
        Matrix Scale;

        Button btnUp;
        Button btnDown;
        Button btnLeft;
        Button btnRight;

        SoundEffect SoundTecla;
        SoundEffect SoundCash;
        SoundEffect SoundFrenazo;
        SoundEffect SoundAmbiente;

        SoundEffectInstance SoundAmbienteInstance;

        Song musica;

        
        bool KeyUpLibre;
        bool KeyDownLibre;
        bool KeyLeftLibre;
        bool KeyRightLibre;

        int VelocidadPierde;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Creamos una pantalla virtual con las dimensiones que consideremos		
            virtualScreen = new Vector2(800, 480);
        }

        public void StopSound()
        {
            if (SoundAmbienteInstance.State == SoundState.Playing)
            {
                if (MediaPlayer.State == MediaState.Playing && MediaPlayer.GameHasControl)
                {
                    MediaPlayer.Stop();
                }

                SoundAmbienteInstance.Stop(true);
            }
        }

        // Calcualmos la escala
        private void CalculateScalingFactor(float width, float height)
        {
            float widthScale = width / virtualScreen.X;
            float heightScale = height / virtualScreen.Y;

            ScalingFactor = new Vector3(widthScale, heightScale, 1);

            Scale = Matrix.CreateScale(ScalingFactor);
        }
                
        internal void Resize(double width, double height)
        {
            CalculateScalingFactor((float)width, (float)height);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            IsMouseVisible = true;
            base.Initialize();

            carriles = new Carril[5];

            CrearBotones();
            
            IniciarPartida();

            faseJuego = FaseJuego.Inicio;
            puntuacion = 0;

            if (MediaPlayer.GameHasControl)
            {
                MediaPlayer.Stop();
            }

        }

        private void CrearBotones()
        {
            btnUp = new Button(btnUpTexture, 10, (int)virtualScreen.Y - 100);
            btnDown = new Button(btnDownTexture, 140, (int)virtualScreen.Y - 100);
            btnLeft = new Button(btnLeftTexture, (int)virtualScreen.X - 240, (int)virtualScreen.Y - 100);
            btnRight = new Button(btnRightTexture, (int)virtualScreen.X - 110, (int)virtualScreen.Y - 100);
        }

        private void CrearVehiculos()
        {
            if (vehiculos != null)
            {
                Array.Clear(vehiculos, 0, vehiculos.Length);
            }

            int totalVehiculos = nivel * 2;
            
            vehiculos = new Vehiculo[totalVehiculos];
            Random r = new Random();

            for (int i = 0; i < totalVehiculos; i++)
            {
                int numerodecarril = i % 5;

                int distancia = r.Next(64,(int)virtualScreen.X- 64);
                vehiculos[i] = new Vehiculo(i, carblue, carriles[numerodecarril], distancia);
            }

        }

        private void CrearCarriles()
        {
            Array.Clear(carriles, 0, carriles.Length);

            Random r = new Random();

            for (int i = 0; i < 5; i++)
            {
                int velocidad = 0;
                int maxvel = Math.Min(nivel, 5);
                do
                {
                    velocidad = r.Next(-maxvel, maxvel);
                } while (velocidad == 0);

                carriles[i] = new Carril(i, 96 + i * 64, velocidad);
            }
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            jugador = Content.Load<Texture2D>("man");
            carblue = Content.Load<Texture2D>("carblue");
            carred = Content.Load<Texture2D>("carred");
            decorado = Content.Load<Texture2D>("decorado");

            gameFont = Content.Load<SpriteFont>("Fonts/LetraNormal");

            btnUpTexture = Content.Load<Texture2D>("Buttons/Up");
            btnDownTexture = Content.Load<Texture2D>("Buttons/Down");
            btnLeftTexture = Content.Load<Texture2D>("Buttons/Left");
            btnRightTexture = Content.Load<Texture2D>("Buttons/Right");

            SoundTecla = Content.Load<SoundEffect>("Sounds/tecla");
            SoundCash = Content.Load<SoundEffect>("Sounds/cash");
            SoundFrenazo = Content.Load<SoundEffect>("Sounds/frenazo");
            SoundAmbiente = Content.Load<SoundEffect>("Sounds/ambiente");

            SoundAmbienteInstance = SoundAmbiente.CreateInstance();
            SoundAmbienteInstance.Volume = 0.2f;
            SoundAmbienteInstance.IsLooped = true;

            musica = Content.Load<Song>("Sounds/musica");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                StopSound();
                Exit();
            }

            // TODO: Add your update logic here
            
            switch (faseJuego)
            {
                case FaseJuego.Inicio:
                    UpdateInicio();
                    break;
                case FaseJuego.Juego:
                    UpdateJuego();
                    break;
                case FaseJuego.Gana:
                    UpdateGana();
                    break;
                case FaseJuego.Pierde:
                    UpdatePierde();
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdatePierde()
        {

            if (posicion.X>0 && posicion.X<virtualScreen.X)
            {
                //MoverVehiculos();
                posicion.X += VelocidadPierde;
            }
            else
            {
                puntuacion = 0;
                nivel = 0;
                IniciarPartida();
                VelocidadPierde = 0;
                faseJuego = FaseJuego.Inicio;
            }
        }

        private void UpdateGana()
        {
            puntuacion++;
            IniciarPartida();
            faseJuego = FaseJuego.Juego;
        }

        private void IniciarPartida()
        {
            nivel++;

            CrearCarriles();
            CrearVehiculos();

            posicion = new Vector2(
               virtualScreen.X/ 2,
               virtualScreen.Y- 64);
        }

        private void UpdateJuego()
        {
            ControlDeTeclado();
            ControlTactil();
            MoverVehiculos();
            ComprobarColisiones();
            ComprobarLimites();
        }

        private void ControlTactil()
        {
            var touchState = TouchPanel.GetState();

            if (btnUp.Pressed(ScalingFactor, ref touchState))
            {
                MueveArriba();
            }
            if (btnDown.Pressed(ScalingFactor, ref touchState))
            {
                MueveAbajo();
            }
            if (btnRight.Pressed(ScalingFactor, ref touchState))
            {
                MueveIzquierda();
            }
            if (btnLeft.Pressed(ScalingFactor, ref touchState))
            {
                MueveDerecha();
            }
        }

        private void Gana()
        {
            faseJuego = FaseJuego.Gana;
            SoundCash.Play(1, 0, 0);
        }

        private void ComprobarLimites()
        {
            if (posicion.X > virtualScreen.X)
                posicion.X = virtualScreen.X;

            if (posicion.X < 0)
                posicion.X = 0;

            if (posicion.Y > virtualScreen.Y)
                posicion.Y = virtualScreen.Y;

            if (posicion.Y < 60) Gana();
        }

        private void UpdateInicio()
        {
            MoverVehiculos();
            
            if (TapScreenOrKey())
            {
                faseJuego = FaseJuego.Juego;
                SoundAmbienteInstance.Play();


                if (MediaPlayer.State != MediaState.Playing && MediaPlayer.GameHasControl)
                {
                    MediaPlayer.Play(musica);
                }

            }
        }

        private void ComprobarColisiones()
        {
            Rectangle rectanguloJugador = new Rectangle(
                    posicion.ToPoint(),
                    jugador.Bounds.Size
                );

            foreach (Vehiculo vehiculo in vehiculos)
            {
                Rectangle rectanguloVehiculo =
                       new Rectangle(vehiculo.getposicion().ToPoint()
                       , vehiculo.Textura.Bounds.Size);

                if (rectanguloVehiculo.Intersects(rectanguloJugador))
                {
                    vehiculo.Textura = carred;

                    Pierde(vehiculo);                
                }
            } 
        }

        private void Pierde(Vehiculo vehiculo)
        {
            StopSound();
            SoundFrenazo.Play(1, 0, 0);
            VelocidadPierde = vehiculo.Carril.Velocidad;
            faseJuego = FaseJuego.Pierde;
        }

        private void MoverVehiculos()
        {
            foreach (Vehiculo vehiculo in vehiculos)
            {
                vehiculo.Distancia += vehiculo.Carril.Velocidad;

                if (vehiculo.Distancia > virtualScreen.X)
                {
                    vehiculo.Distancia = -vehiculo.Textura.Width;
                }

                if (vehiculo.Distancia < -vehiculo.Textura.Width)
                {
                    vehiculo.Distancia = (int)virtualScreen.X;
                }
            }
        }

        private void MueveArriba()
        {
            posicion.Y -= 25;
            SoundTecla.Play(0.5f, 0, 0);
        }

        private void MueveAbajo()
        {
            posicion.Y += 25;
            SoundTecla.Play(0.5f, 0, 0);
        }

        private void MueveIzquierda()
        {
            posicion.X -= 25;
            SoundTecla.Play(0.5f, 0, 0);

        }

        private void MueveDerecha()
        {
            posicion.X += 25;
            SoundTecla.Play(0.5f, 0, 0);

        }
        
        private void ControlDeTeclado()
        {
            var estadoTeclado = Keyboard.GetState();

            if (KeyUpLibre && estadoTeclado.IsKeyDown(Keys.Up))
            {
                MueveArriba();
                KeyUpLibre = false;
            }
            if (KeyDownLibre && estadoTeclado.IsKeyDown(Keys.Down))
            {
                MueveAbajo();
                KeyDownLibre = false;
            }
            if (KeyRightLibre && estadoTeclado.IsKeyDown(Keys.Right))
            {
                MueveDerecha();
                KeyRightLibre = false;
            }
            if (KeyLeftLibre && estadoTeclado.IsKeyDown(Keys.Left))
            {
                MueveIzquierda();
                KeyLeftLibre = false;
            }

            if (estadoTeclado.IsKeyUp(Keys.Up))
            {
                KeyUpLibre = true;
            }
            if (estadoTeclado.IsKeyUp(Keys.Down))
            {
                KeyDownLibre = true;
            }
            if (estadoTeclado.IsKeyUp(Keys.Right))
            {
                KeyRightLibre = true;
            }
            if (estadoTeclado.IsKeyUp(Keys.Left))
            {
                KeyLeftLibre = true;
            }
        }

        private bool TapScreenOrKey()
        {
            int presedkeys = Keyboard.GetState().GetPressedKeys().Length;
            
            TouchCollection touchLocations = TouchPanel.GetState();

            if (touchLocations.Count > 0)
            {
                TouchLocation touchLocation = touchLocations.Last();

                if (touchLocation.State == TouchLocationState.Pressed)
                {
                    return true;
                }
            }
            return (presedkeys > 0);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate,null,null,null,null,null,Scale);

            spriteBatch.Draw(decorado, Vector2.Zero);

            DibujarPuntuacion();
            DibujarBotones();

           
            if (faseJuego == FaseJuego.Inicio)
            {
                var color = Color.DarkRed;

                var tick = gameTime.TotalGameTime.TotalMilliseconds / 100;
                color.A =(byte)(155 + Math.Sin(tick) * 100);

                DibujarMensajeInicio(color);
            }

            float rotation = 0f;

            if (faseJuego == FaseJuego.Pierde)
            {
                rotation = (float)gameTime.TotalGameTime.TotalMilliseconds / 500;
                spriteBatch.Draw(jugador, posicion,
                scale: Vector2.One,
                rotation: rotation,
                origin: new Vector2(jugador.Width / 2, jugador.Height / 2),
                color: Color.White,
                effects: SpriteEffects.None,
                layerDepth: 0,
                sourceRectangle: null);
            }
            else
            {
                spriteBatch.Draw(jugador, posicion,
                scale: Vector2.One,
                rotation: rotation,
                origin: Vector2.Zero,
                color: Color.White,
                effects: SpriteEffects.None,
                layerDepth: 0,
                sourceRectangle: null);
            }

            DibujarVehiculos();
                       
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        private void DibujarBotones()
        {
            btnUp.Draw(spriteBatch);
            btnDown.Draw(spriteBatch);
            btnLeft.Draw(spriteBatch);
            btnRight.Draw(spriteBatch);
        }

        private void DibujarMensajeInicio(Color color)
        {
            string mensaje = "Pulsa una tecla o tap";
            var longitud = gameFont.MeasureString(mensaje).X;

            spriteBatch.DrawString(gameFont, mensaje,
                new Vector2(
                    (virtualScreen.X - longitud) / 2,
                    virtualScreen.Y/ 2)
                , color);
        }

        private void DibujarPuntuacion()
        {
            string mensaje = String.Format("Puntos : {0:D4}", puntuacion);

            var longitud = gameFont.MeasureString(mensaje).X;

            spriteBatch.DrawString(gameFont, mensaje,
                new Vector2(
                   virtualScreen.X- longitud, 40)
                    , Color.White);

        }

        private void DibujarVehiculos()
        {
            foreach (Vehiculo vehiculo in vehiculos)
            {
                spriteBatch.Draw(vehiculo.Textura, vehiculo.getposicion());
            }

        }
    }
}
