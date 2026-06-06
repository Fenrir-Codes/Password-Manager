using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordManager.Models
{
    public class PasswordEntry
    {
        public int Id { get; set; }

        public string Value { get; set; } = "";

        public string Password { get; set; } = "";
    }
}
