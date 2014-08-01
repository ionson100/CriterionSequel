using System;
using System.Diagnostics;
using System.Linq.Expressions;
using CriterionMore;
using CriterionMore.Attributes;

namespace TestCriterion.CriterionModel
{
    [CriterionTemplate(Url = "~/TemplateCriterion/Body.html")]
    public class Body : BaseBody
    {
      

        [CriterionAutoCompleteAttribute(Name = "Имя",Id = "rghfghew",Url = "Home/Auto/")]
        public string Name { get; set; }

        [CriterionHtmlAttribute("data-foo", "sd")]

       
        [CriterionList(Id = "assa", Name = "Изготовитель", Size = 6, IListItem = typeof(BodyListItem), IsMultiple = true)]
        public int? MadeIn { get; set; }

        [CriterionRadioButton(Name = "Тип движения ", ListItem = typeof(BodyListItem))]
        public int? Radio { get; set; }

       
        [CriterionCheckBox(Name = "Тип движения ", ListItem = typeof(BodyListItem))]
        public int? CheckBox { get; set; }



        [CriterionDropDown(Name = "Выпадающий список", IListItem = typeof(BodyListItem))]
        public int? Drop { get; set; }
       
        
        
        [CriterionBetweenDate(Name = "По диапазону дат")]
        public DateTime DateTimeBody { get; set; }

        [CriterionSlider(Name = "Диапазон цен (руб.)", ISliderData = typeof(Slider))]
        public int Price { get; set; }

        [CriterionMyControl(typeof(MyControlBase), Name = "Мой тип")]
        public string MyControl { get; set; }
    }

    public class MyControlBase : IMyType
    {

        public void Validate(string data, CriterionMyControlAttribute atr)
        {
            Debug.WriteLine(data);
        }

        public string Rendering(CriterionMyControlAttribute atr, string formValue)
        {
            string ss = "<div class=\"item cr\">" +
                      "<div class=\"labellist\">" + atr.Name +
                      "</div>" +
                      "<input type=\"text\" name=\"" + atr.Id + "\" data-criterion=\"1\" value=\"" + formValue + "\" id=\"" + atr.Id + "\" />" +
                        "</div>";
            return ss;
        }

        public Expression<Func<T, bool>> GetExpression<T>(string formData)
        {
            var e = System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(" MyControl = @0", formData);
            return e;
        }
    }
}