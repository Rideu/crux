﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using WinForms = System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Crux.dControls;

using static System.Math;

namespace Crux
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Core : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        WinForms.Form GameForm;
        public static Point WinSize;
        public static GameWindow PrimaryWindow;

        public static ToolSet ts;
        public Core()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.GraphicsProfile = GraphicsProfile.Reach;
            WinSize = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Content.RootDirectory = "Content";
            Window.TextInput += Window_TextInput;
            GameForm = (WinForms.Form)WinForms.Form.FromHandle(Window.Handle);
            var scr = WinForms.Screen.PrimaryScreen.WorkingArea.Size;
            Window.Position = new Point(scr.Width / 2 - graphics.PreferredBackBufferWidth / 2, scr.Height / 2 - graphics.PreferredBackBufferHeight / 2);
            PrimaryWindow = Window;

            ts = new ToolSet();
            ts.Show();
        }


        private void Window_TextInput(object sender, TextInputEventArgs e)
        {

        }



        protected override void Initialize()
        {
            base.Initialize();
            IsMouseVisible = true;
            Form f = new Form(30, 100, 290, 500, new Color(70, 70, 70))
            {
                IsResizable = true,
                
            };
            uControl mc, ml, mp;
            f.AddNewControl(mc = new Button(20, 20, 70, 20, new Color(100, 100, 100))
            {
                Text = "Button1"
            });
            f.AddNewControl(new Button(30, 30, 80, 70, new Color(100, 100, 100))
            {
                Text = "Button2"
            });
            f.AddNewControl(ml = new Label(20, 50, 260, 280)
            {
                Font = font1
            });
            
            f.AddNewControl(new Textbox(20, 350, 100, 20)
            {
                Font = font1
            });
            f.AddNewControl(mp = new Panel(20, 380, 260, 80)
            {

            });
            (ml as Label).Text = @"How to reference (Proto)
^n ^nHow to... {#(0,204,255)}Warp :
^n ^n1. Warp {#(0,204,255)}tech is commonly used to travel between {#(0,204,255)}star {#(0,204,255)}systems , but for certain amount of energy or specific fuel to feed your warp core. Press Galaxy Map button (M by default) to view available stars to travel to. The sphere around your current star system shows the bounds within which you can warp.  Now click on any star. The number below star name shows, how much fuel is required to warp to this system. It's labeled as green if you have enough amount of energy and red otherwise. Now choose a reachable star to travel to and press Travel button. The Oscillation window opens. To increase travel stability and speed, you need to alter nodes of the oscillation graph according to the warp noise map: the more accuracy, the more effectivity. Since nodes values are initially precalculated, they also can be left as is, so the travel will take its usual time. Now press Apply button to launch the warp core and travel to the chosen system. Warp can take some time, depending on distance to target star and warp core properties.
^n ^n2. You also can initiate a wave overlap with the ship that has slower warp core, allowing you to stick with other ships during the travel. When this is possible, an notice appears, which displays current distance to the ship and possibility to do this maneuver: it uses significant amount of energy depending on initial warp jump point. 
^n ^nHow to... Build:
^n ^n1. Buildings are primary things that makes the world live, cycle and expand. They are subdivided by their functionality: common factories, research laboratories and energy stations. All of them are consuming various resources, depending on how it is organized and supplied. To manage its work in more simple manner, node mechanic is used. Each node requires specific amount of workers and energy to function. There are three types of nodes in the game: source, processing and storage. Source nodes are consuming local resources depending on its type (mining or farming). Processing nodes are used to process incoming resources and provide the result to the next ones. Storage nodes sends all the incoming resources to the planetary storage to be distributed among other factories or for local sales or intake resources for continued processing. If there is a lack of workers or energy, the production will be limited or, in worst case, disabled, so dependency compliance and optimization are very important. If node's inner storage is overfilled, it can cause blocking state - incoming connections are filling up, keep consuming energy and spending working time, calling continued blocking chain, so the losses are increasing.
^n ^nBuilding sizes can be four types: small, large, complex or arcological. Small ones can contain up to 5 nodes plus one for storage, large can contain up to 20, complex up to 70 and arcological up to 160 nodes.
^n ^n2. The common factories can be built on wide range of surfaces, even on non-atmosphere planets or asteroids. The size is varied by small (up to 6 processing nodes) They need abundant amount of workers and energy.
";
            (mc as Button).OnLeftClick += delegate { MessageBox.Show("Clock!"); };
            mp.AddNewControl(new Button(10, 10, 60, 20, new Color(140, 140, 140)) { Text = "Button3", });

            mp.AddNewControl(ml = new Label(10, 40, 60, 20)
            { Font = font1, });
            ml.Text = "Label2";

            //mp.AddNewControl(ml = new Textbox(10, 70, 60, 20)
            //{ Font = font1, });
            //ml.Text = "Textbox2";

            FormManager.AddForm("MainForm", f);
            f = new Form(340, 100, 210, 250, new Color(40, 40, 40))
            {

            };

            f.IsVisible = false;
            Label lb;

            Button b1, b2, b3, b4, b5, b6, b7, b8, b9, b0,
                bex, bdiv, bmul, bsub, bsum, 
                er;


            f.AddNewControl(
                lb = new Label(10, 10, 140, 30) { Font = font1 },
                er = new Button(160, 10, 40, 30) { Text = "<=" },
                b1 = new Button(10, 50, 40, 40) { Text = "1" },
                b2 = new Button(60, 50, 40, 40) { Text = "2" },
                b3 = new Button(110, 50, 40, 40) { Text = "3" },
                b4 = new Button(10, 100, 40, 40) { Text = "4" },
                b5 = new Button(60, 100, 40, 40) { Text = "5" },
                b6 = new Button(110, 100, 40, 40) { Text = "6" },
                b7 = new Button(10, 150, 40, 40) { Text = "7" },
                b8 = new Button(60, 150, 40, 40) { Text = "8" },
                b9 = new Button(110, 150, 40, 40) { Text = "9" },
                b0 = new Button(10, 200, 90, 40) { Text = "0" },
                bex = new Button(110, 200, 40, 40) { Text = "=" },
                bdiv = new Button(160, 50, 40, 40) { Text = "/" },
                bmul = new Button(160, 100, 40, 40) { Text = "*" },
                bsub = new Button(160, 150, 40, 40) { Text = "-" },
                bsum = new Button(160, 200, 40, 40) { Text = "+" }
                );

            lb.Text = "0";

            List<Button> digs = new List<Button>() { b1, b2, b3, b4, b5, b6, b7, b8, b9, b0 };
            List<Button> acts = new List<Button>() { bdiv, bmul, bsub, bsum };
            String v1 = "";

            Func<string> div = delegate { return (int.Parse(v1) / int.Parse(lb.Text)).ToString(); };
            Func<string> mul = delegate { return (int.Parse(v1) * int.Parse(lb.Text)).ToString(); };
            Func<string> sub = delegate { return (int.Parse(v1) - int.Parse(lb.Text)).ToString(); };
            Func<string> sum = delegate { return (int.Parse(v1) + int.Parse(lb.Text)).ToString(); };

            Func<string> sel = null;

            digs.ForEach(n => n.OnLeftClick += delegate
            {
                if (lb.Text == "0") lb.Text = n.Text;
                else lb.Append = n.Text;
            });
            
            acts.ForEach(n => n.OnLeftClick += delegate
            {
                if (sel == null && lb.Text.Length > 0)
                {
                    sel = n.Text == "/" ? div : n.Text == "*" ? mul : n.Text == "-" ? sub : sum;
                    v1 = lb.Text;
                    lb.Text = "0";
                }
            });

            bex.OnLeftClick += delegate
            {
                if (sel != null)
                {
                    lb.Text = sel();
                    sel = null;
                }
            };

            er.OnLeftClick += delegate
            {
                lb.Text = lb.Text.Length == 1 ? "0" : lb.Text.Substring(0, lb.Text.Length - 1);
            };

            FormManager.AddForm("CalcForm", f);
        }

        // Textures
        public static Texture2D pixel;

        // Fonts
        public static SpriteFont font, font1;

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Fonts

            font = Content.Load<SpriteFont>("fonts\\arial");
            font1 = Content.Load<SpriteFont>("fonts\\Xolonium");

            #endregion

            pixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });

            doc.innerHTML(htdoc);

            //sb = new TextBuilder(font, "{$=>(#(255,200,123)):p}MonoGame is free software used by game developers to make their {blue:h} Windows{=>$} and Windows Phone games run on other systems.", new Vector2(100), new Vector2(300, 400));

        }

        #region Service Globals

        public static u_ps GlobalMousePos = new u_ps(0, 0);
        public static MouseState MS = new MouseState();

        #endregion
        

        public static TextBuilder pssb;
        string htdoc =
