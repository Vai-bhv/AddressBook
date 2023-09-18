namespace AddressBook
{
    public class Recipe : Contact
    {
        public Recipe(string title) : base(null)
        {
            _title = title;
        }

        public override bool Matches(string term)
        {
            return _title.ToLower().Contains(term.ToLower());
        }

        public override string ToString()
        {
            return $"RECIPE: {_title}";
        }
        public string GetTitle() {
            return _title;
        }
        public override int CompareTo(Contact other) {
            Recipe otherRecipe = other as Recipe;
            if(otherRecipe != null) {
                return _title.CompareTo(otherRecipe.GetTitle());
            }
            return -1;
        }
        private string _title;
    }
}
