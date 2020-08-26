using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MarioWorldSharp
{
    public class InputEvent
    {
        public event EventHandler JumpPressEvent;
        public event EventHandler SpinPressEvent;
        public event EventHandler DashPressEvent;

        public event EventHandler UpPressEvent;
        public event EventHandler DownPressEvent;
        public event EventHandler LeftPressEvent;
        public event EventHandler RightPressEvent;

        public event EventHandler DEBUG_ShowHitboxEvent;
        public event EventHandler DEBUG_PrintSpriteTreeEvent;
        public event EventHandler DEBUG_KillAllSpritesEvent;
        public event EventHandler DEBUG_ResetLevelEvent;
        public void Process()
        {
            if (Input.Jump.IsKeyPressed())
                JumpPressEvent.Invoke(this, EventArgs.Empty);
            if (Input.Spinjump.IsKeyPressed())
                SpinPressEvent.Invoke(this, EventArgs.Empty);

            if (Input.ShowHitboxes.IsKeyPressed())
                DEBUG_ShowHitboxEvent.Invoke(this, EventArgs.Empty);
            if (Input.DEBUG_PrintSpriteTree.IsKeyPressed())
                DEBUG_PrintSpriteTreeEvent.Invoke(this, EventArgs.Empty);
            if (Input.DEBUG_KillAllSprites.IsKeyPressed())
                DEBUG_KillAllSpritesEvent.Invoke(this, EventArgs.Empty);
            if (Input.DEBUG_ResetLevel.IsKeyPressed())
                DEBUG_ResetLevelEvent.Invoke(this, EventArgs.Empty);


        }
    }
    public class Input
    {
        public string Name { get; set; }
        public IInputWrapper[] Inputs { get; set; }

        private bool isHeld;

        public bool IsKeyHeld()
        {
            for (int i = 0; i < Inputs.Length; i++)
            {
                if (Inputs[i].IsInputDown())
                    return true;
            }
            return false;
        }

        //Checks to see if key is pressed for one frame
        public bool IsKeyPressed()
        {
            bool keyheld2 = this.IsKeyHeld();
            if (!isHeld && keyheld2)
            {
                isHeld = true;
                return true;
            }
            if (!keyheld2)
                isHeld = false;
            return false;
        }

        public override string ToString()
        {
            return Name;
        }

        public static Input Jump = new Input
        {
            Name = "Jump",
            Inputs = new IInputWrapper[]
            {
                new KeyWrapper(Keys.Z),
                new ButtonWrapper(Buttons.A)
            }
        };
        public static Input Spinjump = new Input
        {
            Name = "Spinjump",
            Inputs = new IInputWrapper[]
            {
                new KeyWrapper(Keys.X),
                new ButtonWrapper(Buttons.B)
            }
        };
        public static Input Dash = new Input
        {
            Name = "Dash",
            Inputs = new IInputWrapper[]
            {
                new KeyWrapper(Keys.A),
                new KeyWrapper(Keys.S),
                new ButtonWrapper(Buttons.X),
                new ButtonWrapper(Buttons.Y)
            }
        };
        public static Input Up = new Input
        {
            Name = "Up",
            Inputs = new IInputWrapper[]
            {
                new KeyWrapper(Keys.Up),
                new ButtonWrapper(Buttons.LeftThumbstickUp),
                new ButtonWrapper(Buttons.DPadUp)
            }
        };
        public static Input Down = new Input
        {
            Name = "Down",
            Inputs = new IInputWrapper[]
            {
                new KeyWrapper(Keys.Down),
                new ButtonWrapper(Buttons.LeftThumbstickDown),
                new ButtonWrapper(Buttons.DPadDown)
            }
        };
        public static Input Left = new Input
        {
            Name = "Left",
            Inputs = new IInputWrapper[]
            {
                new KeyWrapper(Keys.Left),
                new ButtonWrapper(Buttons.LeftThumbstickLeft),
                new ButtonWrapper(Buttons.DPadLeft)
            }
        };
        public static Input Right = new Input
        {
            Name = "Right",
            Inputs = new IInputWrapper[]
            {
                new KeyWrapper(Keys.Right),
                new ButtonWrapper(Buttons.LeftThumbstickRight),
                new ButtonWrapper(Buttons.DPadRight)
            }
        };

        public static Input ShowHitboxes = new Input
        {
            Name = "ShowHitboxes",
            Inputs = new IInputWrapper[]
            {
                new KeyWrapper(Keys.G)
            }
        };
        public static Input DEBUG_PrintSpriteTree = new Input
        {
            Name = "DEBUG_PrintSpriteTree",
            Inputs = new IInputWrapper[]
            {
                new KeyWrapper(Keys.H)
            }
        };
        public static Input DEBUG_KillAllSprites = new Input
        {
            Name = "DEBUG_KillAllSprites",
            Inputs = new IInputWrapper[]
            {
                new KeyWrapper(Keys.K)
            }
        };
        public static Input DEBUG_ResetLevel = new Input
        {
            Name = "DEBUG_ResetLevel",
            Inputs = new IInputWrapper[]
            {
                new KeyWrapper(Keys.R)
            }
        };

    }

    public interface IInputWrapper
    {
        public bool IsInputDown();
    }

    public class ButtonWrapper : IInputWrapper
    {
        private Buttons button;
        public ButtonWrapper(Buttons b)
        {
            button = b;
        }

        public bool IsInputDown()
        {
            return GamePad.GetState(0).IsButtonDown(button);
        }
    }

    public class KeyWrapper : IInputWrapper
    {
        private Keys button;
        public KeyWrapper(Keys b)
        {
            button = b;
        }

        public bool IsInputDown()
        {
            return Keyboard.GetState().IsKeyDown(button);
        }
    }
}
