using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Timers;
using Timer = System.Timers.Timer;

namespace GameJam.Game
{
    public class GameRenderer : IDisposable
    {
        private readonly GameContext context;
        private float frametime;
        private readonly Image image;
        private readonly Font Font;
        private readonly Brush brush;
        private float _countDown = 15f;

        public int goldAmount = 0;
        private string goldAmountText = "###";

        public GameRenderer(GameContext context)
        {
            this.context = context;

            Font = new Font(FontFamily.GenericSansSerif, 6);
            brush = new SolidBrush(Color.White);
            image = Bitmap.FromFile("sprites.png");

        }
        private Graphics InitGraphics(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //make nice pixels
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;


            g.Transform = new Matrix();
            float roomw = context.room.tiles[0].Length * context.tileSize;
            float roomh = context.room.tiles.Length * context.tileSize;
            roomw /= 2;
            roomh /= 2;
            roomw *= context.scaleunit;
            roomh *= context.scaleunit;
            float px = context.player.rectangle.X* context.scaleunit;
            float py = context.player.rectangle.Y * context.scaleunit;

            float screenscale = GetWindowsScaling();
            e.Graphics.TranslateTransform(-px+roomw, -py+roomh);
          g.ScaleTransform(context.scaleunit, context.scaleunit);

            //there will be some tearing between tiles, a solution to that is to render to a bitmap then draw that bitmap, fun challenge?
            g.Clear(Color.Black);
            return g;
        }
        internal void Render(PaintEventArgs e, float frametime)
        {
            this.frametime = frametime;

            _countDown -= frametime;

            Graphics g = InitGraphics(e);

            float roomw = context.room.tiles[0].Length * context.tileSize;
            float roomh = context.room.tiles.Length * context.tileSize;
            

            RenderRoom(g);

            RenderObject(g, context.player);
            
            float aTimer = _countDown;
            int Timer = (int)Math.Round(aTimer);

            if (Timer <= 0) Application.Restart();

            g.Transform = new Matrix();
            g.ScaleTransform(context.scaleunit, context.scaleunit);

            GoldAmountText = GoldAmount.ToString();
            g.DrawString("Gold: " + GoldAmountText, Font, brush, 1,12);
            g.DrawString("Collect as much gold as posible in " + Timer.ToString() + " seconds", Font, brush, 1, 1);
        }

        private void RenderRoom(Graphics g)
        {
            foreach (Tile[] row in context.room.tiles)
            {
                foreach (Tile t in row)
                {
                    g.DrawImage(image, t.rectangle, t.sprite, GraphicsUnit.Pixel);
                }
            }
        }

        private void RenderObject(Graphics g, RenderObject renderObject)
        {
            g.DrawImage(image, renderObject.rectangle, renderObject.frames[(int)renderObject.frame], GraphicsUnit.Pixel);
            renderObject.MoveFrame(frametime);
        }

        public void Dispose()
        {
            image.Dispose();
        }
        public static int GetWindowsScaling()

        {

            return (int)(100 * Screen.PrimaryScreen.WorkingArea.Width/Screen.PrimaryScreen.Bounds.Width );

        }


    }

}


