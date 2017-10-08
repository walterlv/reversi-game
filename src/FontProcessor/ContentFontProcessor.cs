using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using System.IO;
using System.ComponentModel;

// TODO: replace these with the processor input and output types.
using TInput = System.String;
using TOutput = System.String;

namespace FontProcessor
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "FontProcessor.ContentFontProcessor")]
    public class ContentFontProcessor : FontDescriptionProcessor
    {
        public override SpriteFontContent Process(FontDescription input, ContentProcessorContext context)
        {
            string fullPath = Path.GetFullPath(GameFontTextFile);

            context.AddDependency(fullPath);

            string letters = File.ReadAllText(fullPath, System.Text.Encoding.UTF8);

            foreach (char c in letters)
            {
                input.Characters.Add(c);
            }

            return base.Process(input, context);
        }

        [DefaultValue("GameFontText.txt")]
        [DisplayName("Game Font Text")]
        [Description("此文件中的字符将会自动添加到字体当中.")]
        public string GameFontTextFile
        {
            get { return gameFontText; }
            set { gameFontText = value; }
        }
        private string gameFontText = @"..\ReversiGame\GameFontText.txt";
    }
}