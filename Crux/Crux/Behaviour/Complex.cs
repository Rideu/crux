﻿using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Text;

using static System.Text.RegularExpressions.Regex;
using static System.Math;
using static Crux.Game1;

// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION

// NOTE: The following listing contains code which is too complex for perception. It has done of personal preferences. 
//       I'll reduce the maintainability index soon when it gets eligible for directional usage.

namespace Crux
{
    /// <summary>
    /// Represents complex text formatter and renderer.
    /// </summary>
    public class TextBuilder
    {
        SpriteFont f; public SpriteFont Font => f;

        string t; public string Text { get => t; set /*enwrite*/ => t = value; }
        string ct; public string CleanText { get => ct; }
        int len; public int Length => len;

        Vector2 sp; public Vector2 ScrollPosition { get => sp; set /*enwrite*/ => sp = value; }

        Vector2 p; public Vector2 Position { get => p; set /*enwrite*/ => p = value; }

        Vector2 spc; public Vector2 Space => spc;

        Color col; public Color Color => col;

        Vector2 s; public Vector2 GetInitialSize => s;
        Vector2 ts;
        public Vector2 GetTotalSize => ts;
        Label owner; // TODO: to uControl
        bool af;

        public TextBuilder(SpriteFont font, string text, Vector2 pos, Vector2 size, Color color = default, bool applyformat = true, Label label = null)
        {
            //gc = "";
            af = applyformat;
            f = font;
            p = pos;
            s = size;
            col = color;
            owner = label;
            UpdateText(text);
        }

        public void UpdateText(string text)
        {
            t = Replace(text, @"[}]\s", "}");
            t = Replace(t, @"[{]", " {");
            t = Replace(t, @"\s+ (?!\n)", " ").Trim(' '); // Filter input to necessary view so commands can recgonize it properly
            Vector2 cp = new Vector2();
            //f.Spacing = .65f;
            var sp = spc = f.MeasureString(" ") + new Vector2(3, 0);
            var l = 0;
            var c = t.Split(' ');
            ct = Replace(t = text, "{.+?}", "");
            w.Clear();
            sub sb = null;
            foreach (var n in c)
            {
                var ws = f.MeasureString(n);
                len += n.Length;
                var rt = n;
                var tsl = ws.X;// PERF: avoid FindAll with nulling tsl on new line and ++ it on each n iteration
                w.FindAll(u => u.l == l).ForEach(u => { tsl += (int)u.b.Width + sp.X; });
                if (s.X > 0 ? (rt.Contains("\n") || tsl + 2 > s.X) : false)
                {
                    rt = n.Replace("\n", "");
                    // Move words that are newly filtered to left and one line lower
                    // {
                    cp.X = 0;
                    cp.Y += ws.Y;
                    l += 1; // }
                }
                sb = new sub(p + cp, f, rt, col, ws.X, ws.Y, l);
                if(af)
                F_C_APPLY(sb);
                ws = f.MeasureString(sb);
                sb.nw[0] = w.Count > 1 ? w[w.Count - 1] : null;
                if (w.Count > 1)
                    w[w.Count - 1].nw[1] = sb;
                //Match pm; // Proto-code, used to separate punctuation marks
                //if ((pm = Match(rt, "[,.!@#$%^&*()]")).Success)
                //{
                //    var mw = f.MeasureString(pm.Value);
                //    w.Add(new sub(pos + cp + ws /*extra bugged on .ws*/, f, pm.Value, Color.Black, mw.X, mw.Y, l));
                //    cp += new Vector2(mw.X, 0);
                //}
                cp += new Vector2(ws.X + sp.X, 0);
                w.Add(new sub((p + cp) - new Vector2(spc.X, 0), f, " ", col, spc.X, sp.Y, l));
                w.Add(sb);
            }
            ts = new Vector2(s.X, sb.b.Y);
        }

        public void Update()
        {
            foreach (var s in w) //TODO: w update
            {
                s.fc = owner != null ? owner.IsActive : true;
                s.upd(sp);
            }
        }

