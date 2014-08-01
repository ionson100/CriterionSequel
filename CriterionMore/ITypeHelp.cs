using Microsoft.Win32;

namespace CriterionMore
{
    /// <summary>
   /// Объект для поставки данных справки должен реализовать этот интерфейс
   /// </summary>
   public  interface ITypeHelp
    {
       /// <summary>
       ///Получение данных для справки
       /// </summary>
       /// <param name="id"> Клиентский индентификатор</param>
       /// <returns>Данные для отображения справки</returns>
       string GetHelp(string id);
    }
}