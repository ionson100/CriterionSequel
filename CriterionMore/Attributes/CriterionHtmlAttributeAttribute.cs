using System;

namespace CriterionMore.Attributes
{
    /// <summary>
    /// Добавляет пользовательский атрибут в элемент
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class CriterionHtmlAttributeAttribute : Attribute
    {
        /// <summary>
        /// Название, имя атрибута
        /// </summary>
        public  string Key { get; private set; } 

        /// <summary>
        /// Значение атрибута
        /// </summary>
        public  string Value { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="key">Ключ имя атрибута</param>
        /// <param name="value">Значение атрибута</param>
        public CriterionHtmlAttributeAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}