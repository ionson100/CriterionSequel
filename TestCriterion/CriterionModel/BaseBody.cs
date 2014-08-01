using System;
using CriterionMore.Attributes;

namespace TestCriterion.CriterionModel
{
    [CriterionTemplate(Url = "~/TemplateCriterion/BaseBody.html")]
    public class BaseBody
    {
        public string IdBase { get; set; }
        [CriterionList(Name = "Изготовитель", Size = 6, IListItem = typeof(BodyListItem))]
        public string MadeInBase { get; set; }


        [CriterionRadioButton(Name = "Тип движения", ListItem = typeof(BodyListItem))]
        public int RadioBase { get; set; }

        [CriterionCheckBox(Name = "Тип движения", ListItem = typeof(BodyListItem))]
        public int CheckBoxBase { get; set; }

        [CriterionDropDown(Name = "Выпадающий список", IListItem = typeof(BodyListItem))]
        public int? DropBase { get; set; }

        [CriterionBetweenDate(Name = "По диапазону дат")]
        public DateTime DateTimeBase { get; set; }

        [CriterionSlider(Name = "Диапазон цен (руб.)",ISliderData = typeof(Slider))]
        public int PriceBase { get; set; }
    }
}