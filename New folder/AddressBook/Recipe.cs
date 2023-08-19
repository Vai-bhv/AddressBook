namespace AddressBook
{
    public class Recipe : IMatchable
    {
        public Recipe(string title)
        {
            _title = title;
        }

        public bool Matches(string term)
        {
            return _title.ToLower().Contains(term.ToLower());
        }

        public override string ToString()
        {
            return $"RECIPE: {_title}";
        }

        private string _title;
    }
}
