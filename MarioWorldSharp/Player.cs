using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using MonoGame.Utilities;

namespace MarioWorldSharp
{
    public enum Powerup
    {
        Small = 0,
        Mushroom = 1,
        Cape = 2,
        Fire = 3
    }

    public class Player
    {
        public double xPos { get; set; }
        public double yPos { get; set; }
        public double xSpeed { get; set; }
        public double ySpeed { get; set; }
        public double xAccel { get; set; }
        public double yAccel { get; set; }
        public byte vGravity { get; set; }
        public byte hGravity { get; set; }
        public bool blockedBellow { get; set; }
        public bool blockedAbove { get; set; }
        public bool blockedLeft { get; set; }
        public bool blockedRight { get; set; }
        public Powerup power { get; set; }
        private Rectangle collisionBox;
        private bool facingRight;
        private byte animationTimer;
        public Texture2D[] Poses { get; set; }
        public int Pose { get; set; }

        private int colDisp;
        public int yDrawDisp { get; set; }
        private bool jumped;
        public byte dashTimer { get; set; }
        private bool dashJumped;
        private bool spinJumped;
        private bool flip;
        private bool debug;
        private bool debugged;
        private bool ducking;

        public Player()
        {
            facingRight = true;
            xPos = 0;
            yPos = 0;
            vGravity = 1;
            hGravity = 0;
            if (!power.Equals(Powerup.Small))
            {
                collisionBox = new Rectangle((int)xPos, (int)yPos, 16, 32);
                colDisp = 0;
                yDrawDisp = 0;
            }
            else
            {
                yDrawDisp = 10;
                colDisp = 16;
                collisionBox = new Rectangle((int)xPos, (int)yPos + colDisp, 16, 16);
            }
            dashTimer = 0;
            Pose = 0;

            Console.Write("It's a me!");
            Poses = new Texture2D[70];
        }

        public Player(double xPos, double yPos)
        {
            facingRight = true;
            this.xPos = xPos;
            this.yPos = yPos;
            vGravity = 1;
            hGravity = 0;
            collisionBox = new Rectangle((int)xPos, (int)yPos, 16, 16);
            Console.Write("It's a me!");
        }

        public double GetX() { return xPos; }
        public double GetY() { return yPos; }
        public double GetXSpeed() { return xSpeed; }
        public double GetYSpeed() { return ySpeed; }
        public double GetXAccel() { return xAccel; }
        public double GetYAccel() { return yAccel; }
        public Rectangle GetCollisionBox() { return collisionBox; }
        public bool GetBlockedAbove() { return blockedAbove; }
        public bool GetBlockedBellow() { return blockedBellow; }
        public bool GetBlockedLeft() { return blockedLeft; }
        public bool GetBlockedRight() { return blockedRight; }
        public bool IsFacingRight() { return facingRight; }

