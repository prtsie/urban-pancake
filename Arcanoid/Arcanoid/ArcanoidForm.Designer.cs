namespace Arcanoid
{
    partial class ArcanoidForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            gameTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // gameTimer
            // 
            gameTimer.Interval = 30;
            gameTimer.Tick += gameTimerTick;
            // 
            // ArcanoidForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(931, 520);
            MinimumSize = new Size(640, 480);
            Name = "ArcanoidForm";
            Text = "Arcanoid";
            Shown += ArcanoidFormShown;
            Paint += ArcanoidFormOnPaint;
            KeyDown += ArcanoidForm_KeyDown;
            KeyUp += ArcanoidForm_KeyUp;
            Resize += ArcanoidFormOnResize;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer gameTimer;
    }
}
