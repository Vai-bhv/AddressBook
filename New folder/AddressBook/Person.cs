using System;

namespace AddressBook
{
    public class Person : Contact
    {
        public Person(string lastName, string phoneNumber)
            : this(null, lastName, phoneNumber)
        {
        }

        public Person(string firstName, string lastName, string phoneNumber)
            : base(phoneNumber)
        {
            _lastName = lastName;
            _firstName = firstName;
        }

        public override bool Matches(string term)
        {
            return _lastName.ToLower().StartsWith(term.ToLower()) ||
                   _firstName.ToLower().StartsWith(term.ToLower());
        }

        public override string ToString()
        {
            return $"PERSON: {_lastName}, {_firstName} {base.ToString()}";
        }

        public string GetFirstName()
        {
            return _firstName;
        }

        public string GetLastName()
        {
            return _lastName;
        }

        public override int CompareTo(Contact other)
        {
            Company company = other as Company;
            if (company != null)
            {
                return _lastName.CompareTo(company.GetName());
            }
            Person person = other as Person;
            if (person != null)
            {
                int lastNameCompare = _lastName.CompareTo(person.GetLastName());
                if (lastNameCompare == 0)
                {
                    return _firstName.CompareTo(person.GetFirstName());
                }
                return lastNameCompare;
            }
            return -1;
        }

        private string _lastName;
        private string _firstName;
    }
}
