using System;

namespace CriterionMore.Attributes
{
    /// <summary>
    /// Атрибут типа,  делает привязку шаблона для показа к типу ( привязка указывается в виде url)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CriterionTemplateAttribute : Attribute
    {
        /// <summary>
        /// Url шаблона
        /// </summary>
        public string Url { get; set; }
    }
}