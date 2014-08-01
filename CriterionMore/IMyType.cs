using System;
using System.Linq.Expressions;
using CriterionMore.Attributes;

namespace CriterionMore
{
    /// <summary>
    /// Интерфейс который должен реализовывать тип поставшик  готовой html разметки, включая javascript, тип должен имет конструктор по умолчанию
    /// </summary>
    public interface IMyType
    {
        /// <summary>
        /// Проверка данных с клиента (  в случае не успеха, создать исключение)
        /// </summary>
        /// <param name="data">строка с клиента, данные с массива формс</param>
        /// <param name="atr">Заполненный атрибут для поля</param>
        void Validate(string data, CriterionMyControlAttribute atr);

        /// <summary>
        /// Атрибут уже заполненный
        /// </summary>
        /// <param name="atr">Заполненный атрибут для поля</param>
        /// <param name="formValue">данные с клиента в виде строки</param>
        /// <returns></returns>
        string Rendering(CriterionMyControlAttribute atr, string formValue);


        /// <summary>
        ///Получение выражение для поиска
        /// </summary>
        /// <param name="formData">Данные с клиента</param>
        /// <returns>Expression </returns>
        Expression<Func<T,bool>>  GetExpression<T>(string formData);
    }
}