@"
<body>
    <div style='margin:20px'>
        <span>hey</span>
    </div>
</body>        
";
        HTML doc = new HTML();
        public static Rectangle GlobalDrawingBounds, LocalDrawingBounds;
        public static Vector2 CenteratorDevice;
        public static Viewport PrimaryViewport;
        public static GameTime gt;

        protected override void Update(GameTime gameTime)
        {
            MS = Mouse.GetState();
            GlobalMousePos.Pos = MS.Position.ToVector2();
            Control.Update();

            MessageBox.Update();

            FormManager.Update();
            

            base.Update(gt = gameTime);
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }
        //TextBuilder sb;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(100, 100, 100));
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
            {
                //spriteBatch.DrawRect(new Rectangle(new Vector2(100-5).ToPoint(), new Vector2(300, 400).ToPoint()), Color.Gray);
                //sb.Render(spriteBatch);

                //pssb = new StringBuilder(font, ts.textBox3.Text, new Vector2(500-5, 100-5), new Vector2(300, 400));
                //pssb.Render(spriteBatch);

                //spriteBatch.Draw(font.Texture, new Vector2(300), Color.White); 
            }
            spriteBatch.End();
            MessageBox.Draw();
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            FormManager.Draw();
            graphics.GraphicsProfile = GraphicsProfile.Reach;
            base.Draw(gameTime);
        }
    }
}