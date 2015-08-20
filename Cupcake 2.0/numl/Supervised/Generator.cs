// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Generator.cs" company="ChewyMoon">
//   Copyright (C) 2015 ChewyMoon
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   A generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using numl.Math.LinearAlgebra;
    using numl.Model;

    /// <summary>A generator.</summary>
    public abstract class Generator : IGenerator
    {
        #region Public Events

        /// <summary>Event queue for all listeners interested in ModelChanged events.</summary>
        public event EventHandler<ModelEventArgs> ModelChanged;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the descriptor.</summary>
        /// <value>The descriptor.</value>
        public Descriptor Descriptor { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Generate model based on a set of examples.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="examples">Example set.</param>
        /// <returns>Model.</returns>
        public IModel Generate(IEnumerable<object> examples)
        {
            if (examples.Count() == 0)
            {
                throw new InvalidOperationException("Empty example set.");
            }

            if (this.Descriptor == null)
            {
                // try to generate the descriptor
                this.Descriptor = Descriptor.Create(examples.First().GetType());
            }

            return this.Generate(this.Descriptor, examples);
        }

        /// <summary>Generate model based on a set of examples.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="description">The description.</param>
        /// <param name="examples">Example set.</param>
        /// <returns>Model.</returns>
        public IModel Generate(Descriptor description, IEnumerable<object> examples)
        {
            if (examples.Count() == 0)
            {
                throw new InvalidOperationException("Empty example set.");
            }

            this.Descriptor = description;
            if (this.Descriptor.Features == null || this.Descriptor.Features.Length == 0)
            {
                throw new InvalidOperationException("Invalid descriptor: Empty feature set!");
            }

            if (this.Descriptor.Label == null)
            {
                throw new InvalidOperationException("Invalid descriptor: Empty label!");
            }

            var doubles = this.Descriptor.Convert(examples);
            var tuple = doubles.ToExamples();

            return this.Generate(tuple.Item1, tuple.Item2);
        }

        /// <summary>Generates the given examples.</summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <param name="examples">Example set.</param>
        /// <returns>An IModel.</returns>
        public IModel Generate<T>(IEnumerable<T> examples) where T : class
        {
            var descriptor = Descriptor.Create<T>();
            return this.Generate(descriptor, examples);
        }

        /// <summary>Generate model based on a set of examples.</summary>
        /// <param name="x">The Matrix to process.</param>
        /// <param name="y">The Vector to process.</param>
        /// <returns>Model.</returns>
        public abstract IModel Generate(Matrix x, Vector y);

        #endregion

        #region Methods

        /// <summary>Raises the model event.</summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event information to send to registered event handlers.</param>
        protected virtual void OnModelChanged(object sender, ModelEventArgs e)
        {
            var handler = this.ModelChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        #endregion
    }

    /// <summary>Additional information for model events.</summary>
    public class ModelEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ModelEventArgs" /> class. Constructor.
        /// </summary>
        /// <param name="model">
        ///     The model.
        /// </param>
        /// <param name="message">
        ///     (Optional) the message.
        /// </param>
        public ModelEventArgs(IModel model, string message = "")
        {
            this.Message = message;
            this.Model = model;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the message.</summary>
        /// <value>The message.</value>
        public string Message { get; private set; }

        /// <summary>Gets or sets the model.</summary>
        /// <value>The model.</value>
        public IModel Model { get; private set; }

        #endregion

        #region Methods

        /// <summary>Makes.</summary>
        /// <param name="model">The model.</param>
        /// <param name="message">(Optional) the message.</param>
        /// <returns>The ModelEventArgs.</returns>
        internal static ModelEventArgs Make(IModel model, string message = "")
        {
            return new ModelEventArgs(model, message);
        }

        #endregion
    }
}