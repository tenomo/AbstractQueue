using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractQueue;

namespace AbstractQueueUnitTests.Mocks
{
    class NumberCalculateExecuter : AbstractTaskExecuter
    {
        public override void Execute(QueueTask queueTask)
        {
            switch (queueTask.Type)
            {
                case (int) CaluculationType.Add:
                 var body =   Newtonsoft.Json.JsonConvert.DeserializeObject<NumberWraper2D>(queueTask.Body);

                    break;
                case (int) CaluculationType.Diff:
                    break;
                case (int) CaluculationType.Divide:
                    break;
                case (int) CaluculationType.Multipty:
                    break;
                case (int) CaluculationType.Sqrt:
                    break;
                case (int) CaluculationType.Factorial:
                    break;
            }

        }
    }

    public class NumberWraper2D
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class NumberWraper1D
    {
        public double X { get; set; }
    }

    

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
