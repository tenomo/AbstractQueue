using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AbstractQueue;

namespace AbstractQueueUnitTests.Mocks
{
    class NumberCalculateExecuter : AbstractTaskExecuter
    {
        public static List<ExecutionResult> resultList = new List<ExecutionResult>();
        public override void Execute(QueueTask queueTask)
        {
            Calculator(queueTask);

        }


        public static void Calculator(QueueTask queueTask)
        {
            switch (queueTask.Type)
            {
                case (int)CaluculationType.Add:
                    var addBody = Newtonsoft.Json.JsonConvert.DeserializeObject<NumberWraper2D>(queueTask.Body);
                    resultList.Add(new ExecutionResult()
                    {
                        Result = (addBody.X + addBody.Y).ToString(),
                        task = queueTask,
                        Type = CaluculationType.Add
                    });
                    break;
                case (int)CaluculationType.Diff:
                    var difBody = Newtonsoft.Json.JsonConvert.DeserializeObject<NumberWraper2D>(queueTask.Body);
                    resultList.Add(new ExecutionResult()
                    {
                        Result = (difBody.X - difBody.Y).ToString(),
                        task = queueTask,
                        Type = CaluculationType.Diff
                    });
                    break;
                case (int)CaluculationType.Divide:
                    var divideBody = Newtonsoft.Json.JsonConvert.DeserializeObject<NumberWraper2D>(queueTask.Body);
                    resultList.Add(new ExecutionResult()
                    {
                        Result = (divideBody.X / divideBody.Y).ToString(),
                        task = queueTask,
                        Type = CaluculationType.Divide
                    });
                    break;
                case (int)CaluculationType.Multipty:
                    var multiptyBody = Newtonsoft.Json.JsonConvert.DeserializeObject<NumberWraper2D>(queueTask.Body);
                    if (multiptyBody.X == 0 || multiptyBody.Y == 0)
                        throw new InvalidOperationException("X and Y can not be 0");
                    resultList.Add(new ExecutionResult()
                    {
                        Result = (multiptyBody.X * multiptyBody.Y).ToString(),
                        task = queueTask,
                        Type = CaluculationType.Multipty
                    });
                    break;
                case (int)CaluculationType.Sqrt:
                    var SqrtBody = Newtonsoft.Json.JsonConvert.DeserializeObject<NumberWraper1D>(queueTask.Body);
                    if (SqrtBody.X == 0  )
                        throw new InvalidOperationException("X can not be 0");
                    resultList.Add(new ExecutionResult()
                    {
                        Result = Math.Sqrt(SqrtBody.X).ToString(),
                        task = queueTask,
                        Type = CaluculationType.Sqrt
                    });
                    break;
                case (int)CaluculationType.Factorial:
                    var facBody = Newtonsoft.Json.JsonConvert.DeserializeObject<NumberWraper1D>(queueTask.Body);
                    if (facBody.X == 0)
                        throw new InvalidOperationException("X can not be 0");

                    double facRes = facBody.X < 0
                        ? -1
                        : facBody.X == 0 || facBody.X == 1
                            ? 1
                            : Enumerable.Range(1, Convert.ToInt32(facBody.X))
                                .Aggregate((counter, value) => counter * value);

                    resultList.Add(new ExecutionResult()
                    {
                        Result = facRes.ToString(),
                    task = queueTask,
                        Type = CaluculationType.Factorial
                    });
                    break;
                    Thread.Sleep(3000);
            }
        }
    }

    [Serializable]
    public class NumberWraper2D : NumberWraper1D,  ICloneable
    { 
        public double Y { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    [Serializable]
    public class NumberWraper1D :   ICloneable
    {
        public double X { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    [Serializable]
    class ExecutionResult : ICloneable
    {
        public QueueTask task { get; set; }
        public string Result { get; set; }
        public CaluculationType Type { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }


    [Serializable]
    public enum CaluculationType
    {
        Add,
        Diff,
        Divide,
        Multipty,
        Sqrt,
        Factorial
    }
}
