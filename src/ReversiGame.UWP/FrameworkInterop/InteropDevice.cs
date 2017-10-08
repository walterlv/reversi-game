using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Walterlv.ReversiGame.FrameworkInterop;

namespace Walterlv.Gaming.Reversi.FrameworkInterop
{
    internal class InteropKeyboard : IKeyboard
    {
        public IKeyboardState GetState(params Keys[] keys)
        {
            return new InteropKeyboardState(
                keys.Where(x =>
                    Window.Current.CoreWindow.GetKeyState(x.ToVirtualKey())
                        .HasFlag(CoreVirtualKeyStates.Down))
            );
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
        public IMouseState GetState()
        {
            ;
            return new InteropMouseState(0, 0,
                Window.Current.CoreWindow.GetKeyState(VirtualKey.LeftButton)
                    .HasFlag(CoreVirtualKeyStates.Down));
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
