using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;

namespace MarioWorldSharp
{
    public enum PowerupEnum
    {
        Small = 0,
        Mushroom = 1,
        Cape = 2,
        Fire = 3
    }

    public class Player
    {
        private double _xPos;
        private double _yPos;
        public double XPosition
        {
            get => _xPos;
            set
            {
                _xPos = value;
                if (collisionBox != null)
                    collisionBox.X = (int)value;
            }
        }
        public double YPosition
        {
            get => _yPos;
            set
            {
                _yPos = value;
                if (collisionBox != null)
                    collisionBox.Y = (int)value + colDisp;
            }
        }
        public double XSpeed { get; set; }
        public double YSpeed { get; set; }
        public byte VertGravity { get; set; }
        public byte HorizGravity { get; set; }
        public bool BlockedBellow { get; set; }
        public bool BlockedAbove { get; set; }
        public bool BlockedLeft { get; set; }
        public bool BlockedRight { get; set; }
        public PowerupEnum Powerup { get; set; }
        public Texture2D[] Poses { get; set; }
        public int Pose { get; set; }
        public int YDrawDisplacement { get; set; }
        public byte DashTimer { get; set; }

        private Rectangle collisionBox;
        private bool facingRight;
        private byte animationTimer;
        private int colDisp;
        private bool jumped;
        private bool dashJumped;
        private bool spinJumped;
        private bool flip;
        private bool debug;
        private bool debugged;
        private bool ducking;

        public Player() : this(0, 0)
        {}

        public Player(double XPosition, double YPosition)
        {
            facingRight = true;
            VertGravity = 1;
            HorizGravity = 0;
            if (!Powerup.Equals(PowerupEnum.Small))
            {
                colDisp = 0;
                YDrawDisplacement = 0;
                collisionBox = new Rectangle(0, 0, 16, 32);
            }
            else
            {
                YDrawDisplacement = 10;
                colDisp = 16;
                collisionBox = new Rectangle(0, 0, 16, 16);
            }
            DashTimer = 0;
            Pose = 0;

            this.XPosition = XPosition;
            this.YPosition = YPosition;
            Console.Write("It's a me!");
            Poses = new Texture2D[70];

            //Input Events
            MarioWorld.InputEvent.JumpPressEvent += Jump;
            MarioWorld.InputEvent.SpinPressEvent += Spinjump;
            MarioWorld.InputEvent.JumpDownEvent += Jumping;
            MarioWorld.InputEvent.SpinDownEvent += Jumping;
        }

        public Rectangle GetCollisionBox() { return collisionBox; }

        public void Process()
        {
            if (Poses.GetLength(0) < 70)
                throw new FormatException();
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                if (!debugged)
                {
                    if (!debug)
                        debug = true;
                    else
                        debug = false;
                    debugged = true;
                }
            }
            else
                debugged = false;

            if (debug)
            {
                double move = 2.0;
                var keyState = Keyboard.GetState();
                if (keyState.IsKeyDown(Keys.A))
                    move = 6.0;
                if (keyState.IsKeyDown(Keys.Left))
                    XPosition -= move;
                if (keyState.IsKeyDown(Keys.Right))
                    XPosition += move;
                if (keyState.IsKeyDown(Keys.Up))
                    YPosition -= move;
                if (keyState.IsKeyDown(Keys.Down))
                    YPosition += move;
            }
            else
            {
                Move();
                EnvironmentCollision();
                UpdateXPositionition();
                UpdateYPositionition();
            }
        }

        public SpriteEffects GetSpriteEffect()
        {
            if (!flip)
                return SpriteEffects.FlipHorizontally;
            else
                return SpriteEffects.None;
        }

        public Texture2D GetTexture()
        {
            return Poses[Pose];
        }


