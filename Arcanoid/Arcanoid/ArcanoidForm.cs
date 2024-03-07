using static Arcanoid.Measures.Converter;

namespace Arcanoid
{
    public partial class ArcanoidForm : Form
    {
        private const int Rows = 10;
        private const int Cols = 15;
        private Size blocksSpacing;
        private Size blockSize;
        private RelativeObject platform;
        private Ball ball;
        private BufferedGraphics buffer;
        private readonly Pen borderPen = Pens.White;
        private readonly Rectangle[,] blocks = new Rectangle[Rows, Cols];
        private Keys pressedKey;
        private bool isGameOver;

        public ArcanoidForm()
        {
            InitializeComponent();
            buffer = null!;
            platform = null!;
            ball = null!;
        }

        private void Redraw()
        {
            buffer.Graphics.Clear(BackColor);
            foreach (var block in blocks)
            {
                buffer.Graphics.DrawRectangle(borderPen, block);
            }
            buffer.Graphics.DrawRectangle(borderPen, platform.GetRectangle(Size));
            buffer.Graphics.DrawEllipse(borderPen, ball.GetRectangle(Size));
            if (isGameOver)
            {
                var onGameOverMessage = "Game over";
                var size = buffer.Graphics.MeasureString(onGameOverMessage, DefaultFont).ToSize();
                var location = new Point((DisplayRectangle.Width - size.Width) / 2, (DisplayRectangle.Height - size.Height) / 2);
                buffer.Graphics.DrawString(onGameOverMessage, DefaultFont, borderPen.Brush, location);
                buffer.Render();
            }
            buffer.Render();
        }

        private void MovePlatform(Keys key)
        {
            var platformSpeed = 0.009;
            if (key == Keys.Left && platform.GetPosition(Size).X - platformSpeed > 0)
            {
                platform.RelativeHorizontalPos -= platformSpeed;
            }
            else if (key == Keys.Right && platform.GetRectangle(Size).Right + platformSpeed < Size.Width)
            {
                platform.RelativeHorizontalPos += platformSpeed;
            }
        }

        private void InitBlocks()
        {
            CalculateElementsSize();
            var roundUncertainty = CalculateRoundUncertainty() / 2;
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    blocks[row, col] = new Rectangle(blockSize.Width * col + blocksSpacing.Width * (col + 1) + roundUncertainty,
                                                     blockSize.Height * row + blocksSpacing.Height * (row + 1),
                                                     blockSize.Width,
                                                     blockSize.Height);
                }
            }
            ResizeBlocks();
        }

        private void InitPlatform()
        {
            platform = new(0.5 - 0.15 / 2, 0.88, 0.15, 0.03);
        }

        private void InitBall()
        {
            ball = new Ball(0.012, platform.RelativeHorizontalPos, platform.RelativeVerticalPos - 0.1);
        }

        private void Init()
        {
            InitBlocks();
            InitPlatform();
            InitBall();
        }

        private void CalculateElementsSize()
        {
            var spacing = 0.01;
            blocksSpacing = new Size(PercentToPixels(spacing, DisplayRectangle.Width), PercentToPixels(spacing, DisplayRectangle.Height));
            var blockWidth = PercentToPixels((1.0 - spacing * (Cols + 2)) / blocks.GetLength(1), DisplayRectangle.Width); //Ўирина блоков с учЄтом двух отступов Ч слева и справа
            blockSize = new Size(blockWidth, PercentToPixels(0.02, DisplayRectangle.Height));
        }

        private void CheckColliders()
        {
            var ballRect = ball.GetRectangle(Size);
            isGameOver = ballRect.Bottom > DisplayRectangle.Bottom;
            if (isGameOver)
            {
                gameTimer.Stop();
                return;
            }
            var platformRect = platform.GetRectangle(Size);
            var horizontalColliding = ballRect.Right > DisplayRectangle.Right || ballRect.Left < DisplayRectangle.Left;
            var verticalColliding = ballRect.Top < DisplayRectangle.Top;
            var platformIntersect = Rectangle.Intersect(ballRect, platformRect);
            if (platformIntersect != Rectangle.Empty)
            {
                ball.RelativeVerticalPos = platform.RelativeVerticalPos - ball.RelativeHeight;
                if (platformIntersect.Width < platformIntersect.Height)
                {
                    horizontalColliding = true;
                }
                else
                {
                    verticalColliding = true;
                }
            }
            else
            {
                for (var row = 0; row < Rows; row++)
                {
                    for (var col = 0; col < Cols; col++)
                    {
                        var intersect = Rectangle.Intersect(ballRect, blocks[row, col]);
                        if (intersect == Rectangle.Empty)
                        {
                            continue;
                        }
                        blocks[row, col] = Rectangle.Empty;
                        if (intersect.Width > intersect.Height)
                        {
                            verticalColliding = true;
                        }
                        else
                        {
                            horizontalColliding = true;
                        }
                    }
                }
            }
            if (horizontalColliding)
            {
                ball.Speed = ball.Speed with { X = -ball.Speed.X };
            }
            if (verticalColliding)
            {
                ball.Speed = ball.Speed with { Y = -ball.Speed.Y };
            }
        }

        private void ResizeBlocks()
        {
            var xOffset = blockSize.Width + blocksSpacing.Width;
            var yOffset = blockSize.Height + blocksSpacing.Height;
            xOffset -= blockSize.Width + blocksSpacing.Width;
            yOffset -= blockSize.Height + blocksSpacing.Height;
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    if (blocks[row, col] != Rectangle.Empty)
                    {
                        blocks[row, col].Size = blockSize;
                    }
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

        private int CalculateRoundUncertainty()
        {
            return DisplayRectangle.Width - ((blockSize.Width + blocksSpacing.Width) * Cols + blocksSpacing.Width);
        }

        private void ArcanoidFormOnPaint(object _, PaintEventArgs __)
        {
            Redraw();
        }

        private void ArcanoidFormShown(object _, EventArgs __)
        {
            CalculateElementsSize();
            Init();
            buffer = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), DisplayRectangle);
            gameTimer.Start();
        }

        private void ArcanoidFormOnResize(object _, EventArgs __)
        {
            InitBlocks();
            if (DisplayRectangle.Width > 640 && DisplayRectangle.Height > 480)
            {
                buffer = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), DisplayRectangle);
            }
            Redraw();
        }

        private void ReducePlatformSize()
        {
            if (platform.RelativeWidth > 0.02)
            {
                var sizeReductionSpeed = 0.002;
                platform.RelativeWidth -= sizeReductionSpeed;
                platform.RelativeHorizontalPos += sizeReductionSpeed / 2;
            }
        }

        private void gameTimerTick(object _, EventArgs __)
        {
            MovePlatform(pressedKey);
            ReducePlatformSize();
            CheckColliders();
            ball.Move();
            Redraw();
        }

        private void ArcanoidForm_KeyDown(object _, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                pressedKey = e.KeyCode;
            }
        }

        private void ArcanoidForm_KeyUp(object sender, KeyEventArgs e)
        {
            pressedKey = Keys.None;
        }
    }
}
