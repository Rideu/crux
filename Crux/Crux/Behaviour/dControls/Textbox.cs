﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Crux.Simplex;
using static Crux.Game1;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux.dControls
{
    public class Textbox : uControl // Unused
    {
        #region Fields
        private uControl OwnerField;
        public override uControl Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set { align = value; Translate(); } get => align; }

        public override string Text { get { return text.Text; } set { text.UpdateText(value); } }
        SpriteFont font = Game1.font;
        public SpriteFont Font { get => font; set => font = value; }
        new TextBuilder text;
        bool InputMode;

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;

        #endregion

        public Textbox(Vector4 posform)
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public Textbox(Vector2 pos, Vector2 size)
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
        }

        public Textbox(float x, float y, float width, float height)
        {
            X = x; Y = y; Width = width; Height = height;
        }

        internal override void Initialize()
        {
            ID = Owner.GetControlsNum + 1;
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            // Assemble control texture here.
            Tex = new Texture2D(Owner.Batch.GraphicsDevice, (int)Width, (int)Height);
            var layer1 = new Color[(int)Width * (int)Height];
            for (int i = 0; i < layer1.Length; i++)
                if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                    layer1[i] = Color.Black;
                else layer1[i] = new Color(15, 15, 15, 111);
            Tex.SetData(layer1);
            OnMouseLeave += delegate { Invalidate(); };

            text = new TextBuilder(Font, "[N U L L T E X T]", new Vector2(X /*+ (padding.X - scroll.Width)*/, Y), new Vector2(-1 /*- scroll.Width - padding.Width*/, Height), Color.White, false/*, this*/);

            t = new Timer(1000);
            t.OnFinish += delegate { t.Reset(false); t.Start(); };
            PrimaryWindow.TextInput += delegate (object sender, TextInputEventArgs e) // PERF: move to static constructor and apply input only for active control
            {
                if (this.InputMode)
                {
                    var t = text.Text;
                    if (e.Character == 8) // Backspace
                    {
                        if (t.Length > 0)
                        {
                            if (caretpos > 0)
                                t = t.Remove(caretpos - 1, 1);
                            caretpos -= caretpos > 0 ? 1 : 0;
                        }
                    }
                    else
                    if (e.Character == 127)
                    {
                        var ls = t.LastIndexOf(' ');
                        if (t.Length > 0)
                            t = t.Remove(caretpos = ls < 0 ? 0 : ls);
                    }
                    else
                    {
                        //var c = (int)(e.Character);
                        //var v = font.Glyphs[5];
                        //if (font.Glyphs.Any(n => n.ToString()[0] == c))
                        t = t.Insert(caretpos++, e.Character + "");
                        //caretpos += caretpos + 1 == t.Length ? 0 : 1;
                    }
                    text.UpdateText(t);
                    //caretpos = text.CleanText.Length == 0 ? 0 : caretpos;
                }
            };
            base.Initialize();
        }
        Timer t;
        public override void Invalidate()
        {
            InputMode = InputMode && Control.MouseHoverOverG(Bounds);
            t.Stop();
            foreach (var c in Controls)
            {
                c.Update();
            }
        }

        public override void Update()
        {
            UpdateBounds();

            IsHovering = !true;
            if (Bounds.Contains(Game1.MS.Position.ToVector2()))
                IsHovering = true;
            if (IsHovering && Control.LeftClick())
            {
                InputMode = true;
                t.Reset(false);
                t.Start();
            }
            if (InputMode)
            {
                if (Control.PressedKey(Keys.Left))
                    caretpos -= caretpos > 0 ? 1 : 0;
                if (Control.PressedKey(Keys.Right))
                    caretpos += /*text.Length > 0 && */ caretpos < text.Text.Length ? 1 : 0;
                t.Update((float)gt.ElapsedGameTime.TotalMilliseconds);
            }
        }

        public override void InnerUpdate()
        {
            InputMode = InputMode && Control.MouseHoverOverG(Bounds);
            base.EventProcessor();
            OnUpdate?.Invoke();
        }

        private void Translate()
        {

        }

        int caretpos = 0;

        Vector2 AbsolutePosition => new Vector2(Owner.X + X, Owner.Y + Y);
        float ease(float t)
        {
            return -t * (t - 1) * 4;
        }
        public override void Draw()
        {
            Batch.GraphicsDevice.ScissorRectangle = DrawingBounds;
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                Batch.Draw(Tex, Bounds, InputMode ? Color.White : new Color(255, 255, 255, 200));
            }
            Batch.End();
            Batch.GraphicsDevice.ScissorRectangle = Batch.GraphicsDevice.ScissorRectangle.InflateBy(-1);
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                Vector2 cs = new Vector2();
                Vector2 ts = font.MeasureString(text.Text);
                if (InputMode)
                {
                    var sub = text.Text.Substring(0, caretpos);
                    var sp = text.Space;
                    var cc = sub.Count(n => n == ' ');
                    var rep = sub.Replace(" ", "");
                    var mea = font.MeasureString(rep);
                    cs = mea + new Vector2(cc * (sp.X)/* + font.Spacing * sub.Length*/, 0);
                    var b = caretpos == text.Text.Length;
                }
                var offset = (cs.X > Width / 2 ? Width / 2 - cs.X /*+ (ts.X - cs.X < Width / 2? ts.X - cs.X : 0)*/ /*(caretpos == text.Text.Length ? Width / 2  : 0)*/ : 0);

                Line cline = new Line(
                    (AbsolutePosition + new Vector2(1 + cs.X + offset, 2)).ToPoint().ToVector2(),
                    (AbsolutePosition + new Vector2(1 + cs.X + offset, -2 + Bounds.Size.Y)).ToPoint().ToVector2());
                text.Render(Batch, new Vector2(Owner.X + offset, Owner.Y + 1)/* + textpos*/);

                if (InputMode)
                    Batch.DrawLine(cline, new Color(255, 255, 255, 255) * ease(t));

                //Batch.DrawString(font, text, new Vector2(Owner.X + X, Owner.Y + Y) + new Vector2(4, 2), Color.White, 0f, new Vector2(), 0.98f, SpriteEffects.None, 1f);
            }
            Batch.End();
        }
    }
}