using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KillerSodukoLambdaV2.models;

namespace KillerSodukoLambdaV2.Services
{
    public class KillerSodukoCalcService
    {

        public static List<string> calculateUniqueComboSets(List<KillerSodukoSection> sections)
        {
            List<string[]> results = new List<string[]>();

            //calculate combinations for each individual section.
            foreach (var section in sections)
                results.Add(calcSets(section.SumOfDigits, section.NumberOfSpaces));

            //create a list containing the length of each section
            List<int> indexes = results.Select(_ => _.Length).ToList();

            //craete a list of all combinations of the indexes
            var indexCombos = getIndexCombos(indexes);

            List<string> combinedNumbers = new List<string>();

            //Using the index combinations, assemble all combinations of all the input sets
            foreach (List<int> indexCombo in indexCombos)
            {
                int j = 0;
                string newString = string.Empty;
                foreach (int index in indexCombo)
                {
                    newString += results[j][index - 1];
                    j++;
                }
                combinedNumbers.Add(newString);
            }

            // remove any combinations with duplicate digits, then add a dash between each set.
            if (combinedNumbers.Count > 0)
            {
                int len = combinedNumbers[0].Length;
                combinedNumbers = combinedNumbers.Select(_ => string.Join("", _.ToString().ToCharArray().Distinct())).Select(_ => _).Where(_ => _.Length == len).ToList();

                int dashOffset = 0;
                for (int k = 0; k < results.Count - 1; k++)
                {
                    dashOffset += results[k][0].Length;
                    combinedNumbers = combinedNumbers.Select(_ => _.Substring(0, dashOffset) + "-" + _.Substring(dashOffset)).ToList();
                    dashOffset++;
                }
            }

            return combinedNumbers;
        }

        /// <summary>
        /// Create list of list<int> of each combination. so if input was {2,2,2}, output would be {1,1,1}, {1,1,2}, {1,2,1}, {1,2,2}, {2,1,1}, {2,1,2}, {2,2,1}, {2,2,2}
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        static List<List<int>> getIndexCombos(List<int> indexes)
        {
            List<List<int>> combos = new List<List<int>>();

            //create first set based on first index
            for (int i = 1; i <= indexes[0]; i++)
            {
                List<int> row = new List<int> { i };
                combos.Add(row);
            }

            for (int i = 1; i < indexes.Count; i++)
            {
                List<List<int>> combosTemp = new List<List<int>>();
                foreach (List<int> existRow in combos)
                {
                    for (int j = 1; j < indexes[i]; j++)
                    {
                        List<int> row = new List<int>(existRow);
                        row.Add(j + 1);
                        combosTemp.Add(row);
                    }
                    existRow.Add(1);
                }
                combos.AddRange(combosTemp);
            }


            return combos;
        }

        /// <summary>
        /// Calcuates all combinations for a given number of positions (numDigits) and a total of the digits.
        /// For example, if numDigits = 2 and total = 5, what combination of 2 numbers will add up to 5? 
        /// The result will be 4,1 and 3,2.  Position does not matter, so 1,4 is not included because it is the same as 4,1.
        /// </summary>
        /// <param name="total"></param>
        /// <param name="numDigits"></param>
        /// <returns></returns>
        static string[] calcSets(int total, int numDigits)
        {
            //create an integer array, populated with values from 1 to a number of 9's that are equal to numDigits.
            //using the above example of numDigits = 2, the max array will be all number from 1 to 99.
            //if numDigits was 3, then the array would be from 1 to 999
            int[] max = Enumerable.Range(1, int.Parse(new string('9', numDigits))).ToArray();

            //take each number in the integer array; 
            //remove any duplicate numbers; 
            //select only those that are still the length of numDigits and do not contain a 0.
            //select those whose digits add up to input "total"
            //order the digits and select distict (so we have 1,4, but not 4,1)
            string[] result = max.Select(_ => string.Join("", _.ToString().ToCharArray().Distinct())).Where(_ => _.Length == numDigits && !_.Contains('0'))
                .Select(_ => _).Where(i => (i.Sum(n => int.Parse(n.ToString())) == total))
                .Select(_ => string.Concat(_.OrderBy(c => c))).Distinct().ToArray();

            return result;
        }
    }
}
