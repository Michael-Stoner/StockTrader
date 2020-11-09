using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTrader.Approaches.Run
{
    internal class RunDefinitionBuffer
    {
        private readonly RunDefinitionGenerator.SaveRunDefintionsCallback saveFunction;
        private readonly RunDefinitionGenerator.StatusCallback statusFunction;
        private readonly int bufferBeforeSave;
        private readonly int bufferBeforeStatus;
        private readonly int totalPermutations;

        internal int PermutationCount { get; set; }

        internal List<RunDefinition> Buffer { get; set; }

        internal RunDefinitionBuffer(RunDefinitionGenerator.SaveRunDefintionsCallback saveFunction, int bufferBeforeSave, RunDefinitionGenerator.StatusCallback statusFunction, int bufferBeforeStatus, int totalPermutations)
        {
            //Initialize callback items and buffer
            this.saveFunction = saveFunction;
            this.statusFunction = statusFunction;
            this.bufferBeforeSave = bufferBeforeSave;
            this.bufferBeforeStatus = bufferBeforeStatus;
            Buffer = new List<RunDefinition>();
            this.totalPermutations = totalPermutations;
        }

        public async Task AddAsync(RunDefinition runDefinition)
        {
            Buffer.Add(runDefinition);

            PermutationCount++;

            //check if we should report status or save
            await ExcecuteCallbacksAsync();
        }

        private async Task ExcecuteCallbacksAsync()
        {
            if (PermutationCount % bufferBeforeStatus == 0 && statusFunction != null)
            {
                //a status update is required
                await statusFunction(PermutationCount, totalPermutations);
            }

            if (Buffer.Count == bufferBeforeSave)
            {
                //a save is required
                await saveFunction(new List<RunDefinition>(Buffer));
                Buffer.Clear();
            }
        }

        public void FlushBuffer()
        {
            statusFunction(PermutationCount, totalPermutations);
            saveFunction(new List<RunDefinition>(Buffer));
            Buffer.Clear();
        }
    }
}

/*
 *  internal class RunDefinitionBuffer
    {
        private readonly RunDefinitionGenerator.SaveRunDefintionsCallback saveFunction;
        private readonly RunDefinitionGenerator.StatusCallback statusFunction;
        private readonly int bufferBeforeSave;
        private readonly int bufferBeforeStatus;
        private readonly int totalPermutations;

        internal int PermutationCount { get; set; }

        internal List<RunDefinition> Buffer { get; set; }

        internal RunDefinitionBuffer(RunDefinitionGenerator.SaveRunDefintionsCallback saveFunction, int bufferBeforeSave, RunDefinitionGenerator.StatusCallback statusFunction, int bufferBeforeStatus, int totalPermutations)
        {
            //Initialize callback items and buffer
            this.saveFunction = saveFunction;
            this.statusFunction = statusFunction;
            this.bufferBeforeSave = bufferBeforeSave;
            this.bufferBeforeStatus = bufferBeforeStatus;
            Buffer = new List<RunDefinition>();
            this.totalPermutations = totalPermutations;
        }

        public async Task AddAsync(RunDefinition runDefinition)
        {
            Buffer.Add(runDefinition);

            PermutationCount++;

            //check if we should report status or save
            await ExcecuteCallbacksAsync();
        }

        private async Task ExcecuteCallbacksAsync()
        {
            if (PermutationCount % bufferBeforeStatus == 0 && statusFunction != null)
            {
                //a status update is required
                await statusFunction(PermutationCount, totalPermutations);
            }

            if (Buffer.Count == bufferBeforeSave)
            {
                //a save is required
                await saveFunction(new List<RunDefinition>(Buffer));
                Buffer.Clear();
            }
        }

        public void FlushBuffer()
        {
            statusFunction(PermutationCount, totalPermutations);
            saveFunction(new List<RunDefinition>(Buffer));
            Buffer.Clear();
        }
    }
*/