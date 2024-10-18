namespace BudetTracker
{
    internal class Program
    {
        static void Main(string[] args) {
            string? value = Console.ReadLine();
            if (value != null)
            {
                Console.WriteLine(value);
            }
            else
            {
                Console.WriteLine("Value was null");
            }
        }
    }
}