using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTrader.Approaches
{
    public static class Common
    {

        public static string GetAlgorithmsDescriptions(IEnumerable<AlgorithmDefinition> algorithms, bool showCurrentValues = false, string cssForName = "", string cssForValue = "")
        {
            var descriptionList = GetAlgorithmsDescriptionList(algorithms, showCurrentValues, cssForName, cssForValue);
            var algos = "";

            foreach (var item in descriptionList)
            {
                algos += $"={GetAlgorithmString(item.Key, item.Value)}";

            }

            return algos;
        }

        public static Dictionary<string, List<string>> GetAlgorithmsDescriptionList(IEnumerable<AlgorithmDefinition> algorithms, bool showCurrentValues = false, string cssForName = "", string cssForValue = "")
        {
            Dictionary<string, List<string>> algos = new Dictionary<string, List<string>>();

            foreach (var item in algorithms)
            {
                List<string> parameters = GetAlgorithParamtersDescriptions(item, showCurrentValues, cssForName, cssForValue);

                algos.Add(GetAbbreviation(item.Name), parameters);
            }

            return algos;

        }

        public static List<string> GetAlgorithParamtersDescriptions(AlgorithmDefinition item, bool showCurrentValues = false, string cssForName = "", string cssForValue = "")
        {
            List<string> parameters = new List<string>();
            var cssForNameClose = "";
            var cssForValueClose = "";

            if (!string.IsNullOrWhiteSpace(cssForName))
            {
                cssForName = $"<span class=\"{cssForName}\">";
                cssForNameClose = "</span>";
            }

            if (!string.IsNullOrWhiteSpace(cssForValue))
            {
                cssForValue = $"<span class=\"{cssForValue}\">";
                cssForValueClose = "</span>";
            }

            foreach (var parm in item.Parameters)
            {
                if (parm.UseRange && !showCurrentValues)
                {
                    if (parm.ValueIsBoolean)
                        parameters.Add($"{cssForName}{GetAbbreviation(parm.Name)}{cssForNameClose}({cssForValue}TF{cssForValueClose})");
                    else
                        parameters.Add($"{cssForName}{GetAbbreviation(parm.Name)}{cssForNameClose}({cssForValue}{parm.RangeStart:G10}{cssForValueClose}-{cssForValue}{parm.RangeEnd:G10}{cssForValueClose}-{cssForValue}{parm.RangeStep:G10}{cssForValueClose})");
                }
                else
                {
                    if (parm.ValueIsBoolean)
                    {
                        bool boolVal = parm.BooleanValue;

                        parameters.Add($"{cssForName}{GetAbbreviation(parm.Name)}{cssForNameClose}({cssForValue}{(boolVal ? "T" : "F")}{cssForValueClose})");
                    }
                    else
                    {
                        parameters.Add($"{cssForName}{GetAbbreviation(parm.Name)}{cssForNameClose}({cssForValue}{parm.NumericValue:G10}{cssForValueClose})");
                    }
                }
            }

            return parameters;
        }

        public static string GetAlgorithDescription(AlgorithmDefinition item, bool showCurrentValues = false, string cssForName = "", string cssForValue = "")
        {

            return GetAlgorithmString(
                        GetAbbreviation(item.Name),
                        GetAlgorithParamtersDescriptions(item, showCurrentValues, cssForName, cssForValue));
        }

        private static string GetAlgorithmString(string algoAbbreviation, List<string> parameterDescriptions)
        {
            return $"{algoAbbreviation}[{String.Concat(parameterDescriptions)}]";

        }

        public static string GetAbbreviation(string source)
        {
            return string.Concat(source.Where(c => (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')));
        }

        internal static int CalculateApproachCount(List<AlgorithmParameter> parameters)
        {
            int total = 1;

            foreach (var item in parameters.Where(p => p.UseRange).ToList())
            {
                //calculate range values
                if (item.ValueIsBoolean)
                    total *= 2;
                else
                    total *= (int)Math.Round(Math.Abs((item.RangeEnd - item.RangeStart + item.RangeStep) / item.RangeStep), 0);

            }

            return total;
        }

        public static IEnumerable<T> Empty<T>()
        {
            return new T[] { };
        }

        public static IEnumerable<T> Cons<T>(T head, IEnumerable<T> tail)
        {
            yield return head;
            foreach (var t in tail)
                yield return t;
        }

        public static IEnumerable<IEnumerable<T>> Crossproduct<T>(IEnumerable<IEnumerable<T>> sets)
        {
            if (!sets.Any())
                return new[] { Empty<T>() };

            var head = sets.First();
            var tailCross = Crossproduct<T>(sets.Skip(1));

            return
                from h in head
                from ts in tailCross
                select Cons(h, ts);
        }
    }
}