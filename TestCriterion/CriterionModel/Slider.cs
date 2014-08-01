using CriterionMore;

namespace TestCriterion.CriterionModel
{
    public class Slider : ISliderData
    {
        public decimal GetMax()
        {
            return 1000;
        }

        public decimal GetMin()
        {
            return 0;
        }
        public string Name { get; set; }
    }
}