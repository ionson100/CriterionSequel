using System;

namespace CriterionMore.Attributes
{
    /// <summary>
    /// Атрибут для показа чек листа
    /// область размещения таблица,
    /// </summary>
    public class CriterionCheckBoxAttribute : CriterionBaseAttribute
    {
        /// <summary>
        /// Веритикальное или горизоньальное размещение списка
        /// </summary>
        public bool IsHorizontally { get; set; }

        /// <summary>
        /// Тип который ответственнен за загруску значений списка, должен реализовывать интерфейс IListItem
        /// так же должен иметь конструктор по умолчанию
        /// </summary>
        public Type ListItem { get; set; }

        private string _expressionTemplate;
      
        /// <summary>
        /// Шаблон выражения для запроса, по умолчанию : string.Format(" {0} = @0", PropertyInfo.Name)
        /// </summary>
        public override string ExpressionTemplate
        {
            get
            {
                return string.IsNullOrWhiteSpace(_expressionTemplate) ? string.Format(" {0} = @0", PropertyInfo.Name) : _expressionTemplate;
            }
            set { _expressionTemplate = value; }
        }

    }
}