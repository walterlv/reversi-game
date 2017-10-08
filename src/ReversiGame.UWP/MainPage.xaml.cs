using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Walterlv.Gaming.Reversi.FrameworkInterop;
using Walterlv.ReversiGame.FrameworkInterop;

namespace Walterlv.Gaming.Reversi
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Window.Current.CoreWindow.KeyDown += OnKeyDown;
            Window.Current.CoreWindow.KeyUp += OnKeyUp;
        }

        private Game _game;
        private InteropDrawingSession _interopDrawing;

        private async void CanvasAnimatedControl_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            _interopDrawing = new InteropDrawingSession();
            _game = DrawableGameComponent.CreateGame<ReversiXNAGame.ReversiXNAGame>(
                _interopDrawing, new InteropKeyboard(), new InteropMouse());
            _interopDrawing.Game = _game;

            _game.Initialize();
            _game.LoadAllContents();
            await _interopDrawing.CreateResources(sender, args);
        }

        private void CanvasAnimatedControl_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            _game.Update((GameTime) args.Timing.ElapsedTime);
        }

        private void CanvasAnimatedControl_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            using (var ds = args.DrawingSession)
            {
                _interopDrawing.Session = ds;
                try
                {
                    _game.Draw((GameTime) args.Timing.ElapsedTime);
//                    ds.DrawText("123456", new System.Numerics.Vector2(100,100), Color.);
                }
                finally
                {
                    _interopDrawing.Session = null;
                }
            }
        }

        private void CanvasAnimatedControl_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            InteropMouse.EnqueueState(e.GetCurrentPoint((UIElement) sender).Position);
        }

        private void CanvasAnimatedControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            InteropMouse.EnqueueState(true);
        }

        private void CanvasAnimatedControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            InteropMouse.EnqueueState(false);
        }

        private void OnKeyDown(CoreWindow sender, KeyEventArgs e)
        {
            InteropKeyboard.Press(e.VirtualKey);
        }

        private void OnKeyUp(CoreWindow sender, KeyEventArgs e)
        {
            InteropKeyboard.Release(e.VirtualKey);
        }
    }
}