        void F_C_APPLY(sub s)
        {
            while (IsMatch(s.t, "{.+?}")) // Keep processing commands until they gone...
            {
                var pc = rule.al(s.t); // Parse command
                if (pc.ct != null)
                {
                    var sp = f.MeasureString("  ").X;
                    s.t = Replace(s.t, ".+?}+", "");
                    // Set word's bounds width. Example: ":h" directive won't work properly if mouse hovers over this word
                    s.b.Width = (int)f.MeasureString(s.t).X;
                    //s = pc.p(s, pc.ct); // Apply command processor
                    if (pc.ish)
                    {
                        s.hov = pc;
                    }
                    else
                    {
                        s.hov = s.def = pc; s = pc.aplog(s, pc.val);
                    };
                }
                if (IsMatch(s.t, "{.+?}"))
                    s.t = Replace(s.t, "{.+?}", "");
            }
            //if (pc.pr) // Apply every next word, if ":p" directive defined
            //{
            //    s = pc.p(s);
            //}
            //if (gc.ct.Length > 0) //temp
            //{
            //    s = gc.p(s);
            //}
        }

        void F_C_ASSOC() // !Unused
        {
            foreach (var c in rules)
            {
                for (int i = 0; i < w.Count; i++)
                {
                    var n = w[i];
                    while (n.t.Contains(c.ct))
                    {
                        var ic = n.t.IndexOf(c.ct);
                        var ws = n.f.MeasureString(c.ct);
                        var sp = f.MeasureString("  ").X;
                        n.t = n.t.Remove(ic, c.ct.Length);
                        for (var j = i; j < w.Count; j++)
                        {
                            var u = w[j];
                            if (u.l == n.l)
                                //u.b.X -= (int)ws.X/* + (int)n.*/;
                                w[j] = u;
                        }
                        //n = c.p(n);
                        n.b.Width -= (int)ws.X;
                        w[i] = n;
                    }
                }
            }
            //Console.WriteLine(s);

        }

        List<sub> w = new List<sub>();

        internal class sub  // A dedicated word pointer
        {
            public SpriteFont f; // Word's spritefont
            public Rectangle b; // Word's bounds
            public string t; // Text
            public bool fc; // Formatting condition
            public Color c;
            public Color dc; // Default color. Required for :h directive
            public object[] nw; // Reference-list for previous and next words
            public float ww; // Word width
            public float wh; // Word height
            public int l; // Word's line index
            public sub(Vector2 p, SpriteFont f, string t, Color c, float ww, float wh, int l)
            {
                this.t = t; this.f = f; dc = this.c = c; this.ww = ww; this.wh = wh; this.l = l; nw = new object[] { null, null };
                b = new Rectangle(p.ToPoint(), f.MeasureString(t).ToPoint());
                fc = true;
                chs = new List<string>(t.Split(new string[] { "" }, StringSplitOptions.RemoveEmptyEntries)).ConvertAll(n => new ch() { chr = n.ToCharArray()[0], c = c });
                hov = def = new rule()
                {
                    aplog = delegate (sub s, string v) { s.c = c; s.f = f; return s; },
                };
            }

            internal rule def, cur, hov;
            public void upd(Vector2 sp)
            {
                var bd = new Rectangle(b.Location + sp.ToPoint(), b.Size);
                //if (Control.MouseHoverOverG(bd))
                {
                    cur = Control.MouseHoverOverG(bd) && fc ? hov : def;
                    cur.aplog(this, cur.val);
                }
            }
            public List<ch> chs;
            public Action ond = null; // Action applied when word is drawn.
            //public void atc(Action a) => onh.Add(a); // Attach new action
            public static implicit operator string(sub t) { return t.t; }
            public static implicit operator Vector2(sub t) { return t.b.Location.ToVector2(); }
            public static implicit operator Color(sub t) { return t.c; }
            public static implicit operator Rectangle(sub t) { return t.b; }
            public static implicit operator SpriteFont(sub t) { return t.f; }
        }

        //List<subgroup> subs = new List<subgroup>();

        internal class subgroup : List<sub> {} // Proto

        internal struct ch // !Unused
        {
            public char chr;
            public Color c;
        }

