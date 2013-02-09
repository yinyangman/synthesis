using System;
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
namespace Synthesis
{
    public class Tutorial : Level
    {
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

        #region Global Variables
        bool b_Grid = true;
        bool b_Pulsing = false;
        Texture2D t_Tutorial;
        Texture2D[] t_TutorialText;
        Texture2D t_TutorialOverlay;
        Texture2D t_Grid;
        Cue[] tutCues;
        Cue TutMusic;
        DateTime dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 00);
        int iTutorialState = 0;
        bool b_shield = false;

        int i_Number1Counter = 0;
        int i_Number2Counter = 0;
        int i_Number3Counter = 0;
        int i_Number4Counter = 0;
        int i_Number5Counter = 0;
        float funnyNo1 = 0;
        float funnyNo2 = 0;
        float funnyNo3 = 0;
        float funnyNo4 = 0;
        int funnyNoCounter1 = 0;
        int funnyNoCounter2 = 0;
        int funnyNoCounter3 = 0;
        int funnyNoCounter4 = 0;
        int i_TutorialTextCounter = 0;
        public bool b_TutorialFusion = false;
        int i_TutorialCounter = 0;
        public int tutBulletCounter = 0;
        float i_GridCounter = 0;
        int i_TextBubble = 1;
        bool b_Pulse = true;

