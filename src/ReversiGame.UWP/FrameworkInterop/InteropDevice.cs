using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Walterlv.ReversiGame.FrameworkInterop;

namespace Walterlv.Gaming.Reversi.FrameworkInterop
{
    internal class InteropKeyboard : IKeyboard
    {
        private static readonly List<VirtualKey> PressingKeys = new List<VirtualKey>();

        public IKeyboardState GetState(params Keys[] keys)
        {
            return new InteropKeyboardState(
                keys.Where(x => PressingKeys.Contains(x.ToVirtualKey())));
        }

        internal static void Press(VirtualKey key)
        {
            if (!PressingKeys.Contains(key))
            {
                PressingKeys.Add(key);
            }
        }

        internal static void Release(VirtualKey key)
        {
            if (PressingKeys.Contains(key))
            {
                PressingKeys.Remove(key);
            }
        }
    }

    internal class InteropKeyboardState : IKeyboardState
    {
        private readonly List<VirtualKey> _keys;

        public InteropKeyboardState(IEnumerable<Keys> downKeys)
        {
            _keys = downKeys.Select(x => x.ToVirtualKey()).ToList();
        }

        public bool IsKeyDown(Keys key)
        {
            return _keys.Contains(key.ToVirtualKey());
        }

        public bool IsKeyUp(Keys key)
        {
            return !_keys.Contains(key.ToVirtualKey());
        }
    }


    internal class InteropMouse : IMouse
    {
        private static Point _lastPosition;
        private static readonly Queue<bool> LastButtonStates = new Queue<bool>();

        public IMouseState GetState()
        {
            if (LastButtonStates.Count > 1)
            {
                return new InteropMouseState(
                    (int) _lastPosition.X, (int) _lastPosition.Y,
                    LastButtonStates.Dequeue());
            }
            else if (LastButtonStates.Count == 1)
            {
                return new InteropMouseState(
                    (int) _lastPosition.X, (int) _lastPosition.Y,
                    LastButtonStates.First());
            }
            else
            {
                return new InteropMouseState(
                    (int) _lastPosition.X, (int) _lastPosition.Y,
                    false);
            }
        }

        internal static void EnqueueState(Point position)
        {
            _lastPosition = position;
        }

        internal static void EnqueueState(bool leftButtonState)
        {
            if (!LastButtonStates.Any())
            {
                LastButtonStates.Enqueue(leftButtonState);
            }
            else
            {
                var last = LastButtonStates.Last();
                if (last != leftButtonState)
                {
                    LastButtonStates.Enqueue(leftButtonState);
                }
            }
        }
    }

    internal class InteropMouseState : IMouseState
    {
        public int X { get; }
        public int Y { get; }
        public ButtonState LeftButton { get; }

        public InteropMouseState(int x, int y, bool leftButton)
        {
            X = x;
            Y = y;
            LeftButton = leftButton ? ButtonState.Pressed : ButtonState.Released;
        }
    }
}
