using static Arcanoid.Converter;

namespace Arcanoid
{
    public partial class ArcanoidForm : Form
    {
        private const int Rows = 20;
        private const int Cols = 20;
        private Size blocksSpacing;
        private Size platformSize;
        private Size blockSize;
        private BufferedGraphics buffer;
        private readonly Pen borderPen = Pens.White;
        private readonly Rectangle[,] blocks = new Rectangle[Rows, Cols];
        public ArcanoidForm()
        {
            InitializeComponent();
            buffer = null!;
        }

        private void Redraw()
        {
            buffer.Graphics.Clear(BackColor);
            foreach (var block in blocks)
            {
                buffer.Graphics.DrawRectangle(borderPen, block);
            }
            buffer.Render();
        }

        private void InitBlocks()
        {
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    blocks[row, col] = new Rectangle(blockSize.Width * col + blocksSpacing.Width * (col + 1),
                                                     blockSize.Height * row + blocksSpacing.Height * (row + 1),
                                                     blockSize.Width,
                                                     blockSize.Height);
                }
            }
        }

        private void CalculateElementsSize()
        {
            var spacing = 0.01;
            blocksSpacing = new Size(PercentToPixels(spacing, Size.Width), PercentToPixels(spacing, Size.Height));
            var blockWidth = PercentToPixels((1.0 - spacing * (Cols + 2)) / blocks.GetLength(0), Size.Width);
            platformSize = new Size(PercentToPixels(0.2, Size.Width), PercentToPixels(0.03, Size.Height));
            blockSize = new Size(blockWidth, PercentToPixels(0.02, Size.Height));
        }

        private void ResizeBlocks()
        {
            var xOffset = blockSize.Width + blocksSpacing.Width;
            var yOffset = blockSize.Height + blocksSpacing.Height;
            CalculateElementsSize();
            xOffset -= blockSize.Width + blocksSpacing.Width;
            yOffset -= blockSize.Height + blocksSpacing.Height;
            for (var row = 0; row < Rows; row++)
            {
            }
            for (var col = 0; col < Cols; col++)
            {
            }
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    blocks[row, col].Size = blockSize;
                }
            }
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 1; col < Cols; col++)
                {
                    blocks[row, col].Offset(-xOffset * col, 0);
                }
            }
            for (var row = 1; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    blocks[row, col].Offset(0, -yOffset * row);
                }
            }
        }

        private void ArcanoidFormOnPaint(object _, PaintEventArgs __)
        {
            Redraw();
        }

        private void ArcanoidFormShown(object _, EventArgs __)
        {
            CalculateElementsSize();
            InitBlocks();
            buffer = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), DisplayRectangle);
        }

        private void ArcanoidFormOnResize(object _, EventArgs __)
        {
            ResizeBlocks();
            buffer = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), DisplayRectangle);
            Redraw();
        }
    }
}
