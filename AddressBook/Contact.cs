using System;

namespace AddressBook
{
    public abstract class Contact : IMatchable, IComparable<Contact>
    {
        public Contact(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
        }

        public abstract bool Matches(string term);

        public string GetPhoneNumber()
        {
            return _phoneNumber;
        }

        public override string ToString()
        {
            return _phoneNumber;
        }

        public abstract int CompareTo(Contact other);

        private string _phoneNumber;
    }
}
