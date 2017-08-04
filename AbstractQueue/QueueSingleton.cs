using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AbstractQueue
{
    class QueueSingleton
    {
        /// Защищённый конструктор необходим для того, чтобы предотвратить создание экземпляра класса Singleton. 
        /// Он будет вызван из закрытого конструктора наследственного класса.
        protected QueueSingleton()
        {
        }

        /// Фабрика используется для отложенной инициализации экземпляра класса
        private sealed class QueueSingletonCreator
        {
            //Используется Reflection для создания экземпляра класса без публичного конструктора
            private static readonly Queue instance = (Queue) typeof(Queue).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new Type[0],
                new ParameterModifier[0]).Invoke(null);

            public static Queue CreatorInstance
            {
                get { return instance; }
            }
        }

        
        public static Queue Instance => QueueSingletonCreator.CreatorInstance;
    }
}
 
