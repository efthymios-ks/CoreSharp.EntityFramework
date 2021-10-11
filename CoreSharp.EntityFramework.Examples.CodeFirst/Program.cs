using CoreSharp.EntityFramework.Examples.CodeFirst.Database;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Examples.CodeFirst
{
    internal static class Program
    {
        //Methods 
        private static async Task Main()
        {
            using var schoolDb = new SchoolDbContext();

            await Task.CompletedTask;
        }
    }
}
