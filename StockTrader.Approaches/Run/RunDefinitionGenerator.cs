using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StockTrader.Approaches.Run
{
    public static class RunDefinitionGenerator
    {
        //this delagate is used to update the status on the UI
        public delegate Task StatusCallback(int currentPermutation, int totalPermutations);

        //this delegate is used to save the buffered run definitions
        public delegate Task SaveRunDefintionsCallback(List<RunDefinition> runDefinitions);

        //Used to cancel the generation of of run definitions.
        //This is not thread safe but doesn't need to be
        public static bool CancelGeneration { get; set; }

        public static async Task<int> GenerateAsync(ApproachDefinition approachDefinition, SaveRunDefintionsCallback saveFunction, int bufferBeforeSave = 100, StatusCallback statusFunction = null, int bufferBeforeStatus = 100)
        {

            var totalPermutations = 0;

            //the iteration of all the parameters changes the definition,
            //so we deep clone to ensure the original definition is not modified
            ApproachDefinition approach = approachDefinition.DeepClone<ApproachDefinition>();

            //Get all the parameters on all algorithms of the approach and initialize them
            List<AlgorithmParameter> allParameters = PrepareParameters(approach);

            //Get a total number of iterations expected for status updates
            RunDefinitionBuffer buffer = PrepareRunDefinitionBuffer(saveFunction, bufferBeforeSave, statusFunction, bufferBeforeStatus, allParameters);

            //Iterate all the possible combinations of the parameters, and invoke the save and status callbacks stored in the buffer
            //IterateAllParameters(approach, allParameters, buffer);
            await IterateAllParametersNoRecursion(approach, allParameters, buffer);

            //Make sure all items are saved and status updated
            buffer.FlushBuffer();

            totalPermutations = buffer.PermutationCount;

            return totalPermutations;
        }

        private static RunDefinitionBuffer PrepareRunDefinitionBuffer(SaveRunDefintionsCallback saveFunction, int bufferBeforeSave, StatusCallback statusFunction, int bufferBeforeStatus, List<AlgorithmParameter> allParameters)
        {
            //calculate the number of permutations for status updates
            var totalNumberOfIterations = Common.CalculateApproachCount(allParameters);

            //Create the buffer for saving the iterations
            return new RunDefinitionBuffer(saveFunction, bufferBeforeSave,
                                                    statusFunction, bufferBeforeStatus,
                                                    totalNumberOfIterations);
        }

        private static List<AlgorithmParameter> PrepareParameters(ApproachDefinition approach)
        {
            //gather the buy and sell algorithms from the approach into one list
            var algorithmDefs = approach.AllAlgorithms;

            //Initialize all the parameters to their initial value
            InitializeAllParameters(algorithmDefs);

            //Get all the parameters from across all the algorithms
            return algorithmDefs.SelectMany(a => a.Parameters).ToList();
        }

        private static void InitializeAllParameters(List<AlgorithmDefinition> algorithms)
        {
            foreach (var algorithm in algorithms)
            {
                algorithm.Parameters.ForEach(p => p.Initialize());
            }
        }

        private static async Task IterateAllParametersNoRecursion(ApproachDefinition approachDefinition, List<AlgorithmParameter> parameters, RunDefinitionBuffer buffer)
        {
            //build arrays of possible values
            double[][] parameterRanges = new double[parameters.Count][];

            for (int current = 0; current < parameters.Count; current++)
            {
                parameterRanges[current] = parameters[current].GetParamterRange();

            }

            //now get the cartesian product of all the lists
            var allCombinations = Common.Crossproduct<double>(parameterRanges);

            foreach (var combination in allCombinations)
            {
                var index = 0;
                foreach (var value in combination)
                {
                    if (parameters[index].ValueIsBoolean)
                        parameters[index].BooleanValue = value != 0;
                    else
                        parameters[index].NumericValue = value;

                    index++;
                }

                await buffer.AddAsync(new RunDefinition(approachDefinition));
                //await Task.Delay(1);
            }

        }

        private static double[] GetParamterRange(this AlgorithmParameter currentParameter)
        {
            if (currentParameter.ValueIsBoolean)
            {
                if (currentParameter.UseRange)
                    return new double[2] { 0, 1 };
                else
                    return new double[1] { currentParameter.BooleanValue ? 1 : 0 };
            }
            else
            {
                if (currentParameter.UseRange)
                {

                    //this is a numerical range, so we recurse through all the parameters
                    /*
                     * NOTE:  The double extension method of 'LessThanAlmostEqualTo' is used here
                     *          to avoid rounding errors due to binary representation of rational numbers
                     *
                     *          DECIMAL datatype is more appropriate for this reason,
                     *          however is significantly slower than double.  When dealing with
                     *          millions of iterations, the double vs decimal speed difference is meaningful
                     */
                    var values = new List<double>();
                    for (double r = currentParameter.RangeStart; r.LessThanAlmostEqualTo(currentParameter.RangeEnd); r += currentParameter.RangeStep)
                    {
                        values.Add(r);

                    }

                    return values.ToArray();
                }
                else
                {
                    return new double[1] { currentParameter.NumericValue };
                }
            }
        }

        private static void IterateSingleParameter(ApproachDefinition approach, AlgorithmParameter currentParameter, RunDefinitionBuffer buffer)
        {
            if (currentParameter.UseRange)
            {
                if (currentParameter.ValueIsBoolean)
                {
                    //The boolean range is only 2 values...so we just set those two and recurse the other parameters
                    currentParameter.BooleanValue = true;
                    buffer.AddAsync(new RunDefinition(approach));
                    currentParameter.BooleanValue = false;
                    buffer.AddAsync(new RunDefinition(approach));

                }
                else
                {
                    //this is a numerical range, so we recurse through all the parameters
                    /*
                     * NOTE:  The double extension method of 'LessThanAlmostEqualTo' is used here
                     *          to avoid rounding errors due to binary representation of rational numbers
                     *
                     *          DECIMAL datatype is more appropriate for this reason,
                     *          however is significantly slower than double.  When dealing with
                     *          millions of iterations, the double vs decimal speed difference is meaningful
                     */
                    for (double r = currentParameter.RangeStart; r.LessThanAlmostEqualTo(currentParameter.RangeEnd); r += currentParameter.RangeStep)
                    {
                        if (CancelGeneration) return;  //this will prevent an new definitions from buffering, but it still needs to unwind the stack

                        currentParameter.NumericValue = r;

                        buffer.AddAsync(new RunDefinition(approach));
                    }
                }

                currentParameter.NumericValue = currentParameter.RangeStart;
            }
            else
            {
                buffer.AddAsync(new RunDefinition(approach));
            }
        }

        private static void IterateAllParameters(ApproachDefinition approachDefinition, List<AlgorithmParameter> parameters, RunDefinitionBuffer buffer)
        {
            Console.WriteLine("Iterating All Parameters");

            if (CancelGeneration) return;  //this will prevent an new definitions from buffering, but it still needs to unwind the stack

            var currentParameter = parameters.FirstOrDefault();

            if (currentParameter != null)
            {
                //we get a list of the remaining parameters that need to be recursed through
                var otherParameters = parameters.Skip(1).ToList();

                if (currentParameter.UseRange)
                {
                    if (currentParameter.ValueIsBoolean)
                    {
                        //The boolean range is only 2 values...so we just set those two and recurse the other parameters
                        currentParameter.BooleanValue = true;
                        SaveOrNext(approachDefinition, otherParameters, buffer);
                        currentParameter.BooleanValue = false;
                        SaveOrNext(approachDefinition, otherParameters, buffer);
                    }
                    else
                    {
                        //this is a numerical range, so we recurse through all the parameters
                        /*
                         * NOTE:  The double extension method of 'LessThanAlmostEqualTo' is used here
                         *          to avoid rounding errors due to binary representation of rational numbers
                         *
                         *          DECIMAL datatype is more appropriate for this reason,
                         *          however is significantly slower than double.  When dealing with
                         *          millions of iterations, the double vs decimal speed difference is meaningful
                         */
                        for (double r = currentParameter.RangeStart; r.LessThanAlmostEqualTo(currentParameter.RangeEnd); r += currentParameter.RangeStep)
                        {
                            if (CancelGeneration) return;  //this will prevent an new definitions from buffering, but it still needs to unwind the stack

                            currentParameter.NumericValue = r;

                            SaveOrNext(approachDefinition, otherParameters, buffer);
                        }
                    }

                    currentParameter.NumericValue = currentParameter.RangeStart;
                }
                else
                {
                    //The parameter already holds the 1 value that will be evaluated, so we iterate the others and evaluate
                    //the other parameters need to incement for each value of r
                    SaveOrNext(approachDefinition, otherParameters, buffer);
                }
            }
        }

        private static void SaveOrNext(ApproachDefinition approach, List<AlgorithmParameter> otherParameters, RunDefinitionBuffer buffer)
        {
            //we only save if there are no other parameters to iterate
            //this means this iteration definition is complete and all parameters have been set.
            if (otherParameters.Count == 0)
            {
                //create a run definition and buffer it.  The buffer manages flushing/saving the run definitions
                buffer.AddAsync(new RunDefinition(approach));

            }
            else
            {

                Task.Delay(1);
                //there are other parameters that need to be set before we can save the definition
                IterateAllParameters(approach, otherParameters, buffer);
            }
        }
    }
}