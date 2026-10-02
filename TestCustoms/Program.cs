using DataCustoms;
using DataCustoms.ConsoleUtils;

namespace TestCustoms
{
    public class Program : DataMain
    {
        public static void Main(string[] args)
        {
            SelectorMenu menu = new SelectorMenu("This is a menu", "first option").
                AddOption("deltarune")
                .AddOption("this is an option for some reason");
            int response;
            do
            {
                response = menu.Select();
                Logger.LogInfo(response);
            }
            while (response != 0);
        }
    }
}