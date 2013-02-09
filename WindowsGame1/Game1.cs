using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using SeeSharp.Xna.Video;

namespace Synthesis
{
    public enum State
    {
        Menu = 0,
        Controls = 1,
        GameIntro = 2,
        LevelIntro = 3,
        Gameplay = 4,
        LevelEnd = 5,
        GameEnd = 7,
        Options = 8,
        Paused = 9,
        HighScore = 10,
        Tutorial = 11,
        Loading = 12
    }
    public struct Highscore
    {
        public char letter1;
        public char letter2;
        public char letter3;
        public int score;
        public string grade;
    }
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        #region Global Variables

        public Engine gameEngine;
        public Level level1;
        public Tutorial tutorialLevel;
        public State gameState = State.Menu;

        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        //Input/Output
        KeyboardState oldKeyboardState;
        KeyboardState currentKeyboardState;
        public MouseState mouseStateCurrent;
        public GamePadState gamepadStateOld;
        public KeyboardState keyboardStateOld;
        public GamePadState gamepadStateCurr;
        public bool lastLTrigger = false;
        public bool lastEnter = false;
        public bool lastDown = false;
        public bool lastUp = false;
        public bool lastS = false;
        public bool lastW = false;
        public bool last1 = false;
        public bool last2 = false;
        public bool last3 = false;
        public bool last4 = false;
        public bool lastRMouse = false;
        public bool lastEscape = false;
        public int vibrate = 0;

        //Intro
        VideoPlayer v_Video1;

        //Game Settings
        bool TutorialOn = false;
        bool b_LevelComplete = false;

        // Audio objects
        public SoundBank soundBank;
        AudioEngine engine;
        WaveBank waveBank;
        Cue MenuMusic;
        Cue GameOver;
        int i_SoundCounter = 0;
        int i_SoundCounter2 = 0;

        //Textures
        public Texture2D t_BlackScreen;
        Texture2D menuImage;
        Texture2D controlsImage;
        Texture2D optionsImage;
        Texture2D t_NewGame;
        Texture2D t_Options;
        Texture2D t_Controls;
        Texture2D t_Quit;
        Texture2D t_Pixel;
        Texture2D t_UnderScore;
        Texture2D t_HighScore;
        Texture2D t_GORetry;
        Texture2D t_GOContinue;
        Texture2D t_GOQuit;
        Texture2D t_ButtonA;
        Texture2D t_ButtonB;
        Texture2D t_ButtonX;
        Texture2D t_ButtonY;
        Texture2D t_Loading;
        Texture2D t_Tick;
        Texture2D t_Cross;
        Texture2D menuText;

        //Main Menu
        Rectangle newGame;
        Rectangle controls;
        Rectangle options;
        Rectangle quit;
        int OptionsSelected = 1;
        int selected = 1;

        //Particle Effect Variables
        private static Random random = new Random();
        const float TimeBetweenSmokePlumePuffs = .5f;

        //Scoring
        int i_TopScore = 4000;
        float f_Score = 0;
        public float f_EnemiesBigKilled = 0;
        public float f_EnemiesSmallKilled = 0;
        public float f_LevelCompleteBonus = 0;
        float f_ScoreTemp = 0;
        float f_EnemiesBigKilledTemp = 0;
        float f_EnemiesSmallKilledTemp = 0;
        float f_FusionsTemp = 0;
        float f_LevelCompleteBonusTemp = 0;
        int i_LetterPos = 0;
        int i_WhichLetter = 1;
        int i_NewScore = 0;
        int i_UnderScoreTimer = 0;
        string s_NewGrade = "";
        bool entering = false;
        int i_HighScorePosition = 11;
        string s_grade = "N/A";
        string s_Input = "Test";
        Highscore[] highscores;

        //Fonts
        public SpriteFont font;
        public SpriteFont fontBig;
        public SpriteFont fontTimer;
        public SpriteFont fontName;
        public SpriteFont fontQuestion;
        public SpriteFont fontSmallText;

        //Fadings
        public Fader fd_Fader;

        public byte by_BlackCounter = 0;
        float f_BlackAlpha = 0;
        float f_Stats1Alpha = 0;
        float f_Stats2Alpha = 0;
        float f_Stats3Alpha = 0;
        float f_Stats4Alpha = 0;
        float f_ChoicesAlpha = 0;
        float f_LvlCompleteAlpha = 0;
        float f_GradeAlpha = 0;
        bool b_fading = false;

        //Misc
        Color text1Colour = Color.Red;
        Color text2Colour = Color.White;
        bool justStarted = false;
        Vector2 cursorPos = Vector2.Zero;
        Vector2 particleOffset = Vector2.Zero;
        public int i_GameOverSel = 0;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            engine = new AudioEngine("Content\\Audio\\sound.xgs");
            soundBank = new SoundBank(engine, "Content\\Audio\\Sound Bank.xsb");
            waveBank = new WaveBank(engine, "Content\\Audio\\Wave Bank.xwb");

            MenuMusic = soundBank.GetCue("drumLoop");
            GameOver = soundBank.GetCue("gameOver");

            newGame = new Rectangle(350, 500, 100, 20);
            options = new Rectangle(350, 520, 80, 20);
            controls = new Rectangle(350, 540, 80, 20);
            quit = new Rectangle(350, 560, 60, 20);

            tutorialLevel = new Tutorial();
            tutorialLevel.Initialize(soundBank);

            level1 = new Level();
            level1.Initialize(soundBank);

            gameEngine = new Engine();
            gameEngine.Initialize(soundBank);

            fd_Fader = new Fader();
            fd_Fader.Initialize();

