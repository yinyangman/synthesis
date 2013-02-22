﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using SynthesisGameLibrary;

namespace Synthesis
{
    public class Level
    {
        #region Global Variables
        //Textures
        public Texture2D t_StartBackground;
        public Texture2D t_Background;
        public Texture2D t_LevelBounds;
        public Color[] levelBoundsData;

        //Frames
        //public Rectangle levelTop = new Rectangle(508, 0, 2900, 306);
        //public Rectangle levelBottom = new Rectangle(508, 1916, 2900, 388);
        //public Rectangle levelLeft = new Rectangle(0, 0, 508, 2304);
        //public Rectangle levelRight = new Rectangle(3408, 0, 688, 2304);

        //Level Variables
        public Rectangle levelBounds;
        public LevelData levelData;
        #endregion
        public Level(LevelData newData)
        {
            levelData = newData;
        }
        public virtual void Initialize(SoundBank soundBank)
        {
            //NormalLevel
            levelBounds = new Rectangle(100, 100, 3896, 2104);
        }
        public virtual void LoadContent(ContentManager Content)
        {
            t_StartBackground = Content.Load<Texture2D>("MenusBGrounds//Level1Start");
            t_Background = Content.Load<Texture2D>("MenusBGrounds//background");
            t_LevelBounds = Content.Load<Texture2D>("levelBounds");
            levelBoundsData = new Color[t_LevelBounds.Width * t_LevelBounds.Height];
            t_LevelBounds.GetData(levelBoundsData);
        }
        public virtual void Update(GameTime gameTime, Engine engine, Game1 game)
        {
        }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Engine engine, Game1 game)
        {
            spriteBatch.Draw(t_Background, (new Vector2(0, 0) + engine.offset), Color.White);
        }

        void TimeUp()
        {
        }

        public void FinishedFadingToBlack(Game1 game)
        {
        }
    }
}
