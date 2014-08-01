using System;
using System.Threading;
using System.Web.Mvc;

namespace CriterionMore
{
    /// <summary>
    /// Встроенный поставшик данных для справки
    /// </summary>
    public class HelpController : Controller
    {
        private static readonly Lazy<byte[]> Bytes = new Lazy<byte[]>
            (
            () => (byte[])new System.Drawing.ImageConverter().ConvertTo(Properties.Resources.help,
                typeof(byte[])), LazyThreadSafetyMode.ExecutionAndPublication
                );

        /// <summary>
        /// Иконка хелп
        /// </summary>
        /// <param name="id"> индентификатор запроса</param>
        /// <returns></returns>
        public ActionResult Index(string id)
        {
           // /ss/Help/Index/23
            return File(Bytes.Value, "image/png");

        }
        /// <summary>
        /// Текст справки
        /// </summary>
        /// <param name="id">индентификатор запроса</param>
        /// <returns></returns>
        public string Text(string id)
        {
            var inctans = (ITypeHelp)Activator.CreateInstance(CriterionActivator.TypeHelp);
            return "индентификатор запроса   " + inctans.GetHelp(id);

        }
    }
}
