using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using SynthesisGameLibrary;

namespace Synthesis
{
    public class Engine
    {
        public enum TetherState
        {
            shooting,
            tethered,
            detethered
        }

        #region Global Variables

        //Entities
        public Ship ship;
        public Enemy[] enemies;
        public Particle[] Photons;
        public Particle[] Chlor;
        public Particle[] Fused;
        public Bullet[] bullets;
        public Bullet[] tethers;
        public Bullet[] stupidline;

        //Level Stats
        Level loadedLevel;

        //Gameplay Stats
        public float f_Fusions = 0;
        public TetherState tetherState = TetherState.shooting;
        public Vector2 offset;
        bool dead = false;
        int i_ExplosionTimer = 0;
        public int i_ShipCounter = 0;
        public int i_ShipGridCounter = 0;

        //Timer Variables
        DateTime dt_timer = new DateTime(1901, 1, 1, 0, 5, 00);
        int segments = 8;

        //Turret Variabels
        Vector2 v_TurretDirection = new Vector2(0, 1);
        int i_FireRate = 7;
        int i_BulletMax = 1000;
        int i_StupidLineMax = 20;

        //Shield Variables
        public Vector2 v_ShieldSize = new Vector2(1.20f, 1.20f);
        public int i_ShieldPulseCounter = 0;
        public float i_shipAlpha = 0.5f;
        public int i_PulseRate = 60;
        bool b_Pulsing = false;
        public bool b_shield = false;

        //Textures
        public Texture2D t_Ship;
        public Texture2D t_ShipGrid;
        public Texture2D t_Photon;
        public Texture2D t_Chlor;
        public Texture2D t_Fused;
        public Texture2D t_EnemySmall;
        public Texture2D t_EnemyBig;
        public Texture2D t_EngineSmoke;
        public Texture2D t_ClockBase;
        public Texture2D[] t_ClockSegFill;
        public Texture2D t_Shield;
        public Texture2D t_Bullet;
        public Texture2D t_Tether;

        public Texture2D t_Crosshair;

        //Audio
        public Cue Ambient;

        //Misc
        public float f_Friction = 0.987f;
        public float f_EdgeDamper = 0.7f;
        public int i_VibrateCounter = 0;

        //Particle Systems
        public EngineParticleSystem engineSmoke;
        public ExplosionParticleSystem shipExplosion;

        #endregion

