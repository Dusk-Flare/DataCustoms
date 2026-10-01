namespace DataCustoms.ConsoleUtils
{
    public class SelectorMenu
    {
        public List<string> Options = [];
        public string Header;
        public SelectorMenu(string header, string defaultOption)
        {
            Header = header;
            Options.Add(defaultOption);
        }

        public SelectorMenu AddOption(string key)
        {
            Options.Add(key);
            return this;
        }

        public int Select()
        {
            Console.WriteLine(Header);
            for (int i = 0; i < Options.Count; i++) Console.WriteLine($"{i} - {Options[i]}");
            int value;
            while (!Reader.TryReadNumber("Select an option:", out value))
            {
                Console.WriteLine("Invalid selection.");
            }
            if (value < 0 || value >= Options.Count)
            {
                Console.WriteLine($"Value {value} outside option bounds, defaulting to 0"); 
                return 0;
            }
            return value;
        }
    }
}
