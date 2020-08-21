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
        public event EventHandler JumpDownEvent;
        public event EventHandler SpinDownEvent;
        public event EventHandler DashPressEvent;
        public event EventHandler UpPressEvent;
        public event EventHandler DownPressEvent;
        public event EventHandler LeftPressEvent;
        public event EventHandler RightPressEvent;
        public event EventHandler DEBUG_ShowHitboxEvent;
        public void Process()
        {
            if (Input.Jump.IsKeyPressed())
                JumpPressEvent.Invoke(this, EventArgs.Empty);
            if (Input.Spinjump.IsKeyPressed())
                SpinPressEvent.Invoke(this, EventArgs.Empty);

            if (Input.Jump.IsKeyHeld())
                JumpDownEvent.Invoke(this, EventArgs.Empty);
            if (Input.Spinjump.IsKeyHeld())
                SpinDownEvent.Invoke(this, EventArgs.Empty);

            if (Input.ShowHitboxes.IsKeyPressed())
                DEBUG_ShowHitboxEvent.Invoke(this, EventArgs.Empty);
        }
    }
    public class Input
    {
        public string Name { get; set; }
        public IInputWrapper[] Inputs { get; set; }

        private bool isHeld;

        public Input(string n, params IInputWrapper[] kArr)
        {
            Name = n;
            Inputs = kArr;
            isHeld = false;
        }

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

        public static Input Jump = new Input("Jump", 
            new KeyWrapper(Keys.Z),
            new ButtonWrapper(Buttons.A));
        public static Input Spinjump = new Input("Spinump", 
            new KeyWrapper(Keys.X),
            new ButtonWrapper(Buttons.B));
        public static Input Dash = new Input("Dash", 
            new KeyWrapper(Keys.A), 
            new KeyWrapper(Keys.S),
            new ButtonWrapper(Buttons.X),
            new ButtonWrapper(Buttons.Y));
        public static Input Up = new Input("Up",
            new KeyWrapper(Keys.Up),
            new ButtonWrapper(Buttons.LeftThumbstickUp),
            new ButtonWrapper(Buttons.DPadUp));
        public static Input Down = new Input("Down",
            new KeyWrapper(Keys.Down),
            new ButtonWrapper(Buttons.LeftThumbstickDown),
            new ButtonWrapper(Buttons.DPadDown));
        public static Input Left = new Input("Left",
            new KeyWrapper(Keys.Left),
            new ButtonWrapper(Buttons.LeftThumbstickLeft),
            new ButtonWrapper(Buttons.DPadLeft));
        public static Input Right = new Input("Right",
            new KeyWrapper(Keys.Right),
            new ButtonWrapper(Buttons.LeftThumbstickRight),
            new ButtonWrapper(Buttons.DPadRight));

        public static Input ShowHitboxes = new Input("ShowHitboxes",
            new KeyWrapper(Keys.G));

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
