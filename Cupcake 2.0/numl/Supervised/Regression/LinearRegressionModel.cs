// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinearRegressionModel.cs" company="ChewyMoon">
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
//   Linear Regression model
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.Regression
{
    using System;
    using System.Xml;

    using numl.Math.LinearAlgebra;
    using numl.Model;
    using numl.PreProcessing;
    using numl.Utils;

    /// <summary>
    ///     Linear Regression model
    /// </summary>
    [Serializable]
    public class LinearRegressionModel : Model
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinearRegressionModel" /> class.
        ///     Initialises a new LinearRegressionModel object
        /// </summary>
        public LinearRegressionModel()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinearRegressionModel" /> class.
        ///     Initialises a new LinearRegressionModel object
        /// </summary>
        /// <param name="featureAverages">
        ///     The feature averages for use in scaling test case features
        /// </param>
        /// <param name="featureSdv">
        ///     The feature standard deviations for use in scaling test case features
        /// </param>
        public LinearRegressionModel(Vector featureAverages, Vector featureSdv)
        {
            this.FeatureAverages = featureAverages;
            this.FeatureStandardDeviations = featureSdv;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Theta parameters vector mapping X to y.
        /// </summary>
        public Vector Theta { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     A row vector of the feature averages
        /// </summary>
        private Vector FeatureAverages { get; set; }

        /// <summary>
        ///     A row vector of the standard deviation for each feature
        /// </summary>
        private Vector FeatureStandardDeviations { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Create a prediction based on the learned Theta values and the supplied test item.
        /// </summary>
        /// <param name="y">Training record</param>
        /// <returns></returns>
        public override double Predict(Vector y)
        {
            y = this.Normalise(y);

            return y.Dot(this.Theta);
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
            this.Theta = Xml.Read<Vector>(reader);
            this.FeatureAverages = Xml.Read<Vector>(reader);
            this.FeatureStandardDeviations = Xml.Read<Vector>(reader);
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            Xml.Write<Descriptor>(writer, this.Descriptor);
            Xml.Write<Vector>(writer, this.Theta);
            Xml.Write<Vector>(writer, this.FeatureAverages);
            Xml.Write<Vector>(writer, this.FeatureStandardDeviations);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     TODO The normalise.
        /// </summary>
        /// <param name="y">
        ///     TODO The y.
        /// </param>
        /// <returns>
        /// </returns>
        private Vector Normalise(Vector y)
        {
            for (var i = 0; i < y.Length; i++)
            {
                y[i] = FeatureNormalizer.FeatureScale(y[i], this.FeatureAverages[i], this.FeatureStandardDeviations[i]);
            }

            return y.Insert(0, 1.0d);
        }

        #endregion
    }
}