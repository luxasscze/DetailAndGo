namespace DetailAndGoAdmin
{
    public class Utility
    {
        public string GetGreetings()
        {
            if(DateTime.Now.Hour > 0 && DateTime.Now.Hour < 6)
            {
                return "Night night!";
            }
            else if(DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 12)
            {
                return "Good morning!";
            }
            else if(DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 17)
            {
                return "Good afternoon!";
            }
            else
            {
                return "Good evening!";
            }
        }

        public async Task<string[]> GetRandomQuote()
        {
            Random random = new Random();
            string[] quotes = await File.ReadAllLinesAsync("Quotes.txt");            
            string selectedQuote = quotes[random.Next(0, quotes.Length - 1)];
            string[] result = selectedQuote.Split(';');
            return result;
        }
    }
}
