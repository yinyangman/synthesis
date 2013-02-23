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
using SynthesisGameLibrary;

namespace Synthesis
{
    public class Enemy : Entity
    {
        private EnemyData ed_EnemyData;

        private int i_Health;
        private bool b_Alive;
        private int i_SpawnTimer;
        private ShieldSparkParticleSystem shieldSpark;
        private Vector2 v_EnemyShipCollision = Vector2.Zero;
        private bool b_EnemyShipCollision = false;
        private int i_ShieldSparkCounter = 0;

        private ParticleKillParticleSystem particleKill;
        private Vector2 v_ParticleKillPosition = Vector2.Zero;
        private bool b_ParticleKill = false;

        private int i_ParticleKillCounter = 0;
        public Particle closestParticle;

        public bool attacking = false;
        public float f_Rotation = 0;
        public int i_KillingCounter = 0;
        public float f_colour = 1.0f;

        public Particle p_ParticleKilled;

        public Enemy(Game1 game, Level level)
        {
            int randomType = Game1.Random.Next(0, 100);

            if (randomType < level.levelData.i_EnemyMinorChance)
            {
                ed_EnemyData = game.EnemyDataForType(EnemyData.Type.Normal);
            }
            else if (randomType < (level.levelData.i_EnemyMajorChance + level.levelData.i_EnemyMinorChance))
            {
                ed_EnemyData = game.EnemyDataForType(EnemyData.Type.Destroyer);
            }

            InitaliseEnemy(game);
        }
        public Enemy(Game1 game, EnemyData.Type type)
        {
            ed_EnemyData = game.EnemyDataForType(type);
            InitaliseEnemy(game);
        }

        public void InitaliseEnemy(Game1 game)
        {
            vPosition = Vector2.Zero;
            i_KillingCounter = 0;
            f_colour = 1.0f;
            i_SpawnTimer = 0;
            ShieldSpark = new ShieldSparkParticleSystem(game, 8);
            game.Components.Add(ShieldSpark);
            ParticleKill = new ParticleKillParticleSystem(game, 9);
            game.Components.Add(ParticleKill);
            attacking = false;

            i_Health = ed_EnemyData.i_MaxHealth;
        }
        public void LoadTexture(ContentManager Content)
        {
            tTexture = Content.Load<Texture2D>(ed_EnemyData.s_Texture);
        }

        public void EnemyMovement(Ship ship, Engine engine)
        {
            if (ed_EnemyData.t_Type == EnemyData.Type.Normal)
            {
                MoveTowardsPlayer(ship);
            }
            else if (ed_EnemyData.t_Type == EnemyData.Type.Destroyer)
            {
                if (attacking == false)
                {
                    float currentDistance = 99999;
                    for (int i = 0; i < engine.Photons.Length; i++)
                    {
                        float distance = Vector2.Distance(engine.Photons[i].Position, Position);
                        if ((distance < currentDistance) && engine.Photons[i].BeingAttacked == false && engine.Photons[i].ParticleState == Particle.PState.Alive && engine.Photons[i].IsTethered == false)
                        {
                            currentDistance = distance;
                            closestParticle = engine.Photons[i];
                        }
                    }
                    for (int i = 0; i < engine.Chlor.Length; i++)
                    {
                        float distance = Vector2.Distance(engine.Chlor[i].Position, Position);
                        if (distance < currentDistance && engine.Chlor[i].BeingAttacked == false && engine.Chlor[i].ParticleState == Particle.PState.Alive && engine.Chlor[i].IsTethered == false)
                        {
                            currentDistance = distance;
                            closestParticle = engine.Chlor[i];
                        }
                    }

                    if (closestParticle != null)
                    {
                        vVelocity.X = (((closestParticle.Position.X - vPosition.X) / Vector2.Distance(vPosition, closestParticle.Position)) * ed_EnemyData.f_Speed);
                        vVelocity.Y = (((closestParticle.Position.Y - vPosition.Y) / Vector2.Distance(vPosition, closestParticle.Position)) * ed_EnemyData.f_Speed);
                    }
                    else
                    {
                        MoveTowardsPlayer(ship);
                    }
                }
                else if (attacking == true)
                {

                    vVelocity.X = (((closestParticle.Position.X - vPosition.X) / Vector2.Distance(vPosition, closestParticle.Position)) * ed_EnemyData.f_Speed);
                    vVelocity.Y = (((closestParticle.Position.Y - vPosition.Y) / Vector2.Distance(vPosition, closestParticle.Position)) * ed_EnemyData.f_Speed);

                    if (i_KillingCounter < 250)
                    {
                        f_Rotation += 0.15f;
                        i_KillingCounter++;
                        f_colour -= 0.004f;
                        closestParticle.Color = new Color(1.0f, f_colour, f_colour, 1.0f);
                    }
                    else
                    {
                        f_Rotation = 0;
                        i_KillingCounter = 0;
                        f_colour = 1.0f;
                        p_ParticleKilled = closestParticle;
                        ParticleKillPosition = closestParticle.Position + new Vector2((closestParticle.Texture.Width / 2), (closestParticle.Texture.Height / 2));
                        closestParticle.ParticleState = Particle.PState.Dead;
                        attacking = false;
                        IsParticleKill = true;
                    }
                }
            }
        }
        public void MoveTowardsPlayer(Ship ship)
        {
            vVelocity.X = (((ship.Position.X - vPosition.X) / Vector2.Distance(vPosition, ship.Position)) * ed_EnemyData.f_Speed);
            vVelocity.Y = (((ship.Position.Y - vPosition.Y) / Vector2.Distance(vPosition, ship.Position)) * ed_EnemyData.f_Speed);
        }

