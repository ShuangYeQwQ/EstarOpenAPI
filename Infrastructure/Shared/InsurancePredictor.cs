using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Shared
{
    public class InsurancePredictor
    {
        private readonly InferenceSession _session;
        private readonly string[] _featureNames;
        private readonly List<(string Feature, float Importance)> _importance;

        public InsurancePredictor(string modelPath, string featurePath, string importancePath)
        {
            _session = new InferenceSession(modelPath);
            _featureNames = File.ReadAllLines(featurePath);

            _importance = File.ReadAllLines(importancePath)
                .Skip(1)
                .Select(line =>
                {
                    var parts = line.Split(',');
                    return (Feature: parts[0], Importance: float.Parse(parts[1]));
                })
                .OrderByDescending(x => x.Importance)
                .ToList();
        }

        public (float Predicted, float Min, float Max, List<string> TopFactors) Predict(float[] features)
        {
            if (features.Length != _featureNames.Length)
                throw new ArgumentException("输入特征数量与模型不一致");

            var inputTensor = new DenseTensor<float>(new[] { 1, features.Length });
            for (int i = 0; i < features.Length; i++)
                inputTensor[0, i] = features[i];

            var inputName = _session.InputMetadata.Keys.First();
             var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        };

            using var results = _session.Run(inputs);
            var predicted = results.First().AsEnumerable<float>().First();

            // 建议价格区间 ±15%
            float min = predicted * 0.85f;
            float max = predicted * 1.15f;

            // 取出前 3 个主要因素
            var topFactors = _importance.Take(3).Select(x => x.Feature).ToList();

            return (predicted, min, max, topFactors);
        }
    }
}
