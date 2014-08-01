using System;

namespace CriterionMore.Attributes
{

    /// <summary>
    /// Атрибут для показа элемента выбора в виде слайдера, шаблон выражения не используется ( встроенный)
    /// </summary>
    public class CriterionSliderAttribute : CriterionBaseAttribute
    {
        /// <summary>
        /// Шаг слайдера
        /// </summary>
        public float Step { get; set; }

        /// <summary>
        /// Тип который отвечает за поставку максимального и минимально значения для слайдера, должен реализовывать
        /// интрефейс ISliderData
        /// </summary>
        public Type ISliderData { get; set; }
          
           
    }
}