using System;
using System.Collections.Generic;
using System.Reflection;

namespace CriterionMore.Attributes
{
    /// <summary>
    /// Добавляет пользовательский атрибут в элемент
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class CriterionMyControlAttribute :CriterionBaseAttribute
    {
        internal Type MyType { get; set; }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="myType"></param>
        public CriterionMyControlAttribute(Type myType)
        {
            MyType = myType;
        }

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

        /// <summary>
        /// PropertyInfo выбранного свойства
        /// </summary>
        public PropertyInfo MyPropertyInfo
        {
            get
            {
                return base.PropertyInfo;
            }
           
        }



        /// <summary>
        /// Список атрибутов для формирования HTML атрибутов у свойства
        /// </summary>
        public List<CriterionHtmlAttributeAttribute> MyHtmlAttributes
        {
            get
            {
                return HtmlAttributes;
            }
        }
    }
}