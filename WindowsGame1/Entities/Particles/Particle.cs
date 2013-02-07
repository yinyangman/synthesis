
using System;
using System.Collections.Generic;
using System.Linq; // Whats this for?!
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synthesis
{
    public class Particle : Entity
    {
        public enum PState
        {
            Alive = 0,
            Spawning = 1,
            Dead = 2,
            Colliding = 3,
            Fusing = 4
        }
        private bool b_beingAttacked = false;
        private Enemy e_Attacker;
        private PState s_ParticleState = PState.Dead;
        private int i_SpawnTimer = 0;
        private float f_Scale = 0;
        private Vector2 v_FusionPosition;
        public int i_Fusing = 0;

        private FusionParticleSystem fusion;
        private Vector2 v_FusionEffectPosition = Vector2.Zero;
        private bool b_Fusion = true;
        private int i_FusionCounter = 0;

        private bool isTethered;

        public Particle(Rectangle bounding)                   //constructor
        {
            vPosition = new Vector2(Game1.Random.Next((bounding.X + 100), (bounding.X + bounding.Width - 100)), Game1.Random.Next(bounding.Y + 100, (bounding.Y + bounding.Height - 100)));
            state = entityState.stationary;   //initial state
            color = Color.White;            //default color
            vVelocity = Vector2.Zero;        //initialise to 0 speed
            v_FusionEffectPosition = Vector2.Zero;
            b_Fusion = true;
            i_FusionCounter = 0;
            isTethered = false;

        }
        public Particle(Vector2 position)                   //constructor
        {
            vPosition = position;
            state = entityState.stationary;   //initial state
            color = Color.White;            //default color
            vVelocity = Vector2.Zero;        //initialise to 0 speed
            v_FusionEffectPosition = Vector2.Zero;
            b_Fusion = true;
            i_FusionCounter = 0;
            isTethered = false;

        }


        public void LoadTex(Texture2D t_Fused)
        {
            tTexture = t_Fused;
            Rectangle rectangle = new Rectangle((int)vPosition.X, (int)vPosition.Y, tTexture.Width, tTexture.Height);

            tTextureData = new Color[tTexture.Width * tTexture.Height];
            tTexture.GetData(tTextureData);
        }

        public void Spawn(Vector2 offset, Rectangle bounding, bool isPhoton)
        {
            if (isPhoton == true)
            {
                Position = new Vector2(Game1.Random.Next(bounding.X + 100, (bounding.X + bounding.Width - 100)), Game1.Random.Next(bounding.Y + 100, (bounding.Y + (bounding.Height/2) - 100)));
            }
            else
            {
                Position = new Vector2(Game1.Random.Next(bounding.X + 100, (bounding.X + bounding.Width - 100)), Game1.Random.Next((bounding.Y + (bounding.Height / 2)) + 100, (bounding.Y + bounding.Height - 100)));
            }
            ParticleState = PState.Spawning;
        }
        public void Spawn(Vector2 position)
        {
            Position = position;
            ParticleState = PState.Spawning;
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

        public bool BeingAttacked
        {
            get
            {
                return b_beingAttacked;
            }
            set
            {
                b_beingAttacked = value;
            }
        }

        public Enemy Attacker
        {
            get
            {
                return e_Attacker;
            }
            set
            {
                e_Attacker = value;
            }
        }

        public PState ParticleState
        {
            get
            {
                return s_ParticleState;
            }
            set
            {
                s_ParticleState = value;
            }
        }

        public float Scale
        {
            get
            {
                return f_Scale;
            }
            set
            {
                f_Scale = value;
            }
        }

        public FusionParticleSystem Fusion
        {
            get
            {
                return fusion;
            }
            set
            {
                fusion = value;
            }
        }

        public Vector2 FusionEffectPosition
        {
            get
            {
                return v_FusionEffectPosition;
            }
            set
            {
                v_FusionEffectPosition = value;
            }
        }

        public bool IsFusion
        {
            get
            {
                return b_Fusion;
            }
            set
            {
                b_Fusion = value;
            }
        }

        public int FusionCounter
        {
            get
            {
                return i_FusionCounter;
            }
            set
            {
                i_FusionCounter = value;
            }
        }

        public Vector2 FusionPosition
        {
            get
            {
                return v_FusionPosition;
            }
            set
            {
                v_FusionPosition = value;
            }
        }

        #region Get/Set
        public bool IsTethered
        {
            get
            {
                return isTethered;
            }
            set
            {
                isTethered = value;
            }
        }

        #endregion

    }
}
