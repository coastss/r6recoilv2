using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Diagnostics;

namespace r6recoilv2.Utilities
{
    internal class Operators
    {
        public static string FilePath = "opconfig.json";
        
        public static void UpdateDataPoint(string Category, string Name, string NewX, string NewY)
        {
            var JSONFile = File.ReadAllText(FilePath);
            JObject JSONObject = JObject.Parse(JSONFile);

            var CategoryArray = JSONObject[Category] as JArray;

            if (CategoryArray != null)
            {
                var ItemToUpdate = CategoryArray.FirstOrDefault(item => item["Name"]?.ToString() == Name);

                if (ItemToUpdate != null)
                {
                    ItemToUpdate["X"] = NewX;
                    ItemToUpdate["Y"] = NewY;

                    File.WriteAllText(FilePath, JSONObject.ToString(Formatting.Indented));
                    // Trace.WriteLine($"Updated {Name} in {Category} with X: {NewX}, Y: {NewY}");
                }
                else
                {
                    // Trace.WriteLine($"No entry found for Name: {Name} in {Category}");
                }
            }
            else
            {
                // Trace.WriteLine($"Category {Category} not found in the JSON file.");
            }
        }

        public static List<(string Name, string X, string Y)> GetAllDataPointsForCategory(string Category)
        {
            var JSONFile = File.ReadAllText(FilePath);
            var JSONObject = JObject.Parse(JSONFile);
            var CategoryArray = JSONObject[Category] as JArray;

            if (CategoryArray != null)
            {
                var DataPoints = new List<(string Name, string X, string Y)>();

                foreach (var Item in CategoryArray)
                {
                    string? _Name = Item["Name"]?.ToString();
                    string? _X = Item["X"]?.ToString();
                    string? _Y = Item["Y"]?.ToString();

                    if (_Name != null && _X != null && _Y != null)
                    {
                        DataPoints.Add((_Name, _X, _Y));
                    }
                }

                return DataPoints;
            }
            else
            {
                // Trace.WriteLine($"Category {Category} not found in the JSON file.");
                return new List<(string Name, string X, string Y)>();
            }
        }

        public static (string X, string Y)? GetDataPoint(string Category, string Name)
        {
            var JSONFile = File.ReadAllText(FilePath);
            var JSONObject = JObject.Parse(JSONFile);
            var CategoryArray = JSONObject[Category] as JArray;

            if (CategoryArray != null)
            {
                var Item = CategoryArray.FirstOrDefault(i => i["Name"]?.ToString() == Name);

                if (Item != null)
                {
                    var xValue = Item["X"]?.ToString();
                    var yValue = Item["Y"]?.ToString();

                    // Stupid fucking VS always giving warnings
                    #pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                    return (xValue, yValue);
                    #pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                }
                else
                {
                    // Trace.WriteLine($"No entry found for Name: {Name} in {Category}");
                }
            }
            else
            {
                // Trace.WriteLine($"Category {Category} not found in the JSON file.");
            }

            return null;
        }

    }
}
