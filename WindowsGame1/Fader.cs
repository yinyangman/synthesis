using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Synthesis
{
    public class Fader
    {
        public enum FadingMode
        {
            NoFade,
            FadeToBlack,
            FadeIn
        }

        public delegate void FinishedFadingToBlackDelegate(Game1 game);
        FinishedFadingToBlackDelegate fadeToBlackDelegate;

        public FadingMode fadingMode;
        float i_MasterFaderCounter = 0;
        float i_MasterFaderMax = 75;
        bool b_FadeInAgain = false;
        public Texture2D t_MasterFadeBlack;
        Game1 game;

        public Fader()
        {
        }
        public void Initialize(Game1 newGame)
        {
            game = newGame;
        }
        public void LoadContent(ContentManager Content)
        {
            t_MasterFadeBlack = Content.Load<Texture2D>("MenusBGrounds//BlackScreen");
        }
        public void Update(GameTime gameTime)
        {
            if (fadingMode == FadingMode.FadeToBlack)
            {
                UpdateFadeToBlack();
            }
            else if (fadingMode == FadingMode.FadeIn)
            {
                UpdateFadeIn();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (fadingMode != FadingMode.NoFade)
            {
                Color colour = new Color(1,1,1);
                colour.A = (byte)((i_MasterFaderCounter / i_MasterFaderMax) * 255);

                spriteBatch.Draw(t_MasterFadeBlack, new Vector2(0, 0), colour);
            }
        }

        void BeginFadingToBlack(float max, bool fadeIn)
        {
            i_MasterFaderCounter = 0;
            b_FadeInAgain = fadeIn;
            i_MasterFaderMax = max;
            fadingMode = FadingMode.FadeToBlack;
        }
        public void BeginFadingToBlack(float max, bool fadeIn, Tutorial tutorial)
        {
            BeginFadingToBlack(max, fadeIn);
            fadeToBlackDelegate = new FinishedFadingToBlackDelegate(tutorial.FinishedFadingToBlack);
        }
        public void BeginFadingToBlack(float max, bool fadeIn, Level level)
        {
            BeginFadingToBlack(max, fadeIn);
            fadeToBlackDelegate = new FinishedFadingToBlackDelegate(level.FinishedFadingToBlack);
        }
        public void BeginFadingToBlack(float max, bool fadeIn, Game1 game)
        {
            BeginFadingToBlack(max, fadeIn);
            fadeToBlackDelegate = new FinishedFadingToBlackDelegate(game.FinishedFadingToBlack);
        }
        void FinishedFadingToBlack(Game1 game)
        {
            fadeToBlackDelegate(game);

            if (b_FadeInAgain == true)
            {
                BeginFadingIn(i_MasterFaderMax);
            }
            else
            {
                fadingMode = FadingMode.NoFade;
            }
        }
        void BeginFadingIn(float max)
        {
            i_MasterFaderCounter = max;
            i_MasterFaderMax = max;
            fadingMode = FadingMode.FadeIn;
        }
        void FinishedFadingIn()
        {
            fadingMode = FadingMode.NoFade;

            //Finished fading in
            //Send a notification somehow
        }

        void UpdateFadeToBlack()
        {
            if (i_MasterFaderCounter >= i_MasterFaderMax)
            {
                //Fading To Black Finished
                FinishedFadingToBlack(game);
            }
            else
            {
                i_MasterFaderCounter++;
            }
        }
        void UpdateFadeIn()
        {
            if (i_MasterFaderCounter <= 0)
            {
                //Fading In Finished
                FinishedFadingIn();
            }
            else
            {
                i_MasterFaderCounter--;
            }
        }

    }
}
