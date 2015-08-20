// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Learner.cs" company="ChewyMoon">
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
//   Primary class for running model generators. It is designed to abstract the separation of
//   training and test sets as well as best model selection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using numl.Math.LinearAlgebra;
    using numl.Math.Probability;
    using numl.Supervised;
    using numl.Utils;

    /// <summary>
    ///     Primary class for running model generators. It is designed to abstract the separation of
    ///     training and test sets as well as best model selection.
    /// </summary>
    public static class Learner
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Learner" /> class. Static constructor.
        /// </summary>
        static Learner()
        {
            Sampling.SetSeedFromSystemTime();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Retrieve best model (or model with the highest accuracy)</summary>
        /// <param name="models">List of models.</param>
        /// <returns>Best Model.</returns>
        public static LearningModel Best(this IEnumerable<LearningModel> models)
        {
            var q = from m in models where m.Accuracy == models.Select(s => s.Accuracy).Max() select m;

            return q.FirstOrDefault();
        }

        /// <summary>
        ///     Trains an arbitrary number of models on the provided examples by creating a separation of
        ///     data based on training percentage. Each generator is rerun a predetermined amount of times.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="examples">Source data.</param>
        /// <param name="trainingPercentage">Data split percentage.</param>
        /// <param name="repeat">Number of repetitions per generator.</param>
        /// <param name="generators">Model generators used.</param>
        /// <returns>Best models for each generator.</returns>
        public static LearningModel[] Learn(
            IEnumerable<object> examples, 
            double trainingPercentage, 
            int repeat, 
            params IGenerator[] generators)
        {
            if (generators.Length == 0)
            {
                throw new InvalidOperationException("Need to have at least one generator!");
            }

            // set up models
            var models = new LearningModel[generators.Length];

            for (var i = 0; i < generators.Length; i++)
            {
                models[i] = Learn(examples, trainingPercentage, repeat, generators[i]);
            }

            return models;
        }

        /// <summary>
        ///     Trains a single model based on a generator a predefined number of times with the provided
        ///     examples and data split and selects the best (or most accurate) model.
        /// </summary>
        /// <param name="examples">Source data (in datatable form)</param>
        /// <param name="trainingPercentage">Data split percentage.</param>
        /// <param name="repeat">Number of repetitions per generator.</param>
        /// <param name="generator">Model generator used.</param>
        /// <returns>Best model for provided generator.</returns>
        public static LearningModel Learn(
            DataTable examples, 
            double trainingPercentage, 
            int repeat, 
            IGenerator generator)
        {
            return Learn(GetRows(examples), trainingPercentage, repeat, generator);
        }

        /// <summary>
        ///     Trains a single model based on a generator a predefined number of times with the provided
        ///     examples and data split and selects the best (or most accurate) model.
        /// </summary>
        /// <param name="examples">Source data.</param>
        /// <param name="trainingPercentage">Data split percentage.</param>
        /// <param name="repeat">Number of repetitions per generator.</param>
        /// <param name="generator">Model generator used.</param>
        /// <returns>Best model for provided generator.</returns>
        public static LearningModel Learn(
            IEnumerable<object> examples, 
            double trainingPercentage, 
            int repeat, 
            IGenerator generator)
        {
            // count only once
            var total = examples.Count();
            var descriptor = generator.Descriptor;
            var data = descriptor.Convert(examples).ToExamples();

            var x = data.Item1;
            var y = data.Item2;

            var models = new IModel[repeat];
            var accuracy = Vector.Zeros(repeat);

            // run in parallel since they all have 
            // read-only references to the data model
            // and update indices independently
            Parallel.For(
                0, 
                models.Length, 
                i =>
                    {
                        var t = GenerateModel(generator, x, y, examples, trainingPercentage, total);
                        models[i] = t.Model;
                        accuracy[i] = t.Accuracy;
                    });

            var idx = accuracy.MaxIndex();

            return new LearningModel { Generator = generator, Model = models[idx], Accuracy = accuracy[idx] };
        }

        #endregion

        #region Methods

        /// <summary>Generates a model.</summary>
        /// <param name="generator">Model generator used.</param>
        /// <param name="x">The Matrix to process.</param>
        /// <param name="y">The Vector to process.</param>
        /// <param name="examples">Source data.</param>
        /// <param name="trainingPct">The training pct.</param>
        /// <param name="total">Number of Examples</param>
        /// <returns>The model.</returns>
        private static LearningModel GenerateModel(
            IGenerator generator, 
            Matrix x, 
            Vector y, 
            IEnumerable<object> examples, 
            double trainingPct, 
            int total)
        {
            var descriptor = generator.Descriptor;

            // var total = examples.Count();
            var trainingCount = (int)System.Math.Floor(total * trainingPct);

            // 100 - trainingPercentage for testing
            var testingSlice = GetTestPoints(total - trainingCount, total).ToArray();

            // trainingPercentage for training
            var trainingSlice = GetTrainingPoints(testingSlice, total).ToArray();

            // training
            var x_t = x.Slice(trainingSlice);
            var y_t = y.Slice(trainingSlice);

            // generate model
            var model = generator.Generate(x_t, y_t);
            model.Descriptor = descriptor;

            // testing            
            var test = GetTestExamples(testingSlice, examples);
            double accuracy = 0;

            for (var j = 0; j < test.Length; j++)
            {
                // items under test
                var o = test[j];

                // get truth
                var truth = Ject.Get(o, descriptor.Label.Name);

                // if truth is a string, sanitize
                if (descriptor.Label.Type == typeof(string))
                {
                    truth = StringHelpers.Sanitize(truth.ToString());
                }

                // make prediction
                var features = descriptor.Convert(o, false).ToVector();

                var p = model.Predict(features);
                var pred = descriptor.Label.Convert(p);

                // assess accuracy
                if (truth.Equals(pred))
                {
                    accuracy += 1;
                }
            }

            // get percentage correct
            accuracy /= test.Length;

            return new LearningModel { Generator = generator, Model = model, Accuracy = accuracy };
        }

        /// <summary>Gets the rows in this collection.</summary>
        /// <param name="table">The table.</param>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the rows in this collection.
        /// </returns>
        private static IEnumerable<DataRow> GetRows(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                yield return row;
            }
        }

        /// <summary>Gets test examples.</summary>
        /// <param name="slice">The slice.</param>
        /// <param name="examples">Source data.</param>
        /// <returns>An array of object.</returns>
        private static object[] GetTestExamples(IEnumerable<int> slice, IEnumerable<object> examples)
        {
            return examples.Where((o, i) => slice.Contains(i)).ToArray();
        }

        /// <summary>Gets the test points in this collection.</summary>
        /// <param name="testCount">Number of tests.</param>
        /// <param name="total">Number of.</param>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the test points in this collection.
        /// </returns>
        private static IEnumerable<int> GetTestPoints(int testCount, int total)
        {
            var taken = new List<int>(testCount);
            while (taken.Count < testCount)
            {
                var i = Sampling.GetUniform(total);
                if (!taken.Contains(i))
                {
                    taken.Add(i);
                    yield return i;
                }
            }
        }

        /// <summary>Gets the training points in this collection.</summary>
        /// <param name="testPoints">The test points.</param>
        /// <param name="total">Number of.</param>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the training points in this
        ///     collection.
        /// </returns>
        private static IEnumerable<int> GetTrainingPoints(IEnumerable<int> testPoints, int total)
        {
            for (var i = 0; i < total; i++)
            {
                if (!testPoints.Contains(i))
                {
                    yield return i;
                }
            }
        }

        #endregion
    }

    /// <summary>Structure to hold generator, model, and accuracy information.</summary>
    public class LearningModel
    {
        #region Public Properties

        /// <summary>Accuracy of model on test set.</summary>
        /// <value>The accuracy.</value>
        public double Accuracy { get; set; }

        /// <summary>Generator used to create model.</summary>
        /// <value>The generator.</value>
        public IGenerator Generator { get; set; }

        /// <summary>Model created by generator.</summary>
        /// <value>The model.</value>
        public IModel Model { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Textual representation of structure.</summary>
        /// <returns>string.</returns>
        public override string ToString()
        {
            return string.Format(
                "Learning Model:\n  Generator {0}\n  Model:\n{1}\n  Accuracy: {2:p}\n", 
                this.Generator, 
                this.Model, 
                this.Accuracy);
        }

        #endregion
    }
}