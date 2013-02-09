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

        int i_MasterFaderCounter = 0;
        int i_MasterFaderMax = 75;
        bool b_FadeInAgain = false;
        Texture2D t_MasterFadeBlack;
        FadingMode fadingMode;

        public Fader()
        {
        }
        public void Initialize()
        {

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
            spriteBatch.Draw(t_MasterFadeBlack, new Vector2(0, 0), new Color(1, 1, 1, (byte)((i_MasterFaderCounter / i_MasterFaderMax) * 255)));
        }

        void BeginFadingToBlack(int max, bool fadeIn)
        {
            i_MasterFaderCounter = 0;
            b_FadeInAgain = fadeIn;
            i_MasterFaderMax = max;
            fadingMode = FadingMode.FadeToBlack;
        }
        void FinishedFadingToBlack()
        {
            if (b_FadeInAgain == true)
            {
                BeginFadingIn(i_MasterFaderMax);
            }
            else
            {
                fadingMode = FadingMode.NoFade;
                //Finished fading to black
                //Send a notification somehow
            }
        }
        void BeginFadingIn(int max)
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
                FinishedFadingToBlack();
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
