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
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    using CupcakePrediction.Properties;

    using LeagueSharp;

    using numl;
    using numl.Model;
    using numl.Supervised;
    using numl.Supervised.Regression;

    using Newtonsoft.Json;

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
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static volatile bool Initialized;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static void Initialize()
        {
            try
            {
                new Thread(LoadPrediction).Start();  
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        /// <summary>
        /// Loads the prediction.
        /// </summary>
        private static void LoadPrediction()
        {
            var cupcakeData = Encoding.UTF8.GetString(Resources.CupcakeData);
            var cupcakePan = JsonConvert.DeserializeObject<CupcakePan>(cupcakeData);

            // Initalize the X model
            var generatorX = new LinearRegressionGenerator
            {
                Descriptor =
                                         Descriptor.For<CupcakeIngredientX>()
                                         .With(x => x.Delay)
                                         .With(x => x.MissileSpeed)
                                         .WithEnumerable(x => x.SourcePosition, 3)
                                         .WithEnumerable(x => x.TargetPosition, 3)
                                         //.WithEnumerable(x => x.Waypoints, 3)
                                         .With(x => x.TargetMoveSpeed)
                                         .With(x => x.Width)
                                         .Learn(x => x.BakedX)
            };
            var learnedX = Learner.Learn(cupcakePan.X, 0.80, 1000, generatorX);
            xmodel = learnedX.Model;

            // Initalize the Y model
            var generatorY = new LinearRegressionGenerator()
            {
                Descriptor =
                                         Descriptor.For<CupcakeIngredientY>()
                                         .With(x => x.Delay)
                                         .With(x => x.MissileSpeed)
                                         .WithEnumerable(x => x.SourcePosition, 3)
                                         .WithEnumerable(x => x.TargetPosition, 3)
                                         // .WithEnumerable(x => x.Waypoints, 3)
                                         .With(x => x.TargetMoveSpeed)
                                         .With(x => x.Width)
                                         .Learn(x => x.BakedY)
            };
            var learnedY = Learner.Learn(cupcakePan.Y, 0.80, 1000, generatorY);
            ymodel = learnedY.Model;

           

            Initialized = true;
        }

        /// <summary>
        ///     Gets the prediction.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///     <see cref="BakedCupcake" />
        /// </returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static BakedCupcake GetPrediction(CupcakeIngredients input)
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