        internal struct rule // A command
        {
            public string ct; // std marking: {.ct}
            internal bool ish;
            internal string val;
            public Func<sub, string, sub> aplog; // Delegate that applies command's logic.
            //public sub p(sub s, string v = "") => logic.Invoke(s, v); // "v" is addition parameters for commands. Unused currently.
            //public bool pr; // Propagator flag that allows apply specified formatting for next words until new command defined.
            public static rule al(string c) // Command analyser. Selects proper command, applies directive for it if there is.
            {
                var iv = Match(Replace(c, "((?<=}).+)", ""), @"\(.+(?:\))").Value; // Parse params of the command needed for further usage.
                var re = Replace(c, @"\(.+(?:\))|((?<=}).+)", ""); // Select very first command inside string.
                var dir = Matches(re, @"((?<=:|,)\w+)"); // Defines, whether there is any directive. Keeps the directive, if so.
                var cc = Replace(Replace(re, @":(\w+|,)+", ""), ":", "");
                var cm = rules.Find(n => n.ct == cc); // Selects a command from the list, in advance cleaning it up of directives.
                cm.val = iv;
                if (dir.Count > 0)
                {
                    switch (dir[0].Value)
                    {
                        case "h": cm.ish = true; break;
                    }
                }
                //if (cm.ct == null) throw new Exception("No such command found => " + re);
                //if(iv.Length > 0)
                //{
                //    var conf = cm.onf;
                //    cm.onf = delegate (sub s, string v)
                //    {
                //        conf.Invoke(s, iv); // Invoke the processor with defined command.
                //        return s;
                //    };
                //}
                //if (dir.Count > 0)
                //    foreach (var n in dir)
                //        switch ((n.GetType().GetProperty("Value").GetValue(n)))
                //        {
                //            case "p":
                //                {
                //                    cm.pr = true; // Enable propagator for this command.
                //                    pc = cm;
                //                }
                //                break;
                //            case "h":
                //                {
                //                    var conf = cm.onf; // Create delegate-formatter reference. Some kind of copy.
                //                    cm.onf = delegate (sub s, string v) // Redefine 
                //                    {
                //                        if (Control.MouseHoverOverG(s) && !Control.LeftButtonPressed)
                //                            return conf.Invoke(s, v); // Apply formatting when mouse hovers over the "s" word
                //                        return s;
                //                    };
                //                    pc = cm;
                //                }
                //                break;
                //                //default:
                //                //throw new Exception("No such directive found => " + dir);
                //        }
                //else pc.pr = false;
                return cm;
            }
        }

        //static rule pc; // A primary command that applies formatting.
        //static string gc; // 
        static List<sub> tg = new List<sub>();

        static List<rule> rules = new List<rule>() // A predefined list of commands
        {
            new rule() // Sample. A command that makes words blue.
            {
                ct = "{blue}", // Define command text
                aplog = delegate(sub s, string v) // Define an action that will be applied for specified word 
                {
                    //if(v)
                    s.c = new Color(0,0,255);
                    return s;
                }
            },
            new rule()
            {
                ct = "{#}",
                aplog = delegate(sub s, string v)
                {
                    var m = Matches(v, "\\d+");
                    s.c = new Color(int.Parse(m[0].Value), int.Parse(m[1].Value), int.Parse(m[2].Value));
                    return s;
                }
            },
            //new rule()
            //{
            //    ct = "{$=>}",
            //    onf = delegate(sub s, string v)
            //    {
            //        if(gc.Length == 0) gc = v;
            //        tg.Add(s);
            //        return s;
            //    }
            //},
            //new cmd()
            //{
            //    ct = "{=>$}",
            //    onf = delegate(sub s, string v)
            //    {
            //        pc.pr = false;
            //        var f = "{"+gc.Substring(1,gc.Length-2)+"}"; // Extract the parameter
            //        var c = cmd.al(f); // Define the command
            //        if(tg.Exists(n => n.b.Contains(GlobalMousePos)))
            //        foreach(var n in tg)
            //            c.p(n); // Apply this command for each word
            //        tg.Clear();
            //        return s;
            //    }
            //},
            new rule()
            {
                ct = "{exec}",
                aplog = delegate(sub s, string v)
                {

                    return s;
                }
            },
            new rule()
            {
                ct = "{@p}", // A null-command that prevents continued propagation
                aplog = delegate(sub s, string v)
                {
                    return s;
                }
            },
        };


        public void Render(SpriteBatch batch)
        {
            w.ForEach(n =>
            {
                n.ond?.Invoke();
                batch.DrawWord(n);
            });
        }

        public void Render(SpriteBatch batch, Vector2 pos)
        {
            w.ForEach(n =>
            {
                n.ond?.Invoke();
                batch.DrawWord(pos, n);
            });
        }

        public static implicit operator string(TextBuilder tb)
        {
            return tb.t;
        }

        public static TextBuilder operator +(TextBuilder tb, string s)
        {
            tb.UpdateText(tb.t + s);
            return tb;
        }
    }

    public static class Complex
    {
        internal static void DrawWord(this SpriteBatch b, TextBuilder.sub w)
        {
            b.DrawString(w, w, w, w);
        }

        internal static void DrawWord(this SpriteBatch b, Vector2 pos, TextBuilder.sub w)
        {
            //var wb = w.b;
            //wb.Location += pos.ToPoint(); debug
            //b.DrawFill(wb, new Color(52, 115, 52, 40));
            b.DrawString(w, w, w + pos, w);
        }
    }
}