        public void Spawn(Vector2 offset, Rectangle bounding)
        {
            //double side = Game1.Random.Next(0, 4);
            //if (side == 0)
            //{
            //    Position = new Vector2(Game1.Random.Next(-50, 0), Game1.Random.Next(0, 769));
            //    Position -= offset;
            //}
            //else if (side == 1)
            //{
            //    Position = new Vector2(Game1.Random.Next(1024, 1074), Game1.Random.Next(0, 769));
            //    Position -= offset;
            //}
            //else if (side == 2)
            //{
            //    Position = new Vector2(Game1.Random.Next(0, 1025), Game1.Random.Next(-50, 0));
            //    Position -= offset;
            //}
            //else if (side == 3)
            //{
            //    Position = new Vector2(Game1.Random.Next(0, 1025), Game1.Random.Next(768, 818));
            //    Position -= offset;
            //}
            Position = new Vector2(Game1.Random.Next(bounding.X + 100, (bounding.X + bounding.Width - 100)), Game1.Random.Next(bounding.Y + 100, (bounding.Y + bounding.Height - 100)));
            Alive = true;
        }
        public void Spawn(Vector2 position)
        {
            Position = position;
            Alive = true;
        }
        public int SpawnTimer
        {
            get
            {
                return i_SpawnTimer;
            }
            set
            {
                i_SpawnTimer = value;
            }
        }

        public Vector2 OffsetUpdate(Vector2 offset)
        {
            if (vPosition.X > 512 && vPosition.X < 3584)
            {
                offset.X = (-vPosition.X + 512);

            }
            if (vPosition.Y > 384 && vPosition.Y < 1920)
            {
                offset.Y = (-vPosition.Y + 384);
            }

            return offset;
        }
        public bool Alive
        {
            get
            {
                return b_Alive;
            }
            set
            {
                b_Alive = value;
            }
        }
        public EnemyData.Type EnemyType
        {
            get
            {
                return ed_EnemyData.t_Type;
            }
            set
            {
                ed_EnemyData.t_Type = value;
            }
        }

        public ShieldSparkParticleSystem ShieldSpark
        {
            get
            {
                return shieldSpark;
            }
            set
            {
                shieldSpark = value;
            }
        }
        public Vector2 EnemyCollisionPosition
        {
            get
            {
                return v_EnemyShipCollision;
            }
            set
            {
                v_EnemyShipCollision = value;
            }
        }
        public bool EnemyCollision
        {
            get
            {
                return b_EnemyShipCollision;
            }
            set
            {
                b_EnemyShipCollision = value;
            }
        }
        public int ShieldSparkCounter
        {
            get
            {
                return i_ShieldSparkCounter;
            }
            set
            {
                i_ShieldSparkCounter = value;
            }
        }

        public ParticleKillParticleSystem ParticleKill
        {
            get
            {
                return particleKill;
            }
            set
            {
                particleKill = value;
            }
        }
        public Vector2 ParticleKillPosition
        {
            get
            {
                return v_ParticleKillPosition;
            }
            set
            {
                v_ParticleKillPosition = value;
            }
        }
        public bool IsParticleKill
        {
            get
            {
                return b_ParticleKill;
            }
            set
            {
                b_ParticleKill = value;
            }
        }
        public int ParticleKillCounter
        {
            get
            {
                return i_ParticleKillCounter;
            }
            set
            {
                i_ParticleKillCounter = value;
            }
        }

        public void EnemyShot(Bullet bullet, Game1 game)
        {
            i_Health -= bullet.i_BulletDamage;

            if (i_Health <= 0)
            {
                i_Health = 0;

                EnemyCollisionPosition = Position;// -new Vector2(enemy.Texture.Width / 2, enemy.Texture.Height / 2);
                EnemyCollision = true;
                Alive = false;
                game.soundBank.PlayCue("enemyDie");
                if (EnemyType == EnemyData.Type.Destroyer)
                {
                    game.f_EnemiesBigKilled++;
                }
                else if (EnemyType == EnemyData.Type.Normal)
                {
                    game.f_EnemiesSmallKilled++;
                }

                if (closestParticle != null)
                {
                    closestParticle.BeingAttacked = false;
                    closestParticle.Attacker = null;
                    if (closestParticle.IsTethered == false)
                    {
                        closestParticle.Color = Color.White;
                    }
                }
            }
        }

    }
}
