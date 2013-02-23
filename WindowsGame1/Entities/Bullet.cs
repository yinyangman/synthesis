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
    public class Bullet : Entity
    {
        private float f_BulletSpeed;
        private bool b_alive;
        private Vector2 v_direction;
        private Vector2 v_ShipVelocity;
        public int i_SpawnTimer = 0;
        public int i_BulletDamage = 1;

        public Bullet(Texture2D t_Bullet)
        {
            vPosition = Vector2.Zero;
            f_BulletSpeed = 1000;
            Texture = t_Bullet;
            //rectangle = new Rectangle(((int)vPosition.X - (Texture.Width / 2)), ((int)vPosition.Y - (Texture.Height / 2)), Texture.Width, Texture.Height);
            tTextureData = new Color[tTexture.Width * tTexture.Height];
            tTexture.GetData(tTextureData);
        }

        public void Fire(Ship ship, Vector2 direction)
        {
            vPosition = (ship.Position + Vector2.Transform(new Vector2(0, 20), Matrix.CreateRotationZ((float)ship.NextRotation)));
            b_alive = true;
            v_direction = direction;
            v_ShipVelocity = ship.Velocity;
            vVelocity.X = (v_direction.X * f_BulletSpeed) + v_ShipVelocity.X;
            vVelocity.Y = (-v_direction.Y * f_BulletSpeed) + v_ShipVelocity.Y;
        }

        public void BulletMovement(Ship ship)
        {
            if (vPosition.X < 0 || vPosition.X > 4096 || vPosition.Y < 0 || vPosition.Y > 2304)
            {
                b_alive = false;
            }
        }

        public void BulletMovement(Vector2 partPos, Vector2 shipPos, int i, int numTethers)
        {
            Vector2 difference = shipPos - partPos;
            Vector2 spacer = difference / numTethers;
            Position = shipPos - (i * spacer);
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
    }
}
