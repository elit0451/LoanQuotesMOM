using System;
using System.Collections.Generic;
using System.Text;

namespace LoanQuotesMOMClient
{
    public class User
    {
        public Guid Id { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}
