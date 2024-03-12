using static Arcanoid.Measures.Converter;

namespace Arcanoid
{
    public partial class ArcanoidForm : Form
    {
        private const int Rows = 10;
        private const int Cols = 15;
        private const double PlatformSpeed = 0.006;
        private const double MinPlatformSize = 0.03;
        private const double MinBallAngle = Math.PI / 6;
        private const double MaxBallAngle = MinBallAngle + Math.PI / 2;
        private const double RotateAngle = Math.PI / 6;
        private double blocksSpacing;
        private (double Width, double Height) blockSize;
        private Size blockRectSize;
        private RelativeObject platform;
        private Ball ball;
        private BufferedGraphics buffer;
        private readonly Brush fillBrush = Brushes.White;
        private readonly Brush leftSideOfPlatformBrush = Brushes.Blue;
        private readonly Brush rightSideOfPlatformBrush = Brushes.Green;
        private readonly Block[,] blocks = new Block[Rows, Cols];
        private Keys pressedKey;
        private bool isGameOver;

        //TODO: цвета блоков

        public ArcanoidForm()
        {
            InitializeComponent();
            buffer = null!;
            platform = null!;
            ball = null!;
        }

        private static Point CenterOfRect(Rectangle rect)
        {
            return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }

        private void Redraw()
        {
            buffer.Graphics.Clear(BackColor);
            foreach (var block in blocks)
            {
                if (!block.IsBroken)
                {
                    buffer.Graphics.FillRectangle(fillBrush, new Rectangle(block.GetPosition(Size), blockRectSize));
                }
            }
            var platformRect = platform.GetRectangle(Size);
            var sideWidth = PercentToPixels(MinPlatformSize, Size.Width);
            var leftSide = new Rectangle(platformRect.Left, platformRect.Top, sideWidth, platformRect.Height);
            var rightSide = new Rectangle(platformRect.Right - sideWidth, platformRect.Top, sideWidth, platformRect.Height);
            buffer.Graphics.FillRectangle(fillBrush, platformRect);
            buffer.Graphics.FillRectangle(leftSideOfPlatformBrush, leftSide);
            buffer.Graphics.FillRectangle(rightSideOfPlatformBrush, rightSide);
            buffer.Graphics.FillEllipse(fillBrush, ball.GetRectangle(Size));
            if (isGameOver)
            {
                var onGameOverMessage = "Game over";
                var stringSize = buffer.Graphics.MeasureString(onGameOverMessage, DefaultFont).ToSize();
                var location = new Point((DisplayRectangle.Width - stringSize.Width) / 2, (DisplayRectangle.Height - stringSize.Height) / 2);
                buffer.Graphics.DrawString(onGameOverMessage, DefaultFont, fillBrush, location);
                buffer.Render();
            }
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
            ball = new Ball(0.023, platform.RelativeHorizontalPos, platform.RelativeVerticalPos - 0.1);
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

        private void CalculateCollides()
        {
            var ballRect = ball.GetRectangle(Size);
            isGameOver = ballRect.Bottom > DisplayRectangle.Bottom;
            if (isGameOver)
            {
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
                var centerOfBall = CenterOfRect(ballRect);

                //TODO: починить

                if (centerOfBall.X < platformRect.Left + MinPlatformSize / 2)
                {
                    var maxAngle = MaxBallAngle - ball.Speed.Angle;
                    ball.Speed = ball.Speed.Rotate(RotateAngle > maxAngle ? -maxAngle : -RotateAngle);
                }
                else if (centerOfBall.X > platformRect.Right - MinPlatformSize / 2)
                {
                    var maxAngle = ball.Speed.Angle - MinBallAngle;
                    ball.Speed = ball.Speed.Rotate(RotateAngle > maxAngle ? maxAngle : RotateAngle);
                }
            }
            else
            {
                foreach (var block in blocks)
                {
                    if (block.IsBroken)
                    {
                        continue;
                    }
                    var blockRect = block.GetRectangle(Size);
                    var ballCenter = CenterOfRect(ballRect);
                    var closestPoint = new Point(Math.Max(blockRect.Left, Math.Min(ballCenter.X, blockRect.Right)),
                                                 Math.Max(blockRect.Top, Math.Min(ballCenter.Y, blockRect.Bottom)));
                    if (Math.Pow(closestPoint.X - ballCenter.X, 2) + Math.Pow(closestPoint.Y - ballCenter.Y, 2) > Math.Pow(ballRect.Height / 2, 2))
                    {
                        continue;
                    }
                    block.IsBroken = true;
                    horizontalColliding = (closestPoint.X == blockRect.Left && ball.Speed.X > 0)
                                       || (closestPoint.X == blockRect.Right && ball.Speed.X < 0);
                    verticalColliding = (closestPoint.Y == blockRect.Top && ball.Speed.Y > 0)
                                       || (closestPoint.Y == blockRect.Bottom && ball.Speed.Y < 0);
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
            foreach (var block in blocks)
            {
                if (!block.IsBroken)
                {
                    return;
                }
            }
            isGameOver = true;
        }

        private void ReducePlatformSize()
        {
            if (platform.RelativeWidth > MinPlatformSize)
            {
                var sizeReductionSpeed = 0.00001;
                platform.RelativeWidth -= sizeReductionSpeed;
                platform.RelativeHorizontalPos += sizeReductionSpeed / 2;
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

        private void gameTimerTick(object _, EventArgs __)
        {
            MovePlatform(pressedKey);
            ReducePlatformSize();
            CalculateCollides();
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

        private void ArcanoidForm_KeyUp(object _, KeyEventArgs __)
        {
            pressedKey = Keys.None;
        }
    }
}