            base.Initialize();
        }
        protected override void LoadContent()
        {
            //Loading();
            // Create a new SpriteBatch, which can be used to draw textures.
            t_BlackScreen = Content.Load<Texture2D>("MenusBGrounds//BlackScreen");
            t_ButtonA = Content.Load<Texture2D>("Buttons//ButtonA");
            t_ButtonB = Content.Load<Texture2D>("Buttons//ButtonB");
            t_ButtonX = Content.Load<Texture2D>("Buttons//ButtonX");
            t_ButtonY = Content.Load<Texture2D>("Buttons//ButtonY");
            t_Tick = Content.Load<Texture2D>("MenusBGrounds//Tick");
            t_Cross = Content.Load<Texture2D>("MenusBGrounds//Cross");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            menuImage = Content.Load<Texture2D>("MenusBGrounds//MENU");
            controlsImage = Content.Load<Texture2D>("MenusBGrounds//Controls_menu");
            optionsImage = Content.Load<Texture2D>("MenusBGrounds//Options_menu");
            menuText = Content.Load<Texture2D>("MenusBGrounds//menu_text");
            t_NewGame = Content.Load<Texture2D>("MenusBGrounds//NewGame");
            t_Options = Content.Load<Texture2D>("MenusBGrounds//options");
            t_Controls = Content.Load<Texture2D>("MenusBGrounds//controls");
            t_Quit = Content.Load<Texture2D>("MenusBGrounds//Quit");
            font = Content.Load<SpriteFont>("Fonts//Font");
            fontBig = Content.Load<SpriteFont>("Fonts//FontBig");
            fontTimer = Content.Load<SpriteFont>("Fonts//FontTimer");
            fontName = Content.Load<SpriteFont>("Fonts//fontName");
            fontQuestion = Content.Load<SpriteFont>("Fonts//fontQuestion");
            fontSmallText = Content.Load<SpriteFont>("Fonts//fontSmallText");
            t_Loading = Content.Load<Texture2D>("MenusBGrounds//Loading");
            highscores = new Highscore[10];

            tutorialLevel.LoadContent(Content);
            level1.LoadContent(Content);
            gameEngine.LoadContent(Content);
            fd_Fader.LoadContent(Content);
        }
        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            //Updated Fader
            fd_Fader.Update(gameTime);

            // Update the audio engine.
            engine.Update();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (gameState == State.Menu)
            {
                #region Menu Update
                if (MenuMusic.IsPlaying == false)
                {
                    MenuMusic.SetVariable("Volume", 100);
                    MenuMusic.Play();
                }
                else if (MenuMusic.IsPaused == true)
                {
                    MenuMusic.Resume();
                }
                if (b_fading == true)
                {
                    if (by_BlackCounter == 50)
                    {
                        gameState = State.Loading;
                        by_BlackCounter = 0;
                        b_fading = false;
                    }
                    else
                    {
                        by_BlackCounter++;
                        MenuMusic.SetVariable("Volume", (100 - (by_BlackCounter * 2)));
                    }
                }
                else
                {
                    MenuControlsCheck();
                }
                #endregion
            }
            else if (gameState == State.GameIntro)
            {
                #region GameIntro Update
                if ((GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed && GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    v_Video1.Stop();
                    b_fading = true;
                    by_BlackCounter = 0;
                }

                if (b_fading == true)
                {
                    if (by_BlackCounter < 50)
                    {
                        by_BlackCounter++;
                    }
                    else
                    {
                        v_Video1.Dispose();
                        by_BlackCounter = 0;
                        b_fading = false;

                        if (TutorialOn == true)
                        {
                            gameEngine.InitalizeLevel(tutorialLevel, this);
                            gameState = State.Tutorial;
                        }
                        else if (TutorialOn == false)
                        {
                            gameEngine.i_ShipCounter = 75;
                            gameEngine.InitalizeLevel(level1, this);
                            gameState = State.Gameplay;
                        }
                    }
                }
                else
                {
                    if (v_Video1.CurrentPosition == v_Video1.Duration)
                    {
                        v_Video1.Stop();
                        b_fading = true;
                    }
                    else
                    {
                        v_Video1.Update();
                    }
                }
                #endregion
            }
            else if (gameState == State.Gameplay)
            {
                #region Gameplay Update
                gameEngine.Update(gameTime, soundBank, this);
                level1.Update(gameTime, gameEngine, this);
                #endregion
            }
            else if (gameState == State.Controls)
            {
                #region Controls Update
                if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    soundBank.PlayCue("Back");
                    gameState = State.Menu;
                }
                #endregion
            }
            else if (gameState == State.Options)
            {
                #region Options Update
                GamePadState gamepadStateCurr = GamePad.GetState(PlayerIndex.One);
                KeyboardState keyboardStateCurr = Keyboard.GetState();
                if (justStarted == false)
                {
                    justStarted = true;
                }
                else
                {
                    if ((gamepadStateCurr.Buttons.B == ButtonState.Pressed || keyboardStateCurr.IsKeyDown(Keys.Escape)) && (gamepadStateOld.Buttons.B != ButtonState.Pressed && !keyboardStateOld.IsKeyDown(Keys.Escape)))
                    {
                        justStarted = false;
                        soundBank.PlayCue("Back");
                        gameState = State.Menu;
                    }
                    if ((gamepadStateCurr.Buttons.A == ButtonState.Pressed || keyboardStateCurr.IsKeyDown(Keys.Enter)) && (gamepadStateOld.Buttons.A != ButtonState.Pressed && !keyboardStateOld.IsKeyDown(Keys.Enter)))
                    {
                        if (OptionsSelected == 1)
                        {
                            if (TutorialOn == true)
                            {
                                TutorialOn = false;
                            }
                            else
                            {
                                TutorialOn = true;
                            }
                        }
                        else if (OptionsSelected == 2)
                        {
                            graphics.ToggleFullScreen();
                        }
                        soundBank.PlayCue("Confirm");
                    }
                    if (gamepadStateCurr.ThumbSticks.Left.Y > 0)
                    {
                        if (!(gamepadStateOld.ThumbSticks.Left.Y > 0))
                        {
                            if (OptionsSelected == 1)
                            {
                                OptionsSelected = 2;
                                text1Colour = Color.White;
                                text2Colour = Color.Red;
                            }
                            else if (OptionsSelected == 2)
                            {
                                OptionsSelected = 1;
                                text1Colour = Color.Red;
                                text2Colour = Color.White;
                            }
                        }
                    }
                    if (gamepadStateCurr.ThumbSticks.Left.Y < 0)
                    {
                        if (!(gamepadStateOld.ThumbSticks.Left.Y < 0))
                        {
                            if (OptionsSelected == 1)
                            {
                                OptionsSelected = 2;
                                text1Colour = Color.White;
                                text2Colour = Color.Red;
                            }
                            else if (OptionsSelected == 2)
                            {
                                OptionsSelected = 1;
                                text1Colour = Color.Red;
                                text2Colour = Color.White;
                            }
                        }
                    }
                    if ((keyboardStateCurr.IsKeyDown(Keys.Up) || keyboardStateCurr.IsKeyDown(Keys.W)) && (!keyboardStateOld.IsKeyDown(Keys.Up) && !keyboardStateOld.IsKeyDown(Keys.W)))
                    {
                        if (OptionsSelected == 1)
                        {
                            OptionsSelected = 2;
                            text1Colour = Color.White;
                            text2Colour = Color.Red;
                        }
                        else if (OptionsSelected == 2)
                        {
                            OptionsSelected = 1;
                            text1Colour = Color.Red;
                            text2Colour = Color.White;
                        }
                        soundBank.PlayCue("Select");
                    }
                    if ((keyboardStateCurr.IsKeyDown(Keys.Down) || keyboardStateCurr.IsKeyDown(Keys.S)) && (!keyboardStateOld.IsKeyDown(Keys.Down) && !keyboardStateOld.IsKeyDown(Keys.S)))
                    {
                        if (OptionsSelected == 1)
                        {
                            OptionsSelected = 2;
                            text1Colour = Color.White;
                            text2Colour = Color.Red;
                        }
                        else if (OptionsSelected == 2)
                        {
                            OptionsSelected = 1;
                            text1Colour = Color.Red;
                            text2Colour = Color.White;
                        }
                        soundBank.PlayCue("Select");
                    }
                }
                gamepadStateOld = gamepadStateCurr;
                keyboardStateOld = keyboardStateCurr;
                #endregion
            }
            else if (gameState == State.Paused)
            {
                #region Paused Update
                gameEngine.PauseCheck(this);
                #endregion
            }
            else if (gameState == State.GameEnd)
            {
                #region GameEnd Update
                if (by_BlackCounter > 0)
                {
                    by_BlackCounter--;
                }
                if (b_LevelComplete == false)
                {
                    if (GameOver.IsPlaying == false)
                    {
                        GameOver.Play();
                    }
                    else if (GameOver.IsPaused == true)
                    {
                        GameOver.Resume();
                    }
                }

                #endregion
                //gameEngine.Gameover(b_LevelComplete);
            }
            else if (gameState == State.HighScore)
            {
                #region HighScore Update
                HighScoreInput();
                if (entering == true)
                {
                    if (i_WhichLetter == 1)
                    {
                        highscores[i_HighScorePosition].letter1 = NumberToLetter(highscores[i_HighScorePosition].letter1);
                    }
                    if (i_WhichLetter == 2)
                    {
                        highscores[i_HighScorePosition].letter2 = NumberToLetter(highscores[i_HighScorePosition].letter2);
                    }
                    if (i_WhichLetter == 3)
                    {
                        highscores[i_HighScorePosition].letter3 = NumberToLetter(highscores[i_HighScorePosition].letter3);
                    }
                }
                else
                {
                }
                #endregion
            }
            else if (gameState == State.Tutorial)
            {
                #region Tutorial Update
                gameEngine.Update(gameTime, soundBank, this);
                tutorialLevel.Update(gameTime, gameEngine, this);
                #endregion
            }
            else if (gameState == State.Loading)
            {
                #region Loading
                Loading();
                gameState = State.GameIntro;
                #endregion
            }
            else if (gameState == State.LevelIntro)
            {
                #region Level Intro Update
                GamePadState gamepadStateCurr = GamePad.GetState(PlayerIndex.One);
                if (gamepadStateCurr.IsButtonDown(Buttons.A) || Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    gameEngine.InitalizeLevel(level1, this);
                    gameState = State.Gameplay;
                }
                #endregion
            }
            Vibrate();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            //spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (gameState == State.Menu)
            {
                #region Menu Draw
                spriteBatch.Draw(menuImage, new Vector2(0, 0), Color.White);
                spriteBatch.Draw(menuText, new Vector2((1024 - 351), (768 - 491)), Color.White);
                if (selected == 1)
                {
                    spriteBatch.Draw(t_NewGame, new Vector2(712f, 320f), Color.White);
                    spriteBatch.Draw(t_ButtonA, new Vector2(645, 340), null, Color.White, 0, new Vector2(0, 0), new Vector2(0.6f, 0.6f), SpriteEffects.None, 0);
                }
                if (selected == 2)
                {
                    spriteBatch.Draw(t_Options, new Vector2(705, 436), Color.White);
                    spriteBatch.Draw(t_ButtonA, new Vector2(650, 450), null, Color.White, 0, new Vector2(0, 0), new Vector2(0.6f, 0.6f), SpriteEffects.None, 0);
                }
                if (selected == 3)
                {
                    spriteBatch.Draw(t_Controls, new Vector2(700, 555), Color.White);
                    spriteBatch.Draw(t_ButtonA, new Vector2(625, 575), null, Color.White, 0, new Vector2(0, 0), new Vector2(0.6f, 0.6f), SpriteEffects.None, 0);
                }
                if (selected == 4)
                {
                    spriteBatch.Draw(t_Quit, new Vector2(694, 655), Color.White);
                    spriteBatch.Draw(t_ButtonA, new Vector2(640, 665), null, Color.White, 0, new Vector2(0, 0), new Vector2(0.6f, 0.6f), SpriteEffects.None, 0);
                }
                spriteBatch.Draw(t_BlackScreen, new Vector2(0, 0), new Color(1, 1, 1, (by_BlackCounter / 50f)));
                #endregion
            }
            else if (gameState == State.Controls)
            {
                #region Controls Draw
                spriteBatch.Draw(controlsImage, new Vector2(0, 0), Color.White);
                #endregion
            }
            else if (gameState == State.Options)
            {
                #region Options Draw
                spriteBatch.Draw(optionsImage, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(font, "Tutorial Mode: ", new Vector2(400, 300), text1Colour);
                if (TutorialOn == true)
                {
                    spriteBatch.DrawString(font, "On", new Vector2(580, 300), text1Colour);
                }
                else
                {
                    spriteBatch.DrawString(font, "Off", new Vector2(580, 300), text1Colour);
                }
                spriteBatch.DrawString(font, "Fullscreen: ", new Vector2(400, 330), text2Colour);

                if (graphics.IsFullScreen == true)
                {
                    spriteBatch.DrawString(font, "On", new Vector2(580, 330), text2Colour);
                }
                else
                {
                    spriteBatch.DrawString(font, "Off", new Vector2(580, 330), text2Colour);
                }
                #endregion
            }
            else if (gameState == State.GameIntro)
            {
                #region GameIntro Draw
                spriteBatch.Draw(v_Video1.OutputFrame, new Vector2(0, 0), null, Color.White, 0, new Vector2(0,0),1.6f, SpriteEffects.None, 0.1f);
                spriteBatch.DrawString(font, "Press        +         to skip intro", new Vector2(10, 740), Color.Black);
                spriteBatch.Draw(t_ButtonA, new Vector2(65, 735), null, Color.White, 0, new Vector2(0, 0), new Vector2(0.4f, 0.4f), SpriteEffects.None, 0.1f);
                spriteBatch.Draw(t_ButtonX, new Vector2(117, 735), null, Color.White, 0, new Vector2(0, 0), new Vector2(0.4f, 0.4f), SpriteEffects.None, 0.1f);
                spriteBatch.Draw(t_BlackScreen, new Vector2(0, 0), new Color(1, 1, 1, (by_BlackCounter / 50f)));
                #endregion
            }
            else if (gameState == State.Loading)
            {
                #region Loading Draw
                spriteBatch.Draw(t_Loading, new Vector2(0, 0), Color.White);
                #endregion
            }
            else if (gameState == State.LevelIntro)
            {
                #region Level Intro Draw
                spriteBatch.Draw(level1.t_StartBackground, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(fontBig, "Level 1", new Vector2(10, 620), Color.White);
                spriteBatch.DrawString(font, "Press       to start", new Vector2(855, 730), Color.White);
                spriteBatch.Draw(t_ButtonA, new Vector2(905, 725), null, Color.White, 0, new Vector2(0, 0), new Vector2(0.4f, 0.4f), SpriteEffects.None, 0);
                #endregion
            }
            else if (gameState == State.Gameplay)
            {
                #region Gameplay Draw
                level1.Draw(gameTime, spriteBatch, gameEngine, this);
                gameEngine.Draw(gameTime, spriteBatch, this);
                spriteBatch.Draw(t_BlackScreen, new Vector2(0, 0), new Color(1, 1, 1, (by_BlackCounter / 75f)));
                #endregion
            }
            else if (gameState == State.Tutorial)
            {
                #region Tutorial Draw
                tutorialLevel.Draw(gameTime, spriteBatch, gameEngine, this);
                gameEngine.Draw(gameTime, spriteBatch, this);
                spriteBatch.Draw(t_BlackScreen, new Vector2(0, 0), new Color(1, 1, 1, (by_BlackCounter / 75f)));
                #endregion
            }
            else if (gameState == State.Paused)
            {
                #region Paused Draw
                gameEngine.Draw(gameTime, spriteBatch, this);
                level1.Draw(gameTime, spriteBatch, gameEngine, this);
                spriteBatch.Draw(t_BlackScreen, new Vector2(0, 0), new Color(1, 1, 1, (by_BlackCounter / 75f)));
                #endregion
            }
            if (gameState == State.GameEnd)
            {
                #region GameEnd Draw
                spriteBatch.Draw(t_BlackScreen, new Vector2(0, 0), new Color(1f, 1f, 1f, f_BlackAlpha));
                spriteBatch.Draw(level1.t_WinEndBackground, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(font, "Enemies(Small) Killed: ", new Vector2(100, 500), new Color(1f, 1f, 1f, f_Stats1Alpha));
                spriteBatch.DrawString(font, "Enemies(Big) Killed: ", new Vector2(119, 530), new Color(1f, 1f, 1f, f_Stats2Alpha));
                spriteBatch.DrawString(font, "Fusions: ", new Vector2(218, 560), new Color(1f, 1f, 1f, f_Stats3Alpha));
                spriteBatch.DrawString(font, "Level Complete Bonus: ", new Vector2(85, 590), new Color(1f, 1f, 1f, f_LvlCompleteAlpha));
                spriteBatch.DrawString(font, "Final Score: ", new Vector2(190, 650), new Color(1f, 1f, 1f, f_Stats4Alpha));
                spriteBatch.DrawString(font, "Grade: ", new Vector2(225, 680), new Color(1f, 1f, 1f, f_GradeAlpha));

                spriteBatch.DrawString(font, f_EnemiesSmallKilledTemp.ToString("000") + " x5", new Vector2(350, 500), new Color(1f, 1f, 1f, f_Stats1Alpha));
                spriteBatch.DrawString(font, f_EnemiesBigKilledTemp.ToString("000") + " x10", new Vector2(350, 530), new Color(1f, 1f, 1f, f_Stats2Alpha));
                spriteBatch.DrawString(font, f_FusionsTemp.ToString("000") + " x100", new Vector2(350, 560), new Color(1f, 1f, 1f, f_Stats3Alpha));
                spriteBatch.DrawString(font, f_LevelCompleteBonusTemp.ToString("000"), new Vector2(350, 590), new Color(1f, 1f, 1f, f_LvlCompleteAlpha));
                spriteBatch.DrawString(font, f_ScoreTemp.ToString("00000"), new Vector2(350, 650), new Color(1f, 1f, 1f, f_Stats4Alpha));
                spriteBatch.DrawString(font, s_grade, new Vector2(350, 680), new Color(1f, 1f, 1f, f_GradeAlpha));

                if (i_GameOverSel == 0)
                {
                    spriteBatch.Draw(t_GORetry, new Vector2(660, 475), Color.White);
                }
                else if (i_GameOverSel == 1)
                {
                    spriteBatch.Draw(t_GOContinue, new Vector2(641, 579), Color.White);
                }
                else if (i_GameOverSel == 2)
                {
                    spriteBatch.Draw(t_GOQuit, new Vector2(670, 655), Color.White);
                }

                spriteBatch.Draw(t_BlackScreen, new Vector2(0, 0), new Color(1, 1, 1, (by_BlackCounter / 75f)));
                #endregion
            }
            if (gameState == State.HighScore)
            {
                #region Highscore
                spriteBatch.Draw(t_HighScore, new Vector2(0, 0), Color.White);
                for (int i = 0; i < 10; i++)
                {
                    spriteBatch.DrawString(font, "NO.", new Vector2(559, 229), Color.ForestGreen, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
                    spriteBatch.DrawString(font, "NAME", new Vector2(629, 229), Color.ForestGreen, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
                    spriteBatch.DrawString(font, "SCORE", new Vector2(739, 229), Color.ForestGreen, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
                    spriteBatch.DrawString(font, "GRADE", new Vector2(844, 229), Color.ForestGreen, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
                    spriteBatch.DrawString(font, "NO.", new Vector2(560, 230), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font, "NAME", new Vector2(630, 230), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font, "SCORE", new Vector2(740, 230), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font, "GRADE", new Vector2(845, 230), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);

                    if (i == i_HighScorePosition)
                    {
                        spriteBatch.DrawString(fontName, highscores[i].letter1.ToString(), new Vector2(630, 260 + (i * 40)), Color.Red);
                        spriteBatch.DrawString(fontName, highscores[i].letter2.ToString(), new Vector2(650, 260 + (i * 40)), Color.Red);
                        spriteBatch.DrawString(fontName, highscores[i].letter3.ToString(), new Vector2(670, 260 + (i * 40)), Color.Red);
                        spriteBatch.DrawString(font, (i + 1) + ".", new Vector2(560, 260 + (i * 40)), Color.Red);
                        spriteBatch.DrawString(font, highscores[i].score.ToString("0000"), new Vector2(750, 260 + (i * 40)), Color.Red);
                        spriteBatch.DrawString(font, highscores[i].grade, new Vector2(860, 260 + (i * 40)), Color.Red);
                    }
                    else
                    {
                        spriteBatch.DrawString(fontName, highscores[i].letter1.ToString(), new Vector2(630, 260 + (i * 40)), Color.White);
                        spriteBatch.DrawString(fontName, highscores[i].letter2.ToString(), new Vector2(650, 260 + (i * 40)), Color.White);
                        spriteBatch.DrawString(fontName, highscores[i].letter3.ToString(), new Vector2(670, 260 + (i * 40)), Color.White);
                        spriteBatch.DrawString(font, (i + 1) + ".", new Vector2(560, 260 + (i * 40)), Color.White);
                        spriteBatch.DrawString(font, highscores[i].score.ToString("0000"), new Vector2(750, 260 + (i * 40)), Color.White);
                        spriteBatch.DrawString(font, highscores[i].grade, new Vector2(860, 260 + (i * 40)), Color.White);
                    }
                }
                if (i_UnderScoreTimer < 20 && entering == true)
                {
                    spriteBatch.Draw(t_UnderScore, new Vector2(627 + ((i_WhichLetter - 1) * 20), 290 + (i_HighScorePosition * 40)), Color.Red);
                }
                if (i_UnderScoreTimer == 40)
                {
                    i_UnderScoreTimer = 0;
                }
                else
                {
                    i_UnderScoreTimer++;
                }
                #endregion
            }

            //Draw Fader
            fd_Fader.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        void MouseClick()
        {
            if (ButtonCheck(newGame) == true)
            {
                gameState = State.Gameplay;
            }
            if (ButtonCheck(options) == true)
            {
                gameState = State.Options;
            }
            if (ButtonCheck(controls) == true)
            {
                gameState = State.Controls;
            }
            if (ButtonCheck(quit) == true)
            {
                this.Exit();
            }
            
        }
        bool ButtonCheck(Rectangle button)
        {
            mouseStateCurrent = Mouse.GetState();

            if (mouseStateCurrent.LeftButton == ButtonState.Pressed)
            {
                if (mouseStateCurrent.X >= button.Left && mouseStateCurrent.X <= button.Right && mouseStateCurrent.Y <= button.Bottom && mouseStateCurrent.Y >= button.Top)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        void MenuControlsCheck()
        {
            #region Controller
            GamePadState gamepadStateCurr = GamePad.GetState(PlayerIndex.One);
            if (gamepadStateCurr.ThumbSticks.Left.Y > 0)
            {
                if (!(gamepadStateOld.ThumbSticks.Left.Y > 0))
                {
                    if (selected == 1)
                    {
                        selected = 4;
                    }
                    else
                    {
                        selected--;
                    }
                    soundBank.PlayCue("Select");
                }
            }
            if (gamepadStateCurr.ThumbSticks.Left.Y < 0)
            {
                if (!(gamepadStateOld.ThumbSticks.Left.Y < 0))
                {
                    if (selected == 4)
                    {
                        selected = 1;
                    }
                    else
                    {
                        selected++;
                    }
                    soundBank.PlayCue("Select");
                }
            }

            if (gamepadStateCurr.Buttons.A == ButtonState.Pressed)
            {
                if (selected == 1)
                {
                    b_fading = true;
                }
                else if (selected == 2)
                {
                    gameState = State.Options;
                }
                else if (selected == 3)
                {
                    gameState = State.Controls;
                }
                else if (selected == 4)
                {
                    this.Exit();
                }
                soundBank.PlayCue("Confirm");
            }
            gamepadStateOld = gamepadStateCurr;
            #endregion
            #region Keyboard
            if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (lastUp == false && lastW == false)
                {
                    if (selected == 1)
                    {
                        selected = 4;
                    }
                    else
                    {
                        selected--;
                    }
                    soundBank.PlayCue("Select");
                }
                lastUp = true;
                lastW = true;
            }
            else
            {
                lastW = false;
                lastUp = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (lastS == false && lastDown == false)
                {
                    if (selected == 4)
                    {
                        selected = 1;
                    }
                    else
                    {
                        selected++;
                    }
                    soundBank.PlayCue("Select");
                }
                lastDown = true;
                lastS = true;
            }
            else
            {
                lastDown = false;
                lastS = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (lastEnter == false)
                {
                    if (selected == 1)
                    {
                        b_fading = true;
                    }
                    else if (selected == 2)
                    {
                        gameState = State.Options;
                    }
                    else if (selected == 3)
                    {
                        gameState = State.Controls;
                    }
                    else if (selected == 4)
                    {
                        this.Exit();
                    }
                    soundBank.PlayCue("Confirm");

                    lastEnter = true;
                }

            }
            else
            {
                lastEnter = false;
            }
            #endregion
        }
        void Vibrate()
        {
            if (vibrate > 0)
            {
                GamePad.SetVibration(PlayerIndex.One, (0.3f), (0.3f));
                vibrate--;

            }
        }
        void LevelOver(bool LevelComplete)
        {
            f_Score = ((f_EnemiesBigKilled * 10) + (f_EnemiesSmallKilled * 5) + (gameEngine.f_Fusions * 100) + f_LevelCompleteBonus);
            Grade(f_Score);
            if (f_BlackAlpha < 1)
            {
                f_BlackAlpha += 0.02f;
            }
            else if (f_Stats1Alpha < 1)
            {
                f_Stats1Alpha += 0.02f;
            }
            else if (f_EnemiesSmallKilled != f_EnemiesSmallKilledTemp)
            {
                //Assign blip to blipCue
                Cue blipCue = soundBank.GetCue("blip");

                blipCue.SetVariable("Pitch", f_EnemiesSmallKilledTemp);

                if((f_EnemiesSmallKilled - f_EnemiesSmallKilledTemp) % 4 == 0)
                {
                    blipCue.Play();
                }
                f_EnemiesSmallKilledTemp++;
            }
            else if (f_Stats2Alpha < 1)
            {
                if (f_Stats2Alpha == 0)
                {
                    soundBank.PlayCue("bell");
                }
                f_Stats2Alpha += 0.02f;
            }
            else if (f_EnemiesBigKilled != f_EnemiesBigKilledTemp)
            {
                //asign blip to blipCue
                Cue blipCue = soundBank.GetCue("blip");

                blipCue.SetVariable("Pitch", f_EnemiesBigKilledTemp);

                if ((f_EnemiesBigKilled - f_EnemiesBigKilledTemp) % 4 == 0)
                {
                    blipCue.Play();
                }
                f_EnemiesBigKilledTemp++;
            }
            else if (f_Stats3Alpha < 1)
            {
                if (f_Stats3Alpha == 0)
                {
                    soundBank.PlayCue("bell");
                }
                f_Stats3Alpha += 0.02f;
            }
            else if (gameEngine.f_Fusions != f_FusionsTemp)
            {
                //asign blip to blipCue
                Cue blipCue = soundBank.GetCue("blip");

                blipCue.SetVariable("Pitch", f_FusionsTemp);

                if ((gameEngine.f_Fusions - f_FusionsTemp) % 4 == 0)
                {
                    blipCue.Play();
                }
                f_FusionsTemp++;
            }
            else if (f_LvlCompleteAlpha < 1)
            {
                if (f_LvlCompleteAlpha == 0)
                {
                    soundBank.PlayCue("bell");
                }
                f_LvlCompleteAlpha += 0.02f;
            }
            else if (f_LevelCompleteBonus != f_LevelCompleteBonusTemp)
            {
                //asign blip to blipCue
                Cue blipCue = soundBank.GetCue("blip");

                blipCue.SetVariable("Pitch", (f_LevelCompleteBonusTemp / 10));

                if ((i_SoundCounter2 % 2) == 0)
                {
                    blipCue.Play();
                }
                f_LevelCompleteBonusTemp += 10;
                if (f_LevelCompleteBonus < f_LevelCompleteBonusTemp)
                {
                    f_LevelCompleteBonusTemp = f_LevelCompleteBonus;
                }
                i_SoundCounter2++;
            }
            else if (f_Stats4Alpha < 1)
            {
                if (f_Stats4Alpha == 0)
                {
                    soundBank.PlayCue("bell");
                }
                f_Stats4Alpha += 0.02f;
            }
            else if (f_Score > f_ScoreTemp)
            {
                //asign blip to blipCue
                Cue blipCue = soundBank.GetCue("blip");

                blipCue.SetVariable("Pitch", (f_ScoreTemp/20));

                if ((i_SoundCounter % 4) == 0)
                {
                    blipCue.Play();
                }
                f_ScoreTemp += 20;
                if (f_Score < f_ScoreTemp)
                {
                    f_ScoreTemp = f_Score;
                } 
                i_SoundCounter++;
            }
            else if (f_GradeAlpha < 1)
            {
                if (f_GradeAlpha == 0)
                {
                    soundBank.PlayCue("bell");
                }
                f_GradeAlpha += 0.02f;
            }
            else if (f_ChoicesAlpha < 1)
            {
                if (f_ChoicesAlpha == 0)
                {
                    soundBank.PlayCue("bell");
                }
                f_ChoicesAlpha += 0.02f;
            }
            else
            {
                #region Controller
                GamePadState gamepadStateCurr = GamePad.GetState(PlayerIndex.One);
                if (gamepadStateCurr.ThumbSticks.Left.Y > 0)
                {
                    if (!(gamepadStateOld.ThumbSticks.Left.Y > 0))
                    {
                        soundBank.PlayCue("Select");
                        if (LevelComplete == false)
                        {
                            if (i_GameOverSel == 0)
                            {
                                i_GameOverSel = 2;
                            }
                            else
                            {
                                i_GameOverSel--;
                            }
                        }
                        else if (LevelComplete == true)
                        {
                            if (i_GameOverSel == 1)
                            {
                                i_GameOverSel = 2;
                            }
                            else
                            {
                                i_GameOverSel--;
                            }
                        }
                    }
                }
                if (gamepadStateCurr.ThumbSticks.Left.Y < 0)
                {
                    if (!(gamepadStateOld.ThumbSticks.Left.Y < 0))
                    {
                        soundBank.PlayCue("Select");
                        if (LevelComplete == false)
                        {
                            if (i_GameOverSel == 2)
                            {
                                i_GameOverSel = 0;
                            }
                            else
                            {
                                i_GameOverSel++;
                            }
                        }
                        else if (LevelComplete == true)
                        {
                            if (i_GameOverSel == 2)
                            {
                                i_GameOverSel = 1;
                            }
                            else
                            {
                                i_GameOverSel++;
                            }
                        }
                    }
                }
                if (gamepadStateCurr.IsButtonDown(Buttons.A))
                {
                    if (!(gamepadStateOld.IsButtonDown(Buttons.A)))
                    {
                        if (i_GameOverSel == 0)
                        {
                            gameEngine.InitalizeLevel(level1, this);
                            Reset();
                            gameState = State.Gameplay;
                        }
                        else if (i_GameOverSel == 2)
                        {
                            Reset();
                            gameState = State.Menu;
                        }
                        soundBank.PlayCue("Confirm");
                        GameOver.Pause();
                    }
                }
                gamepadStateOld = gamepadStateCurr;
                #endregion
                #region Keyboard
            if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (lastUp == false && lastW == false)
                {
                    soundBank.PlayCue("Select");
                    if (LevelComplete == false)
                    {
                        if (i_GameOverSel == 0)
                        {
                            i_GameOverSel = 2;
                        }
                        else
                        {
                            i_GameOverSel--;
                        }
                    }
                    else if (LevelComplete == true)
                    {
                        if (i_GameOverSel == 1)
                        {
                            i_GameOverSel = 2;
                        }
                        else
                        {
                            i_GameOverSel--;
                        }
                    }
                }
                lastUp = true;
                lastW = true;
            }
            else
            {
                lastW = false;
                lastUp = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (lastS == false && lastDown == false)
                {
                    soundBank.PlayCue("Select");
                    if (LevelComplete == false)
                    {
                        if (i_GameOverSel == 2)
                        {
                            i_GameOverSel = 0;
                        }
                        else
                        {
                            i_GameOverSel++;
                        }
                    }
                    else if (LevelComplete == true)
                    {
                        if (i_GameOverSel == 2)
                        {
                            i_GameOverSel = 1;
                        }
                        else
                        {
                            i_GameOverSel++;
                        }
                    }
                }
                lastDown = true;
                lastS = true;
            }
            else
            {
                lastDown = false;
                lastS = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (lastEnter == false)
                {
                    if (i_GameOverSel == 0)
                    {
                        gameEngine.InitalizeLevel(level1, this);
                        Reset();
                        gameState = State.Gameplay;
                    }
                    else if (i_GameOverSel == 2)
                    {
                        Reset();
                        gameState = State.Menu;
                    }
                    soundBank.PlayCue("Confirm");
                    GameOver.Pause();
                    lastEnter = true;
                }

            }
            else
            {
                lastEnter = false;
            }
#endregion
            }
        }
        void Grade(float score)
        {
            float perc = 0;

            //i_TopScore set globally, this code will then work out your score
            //as a percentage of i_TopScore and set your grade accordingly
            perc = ((score / i_TopScore) * 100);

            if (perc < 12.5f)
            {
                s_grade = "F-";
            }
            else if (perc < 25)
            {
                s_grade = "F";
            }
            else if (perc < 37.5f)
            {
                s_grade = "E";
            }
            else if (perc < 50)
            {
                s_grade = "D";
            }
            else if (perc < 62.5f)
            {
                s_grade = "C";
            }
            else if (perc < 75)
            {
                s_grade = "B";
            }
            else if (perc < 87.5f)
            {
                s_grade = "A";
            }
            else
            {
                s_grade = "A+";
            }
        }
        void Reset()
        {
            //segments = 8;
            //b_Pulse = true;
            //b_Pulsing = false;
            //b_LevelComplete = false;
            //i_GridCounter = 0;
            //i_TutorialCounter = 0;
            //i_ShipGridCounter = 0;
            //i_Number1Counter = 0;
            //i_Number2Counter = 0;
            //i_Number3Counter = 0;
            //i_Number4Counter = 0;
            //i_Number5Counter = 0;
            //i_TutorialTextCounter = 0;
            //i_ShipCounter = 0;
            //b_Grid = true;
            //b_shield = false;
            //b_fading = false;
            //tutBulletCounter = 0;
            //f_QuizScore = 0;
            //f_TotalScore = 0;
            //i_LetterPos = 0;
            //i_HighScorePosition = 11;
            //i_WhichLetter = 1;
            //i_NewScore = 0;
            //i_UnderScoreTimer = 0;
            //s_NewGrade = "";
            //dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 00);
            //iTutorialState = 0;
            //b_TutorialFusion = false;
            //entering = false;
            //dt_timer = new DateTime(1901, 1, 1, 0, 5, 00);
            //i_SoundCounter2 = 0;
            //i_SoundCounter = 0;
            //vibrate = 0;
            //v_ShieldSize = new Vector2(1.20f, 1.20f);
            //i_ShieldPulseCounter = 0;
            //i_shipAlpha = 0.5f;
            //i_PulseRate = 60;
            //i_ExplosionTimer = 0;
            //dead = false;
            //ship.ResetShip(500, 350);
            //offset = new Vector2(0, 0);
            //f_BlackAlpha = 0;
            //f_Stats1Alpha = 0;
            //f_Stats2Alpha = 0;
            //f_Stats3Alpha = 0;
            //f_Stats4Alpha = 0;
            //f_ChoicesAlpha = 0;
            //i_GameOverSel = 0;
            //f_Score = 0;
            //f_EnemiesBigKilled = 0;
            //f_EnemiesSmallKilled = 0;
            //f_Fusions = 0;
            //f_FusionsTemp = 0;
            //f_EnemiesSmallKilledTemp = 0;
            //f_EnemiesBigKilledTemp = 0;
            //f_ScoreTemp = 0;
            //s_grade = "n/a";
            //f_GradeAlpha = 0;
            //f_LevelCompleteBonus = 0;
            //f_LevelCompleteBonusTemp = 0;
            //f_LvlCompleteAlpha = 0;

            //ship = null;
            //enemies = null;
            //Photons = null;
            //Chlor = null;
            //Fused = null;
            //bullets = null;
            //tethers = null;

        }
        void Loading()
        {
            t_Tick = Content.Load<Texture2D>("MenusBGrounds//Tick");
            t_Cross = Content.Load<Texture2D>("MenusBGrounds//Cross");
            t_UnderScore = Content.Load<Texture2D>("MenusBGrounds//Underscore");
            t_HighScore = Content.Load<Texture2D>("MenusBGrounds//High score screen");
            t_GOContinue = Content.Load<Texture2D>("MenusBGrounds//continue");
            t_GOQuit = Content.Load<Texture2D>("MenusBGrounds//Game_over_quit");
            t_GORetry = Content.Load<Texture2D>("MenusBGrounds//retry");
            t_Pixel = Content.Load<Texture2D>("mightypixel");

            v_Video1 = new VideoPlayer(@"intro.avi", GraphicsDevice);
            v_Video1.Play();
            MenuMusic.Pause();
        }

        public static Random Random
        {
            get { return random; }
        }
        public static float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        private void KeyboardTextInput()
        {
            oldKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            Keys[] pressedKeys;
            pressedKeys = currentKeyboardState.GetPressedKeys();

            foreach (Keys key in pressedKeys)
            {
                if (oldKeyboardState.IsKeyUp(key))
                {
                    if (key == Keys.Back) // overflows
                        s_Input = s_Input.Remove(s_Input.Length - 1, 1);
                    else
                        if (key == Keys.Space)
                            s_Input = s_Input.Insert(s_Input.Length, " ");
                        else
                            s_Input += key.ToString();
                }
            }
        }
        public void HighScoreInput()
        {
            #region Controller
            GamePadState gamepadStateCurr = GamePad.GetState(PlayerIndex.One);
            if (gamepadStateCurr.ThumbSticks.Left.Y < 0)
            {
                if (!(gamepadStateOld.ThumbSticks.Left.Y < 0))
                {
                    if (i_LetterPos == 25)
                    {
                        i_LetterPos = 0;
                    }
                    else
                    {
                        i_LetterPos++;
                    }
                    soundBank.PlayCue("Select");
                }
            }
            if (gamepadStateCurr.ThumbSticks.Left.Y > 0)
            {
                if (!(gamepadStateOld.ThumbSticks.Left.Y > 0))
                {
                    if (i_LetterPos == 0)
                    {
                        i_LetterPos = 25;
                    }
                    else
                    {
                        i_LetterPos--;
                    }
                    soundBank.PlayCue("Select");
                }
            }
            if (gamepadStateCurr.IsButtonDown(Buttons.A))
            {
                if (!(gamepadStateOld.IsButtonDown(Buttons.A)))
                {
                    if (i_WhichLetter < 3)
                    {
                        i_WhichLetter++;
                        i_LetterPos = 0;
                        i_UnderScoreTimer = 0;
                    }
                    else if (entering == false)
                    {
                        Reset();
                        i_WhichLetter = 1;
                        i_LetterPos = 0;
                        gameState = State.Menu;
                    }
                    else
                    {
                        entering = false;
                    }
                    soundBank.PlayCue("Confirm");
                }
            }
            gamepadStateOld = gamepadStateCurr;
            #endregion
            #region Keyboard
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (lastUp == false)
                {
                    if (i_LetterPos == 0)
                    {
                        i_LetterPos = 25;
                    }
                    else
                    {
                        i_LetterPos--;
                    }
                    soundBank.PlayCue("Select");
                    lastUp = true;
                }
            }
            else
            {
                lastUp = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (lastDown == false)
                {
                    if (i_LetterPos == 25)
                    {
                        i_LetterPos = 0;
                    }
                    else
                    {
                        i_LetterPos++;
                    }
                    soundBank.PlayCue("Select");
                    lastDown = true;
                }
            }
            else
            {
                lastDown = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (lastEnter == false)
                {
                    if (i_WhichLetter < 3)
                    {
                        i_WhichLetter++;
                        i_LetterPos = 0;
                        i_UnderScoreTimer = 0;
                    }
                    else if (entering == false)
                    {
                        Reset();
                        i_WhichLetter = 1;
                        i_LetterPos = 0;
                        gameState = State.Menu;
                    }
                    else
                    {
                        entering = false;
                    }
                    soundBank.PlayCue("Confirm");
                    lastEnter = true;
                }
            }
            else
            {
                lastEnter = false;
            }
            #endregion

        }
        public char NumberToLetter(char letter)
        {
            if (i_LetterPos == 0)
            {
                return 'A';
            }
            else if (i_LetterPos == 1)
            {
                return 'B';
            }
            else if (i_LetterPos == 2)
            {
                return 'C';
            }
            else if (i_LetterPos == 3)
            {
                return 'D';
            }
            else if (i_LetterPos == 4)
            {
                return 'E';
            }
            else if (i_LetterPos == 5)
            {
                return 'F';
            }
            else if (i_LetterPos == 6)
            {
                return 'G';
            }
            else if (i_LetterPos == 7)
            {
                return 'H';
            }
            else if (i_LetterPos == 8)
            {
                return 'I';
            }
            else if (i_LetterPos == 9)
            {
                return 'J';
            }
            else if (i_LetterPos == 10)
            {
                return 'K';
            }
            else if (i_LetterPos == 11)
            {
                return 'L';
            }
            else if (i_LetterPos == 12)
            {
                return 'M';
            }
            else if (i_LetterPos == 13)
            {
                return 'N';
            }
            else if (i_LetterPos == 14)
            {
                return 'O';
            }
            else if (i_LetterPos == 15)
            {
                return 'P';
            }
            else if (i_LetterPos == 16)
            {
                return 'Q';
            }
            else if (i_LetterPos == 17)
            {
                return 'R';
            }
            else if (i_LetterPos == 18)
            {
                return 'S';
            }
            else if (i_LetterPos == 19)
            {
                return 'T';
            }
            else if (i_LetterPos == 20)
            {
                return 'U';
            }
            else if (i_LetterPos == 21)
            {
                return 'V';
            }
            else if (i_LetterPos == 22)
            {
                return 'W';
            }
            else if (i_LetterPos == 23)
            {
                return 'X';
            }
            else if (i_LetterPos == 24)
            {
                return 'Y';
            }
            else if (i_LetterPos == 25)
            {
                return 'Z';
            }
            else
            {
                return ' ';
            }

        }
        public void UpdateHighScoreTable()
        {
            for (int i = 0; i < 10; i++)
            {
                if (highscores[i].score < i_NewScore)
                {
                    i_HighScorePosition = i;
                    entering = true;
                    for (int j = 9; j > -1; j--)
                    {
                        if (j == i_HighScorePosition)
                        {
                            highscores[j].score = i_NewScore;
                            highscores[j].grade = s_NewGrade;
                            break;
                        }
                        else
                        {
                            highscores[j].letter1 = highscores[j - 1].letter1;
                            highscores[j].letter2 = highscores[j - 1].letter2;
                            highscores[j].letter3 = highscores[j - 1].letter3;
                            highscores[j].score = highscores[j - 1].score;
                            highscores[j].grade = highscores[j - 1].grade;
                        }
                    }
                    break;
                }
            }
        }

    }
}