        public static readonly int VertColisionOffset = 5;
        public static readonly int HorizCollisionOffset = 3;
        private void EnvironmentCollision()
        {
            BlockedAbove = false;
            BlockedBellow = false;
            BlockedLeft = false;
            BlockedRight = false;

            //Check left collision
            MarioWorld.level.GetMap16FromPosition(collisionBox.Left + HorizCollisionOffset, collisionBox.Top + collisionBox.Height / 2).Left(this, collisionBox.Left + HorizCollisionOffset, collisionBox.Top + collisionBox.Height / 2);

            //Check right collision
            MarioWorld.level.GetMap16FromPosition(collisionBox.Right - HorizCollisionOffset, collisionBox.Top + collisionBox.Height / 2).Right(this, collisionBox.Right - HorizCollisionOffset, collisionBox.Top + collisionBox.Height / 2);

            //Check bottom collision
            MarioWorld.level.GetMap16FromPosition(collisionBox.Left + VertColisionOffset, collisionBox.Bottom).Bellow(this, collisionBox.Left + VertColisionOffset, collisionBox.Bottom);
            MarioWorld.level.GetMap16FromPosition(collisionBox.Right - VertColisionOffset, collisionBox.Bottom).Bellow(this, collisionBox.Right - VertColisionOffset, collisionBox.Bottom);

            //Check top collision
            MarioWorld.level.GetMap16FromPosition(collisionBox.Left + VertColisionOffset, collisionBox.Top).Above(this, collisionBox.Left + VertColisionOffset, collisionBox.Top);
            MarioWorld.level.GetMap16FromPosition(collisionBox.Right - VertColisionOffset, collisionBox.Top).Above(this, collisionBox.Right - VertColisionOffset, collisionBox.Top);
        }

        private void UpdateXPositionition()
        {
            XPosition += XSpeed;
        }

        private void UpdateYPositionition()
        {
            double gravity = VertGravity * .375;
            if (Input.Jump.IsKeyHeld() || Input.Spinjump.IsKeyHeld())
                gravity = VertGravity * 0.1875;
            if (YSpeed < 64.0 / 16.0)
                YSpeed += gravity;
            YPosition += YSpeed;
        }

        private void Jump(object sender, EventArgs e)
        {
            if (!jumped && BlockedBellow)
            {
                YSpeed = (-80.0 - ((640.0 * Math.Abs(XSpeed * 2.0)) / 256.0)) / 16.0;
            }
        }

        private void Jumping(object sender, EventArgs e)
        {
            if (animationTimer == 0 && YSpeed <= 0 && !BlockedBellow)
            {
                if (dashJumped)
                    Pose = 0x0C;
                else
                    Pose = 0x0B;
            }
            if (!jumped && BlockedBellow)
            {
                jumped = true;
                if (DashTimer >= 112)
                    dashJumped = true;
            }
        }

        private void Spinjump(object sender, EventArgs e)
        {
            if (!jumped && BlockedBellow)
            {
                spinJumped = true;
                YSpeed = (-74.0 - ((592.0 * Math.Abs(XSpeed * 2.0)) / 256.0)) / 16.0;
            }
        }

