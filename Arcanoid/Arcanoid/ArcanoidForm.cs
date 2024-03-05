using static Arcanoid.Converter;

namespace Arcanoid
{
    public partial class ArcanoidForm : Form
    {
        private const int Rows = 10;
        private const int Cols = 15;
        private const double PlatformSpeed = 0.015;
        private Size blocksSpacing;
        private Size blockSize;
        private Platform platform;
        private BufferedGraphics buffer;
        private readonly Pen borderPen = Pens.White;
        private readonly Rectangle[,] blocks = new Rectangle[Rows, Cols];
        private Keys pressedKey;

        public ArcanoidForm()
        {
            InitializeComponent();
            buffer = null!;
            platform = null!;
        }

        private void Redraw()
        {
            buffer.Graphics.Clear(BackColor);
            foreach (var block in blocks)
            {
                buffer.Graphics.DrawRectangle(borderPen, block);
            }
            buffer.Graphics.DrawRectangle(borderPen, platform.GetRectangle(Size));
            buffer.Render();
        }

        private void MovePlatform(Keys key)
        {
            if (key == Keys.Left && platform.GetPosition(Size).X - PlatformSpeed > 0)
            {
                platform.RelativeHorizontalPos -= PlatformSpeed;
            }
            else if (key == Keys.Right && platform.GetRectangle(Size).Right + PlatformSpeed < Size.Width)
            {
                platform.RelativeHorizontalPos += PlatformSpeed;
            }
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

        private void InitPlatform()
        {
            var platformSize = new Size(PercentToPixels(0.15, DisplayRectangle.Width), PercentToPixels(0.03, DisplayRectangle.Height));
            var platformLocation = new Point(DisplayRectangle.Width / 2 - platformSize.Width / 2, DisplayRectangle.Height - PercentToPixels(0.12, DisplayRectangle.Height));
            platform = new Platform(new Rectangle(platformLocation, platformSize), Size);
        }

        private void CalculateElementsSize()
        {
            var spacing = 0.01;
            blocksSpacing = new Size(PercentToPixels(spacing, DisplayRectangle.Width), PercentToPixels(spacing, DisplayRectangle.Height));
            var blockWidth = PercentToPixels((1.0 - spacing * (Cols + 2)) / blocks.GetLength(1), DisplayRectangle.Width); //Ўирина блоков с учЄтом двух отступов Ч слева и справа
            blockSize = new Size(blockWidth, PercentToPixels(0.02, DisplayRectangle.Height));
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
            InitPlatform();
            buffer = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), DisplayRectangle);
        }

        private void ArcanoidFormOnResize(object _, EventArgs __)
        {
            ResizeBlocks();
            if (DisplayRectangle.Width > 10 && DisplayRectangle.Height > 10)
            {
                buffer = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), DisplayRectangle);
            }
            Redraw();
        }

        private void moveDelayTick(object _, EventArgs __)
        {
            MovePlatform(pressedKey);
            Redraw();
        }

        private void ArcanoidForm_KeyDown(object _, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                pressedKey = e.KeyCode;
                moveDelay.Start();
            }
        }

        private void ArcanoidForm_KeyUp(object sender, KeyEventArgs e)
        {
            moveDelay.Stop();
            pressedKey = Keys.None;
        }
    }
}
