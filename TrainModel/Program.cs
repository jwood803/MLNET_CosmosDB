using Microsoft.Azure.Cosmos;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TrainModel
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = "";
            var data = new List<DiabetesData>();

            var client = new CosmosClient(connection);

            var db = client.GetDatabase("diabetes");
            var container = db.GetContainer("diabetes");

            var queryDef = new QueryDefinition("SELECT * FROM c");

            var queryIterator = container.GetItemQueryIterator<DiabetesData>(queryDef);

            while (queryIterator.HasMoreResults)
            {
                var currentResultSet = queryIterator.ReadNextAsync().Result;
                foreach (var family in currentResultSet)
                {
                    data.Add(family);
                }
            }

            var context = new MLContext();

            var dataView = context.Data.LoadFromEnumerable(data);

            var stringColumns = dataView.Schema
                .Select(col => col.Name)
                .Where(col => col != "Id" && col != "Age" && col != "Class")
                .ToArray();

            var features = stringColumns.Where(col => col != "Class").ToArray();

            var inputColumnPairs = stringColumns.Select(col => new InputOutputColumnPair(col)).ToArray();

            var pipeline = context.Transforms.Conversion.ConvertType(inputColumnPairs, DataKind.Boolean)
                .Append(context.Transforms.Conversion.ConvertType(inputColumnPairs, DataKind.Single))
                .Append(context.Transforms.Conversion.ConvertType("Age", "Age", DataKind.Single))
                .Append(context.Transforms.Conversion.ConvertType("Label", "Class", DataKind.Boolean))
                .Append(context.Transforms.Concatenate("Features", features))
                .Append(context.Transforms.Concatenate("Features", "Features", "Age"))
                .Append(context.BinaryClassification.Trainers.LbfgsLogisticRegression());

            var model = pipeline.Fit(dataView);

            var predictionEngine = context.Model.CreatePredictionEngine<DiabetesData, DiabetesPrediction>(model);

            var item = new DiabetesData
            {
                Age = "54",
                Alopecia = "Yes",
                DelayedHealing = "No",
                Gender = "Male",
                GenitalThrush = "No",
                Irritability = "Yes",
                Itching = "Yes",
                MuscleStiffness = "No",
                Obesity = "No",
                PartialParesis = "No",
                Plyuria = "No",
                Polydipsia = "Yes",
                Polyphagia = "Yes",
                SuddenWeightLoss = "No",
                VisualBlurring = "Yes",
                Weakness = "No"
            };

            var prediction = predictionEngine.Predict(item);

            Console.WriteLine($"Has diabetes? {prediction.HasDiabetes}");
        }
    }
}
