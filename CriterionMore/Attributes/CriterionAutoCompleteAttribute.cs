using System;

namespace CriterionMore.Attributes
{
    /// <summary>
    /// Атрибут для формирования поля ввода автозаполнения
    /// </summary>
     [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class CriterionAutoCompleteAttribute : CriterionBaseAttribute
    {
        /// <summary>
        /// Адрес ресурса для ajax запроса
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Количество миллисекунд, которое должно пройти после нажатия очередной клавиши, чтобы активизировался запрос – значение по умолчанию 300.
        /// </summary>
        public int Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }


        /// <summary>
        /// Количество символов, которое должно быть введено в поле ввода прежде, чем активизируются подсказки. – значение по умолчанию 1
        /// </summary>
        public int MinLength
        {
            get { return _minLength; }
            set { _minLength = value; }
        }

        private string _expressionTemplate;
        private int _minLength=1;
        private int _delay=300;

        /// <summary>
        /// Шаблон выражения для запроса, по умолчанию : string.Format(" {0} = @0", PropertyInfo.Name)
        /// </summary>
        public override string ExpressionTemplate
        {
            ////
         
            get
            { 
                return string.IsNullOrWhiteSpace(_expressionTemplate) ? string.Format(" {0} = @0", PropertyInfo.Name) : _expressionTemplate;
            }
            set { _expressionTemplate = value; }
        }
    }
}