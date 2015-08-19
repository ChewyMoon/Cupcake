// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cupcake.cs" company="ChewyMoon">
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
//   Provides API for getting prediction using machine learning algorithms.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CupcakePrediction
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;

    using numl;
    using numl.Model;
    using numl.Supervised;
    using numl.Supervised.Regression;

    /// <summary>
    ///     Provides API for getting prediction using machine learning algorithms.
    /// </summary>
    public class Cupcake
    {
        #region Static Fields

        /// <summary>
        ///     The generated machine learning model for the x coordinate.
        /// </summary>
        private static IModel xmodel;

        /// <summary>
        ///     The generated machine learning model for the y coordinate.
        /// </summary>
        private static IModel ymodel;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Cupcake" /> is initialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if initialized; otherwise, <c>false</c>.
        /// </value>
        public static bool Initialized { get; internal set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            // Initalize the X model
            var generatorX = new LinearRegressionGenerator { Descriptor = Descriptor.Create<CupcakeIngredientX>() };
            var learnedX = Learner.Learn(new List<CupcakeIngredientX>(), 0.80, 1000, generatorX);
            xmodel = learnedX.Model;

            // Initalize the Y model
            var generatorY = new LinearRegressionGenerator() { Descriptor = Descriptor.Create<CupcakeIngredientY>() };
            var learnedY = Learner.Learn(new List<CupcakeIngredientY>(), 0.80, 1000, generatorY);
            ymodel = learnedY.Model;

            Game.PrintChat(
                "<font color=\"#E536F5\"><b>Cupcake 2.0:</b></font> loaded! Accuracy: {0}%", 
                (int)((learnedX.Accuracy + learnedY.Accuracy) / 2));

            Initialized = true;
        }

        /// <summary>
        ///     Gets the prediction.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///     <see cref="BakedCupcake" />
        /// </returns>
        public BakedCupcake GetPrediction(CupcakeIngredients input)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Cupcake Prediction isn't initialized!");
            }

            var x = input.ToXIngredient();
            var y = input.ToYIngredient();

            var predictedX = xmodel.Predict(x);
            var predictedY = ymodel.Predict(y);

            return new BakedCupcake(predictedX.BakedX, predictedY.BakedY);
        }

        #endregion
    }
}