using static Arcanoid.Measures.Converter;

namespace Arcanoid
{
    public partial class ArcanoidForm : Form
    {
        private const int Rows = 10;
        private const int Cols = 15;
        private double blocksSpacing;
        private (double Width, double Height) blockSize;
        private Size blockRectSize;
        private RelativeObject platform;
        private Ball ball;
        private BufferedGraphics buffer;
        private readonly Pen borderPen = Pens.White;
        private readonly Block[,] blocks = new Block[Rows, Cols];
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
                if (!block.IsBroken)
                {
                    buffer.Graphics.DrawRectangle(borderPen, new Rectangle(block.GetPosition(Size), blockRectSize));
                }
            }
            buffer.Graphics.DrawRectangle(borderPen, platform.GetRectangle(Size));
            buffer.Graphics.DrawEllipse(borderPen, ball.GetRectangle(Size));
            if (isGameOver)
            {
                var onGameOverMessage = "Game over";
                var stringSize = buffer.Graphics.MeasureString(onGameOverMessage, DefaultFont).ToSize();
                var location = new Point((DisplayRectangle.Width - stringSize.Width) / 2, (DisplayRectangle.Height - stringSize.Height) / 2);
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

        private void RelocateBlocks()
        {
            CalculateElementsSize();
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    blocks[row, col].RelativeHorizontalPos = blockSize.Width * col + blocksSpacing * (col + 1);
                    blocks[row, col].RelativeVerticalPos = blockSize.Height * row + blocksSpacing * (row + 1);
                    blocks[row, col].RelativeWidth = blockSize.Width;
                    blocks[row, col].RelativeHeight = blockSize.Height;
                }
            }
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
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    blocks[row, col] = new Block(0, 0, 0, 0);
                }
            }
            InitPlatform();
            InitBall();
            RelocateBlocks();
        }

        private void CalculateElementsSize()
        {
            blocksSpacing = 0.015;
            var blockWidth = (1.0 - blocksSpacing * (Cols + 2)) / Cols; //Ўирина блоков с учЄтом двух отступов Ч слева и справа
            blockSize = (blockWidth, 0.02);
            blockRectSize = new(PercentToPixels(blockWidth, DisplayRectangle.Width), PercentToPixels(0.02, DisplayRectangle.Height));
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
                        if (blocks[row, col].IsBroken)
                        {
                            continue;
                        }
                        var intersect = Rectangle.Intersect(ballRect, blocks[row, col].GetRectangle(Size));
                        if (intersect == Rectangle.Empty)
                        {
                            continue;
                        }
                        blocks[row, col].IsBroken = true;
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

        private void ArcanoidFormOnPaint(object _, PaintEventArgs __)
        {
            Redraw();
        }

        private void ArcanoidFormShown(object _, EventArgs __)
        {
            Init();
            buffer = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), DisplayRectangle);
            gameTimer.Start();
        }

        private void ArcanoidFormOnResize(object _, EventArgs __)
        {
            RelocateBlocks();
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
                var sizeReductionSpeed = 0.00002;
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
