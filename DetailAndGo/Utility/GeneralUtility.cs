namespace DetailAndGo.Utility
{
    public class GeneralUtility
    {
        public decimal ConvertPriceToDecimal(long input)
        {
            return Math.Round(Convert.ToDecimal(input) / 100, 2);
        }
    }
}
