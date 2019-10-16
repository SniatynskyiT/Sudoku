using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
    public static class SudokuHandler
    {
        private static readonly string digits = "123456789";

        private static readonly string rows = "ABCDEFGHI";

        private static readonly string cols = digits;

        private static readonly string[] squares = GetCross(rows, cols);

        private static readonly string[][] unitList = GetUnitList();

        private static readonly Dictionary<string, string[][]> units = GetUnits();

        private static readonly Dictionary<string, HashSet<string>> peers = GetPeers();

        public static Dictionary<string, string> Solution { get; set; }

        private static string[] GetCross(string a, string b)
        {
            return a.Select(item => b.Select(bItem => item.ToString() + bItem.ToString())).SelectMany(item => item).ToArray();
        }

        private static string[][] GetUnitList()
        {
            string[] rowsX = { "ABC", "DEF", "GHI" };
            string[] colsX = { "123", "456", "789" };

            return cols.Select(item => GetCross(rows, item.ToString()))
                  .Concat(rows.Select(item => GetCross(item.ToString(), cols)))
                  .Concat(rowsX.Select(row => colsX.Select(col => GetCross(row, col))).SelectMany(item => item)).ToArray();
        }

        private static Dictionary<string, string[][]> GetUnits()
        {
            return squares.ToDictionary(s => s, s => unitList.Where(item => item.Contains(s)).ToArray());
        }

        private static Dictionary<string, HashSet<string>> GetPeers()
        {
            return squares.ToDictionary(s => s, s => units[s].SelectMany(unit => unit).Where(item => item != s).ToHashSet());
        }

        public static Dictionary<string, string> GetGridValues(string grid)
        {
            var chars = grid.Where(item => digits.Contains(item) || "0.".Contains(item)).ToList();
            return chars.Count() == 81 ? chars.Select((v, i) => new { i, v }).ToDictionary(item => squares[item.i], item => item.v.ToString()) : null;
        }

        private static Dictionary<string, string> ParseGrid(string grid)
        {
            var values = squares.ToDictionary(item => item, item => digits);

            var newValues = GetGridValues(grid);

            if (newValues != null)
            {
                foreach (var item in newValues)
                {
                    if (digits.Contains(item.Value) && Assign(values, item.Key, item.Value) == null)
                    {
                        return null;
                    }
                }
            }

            return values;
        }

        private static Dictionary<string, string> Assign(Dictionary<string, string> values, string s, string d)
        {
            var otherValues = values[s].Replace(d, "");
            var condition = otherValues.All(item => Eliminate(values, s, item.ToString()) != null);

            return condition ? values : null;
        }

        private static Dictionary<string, string> Eliminate(Dictionary<string, string> values, string s, string d)
        {
            if (!values[s].Contains(d))
            {
                return values;
            }

            values[s] = values[s].Replace(d, "");

            if (values[s].Length == 0)
            {
                return null;
            }
            else
            {
                if (values[s].Length == 1)
                {
                    var d2 = values[s];
                    var condition = peers[s].All(item => Eliminate(values, item, d2) != null);

                    if (!condition)
                    {
                        return null;
                    }
                }
            }
            foreach (var item in units[s])
            {
                var dPlaces = item.Where(i => values[i].Contains(d)).ToList();
                if (dPlaces.Count() == 0)
                {
                    return null;
                }
                else
                {
                    if (dPlaces.Count() == 1)
                    {
                        if (Assign(values, dPlaces[0], d) == null)
                        {
                            return null;
                        }
                    }
                }
            }

            return values;
        }

        private static void Search(Dictionary<string, string> values)
        {
            if (Solution == null)
            {
                Display(values);
                Console.WriteLine();
                Console.WriteLine();
            }
           
            var condition = squares.All(s => values[s].Length == 1);
            if (condition)
            {
                Solution = values;
            }
            else
            {
                var val = squares.Where(s => values[s].Length > 1).Select(s => new { Length = values[s].Length, Value = s }).OrderBy(item => item.Length).First();

                foreach (var item in values[val.Value])
                {
                    var valuesCopy = CloneDictionaryCloningValues(values);
                    var nextValues = Assign(valuesCopy, val.Value, item.ToString());
                    if (nextValues != null)
                    {
                        Search(nextValues);
                    }
                }
            }
        }

        public static Dictionary<string, string> Solve(string grid)
        {
            Search(ParseGrid(grid));

            return Solution;
        }

        public static void Display(Dictionary<string, string> values)
        {
            const int width = 2;

            var line = string.Join('+', Enumerable.Repeat(new string(Enumerable.Repeat('-', (width * 3) + 1).ToArray()), 3));

            foreach (var r in rows)
            {
                var result = " " + string.Join(' ', cols.Select(c => values[r.ToString() + c.ToString()] + ("36".Contains(c) ? " |" : ""))) + " ";

                Console.WriteLine(result);

                if ("CF".Contains(r))
                {
                    Console.WriteLine(line);
                }
            }
        }

        private static Dictionary<string, string> CloneDictionaryCloningValues(Dictionary<string, string> original)
        {
            Dictionary<string, string> res = new Dictionary<string, string>(original.Count, original.Comparer);
            foreach (var entry in original)
            {
                var value = new string(entry.Value);
                res.Add(entry.Key, value);
            }
            return res;
        }
    }
}
