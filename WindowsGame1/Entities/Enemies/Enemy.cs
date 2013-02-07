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

namespace Synthesis
{
    public class Enemy : Entity
    {
        public enum Type
        {
            Small = 0,
            Big = 1
        }

        public enum ParticleWant
        {
            Photon = 0,
            Chloro = 1,
            None = 2
        }

        private float f_EnemySpeed;
        private bool b_alive;
        private Type t_Type;
        private int i_SpawnTimer;
        private ShieldSparkParticleSystem shieldSpark;
        private Vector2 v_EnemyShipCollision = Vector2.Zero;
        private bool b_EnemyShipCollision = false;
        private int i_ShieldSparkCounter = 0;


        private ParticleKillParticleSystem particleKill;
        private Vector2 v_ParticleKillPosition = Vector2.Zero;
        private bool b_ParticleKill = false;

        private int i_ParticleKillCounter = 0;
        public ParticleWant desiredParticle = ParticleWant.None;
        public int ClosestPhoton = 0;
        public int ClosestChloro = 0;
        public bool attacking = false;
        public float f_Rotation = 0;
        public int i_KillingCounter = 0;
        public float f_colour = 1.0f;

        public Particle p_ParticleKilled;

        public Enemy(Game1 game)
        {
            vPosition = Vector2.Zero;
            i_KillingCounter = 0;
            f_colour = 1.0f;
            i_SpawnTimer = 0;
            t_Type = (Type)Game1.Random.Next(0, 2);
            ShieldSpark = new ShieldSparkParticleSystem(game, 8);
            game.Components.Add(ShieldSpark);
            ParticleKill = new ParticleKillParticleSystem(game, 9);
            game.Components.Add(ParticleKill);
            attacking = false;
        }

        public Enemy(Game1 game, int type)
        {
            vPosition = Vector2.Zero;
            i_KillingCounter = 0;
            f_colour = 1.0f;
            i_SpawnTimer = 0;
            t_Type = (Type)type;
            ShieldSpark = new ShieldSparkParticleSystem(game, 8);
            game.Components.Add(ShieldSpark);
            ParticleKill = new ParticleKillParticleSystem(game, 9);
            game.Components.Add(ParticleKill);
            attacking = false;
        }

        public void LoadTex(Texture2D t_EnemySmall, Texture2D t_EnemyBig)
        {
            if (t_Type == Type.Big)
            {
                tTexture = t_EnemyBig;
                f_EnemySpeed = 120;
            }
            else
            {
                tTexture = t_EnemySmall;
                f_EnemySpeed = 80;
            }
        }

