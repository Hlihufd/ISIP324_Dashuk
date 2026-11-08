namespace ISIP324_Dashuk.Services
{
    public static class CreditCalculator
    {
        // 10% годовых
        public static double GetMonthly(double loan, int months) =>
            loan * (1 + 0.1 * months / 12) / months;
    }
}