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
        Quiz = 6,
        GameEnd = 7,
        Options = 8,
        Paused = 9,
        HighScore = 10,
        Tutorial = 11,
        Loading = 12
    }
    public enum TutorialState
    {
        Begin = 0,
        Movement = 1,
        Photon = 2,
        Tether = 3,
        Chloro = 4,
        Fusion = 5,
        Turret = 6,
        EnemyS = 7,
        EnemyB = 8,
        End = 9
    }
    public enum TetherState
    {
        shooting,
        tethered,
        detethered
    }
    public struct Highscore
    {
        public char letter1;
        public char letter2;
        public char letter3;
        public int score;
        public string grade;
    }
    public struct Questions
    {
        public string Question;
        public string[] answers;
        public int correctAns;
        public int youranswer;
    }
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        #region Global Variables
        bool b_LevelComplete = false;
        bool b_Flash = false;
        int i_WhichFlash = -1;
        int i_FlashTimer = 0;
        VideoPlayer v_Video1;
        int i_LetterPos = 0;
        int i_WhichLetter = 1;
        int i_NewScore = 0;
        int i_UnderScoreTimer = 0;
        string s_NewGrade = "";
        DateTime dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 00);
        int iTutorialState = 0;
        bool b_TutorialFusion = false;
        int i_CurrentQuestion = 0;
        int i_CorrectAnswers = 0;
        bool entering = false;
        int i_HighScorePosition = 11;
        int i_TextBubble = 1;
        int i_StupidLineMax = 20;
        bool b_Pulse = true;
        bool b_Pulsing = false;
        int segments = 8;

        float funnyNo1 = 0;
        float funnyNo2 = 0;
        float funnyNo3 = 0;
        float funnyNo4 = 0;
        int funnyNoCounter1 = 0;
        int funnyNoCounter2 = 0;
        int funnyNoCounter3 = 0;
        int funnyNoCounter4 = 0;
        int i_Number1Counter = 0;
        int i_Number2Counter = 0;
        int i_Number3Counter = 0;
        int i_Number4Counter = 0;
        int i_Number5Counter = 0;
        int i_TutorialTextCounter = 0;
        int i_ShipCounter = 0;
        bool b_start = true;
        bool b_Grid = true;
        bool b_shield = false;
        bool b_fading = false;
        State gameState = State.Menu;

        // Audio objects
        AudioEngine engine;
        SoundBank soundBank;
        WaveBank waveBank;
        Cue MenuMusic;
        Cue Ambient;
        Cue GameOver;
        Cue[] tutCues;
        Cue TutMusic;
   
        //textures
        Texture2D menuImage;
        Texture2D controlsImage;
        Texture2D optionsImage;
        Texture2D t_NewGame;
        Texture2D t_Options;
        Texture2D t_Controls;
        Texture2D t_Quit;
        Texture2D t_Ship;
        Texture2D t_Level1;
        Texture2D t_Tutorial;
        Texture2D t_Photon;
        Texture2D t_Chlor;
        Texture2D t_Fused;
        Texture2D t_EnemySmall;
        Texture2D t_EnemyBig;
        Texture2D t_Pixel;
        Texture2D t_Bullet;
        Texture2D t_Tether;
        Texture2D t_BlackScreen;
        Texture2D t_UnderScore;
        Texture2D t_HighScore;
        Texture2D t_LevelEndScreen;
        Texture2D[] t_TutorialText;
        Texture2D t_GORetry;
        Texture2D t_GOContinue;
        Texture2D t_GOQuit;
        Texture2D t_ButtonA;
        Texture2D t_ButtonB;
        Texture2D t_ButtonX;
        Texture2D t_ButtonY;
        Texture2D t_Loading;
        Texture2D t_Level1Start;
        Texture2D t_Tick;
        Texture2D t_Cross;
        Texture2D t_Answers;
        Texture2D t_Questions;
        Texture2D t_Grid;
        Texture2D t_ShipGrid;
        Texture2D t_TutorialOverlay;
        Texture2D t_ClockBase;
        Texture2D[] t_ClockSegFill;
        Texture2D t_Crosshair;

        float i_GridCounter = 0;
        int i_TutorialCounter = 0;
        int i_ShipGridCounter = 0;

        //Level shiz
        Texture2D t_LevelBounds;
        Color[] levelBoundsData;
        Rectangle levelTop = new Rectangle(508, 0, 2900, 306);
        Rectangle levelBottom = new Rectangle(508, 1916, 2900, 388);
        Rectangle levelLeft = new Rectangle(0, 0, 508, 2304);
        Rectangle levelRight = new Rectangle(3408, 0, 688, 2304);


        //class objects
        Ship ship;
        Enemy[] enemies;
        Particle[] Photons;
        Particle[] Chlor;
        Particle[] Fused;
        Bullet[] bullets;
        Bullet[] tethers;
        Bullet[] stupidline;
        Highscore[] highscores;
        Questions[] questions;

        //menu
        Rectangle newGame;
        Rectangle controls;
        Rectangle options;
        Rectangle quit;
        
        //misc
        MouseState mouseStateCurrent;
        GamePadState gamepadStateOld;
        KeyboardState keyboardStateOld;
        int selected = 1;
        int vibrateCounter = 0;
        Vector2 cursorPos = Vector2.Zero;
        Vector2 particleOffset = Vector2.Zero;
        Vector2 offset;
        SpriteFont font;
        SpriteFont fontBig;
        SpriteFont fontTimer;
        SpriteFont fontName;
        SpriteFont fontQuestion;
        SpriteFont fontSmallText;
        GamePadState gamepadStateCurr;
        int i_BulletMax = 1000;
        Vector2 v_TurretDirection = new Vector2(0, 1);
        bool dead = false;
        bool lastLTrigger = false;
        bool lastEnter = false;
        bool lastDown = false;
        bool lastUp = false;
        bool lastS = false;
        bool lastW = false;
        bool last1 = false;
        bool last2 = false;
        bool last3 = false;
        bool last4 = false;
        bool lastRMouse = false;
        bool lastEscape = false;
        bool TutorialOn = true;
        float f_BlackAlpha = 0;
        float f_Stats1Alpha = 0;
        float f_Stats2Alpha = 0;
        float f_Stats3Alpha = 0;
        float f_Stats4Alpha = 0;
        float f_ChoicesAlpha = 0;
        float f_LvlCompleteAlpha = 0;
        float f_GradeAlpha = 0;
        int i_GameOverSel = 0;
        string s_grade = "N/A";
        string s_Input = "Test";

        Color text1Colour = Color.Red;
        Color text2Colour = Color.White;
        int OptionsSelected = 1;
        bool justStarted = false;


        TetherState TetherState = TetherState.shooting;
  
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState oldKeyboardState;
        KeyboardState currentKeyboardState;

        //Particle Effect Variables
        private static Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }
        const float TimeBetweenSmokePlumePuffs = .5f;
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }
        EngineParticleSystem engineSmoke;
        ExplosionParticleSystem shipExplosion;
        Texture2D engineSmokeT;
        Texture2D menuText;
        int vibrate = 0;
        Texture2D t_Shield;
        Vector2 v_ShieldSize = new Vector2(1.20f, 1.20f);
        int i_ShieldPulseCounter = 0;
        float i_shipAlpha = 0.5f;
        int i_PulseRate = 60;
        int i_ExplosionTimer = 0;

        //Level Variables
        int i_EnemySpawnRate;
        int i_MaxNumberEnemies;
        int i_PhotonSpawnRate;
        int i_MaxNumberPhotons;
        int i_ChloroSpawnRate;
        int i_MaxNumberChloro;
        int i_MaxNumberFused;

        int i_FireRate = 7;
        int i_TopScore = 4000;
        Rectangle levelBounds;
        float f_Friction = 0.987f;
        float edgeDamper = 0.7f;
        DateTime dt_timer = new DateTime(1901, 1, 1, 0, 5, 00);
        int i_TargetFusions = 15;
        int i_SoundCounter = 0;
        int i_SoundCounter2 = 0;
        byte i_BlackCounter = 0;

        //Scoring
        float f_Score = 0;
        float f_QuizScore = 0;
        float f_TotalScore = 0;
        float f_EnemiesBigKilled = 0;
        float f_EnemiesSmallKilled = 0;
        float f_Fushions = 0;
        float f_LevelCompleteBonus = 0;

        float f_ScoreTemp = 0;
        float f_EnemiesBigKilledTemp = 0;
        float f_EnemiesSmallKilledTemp = 0;
        float f_FushionsTemp = 0;
        float f_LevelCompleteBonusTemp = 0;

        //Tutorial Stuffs
        Vector2 totalPixelsTravelled;
        Vector2 lastShipPos;
        int tutBulletCounter = 0;

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
            Ambient = soundBank.GetCue("ambient");
            TutMusic = soundBank.GetCue("tutMusic");
            GameOver = soundBank.GetCue("gameOver");

            newGame = new Rectangle(350, 500, 100, 20);
            options = new Rectangle(350, 520, 80, 20);
            controls = new Rectangle(350, 540, 80, 20);
            quit = new Rectangle(350, 560, 60, 20);
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
            t_LevelBounds = Content.Load<Texture2D>("levelBounds");
            t_Cross = Content.Load<Texture2D>("MenusBGrounds//Cross");
            t_Answers = Content.Load<Texture2D>("MenusBGrounds//Answers");
            t_Questions = Content.Load<Texture2D>("MenusBGrounds//Questions");
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
            levelBoundsData = new Color[t_LevelBounds.Width * t_LevelBounds.Height];
            t_LevelBounds.GetData(levelBoundsData);
            highscores = new Highscore[10];
            for (int i = 0; i < 10; i++)
            {
                highscores[i].letter1 = 'A';
                highscores[i].letter2 = 'A';
                highscores[i].letter3 = 'A';
                highscores[i].score = ((10 - i) * 100);
                highscores[i].grade = "N/A";
            }
            questions = new Questions[5];
            for (int i = 0; i < 5; i++)
            {
                if (i == 0)
                {
                    questions[i].Question = "Why do plants perform photosynthesis?";
                    questions[i].answers = new string[4];
                    questions[i].answers[0] = "To lower temperature";
                    questions[i].answers[1] = "To get rid of toxins";
                    questions[i].answers[2] = "To make their own food";
                    questions[i].answers[3] = "To give more oxygen";
                    questions[i].correctAns = 2;
                }
                if (i == 1)
                {
                    questions[i].Question = "What are the two products of photosynthesis?";
                    questions[i].answers = new string[4];
                    questions[i].answers[0] = "Glucose and Water";
                    questions[i].answers[1] = "Glucose and Oxygen";
                    questions[i].answers[2] = "Carbon Dioxide and Water";
                    questions[i].answers[3] = "Carbon Dioxide and Oxygen";
                    questions[i].correctAns = 1;
                }
                if (i == 2)
                {
                    questions[i].Question = "What is glucose NOT used for?";
                    questions[i].answers = new string[4];
                    questions[i].answers[0] = "Being turned into starch";
                    questions[i].answers[1] = "To make amino acids";
                    questions[i].answers[2] = "Being turned into lipids";
                    questions[i].answers[3] = "For protection";
                    questions[i].correctAns = 3;
                }
                if (i == 3)
                {
                    questions[i].Question = "What is the green pigment in Chloroplasts called?";
                    questions[i].answers = new string[4];
                    questions[i].answers[0] = "Chlorophyll";
                    questions[i].answers[1] = "Chlorine";
                    questions[i].answers[2] = "Chloride";
                    questions[i].answers[3] = "Carbohydrate";
                    questions[i].correctAns = 0;
                }
                if (i == 4)
                {
                    questions[i].Question = "What is the main requirement of Photosynthesis?";
                    questions[i].answers = new string[4];
                    questions[i].answers[0] = "Carbohydrates";
                    questions[i].answers[1] = "Rainwater";
                    questions[i].answers[2] = "Other Plants";
                    questions[i].answers[3] = "Sufficient Light";
                    questions[i].correctAns = 3;
                }
            }
        }
        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            // Update the audio engine.
            engine.Update();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (gameState == State.Menu)
            {
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
                    if (i_BlackCounter == 50)
                    {
                        gameState = State.Loading;
                        i_BlackCounter = 0;
                        b_fading = false;
                    }
                    else
                    {
                        i_BlackCounter++;
                        MenuMusic.SetVariable("Volume", (100 - (i_BlackCounter * 2)));
                    }
                }
                else
                {
                    MenuControlsCheck();
                }
            }
            else if (gameState == State.GameIntro)
            {
                if ((GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed && GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    v_Video1.Stop();
                    b_fading = true;
                    i_BlackCounter = 0;
                }

                if (b_fading == true)
                {
                    if (i_BlackCounter < 50)
                    {
                        i_BlackCounter++;
                    }
                    else
                    {
                        v_Video1.Dispose();
                        i_BlackCounter = 0;
                        b_fading = false;

                        if (TutorialOn == true)
                        {
                            InitalizeLevel(0);
                            gameState = State.Tutorial;
                        }
                        else if (TutorialOn == false)
                        {
                            i_ShipCounter = 75;
                            InitalizeLevel(0);
                            InitalizeLevel(1);
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
            }
            else if (gameState == State.Gameplay)
            {
                if (b_start == true)
                {
                    if (i_BlackCounter > 0)
                    {
                        i_BlackCounter--;
                    }
                    else
                    {
                        b_start = false;
                    }
                }
                GamePlay(gameTime);
                SegmentUpdate();
                if (Ambient.IsPlaying == false)
                {
                    Ambient.Play();
                }
                else if (Ambient.IsPaused == true)
                {
                    Ambient.Resume();
                }
                if (f_Fushions >= i_TargetFusions)
                {
                    t_LevelEndScreen = Content.Load<Texture2D>("MenusBGrounds//Level_complete");
                    b_LevelComplete = true;
                    i_GameOverSel = 1;
                    f_LevelCompleteBonus = 500;
                    if (i_BlackCounter == 75)
                    {
                        soundBank.PlayCue("Victory");
                        gameState = State.GameEnd;
                        Ambient.Pause();
                    }
                    else
                    {
                        i_BlackCounter++;
                    }
                }
                if (dt_timer.Minute == 0 && dt_timer.Second == 0)
                {
                    t_LevelEndScreen = Content.Load<Texture2D>("MenusBGrounds//TimesUpScreen");
                    if (i_BlackCounter == 75)
                    {
                        gameState = State.GameEnd;
                        Ambient.Pause();
                    }
                    else
                    {
                        i_BlackCounter++;
                    }
                }
                else
                {
                    dt_timer -= gameTime.ElapsedGameTime;
                }
                offset = ship.OffsetUpdate(offset);
                if (dead == true)
                {
                    UpdateExplosion(ship.Position);
                }
                #region Enemy Spawn and Update
                for (int i = 0; i < i_MaxNumberEnemies; i++)
                {
                    if (enemies[i].EnemyCollision == true)
                    {
                        UpdateShieldSpark(enemies[i].EnemyCollisionPosition, enemies[i]);
                        enemies[i].ShieldSparkCounter++;
                        if (enemies[i].ShieldSparkCounter == 40)
                        {
                            enemies[i].EnemyCollision = false;
                            enemies[i].ShieldSparkCounter = 0;
                        }
                    }

                    if (enemies[i].IsParticleKill == true)
                    {
                        UpdateParticleKill(enemies[i].ParticleKillPosition, enemies[i].p_ParticleKilled, enemies[i]);
                        enemies[i].ParticleKillCounter++;
                        if (enemies[i].ParticleKillCounter == 40)
                        {
                            enemies[i].IsParticleKill = false;
                            enemies[i].ParticleKillCounter = 0;
                        }
                    }



                    if (enemies[i].Alive == true)
                    {
                        enemies[i].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction);
                    }
                    else if (enemies[i].SpawnTimer == i_EnemySpawnRate)
                    {
                        enemies[i] = new Enemy(this);
                        enemies[i].LoadTex(t_EnemySmall, t_EnemyBig);
                        enemies[i].Spawn(offset, levelBounds);
                        enemies[i].SpawnTimer = 0;
                        enemies[i].Rectangle = new Rectangle(((int)enemies[i].Position.X - (enemies[i].Texture.Width / 2)), ((int)enemies[i].Position.Y - (enemies[i].Texture.Height / 2)), enemies[i].Texture.Width, enemies[i].Texture.Height);
                        enemies[i].TextureData = new Color[enemies[i].Texture.Width * enemies[i].Texture.Height];
                        enemies[i].Texture.GetData(enemies[i].TextureData);
                    }
                }
                for (int i = 0; i < i_MaxNumberEnemies; i++)
                {
                    if (enemies[i].Alive == false)
                    {
                        enemies[i].SpawnTimer++;
                        break;
                    }
                }
                #endregion
                #region Particles Spawning

                for (int i = 0; i < Photons.Length; i++)
                {
                    if (Photons[i].ParticleState == Particle.State.Fusing)
                    {
                        if (Photons[i].i_Fusing > 50)
                        {
                            Photons[i].ParticleState = Particle.State.Dead;
                            Photons[i].i_Fusing = 0;
                        }
                        else
                        {
                            Photons[i].i_Fusing++;
                        }
                    }
                    else if (Photons[i].ParticleState == Particle.State.Colliding)
                    {
                        Photons[i].Velocity = new Vector2((((Photons[i].FusionPosition.X - Photons[i].Position.X) / Vector2.Distance(Photons[i].Position, Photons[i].FusionPosition)) * 10), (((Photons[i].FusionPosition.Y - Photons[i].Position.Y) / Vector2.Distance(Photons[i].Position, Photons[i].FusionPosition)) * 10));
                        Photons[i].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction);
                        if (Photons[i].Position.X < (Photons[i].FusionPosition.X + 1) && Photons[i].Position.X > (Photons[i].FusionPosition.X - 1) && Photons[i].Position.Y < (Photons[i].FusionPosition.Y + 1) && Photons[i].Position.Y > (Photons[i].FusionPosition.Y - 1))
                        {
                            for (int j = 0; j < i_MaxNumberFused; j++)
                            {
                                if (Fused[j].ParticleState == Particle.State.Dead)
                                {
                                    Fused[j].ParticleState = Particle.State.Spawning;
                                    Photons[i].ParticleState = Particle.State.Fusing;
                                    Fused[j].Position = Photons[i].Position;
                                    break;
                                }
                            }
                        }
                    }
                    else if (Photons[i].ParticleState == Particle.State.Alive)
                    {
                        if (Photons[i].IsTethered)
                        {
                            if (!ship.BoundSphere.Intersects(new BoundingSphere(new Vector3(Photons[i].Position, 0), (Photons[i].Texture.Width / 2))))
                            {
                                Vector2 distance = ship.Position - Photons[i].Position;
                                Photons[i].Velocity += Vector2.Normalize(distance) * 20;
                            }
                        }
                        else
                        {
                            Photons[i].Position -= new Vector2(0, 0.07f);
                        }
                        Photons[i].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction);
                    }
                    else if (Photons[i].ParticleState == Particle.State.Spawning)
                    {
                        if (Photons[i].Scale > 1)
                        {
                            Photons[i].Scale = 1.0f;
                            Photons[i].ParticleState = Particle.State.Alive;
                        }
                        else
                        {
                            Photons[i].Scale += 0.02f;
                        }
                    }
                    else if (Photons[i].SpawnTimer == i_PhotonSpawnRate)
                    {
                        Photons[i] = new Particle(levelBounds, this);
                        Photons[i].LoadTex(t_Photon);
                        Photons[i].ParticleState = Particle.State.Spawning;
                        Photons[i].Spawn(offset, levelBounds, true);
                        Photons[i].SpawnTimer = 0;
                        soundBank.PlayCue("photonPop");
                    }
                }

                for (int i = 0; i < Chlor.Length; i++)
                {
                    if (Chlor[i].ParticleState == Particle.State.Fusing)
                    {
                        if (Chlor[i].i_Fusing > 50)
                        {
                            Chlor[i].ParticleState = Particle.State.Dead;
                            Chlor[i].i_Fusing = 0;
                        }
                        else
                        {
                            Chlor[i].i_Fusing++;
                        }
                    }
                    else if (Chlor[i].ParticleState == Particle.State.Colliding)
                    {
                        Chlor[i].Velocity = new Vector2((((Chlor[i].FusionPosition.X - Chlor[i].Position.X) / Vector2.Distance(Chlor[i].Position, Chlor[i].FusionPosition)) * 10), (((Chlor[i].FusionPosition.Y - Chlor[i].Position.Y) / Vector2.Distance(Chlor[i].Position, Chlor[i].FusionPosition)) * 10));
                        Chlor[i].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction);

                        if (Chlor[i].Position.X < (Chlor[i].FusionPosition.X + 1) && Chlor[i].Position.X > (Chlor[i].FusionPosition.X - 1) && Chlor[i].Position.Y < (Chlor[i].FusionPosition.Y + 1) && Chlor[i].Position.Y > (Chlor[i].FusionPosition.Y - 1))
                        {
                            Chlor[i].ParticleState = Particle.State.Fusing;
                        }
                    }
                    else if (Chlor[i].ParticleState == Particle.State.Alive)
                    {
                        if (Chlor[i].IsTethered)
                        {
                            if (!ship.BoundSphere.Intersects(new BoundingSphere(new Vector3(Chlor[i].Position, 0), (Chlor[i].Texture.Width / 2))))
                            {
                                Vector2 distance = ship.Position - Chlor[i].Position;
                                Chlor[i].Velocity += Vector2.Normalize(distance) * 20;
                            }
                        }
                        else
                        {
                            Chlor[i].Position += new Vector2(0, 0.07f);
                        }
                        Chlor[i].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction);
                    }
                    else if (Chlor[i].ParticleState == Particle.State.Spawning)
                    {
                        if (Chlor[i].Scale > 1)
                        {
                            Chlor[i].Scale = 1.0f;
                            Chlor[i].ParticleState = Particle.State.Alive;
                        }
                        else
                        {
                            Chlor[i].Scale += 0.02f;
                        }
                    }
                    else if (Chlor[i].SpawnTimer == i_ChloroSpawnRate)
                    {
                        Chlor[i] = new Particle(levelBounds, this);
                        Chlor[i].LoadTex(t_Chlor);
                        Chlor[i].ParticleState = Particle.State.Spawning;
                        Chlor[i].Spawn(offset, levelBounds, false);
                        Chlor[i].SpawnTimer = 0;
                        soundBank.PlayCue("chlorPop");
                    }
                }

                for (int i = 0; i < i_MaxNumberPhotons; i++)
                {
                    if (Photons[i].ParticleState == Particle.State.Dead)
                    {
                        Photons[i].SpawnTimer++;
                        break;
                    }
                }
                for (int i = 0; i < i_MaxNumberChloro; i++)
                {
                    if (Chlor[i].ParticleState == Particle.State.Dead)
                    {
                        Chlor[i].SpawnTimer++;
                        break;
                    }
                }
                #endregion
            }
            else if (gameState == State.Controls)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    soundBank.PlayCue("Back");
                    gameState = State.Menu;
                }
            }
            else if (gameState == State.Options)
            {
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
            }
            else if (gameState == State.Paused)
            {
                PauseCheck();
            }
            else if (gameState == State.GameEnd)
            {
                if (i_BlackCounter > 0)
                {
                    i_BlackCounter--;
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
                Gameover(b_LevelComplete);
            }
            else if (gameState == State.HighScore)
            {
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
            }
            else if (gameState == State.Tutorial)
            {
                if (TutMusic.IsPlaying == false)
                {
                    TutMusic.Play();
                }
                else if (TutMusic.IsPaused == true)
                {
                    TutMusic.Resume();
                }
                if (i_ShipGridCounter < 75 && b_Grid == true)
                {
                    i_ShipGridCounter++;
                }
                else if(i_ShipGridCounter == 75 && b_Grid == true)
                {
                    b_Grid = false;
                }
                else if (i_GridCounter < 100 && b_Pulsing == false)
                {
                    i_GridCounter++;
                }
                else if (i_GridCounter == 100 && b_Pulsing == false)
                {
                    b_Pulsing = true;
                }
                else if (i_Number1Counter < 20)
                {
                    i_Number1Counter++;
                }
                else if (i_Number2Counter < 20)
                {
                    i_Number2Counter++;
                }
                else if (i_Number3Counter < 20)
                {
                    i_Number3Counter++;
                }
                else if (i_Number4Counter < 20)
                {
                    i_Number4Counter++;
                }
                else if (i_Number5Counter < 20)
                {
                    i_Number5Counter++;
                }
                else if (i_ShipCounter < 75)
                {
                    i_ShipCounter++;
                }
                else if (i_ShipCounter == 75 && b_Grid == false && i_ShipGridCounter > 0)
                {
                    i_ShipGridCounter--;
                }
                else if (i_ShipCounter == 75 && b_Grid == false && i_ShipGridCounter == 0 && b_shield == false)
                {
                    b_shield = true;
                }
                else if (i_TutorialCounter < 100)
                {
                    i_TutorialCounter++;
                }
                else if (i_TutorialTextCounter < 75)
                {
                    i_TutorialTextCounter++;
                }
                else
                {
                    if (iTutorialState == 0)
                    {
                        tutCues[0].Play();
                        iTutorialState++;
                    }
                    else if (iTutorialState == 1)
                    {
                        if (tutCues[0].IsStopped == true)
                        {
                            iTutorialState++;
                        }
                    }
                    else if (iTutorialState == 2)
                    {
                        totalPixelsTravelled.X += (float)Math.Sqrt(
                            (ship.Position.X - lastShipPos.X) *
                            (ship.Position.X - lastShipPos.X));

                        totalPixelsTravelled.Y += (float)Math.Sqrt(
                            (ship.Position.Y - lastShipPos.Y) *
                            (ship.Position.Y - lastShipPos.Y));

                        if ((totalPixelsTravelled.X + totalPixelsTravelled.Y) > 3000)
                        {
                            tutCues[1].Play();
                            i_TextBubble = 2;
                            iTutorialState++;
                            dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                            Photons[0] = new Particle(new Vector2(768, 350), this);
                            Photons[0].LoadTex(t_Photon);
                            Photons[0].ParticleState = Particle.State.Spawning;
                            Photons[0].Spawn(new Vector2(768, 350));
                            Photons[0].SpawnTimer = 0;
                            soundBank.PlayCue("photonPop");
                        }

                        lastShipPos = ship.Position;
                    }
                    else if (iTutorialState == 3)
                    {
                        if (dt_TutorialTimer.Second == 6)
                        {
                            tutCues[2].Play();
                            i_TextBubble = 3;
                            iTutorialState++;
                            dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                        }
                    }
                    else if (iTutorialState == 4)
                    {
                        if (TetherState == TetherState.tethered && dt_TutorialTimer.Second >= 11)
                        {
                            tutCues[2].Stop(AudioStopOptions.Immediate);
                            tutCues[3].Play();
                            i_TextBubble = 4;
                            iTutorialState++;
                            dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                        }
                    }
                    else if (iTutorialState == 5)
                    {
                        if (TetherState == TetherState.detethered && dt_TutorialTimer.Second >= 6)
                        {
                            tutCues[4].Play();
                            i_TextBubble = 5;
                            iTutorialState++;
                            Photons[0].ParticleState = Particle.State.Dead;
                            dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                            Chlor[0] = new Particle(new Vector2(768, 350), this);
                            Chlor[0].LoadTex(t_Chlor);
                            Chlor[0].ParticleState = Particle.State.Spawning;
                            Chlor[0].Spawn(new Vector2(768, 350));
                            Chlor[0].SpawnTimer = 0;
                            soundBank.PlayCue("chlorPop");
                        }
                    }
                    else if (iTutorialState == 6)
                    {
                        if (TetherState == TetherState.tethered && ship.IsPhoton == false && dt_TutorialTimer.Second >= 8)
                        {
                            tutCues[5].Play();
                            i_TextBubble = 6;
                            iTutorialState++;
                            dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                            Photons[0] = new Particle(new Vector2(768, 350), this);
                            Photons[0].LoadTex(t_Photon);
                            Photons[0].ParticleState = Particle.State.Spawning;
                            Photons[0].Spawn(new Vector2(768, 350));
                            Photons[0].SpawnTimer = 0;
                            soundBank.PlayCue("photonPop");
                        }
                    }
                    else if (iTutorialState == 7)
                    {
                        if (b_TutorialFusion == true && dt_TutorialTimer.Second >= 7)
                        {
                            tutCues[6].Play();
                            i_TextBubble = 7;
                            iTutorialState++;
                            dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                        }
                    }
                    else if (iTutorialState == 8)
                    {
                        if (dt_TutorialTimer.Second >= 10 && tutBulletCounter > 20)
                        {
                            tutCues[7].Play();
                            i_TextBubble = 8;
                            iTutorialState++;
                            dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                        }
                    }
                    else if (iTutorialState == 9)
                    {
                        if (dt_TutorialTimer.Second >= 10)
                        {
                            tutCues[8].Play();
                            enemies[0] = new Enemy(this, 0);
                            enemies[0].LoadTex(t_EnemySmall, t_EnemyBig);
                            enemies[0].Spawn(new Vector2(900, 100));
                            enemies[0].SpawnTimer = 0;
                            enemies[0].Rectangle = new Rectangle(((int)enemies[0].Position.X - (enemies[0].Texture.Width / 2)), ((int)enemies[0].Position.Y - (enemies[0].Texture.Height / 2)), enemies[0].Texture.Width, enemies[0].Texture.Height);
                            enemies[0].TextureData = new Color[enemies[0].Texture.Width * enemies[0].Texture.Height];
                            enemies[0].Texture.GetData(enemies[0].TextureData);

                            i_TextBubble = 9;
                            iTutorialState++;
                            dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                        }
                    }
                    else if (iTutorialState == 10)
                    {
                        if (dt_TutorialTimer.Second >= 10 && enemies[0].Alive == false)
                        {
                            tutCues[9].Play();
                            enemies[0] = new Enemy(this, 1);
                            enemies[0].LoadTex(t_EnemySmall, t_EnemyBig);
                            enemies[0].Spawn(new Vector2(900, 100));
                            enemies[0].SpawnTimer = 0;
                            enemies[0].Rectangle = new Rectangle(((int)enemies[0].Position.X - (enemies[0].Texture.Width / 2)), ((int)enemies[0].Position.Y - (enemies[0].Texture.Height / 2)), enemies[0].Texture.Width, enemies[0].Texture.Height);
                            enemies[0].TextureData = new Color[enemies[0].Texture.Width * enemies[0].Texture.Height];
                            enemies[0].Texture.GetData(enemies[0].TextureData);

                            Chlor[0] = new Particle(new Vector2(768, 350), this);
                            Chlor[0].LoadTex(t_Chlor);
                            Chlor[0].ParticleState = Particle.State.Spawning;
                            Chlor[0].Spawn(new Vector2(768, 350));
                            Chlor[0].SpawnTimer = 0;
                            soundBank.PlayCue("chlorPop");
                            i_TextBubble = 10;
                            iTutorialState++;
                            dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                        }
                    }
                    else if (iTutorialState == 11)
                    {
                        if (enemies[0].Alive == false && dt_TutorialTimer.Second >= 9)
                        {
                            tutCues[10].Play();
                            i_TextBubble = 11;
                            iTutorialState++;
                            dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                        }
                    }
                    else if (iTutorialState == 12)
                    {
                        if (dt_TutorialTimer.Second == 10)
                        {
                            iTutorialState = 13;
                            dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                        }
                    }
                    else if (iTutorialState == 13)
                    {
                        if (i_BlackCounter == 75)
                        {
                            gameState = State.LevelIntro;
                            TutMusic.Pause();
                            dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                        }
                        else
                        {
                            i_BlackCounter++;
                            TutMusic.SetVariable("Volume", (100 - (i_BlackCounter * (1 + (1 / 3)))));
                        }
                    }

                    #region Photon Updates
                    if (Photons[0].ParticleState == Particle.State.Alive)
                    {
                        if (Photons[0].IsTethered)
                        {
                            if (!ship.BoundSphere.Intersects(new BoundingSphere(new Vector3(Photons[0].Position, 0), (Photons[0].Texture.Width / 2))))
                            {
                                Vector2 distance = ship.Position - Photons[0].Position;
                                Photons[0].Velocity += Vector2.Normalize(distance) * 20;
                            }
                        }
                        else
                        {
                            Photons[0].Position -= new Vector2(0, 0.07f);
                        }
                        Photons[0].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction);
                    }
                    else if (Photons[0].ParticleState == Particle.State.Spawning)
                    {
                        if (Photons[0].Scale > 1)
                        {
                            Photons[0].Scale = 1.0f;
                            Photons[0].ParticleState = Particle.State.Alive;
                        }
                        else
                        {
                            Photons[0].Scale += 0.02f;
                        }
                    }
                    else if (Photons[0].ParticleState == Particle.State.Fusing)
                    {
                        if (Photons[0].i_Fusing > 50)
                        {
                            Photons[0].ParticleState = Particle.State.Dead;
                            Photons[0].i_Fusing = 0;
                        }
                        else
                        {
                            Photons[0].i_Fusing++;
                        }
                    }
                    else if (Photons[0].ParticleState == Particle.State.Colliding)
                    {
                        Photons[0].Velocity = new Vector2((((Photons[0].FusionPosition.X - Photons[0].Position.X) / Vector2.Distance(Photons[0].Position, Photons[0].FusionPosition)) * 10), (((Photons[0].FusionPosition.Y - Photons[0].Position.Y) / Vector2.Distance(Photons[0].Position, Photons[0].FusionPosition)) * 10));
                        Photons[0].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction);
                        if (Photons[0].Position.X < (Photons[0].FusionPosition.X + 1) && Photons[0].Position.X > (Photons[0].FusionPosition.X - 1) && Photons[0].Position.Y < (Photons[0].FusionPosition.Y + 1) && Photons[0].Position.Y > (Photons[0].FusionPosition.Y - 1))
                        {
                            for (int j = 0; j < i_MaxNumberFused; j++)
                            {
                                if (Fused[j].ParticleState == Particle.State.Dead)
                                {
                                    Fused[j].ParticleState = Particle.State.Spawning;
                                    Photons[0].ParticleState = Particle.State.Fusing;
                                    Fused[j].Position = Photons[0].Position;
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                    #region Chloro Updates
                    if (Chlor[0].ParticleState == Particle.State.Alive)
                    {
                        if (Chlor[0].IsTethered)
                        {
                            if (!ship.BoundSphere.Intersects(new BoundingSphere(new Vector3(Chlor[0].Position, 0), (Chlor[0].Texture.Width / 2))))
                            {
                                Vector2 distance = ship.Position - Chlor[0].Position;
                                Chlor[0].Velocity += Vector2.Normalize(distance) * 20;
                            }
                        }
                        else
                        {
                            Chlor[0].Position += new Vector2(0, 0.07f);
                        }
                        Chlor[0].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction);
                    }
                    else if (Chlor[0].ParticleState == Particle.State.Fusing)
                    {
                        if (Chlor[0].i_Fusing > 50)
                        {
                            Chlor[0].ParticleState = Particle.State.Dead;
                            Chlor[0].i_Fusing = 0;
                        }
                        else
                        {
                            Chlor[0].i_Fusing++;
                        }
                    }
                    else if (Chlor[0].ParticleState == Particle.State.Colliding)
                    {
                        Chlor[0].Velocity = new Vector2((((Chlor[0].FusionPosition.X - Chlor[0].Position.X) / Vector2.Distance(Chlor[0].Position, Chlor[0].FusionPosition)) * 10), (((Chlor[0].FusionPosition.Y - Chlor[0].Position.Y) / Vector2.Distance(Chlor[0].Position, Chlor[0].FusionPosition)) * 10));
                        Chlor[0].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction);

                        if (Chlor[0].Position.X < (Chlor[0].FusionPosition.X + 1) && Chlor[0].Position.X > (Chlor[0].FusionPosition.X - 1) && Chlor[0].Position.Y < (Chlor[0].FusionPosition.Y + 1) && Chlor[0].Position.Y > (Chlor[0].FusionPosition.Y - 1))
                        {
                            Chlor[0].ParticleState = Particle.State.Fusing;
                        }
                    }
                    else if (Chlor[0].ParticleState == Particle.State.Spawning)
                    {
                        if (Chlor[0].Scale > 1)
                        {
                            Chlor[0].Scale = 1.0f;
                            Chlor[0].ParticleState = Particle.State.Alive;
                        }
                        else
                        {
                            Chlor[0].Scale += 0.02f;
                        }
                    }
                    #endregion
                    #region Enemy Updates
                    if (enemies[0].EnemyCollision == true)
                    {
                        UpdateShieldSpark(enemies[0].EnemyCollisionPosition, enemies[0]);
                        enemies[0].ShieldSparkCounter++;
                        if (enemies[0].ShieldSparkCounter == 40)
                        {
                            enemies[0].EnemyCollision = false;
                            enemies[0].ShieldSparkCounter = 0;
                        }
                    }

                    if (enemies[0].IsParticleKill == true)
                    {
                        UpdateParticleKill(enemies[0].ParticleKillPosition, enemies[0].p_ParticleKilled, enemies[0]);
                        enemies[0].ParticleKillCounter++;
                        if (enemies[0].ParticleKillCounter == 40)
                        {
                            enemies[0].IsParticleKill = false;
                            enemies[0].ParticleKillCounter = 0;
                        }
                    }
                    if (enemies[0].Alive == true)
                    {
                        enemies[0].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction);
                    }
                    #endregion
                }
                dt_TutorialTimer += gameTime.ElapsedGameTime;
                GamePlay(gameTime);
                FunnyNumbers();

                if (b_Pulsing == true)
                {
                    Pulsing();
                }
            }
            else if (gameState == State.Quiz)
            {
                if (TutMusic.IsPlaying == false)
                {
                    TutMusic.Play();
                }
                else if (TutMusic.IsPaused == true)
                {
                    TutMusic.Resume();
                }
                if (b_Flash == true)
                {
                    if (i_FlashTimer < 30)
                    {
                        i_FlashTimer++;
                    }
                    else
                    {
                        i_FlashTimer = 0;
                        i_WhichFlash = -1;
                        b_Flash = false;
                        i_CurrentQuestion++;
                    }
                }
                else
                {
                    QuestionsUpdate();
                }
            }
            else if (gameState == State.Loading)
            {
                Loading();
                gameState = State.GameIntro;
            }
            else if (gameState == State.LevelIntro)
            {
                GamePadState gamepadStateCurr = GamePad.GetState(PlayerIndex.One);
                if (gamepadStateCurr.IsButtonDown(Buttons.A) || Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    InitalizeLevel(1);
                    gameState = State.Gameplay;
                }
            }
            Vibrate();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            //spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (gameState == State.Menu)
            {
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
                spriteBatch.Draw(t_BlackScreen, new Vector2(0, 0), new Color(1, 1, 1, (i_BlackCounter / 50f)));
            }
            else if (gameState == State.Controls)
            {
                spriteBatch.Draw(controlsImage, new Vector2(0, 0), Color.White);
            }
            else if (gameState == State.Options)
            {
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
            }
            else if (gameState == State.GameIntro)
            {
                spriteBatch.Draw(v_Video1.OutputFrame, new Vector2(0, 0), null, Color.White, 0, new Vector2(0,0),1.6f, SpriteEffects.None, 0.1f);
                spriteBatch.DrawString(font, "Press        +         to skip intro", new Vector2(10, 740), Color.Black);
                spriteBatch.Draw(t_ButtonA, new Vector2(65, 735), null, Color.White, 0, new Vector2(0, 0), new Vector2(0.4f, 0.4f), SpriteEffects.None, 0.1f);
                spriteBatch.Draw(t_ButtonX, new Vector2(117, 735), null, Color.White, 0, new Vector2(0, 0), new Vector2(0.4f, 0.4f), SpriteEffects.None, 0.1f);
                spriteBatch.Draw(t_BlackScreen, new Vector2(0, 0), new Color(1, 1, 1, (i_BlackCounter / 50f)));
            }
            else if (gameState == State.Loading)
            {
                spriteBatch.Draw(t_Loading, new Vector2(0, 0), Color.White);
            }
            else if (gameState == State.LevelIntro)
            {
                spriteBatch.Draw(t_Level1Start, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(fontBig, "Level 1", new Vector2(10, 620), Color.White);
                spriteBatch.DrawString(font, "Press       to start", new Vector2(855, 730), Color.White);
                spriteBatch.Draw(t_ButtonA, new Vector2(905, 725), null, Color.White, 0, new Vector2(0, 0), new Vector2(0.4f, 0.4f), SpriteEffects.None, 0);
            }
            else if (gameState == State.Gameplay || gameState == State.Paused || /*gameState == State.GameEnd || */gameState == State.Tutorial)
            {
                if (gameState == State.Gameplay)
                {
                    spriteBatch.Draw(t_Level1, (new Vector2(0, 0) + offset), Color.White);
                }
                else if (gameState == State.Tutorial)
                {
                    spriteBatch.Draw(t_Tutorial, new Vector2(0, 0), new Color(1, 1, 1, (i_TutorialCounter / 100f)));
                    spriteBatch.Draw(t_Grid, new Vector2(0, 0), new Color(1, 1, 1, (i_GridCounter / 100f)));
                    spriteBatch.DrawString(fontSmallText, "Tachyon Emissions: " + funnyNo1 + "htz", new Vector2(32, 29), new Color(0, 1, 1, (i_Number2Counter / 20f)));
                    spriteBatch.DrawString(fontSmallText, "Tractor Beam Cohesion: " + funnyNo2 + "%", new Vector2(32, 44), new Color(0, 1, 1, (i_Number3Counter / 20f)));
                    spriteBatch.DrawString(fontSmallText, "Shield Matrix Variance: " + funnyNo3, new Vector2(32, 60), new Color(0, 1, 1, (i_Number4Counter / 20f)));
                    spriteBatch.DrawString(fontSmallText, "AVA Targeting Alignment: " + funnyNo4, new Vector2(32, 75), new Color(0, 1, 1, (i_Number5Counter / 20f)));
                }
                if (TetherState == TetherState.tethered)
                {
                    for (int i = 0; i < i_StupidLineMax; i++)
                    {
                        if (stupidline[i].Alive == true && TetherState == TetherState.tethered)
                        {
                            spriteBatch.Draw(stupidline[i].Texture, stupidline[i].Position + offset, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        }
                    }
                }
                for (int i = 0; i < Photons.Length; i++)
                {
                    if (Photons[i].ParticleState == Particle.State.Alive || Photons[i].ParticleState == Particle.State.Spawning || Photons[i].ParticleState == Particle.State.Colliding || Photons[i].ParticleState == Particle.State.Fusing)
                    {
                        spriteBatch.Draw(Photons[i].Texture, Photons[i].Position + offset, null, Photons[i].Color, 0, new Vector2(Photons[i].Texture.Width / 2, Photons[i].Texture.Height / 2), Photons[i].Scale, SpriteEffects.None, 0);
                    }
                }
                for (int i = 0; i < Chlor.Length; i++)
                {
                    if (Chlor[i].ParticleState == Particle.State.Alive || Chlor[i].ParticleState == Particle.State.Spawning || Chlor[i].ParticleState == Particle.State.Colliding || Chlor[i].ParticleState == Particle.State.Fusing)
                    {
                        spriteBatch.Draw(Chlor[i].Texture, Chlor[i].Position + offset, null, Chlor[i].Color, 0, new Vector2(Chlor[i].Texture.Width / 2, Chlor[i].Texture.Height / 2), Chlor[i].Scale, SpriteEffects.None, 0);
                    }
                }
                for (int i = 0; i < Fused.Length; i++)
                {
                    if (Fused[i].ParticleState == Particle.State.Alive)
                    {
                        spriteBatch.Draw(Fused[i].Texture, Fused[i].Position + offset, null, Fused[i].Color, 0, new Vector2(Fused[i].Texture.Width / 2, Fused[i].Texture.Height / 2), 1, SpriteEffects.None, 0);
                    }
                    Fused[i].Fusion.DrawParticle(gameTime, offset);

                }

                for (int i = 0; i < i_MaxNumberEnemies; i++)
                {
                    enemies[i].ParticleKill.DrawParticle(gameTime, offset);
                    if (enemies[i].Alive == true)
                    {
                        spriteBatch.Draw(enemies[i].Texture, enemies[i].Position + offset, null, Color.White, enemies[i].f_Rotation, new Vector2(enemies[i].Texture.Width / 2, enemies[i].Texture.Height / 2), 1, SpriteEffects.None, 0);
                    }
                    enemies[i].ShieldSpark.DrawParticle(gameTime, offset);
                }

                if (dead == false)
                {
                    Color newColor = ship.Color;
                    newColor.A = (byte)(i_ShipCounter / 75f);

                    spriteBatch.Draw(ship.Texture, (ship.Position + offset), null, newColor, (float)ship.NextRotation, new Vector2(30, 50), new Vector2(1.0f, 1.0f), SpriteEffects.None, 0);
                    spriteBatch.Draw(ship.Turret, (ship.Position + offset + Vector2.Transform(new Vector2(0, 20), Matrix.CreateRotationZ((float)ship.NextRotation))), null, newColor, (float)ship.NextRotationTurret, new Vector2(12f, 18f), new Vector2(1.0f, 1.0f), SpriteEffects.None, 0);
                    spriteBatch.Draw(t_ShipGrid, (ship.Position + offset), null, newColor, (float)ship.NextRotation, new Vector2(30, 50), new Vector2(1.0f, 1.0f), SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(ship.Texture, (ship.Position + offset), null, new Color(0.2f, 0.2f, 0.2f), (float)ship.NextRotation, new Vector2(30, 50), new Vector2(1.0f, 1.0f), SpriteEffects.None, 0);
                    spriteBatch.Draw(ship.Turret, (ship.Position + offset + Vector2.Transform(new Vector2(0, 20), Matrix.CreateRotationZ((float)ship.NextRotation))), null, Color.Black, (float)ship.NextRotationTurret, new Vector2(12f, 18f), new Vector2(1.0f, 1.0f), SpriteEffects.None, 0);

                    if (i_ExplosionTimer < 100)
                    {
                        shipExplosion.DrawParticle(gameTime, offset);
                        i_ExplosionTimer++;
                    }
                    else
                    {
                        if (i_BlackCounter == 75)
                        {
                            t_LevelEndScreen = Content.Load<Texture2D>("MenusBGrounds//Game over screen");
                            gameState = State.GameEnd;
                            Ambient.Pause();
                        }
                        else
                        {
                            i_BlackCounter++;
                        }
                    }
                }
                for (int i = 0; i < i_BulletMax; i++)
                {
                    if (bullets[i].Alive == true)
                    {
                        spriteBatch.Draw(bullets[i].Texture, bullets[i].Position + offset, null, Color.White, 0, new Vector2(bullets[i].Texture.Width / 2, bullets[i].Texture.Height / 2), 1, SpriteEffects.None, 0);
                    }
                    if (tethers[i].Alive == true)
                    {
                        spriteBatch.Draw(tethers[i].Texture, tethers[i].Position + offset, null, Color.White, 0, new Vector2(tethers[i].Texture.Width / 2, tethers[i].Texture.Height / 2), 1, SpriteEffects.None, 0);
                    }
                }
                if (gameState == State.Tutorial)
                {
                    if (b_shield == true)
                    {
                        engineSmoke.DrawParticle(gameTime, offset);
                        spriteBatch.Draw(engineSmokeT, (ship.Position + offset + (Vector2.Transform(new Vector2(-9, 45), Matrix.CreateRotationZ((float)ship.NextRotation)))), null, Color.Aqua, (float)ship.NextRotation, new Vector2(0, 0), new Vector2(1.0f, 1.0f), SpriteEffects.None, 0);
                        spriteBatch.Draw(t_Shield, (ship.Position + offset), null, Shield(), (float)ship.NextRotation, new Vector2(30, 50), v_ShieldSize, SpriteEffects.None, 0);
                    }
                    spriteBatch.Draw(t_TutorialOverlay, new Vector2(0, 0), new Color(1, 1, 1, (i_GridCounter / 75f)));
                    spriteBatch.DrawString(fontName, "Synthesis Training Program: Ver 4.7", new Vector2(280, 0), new Color(0, 1, 1, (i_Number1Counter / 20f)));
                    spriteBatch.Draw(t_TutorialText[(i_TextBubble - 1)], new Vector2(25, 580), new Color(1, 1, 1, (i_TutorialTextCounter / 75f)));
                }
                else
                {
                    engineSmoke.DrawParticle(gameTime, offset);
                    spriteBatch.Draw(engineSmokeT, (ship.Position + offset + (Vector2.Transform(new Vector2(-9, 45), Matrix.CreateRotationZ((float)ship.NextRotation)))), null, Color.Aqua, (float)ship.NextRotation, new Vector2(0, 0), new Vector2(1.0f, 1.0f), SpriteEffects.None, 0); 
                    spriteBatch.Draw(t_Shield, (ship.Position + offset), null, Shield(), (float)ship.NextRotation, new Vector2(30, 50), v_ShieldSize, SpriteEffects.None, 0);
                    spriteBatch.Draw(t_Fused, new Vector2(10, 725), null, Color.White, 0, new Vector2(0, 0), new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, f_Fushions.ToString("00") + "/" + i_TargetFusions.ToString("00"), new Vector2(46, 727), Color.DarkGray);
                    spriteBatch.DrawString(font, f_Fushions.ToString("00") + "/" + i_TargetFusions.ToString("00"), new Vector2(47, 728), Color.White);
                    spriteBatch.Draw(t_ClockBase, new Vector2(15, 650), Color.White);
                    spriteBatch.DrawString(font, dt_timer.Minute.ToString("0") + ":" + dt_timer.Second.ToString("00"), new Vector2(25, 668), Color.Black);
                    for (int i = 0; i < 8; i++)
                    {
                        if (i < segments)
                        {
                            spriteBatch.Draw(t_ClockSegFill[7 - i], new Vector2(15, 650), Color.White);
                        }
                        else
                        {

                        }
                    }
                }
                spriteBatch.Draw(t_BlackScreen, new Vector2(0, 0), new Color(1, 1, 1, (i_BlackCounter / 75f)));
                spriteBatch.Draw(t_Crosshair, new Vector2((Mouse.GetState().X - (t_Crosshair.Width / 2)), (Mouse.GetState().Y - (t_Crosshair.Height / 2))), Color.White);
            }
            if (gameState == State.Paused)
            {
                spriteBatch.Draw(t_BlackScreen, new Vector2(0, 0), new Color(1f, 1f, 1f, 0.5f));
                spriteBatch.DrawString(fontBig, "Paused", new Vector2(282, 277), Color.DarkGray);
                spriteBatch.DrawString(font, "Shield Strength: " + ship.ShieldStrength + "%", new Vector2(414, 419), Color.DarkGray);
                spriteBatch.DrawString(fontBig, "Paused", new Vector2(285, 280), Color.White);
                spriteBatch.DrawString(font, "Shield Strength: " + ship.ShieldStrength + "%", new Vector2(415, 420), Color.White);
            }
            if (gameState == State.GameEnd)
            {
                spriteBatch.Draw(t_BlackScreen, new Vector2(0, 0), new Color(1f, 1f, 1f, f_BlackAlpha));
                spriteBatch.Draw(t_LevelEndScreen, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(font, "Enemies(Small) Killed: ", new Vector2(100, 500), new Color(1f, 1f, 1f, f_Stats1Alpha));
                spriteBatch.DrawString(font, "Enemies(Big) Killed: ", new Vector2(119, 530), new Color(1f, 1f, 1f, f_Stats2Alpha));
                spriteBatch.DrawString(font, "Fusions: ", new Vector2(218, 560), new Color(1f, 1f, 1f, f_Stats3Alpha));
                spriteBatch.DrawString(font, "Level Complete Bonus: ", new Vector2(85, 590), new Color(1f, 1f, 1f, f_LvlCompleteAlpha));
                spriteBatch.DrawString(font, "Final Score: ", new Vector2(190, 650), new Color(1f, 1f, 1f, f_Stats4Alpha));
                spriteBatch.DrawString(font, "Grade: ", new Vector2(225, 680), new Color(1f, 1f, 1f, f_GradeAlpha));

                spriteBatch.DrawString(font, f_EnemiesSmallKilledTemp.ToString("000") + " x5", new Vector2(350, 500), new Color(1f, 1f, 1f, f_Stats1Alpha));
                spriteBatch.DrawString(font, f_EnemiesBigKilledTemp.ToString("000") + " x10", new Vector2(350, 530), new Color(1f, 1f, 1f, f_Stats2Alpha));
                spriteBatch.DrawString(font, f_FushionsTemp.ToString("000") + " x100", new Vector2(350, 560), new Color(1f, 1f, 1f, f_Stats3Alpha));
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

                spriteBatch.Draw(t_BlackScreen, new Vector2(0, 0), new Color(1, 1, 1, (i_BlackCounter / 75f)));
            }
            if (gameState == State.HighScore)
            {
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
            }
            if (gameState == State.Quiz)
            {
                if (i_CurrentQuestion < 5)
                {
                    spriteBatch.Draw(t_Questions, new Vector2(0, 0), Color.White);
                    spriteBatch.DrawString(fontQuestion, questions[i_CurrentQuestion].Question, new Vector2(50, 75), Color.White);
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 Position = Vector2.Zero;
                        if (i == 0)
                        {
                            Position = new Vector2(390, 270);
                        }
                        else if (i == 1)
                        {
                            Position = new Vector2(655, 455);
                        }
                        else if (i == 2)
                        {
                            Position = new Vector2(390, 630);
                        }
                        else if (i == 3)
                        {
                            Position = new Vector2(140, 455);
                        }

                        if (i_WhichFlash == i)
                        {
                            spriteBatch.DrawString(fontTimer, questions[i_CurrentQuestion].answers[i], Position, new Color(1.0f, (i_FlashTimer / 30f), (i_FlashTimer / 30f), 1.0f));
                        }
                        else
                        {
                            spriteBatch.DrawString(fontTimer, questions[i_CurrentQuestion].answers[i], Position, Color.White);
                        }
                    }
                }
                else
                {
                    spriteBatch.Draw(t_Answers, new Vector2(0, 0), Color.White);
                    spriteBatch.DrawString(fontTimer, "1.", new Vector2(100, 40), Color.Black);
                    spriteBatch.DrawString(fontTimer, "2.", new Vector2(100, 160), Color.Black);
                    spriteBatch.DrawString(fontTimer, "3.", new Vector2(100, 285), Color.Black);
                    spriteBatch.DrawString(fontTimer, "4.", new Vector2(100, 410), Color.Black);
                    spriteBatch.DrawString(fontTimer, "5.", new Vector2(100, 530), Color.Black);
                    spriteBatch.DrawString(fontTimer, questions[0].Question, new Vector2(165, 40), Color.Black);
                    spriteBatch.DrawString(fontTimer, questions[1].Question, new Vector2(165, 160), Color.Black);
                    spriteBatch.DrawString(fontTimer, questions[2].Question, new Vector2(165, 285), Color.Black);
                    spriteBatch.DrawString(fontTimer, questions[3].Question, new Vector2(165, 410), Color.Black);
                    spriteBatch.DrawString(fontTimer, questions[4].Question, new Vector2(165, 530), Color.Black);

                    for (int i = 0; i < 5; i++)
                    {
                        spriteBatch.DrawString(fontTimer, questions[i].answers[questions[i].correctAns], new Vector2(500, 95 + (i * 125)), Color.Black);
                        if (questions[i].youranswer == questions[i].correctAns)
                        {
                            spriteBatch.DrawString(fontTimer, questions[i].answers[questions[i].youranswer], new Vector2(75, 95 + (i * 125)), Color.Green);
                            spriteBatch.Draw(t_Tick, new Vector2(850, 30 + (i*125)), Color.White);

                        }
                        else
                        {
                            spriteBatch.DrawString(fontTimer, questions[i].answers[questions[i].youranswer], new Vector2(75, 95 + (i * 125)), Color.Red);
                            spriteBatch.Draw(t_Cross, new Vector2(850, 30 + (i * 125)), Color.White);

                        }
                        spriteBatch.DrawString(fontBig, i_CorrectAnswers + "/5", new Vector2(760, 626), Color.Black);
                        spriteBatch.DrawString(fontTimer, "Game Score:", new Vector2(450, 650), Color.Black);
                        spriteBatch.DrawString(fontTimer, "Quiz Score:", new Vector2(471, 680), Color.Black);
                        spriteBatch.DrawString(fontTimer, "Total Score:", new Vector2(465, 710), Color.Black);
                        spriteBatch.DrawString(fontTimer, f_Score.ToString("0000"), new Vector2(650, 650), Color.Black);
                        spriteBatch.DrawString(fontTimer, f_QuizScore.ToString("0000"), new Vector2(650, 680), Color.Black);
                        spriteBatch.DrawString(fontTimer, f_TotalScore.ToString("0000"), new Vector2(650, 710), Color.Black);
                    }
                }
            }

            //if (GamePad.GetState(PlayerIndex.One).IsConnected == false)
            //{
            //    spriteBatch.Draw(t_BlackScreen, new Vector2(0, 0), new Color(1f, 1f, 1f, 0.7f));
            //    spriteBatch.DrawString(fontTimer, "Please connect controller to port 1!", new Vector2((graphics.PreferredBackBufferWidth / 2) - 220, (graphics.PreferredBackBufferHeight / 2) - 6), Color.White);
            //}

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
        Color Shield()
        {
            if (ship.ShipShield == 100)
            {
                return new Color(0, 0, 255, i_shipAlpha);
            }
            else if (ship.ShipShield == 75)
            {
                i_PulseRate = 50;
                return new Color(0, 150, 150, i_shipAlpha);
            }
            else if (ship.ShipShield == 50)
            {
                i_PulseRate = 40;
                return new Color(150, 150, 0, i_shipAlpha);
            }
            else if (ship.ShipShield == 25)
            {
                i_PulseRate = 30;
                return new Color(150, 0, 0, i_shipAlpha);
            }
            else
            {
                return new Color(255, 255, 255, 0);
            }
        }
        void ShieldPulse()
        {
            if ((i_ShieldPulseCounter % (i_PulseRate/10)) == 0)
            {
                v_ShieldSize.X += 0.02f;
                v_ShieldSize.Y += 0.02f;
                i_shipAlpha -= 0.05f;
            }
            if (i_ShieldPulseCounter >= i_PulseRate)
            {
                v_ShieldSize = new Vector2(1.20f, 1.20f);
                i_shipAlpha = 0.5f;
                i_ShieldPulseCounter = 0;
            }
            else
            {
                i_ShieldPulseCounter++;
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
        void FireCheck()
        {
            #region Shooting
            #region Controller
            GamePadState gamepadStateCurr = GamePad.GetState(PlayerIndex.One);
            if (gamepadStateCurr.IsButtonDown(Buttons.RightTrigger))
            {
                if (i_FireRate == 7)
                {
                    i_FireRate = 0;
                    //FireBullet
                    for (int i = 0; i < i_BulletMax; i++)
                    {
                        if (bullets[i].Alive == false)
                        {
                            tutBulletCounter++;
                            v_TurretDirection = ship.TurretDirection;
                            bullets[i].Fire(ship, v_TurretDirection);
                            soundBank.PlayCue("lazer");
                            break;
                        }
                    }
                }
                else
                {
                    i_FireRate++;
                }
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released && gamepadStateCurr.IsButtonDown(Buttons.RightTrigger) == false)
            {
                i_FireRate = 7;
            }
            #endregion
            #region Mouse
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (i_FireRate == 7)
                {
                    i_FireRate = 0;
                    //FireBullet
                    for (int i = 0; i < i_BulletMax; i++)
                    {
                        if (bullets[i].Alive == false)
                        {
                            tutBulletCounter++;
                            v_TurretDirection = ship.TurretDirection;
                            bullets[i].Fire(ship, v_TurretDirection);
                            soundBank.PlayCue("lazer");
                            break;
                        }
                    }
                }
                else
                {
                    i_FireRate++;
                }
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released && gamepadStateCurr.IsButtonDown(Buttons.RightTrigger) == false)
            {
                i_FireRate = 7;
            }
            #endregion
            #endregion
            #region Tethering
            #region Controller
            if (gamepadStateCurr.IsButtonDown(Buttons.LeftTrigger))
            {
                switch (TetherState)
                {
                    case TetherState.shooting:
                        if (lastLTrigger == false)
                        {
                            soundBank.PlayCue("tetherFire");
                        }
                        //FireBullet
                        for (int i = 0; i < i_BulletMax; i++)
                        {
                            if (tethers[i].Alive == false)
                            {
                                v_TurretDirection = ship.TurretDirection;
                                tethers[i].Fire(ship, v_TurretDirection);
                                break;
                            }
                        }
                        break;

                    case TetherState.tethered:


                        if (lastLTrigger == false)
                        {




                            if (ship.IsPhoton)
                            {
                                Photons[ship.TetheredParticleID].IsTethered = false;
                                Photons[ship.TetheredParticleID].Color = Color.White;
                            }
                            else
                            {
                                Chlor[ship.TetheredParticleID].IsTethered = false;
                                Chlor[ship.TetheredParticleID].Color = Color.White;
                            }
                            TetherState = TetherState.detethered;
                            soundBank.PlayCue("deTether");
                        }
                        break;

                    case TetherState.detethered:
                        if (lastLTrigger == false)
                        {
                            TetherState = TetherState.shooting;
                        }
                        break;
                }

                lastLTrigger = true;
            }
            else
            {
                lastLTrigger = false;
            }
            #endregion
            #region Mouse
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                switch (TetherState)
                {
                    case TetherState.shooting:
                        if (lastRMouse == false)
                        {
                            soundBank.PlayCue("tetherFire");
                        }
                        //FireBullet
                        for (int i = 0; i < i_BulletMax; i++)
                        {
                            if (tethers[i].Alive == false)
                            {
                                v_TurretDirection = ship.TurretDirection;
                                tethers[i].Fire(ship, v_TurretDirection);
                                break;
                            }
                        }
                        break;

                    case TetherState.tethered:


                        if (lastRMouse == false)
                        {
                            if (ship.IsPhoton)
                            {
                                Photons[ship.TetheredParticleID].IsTethered = false;
                                Photons[ship.TetheredParticleID].Color = Color.White;
                            }
                            else
                            {
                                Chlor[ship.TetheredParticleID].IsTethered = false;
                                Chlor[ship.TetheredParticleID].Color = Color.White;
                            }
                            TetherState = TetherState.detethered;
                            soundBank.PlayCue("deTether");
                        }
                        break;

                    case TetherState.detethered:
                        if (lastRMouse == false)
                        {
                            TetherState = TetherState.shooting;
                        }
                        break;
                }

                lastRMouse = true;
            }
            else
            {
                lastRMouse = false;
            }
            #endregion
            #endregion
        }
        void PauseCheck()
        {
            #region Controller
            GamePadState gamepadStateCurr = GamePad.GetState(PlayerIndex.One);
            if (gamepadStateCurr.IsButtonDown(Buttons.Start))
            {
                if (!gamepadStateOld.IsButtonDown(Buttons.Start))
                {
                    if (gameState == State.Gameplay)
                    {
                        gameState = State.Paused;
                    }
                    else if (gameState == State.Paused)
                    {
                        gameState = State.Gameplay;
                    }
                }
            }
            gamepadStateOld = gamepadStateCurr;
            #endregion
            #region Keyboard
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (lastEscape == false)
                {
                    if (gameState == State.Gameplay)
                    {
                        gameState = State.Paused;
                    }
                    else if (gameState == State.Paused)
                    {
                        gameState = State.Gameplay;
                    }
                }
                lastEscape = true;
            }
            else
            {
                lastEscape = false;
            }

            gamepadStateOld = gamepadStateCurr;
            #endregion
        }
        void Gameover(bool LevelComplete)
        {
            f_Score = ((f_EnemiesBigKilled * 10) + (f_EnemiesSmallKilled * 5) + (f_Fushions * 100) + f_LevelCompleteBonus);
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
            else if (f_Fushions != f_FushionsTemp)
            {
                //asign blip to blipCue
                Cue blipCue = soundBank.GetCue("blip");

                blipCue.SetVariable("Pitch", f_FushionsTemp);

                if ((f_Fushions - f_FushionsTemp) % 4 == 0)
                {
                    blipCue.Play();
                }
                f_FushionsTemp++;
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
                            InitalizeLevel(1);
                            Reset();
                            gameState = State.Gameplay;
                        }
                        else if (i_GameOverSel == 1)
                        {
                            InitalizeLevel(0);
                            gameState = State.Quiz;
                            TutMusic.SetVariable("Volume", 100);
                            TutMusic.Resume();
                        }
                        else if (i_GameOverSel == 2)
                        {
                            InitalizeLevel(0);
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
                        InitalizeLevel(1);
                        Reset();
                        gameState = State.Gameplay;
                    }
                    else if (i_GameOverSel == 1)
                    {
                        InitalizeLevel(0);
                        gameState = State.Quiz;
                        TutMusic.SetVariable("Volume", 100);
                        TutMusic.Resume();
                    }
                    else if (i_GameOverSel == 2)
                    {
                        InitalizeLevel(0);
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
            segments = 8;
            b_Pulse = true;
            b_Pulsing = false;
            b_LevelComplete = false;
            i_GridCounter = 0;
            i_TutorialCounter = 0;
            i_ShipGridCounter = 0;
            i_Number1Counter = 0;
            i_Number2Counter = 0;
            i_Number3Counter = 0;
            i_Number4Counter = 0;
            i_Number5Counter = 0;
            i_TutorialTextCounter = 0;
            i_ShipCounter = 0;
            b_Grid = true;
            b_shield = false;
            b_fading = false;
            b_start = true;
            tutBulletCounter = 0;
            f_QuizScore = 0;
            f_TotalScore = 0;
            i_LetterPos = 0;
            i_HighScorePosition = 11;
            i_WhichLetter = 1;
            i_NewScore = 0;
            i_UnderScoreTimer = 0;
            s_NewGrade = "";
            dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 00);
            iTutorialState = 0;
            b_TutorialFusion = false;
            i_CurrentQuestion = 0;
            i_CorrectAnswers = 0;
            entering = false;
            dt_timer = new DateTime(1901, 1, 1, 0, 5, 00);
            i_SoundCounter2 = 0;
            i_SoundCounter = 0;
            vibrate = 0;
            v_ShieldSize = new Vector2(1.20f, 1.20f);
            i_ShieldPulseCounter = 0;
            i_shipAlpha = 0.5f;
            i_PulseRate = 60;
            i_ExplosionTimer = 0;
            dead = false;
            ship.ResetShip(500, 350);
            offset = new Vector2(0, 0);
            f_BlackAlpha = 0;
            f_Stats1Alpha = 0;
            f_Stats2Alpha = 0;
            f_Stats3Alpha = 0;
            f_Stats4Alpha = 0;
            f_ChoicesAlpha = 0;
            i_GameOverSel = 0;
            f_Score = 0;
            f_EnemiesBigKilled = 0;
            f_EnemiesSmallKilled = 0;
            f_Fushions = 0;
            f_FushionsTemp = 0;
            f_EnemiesSmallKilledTemp = 0;
            f_EnemiesBigKilledTemp = 0;
            f_ScoreTemp = 0;
            s_grade = "n/a";
            f_GradeAlpha = 0;
            f_LevelCompleteBonus = 0;
            f_LevelCompleteBonusTemp = 0;
            f_LvlCompleteAlpha = 0;

            ship = null;
            enemies = null;
            Photons = null;
            Chlor = null;
            Fused = null;
            bullets = null;
            tethers = null;

            //for (int i = 0; i < i_MaxNumberEnemies; i++)
            //{
            //    enemies[i] = new Enemy(this);
            //}
            //for (int i = 0; i < i_MaxNumberPhotons; i++)
            //{
            //    Photons[i] = new Particle(levelBounds, this);
            //    Photons[i].LoadTex(t_Photon);
            //}
            //for (int i = 0; i < i_MaxNumberChloro; i++)
            //{
            //    Chlor[i] = new Particle(levelBounds, this);
            //    Chlor[i].LoadTex(t_Chlor);
            //}
            //for (int i = 0; i < i_MaxNumberFused; i++)
            //{
            //    Fused[i] = new Particle(levelBounds, this);
            //    Fused[i].LoadTex(t_Fused);
            //    Fused[i].Fusion = new FusionParticleSystem(this, 10);
            //    Components.Add(Fused[i].Fusion);
            //}
        }
        void FunnyNumbers()
        {
            if (funnyNoCounter1 == 5)
            {
                funnyNo1 = Random.Next(1775, 1799);
                funnyNoCounter1 = 0;
            }
            else
            {
                funnyNoCounter1++;
            }
            if (funnyNoCounter2 == 80)
            {
                funnyNo2 = Random.Next(85, 92);
                funnyNoCounter2 = 0;
            }
            else
            {
                funnyNoCounter2++;
            }
            if (funnyNoCounter3 == 20)
            {
                funnyNo3 = (Random.Next(165, 189) / 1000f);
                funnyNoCounter3 = 0;
            }
            else
            {
                funnyNoCounter3++;
            }
            if (funnyNoCounter4 == 35)
            {
                funnyNo4 = (Random.Next(-30, 30) / 1000f);
                funnyNoCounter4 = 0;
            }
            else
            {
                funnyNoCounter4++;
            }
        }   
        void Loading()
        {
            t_Crosshair = Content.Load<Texture2D>("Gameplay//crosshair");
            t_TutorialOverlay = Content.Load<Texture2D>("Tutorial//TutorialOverlay"); 
            t_ShipGrid = Content.Load<Texture2D>("Ship//ShipGrid");
            t_Tick = Content.Load<Texture2D>("MenusBGrounds//Tick");
            t_Cross = Content.Load<Texture2D>("MenusBGrounds//Cross");
            t_Answers = Content.Load<Texture2D>("MenusBGrounds//Answers");
            t_Questions = Content.Load<Texture2D>("MenusBGrounds//Questions");
            t_Ship = Content.Load<Texture2D>("Ship//Ship");
            t_Photon = Content.Load<Texture2D>("Particles//Photon");
            t_Chlor = Content.Load<Texture2D>("Particles//Chloroplasts");
            t_Fused = Content.Load<Texture2D>("Particles//Fused");
            t_EnemySmall = Content.Load<Texture2D>("Enemies//Enemy_Small");
            t_EnemyBig = Content.Load<Texture2D>("Enemies//Enemy_Big");
            t_Bullet = Content.Load<Texture2D>("Ship//Bullet");
            t_Tether = Content.Load<Texture2D>("Ship//tether");
            t_UnderScore = Content.Load<Texture2D>("MenusBGrounds//Underscore");
            t_HighScore = Content.Load<Texture2D>("MenusBGrounds//High score screen");
            t_LevelEndScreen = Content.Load<Texture2D>("MenusBGrounds//Game over screen");
            t_GOContinue = Content.Load<Texture2D>("MenusBGrounds//continue");
            t_GOQuit = Content.Load<Texture2D>("MenusBGrounds//Game_over_quit");
            t_GORetry = Content.Load<Texture2D>("MenusBGrounds//retry");
            t_Level1 = Content.Load<Texture2D>("MenusBGrounds//background");
            t_Level1Start = Content.Load<Texture2D>("MenusBGrounds//Level1Start");
            t_Tutorial = Content.Load<Texture2D>("Tutorial//Tutorial"); 
            t_Pixel = Content.Load<Texture2D>("mightypixel");
            engineSmokeT = Content.Load<Texture2D>("Ship//engine_smoke");
            t_Shield = Content.Load<Texture2D>("Ship//ShieldTemp");
            t_Grid = Content.Load<Texture2D>("Tutorial//Grid");
            t_TutorialText = new Texture2D[11];
            for (int i = 0; i < 11; i++)
            {
                t_TutorialText[i] = Content.Load<Texture2D>("Tutorial//tutorial_text_" + (i + 1));
            } 
            t_ClockSegFill = new Texture2D[8];
            for (int i = 0; i < 8; i++)
            {
                t_ClockSegFill[i] = Content.Load<Texture2D>("Gameplay//Clock_" + (i + 1));
            }
            t_ClockBase = Content.Load<Texture2D>("GamePlay//ClockBase");

            v_Video1 = new VideoPlayer(@"intro.avi", GraphicsDevice);
            v_Video1.Play();
            MenuMusic.Pause();
        }
        void Pulsing()
        {
            if (b_Pulse == true)
            {
                if (i_GridCounter >= 75)
                {
                    b_Pulse = false;
                }
                else
                {
                    i_GridCounter += 0.5f;
                }
            }
            else if (b_Pulse == false)
            {
                if (i_GridCounter <= 50)
                {
                    b_Pulse = true;
                }
                else
                {
                    i_GridCounter -= 0.5f;
                }
            }
        }
        void SegmentUpdate()
        {
            if (dt_timer.Second + (dt_timer.Minute * 60) > 262.5)
            {
                segments = 8;
            }
            else if (dt_timer.Second + (dt_timer.Minute * 60) > 225)
            {
                segments = 7;
            }
            else if (dt_timer.Second + (dt_timer.Minute * 60) > 187.5)
            {
                segments = 6;
            }
            else if (dt_timer.Second + (dt_timer.Minute * 60) > 150)
            {
                segments = 5;
            }
            else if (dt_timer.Second + (dt_timer.Minute * 60) > 112.5)
            {
                segments = 4;
            }
            else if (dt_timer.Second + (dt_timer.Minute * 60) > 75)
            {
                segments = 3;
            }
            else if (dt_timer.Second + (dt_timer.Minute * 60) > 37.5)
            {
                segments = 2;
            }
            else if (dt_timer.Second + (dt_timer.Minute * 60) > 0)
            {
                segments = 1;
            }
            else
            {
                segments = 0;
            }
        }

        public static float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }
        private void UpdateEngineSmoke()
        {
            engineSmoke.AddParticles(ship.Position + (Vector2.Transform(new Vector2(0, 45), Matrix.CreateRotationZ((float)ship.NextRotation))), ship.ShipDirection, offset, (float)ship.NextRotation, false);
        }
        private void UpdateShieldSpark(Vector2 collision, Enemy enemy)
        {
            enemy.ShieldSpark.AddParticles(collision, new Vector2(0, 0), ship.Position, 0, true);
        }
        private void UpdateParticleKill(Vector2 collision, Particle particle, Enemy enemy)
        {
            enemy.ParticleKill.AddParticles((collision - new Vector2((particle.Texture.Width / 2), (particle.Texture.Width / 4))), new Vector2(0, 0), ship.Position, 0, true);
        }
        private void UpdateFusionEffect(Vector2 collision, Particle particle)
        {
            particle.Fusion.AddParticles((collision - new Vector2((particle.Texture.Width / 2), (particle.Texture.Width / 4))), new Vector2(0, 0), ship.Position, 0, true);
        }
        private void UpdateExplosion(Vector2 collision)
        {
            shipExplosion.AddParticles(collision, new Vector2(0, 0), ship.Position, 0, true);
        }
        
        public void COLLISION(Ship ship, Particle particle)
        {
            if (particle.IsTethered == true)
            {
                if (ship.BoundSphere.Intersects(new BoundingSphere(new Vector3(particle.Position, 0), (particle.Texture.Width / 2))))
                {
                    particle.Velocity *= -0.5f;
                }
            }
            else
            {
                if (ship.Rectangle.Intersects(particle.Rectangle))
                {
                    if (IntersectPixels(particle.RectanglePosTransform,
                                        particle.Texture.Width,
                                        particle.Texture.Height,
                                        particle.TextureData,
                                        ship.RotationTransform,
                                        ship.Texture.Width,
                                        ship.Texture.Height,
                                        ship.TextureData))
                    {
                        if (particle.BeingAttacked == false)
                        {
                            vibrateCounter++;

                            Vector2 impactSpeed;
                            Vector2 impulse;
                            float resultantImpulse;

                            impactSpeed = particle.Velocity - ship.Velocity;
                            impulse = Vector2.Normalize(particle.Position - ship.Position);
                            resultantImpulse = Vector2.Dot(impulse, impactSpeed);
                            if (resultantImpulse < 0)
                            {
                                resultantImpulse *= -1;
                            }
                            impulse = impulse * (float)Math.Sqrt(resultantImpulse);

                            particle.Velocity += impulse;
                        }
                    }
                }
            }
        }
        public void COLLISION(Bullet tether, Particle particle, int ID, bool isAPhoton)
        {
            if (TetherState == TetherState.shooting)
            {
                if (tether.Rectangle.Intersects(particle.Rectangle))
                {
                    if (IntersectPixels(particle.RectanglePosTransform,
                                        particle.Texture.Width,
                                        particle.Texture.Height,
                                        particle.TextureData,
                                        tether.RectanglePosTransform,
                                        tether.Texture.Width,
                                        tether.Texture.Height,
                                        tether.TextureData))
                    {
                        if (particle.ParticleState == Particle.State.Alive && particle.BeingAttacked == false)
                        {
                            particle.Color = Color.Aqua;
                            ship.TetheredParticleID = ID;
                            ship.IsPhoton = isAPhoton;
                            ship.Tethering = true;
                            particle.IsTethered = true;
                            tether.Alive = false;
                            TetherState = TetherState.tethered;
                            soundBank.PlayCue("tethered");

                            if (particle.Attacker != null)
                            {
                                particle.Attacker.i_KillingCounter = 0;
                                particle.Attacker.f_colour = 1.0f;
                                particle.Attacker.attacking = false;
                                particle.Attacker.f_Rotation = 0;
                            }
                            particle.BeingAttacked = false;
                            particle.Attacker = null;
                            if (particle.IsTethered == false)
                            {
                                particle.Color = Color.White;
                            }
                        }
                    }
                }
            }
        }
        public void COLLISION(Particle particleA, Particle particleB, bool fusion)
        {
            if (particleA.Rectangle.Intersects(particleB.Rectangle))
            {


                if (IntersectPixels(particleB.RectanglePosTransform,
                                    particleB.Texture.Width,
                                    particleB.Texture.Height,
                                    particleB.TextureData,
                                    particleA.RectanglePosTransform,
                                    particleA.Texture.Width,
                                    particleA.Texture.Height,
                                    particleA.TextureData))
                {
                    if (fusion == true)
                    {
                        ship.Tethering = false;
                        TetherState = TetherState.detethered;
                        particleA.ParticleState = Particle.State.Colliding;
                        particleB.ParticleState = Particle.State.Colliding;
                        soundBank.PlayCue("fusion");
                        particleA.FusionPosition = new Vector2((particleA.Position.X + ((particleB.Position.X - particleA.Position.X) / 2)), (particleA.Position.Y + ((particleB.Position.Y - particleA.Position.Y) / 2)));
                        particleB.FusionPosition = new Vector2((particleB.Position.X + ((particleA.Position.X - particleB.Position.X) / 2)), (particleB.Position.Y + ((particleA.Position.Y - particleB.Position.Y) / 2)));
                    }
                    else if (fusion == false)
                    {
                        Vector2 impactSpeed;
                        Vector2 impulse;
                        float resultantImpulse;

                        impactSpeed = particleB.Velocity - particleA.Velocity;
                        impulse = Vector2.Normalize(particleB.Position - particleA.Position);
                        resultantImpulse = Vector2.Dot(impulse, impactSpeed);
                        if (resultantImpulse < 0)
                        {
                            resultantImpulse *= -1;
                        }
                        impulse = impulse * (float)Math.Sqrt(resultantImpulse);

                        particleB.Velocity += impulse;
                        particleA.Velocity -= impulse;
                    }
                }
            }
        }
        public void COLLISION(Ship ship, Enemy enemy)
        {
            if (ship.Rectangle.Intersects(enemy.Rectangle))
            {


                if (IntersectPixels(enemy.RectanglePosTransform,
                                    enemy.Texture.Width,
                                    enemy.Texture.Height,
                                    enemy.TextureData,
                                    ship.RotationTransform,
                                    ship.Texture.Width,
                                    ship.Texture.Height,
                                    ship.TextureData))
                {

                    vibrateCounter++; //add to vibration amount

                    enemy.EnemyCollisionPosition = enemy.Position;// -new Vector2(enemy.Texture.Width / 2, enemy.Texture.Height / 2);
                    ship.ShipShield -= 25;
                    if (ship.ShipShield < 0)
                    {
                        //You are Dead
                        soundBank.PlayCue("shipExplode");
                        dead = true;
                    }
                    if (ship.ShipShield == 75)
                    {
                        soundBank.PlayCue("75percent");
                    }
                    else if (ship.ShipShield == 50)
                    {
                        soundBank.PlayCue("50percent");
                    }
                    else if (ship.ShipShield == 25)
                    {
                        soundBank.PlayCue("25percent");
                    }
                    else if (ship.ShipShield == 0)
                    {
                        ship.ShipShield = -1;
                        soundBank.PlayCue("shieldsdown");
                    }
                    else 
                    enemy.EnemyCollision = true;
                    ship.Hit = true;
                    enemy.Alive = false;
                    soundBank.PlayCue("enemyDie");
                    vibrate = 20;

                    if (enemy.desiredParticle != Enemy.ParticleWant.None)
                    {
                        if (enemy.desiredParticle == Enemy.ParticleWant.Chloro)
                        {
                            Chlor[enemy.ClosestChloro].BeingAttacked = false;
                            Chlor[enemy.ClosestChloro].Attacker = null;
                            if (Chlor[enemy.ClosestChloro].IsTethered == false)
                            {
                                Chlor[enemy.ClosestChloro].Color = Color.White;
                            }
                        }
                        else if (enemy.desiredParticle == Enemy.ParticleWant.Photon)
                        {
                            Photons[enemy.ClosestPhoton].BeingAttacked = false;
                            Photons[enemy.ClosestPhoton].Attacker = null;
                            if (Photons[enemy.ClosestPhoton].IsTethered == false)
                            {
                                Photons[enemy.ClosestPhoton].Color = Color.White;
                            }
                        }
                    }
                }
            }
        }
        public void COLLISION(Enemy enemy, Particle particle)
        {
            if (enemy.Rectangle.Intersects(particle.Rectangle))
            {


                if (IntersectPixels(particle.RectanglePosTransform,
                                    particle.Texture.Width,
                                    particle.Texture.Height,
                                    particle.TextureData,
                                    enemy.RectanglePosTransform,
                                    enemy.Texture.Width,
                                    enemy.Texture.Height,
                                    enemy.TextureData))
                {
                    if (enemy.desiredParticle != Enemy.ParticleWant.None)
                    {
                        if (enemy.desiredParticle == Enemy.ParticleWant.Chloro)
                        {
                            if (Chlor[enemy.ClosestChloro] == particle)
                            {
                                particle.BeingAttacked = true;
                                particle.Attacker = enemy;
                                enemy.attacking = true;
                            }
                        }
                        else if (enemy.desiredParticle == Enemy.ParticleWant.Photon)
                        {
                            if (Photons[enemy.ClosestPhoton] == particle)
                            {
                                particle.BeingAttacked = true;
                                particle.Attacker = enemy;
                                enemy.attacking = true;
                            }
                        }
                    }
                    else
                    {

                        Vector2 impactSpeed;
                        Vector2 impulse;
                        float resultantImpulse;

                        impactSpeed = particle.Velocity - enemy.Velocity;
                        impulse = Vector2.Normalize(particle.Position - enemy.Position);
                        resultantImpulse = Vector2.Dot(impulse, impactSpeed);
                        if (resultantImpulse < 0)
                        {
                            resultantImpulse *= -1;
                        }
                        impulse = impulse * (float)Math.Sqrt(resultantImpulse);

                        particle.Velocity += impulse;
                        //enemy.Velocity -= impulse; //enemies aren't affected by particle collisions :P
                    }
                }
            }
        }
        public void COLLISION(Enemy enemyA, Enemy enemyB)
        {
            if (enemyA.Rectangle.Intersects(enemyB.Rectangle))
            {


                if (IntersectPixels(enemyB.RectanglePosTransform,
                                    enemyB.Texture.Width,
                                    enemyB.Texture.Height,
                                    enemyB.TextureData,
                                    enemyA.RectanglePosTransform,
                                    enemyA.Texture.Width,
                                    enemyA.Texture.Height,
                                    enemyA.TextureData))
                {
                    Vector2 impactSpeed;
                    Vector2 impulse;
                    float resultantImpulse;

                    impactSpeed = enemyB.Velocity - enemyA.Velocity;
                    impulse = Vector2.Normalize(enemyB.Position - enemyA.Position);
                    resultantImpulse = Vector2.Dot(impulse, impactSpeed);
                    if (resultantImpulse < 0)
                    {
                        resultantImpulse *= -1;
                    }
                    impulse = impulse * (float)Math.Sqrt(resultantImpulse);

                    enemyB.Velocity += impulse*5;
                    enemyA.Velocity -= impulse*5; //enemies aren't affected by enemyB collisions :P
                }
            }
        }
        public void COLLISION(Enemy enemy, Bullet bullet)
        {
            if (bullet.Rectangle.Intersects(enemy.Rectangle))
            {
                if (IntersectPixels(enemy.RectanglePosTransform,
                                    enemy.Texture.Width,
                                    enemy.Texture.Height,
                                    enemy.TextureData,
                                    bullet.RectanglePosTransform,
                                    bullet.Texture.Width,
                                    bullet.Texture.Height,
                                    bullet.TextureData))
                {
                    bullet.Alive = false;
                    enemy.EnemyCollisionPosition = enemy.Position;// -new Vector2(enemy.Texture.Width / 2, enemy.Texture.Height / 2);
                    enemy.EnemyCollision = true;
                    enemy.Alive = false;
                    soundBank.PlayCue("enemyDie");
                    if (enemy.EnemyType == Enemy.Type.Big)
                    {
                        f_EnemiesBigKilled++;
                    }
                    else if (enemy.EnemyType == Enemy.Type.Small)
                    {
                        f_EnemiesSmallKilled++;
                    }

                    if (enemy.desiredParticle != Enemy.ParticleWant.None)
                    {
                        if (enemy.desiredParticle == Enemy.ParticleWant.Chloro)
                        {
                            Chlor[enemy.ClosestChloro].BeingAttacked = false;
                            Chlor[enemy.ClosestChloro].Attacker = null;
                            if (Chlor[enemy.ClosestChloro].IsTethered == false)
                            {
                                Chlor[enemy.ClosestChloro].Color = Color.White;
                            }
                        }
                        else if (enemy.desiredParticle == Enemy.ParticleWant.Photon)
                        {
                            Photons[enemy.ClosestPhoton].BeingAttacked = false;
                            Photons[enemy.ClosestPhoton].Attacker = null;
                            if (Photons[enemy.ClosestPhoton].IsTethered == false)
                            {
                                Photons[enemy.ClosestPhoton].Color = Color.White;
                            }
                        }
                    }
                }
            }
        }

        public void EDGECOLLISION(Ship thing, Rectangle bounds)
        {
            if (thing.Position.Y < bounds.Y)
            {
                thing.Position += new Vector2(0, bounds.Y - thing.Position.Y);
                thing.Velocity *= new Vector2(1, -edgeDamper);
                soundBank.PlayCue("sideBump");
            }
            else if (thing.Position.Y > bounds.Y + bounds.Height)
            {
                thing.Position -= new Vector2(0, thing.Position.Y - (bounds.Y + bounds.Height));
                thing.Velocity *= new Vector2(1, -edgeDamper);
                soundBank.PlayCue("sideBump");
            }
            if (thing.Position.X < bounds.X)
            {
                thing.Position += new Vector2(bounds.X - thing.Position.X, 0);
                thing.Velocity *= new Vector2(-edgeDamper, 1);
                soundBank.PlayCue("sideBump");
            }
            else if (thing.Position.X > bounds.X + bounds.Width)
            {
                thing.Position -= new Vector2(thing.Position.X - (bounds.X + bounds.Width),0);
                thing.Velocity *= new Vector2(-edgeDamper, 1);
                soundBank.PlayCue("sideBump");
            }
        }
        public void EDGECOLLISION(Enemy thing, Rectangle bounds)
        {
            if (thing.Position.Y < bounds.Y)
            {
                thing.Position += new Vector2(0, bounds.Y - thing.Position.Y);
                thing.Velocity *= new Vector2(1, -edgeDamper);
            }
            else if (thing.Position.Y > bounds.Y + bounds.Height)
            {
                thing.Position -= new Vector2(0, thing.Position.Y - (bounds.Y + bounds.Height));
                thing.Velocity *= new Vector2(1, -edgeDamper);
            }
            if (thing.Position.X < bounds.X)
            {
                thing.Position += new Vector2(bounds.X - thing.Position.X, 0);
                thing.Velocity *= new Vector2(-edgeDamper, 1);
            }
            else if (thing.Position.X > bounds.X + bounds.Width)
            {
                thing.Position -= new Vector2(thing.Position.X - (bounds.X + bounds.Width), 0);
                thing.Velocity *= new Vector2(-edgeDamper, 1);
            }
        }
        public void EDGECOLLISION(Particle thing, Rectangle bounds)
        {
            if (thing.Position.Y < bounds.Y)
            {
                thing.Position += new Vector2(0, bounds.Y - thing.Position.Y);
                thing.Velocity *= new Vector2(1, -edgeDamper);
            }
            else if (thing.Position.Y > bounds.Y + bounds.Height)
            {
                thing.Position -= new Vector2(0, thing.Position.Y - (bounds.Y + bounds.Height));
                thing.Velocity *= new Vector2(1, -edgeDamper);
            }
            if (thing.Position.X < bounds.X)
            {
                thing.Position += new Vector2(bounds.X - thing.Position.X, 0);
                thing.Velocity *= new Vector2(-edgeDamper, 1);
            }
            else if (thing.Position.X > bounds.X + bounds.Width)
            {
                thing.Position -= new Vector2(thing.Position.X - (bounds.X + bounds.Width), 0);
                thing.Velocity *= new Vector2(-edgeDamper, 1);
            }
        }

        public void LEVELCOLLISION(Ship item)
        {
            if (item.Rectangle.Intersects(levelLeft)  ||
                item.Rectangle.Intersects(levelRight) ||
                item.Rectangle.Intersects(levelTop)   ||
                item.Rectangle.Intersects(levelBottom))
            {


                if (IntersectPixels(item.RotationTransform,
                                    item.Texture.Width,
                                    item.Texture.Height,
                                    item.TextureData,
                                    Matrix.Identity,
                                    t_LevelBounds.Width,
                                    t_LevelBounds.Height,
                                    levelBoundsData))
                {

                    vibrateCounter++;

                    if (item.Position.Y < levelTop.Height)
                    {
                        item.Position += new Vector2(0, 5);
                        item.Velocity *= new Vector2(1, -edgeDamper);
                    }
                    else if (item.Position.Y > levelTop.Height + levelBottom.Y)
                    {
                        item.Position -= new Vector2(0, item.Position.Y - (levelTop.Height + levelBottom.Y));
                        item.Velocity *= new Vector2(1, -edgeDamper);
                    }
                    if (item.Position.X < levelTop.X)
                    {
                        item.Position += new Vector2(levelTop.X - item.Position.X, 0);
                        item.Velocity *= new Vector2(-edgeDamper, 1);
                    }
                    else if (item.Position.X > levelTop.X + levelRight.X)
                    {
                        item.Position -= new Vector2(item.Position.X - (levelTop.X + levelRight.X), 0);
                        item.Velocity *= new Vector2(-edgeDamper, 1);
                    }

                }
            }
        }

        public static bool IntersectPixels(
                            Matrix transformA, int widthA, int heightA, Color[] dataA,
                            Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
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
                        TutMusic.Pause();
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
                        TutMusic.Pause();
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

        public void InitalizeLevel(int i)
        {
            //TutorialLevel
            if (i == 0)
            {
                tutCues = new Cue[11];
                for (int j = 0; j < 11; j++)
                {
                    tutCues[j] = soundBank.GetCue((j+1).ToString("000"));
                }
                levelBounds = new Rectangle(75, 75, 874, 460);
                i_EnemySpawnRate = 0;
                i_MaxNumberEnemies = 1;
                i_PhotonSpawnRate = 0;
                i_MaxNumberPhotons = 1;
                i_ChloroSpawnRate = 0;
                i_MaxNumberChloro = 1;
                i_MaxNumberFused = 1;
                f_Fushions = 0;
                i_ShieldPulseCounter = 0;
                i_shipAlpha = 0.5f;
                i_PulseRate = 60;

                enemies = new Enemy[i_MaxNumberEnemies];
                for (int j = 0; j < i_MaxNumberEnemies; j++)
                {
                    enemies[i] = new Enemy(this);
                    enemies[j].LoadTex(t_EnemySmall, t_EnemyBig);
                    enemies[j].Rectangle = new Rectangle(((int)enemies[j].Position.X - (enemies[j].Texture.Width / 2)), ((int)enemies[j].Position.Y - (enemies[j].Texture.Height / 2)), enemies[j].Texture.Width, enemies[j].Texture.Height);
                    enemies[j].TextureData = new Color[enemies[j].Texture.Width * enemies[j].Texture.Height];
                    enemies[j].Texture.GetData(enemies[j].TextureData);
                    enemies[j].ShieldSpark = new ShieldSparkParticleSystem(this, 3);
                    Components.Add(enemies[j].ShieldSpark);
                    enemies[j].ParticleKill = new ParticleKillParticleSystem(this, 4);
                    Components.Add(enemies[j].ParticleKill);
                }

                Fused = new Particle[i_MaxNumberFused];
                for (int j = 0; j < i_MaxNumberFused; j++)
                {
                    Fused[j] = new Particle(levelBounds, this);
                    Fused[j].Fusion = new FusionParticleSystem(this, 20);
                    Components.Add(Fused[j].Fusion);
                    Fused[j].LoadTex(t_Fused);
                }

                Photons = new Particle[i_MaxNumberPhotons];
                for (int j = 0; j < i_MaxNumberPhotons; j++)
                {
                    Photons[j] = new Particle(levelBounds, this);
                    Photons[j].LoadTex(t_Photon);
                    Photons[j].Fusion = new FusionParticleSystem(this, 20);
                    Components.Add(Photons[j].Fusion);
                }
                Chlor = new Particle[i_MaxNumberChloro];
                for (int j = 0; j < i_MaxNumberChloro; j++)
                {
                    Chlor[j] = new Particle(levelBounds, this);
                    Chlor[j].LoadTex(t_Chlor);
                    Chlor[j].Fusion = new FusionParticleSystem(this, 20);
                    Components.Add(Chlor[j].Fusion);
                }

                bullets = new Bullet[i_BulletMax];
                tethers = new Bullet[i_BulletMax];
                for (int j = 0; j < i_BulletMax; j++)
                {
                    bullets[j] = new Bullet(t_Bullet);
                    tethers[j] = new Bullet(t_Tether);
                }
                stupidline = new Bullet[i_StupidLineMax];
                for (int j = 0; j < i_StupidLineMax; j++)
                {
                    stupidline[j] = new Bullet(t_Tether);
                }

                ship = new Ship(500, 350, t_Ship);
                ship.Turret = Content.Load<Texture2D>("Ship//turret");
                TetherState = TetherState.shooting;

                offset = ship.OffsetUpdate(offset);
                engineSmoke = new EngineParticleSystem(this, 9);
                Components.Add(engineSmoke);
                shipExplosion = new ExplosionParticleSystem(this, 9);
                Components.Add(shipExplosion);
            }
            else if (i == 1)
            {
                //NormalLevel
                levelBounds = new Rectangle(100, 100, 3896, 2104);
                i_EnemySpawnRate = 200;
                i_MaxNumberEnemies = 10;
                i_PhotonSpawnRate = 200;
                i_MaxNumberPhotons = 10;
                i_ChloroSpawnRate = 200;
                i_MaxNumberChloro = 10;
                i_MaxNumberFused = 100;
                f_Fushions = 0;
                i_ShieldPulseCounter = 0;
                i_shipAlpha = 0.5f;
                i_PulseRate = 60;

                enemies = new Enemy[i_MaxNumberEnemies];
                for (int j = 0; j < i_MaxNumberEnemies; j++)
                {
                    enemies[j] = new Enemy(this);
                    enemies[j].LoadTex(t_EnemySmall, t_EnemyBig);
                    enemies[j].Rectangle = new Rectangle(((int)enemies[j].Position.X - (enemies[j].Texture.Width / 2)), ((int)enemies[j].Position.Y - (enemies[j].Texture.Height / 2)), enemies[j].Texture.Width, enemies[j].Texture.Height);
                    enemies[j].TextureData = new Color[enemies[j].Texture.Width * enemies[j].Texture.Height];
                    enemies[j].Texture.GetData(enemies[j].TextureData);
                    enemies[j].ShieldSpark = new ShieldSparkParticleSystem(this, 3);
                    Components.Add(enemies[j].ShieldSpark);
                    enemies[j].ParticleKill = new ParticleKillParticleSystem(this, 4);
                    Components.Add(enemies[j].ParticleKill);
                }

                Fused = new Particle[i_MaxNumberFused];
                for (int j = 0; j < i_MaxNumberFused; j++)
                {
                    Fused[j] = new Particle(levelBounds, this);
                    Fused[j].Fusion = new FusionParticleSystem(this, 20);
                    Components.Add(Fused[j].Fusion);
                    Fused[j].LoadTex(t_Fused);
                }

                Photons = new Particle[i_MaxNumberPhotons];
                for (int j = 0; j < i_MaxNumberPhotons; j++)
                {
                    Photons[j] = new Particle(levelBounds, this);
                    Photons[j].LoadTex(t_Photon);
                    Photons[j].Fusion = new FusionParticleSystem(this, 20);
                    Components.Add(Photons[j].Fusion);
                }
                Chlor = new Particle[i_MaxNumberChloro];
                for (int j = 0; j < i_MaxNumberChloro; j++)
                {
                    Chlor[j] = new Particle(levelBounds, this);
                    Chlor[j].LoadTex(t_Chlor);
                    Chlor[j].Fusion = new FusionParticleSystem(this, 20);
                    Components.Add(Chlor[j].Fusion);
                }

                bullets = new Bullet[i_BulletMax];
                tethers = new Bullet[i_BulletMax];
                for (int j = 0; j < i_BulletMax; j++)
                {
                    bullets[j] = new Bullet(t_Bullet);
                    tethers[j] = new Bullet(t_Tether);
                }
                ship = new Ship(500, 350, t_Ship);
                ship.Turret = Content.Load<Texture2D>("Ship//turret");
                TetherState = TetherState.shooting;

                offset = ship.OffsetUpdate(offset);
                engineSmoke = new EngineParticleSystem(this, 9);
                Components.Add(engineSmoke);
                shipExplosion = new ExplosionParticleSystem(this, 9);
                Components.Add(shipExplosion);
            }
        }
        public void GamePlay(GameTime gameTime)
        {
            if (ship.Tethering == true)
            {
                if (ship.IsPhoton == true)
                {
                    if (Photons[ship.TetheredParticleID].Attacker != null)
                    {
                        Photons[ship.TetheredParticleID].Attacker.i_KillingCounter = 0;
                        Photons[ship.TetheredParticleID].Attacker.f_colour = 1.0f;
                        Photons[ship.TetheredParticleID].Attacker.attacking = false;
                        Photons[ship.TetheredParticleID].Attacker.f_Rotation = 0;
                    }

                    Photons[ship.TetheredParticleID].BeingAttacked = false;
                    Photons[ship.TetheredParticleID].Attacker = null;
                    if (Photons[ship.TetheredParticleID].IsTethered == false)
                    {
                        Photons[ship.TetheredParticleID].Color = Color.White;
                    }
                }
                else if (ship.IsPhoton == false)
                {
                    if (Chlor[ship.TetheredParticleID].Attacker != null)
                    {
                        Chlor[ship.TetheredParticleID].Attacker.i_KillingCounter = 0;
                        Chlor[ship.TetheredParticleID].Attacker.f_colour = 1.0f;
                        Chlor[ship.TetheredParticleID].Attacker.attacking = false;
                        Chlor[ship.TetheredParticleID].Attacker.f_Rotation = 0;
                    }

                    Chlor[ship.TetheredParticleID].BeingAttacked = false;
                    Chlor[ship.TetheredParticleID].Attacker = null;
                    if (Chlor[ship.TetheredParticleID].IsTethered == false)
                    {
                        Chlor[ship.TetheredParticleID].Color = Color.White;
                    }
                }
            }
            gamepadStateCurr = GamePad.GetState(PlayerIndex.One);
            if (dead == false)
            {
                ship.ShipMovement(gamepadStateCurr, offset);
                FireCheck();
            }
            PauseCheck();
            #region Enemy Movement
            for (int i = 0; i < i_MaxNumberEnemies; i++)
            {
                if (enemies[i].Alive == true)
                {
                    enemies[i].EnemyMovement(ship, Photons, Chlor, i_MaxNumberPhotons, i_MaxNumberChloro, this);
                }
            }
            #endregion
            #region Collision
            //if Photon collide with Chlor,  enemy1, enemy2

            //if Chlor collide with enemy1, enemy2

            //if Fused collide with enemy1, enemy2

            //if enemy1 collide with enemy2

            vibrateCounter = 0;

            //SHIP COLLISION
            #region SHIP COLLISION
            //EDGECOLLISION(ship, levelBounds);
            EDGECOLLISION(ship, levelBounds);
            for (int i = 0; i < Photons.Length; i++)
            {
                if (Photons[i].ParticleState == Particle.State.Alive)
                {
                    COLLISION(ship, Photons[i]);
                }
            }

            for (int i = 0; i < Chlor.Length; i++)
            {
                if (Chlor[i].ParticleState == Particle.State.Alive)
                {
                    COLLISION(ship, Chlor[i]);
                }
            }

            if (ship.Hit == false)
            {
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i].Alive == true && ship.Hit == false)
                    {
                        COLLISION(ship, enemies[i]);
                    }
                }
            }

            #endregion
            //PHOTON COLLISION
            #region PHOTON COLLISION

            for (int i = 0; i < Photons.Length; i++)
            {
                if (Photons[i].ParticleState == Particle.State.Alive)
                {
                    EDGECOLLISION(Photons[i], levelBounds);
                    for (int j = 0; j < Photons.Length; j++)
                    {
                        if (i != j)
                        {
                            COLLISION(Photons[i], Photons[j], false);
                        }
                    }
                }
            }

            for (int i = 0; i < Photons.Length; i++)
            {
                for (int j = 0; j < Chlor.Length; j++)
                {
                    if (Photons[i].ParticleState == Particle.State.Alive && Chlor[j].ParticleState == Particle.State.Alive)
                    {
                        //FusionCode
                        COLLISION(Photons[i], Chlor[j], true);
                    }
                }
            }

            for (int i = 0; i < Photons.Length; i++)
            {
                for (int j = 0; j < enemies.Length; j++)
                {
                    if (enemies[j].Alive == true && Photons[i].ParticleState == Particle.State.Alive && Photons[i].IsTethered == false)
                    {
                        COLLISION(enemies[j], Photons[i]);
                    }
                }
            }


            #endregion
            //CHLOR COLLISION
            #region CHLOR COLLISION
            for (int i = 0; i < Chlor.Length; i++)
            {
                if (Chlor[i].ParticleState == Particle.State.Alive)
                {
                    EDGECOLLISION(Chlor[i], levelBounds);
                    for (int j = 0; j < Chlor.Length; j++)
                    {
                        if (i != j)
                        {
                            COLLISION(Chlor[i], Chlor[j], false);
                        }
                    }
                }
            }

            for (int i = 0; i < Chlor.Length; i++)
            {
                for (int j = 0; j < enemies.Length; j++)
                {
                    if (enemies[j].Alive == true && Chlor[i].ParticleState == Particle.State.Alive && Photons[i].IsTethered == false)
                    {
                        COLLISION(enemies[j], Chlor[i]);
                    }
                }
            }
            #endregion
            //ENEMY COLLISION
            #region ENEMY COLLISION
            for (int i = 0; i < enemies.Length; i++)
            {
                EDGECOLLISION(enemies[i], levelBounds);
                for (int j = 0; j < enemies.Length; j++)
                {
                    if (i != j)
                    {
                        if (enemies[i].Alive == true && enemies[j].Alive == true)
                        {
                            COLLISION(enemies[i], enemies[j]);
                        }
                    }
                }
            }
            #endregion
            //BULLET COLLISION
            #region BULLET COLLISION
            for (int i = 0; i < enemies.Length; i++)
            {
                for (int j = 0; j < bullets.Length; j++)
                {
                    if (enemies[i].Alive == true && bullets[j].Alive == true)
                    {
                        COLLISION(enemies[i], bullets[j]);
                    }
                }
            }
            #endregion
            //TETHER COLLISION
            #region TETHER COLLISION

            for (int i = 0; i < tethers.Length; i++)
            {
                for (int j = 0; j < Photons.Length; j++)
                {
                    if (tethers[i].Alive)
                    {
                        if (tethers[i].Position.X < (ship.Position.X - 200))
                        {
                            tethers[i].Alive = false;
                        }
                        else if (tethers[i].Position.X > (ship.Position.X + 200))
                        {
                            tethers[i].Alive = false;
                        }
                        else if (tethers[i].Position.Y < (ship.Position.Y - 200))
                        {
                            tethers[i].Alive = false;
                        }
                        else if (tethers[i].Position.Y > (ship.Position.Y + 200))
                        {
                            tethers[i].Alive = false;
                        }
                        else
                        {
                            COLLISION(tethers[i], Photons[j], j, true);
                            COLLISION(tethers[i], Chlor[j], j, false);
                        }
                    }
                }
            }

            #endregion

            GamePad.SetVibration(PlayerIndex.One, (0.2f * vibrateCounter), (0.2f * vibrateCounter));
            #endregion
            for (int i = 0; i < i_BulletMax; i++)
            {
                if (bullets[i].Alive == true)
                {
                    bullets[i].BulletMovement(ship);
                    bullets[i].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, 1.0f);
                }
                if (tethers[i].Alive == true)
                {
                    tethers[i].BulletMovement(ship);
                    tethers[i].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, 1.0f);
                }

            }

            for (int i = 0; i < Fused.Length; i++)
            {
                if (Fused[i].ParticleState == Particle.State.Spawning)
                {
                    if (Fused[i].IsFusion == true)
                    {
                        UpdateFusionEffect(new Vector2((Fused[i].Position.X + (Fused[i].Texture.Width / 2)), (Fused[i].Position.Y - 15 + (Fused[i].Texture.Height / 2))), Fused[i]);
                        Fused[i].FusionCounter++;
                        if (Fused[i].FusionCounter == 70)
                        {
                            f_Fushions++;
                            Fused[i].IsFusion = false;
                            Fused[i].FusionCounter = 0;
                            Fused[i].ParticleState = Particle.State.Alive;
                            if (gameState == State.Tutorial)
                            {
                                b_TutorialFusion = true;
                            }
                        }
                    }
                }
                if (Fused[i].ParticleState == Particle.State.Alive)
                {
                    Fused[i].Velocity += new Vector2((((4096 - Fused[i].Position.X) / Vector2.Distance(Fused[i].Position, new Vector2(4096, 1152))) * 10), (((1152 - Fused[i].Position.Y) / Vector2.Distance(Fused[i].Position, new Vector2(4096, 1152)))) * 10);
                    Fused[i].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction);
                }
            }

            UpdateEngineSmoke();
            ShieldPulse();
            ship.updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction, TetherState);

            if (ship.Tethering == true)
            {
                for (int i = 0; i < i_StupidLineMax; i++)
                {
                    if (stupidline[i].Alive == true)
                    {
                        if (ship.IsPhoton == true)
                        {
                            stupidline[i].BulletMovement(Photons[ship.TetheredParticleID].Position, ship.Position, i, stupidline.Length);
                        }
                        else
                        {
                            stupidline[i].BulletMovement(Chlor[ship.TetheredParticleID].Position, ship.Position, i, stupidline.Length);
                        }
                        stupidline[i].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, 1);
                    }
                }
                for (int i = 0; i < i_StupidLineMax; i++)
                {
                    if (stupidline[i].Alive == false)
                    {
                        if (stupidline[i].i_SpawnTimer == 4)
                        {
                            stupidline[i].Alive = true;
                            stupidline[i].Position = ship.Position;
                            stupidline[i].i_SpawnTimer = 0;
                            break;
                        }
                        else
                        {
                            stupidline[i].i_SpawnTimer++;
                            break;
                        }
                    }
                }
            }
        }
        public void QuestionsUpdate()
        {
            #region Controller
            GamePadState gamepadStateCurr = GamePad.GetState(PlayerIndex.One);
            if (gamepadStateCurr.IsButtonDown(Buttons.Y))
            {
                if (!(gamepadStateOld.IsButtonDown(Buttons.Y)))
                {
                    if (i_CurrentQuestion < 5)
                    {
                        questions[i_CurrentQuestion].youranswer = 0;
                        if (questions[i_CurrentQuestion].correctAns == 0)
                        {
                            //Answered Correctly
                            i_CorrectAnswers++;
                        }
                        else
                        {
                            //Incorrect
                        }
                        b_Flash = true;
                        i_WhichFlash = 0;
                        soundBank.PlayCue("Confirm");
                    }
                    else
                    {
                    }
                }
            }
            if (gamepadStateCurr.IsButtonDown(Buttons.B))
            {
                if (!(gamepadStateOld.IsButtonDown(Buttons.B)))
                {
                    if (i_CurrentQuestion < 5)
                    {
                        questions[i_CurrentQuestion].youranswer = 1;
                        if (questions[i_CurrentQuestion].correctAns == 1)
                        {
                            //Answered Correctly
                            i_CorrectAnswers++;
                        }
                        else
                        {
                            //Incorrect
                        }
                        b_Flash = true;
                        i_WhichFlash = 1;
                        soundBank.PlayCue("Confirm");
                    }
                    else
                    {
                    }
                }
            }
            if (gamepadStateCurr.IsButtonDown(Buttons.A))
            {
                if (!(gamepadStateOld.IsButtonDown(Buttons.A)))
                {
                    if (i_CurrentQuestion < 5)
                    {
                        questions[i_CurrentQuestion].youranswer = 2;
                        if (questions[i_CurrentQuestion].correctAns == 2)
                        {
                            //Answered Correctly
                            i_CorrectAnswers++;
                        }
                        else
                        {
                            //Incorrect
                        }
                        b_Flash = true;
                        i_WhichFlash = 2;
                        soundBank.PlayCue("Confirm");
                    }
                    else
                    {
                        //Questions Ended
                        i_NewScore = (int)f_TotalScore;
                        Grade(i_NewScore);
                        s_NewGrade = s_grade;
                        UpdateHighScoreTable();
                        gameState = State.HighScore;
                    }
                }
            }
            if (gamepadStateCurr.IsButtonDown(Buttons.X))
            {
                if (!(gamepadStateOld.IsButtonDown(Buttons.X)))
                {
                    if (i_CurrentQuestion < 5)
                    {
                        questions[i_CurrentQuestion].youranswer = 3;
                        if (questions[i_CurrentQuestion].correctAns == 3)
                        {
                            //Answered Correctly
                            i_CorrectAnswers++;
                        }
                        else
                        {
                            //Incorrect
                        }
                        b_Flash = true;
                        i_WhichFlash = 3;
                    }
                    else
                    {
                    }
                    soundBank.PlayCue("Confirm");
                }
            }
            if (i_CurrentQuestion == 5)
            {
                f_QuizScore = 500 * i_CorrectAnswers;
                f_TotalScore = f_Score + f_QuizScore;
            }
            gamepadStateOld = gamepadStateCurr;
            #endregion
            #region Keyboard
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                if (last1 == false)
                {
                    if (i_CurrentQuestion < 5)
                    {
                        questions[i_CurrentQuestion].youranswer = 0;
                        if (questions[i_CurrentQuestion].correctAns == 0)
                        {
                            //Answered Correctly
                            i_CorrectAnswers++;
                        }
                        else
                        {
                            //Incorrect
                        }
                        b_Flash = true;
                        i_WhichFlash = 0;
                        soundBank.PlayCue("Confirm");
                    }
                    else
                    {
                    }
                }
                last1 = true;
            }
            else
            {
                last1 = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                if (last3 == false)
                {
                    if (i_CurrentQuestion < 5)
                    {
                        questions[i_CurrentQuestion].youranswer = 1;
                        if (questions[i_CurrentQuestion].correctAns == 1)
                        {
                            //Answered Correctly
                            i_CorrectAnswers++;
                        }
                        else
                        {
                            //Incorrect
                        }
                        b_Flash = true;
                        i_WhichFlash = 1;
                        soundBank.PlayCue("Confirm");
                    }
                    else
                    {
                    }
                }
                last3 = true;
            }
            else
            {
                last3 = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D4))
            {
                if (last4 == false)
                {
                    if (i_CurrentQuestion < 5)
                    {
                        questions[i_CurrentQuestion].youranswer = 2;
                        if (questions[i_CurrentQuestion].correctAns == 2)
                        {
                            //Answered Correctly
                            i_CorrectAnswers++;
                        }
                        else
                        {
                            //Incorrect
                        }
                        b_Flash = true;
                        i_WhichFlash = 2;
                        soundBank.PlayCue("Confirm");
                    }
                    else
                    {
                    }
                }
                last4 = true;
            }
            else
            {
                last4 = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                if (last2 == false)
                {
                    if (i_CurrentQuestion < 5)
                    {
                        questions[i_CurrentQuestion].youranswer = 3;
                        if (questions[i_CurrentQuestion].correctAns == 3)
                        {
                            //Answered Correctly
                            i_CorrectAnswers++;
                        }
                        else
                        {
                            //Incorrect
                        }
                        b_Flash = true;
                        i_WhichFlash = 3;
                    }
                    else
                    {
                    }
                    soundBank.PlayCue("Confirm");
                }
                last2 = true;
            }
            else
            {
                last2 = false;
            }
            if (i_CurrentQuestion == 5)
            {
                f_QuizScore = 500 * i_CorrectAnswers;
                f_TotalScore = f_Score + f_QuizScore;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (i_CurrentQuestion == 5)
                {
                    //Questions Ended
                    i_NewScore = (int)f_TotalScore;
                    Grade(i_NewScore);
                    s_NewGrade = s_grade;
                    UpdateHighScoreTable();
                    gameState = State.HighScore;
                }
            }
            #endregion
        }
    }
}
