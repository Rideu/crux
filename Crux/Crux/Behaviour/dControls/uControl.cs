﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;
/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux.dControls
{
    /// <summary>
    /// Base control class.
    /// </summary>
    [DebuggerDisplay("Name: {Name}")]
    public abstract class uControl : IControl, IDisposable
    {
        /// <summary>
        /// Returns the owner of this element.
        /// </summary>
        public abstract uControl Owner { get; set; }
        Form originForm;
        public Form MainForm => originForm;
        /// <summary>
        /// Returns id which is assigned once attached to the form.
        /// </summary>
        public abstract int GetID { get; }
        public Color FormColor;
        protected Texture2D Tex;
        protected Point InitialPosition;
        public Rectangle Bounds;
        public Rectangle DrawingBounds => Bounds.Intersect(Owner.Bounds).Intersect(MainForm.Bounds);
        public float X, Y, Width, Height;
        public string Name => GetType().ToString() + " : " + Alias;
        internal protected string Alias;
        /// <summary>
        /// Returns true if mouse stays inside form's bounds.
        /// </summary>
        public bool IsActive { get; set; }  // TODO: Setter / (getter to public) to internal
        /// <summary>
        /// Same as "IsActive", but allows ignoring control handling in the game environment. Switches if "IgnoreControl" equals "true".
        /// </summary>
        public bool IsFadable { get; set; }

        public bool EnterHold { get; set; }

        protected string text = "";
        public abstract string Text { get; set; }

        public enum Align
        {
            Left,
            Right,
            Top,
            Bottom,
            None
        }
        public abstract Align CurrentAlign { set; get; }

        public abstract Action UpdateHandler { set; }
        public abstract event Action OnUpdate;
        public event EventHandler OnMouseEnter; internal bool OMEOccured;
        public event EventHandler OnMouseLeave; internal bool OMLOccured = true;

        internal bool F_Focus;
        public event EventHandler OnFocus; 
        public event EventHandler OnLeave;
        public event EventHandler OnFocusChange;

        #region Controls

        public List<uControl> Controls = new List<uControl>();
        public int GetControlsNum => Controls.Count;
        /// <summary>
        /// Adds specified Control.
        /// </summary>
        /// <param name="c">Specified Control.</param>
        public void AddNewControl(uControl c)
        {
            c.Owner = this;
            c.Initialize();
            Controls.Add(c);
        }

        /// <summary>
        /// Adds specified Controls list.
        /// </summary>
        /// <param name="cl">Specified Controls.</param>
        public void AddNewControl(params uControl[] cl)
        {
            foreach (var c in cl)
            {
                AddNewControl(c);
            }
        }

        /// <summary>
        /// Adds specified Controls list.
        /// </summary>
        /// <param name="cl">Specified Controls.</param>
        public void AddNewControl(List<uControl> cl)
        {
            foreach (var c in cl)
            {
                c.Owner = this;

                c.Initialize();
                Controls.Add(c);
            }
        }

        /// <summary>
        /// Gets Control using specified id.
        /// </summary>
        /// <param name="id">Id of the element that has been added by specified order.</param>
        /// <returns></returns>
        public uControl GetControl(int id) => Controls[id - 1];
        #endregion

        /// <summary>
        /// Causes this control to reupdate itself and its content.
        /// </summary>
        public abstract void Invalidate();

        public void UpdateBounds()
        {
            X = Owner.X + InitialPosition.X;
            Y = Owner.Y + InitialPosition.Y;
            Bounds = Rectangle(X, Y, Width, Height);
        }

        /// <summary>
        /// Describes update-per-frame logic.
        /// </summary>
        public abstract void Update();
        public virtual void EventProcessor()
        {
            if (F_Focus != IsActive)
            {
                if (F_Focus == false && IsActive == true)
                {
                    OnFocus?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    Invalidate();
                    OnLeave?.Invoke(this, EventArgs.Empty);
                }
                OnFocusChange?.Invoke(this, EventArgs.Empty);
            }

            if (!IsActive) OMEOccured = false;
            if (IsActive && !OMEOccured)
            {
                OnMouseEnter?.Invoke(this, EventArgs.Empty);
                EnterHold = Control.LeftButtonPressed;
                OMEOccured = true;
            }


            if (IsActive) OMLOccured = false;
            if (!IsActive && !OMLOccured)
            {
                OnMouseLeave?.Invoke(this, EventArgs.Empty);
                EnterHold = !(OMLOccured = true);
            }

            F_Focus = IsActive;
        }
        public abstract void InnerUpdate();

        public bool IsHovering;
        public bool IsHolding;
        public bool IsClicked;
        /// <summary>
        /// Describes the sequence of actions once constructor called.
        /// </summary>
        internal virtual void Initialize()
        {
            originForm = Owner is Form ? (Owner as Form) : Owner.MainForm;
            InitialPosition = new Point((int)X, (int)Y);
            UpdateBounds();
        }

        public SpriteBatch Batch = Game1.spriteBatch;

        protected RasterizerState rasterizer = new RasterizerState()
        {
            ScissorTestEnable = true,
        };

        public Rectangle GetRelativeBounds() => Rectangle((X + Owner.X + (Owner.Owner == null ? 0 : Owner.Owner.X)), (Y + Owner.Y + (Owner.Owner == null ? 0 : Owner.Owner.Y)), Width, Height);

        public Point GetOwnerClipping() => new Point((int)Width + (int)(Owner.Width - Width - X), (int)Height + (int)(Owner.Height - Height - Y));

        /// <summary>
        /// Describes draw-per-frame logic.
        /// </summary>
        public abstract void Draw();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Tex.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        ~uControl()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
