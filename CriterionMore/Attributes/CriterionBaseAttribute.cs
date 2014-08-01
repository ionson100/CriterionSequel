using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CriterionMore.Attributes
{
    /// <summary>
    /// Базовый атрибут 
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public abstract class CriterionBaseAttribute : Attribute
    {
        readonly DisplayAttribute _displayAttribute = new DisplayAttribute();

        /// <summary>
        /// Группировка значений ( не реализован)
        /// </summary>
        public string GroupName
        {
            get
            {
                return _displayAttribute.GetGroupName();
            }
            set
            {
                _displayAttribute.GroupName = value;
            }
        }

        /// <summary>
        /// Всплывающая подсказка на клиенте
        /// </summary>
        public string Description
        {
            get
            {
                return _displayAttribute.GetDescription();
            }
            set
            {
                _displayAttribute.Description = value;
            }
        }

        /// <summary>
        /// Название элемента поиска ( по умолчанию имеет название свойства типа, на котором применен атрибут)
        /// </summary>
        public string Name
        {
            get
            {
                return _displayAttribute.GetName();
            }
            set
            {
                _displayAttribute.Name = value;
            }
        }

        /// <summary>
        /// Ордер размещения,  не реализован  в фильтрах
        /// </summary>
        public int Order
        {
            get
            {
                var order = _displayAttribute.GetOrder();

                if (order != null) return (int) order;
                return 0;
            }
            set {  _displayAttribute.Order = value; }
        }

        /// <summary>
        /// Тип ресурса, для локализации приложения
        /// </summary>
        public Type ResourceType
        {
            get
            {
                return _displayAttribute.ResourceType;
            }
            set
            {
                _displayAttribute.ResourceType = value;
            }
        }

        /// <summary>
        /// Индентификатор, который на клиенте заменяется на id="",
        ///  ( по умолчанию имеет название свойства типа, на котором применен атрибут) в нижнем регистре
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Шаблон выражения для запроса, в каждом наследние переопределен
        /// </summary>
        public virtual string ExpressionTemplate { get; set; }

      

        internal PropertyInfo PropertyInfo { get; set; }



        internal List<CriterionHtmlAttributeAttribute> HtmlAttributes =new List<CriterionHtmlAttributeAttribute>();


    }
}


