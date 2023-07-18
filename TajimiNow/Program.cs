namespace TajimiNow
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task.WaitAll(Jobs.RunAmedas(), Jobs.RunDaily());
        }
    }
}