        Vector2 totalPixelsTravelled;
        Vector2 lastShipPos;
        #endregion
        public override void Initialize(SoundBank soundBank)
        {
            TutMusic = soundBank.GetCue("tutMusic");

            levelBounds = new Rectangle(75, 75, 874, 460);
            i_EnemySpawnRate = 0;
            i_MaxNumberEnemies = 1;
            i_PhotonSpawnRate = 0;
            i_MaxNumberPhotons = 1;
            i_ChloroSpawnRate = 0;
            i_MaxNumberChloro = 1;
            i_MaxNumberFused = 1;

            tutCues = new Cue[11];
            for (int j = 0; j < 11; j++)
            {
                tutCues[j] = soundBank.GetCue((j + 1).ToString("000"));
            }
        }
        public override void LoadContent(ContentManager Content)
        {
            t_Grid = Content.Load<Texture2D>("Tutorial//Grid");
            t_TutorialText = new Texture2D[11];
            t_Tutorial = Content.Load<Texture2D>("Tutorial//Tutorial");
            t_TutorialOverlay = Content.Load<Texture2D>("Tutorial//TutorialOverlay");

            for (int i = 0; i < 11; i++)
            {
                t_TutorialText[i] = Content.Load<Texture2D>("Tutorial//tutorial_text_" + (i + 1));
            } 
        }
        public override void Update(GameTime gameTime, Engine engine, Game1 game)
        {

            if (engine.i_ShipGridCounter < 75 && b_Grid == true)
            {
                engine.i_ShipGridCounter++;
            }
            else if (engine.i_ShipGridCounter == 75 && b_Grid == true)
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
            else if (engine.i_ShipCounter < 75)
            {
                engine.i_ShipCounter++;
            }
            else if (engine.i_ShipCounter == 75 && b_Grid == false && engine.i_ShipGridCounter > 0)
            {
                engine.i_ShipGridCounter--;
            }
            else if (engine.i_ShipCounter == 75 && b_Grid == false && engine.i_ShipGridCounter == 0 && b_shield == false)
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
                #region Photon Updates
                if (engine.Photons[0].ParticleState == Particle.PState.Alive)
                {
                    if (engine.Photons[0].IsTethered)
                    {
                        if (!engine.ship.BoundSphere.Intersects(new BoundingSphere(new Vector3(engine.Photons[0].Position, 0), (engine.Photons[0].Texture.Width / 2))))
                        {
                            Vector2 distance = engine.ship.Position - engine.Photons[0].Position;
                            engine.Photons[0].Velocity += Vector2.Normalize(distance) * 20;
                        }
                    }
                    else
                    {
                        engine.Photons[0].Position -= new Vector2(0, 0.07f);
                    }
                    engine.Photons[0].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, engine.f_Friction);
                }
                else if (engine.Photons[0].ParticleState == Particle.PState.Spawning)
                {
                    if (engine.Photons[0].Scale > 1)
                    {
                        engine.Photons[0].Scale = 1.0f;
                        engine.Photons[0].ParticleState = Particle.PState.Alive;
                    }
                    else
                    {
                        engine.Photons[0].Scale += 0.02f;
                    }
                }
                else if (engine.Photons[0].ParticleState == Particle.PState.Fusing)
                {
                    if (engine.Photons[0].i_Fusing > 50)
                    {
                        engine.Photons[0].ParticleState = Particle.PState.Dead;
                        engine.Photons[0].i_Fusing = 0;
                    }
                    else
                    {
                        engine.Photons[0].i_Fusing++;
                    }
                }
                else if (engine.Photons[0].ParticleState == Particle.PState.Colliding)
                {
                    engine.Photons[0].Velocity = new Vector2((((engine.Photons[0].FusionPosition.X - engine.Photons[0].Position.X) / Vector2.Distance(engine.Photons[0].Position, engine.Photons[0].FusionPosition)) * 10), (((engine.Photons[0].FusionPosition.Y - engine.Photons[0].Position.Y) / Vector2.Distance(engine.Photons[0].Position, engine.Photons[0].FusionPosition)) * 10));
                    engine.Photons[0].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, engine.f_Friction);
                    if (engine.Photons[0].Position.X < (engine.Photons[0].FusionPosition.X + 1) && engine.Photons[0].Position.X > (engine.Photons[0].FusionPosition.X - 1) && engine.Photons[0].Position.Y < (engine.Photons[0].FusionPosition.Y + 1) && engine.Photons[0].Position.Y > (engine.Photons[0].FusionPosition.Y - 1))
                    {
                        for (int j = 0; j < i_MaxNumberFused; j++)
                        {
                            if (engine.Fused[j].ParticleState == Particle.PState.Dead)
                            {
                                engine.Fused[j].ParticleState = Particle.PState.Spawning;
                                engine.Photons[0].ParticleState = Particle.PState.Fusing;
                                engine.Fused[j].Position = engine.Photons[0].Position;
                                break;
                            }
                        }
                    }
                }
                #endregion
                #region engine.Chloro Updates
                if (engine.Chlor[0].ParticleState == Particle.PState.Alive)
                {
                    if (engine.Chlor[0].IsTethered)
                    {
                        if (!engine.ship.BoundSphere.Intersects(new BoundingSphere(new Vector3(engine.Chlor[0].Position, 0), (engine.Chlor[0].Texture.Width / 2))))
                        {
                            Vector2 distance = engine.ship.Position - engine.Chlor[0].Position;
                            engine.Chlor[0].Velocity += Vector2.Normalize(distance) * 20;
                        }
                    }
                    else
                    {
                        engine.Chlor[0].Position += new Vector2(0, 0.07f);
                    }
                    engine.Chlor[0].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, engine.f_Friction);
                }
                else if (engine.Chlor[0].ParticleState == Particle.PState.Fusing)
                {
                    if (engine.Chlor[0].i_Fusing > 50)
                    {
                        engine.Chlor[0].ParticleState = Particle.PState.Dead;
                        engine.Chlor[0].i_Fusing = 0;
                    }
                    else
                    {
                        engine.Chlor[0].i_Fusing++;
                    }
                }
                else if (engine.Chlor[0].ParticleState == Particle.PState.Colliding)
                {
                    engine.Chlor[0].Velocity = new Vector2((((engine.Chlor[0].FusionPosition.X - engine.Chlor[0].Position.X) / Vector2.Distance(engine.Chlor[0].Position, engine.Chlor[0].FusionPosition)) * 10), (((engine.Chlor[0].FusionPosition.Y - engine.Chlor[0].Position.Y) / Vector2.Distance(engine.Chlor[0].Position, engine.Chlor[0].FusionPosition)) * 10));
                    engine.Chlor[0].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, engine.f_Friction);

                    if (engine.Chlor[0].Position.X < (engine.Chlor[0].FusionPosition.X + 1) && engine.Chlor[0].Position.X > (engine.Chlor[0].FusionPosition.X - 1) && engine.Chlor[0].Position.Y < (engine.Chlor[0].FusionPosition.Y + 1) && engine.Chlor[0].Position.Y > (engine.Chlor[0].FusionPosition.Y - 1))
                    {
                        engine.Chlor[0].ParticleState = Particle.PState.Fusing;
                    }
                }
                else if (engine.Chlor[0].ParticleState == Particle.PState.Spawning)
                {
                    if (engine.Chlor[0].Scale > 1)
                    {
                        engine.Chlor[0].Scale = 1.0f;
                        engine.Chlor[0].ParticleState = Particle.PState.Alive;
                    }
                    else
                    {
                        engine.Chlor[0].Scale += 0.02f;
                    }
                }
                #endregion
                #region Enemy Updates
                if (engine.enemies[0].EnemyCollision == true)
                {
                    engine.UpdateShieldSpark(engine.enemies[0].EnemyCollisionPosition, engine.enemies[0]);
                    engine.enemies[0].ShieldSparkCounter++;
                    if (engine.enemies[0].ShieldSparkCounter == 40)
                    {
                        engine.enemies[0].EnemyCollision = false;
                        engine.enemies[0].ShieldSparkCounter = 0;
                    }
                }

                if (engine.enemies[0].IsParticleKill == true)
                {
                    engine.UpdateParticleKill(engine.enemies[0].ParticleKillPosition, engine.enemies[0].p_ParticleKilled, engine.enemies[0]);
                    engine.enemies[0].ParticleKillCounter++;
                    if (engine.enemies[0].ParticleKillCounter == 40)
                    {
                        engine.enemies[0].IsParticleKill = false;
                        engine.enemies[0].ParticleKillCounter = 0;
                    }
                }
                if (engine.enemies[0].Alive == true)
                {
                    engine.enemies[0].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, engine.f_Friction);
                }
                #endregion
            }
            
            dt_TutorialTimer += gameTime.ElapsedGameTime;
            FunnyNumbers();

            if (b_Pulsing == true)
            {
                Pulsing();
            }

            if (TutMusic.IsPlaying == false)
            {
                TutMusic.Play();
            }
            else if (TutMusic.IsPaused == true)
            {
                TutMusic.Resume();
            }

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
                    (engine.ship.Position.X - lastShipPos.X) *
                    (engine.ship.Position.X - lastShipPos.X));

                totalPixelsTravelled.Y += (float)Math.Sqrt(
                    (engine.ship.Position.Y - lastShipPos.Y) *
                    (engine.ship.Position.Y - lastShipPos.Y));

                if ((totalPixelsTravelled.X + totalPixelsTravelled.Y) > 3000)
                {
                    tutCues[1].Play();
                    i_TextBubble = 2;
                    iTutorialState++;
                    dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                    engine.Photons[0] = new Particle(new Vector2(768, 350));
                    engine.Photons[0].LoadTex(engine.t_Photon);
                    engine.Photons[0].ParticleState = Particle.PState.Spawning;
                    engine.Photons[0].Spawn(new Vector2(768, 350));
                    engine.Photons[0].SpawnTimer = 0;
                    game.soundBank.PlayCue("photonPop");
                }

                lastShipPos = engine.ship.Position;
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
                if (engine.tetherState == Engine.TetherState.tethered && dt_TutorialTimer.Second >= 11)
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
                if (engine.tetherState == Engine.TetherState.detethered && dt_TutorialTimer.Second >= 6)
                {
                    tutCues[4].Play();
                    i_TextBubble = 5;
                    iTutorialState++;
                    engine.Photons[0].ParticleState = Particle.PState.Dead;
                    dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                    engine.Chlor[0] = new Particle(new Vector2(768, 350));
                    engine.Chlor[0].LoadTex(engine.t_Chlor);
                    engine.Chlor[0].ParticleState = Particle.PState.Spawning;
                    engine.Chlor[0].Spawn(new Vector2(768, 350));
                    engine.Chlor[0].SpawnTimer = 0;
                    game.soundBank.PlayCue("chlorPop");
                }
            }
            else if (iTutorialState == 6)
            {
                if (engine.tetherState == Engine.TetherState.tethered && engine.ship.IsPhoton == false && dt_TutorialTimer.Second >= 8)
                {
                    tutCues[5].Play();
                    i_TextBubble = 6;
                    iTutorialState++;
                    dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                    engine.Photons[0] = new Particle(new Vector2(768, 350));
                    engine.Photons[0].LoadTex(engine.t_Photon);
                    engine.Photons[0].ParticleState = Particle.PState.Spawning;
                    engine.Photons[0].Spawn(new Vector2(768, 350));
                    engine.Photons[0].SpawnTimer = 0;
                    game.soundBank.PlayCue("photonPop");
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
                    engine.enemies[0] = new Enemy(game, 0);
                    engine.enemies[0].LoadTex(engine.t_EnemySmall, engine.t_EnemyBig);
                    engine.enemies[0].Spawn(new Vector2(900, 100));
                    engine.enemies[0].SpawnTimer = 0;
                    engine.enemies[0].Rectangle = new Rectangle(((int)engine.enemies[0].Position.X - (engine.enemies[0].Texture.Width / 2)), ((int)engine.enemies[0].Position.Y - (engine.enemies[0].Texture.Height / 2)), engine.enemies[0].Texture.Width, engine.enemies[0].Texture.Height);
                    engine.enemies[0].TextureData = new Color[engine.enemies[0].Texture.Width * engine.enemies[0].Texture.Height];
                    engine.enemies[0].Texture.GetData(engine.enemies[0].TextureData);

                    i_TextBubble = 9;
                    iTutorialState++;
                    dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                }
            }
            else if (iTutorialState == 10)
            {
                if (dt_TutorialTimer.Second >= 10 && engine.enemies[0].Alive == false)
                {
                    tutCues[9].Play();
                    engine.enemies[0] = new Enemy(game, 1);
                    engine.enemies[0].LoadTex(engine.t_EnemySmall, engine.t_EnemyBig);
                    engine.enemies[0].Spawn(new Vector2(900, 100));
                    engine.enemies[0].SpawnTimer = 0;
                    engine.enemies[0].Rectangle = new Rectangle(((int)engine.enemies[0].Position.X - (engine.enemies[0].Texture.Width / 2)), ((int)engine.enemies[0].Position.Y - (engine.enemies[0].Texture.Height / 2)), engine.enemies[0].Texture.Width, engine.enemies[0].Texture.Height);
                    engine.enemies[0].TextureData = new Color[engine.enemies[0].Texture.Width * engine.enemies[0].Texture.Height];
                    engine.enemies[0].Texture.GetData(engine.enemies[0].TextureData);

                    engine.Chlor[0] = new Particle(new Vector2(768, 350));
                    engine.Chlor[0].LoadTex(engine.t_Chlor);
                    engine.Chlor[0].ParticleState = Particle.PState.Spawning;
                    engine.Chlor[0].Spawn(new Vector2(768, 350));
                    engine.Chlor[0].SpawnTimer = 0;
                    game.soundBank.PlayCue("chlorPop");
                    i_TextBubble = 10;
                    iTutorialState++;
                    dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                }
            }
            else if (iTutorialState == 11)
            {
                if (engine.enemies[0].Alive == false && dt_TutorialTimer.Second >= 9)
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
                if (game.by_BlackCounter == 75)
                {
                    game.gameState = State.LevelIntro;
                    TutMusic.Pause();
                    dt_TutorialTimer = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                }
                else
                {
                    game.by_BlackCounter++;
                    TutMusic.SetVariable("Volume", (100 - (game.by_BlackCounter * (1 + (1 / 3)))));
                }
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Engine engine, Game1 game)
        {
            spriteBatch.Draw(t_Tutorial, new Vector2(0, 0), new Color(1, 1, 1, (i_TutorialCounter / 100f)));
            spriteBatch.Draw(t_Grid, new Vector2(0, 0), new Color(1, 1, 1, (i_GridCounter / 100f)));
            spriteBatch.DrawString(game.fontSmallText, "Tachyon Emissions: " + funnyNo1 + "htz", new Vector2(32, 29), new Color(0, 1, 1, (i_Number2Counter / 20f)));
            spriteBatch.DrawString(game.fontSmallText, "Tractor Beam Cohesion: " + funnyNo2 + "%", new Vector2(32, 44), new Color(0, 1, 1, (i_Number3Counter / 20f)));
            spriteBatch.DrawString(game.fontSmallText, "Shield Matrix Variance: " + funnyNo3, new Vector2(32, 60), new Color(0, 1, 1, (i_Number4Counter / 20f)));
            spriteBatch.DrawString(game.fontSmallText, "AVA Targeting Alignment: " + funnyNo4, new Vector2(32, 75), new Color(0, 1, 1, (i_Number5Counter / 20f)));

            if (b_shield == true)
            {
                engine.engineSmoke.DrawParticle(gameTime, engine.offset);
                spriteBatch.Draw(engine.t_EngineSmoke, (engine.ship.Position + engine.offset + (Vector2.Transform(new Vector2(-9, 45), Matrix.CreateRotationZ((float)engine.ship.NextRotation)))), null, Color.Aqua, (float)engine.ship.NextRotation, new Vector2(0, 0), new Vector2(1.0f, 1.0f), SpriteEffects.None, 0);
                spriteBatch.Draw(engine.t_Shield, (engine.ship.Position + engine.offset), null, engine.Shield(), (float)engine.ship.NextRotation, new Vector2(30, 50), engine.v_ShieldSize, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(t_TutorialOverlay, new Vector2(0, 0), new Color(1, 1, 1, (i_GridCounter / 75f)));
            spriteBatch.DrawString(game.fontName, "Synthesis Training Program: Ver 4.7", new Vector2(280, 0), new Color(0, 1, 1, (i_Number1Counter / 20f)));
            spriteBatch.Draw(t_TutorialText[(i_TextBubble - 1)], new Vector2(25, 580), new Color(1, 1, 1, (i_TutorialTextCounter / 75f)));
        }

        void FunnyNumbers()
        {
            if (funnyNoCounter1 == 5)
            {
                funnyNo1 = Game1.Random.Next(1775, 1799);
                funnyNoCounter1 = 0;
            }
            else
            {
                funnyNoCounter1++;
            }
            if (funnyNoCounter2 == 80)
            {
                funnyNo2 = Game1.Random.Next(85, 92);
                funnyNoCounter2 = 0;
            }
            else
            {
                funnyNoCounter2++;
            }
            if (funnyNoCounter3 == 20)
            {
                funnyNo3 = (Game1.Random.Next(165, 189) / 1000f);
                funnyNoCounter3 = 0;
            }
            else
            {
                funnyNoCounter3++;
            }
            if (funnyNoCounter4 == 35)
            {
                funnyNo4 = (Game1.Random.Next(-30, 30) / 1000f);
                funnyNoCounter4 = 0;
            }
            else
            {
                funnyNoCounter4++;
            }
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
    }
}
