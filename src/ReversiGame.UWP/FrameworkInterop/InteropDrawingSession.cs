using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Walterlv.ReversiGame.FrameworkInterop;
using InteropColor = Walterlv.ReversiGame.FrameworkInterop.Color;
using IDrawingSession = Walterlv.ReversiGame.FrameworkInterop.IDrawingSession;
using SpriteFont = Walterlv.ReversiGame.FrameworkInterop.SpriteFont;
using WindowsRuntimeSystemExtensions = System.WindowsRuntimeSystemExtensions;

namespace Walterlv.Gaming.Reversi.FrameworkInterop
{
    public class InteropDrawingSession : IDrawingSession
    {
        public CanvasDrawingSession Session { get; set; }
        public Game Game { get; set; }
        private Dictionary<string, CanvasBitmap> _bitmapDictionary = new Dictionary<string, CanvasBitmap>();

        public void DrawString(SpriteFont fpsFont, string fps, Vector2 point, InteropColor color)
        {
            Session?.DrawText(fps, point, color.ToColor());
        }

        public void Draw(Texture2D texture, Rectangle area, InteropColor color)
        {
            if (_bitmapDictionary.TryGetValue(texture.Path, out var bitmap))
            {
                Session?.DrawImage(bitmap, area.ToRect());
            }
        }

        public async Task CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            var tasks = Game.Contents
                .Where(x => !(x is SpriteFont))
                .Select(x =>
                    (x, CanvasBitmap.LoadAsync(sender.Device,
                        new Uri($@"ms-appx:///GameContent/{x.Path}.png")).AsTask()))
                .ToArray();
            await Task.WhenAll(tasks.Select(x => x.Item2)).ConfigureAwait(false);
            foreach (var task in tasks)
            {
                _bitmapDictionary[task.Item1.Path] = task.Item2.Result;
            }
        }
    }
}