        private void Move()
        {
            double acceleration = 0.09375;
            double decceleration = .3125;
            //SMW velocity to double: v / 16.0
            double maxXSpeed = 20.0 / 16.0;

            if (BlockedBellow && Input.Down.IsKeyHeld())
                ducking = true;
            else if (BlockedBellow)
                ducking = false;

            bool moving = (Input.Left.IsKeyHeld() || Input.Right.IsKeyHeld()) && !ducking;

            if (Input.Dash.IsKeyHeld())
            {
                decceleration = .3125;
                acceleration = 0.09375;
                if (DashTimer >= 112)
                    maxXSpeed = 48.0 / 16.0;
                else
                    maxXSpeed = 36.0 / 16.0;
            }

            if (BlockedBellow && (moving || XSpeed != 0))
            {
                if (animationTimer == 0)
                {
                    Pose++;
                    if (DashTimer == 112)
                        Pose = (Pose % 2) + 4;
                    else
                        Pose %= 2;
                    if (Math.Abs(XSpeed) >= 48.0 / 16.0)
                        animationTimer = 2;
                    else if (Math.Abs(XSpeed) >= 36.0 / 16.0)
                        animationTimer = 3;
                    else if (Math.Abs(XSpeed) >= 20.0 / 16.0)
                        animationTimer = 6;
                    else
                        animationTimer = 10;
                }
            }
            else
            {
                if (BlockedBellow)
                    Pose = 0;
            }

            if (animationTimer > 0)
                animationTimer--;
            
            if (BlockedBellow && moving && Input.Dash.IsKeyHeld())
            {
                if (Math.Abs(XSpeed) >= 36.0 / 16.0)
                {
                    if (DashTimer < 112)
                        DashTimer += 2;
                    else
                        DashTimer = 112;
                }
                else
                    if (DashTimer > 0)
                    DashTimer--;
            }
            else
            {
                if (!BlockedBellow && Input.Dash.IsKeyHeld() && DashTimer >= 112)
                {
                    if (Math.Abs(XSpeed) < 48.0 / 16.0)
                        DashTimer--;
                }
                else if (DashTimer > 0)
                    DashTimer--;
            }

            if (!dashJumped && DashTimer < 112 && animationTimer == 0 && !BlockedBellow && (YSpeed > 0 || (YSpeed < 0 && !jumped)))
                Pose = 0x24;

            if (!(Input.Spinjump.IsKeyHeld() || Input.Jump.IsKeyHeld()))
            {
                if (BlockedBellow)
                {
                    if (jumped)
                        animationTimer = 0;
                    jumped = false;
                    dashJumped = false;
                    spinJumped = false;
                }
            }

            if (spinJumped && !BlockedBellow)
            {
                if (animationTimer == 0 || animationTimer > 7)
                    animationTimer = 7;
                int[] posePointers = { 0, 0x0F, 0, 0x39 };
                bool[] facingRightSpinning = { false, false, true, false };
                Pose = posePointers[(animationTimer - 1) / 2];
                flip ^= facingRightSpinning[(animationTimer - 1) / 2];
                ducking = false;
            }
            else
                flip = facingRight;

            if (XSpeed == 0 && !moving && BlockedBellow && Input.Up.IsKeyHeld())
                Pose = 0x03;
            if (ducking)
                Pose = 0x3C;

            moving = (Input.Left.IsKeyHeld() || Input.Right.IsKeyHeld()) && !ducking;
            bool notTwoHDir = !(Input.Left.IsKeyHeld() && Input.Right.IsKeyHeld());

            #region Moving
            if ((!BlockedBellow || !ducking) && notTwoHDir && Input.Left.IsKeyHeld())
            {
                facingRight = false;
                if (XSpeed > 0)
                {
                    if (BlockedBellow)
                        Pose = 0x0D;
                    XSpeed -= decceleration;
                }
                else
                {
                    if (XSpeed + acceleration > -maxXSpeed)
                        XSpeed -= acceleration;
                    else
                        XSpeed = -maxXSpeed;
                }
            }
            else if ((!BlockedBellow || !ducking) && notTwoHDir && Input.Right.IsKeyHeld())
            {
                facingRight = true;
                if (XSpeed < 0)
                {
                    if (BlockedBellow)
                        Pose = 0x0D;
                    XSpeed += decceleration;
                }
                else
                {
                    if (XSpeed + acceleration < maxXSpeed)
                        XSpeed += acceleration;
                    else
                        XSpeed = maxXSpeed;
                }
            }
            #endregion
            else
            {
                # region Friction (Decceleration only on ground)
                if (BlockedBellow)
                {
                    if (!moving && XSpeed > 0)
                    {
                        if (XSpeed - .125 <= 0)
                            XSpeed = 0;
                        else
                            XSpeed -= .125;
                    }
                    else if (!moving && XSpeed < 0)
                    {
                        if (XSpeed + .125 >= 0)
                            XSpeed = 0;
                        else
                            XSpeed += .125;
                    }
                }
                #endregion

                if (facingRight && moving)
                {
                    if (XSpeed + .125 <= maxXSpeed)
                        XSpeed += .125;
                }
                else if (!facingRight && moving)
                {
                    if (XSpeed - .125 >= -maxXSpeed)
                        XSpeed -= .125;
                }
            }
        }
    }
}
