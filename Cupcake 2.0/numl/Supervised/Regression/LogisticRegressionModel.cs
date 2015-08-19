// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogisticRegressionModel.cs" company="ChewyMoon">
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
//   A Logistic Regression Model object
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.Regression
{
    using System;
    using System.Xml;

    using numl.Math.Functions;
    using numl.Math.LinearAlgebra;
    using numl.Model;
    using numl.PreProcessing;
    using numl.Utils;

    /// <summary>
    ///     A Logistic Regression Model object
    /// </summary>
    [Serializable]
    public class LogisticRegressionModel : Model
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogisticRegressionModel" /> class.
        ///     Default constructor
        /// </summary>
        public LogisticRegressionModel()
        {
            this.PolynomialFeatures = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Logistic function
        /// </summary>
        public IFunction LogisticFunction { get; set; }

        /// <summary>
        ///     The additional number of quadratic features to create as used in generating the model
        /// </summary>
        public int PolynomialFeatures { get; set; }

        /// <summary>
        ///     Theta parameters vector mapping X to y.
        /// </summary>
        public Vector Theta { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Create a prediction based on the learned Theta values and the supplied test item.
        /// </summary>
        /// <param name="y">Training record</param>
        /// <returns></returns>
        public override double Predict(Vector y)
        {
            var tempy = this.PolynomialFeatures > 0
                            ? FeatureDimensions.IncreaseDimensions(y, this.PolynomialFeatures)
                            : y;
            tempy = tempy.Insert(0, 1.0);
            return this.LogisticFunction.Compute((tempy * this.Theta).ToDouble()) >= 0.5 ? 1d : 0d;
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">
        ///     The <see cref="T:System.Xml.XmlReader" /> stream from which the object is
        ///     deserialized.
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            var sigmoid = Ject.FindType(reader.GetAttribute("LogisticFunction"));
            this.LogisticFunction = (IFunction)Activator.CreateInstance(sigmoid);

            reader.ReadStartElement();

            this.Descriptor = Xml.Read<Descriptor>(reader);
            this.Theta = Xml.Read<Vector>(reader);
            this.PolynomialFeatures = Xml.Read<int>(reader);
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("LogisticFunction", this.LogisticFunction.GetType().Name);

            Xml.Write<Descriptor>(writer, this.Descriptor);
            Xml.Write<Vector>(writer, this.Theta);
            Xml.Write<int>(writer, this.PolynomialFeatures);
        }

        #endregion
    }
}