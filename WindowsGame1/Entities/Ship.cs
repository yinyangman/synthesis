using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Synthesis
{
    public class Ship : Entity
    {

        private double d_NextRotation;
        private double d_LastRotation;
        private double d_DestinationRotation;
        private float f_ShipSpeed;
        private float f_MaxSpeed;
        private float f_TurnSpeed;
        private double d_NextRotationTurret;
        private double d_LastRotationTurret;
        private double d_DestinationRotationTurret;
        private float f_TurnSpeedTurret;
        private Vector2 v_ShipDirection;
        private Texture2D t_Turret;
        private Vector2 v_TurretDirection;
        private int tetheredParticleID;
        private int invulnTime;
        private float hitTime;
        private bool isPhoton;
        private bool tethering;
        private bool hit;
        private float i;
        private float alpha;
        private BoundingSphere boundSphere;

        int i_shield;
        private Matrix rectangleTransform;                       //Rotation stored in matrix for rotating rectangle

        public Ship(float x, float y, Texture2D t_ship)
        {
            
            color = Color.White;
            invulnTime = 150;
            hitTime = 0;
            alpha = 1.0f;
            i = -0.05f;
            vPosition = new Vector2(x, y);
            i_shield = 100;
            Texture = t_ship;
            f_ShipSpeed = 15;
            f_MaxSpeed = 450;
            f_TurnSpeed = 5;
            d_NextRotation = 0;
            f_TurnSpeedTurret = 10;
            rectangle = new Rectangle(((int)vPosition.X - (Texture.Width / 2)), ((int)vPosition.Y - (Texture.Height / 2)), Texture.Width, Texture.Height);
            origin = new Vector2((tTexture.Width / 2), (tTexture.Height / 2));

            hit = false;
            boundSphere = new BoundingSphere(new Vector3(vPosition, 0), (tTexture.Height / 2));

            tTextureData = new Color[tTexture.Width * tTexture.Height];
            tTexture.GetData(tTextureData);
        }

        public void ResetShip(float x, float y)
        {
            vPosition = new Vector2(x, y);
            i_shield = 100;
            d_NextRotation = 0;
            hit = false;
            boundSphere = new BoundingSphere(new Vector3(vPosition, 0), (tTexture.Height / 2));
        }

        ///////////////////////////////////////////////////////////
        //updatePosition - this overides the update from the Entity class
        //because the bounding rectangle is updated elsewhere
        public void updatePosition(float Time, float Friction, TetherState tethering)
        {
            if (hit)
            {
                hitTime++;
                if (hitTime< invulnTime)
                {
                    alpha += i;

                    if (alpha < 0.2f)
                    {
                        i *= -1;
                        alpha += i;
                        //left motor vibration
                    }
                    else if (alpha > 1)
                    {
                        i *= -1;
                        alpha += i;
                        //right motor vibration
                    }


                    color = new Color(1.0f, 1.0f, 1.0f, alpha);
                }
                else
                {
                    hit = false;
                    alpha = 1.0f;
                    hitTime = 0;

                    color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
            }

            //if (tethering == tetherState.tethered)
            //{
            //    ShipSpeed = (15/2);
            //    f_MaxSpeed = (450/2);
            //}
            //else
            //{
            //    ShipSpeed = 15;
            //    f_MaxSpeed = 450;
            //}
            vVelocity *= Friction;
            vPosition += vVelocity * Time;
            boundSphere = new BoundingSphere(new Vector3(vPosition, 0), (tTexture.Height / 2));

            if (vVelocity.Length() <= Friction)
            {
                vVelocity = Vector2.Zero;
            }

            if (tTexture != null)
            {
                f_Radius = tTexture.Height / 2;
                origin = new Vector2((tTexture.Width / 2), (tTexture.Height / 2));
            }

            //Updates the rectangle position in a matrix, this is used for everything that doesn't have rotation.
            rectanglePosTransform = Matrix.CreateTranslation(new Vector3(vPosition, 0.0f));
        }
        ///////////////////////////////////////////////////////////

        /// <summary>
        /// Rotation code!
        /// </summary>
        public void ShipMovement(GamePadState gamepadStateCurr, Vector2 offset)
        {
            if (gamepadStateCurr.IsConnected)
            {
                #region Controller
                double inverse = (gamepadStateCurr.ThumbSticks.Left.X / gamepadStateCurr.ThumbSticks.Left.Y);
                if (gamepadStateCurr.ThumbSticks.Left.Length() == 0)
                {
                    d_NextRotation = d_LastRotation;
                }
                else
                {
                    if (gamepadStateCurr.ThumbSticks.Left.Y < 0)
                    {
                        d_DestinationRotation = Math.Atan(inverse) + MathHelper.ToRadians(180);
                    }
                    else
                    {
                        d_DestinationRotation = Math.Atan(inverse);
                    }
                }
                if (d_NextRotation != d_DestinationRotation)
                {
                    double difference1 = d_LastRotation - d_DestinationRotation;
                    double difference2 = d_DestinationRotation - d_LastRotation;

                    if ((difference1 > MathHelper.ToRadians((float)180) && difference2 < MathHelper.ToRadians((float)-180)) || (difference1 < MathHelper.ToRadians((float)-180) && difference2 > MathHelper.ToRadians((float)180)))
                    {
                        if (difference1 > difference2)
                        {
                            d_NextRotation += MathHelper.ToRadians(f_TurnSpeed);
                        }
                        else
                        {
                            d_NextRotation -= MathHelper.ToRadians(f_TurnSpeed);
                        }
                    }
                    else
                    {
                        if (difference1 > difference2)
                        {
                            d_NextRotation -= MathHelper.ToRadians(f_TurnSpeed);
                        }
                        else
                        {
                            d_NextRotation += MathHelper.ToRadians(f_TurnSpeed);
                        }
                    }
                }
                if ((d_DestinationRotation > d_LastRotation && d_DestinationRotation < d_NextRotation) || (d_DestinationRotation < d_LastRotation && d_DestinationRotation > d_NextRotation))
                {
                    d_NextRotation = d_DestinationRotation;
                }
                if (d_NextRotation > MathHelper.ToRadians(270))
                {
                    d_NextRotation = MathHelper.ToRadians(-90);
                }
                else if (d_NextRotation < MathHelper.ToRadians(-90))
                {
                    d_NextRotation = MathHelper.ToRadians(270);
                }
                vVelocity.X += gamepadStateCurr.ThumbSticks.Left.X * f_ShipSpeed;
                vVelocity.Y -= (gamepadStateCurr.ThumbSticks.Left.Y * f_ShipSpeed);

                if (vVelocity.X > f_MaxSpeed)
                {
                    vVelocity.X = f_MaxSpeed;
                }
                else if (vVelocity.X < -f_MaxSpeed)
                {
                    vVelocity.X = -f_MaxSpeed;
                }

                if (vVelocity.Y > f_MaxSpeed)
                {
                    vVelocity.Y = f_MaxSpeed;
                }
                else if (vVelocity.Y < -f_MaxSpeed)
                {
                    vVelocity.Y = -f_MaxSpeed;
                }
                d_LastRotation = d_NextRotation;

                if ((gamepadStateCurr.ThumbSticks.Right.X < -0.7 || gamepadStateCurr.ThumbSticks.Right.X > 0.7) || (gamepadStateCurr.ThumbSticks.Right.Y < -0.7 || gamepadStateCurr.ThumbSticks.Right.Y > 0.7))
                {
                    if (gamepadStateCurr.ThumbSticks.Right.Length() == 0)
                    {
                        d_NextRotationTurret = d_LastRotationTurret;
                    }
                    double inverse2 = (gamepadStateCurr.ThumbSticks.Right.X / gamepadStateCurr.ThumbSticks.Right.Y);
                    if (gamepadStateCurr.ThumbSticks.Right.Length() == 0)
                    {
                        d_NextRotationTurret = d_LastRotationTurret;
                    }
                    else
                    {
                        if (gamepadStateCurr.ThumbSticks.Right.Y < 0)
                        {
                            d_DestinationRotationTurret = Math.Atan(inverse2) + MathHelper.ToRadians(180);
                        }
                        else
                        {
                            d_DestinationRotationTurret = Math.Atan(inverse2);
                        }
                        v_TurretDirection = gamepadStateCurr.ThumbSticks.Right;
                    }
                    if (d_NextRotationTurret != d_DestinationRotationTurret)
                    {
                        double difference1 = d_LastRotationTurret - d_DestinationRotationTurret;
                        double difference2 = d_DestinationRotationTurret - d_LastRotationTurret;

                        if ((difference1 > MathHelper.ToRadians((float)180) && difference2 < MathHelper.ToRadians((float)-180)) || (difference1 < MathHelper.ToRadians((float)-180) && difference2 > MathHelper.ToRadians((float)180)))
                        {
                            if (difference1 > difference2)
                            {
                                d_NextRotationTurret += MathHelper.ToRadians(f_TurnSpeedTurret);
                            }
                            else
                            {
                                d_NextRotationTurret -= MathHelper.ToRadians(f_TurnSpeedTurret);
                            }
                        }
                        else
                        {
                            if (difference1 > difference2)
                            {
                                d_NextRotationTurret -= MathHelper.ToRadians(f_TurnSpeedTurret);
                            }
                            else
                            {
                                d_NextRotationTurret += MathHelper.ToRadians(f_TurnSpeedTurret);
                            }
                        }
                    }
                    if ((d_DestinationRotationTurret > d_LastRotationTurret && d_DestinationRotationTurret < d_NextRotationTurret) || (d_DestinationRotationTurret < d_LastRotationTurret && d_DestinationRotationTurret > d_NextRotationTurret))
                    {
                        d_NextRotationTurret = d_DestinationRotationTurret;
                    }
                    if (d_NextRotationTurret > MathHelper.ToRadians(270))
                    {
                        d_NextRotationTurret = MathHelper.ToRadians(-90);
                    }
                    else if (d_NextRotationTurret < MathHelper.ToRadians(-90))
                    {
                        d_NextRotationTurret = MathHelper.ToRadians(270);
                    }
                    d_LastRotationTurret = d_NextRotationTurret;
                }

                rectangleTransform =
                    Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                    Matrix.CreateRotationZ((float)d_NextRotation) *
                    Matrix.CreateTranslation(new Vector3(vPosition, 0.0f));
                rectangle = RotateRectangle(rectangle, rectangleTransform);
                #endregion
            }
            else
            {
                #region Keyboard
                #region Movement
                //SW
                if ((Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S)) && (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A)))
                {
                    vVelocity.Y += (f_ShipSpeed / 2);
                    vVelocity.X -= (f_ShipSpeed / 2);
                    d_DestinationRotation = MathHelper.ToRadians(225);
                }
                //SE
                else if ((Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S)) && (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D)))
                {
                    vVelocity.Y += (f_ShipSpeed / 2);
                    vVelocity.X += (f_ShipSpeed / 2);
                    d_DestinationRotation = MathHelper.ToRadians(135);
                }
                //NW
                else if ((Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W)) && (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A)))
                {
                    vVelocity.Y -= (f_ShipSpeed / 2);
                    vVelocity.X -= (f_ShipSpeed / 2);
                    d_DestinationRotation = MathHelper.ToRadians(-45);
                }
                //NE
                else if ((Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W)) && (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D)))
                {
                    vVelocity.X += (f_ShipSpeed / 2);
                    vVelocity.Y -= (f_ShipSpeed / 2);
                    d_DestinationRotation = MathHelper.ToRadians(45);
                }
                //S
                else if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    vVelocity.Y += f_ShipSpeed;
                    d_DestinationRotation = MathHelper.ToRadians(180);
                }
                //W
                else if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    vVelocity.X -= f_ShipSpeed;
                    d_DestinationRotation = MathHelper.ToRadians(270);
                }
                //E
                else if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    vVelocity.X += f_ShipSpeed;
                    d_DestinationRotation = MathHelper.ToRadians(90);
                }
                //N
                else if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    vVelocity.Y -= f_ShipSpeed;
                    d_DestinationRotation = MathHelper.ToRadians(0);
                }

                if (vVelocity.X > f_MaxSpeed)
                {
                    vVelocity.X = f_MaxSpeed;
                }
                else if (vVelocity.X < -f_MaxSpeed)
                {
                    vVelocity.X = -f_MaxSpeed;
                }

                if (vVelocity.Y > f_MaxSpeed)
                {
                    vVelocity.Y = f_MaxSpeed;
                }
                else if (vVelocity.Y < -f_MaxSpeed)
                {
                    vVelocity.Y = -f_MaxSpeed;
                }

                if (d_NextRotation != d_DestinationRotation)
                {
                    double difference1 = d_LastRotation - d_DestinationRotation;
                    double difference2 = d_DestinationRotation - d_LastRotation;

                    if ((difference1 > MathHelper.ToRadians((float)180) && difference2 < MathHelper.ToRadians((float)-180)) || (difference1 < MathHelper.ToRadians((float)-180) && difference2 > MathHelper.ToRadians((float)180)))
                    {
                        if (difference1 > difference2)
                        {
                            d_NextRotation += MathHelper.ToRadians(f_TurnSpeed);
                        }
                        else
                        {
                            d_NextRotation -= MathHelper.ToRadians(f_TurnSpeed);
                        }
                    }
                    else
                    {
                        if (difference1 > difference2)
                        {
                            d_NextRotation -= MathHelper.ToRadians(f_TurnSpeed);
                        }
                        else
                        {
                            d_NextRotation += MathHelper.ToRadians(f_TurnSpeed);
                        }
                    }
                }
                if ((d_DestinationRotation > d_LastRotation && d_DestinationRotation < d_NextRotation) || (d_DestinationRotation < d_LastRotation && d_DestinationRotation > d_NextRotation))
                {
                    d_NextRotation = d_DestinationRotation;
                }
                if (d_NextRotation > MathHelper.ToRadians(270))
                {
                    d_NextRotation = MathHelper.ToRadians(-90);
                }
                else if (d_NextRotation < MathHelper.ToRadians(-90))
                {
                    d_NextRotation = MathHelper.ToRadians(270);
                }

                d_LastRotation = d_NextRotation;
                #endregion
                #region Aiming

           
                Vector2 mouseDiff = new Vector2(Mouse.GetState().X, Mouse.GetState().Y) - new Vector2(vPosition.X, vPosition.Y + 20) - offset;

                if (mouseDiff.Y > 0)
                {
                    d_DestinationRotationTurret = Math.Atan(-mouseDiff.X / mouseDiff.Y) + MathHelper.ToRadians(180);
                }
                else
                {
                    d_DestinationRotationTurret = Math.Atan(-mouseDiff.X / mouseDiff.Y);
                }

               
                v_TurretDirection = Vector2.Normalize(new Vector2(mouseDiff.X, -mouseDiff.Y));
                
                if (d_NextRotationTurret != d_DestinationRotationTurret)
                {
                    double difference1 = d_LastRotationTurret - d_DestinationRotationTurret;
                    double difference2 = d_DestinationRotationTurret - d_LastRotationTurret;

                    if ((difference1 > MathHelper.ToRadians((float)180) && difference2 < MathHelper.ToRadians((float)-180)) || (difference1 < MathHelper.ToRadians((float)-180) && difference2 > MathHelper.ToRadians((float)180)))
                    {
                        if (difference1 > difference2)
                        {
                            d_NextRotationTurret += MathHelper.ToRadians(f_TurnSpeedTurret);
                        }
                        else
                        {
                            d_NextRotationTurret -= MathHelper.ToRadians(f_TurnSpeedTurret);
                        }
                    }
                    else
                    {
                        if (difference1 > difference2)
                        {
                            d_NextRotationTurret -= MathHelper.ToRadians(f_TurnSpeedTurret);
                        }
                        else
                        {
                            d_NextRotationTurret += MathHelper.ToRadians(f_TurnSpeedTurret);
                        }
                    }
                }
                if ((d_DestinationRotationTurret > d_LastRotationTurret && d_DestinationRotationTurret < d_NextRotationTurret) || (d_DestinationRotationTurret < d_LastRotationTurret && d_DestinationRotationTurret > d_NextRotationTurret))
                {
                    d_NextRotationTurret = d_DestinationRotationTurret;
                }
                if (d_NextRotationTurret > MathHelper.ToRadians(270))
                {
                    d_NextRotationTurret = MathHelper.ToRadians(-90);
                }
                else if (d_NextRotationTurret < MathHelper.ToRadians(-90))
                {
                    d_NextRotationTurret = MathHelper.ToRadians(270);
                }
                d_LastRotationTurret = d_NextRotationTurret;

                #endregion

                rectangleTransform =
                    Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                    Matrix.CreateRotationZ((float)d_NextRotation) *
                    Matrix.CreateTranslation(new Vector3(vPosition, 0.0f));
                rectangle = RotateRectangle(rectangle, rectangleTransform);
                #endregion
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


        /// <summary>
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily
        /// transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
        //Transforms the rectangle to match the ship sprite
        public static Rectangle RotateRectangle(Rectangle rectangle, Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }


        #region Get/Sets
        public Matrix RotationTransform
        {
            get
            {
                return rectangleTransform;
            }
            set
            {
                rectangleTransform = value;
            }
        }

        public int TetheredParticleID
        {
            get
            {
                return tetheredParticleID;
            }
            set
            {
                tetheredParticleID = value;
            }
        }

        public bool IsPhoton
        {
            get
            {
                return isPhoton;
            }
            set
            {
                isPhoton = value;
            }
        }

        public BoundingSphere BoundSphere
        {
            get
            {
                return boundSphere;
            }
            set
            {
                boundSphere = value;
            }
        }

        public bool Hit
        {
            get
            {
                return hit;
            }
            set
            {
                hit = value;
            }
        }

        public bool Tethering
        {
            get
            {
                return tethering;
            }
            set
            {
                tethering = value;
            }
        }
        
        public double NextRotation
        {
            get
            {
                return d_NextRotation;
            }
            set
            {
                d_NextRotation = value;
            }
        }

        public double LastRotation
        {
            get
            {
                return d_LastRotation;
            }
            set
            {
                d_LastRotation = value;
            }
        }

        public double DestinationRotation
        {
            get
            {
                return d_DestinationRotation;
            }
            set
            {
                d_DestinationRotation = value;
            }
        }

        public double NextRotationTurret
        {
            get
            {
                return d_NextRotationTurret;
            }
            set
            {
                d_NextRotationTurret = value;
            }
        }

        public double LastRotationTurret
        {
            get
            {
                return d_LastRotationTurret;
            }
            set
            {
                d_LastRotationTurret = value;
            }
        }

        public double DestinationRotationTurret
        {
            get
            {
                return d_DestinationRotationTurret;
            }
            set
            {
                d_DestinationRotationTurret = value;
            }
        }

        public float ShipSpeed
        {
            get
            {
                return f_ShipSpeed;
            }
            set
            {
                f_ShipSpeed = value;
            }
        }

        public float TurnSpeed
        {
            get
            {
                return f_TurnSpeed;
            }
            set
            {
                f_TurnSpeed = value;
            }
        }

        public Texture2D Turret
        {
            get
            {
                return t_Turret;
            }
            set
            {
                t_Turret = value;
            }
        }

        public Vector2 ShipDirection
        {
            get
            {
                return v_ShipDirection;
            }
            set
            {
                v_ShipDirection = value;
            }
        }

        public int ShipShield
        {
            get
            {
                return i_shield;
            }
            set
            {
                i_shield = value;
            }
        }

        public Vector2 TurretDirection
        {
            get
            {
                return v_TurretDirection;
            }
            set
            {
                v_TurretDirection = value;
            }
        }

        public int ShieldStrength
        {
            get
            {
                return i_shield;
            }
            set
            {
                i_shield = value;
            }
        }

        #endregion


    }
}
