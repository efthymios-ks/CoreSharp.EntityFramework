namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain
{
    internal static class Configuration
    {
        public static string ConnectionString => @" Data Source=.\SQLEXPRESS; 
                                                    Initial Catalog=SchoolDB; 
                                                    Integrated Security=true; 
                                                    MultipleActiveResultSets=true;";
    }
}
