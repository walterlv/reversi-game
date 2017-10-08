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
        private CoreDispatcher _dispatcher;
        private InteropKeyboardState _lastAsyncState;

        public IKeyboardState GetState(params Keys[] keys)
        {
            if (Window.Current == null)
            {
                _dispatcher?.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    _lastAsyncState = new InteropKeyboardState(
                        keys.Where(x =>
                            Window.Current.CoreWindow.GetKeyState(x.ToVirtualKey())
                                .HasFlag(CoreVirtualKeyStates.Down)));
                });
                return _lastAsyncState ?? new InteropKeyboardState(Enumerable.Empty<Keys>());
            }
            else
            {
                _dispatcher = Window.Current.Dispatcher;
                return new InteropKeyboardState(
                    keys.Where(x =>
                        Window.Current.CoreWindow.GetKeyState(x.ToVirtualKey())
                            .HasFlag(CoreVirtualKeyStates.Down))
                );
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
        internal static Point LastMousePoint;
        private CoreDispatcher _dispatcher;
        private InteropMouseState _lastAsyncState;

        public IMouseState GetState()
        {
            if (Window.Current == null)
            {
                _dispatcher?.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    _lastAsyncState = new InteropMouseState(
                        (int)LastMousePoint.X, (int)LastMousePoint.Y,
                        Window.Current.CoreWindow.GetKeyState(VirtualKey.LeftButton)
                            .HasFlag(CoreVirtualKeyStates.Down));
                });
                return _lastAsyncState ?? new InteropMouseState(0, 0, false);
            }
            else
            {
                _dispatcher = Window.Current.Dispatcher;
                return new InteropMouseState(
                    (int)LastMousePoint.X, (int)LastMousePoint.Y,
                    Window.Current.CoreWindow.GetKeyState(VirtualKey.LeftButton)
                        .HasFlag(CoreVirtualKeyStates.Down));
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