        public Engine()
        {
        }
        public void Initialize(SoundBank soundBank)
        {
            Ambient = soundBank.GetCue("ambient");
        }
        public void LoadContent(ContentManager Content)
        {
            t_Ship = Content.Load<Texture2D>("Ship//Ship");
            t_Photon = Content.Load<Texture2D>("Particles//Photon");
            t_Chlor = Content.Load<Texture2D>("Particles//Chloroplasts");
            t_Fused = Content.Load<Texture2D>("Particles//Fused");
            t_EnemySmall = Content.Load<Texture2D>("Enemies//Enemy_Small");
            t_EnemyBig = Content.Load<Texture2D>("Enemies//Enemy_Big");
            t_EngineSmoke = Content.Load<Texture2D>("Ship//engine_smoke");
            t_Shield = Content.Load<Texture2D>("Ship//ShieldTemp");
            t_Bullet = Content.Load<Texture2D>("Ship//Bullet");
            t_Tether = Content.Load<Texture2D>("Ship//tether");
            t_ShipGrid = Content.Load<Texture2D>("Ship//ShipGrid");
            t_Crosshair = Content.Load<Texture2D>("Gameplay//crosshair");

            t_ClockSegFill = new Texture2D[8];
            for (int i = 0; i < 8; i++)
            {
                t_ClockSegFill[i] = Content.Load<Texture2D>("Gameplay//Clock_" + (i + 1));
            }
            t_ClockBase = Content.Load<Texture2D>("GamePlay//ClockBase");
        }
        public void Update(GameTime gameTime, SoundBank soundBank, Game1 game)
        {
            if (game.gameState != State.Tutorial)
            {
                SegmentUpdate();

                if (Ambient.IsPlaying == false)
                {
                    Ambient.Play();
                }
                else if (Ambient.IsPaused == true)
                {
                    Ambient.Resume();
                }
                if (f_Fusions >= loadedLevel.levelData.i_TargetFusions)
                {
                    game.i_GameOverSel = 1;
                    game.f_LevelCompleteBonus = 500;
                    game.fd_Fader.BeginFadingToBlack(75, true, game);
                }
                if (dt_timer.Minute == 0 && dt_timer.Second == 0)
                {
                    game.fd_Fader.BeginFadingToBlack(75, true, game);
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
                for (int i = 0; i < loadedLevel.levelData.i_MaxNumberEnemies; i++)
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
                    else if (enemies[i].SpawnTimer == loadedLevel.levelData.i_EnemySpawnRate)
                    {
                        enemies[i] = new Enemy(game, loadedLevel);
                        enemies[i].LoadTexture(game.Content);
                        enemies[i].Spawn(offset, loadedLevel.levelBounds);
                        enemies[i].SpawnTimer = 0;
                        enemies[i].Rectangle = new Rectangle(((int)enemies[i].Position.X - (enemies[i].Texture.Width / 2)), ((int)enemies[i].Position.Y - (enemies[i].Texture.Height / 2)), enemies[i].Texture.Width, enemies[i].Texture.Height);
                        enemies[i].TextureData = new Color[enemies[i].Texture.Width * enemies[i].Texture.Height];
                        enemies[i].Texture.GetData(enemies[i].TextureData);
                    }
                }
                for (int i = 0; i < loadedLevel.levelData.i_MaxNumberEnemies; i++)
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
                    if (Photons[i].ParticleState == Particle.PState.Fusing)
                    {
                        if (Photons[i].i_Fusing > 50)
                        {
                            Photons[i].ParticleState = Particle.PState.Dead;
                            Photons[i].i_Fusing = 0;
                        }
                        else
                        {
                            Photons[i].i_Fusing++;
                        }
                    }
                    else if (Photons[i].ParticleState == Particle.PState.Colliding)
                    {
                        Photons[i].Velocity = new Vector2((((Photons[i].FusionPosition.X - Photons[i].Position.X) / Vector2.Distance(Photons[i].Position, Photons[i].FusionPosition)) * 10), (((Photons[i].FusionPosition.Y - Photons[i].Position.Y) / Vector2.Distance(Photons[i].Position, Photons[i].FusionPosition)) * 10));
                        Photons[i].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction);
                        if (Photons[i].Position.X < (Photons[i].FusionPosition.X + 1) && Photons[i].Position.X > (Photons[i].FusionPosition.X - 1) && Photons[i].Position.Y < (Photons[i].FusionPosition.Y + 1) && Photons[i].Position.Y > (Photons[i].FusionPosition.Y - 1))
                        {
                            for (int j = 0; j < loadedLevel.levelData.i_MaxNumberFused; j++)
                            {
                                if (Fused[j].ParticleState == Particle.PState.Dead)
                                {
                                    Fused[j].ParticleState = Particle.PState.Spawning;
                                    Photons[i].ParticleState = Particle.PState.Fusing;
                                    Fused[j].Position = Photons[i].Position;
                                    break;
                                }
                            }
                        }
                    }
                    else if (Photons[i].ParticleState == Particle.PState.Alive)
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
                    else if (Photons[i].ParticleState == Particle.PState.Spawning)
                    {
                        if (Photons[i].Scale > 1)
                        {
                            Photons[i].Scale = 1.0f;
                            Photons[i].ParticleState = Particle.PState.Alive;
                        }
                        else
                        {
                            Photons[i].Scale += 0.02f;
                        }
                    }
                    else if (Photons[i].SpawnTimer == loadedLevel.levelData.i_PhotonSpawnRate)
                    {
                        Photons[i] = new Particle(loadedLevel.levelBounds);
                        Photons[i].LoadTex(t_Photon);
                        Photons[i].ParticleState = Particle.PState.Spawning;
                        Photons[i].Spawn(offset, loadedLevel.levelBounds, true);
                        Photons[i].SpawnTimer = 0;
                        soundBank.PlayCue("photonPop");
                    }
                }

                for (int i = 0; i < Chlor.Length; i++)
                {
                    if (Chlor[i].ParticleState == Particle.PState.Fusing)
                    {
                        if (Chlor[i].i_Fusing > 50)
                        {
                            Chlor[i].ParticleState = Particle.PState.Dead;
                            Chlor[i].i_Fusing = 0;
                        }
                        else
                        {
                            Chlor[i].i_Fusing++;
                        }
                    }
                    else if (Chlor[i].ParticleState == Particle.PState.Colliding)
                    {
                        Chlor[i].Velocity = new Vector2((((Chlor[i].FusionPosition.X - Chlor[i].Position.X) / Vector2.Distance(Chlor[i].Position, Chlor[i].FusionPosition)) * 10), (((Chlor[i].FusionPosition.Y - Chlor[i].Position.Y) / Vector2.Distance(Chlor[i].Position, Chlor[i].FusionPosition)) * 10));
                        Chlor[i].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction);

                        if (Chlor[i].Position.X < (Chlor[i].FusionPosition.X + 1) && Chlor[i].Position.X > (Chlor[i].FusionPosition.X - 1) && Chlor[i].Position.Y < (Chlor[i].FusionPosition.Y + 1) && Chlor[i].Position.Y > (Chlor[i].FusionPosition.Y - 1))
                        {
                            Chlor[i].ParticleState = Particle.PState.Fusing;
                        }
                    }
                    else if (Chlor[i].ParticleState == Particle.PState.Alive)
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
                    else if (Chlor[i].ParticleState == Particle.PState.Spawning)
                    {
                        if (Chlor[i].Scale > 1)
                        {
                            Chlor[i].Scale = 1.0f;
                            Chlor[i].ParticleState = Particle.PState.Alive;
                        }
                        else
                        {
                            Chlor[i].Scale += 0.02f;
                        }
                    }
                    else if (Chlor[i].SpawnTimer == loadedLevel.levelData.i_ChloroSpawnRate)
                    {
                        Chlor[i] = new Particle(loadedLevel.levelBounds);
                        Chlor[i].LoadTex(t_Chlor);
                        Chlor[i].ParticleState = Particle.PState.Spawning;
                        Chlor[i].Spawn(offset, loadedLevel.levelBounds, false);
                        Chlor[i].SpawnTimer = 0;
                        soundBank.PlayCue("chlorPop");
                    }
                }