        public void EnemyMovement(Ship ship, Particle[] photons, Particle[] chloros, int PhotonsMax, int ChlorosMax, Game1 game)
        {
            if (t_Type == Type.Small)
            {
                //Move towards the plaer
                vVelocity.X = (((ship.Position.X - vPosition.X) / Vector2.Distance(vPosition, ship.Position)) * f_EnemySpeed);
                vVelocity.Y = (((ship.Position.Y - vPosition.Y) / Vector2.Distance(vPosition, ship.Position)) * f_EnemySpeed);
            }
            else if (t_Type == Type.Big)
            {
                if (attacking == false)
                {
                    float currentPhoton = 99999;
                    float currentChloro = 99999;
                    for (int i = 0; i < PhotonsMax; i++)
                    {
                        float distance = Vector2.Distance(photons[i].Position, Position);
                        if ((distance < currentPhoton) && photons[i].BeingAttacked == false && photons[i].ParticleState == Particle.PState.Alive && photons[i].IsTethered == false)
                        {
                            currentPhoton = distance;
                            ClosestPhoton = i;
                        }
                    }
                    for (int i = 0; i < ChlorosMax; i++)
                    {
                        float distance = Vector2.Distance(chloros[i].Position, Position);
                        if (distance < currentChloro && chloros[i].BeingAttacked == false && chloros[i].ParticleState == Particle.PState.Alive && chloros[i].IsTethered == false)
                        {
                            currentChloro = distance;
                            ClosestChloro = i;
                        }
                    }
                    if (currentChloro < currentPhoton)
                    {
                        //Go after closest chloro
                        desiredParticle = ParticleWant.Chloro;
                        vVelocity.X = (((chloros[ClosestChloro].Position.X - vPosition.X) / Vector2.Distance(vPosition, chloros[ClosestChloro].Position)) * f_EnemySpeed);
                        vVelocity.Y = (((chloros[ClosestChloro].Position.Y - vPosition.Y) / Vector2.Distance(vPosition, chloros[ClosestChloro].Position)) * f_EnemySpeed);
                    }
                    else if (currentPhoton < currentChloro)
                    {
                        //Go after closest photon
                        desiredParticle = ParticleWant.Photon;
                        vVelocity.X = (((photons[ClosestPhoton].Position.X - vPosition.X) / Vector2.Distance(vPosition, photons[ClosestPhoton].Position)) * f_EnemySpeed);
                        vVelocity.Y = (((photons[ClosestPhoton].Position.Y - vPosition.Y) / Vector2.Distance(vPosition, photons[ClosestPhoton].Position)) * f_EnemySpeed);
                    }
                }
                else if (attacking == true)
                {
                    if (desiredParticle == ParticleWant.Chloro)
                    {
                        vVelocity.X = (((chloros[ClosestChloro].Position.X - vPosition.X) / Vector2.Distance(vPosition, chloros[ClosestChloro].Position)) * f_EnemySpeed);
                        vVelocity.Y = (((chloros[ClosestChloro].Position.Y - vPosition.Y) / Vector2.Distance(vPosition, chloros[ClosestChloro].Position)) * f_EnemySpeed);
                    }
                    else if (desiredParticle == ParticleWant.Photon)
                    {
                        vVelocity.X = (((photons[ClosestPhoton].Position.X - vPosition.X) / Vector2.Distance(vPosition, photons[ClosestPhoton].Position)) * f_EnemySpeed);
                        vVelocity.Y = (((photons[ClosestPhoton].Position.Y - vPosition.Y) / Vector2.Distance(vPosition, photons[ClosestPhoton].Position)) * f_EnemySpeed);
                    }

                    if (i_KillingCounter < 250)
                    {
                        f_Rotation += 0.15f;
                        i_KillingCounter++;
                        f_colour -= 0.004f;
                        if (desiredParticle == ParticleWant.Chloro)
                        {
                            chloros[ClosestChloro].Color = new Color(1.0f, f_colour, f_colour, 1.0f);
                        }
                        else if (desiredParticle == ParticleWant.Photon)
                        {
                            photons[ClosestPhoton].Color = new Color(1.0f, f_colour, f_colour, 1.0f);
                        }
                    }
                    else
                    {
                        f_Rotation = 0;
                        i_KillingCounter = 0;
                        f_colour = 1.0f;
                        if (desiredParticle == ParticleWant.Chloro)
                        {
                            p_ParticleKilled = chloros[ClosestChloro];
                            ParticleKillPosition = chloros[ClosestChloro].Position + new Vector2((chloros[ClosestChloro].Texture.Width / 2), (chloros[ClosestChloro].Texture.Height / 2));
                            chloros[ClosestChloro].ParticleState = Particle.PState.Dead;
                        }
                        else if (desiredParticle == ParticleWant.Photon)
                        {
                            p_ParticleKilled = photons[ClosestPhoton];
                            ParticleKillPosition = photons[ClosestPhoton].Position + new Vector2((photons[ClosestPhoton].Texture.Width / 2), (photons[ClosestPhoton].Texture.Height / 2));
                            photons[ClosestPhoton].ParticleState = Particle.PState.Dead;
                        }
                        attacking = false;
                        IsParticleKill = true;
                    }
                }

            }
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
                return b_alive;
            }
            set
            {
                b_alive = value;
            }
        }

        public Type EnemyType
        {
            get
            {
                return t_Type;
            }
            set
            {
                t_Type = value;
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
    }
}
