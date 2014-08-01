using System;

namespace CriterionMore.Attributes
{
    /// <summary>
    /// Атрибут для прказа элемента поиска в виде листа
    /// </summary>
    public class CriterionListAttribute : CriterionBaseAttribute
    {
        /// <summary>
        /// Мульти выбор
        /// </summary>
        public bool IsMultiple { get; set; }

        /// <summary>
        /// Размер  для листа по вертикали
        /// </summary>
        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <summary>
        /// Тип который ответственнен за загруску значений списка, должен реализовывать интерфейс IListItem
        /// так же должен иметь конструктор по умолчанию
        /// </summary>
        public Type IListItem { get; set; }

        private string _expressionTemplate;
        private int _size=4;

        /// <summary>
        /// Шаблон выражения для запроса, по умолчанию : string.Format(" {0} = @0", PropertyInfo.Name)
        /// </summary>
        public override string ExpressionTemplate
        {
            get
            {
                return string.IsNullOrWhiteSpace(_expressionTemplate) ? string.Format(" {0} = @0 ", PropertyInfo.Name) : _expressionTemplate;
            }
            set { _expressionTemplate = value; }
        }
    }
}