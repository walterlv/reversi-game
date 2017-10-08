using Windows.Foundation;
using Windows.UI.Xaml.Controls;
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
        }

        private Game _game;
        private InteropDrawingSession _interopDrawing;

        private void CanvasAnimatedControl_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            _interopDrawing = new InteropDrawingSession();
            _game = DrawableGameComponent.CreateGame<ReversiXNAGame.ReversiXNAGame>(_interopDrawing);
            _game.Initialize();
            _game.LoadContent();
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
                }
                finally
                {
                    _interopDrawing.Session = null;
                }
            }
        }
    }
}
