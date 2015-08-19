// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="NaiveBayesModel.cs">
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
//   A data Model for the naive bayes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.NaiveBayes
{
    using System;
    using System.Xml;

    using numl.Math.LinearAlgebra;
    using numl.Model;
    using numl.Utils;

    /// <summary>A data Model for the naive bayes.</summary>
    public class NaiveBayesModel : Model
    {
        #region Public Properties

        /// <summary>Gets or sets the root.</summary>
        /// <value>The root.</value>
        public Measure Root { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Predicts the given o.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="y">The Vector to process.</param>
        /// <returns>An object.</returns>
        public override double Predict(Vector y)
        {
            if (this.Root == null || this.Descriptor == null)
            {
                throw new InvalidOperationException("Invalid Model - Missing information");
            }

            var lp = Vector.Zeros(this.Root.Probabilities.Length);
            for (var i = 0; i < this.Root.Probabilities.Length; i++)
            {
                var stat = this.Root.Probabilities[i];
                lp[i] = Math.Log(stat.Probability);
                for (var j = 0; j < y.Length; j++)
                {
                    var conditional = stat.Conditionals[j];
                    var p = conditional.GetStatisticFor(y[j]);

                    // check for missing range, assign bad probability
                    lp[i] += Math.Log(p == null ? 10e-10 : p.Probability);
                }
            }

            var idx = lp.MaxIndex();
            return this.Root.Probabilities[idx].X.Min;
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">
        ///     The <see cref="T:System.Xml.XmlReader" /> stream from which the object is
        ///     deserialized.
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement();
            this.Descriptor = Xml.Read<Descriptor>(reader);
            this.Root = Xml.Read<Measure>(reader);
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            Xml.Write<Descriptor>(writer, this.Descriptor);
            Xml.Write<Measure>(writer, this.Root);
        }

        #endregion
    }
}