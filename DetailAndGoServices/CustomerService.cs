using DetailAndGoServices.DAL;
using DetailAndGoServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetailAndGoServices
{
    public class CustomerService
    {
        ContentContext _context;

        public CustomerService(ContentContext context = null)
        {
            _context = context;
        }

        public Customer Test()
        {
            Customer customer = _context.Customers.First();
            return customer;
        }
    }
}
