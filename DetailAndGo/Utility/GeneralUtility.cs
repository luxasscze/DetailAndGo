using DetailAndGo.Models;

namespace DetailAndGo.Utility
{
    public class GeneralUtility
    {
        public decimal ConvertPriceToDecimal(long input)
        {
            return Math.Round(Convert.ToDecimal(input) / 100, 2);
        }

        public string GetServiceNamesForEmail(List<Service> services, Booking booking)
        {                                                                               
            string output = String.Empty;
            
            if(services == null)
            {
                return "No Services";
            }

            foreach(Service service in services)
            {
                if(booking.SubServicesArray != null && booking.SubServicesArray.Contains(service.Id.ToString()))
                {
                    output += "▪ " + service.Name + " <b style=\"color: #ffc107;\">[with addons]</b>" + "<br />";
                }              
                else
                {
                    output += "▪ " + service.Name + "<br />";
                }
            }

            return output;
        }
    }
}
