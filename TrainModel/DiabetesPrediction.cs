using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainModel
{
    public class DiabetesPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool HasDiabetes;

        public float Score;
    }
}