        public void SetX(double newX) { xPos = newX; }
        public void SetY(double newY) { yPos = newY; }
        public void SetXSpeed(double newX) { xSpeed = newX; }
        public void SetYSpeed(double newY) { ySpeed = newY; }
        public void SetXAccel(double newX) { xAccel = newX; }
        public void SetYAccel(double newY) { yAccel = newY; }
        public void SetBlockedAbove(bool b) { blockedAbove = b; }
        public void SetBlockedBellow(bool b) { blockedBellow = b; }
        public void SetBlockedLeft(bool b) { blockedLeft = b; }
        public void SetBlockedRight(bool b) { blockedRight = b; }

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
                    xPos -= move;
                if (keyState.IsKeyDown(Keys.Right))
                    xPos += move;
                if (keyState.IsKeyDown(Keys.Up))
                    yPos -= move;
                if (keyState.IsKeyDown(Keys.Down))
                    yPos += move;
            }
            else
            {
                Move();
                UpdateXPosition();
                UpdateYPosition();
                EnvironmentCollision();
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

        private void EnvironmentCollision()
        {
            collisionBox.X = (int)xPos;
            collisionBox.Y = (int)yPos + colDisp;

            Level level = new Level();
            blockedAbove = false;
            blockedBellow = false;
            blockedLeft = false;
            blockedRight = false;

            using (Block map16 = level.GetMap16(collisionBox.X + 2 / 16, collisionBox.Bottom / 16))
            {
                map16.Bellow(this, collisionBox.X+2, collisionBox.Bottom);
            }
            if (!blockedBellow)
            {
                using (Block map16 = level.GetMap16(collisionBox.Right-2 / 16, collisionBox.Bottom / 16))
                {
                    map16.Bellow(this, collisionBox.Right-2, collisionBox.Bottom);
                }
            }

            using (Block map16 = level.GetMap16(collisionBox.X+2 / 16, collisionBox.Y / 16))
            {
                map16.Above(this, collisionBox.X+2, collisionBox.Y);
            }
            if (!blockedBellow)
            {
                using (Block map16 = level.GetMap16(collisionBox.Right-2 / 16, collisionBox.Y / 16))
                {
                    map16.Above(this, collisionBox.Right-2, collisionBox.Y);
                }
            }

            //level.GetMap16(collisionBox.X / 16, collisionBox.Y + (collisionBox.Height/2) / 16).Left(this, collisionBox.X, collisionBox.Y + (collisionBox.Height / 2));
            //if (!blockedLeft)
            //    level.GetMap16(collisionBox.Right / 16, collisionBox.Y + (collisionBox.Height / 2) / 16).Right(this, collisionBox.Right, collisionBox.Y + (collisionBox.Height / 2));

            collisionBox.X = (int)xPos;
            collisionBox.Y = (int)yPos + colDisp;
        }

        private void UpdateXPosition()
        {
            bool neg = xAccel < 0;
            xSpeed += xAccel;
            xPos += xSpeed;
        }

        private void UpdateYPosition()
        {
            var keySate = Keyboard.GetState();
            double gravity = vGravity * .375;
            if (keySate.IsKeyDown(Keys.Z) || keySate.IsKeyDown(Keys.X))
                gravity = vGravity * 0.1875;
            if (ySpeed + yAccel < 64.0 / 16.0)
                ySpeed += (yAccel + gravity);
            yPos += ySpeed;
        }

        private void Move()
        {
            double acceleration = 0.09375;
            double decceleration = .3125;
            //0.0078125
            double maxXSpeed = 20.0 / 16.0;
            var keySate = Keyboard.GetState();  //Lol misspelled state
            if (keySate.IsKeyDown(Keys.A))
            {
                decceleration = .3125;
                acceleration = 0.09375;
                if (dashTimer >= 112)
                    maxXSpeed = 48.0 / 16.0;
                else
                    maxXSpeed = 36.0 / 16.0;
            }

            if (blockedBellow && xSpeed != 0)
            {
                if (animationTimer == 0)
                {
                    Pose++;
                    if (dashTimer == 112)
                        Pose = (Pose % 2) + 4;
                    else
                        Pose %= 2;
                    if (Math.Abs(xSpeed) >= 48.0 / 16.0)
                        animationTimer = 2;
                    else if (Math.Abs(xSpeed) >= 36.0 / 16.0)
                        animationTimer = 3;
                    else if (Math.Abs(xSpeed) >= 20.0 / 16.0)
                        animationTimer = 6;
                    else
                        animationTimer = 10;
                }
            }
            else
            {
                if (blockedBellow)
                    Pose = 0;
            }

            if (animationTimer > 0)
                animationTimer--;
            
            if (blockedBellow && keySate.IsKeyDown(Keys.A) && (keySate.IsKeyDown(Keys.Left) || keySate.IsKeyDown(Keys.Right)))
            {
                if (!facingRight)
                {
                    if (xSpeed <= -20.0 / 16.0)
                    {
                        if (dashTimer < 112)
                            dashTimer += 2;
                        else
                            dashTimer = 112;
                    }
                    else
                        if (dashTimer > 0)
                        dashTimer--;
                }
                else
                {
                    if (xSpeed >= 20.0 / 16.0)
                    {
                        if (dashTimer < 112)
                            dashTimer += 2;
                        else
                            dashTimer = 112;
                    }
                    else
                        if (dashTimer > 0)
                        dashTimer--;
                }
            }
            else
            {
                if (!blockedBellow && keySate.IsKeyDown(Keys.A) && dashTimer == 112)
                {
                    if (Math.Abs(xSpeed) < 48.0 / 16.0)
                        dashTimer--;
                }
                else if (dashTimer > 0)
                    dashTimer--;
            }

            if (!dashJumped && dashTimer < 112 && animationTimer == 0 && !blockedBellow && (ySpeed > 0 || (ySpeed < 0 && !jumped)))
                Pose = 0x24;

            if (keySate.IsKeyDown(Keys.Z) || keySate.IsKeyDown(Keys.X))
            {
                if (animationTimer == 0 && ySpeed <= 0 && !blockedBellow)
                {
                    if (dashJumped)
                        Pose = 0x0C;
                    else
                        Pose = 0x0B;
                }
                if (!jumped && blockedBellow)
                {
                    spinJumped = keySate.IsKeyDown(Keys.X);
                    if (spinJumped)
                        ySpeed = (-74.0 - ((592.0 * Math.Abs(xSpeed * 2.0)) / 256.0)) / 16.0;
                    else
                        ySpeed = (-80.0 - ((640.0 * Math.Abs(xSpeed * 2.0)) / 256.0)) / 16.0;
                    jumped = true;
                    if (dashTimer >= 112)
                        dashJumped = true;
                }
            }
            else
            {
                if (blockedBellow)
                {
                    if (jumped)
                        animationTimer = 0;
                    jumped = false;
                    dashJumped = false;
                    spinJumped = false;
                }
            }

            if (spinJumped && !blockedBellow)
            {
                if (animationTimer == 0 || animationTimer > 7)
                    animationTimer = 7;
                int[] posePointers = { 0, 0x0F, 0, 0x39 };
                bool[] facingRightSpinning = { false, false, true, false };
                Pose = posePointers[(animationTimer - 1) / 2];
                flip ^= facingRightSpinning[(animationTimer - 1) / 2];
            }
            else
                flip = facingRight;

            if (xSpeed == 0 && blockedBellow && keySate.IsKeyDown(Keys.Up))
                Pose = 0x03;
            if (blockedBellow && keySate.IsKeyDown(Keys.Down))
                ducking = true;
            else if (blockedBellow)
                ducking = false;
            if (ducking)
                Pose = 0x3C;

            bool notTwoHDir = !(keySate.IsKeyDown(Keys.Left) && keySate.IsKeyDown(Keys.Right));

            if ((!blockedBellow || !ducking) && notTwoHDir && keySate.IsKeyDown(Keys.Left) && xSpeed > -maxXSpeed)
            {
                facingRight = false;
                if (xSpeed > 0)
                {
                    if (blockedBellow)
                        Pose = 0x0D;
                    xAccel = -decceleration;
                }
                else
                {
                    if (-maxXSpeed + xSpeed > acceleration)
                        xAccel = -maxXSpeed + xSpeed;
                    else
                        xAccel = -acceleration;
                }
                return;
            }
            else if ((!blockedBellow || !ducking) && notTwoHDir && keySate.IsKeyDown(Keys.Right) && xSpeed < maxXSpeed)
            {
                facingRight = true;
                if (xSpeed < 0)
                {
                    if (blockedBellow)
                        Pose = 0x0D;
                    xAccel = decceleration;
                }
                else
                {
                    if (maxXSpeed - xSpeed < acceleration)
                        xAccel = maxXSpeed - xSpeed;
                    else
                        xAccel = acceleration;
                }
                return;
            }
            else
            {
                //Friction (Decceleration only on ground)
                if (blockedBellow)
                {
                    if ((ducking || !keySate.IsKeyDown(Keys.Right)) && xSpeed > 0)
                    {
                        if (xSpeed < .125)
                            xAccel = xSpeed;
                        else
                            xAccel = -.125;
                    }
                    else if ((ducking || !keySate.IsKeyDown(Keys.Left)) && xSpeed < 0)
                    {
                        if (xSpeed > .125)
                            xAccel = xSpeed;
                        else
                            xAccel = .125;
                    }
                    else
                    {
                        if (facingRight && xSpeed > maxXSpeed)
                        {
                            if (xSpeed - maxXSpeed < .125)
                                xAccel = xSpeed;
                            else
                                xAccel = -.125;
                        }
                        else if (!facingRight && xSpeed < -maxXSpeed)
                        {
                            if (xSpeed + maxXSpeed > .125)
                                xAccel = xSpeed;
                            else
                                xAccel = .125;
                        }
                        else
                            xAccel = 0;
                    }
                }
                else
                {
                    if (facingRight && xSpeed > maxXSpeed)
                    {
                        if (xSpeed - maxXSpeed < .125)
                            xAccel = xSpeed - maxXSpeed;
                        else
                            xAccel = -.125;
                    }
                    else if (!facingRight && xSpeed < -maxXSpeed)
                    {
                        if (xSpeed + maxXSpeed > .125)
                            xAccel = xSpeed - maxXSpeed;
                        else
                            xAccel = .125;
                    }
                    else
                        xAccel = 0;
                }
            }
        }
    }
}
