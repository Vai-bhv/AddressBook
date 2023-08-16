using System;

namespace AddressBook
{
    public class Company : Contact
    {
        public Company(string name, string phoneNumber)
            : base(phoneNumber)
        {
            _name = name;
        }

        public override bool Matches(string term)
        {
            return _name.ToLower().StartsWith(term.ToLower());
        }

        public override string ToString()
        {
            return $"CMPANY: {_name} {base.ToString()}";
        }

        public string GetName()
        {
            return _name;
        }

        public override int CompareTo(Contact other)
        {
            Company company = other as Company;
            if (company != null)
            {
                return _name.CompareTo(company.GetName());
            }
            Person person = other as Person;
            if (person != null)
            {
                return _name.CompareTo(person.GetLastName());
            }
            return -1;
        }

        private string _name;
    }
}