                for (int i = 0; i < loadedLevel.levelData.i_MaxNumberPhotons; i++)
                {
                    if (Photons[i].ParticleState == Particle.PState.Dead)
                    {
                        Photons[i].SpawnTimer++;
                        break;
                    }
                }
                for (int i = 0; i < loadedLevel.levelData.i_MaxNumberChloro; i++)
                {
                    if (Chlor[i].ParticleState == Particle.PState.Dead)
                    {
                        Chlor[i].SpawnTimer++;
                        break;
                    }
                }
                #endregion
            }

            GamePlay(gameTime, game);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Game1 game)
        {
            if (game.gameState != State.Tutorial)
            {
                spriteBatch.DrawString(game.font, f_Fusions.ToString("00") + "/" + loadedLevel.levelData.i_TargetFusions.ToString("00"), new Vector2(46, 727), Color.DarkGray);
                spriteBatch.DrawString(game.font, f_Fusions.ToString("00") + "/" + loadedLevel.levelData.i_TargetFusions.ToString("00"), new Vector2(47, 728), Color.White);
                spriteBatch.Draw(t_ClockBase, new Vector2(15, 650), Color.White);
                spriteBatch.DrawString(game.font, dt_timer.Minute.ToString("0") + ":" + dt_timer.Second.ToString("00"), new Vector2(25, 668), Color.Black);
                spriteBatch.Draw(t_Fused, new Vector2(10, 725), null, Color.White, 0, new Vector2(0, 0), new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
                for (int i = 0; i < 8; i++)
                {
                    if (i < segments)
                    {
                        spriteBatch.Draw(t_ClockSegFill[7 - i], new Vector2(15, 650), Color.White);
                    }
                }
            }

            engineSmoke.DrawParticle(gameTime, offset);
            spriteBatch.Draw(t_EngineSmoke, (ship.Position + offset + (Vector2.Transform(new Vector2(-9, 45), Matrix.CreateRotationZ((float)ship.NextRotation)))), null, Color.Aqua, (float)ship.NextRotation, new Vector2(0, 0), new Vector2(1.0f, 1.0f), SpriteEffects.None, 0);

            if (b_shield)
            {
                spriteBatch.Draw(t_Shield, (ship.Position + offset), null, Shield(), (float)ship.NextRotation, new Vector2(30, 50), v_ShieldSize, SpriteEffects.None, 0);
            }

            if (tetherState == Engine.TetherState.tethered)
            {
                for (int i = 0; i < i_StupidLineMax; i++)
                {
                    if (stupidline[i].Alive == true && tetherState == Engine.TetherState.tethered)
                    {
                        spriteBatch.Draw(stupidline[i].Texture, stupidline[i].Position + offset, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    }
                }
            }
            for (int i = 0; i < Photons.Length; i++)
            {
                if (Photons[i].ParticleState == Particle.PState.Alive || Photons[i].ParticleState == Particle.PState.Spawning || Photons[i].ParticleState == Particle.PState.Colliding || Photons[i].ParticleState == Particle.PState.Fusing)
                {
                    spriteBatch.Draw(Photons[i].Texture, Photons[i].Position + offset, null, Photons[i].Color, 0, new Vector2(Photons[i].Texture.Width / 2, Photons[i].Texture.Height / 2), Photons[i].Scale, SpriteEffects.None, 0);
                }
            }
            for (int i = 0; i < Chlor.Length; i++)
            {
                if (Chlor[i].ParticleState == Particle.PState.Alive || Chlor[i].ParticleState == Particle.PState.Spawning || Chlor[i].ParticleState == Particle.PState.Colliding || Chlor[i].ParticleState == Particle.PState.Fusing)
                {
                    spriteBatch.Draw(Chlor[i].Texture, Chlor[i].Position + offset, null, Chlor[i].Color, 0, new Vector2(Chlor[i].Texture.Width / 2, Chlor[i].Texture.Height / 2), Chlor[i].Scale, SpriteEffects.None, 0);
                }
            }
            for (int i = 0; i < Fused.Length; i++)
            {
                if (Fused[i].ParticleState == Particle.PState.Alive)
                {
                    spriteBatch.Draw(Fused[i].Texture, Fused[i].Position + offset, null, Fused[i].Color, 0, new Vector2(Fused[i].Texture.Width / 2, Fused[i].Texture.Height / 2), 1, SpriteEffects.None, 0);
                }
                Fused[i].Fusion.DrawParticle(gameTime, offset);

            }

            for (int i = 0; i < loadedLevel.levelData.i_MaxNumberEnemies; i++)
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
                Color shipColor = ship.Color;
                shipColor.A = (byte)((i_ShipCounter / 75f)*255);

                Color shipGridColor = ship.Color;
                shipGridColor.A = (byte)((i_ShipGridCounter / 75f)*255);

                spriteBatch.Draw(ship.Texture, (ship.Position + offset), null, shipColor, (float)ship.NextRotation, new Vector2(30, 50), new Vector2(1.0f, 1.0f), SpriteEffects.None, 0);
                spriteBatch.Draw(ship.Turret, (ship.Position + offset + Vector2.Transform(new Vector2(0, 20), Matrix.CreateRotationZ((float)ship.NextRotation))), null, shipColor, (float)ship.NextRotationTurret, new Vector2(12f, 18f), new Vector2(1.0f, 1.0f), SpriteEffects.None, 0);
                spriteBatch.Draw(t_ShipGrid, (ship.Position + offset), null, shipGridColor, (float)ship.NextRotation, new Vector2(30, 50), new Vector2(1.0f, 1.0f), SpriteEffects.None, 0);
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
                    game.fd_Fader.BeginFadingToBlack(75, true, game);
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

            if (game.gameState == State.Paused)
            {
                spriteBatch.Draw(game.fd_Fader.t_MasterFadeBlack , new Vector2(0, 0), new Color(1f, 1f, 1f, 0.5f));
                spriteBatch.DrawString(game.fontBig, "Paused", new Vector2(282, 277), Color.DarkGray);
                spriteBatch.DrawString(game.font, "Shield Strength: " + ship.ShieldStrength + "%", new Vector2(414, 419), Color.DarkGray);
                spriteBatch.DrawString(game.fontBig, "Paused", new Vector2(285, 280), Color.White);
                spriteBatch.DrawString(game.font, "Shield Strength: " + ship.ShieldStrength + "%", new Vector2(415, 420), Color.White);
            }

            spriteBatch.Draw(t_Crosshair, new Vector2((Mouse.GetState().X - (t_Crosshair.Width / 2)), (Mouse.GetState().Y - (t_Crosshair.Height / 2))), Color.White);
        }

        public void GamePlay(GameTime gameTime, Game1 game)
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
            game.gamepadStateCurr = GamePad.GetState(PlayerIndex.One);
            if (dead == false)
            {
                ship.ShipMovement(game.gamepadStateCurr, offset);
                FireCheck(game);
            }
            PauseCheck(game);
            #region Enemy Movement
            for (int i = 0; i < loadedLevel.levelData.i_MaxNumberEnemies; i++)
            {
                if (enemies[i].Alive == true)
                {
                    enemies[i].EnemyMovement(ship, this);
                }
            }
            #endregion
            #region Collision
            //if Photon collide with Chlor,  enemy1, enemy2

            //if Chlor collide with enemy1, enemy2

            //if Fused collide with enemy1, enemy2

            //if enemy1 collide with enemy2

            i_VibrateCounter = 0;

            //SHIP COLLISION
            #region SHIP COLLISION
            //EDGECOLLISION(ship, levelBounds);
            EDGECOLLISION(ship, loadedLevel.levelBounds, game);
            for (int i = 0; i < Photons.Length; i++)
            {
                if (Photons[i].ParticleState == Particle.PState.Alive)
                {
                    COLLISION(ship, Photons[i]);
                }
            }

            for (int i = 0; i < Chlor.Length; i++)
            {
                if (Chlor[i].ParticleState == Particle.PState.Alive)
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
                        COLLISION(ship, enemies[i], game);
                    }
                }
            }

            #endregion
            //PHOTON COLLISION
            #region PHOTON COLLISION

            for (int i = 0; i < Photons.Length; i++)
            {
                if (Photons[i].ParticleState == Particle.PState.Alive)
                {
                    EDGECOLLISION(Photons[i], loadedLevel.levelBounds);
                    for (int j = 0; j < Photons.Length; j++)
                    {
                        if (i != j)
                        {
                            COLLISION(Photons[i], Photons[j], false, game);
                        }
                    }
                }
            }

            for (int i = 0; i < Photons.Length; i++)
            {
                for (int j = 0; j < Chlor.Length; j++)
                {
                    if (Photons[i].ParticleState == Particle.PState.Alive && Chlor[j].ParticleState == Particle.PState.Alive)
                    {
                        //FusionCode
                        COLLISION(Photons[i], Chlor[j], true, game);
                    }
                }
            }

            for (int i = 0; i < Photons.Length; i++)
            {
                for (int j = 0; j < enemies.Length; j++)
                {
                    if (enemies[j].Alive == true && Photons[i].ParticleState == Particle.PState.Alive && Photons[i].IsTethered == false)
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
                if (Chlor[i].ParticleState == Particle.PState.Alive)
                {
                    EDGECOLLISION(Chlor[i], loadedLevel.levelBounds);
                    for (int j = 0; j < Chlor.Length; j++)
                    {
                        if (i != j)
                        {
                            COLLISION(Chlor[i], Chlor[j], false, game);
                        }
                    }
                }
            }

            for (int i = 0; i < Chlor.Length; i++)
            {
                for (int j = 0; j < enemies.Length; j++)
                {
                    if (enemies[j].Alive == true && Chlor[i].ParticleState == Particle.PState.Alive && Photons[i].IsTethered == false)
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
                EDGECOLLISION(enemies[i], loadedLevel.levelBounds);
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
                        COLLISION(enemies[i], bullets[j], game);
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
                            COLLISION(tethers[i], Photons[j], j, true, game);
                            COLLISION(tethers[i], Chlor[j], j, false, game);
                        }
                    }
                }
            }

            #endregion

            GamePad.SetVibration(PlayerIndex.One, (0.2f * i_VibrateCounter), (0.2f * i_VibrateCounter));
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
                if (Fused[i].ParticleState == Particle.PState.Spawning)
                {
                    if (Fused[i].IsFusion == true)
                    {
                        UpdateFusionEffect(new Vector2((Fused[i].Position.X + (Fused[i].Texture.Width / 2)), (Fused[i].Position.Y - 15 + (Fused[i].Texture.Height / 2))), Fused[i]);
                        Fused[i].FusionCounter++;
                        if (Fused[i].FusionCounter == 70)
                        {
                            f_Fusions++;
                            Fused[i].IsFusion = false;
                            Fused[i].FusionCounter = 0;
                            Fused[i].ParticleState = Particle.PState.Alive;
                            if (game.gameState == State.Tutorial)
                            {
                                Tutorial tutorial = (Tutorial)loadedLevel;
                                tutorial.b_TutorialFusion = true;
                            }
                        }
                    }
                }
                if (Fused[i].ParticleState == Particle.PState.Alive)
                {
                    Fused[i].Velocity += new Vector2((((4096 - Fused[i].Position.X) / Vector2.Distance(Fused[i].Position, new Vector2(4096, 1152))) * 10), (((1152 - Fused[i].Position.Y) / Vector2.Distance(Fused[i].Position, new Vector2(4096, 1152)))) * 10);
                    Fused[i].updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction);
                }
            }

            UpdateEngineSmoke();
            ShieldPulse();
            ship.updatePosition((float)gameTime.ElapsedGameTime.TotalSeconds, f_Friction, tetherState);

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
        public Color Shield()
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
            if ((i_ShieldPulseCounter % (i_PulseRate / 10)) == 0)
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
        public void FireCheck(Game1 game)
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
                            bullets[i].ResetBullet();
                            if (game.gameState == State.Tutorial)
                            {
                                Tutorial tutorial = (Tutorial)loadedLevel;
                                tutorial.tutBulletCounter++;
                            }
                            v_TurretDirection = ship.TurretDirection;
                            bullets[i].Fire(ship, v_TurretDirection);
                            game.soundBank.PlayCue("lazer");
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
                            if (game.gameState == State.Tutorial)
                            {
                                Tutorial tutorial = (Tutorial)loadedLevel;
                                tutorial.tutBulletCounter++;
                            }
                            v_TurretDirection = ship.TurretDirection;
                            bullets[i].Fire(ship, v_TurretDirection);
                            game.soundBank.PlayCue("lazer");
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
                switch (tetherState)
                {
                    case TetherState.shooting:
                        if (game.lastLTrigger == false)
                        {
                            game.soundBank.PlayCue("tetherFire");
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
                        if (game.lastLTrigger == false)
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
                            tetherState = TetherState.detethered;
                            game.soundBank.PlayCue("deTether");
                        }
                        break;

                    case TetherState.detethered:
                        if (game.lastLTrigger == false)
                        {
                            tetherState = TetherState.shooting;
                        }
                        break;
                }

                game.lastLTrigger = true;
            }
            else
            {
                game.lastLTrigger = false;
            }
            #endregion
            #region Mouse
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                switch (tetherState)
                {
                    case TetherState.shooting:
                        if (game.lastRMouse == false)
                        {
                            game.soundBank.PlayCue("tetherFire");
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


                        if (game.lastRMouse == false)
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
                            tetherState = TetherState.detethered;
                            game.soundBank.PlayCue("deTether");
                        }
                        break;

                    case TetherState.detethered:
                        if (game.lastRMouse == false)
                        {
                            tetherState = TetherState.shooting;
                        }
                        break;
                }

                game.lastRMouse = true;
            }
            else
            {
                game.lastRMouse = false;
            }
            #endregion
            #endregion
        }
        public void PauseCheck(Game1 game)
        {
            #region Controller
            GamePadState gamepadStateCurr = GamePad.GetState(PlayerIndex.One);
            if (gamepadStateCurr.IsButtonDown(Buttons.Start))
            {
                if (!game.gamepadStateOld.IsButtonDown(Buttons.Start))
                {
                    if (game.gameState == State.Gameplay)
                    {
                        game.gameState = State.Paused;
                    }
                    else if (game.gameState == State.Paused)
                    {
                        game.gameState = State.Gameplay;
                    }
                }
            }
            game.gamepadStateOld = gamepadStateCurr;
            #endregion
            #region Keyboard
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (game.lastEscape == false)
                {
                    if (game.gameState == State.Gameplay)
                    {
                        game.gameState = State.Paused;
                    }
                    else if (game.gameState == State.Paused)
                    {
                        game.gameState = State.Gameplay;
                    }
                }
                game.lastEscape = true;
            }
            else
            {
                game.lastEscape = false;
            }

            game.gamepadStateOld = gamepadStateCurr;
            #endregion
        }
        public void SegmentUpdate()
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

        public void InitalizeLevel(Level level, Game1 game)
        {
            loadedLevel = level;

            f_Fusions = 0;
            i_ShieldPulseCounter = 0;
            i_shipAlpha = 0.5f;
            i_PulseRate = 60;

            enemies = new Enemy[loadedLevel.levelData.i_MaxNumberEnemies];
            for (int j = 0; j < loadedLevel.levelData.i_MaxNumberEnemies; j++)
            {
                enemies[j] = new Enemy(game, loadedLevel);
                enemies[j].LoadTexture(game.Content);
                enemies[j].Rectangle = new Rectangle(((int)enemies[j].Position.X - (enemies[j].Texture.Width / 2)), ((int)enemies[j].Position.Y - (enemies[j].Texture.Height / 2)), enemies[j].Texture.Width, enemies[j].Texture.Height);
                enemies[j].TextureData = new Color[enemies[j].Texture.Width * enemies[j].Texture.Height];
                enemies[j].Texture.GetData(enemies[j].TextureData);
                enemies[j].ShieldSpark = new ShieldSparkParticleSystem(game, 3);
                game.Components.Add(enemies[j].ShieldSpark);
                enemies[j].ParticleKill = new ParticleKillParticleSystem(game, 4);
                game.Components.Add(enemies[j].ParticleKill);
            }

            Fused = new Particle[loadedLevel.levelData.i_MaxNumberFused];
            for (int j = 0; j < loadedLevel.levelData.i_MaxNumberFused; j++)
            {
                Fused[j] = new Particle(loadedLevel.levelBounds);
                Fused[j].Fusion = new FusionParticleSystem(game, 20);
                game.Components.Add(Fused[j].Fusion);
                Fused[j].LoadTex(t_Fused);
            }

            Photons = new Particle[loadedLevel.levelData.i_MaxNumberPhotons];
            for (int j = 0; j < loadedLevel.levelData.i_MaxNumberPhotons; j++)
            {
                Photons[j] = new Particle(loadedLevel.levelBounds);
                Photons[j].LoadTex(t_Photon);
                Photons[j].Fusion = new FusionParticleSystem(game, 20);
                game.Components.Add(Photons[j].Fusion);
            }
            Chlor = new Particle[loadedLevel.levelData.i_MaxNumberChloro];
            for (int j = 0; j < loadedLevel.levelData.i_MaxNumberChloro; j++)
            {
                Chlor[j] = new Particle(loadedLevel.levelBounds);
                Chlor[j].LoadTex(t_Chlor);
                Chlor[j].Fusion = new FusionParticleSystem(game, 20);
                game.Components.Add(Chlor[j].Fusion);
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
            ship.Turret = game.Content.Load<Texture2D>("Ship//turret");
            tetherState = TetherState.shooting;

            offset = ship.OffsetUpdate(offset);
            engineSmoke = new EngineParticleSystem(game, 9);
            game.Components.Add(engineSmoke);
            shipExplosion = new ExplosionParticleSystem(game, 9);
            game.Components.Add(shipExplosion);
        }

        public void UpdateEngineSmoke()
        {
            engineSmoke.AddParticles(ship.Position + (Vector2.Transform(new Vector2(0, 45), Matrix.CreateRotationZ((float)ship.NextRotation))), ship.ShipDirection, offset, (float)ship.NextRotation, false);
        }
        public void UpdateShieldSpark(Vector2 collision, Enemy enemy)
        {
            enemy.ShieldSpark.AddParticles(collision, new Vector2(0, 0), ship.Position, 0, true);
        }
        public void UpdateParticleKill(Vector2 collision, Particle particle, Enemy enemy)
        {
            enemy.ParticleKill.AddParticles((collision - new Vector2((particle.Texture.Width / 2), (particle.Texture.Width / 4))), new Vector2(0, 0), ship.Position, 0, true);
        }
        public void UpdateFusionEffect(Vector2 collision, Particle particle)
        {
            particle.Fusion.AddParticles((collision - new Vector2((particle.Texture.Width / 2), (particle.Texture.Width / 4))), new Vector2(0, 0), ship.Position, 0, true);
        }
        public void UpdateExplosion(Vector2 collision)
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
                            i_VibrateCounter++;

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
        public void COLLISION(Bullet tether, Particle particle, int ID, bool isAPhoton, Game1 game)
        {
            if (tetherState == TetherState.shooting)
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
                        if (particle.ParticleState == Particle.PState.Alive && particle.BeingAttacked == false)
                        {
                            particle.Color = Color.Aqua;
                            ship.TetheredParticleID = ID;
                            ship.IsPhoton = isAPhoton;
                            ship.Tethering = true;
                            particle.IsTethered = true;
                            tether.Alive = false;
                            tetherState = TetherState.tethered;
                            game.soundBank.PlayCue("tethered");

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
        public void COLLISION(Particle particleA, Particle particleB, bool fusion, Game1 game)
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
                        tetherState = TetherState.detethered;
                        particleA.ParticleState = Particle.PState.Colliding;
                        particleB.ParticleState = Particle.PState.Colliding;
                        game.soundBank.PlayCue("fusion");
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
        public void COLLISION(Ship ship, Enemy enemy, Game1 game)
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

                    i_VibrateCounter++; //add to vibration amount

                    enemy.EnemyCollisionPosition = enemy.Position;// -new Vector2(enemy.Texture.Width / 2, enemy.Texture.Height / 2);
                    ship.ShipShield -= 25;
                    if (ship.ShipShield < 0)
                    {
                        //You are Dead
                        game.soundBank.PlayCue("shipExplode");
                        dead = true;
                    }
                    if (ship.ShipShield == 75)
                    {
                        game.soundBank.PlayCue("75percent");
                    }
                    else if (ship.ShipShield == 50)
                    {
                        game.soundBank.PlayCue("50percent");
                    }
                    else if (ship.ShipShield == 25)
                    {
                        game.soundBank.PlayCue("25percent");
                    }
                    else if (ship.ShipShield == 0)
                    {
                        ship.ShipShield = -1;
                        game.soundBank.PlayCue("shieldsdown");
                    }
                    else
                        enemy.EnemyCollision = true;
                    ship.Hit = true;
                    enemy.Alive = false;
                    game.soundBank.PlayCue("enemyDie");
                    game.vibrate = 20;

                    if (enemy.closestParticle != null)
                    {
                        enemy.closestParticle.BeingAttacked = false;
                        enemy.closestParticle.Attacker = null;
                        if (enemy.closestParticle.IsTethered == false)
                        {
                            enemy.closestParticle.Color = Color.White;
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
                    if (enemy.closestParticle != null)
                    {
                        if (enemy.closestParticle == particle)
                        {
                            particle.BeingAttacked = true;
                            particle.Attacker = enemy;
                            enemy.attacking = true;
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

                    enemyB.Velocity += impulse * 5;
                    enemyA.Velocity -= impulse * 5; //enemies aren't affected by enemyB collisions :P
                }
            }
        }
        public void COLLISION(Enemy enemy, Bullet bullet, Game1 game)
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
                    enemy.EnemyShot(bullet, game);
                }
            }
        }

        public void EDGECOLLISION(Ship thing, Rectangle bounds, Game1 game)
        {
            if (thing.Position.Y < bounds.Y)
            {
                thing.Position += new Vector2(0, bounds.Y - thing.Position.Y);
                thing.Velocity *= new Vector2(1, -f_EdgeDamper);
                game.soundBank.PlayCue("sideBump");
            }
            else if (thing.Position.Y > bounds.Y + bounds.Height)
            {
                thing.Position -= new Vector2(0, thing.Position.Y - (bounds.Y + bounds.Height));
                thing.Velocity *= new Vector2(1, -f_EdgeDamper);
                game.soundBank.PlayCue("sideBump");
            }
            if (thing.Position.X < bounds.X)
            {
                thing.Position += new Vector2(bounds.X - thing.Position.X, 0);
                thing.Velocity *= new Vector2(-f_EdgeDamper, 1);
                game.soundBank.PlayCue("sideBump");
            }
            else if (thing.Position.X > bounds.X + bounds.Width)
            {
                thing.Position -= new Vector2(thing.Position.X - (bounds.X + bounds.Width), 0);
                thing.Velocity *= new Vector2(-f_EdgeDamper, 1);
                game.soundBank.PlayCue("sideBump");
            }
        }
        public void EDGECOLLISION(Enemy thing, Rectangle bounds)
        {
            if (thing.Position.Y < bounds.Y)
            {
                thing.Position += new Vector2(0, bounds.Y - thing.Position.Y);
                thing.Velocity *= new Vector2(1, -f_EdgeDamper);
            }
            else if (thing.Position.Y > bounds.Y + bounds.Height)
            {
                thing.Position -= new Vector2(0, thing.Position.Y - (bounds.Y + bounds.Height));
                thing.Velocity *= new Vector2(1, -f_EdgeDamper);
            }
            if (thing.Position.X < bounds.X)
            {
                thing.Position += new Vector2(bounds.X - thing.Position.X, 0);
                thing.Velocity *= new Vector2(-f_EdgeDamper, 1);
            }
            else if (thing.Position.X > bounds.X + bounds.Width)
            {
                thing.Position -= new Vector2(thing.Position.X - (bounds.X + bounds.Width), 0);
                thing.Velocity *= new Vector2(-f_EdgeDamper, 1);
            }
        }
        public void EDGECOLLISION(Particle thing, Rectangle bounds)
        {
            if (thing.Position.Y < bounds.Y)
            {
                thing.Position += new Vector2(0, bounds.Y - thing.Position.Y);
                thing.Velocity *= new Vector2(1, -f_EdgeDamper);
            }
            else if (thing.Position.Y > bounds.Y + bounds.Height)
            {
                thing.Position -= new Vector2(0, thing.Position.Y - (bounds.Y + bounds.Height));
                thing.Velocity *= new Vector2(1, -f_EdgeDamper);
            }
            if (thing.Position.X < bounds.X)
            {
                thing.Position += new Vector2(bounds.X - thing.Position.X, 0);
                thing.Velocity *= new Vector2(-f_EdgeDamper, 1);
            }
            else if (thing.Position.X > bounds.X + bounds.Width)
            {
                thing.Position -= new Vector2(thing.Position.X - (bounds.X + bounds.Width), 0);
                thing.Velocity *= new Vector2(-f_EdgeDamper, 1);
            }
        }

        public void LEVELCOLLISION(Ship item, Game1 game)
        {
            if (item.Rectangle.Intersects(loadedLevel.levelData.levelLeft) ||
                item.Rectangle.Intersects(loadedLevel.levelData.levelRight) ||
                item.Rectangle.Intersects(loadedLevel.levelData.levelTop) ||
                item.Rectangle.Intersects(loadedLevel.levelData.levelBottom))
            {


                if (IntersectPixels(item.RotationTransform,
                                    item.Texture.Width,
                                    item.Texture.Height,
                                    item.TextureData,
                                    Matrix.Identity,
                                    loadedLevel.t_LevelBounds.Width,
                                    loadedLevel.t_LevelBounds.Height,
                                    loadedLevel.levelBoundsData))
                {

                    i_VibrateCounter++;

                    if (item.Position.Y < loadedLevel.levelData.levelTop.Height)
                    {
                        item.Position += new Vector2(0, 5);
                        item.Velocity *= new Vector2(1, -f_EdgeDamper);
                    }
                    else if (item.Position.Y > loadedLevel.levelData.levelTop.Height + loadedLevel.levelData.levelBottom.Y)
                    {
                        item.Position -= new Vector2(0, item.Position.Y - (loadedLevel.levelData.levelTop.Height + loadedLevel.levelData.levelBottom.Y));
                        item.Velocity *= new Vector2(1, -f_EdgeDamper);
                    }
                    if (item.Position.X < loadedLevel.levelData.levelTop.X)
                    {
                        item.Position += new Vector2(loadedLevel.levelData.levelTop.X - item.Position.X, 0);
                        item.Velocity *= new Vector2(-f_EdgeDamper, 1);
                    }
                    else if (item.Position.X > loadedLevel.levelData.levelTop.X + loadedLevel.levelData.levelRight.X)
                    {
                        item.Position -= new Vector2(item.Position.X - (loadedLevel.levelData.levelTop.X + loadedLevel.levelData.levelRight.X), 0);
                        item.Velocity *= new Vector2(-f_EdgeDamper, 1);
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
    }
}
