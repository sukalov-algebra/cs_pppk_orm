using orm.sql;

namespace console
{
    internal class Program
    {
        public class Example1
        {
            public int pk_id { get; set; }
            public int fk_id { get; set; }
            public string name { get; set; }
            public bool exists { get; set; }
        }

        static void Main(string[] args)
        {
            string sql;
            Example1 example1 = new Example1 { pk_id = 1, fk_id = 1, exists = true, name = "something" };

            Console.WriteLine("Hello, World!");

            sql = SqlBuilder<Example1>.Where(x => x.fk_id == 1 && x.pk_id == 5);

            Console.WriteLine(sql);

            sql = SqlBuilder<Example1>.Insert(example1);

            Console.WriteLine(sql);

            sql = SqlBuilder<Example1>.Update(example1, x => x.fk_id == 1 && x.pk_id == 5);

            Console.WriteLine(sql);

            sql = SqlBuilder<Example1>.Delete(x => x.fk_id == 1 && x.pk_id == 5 );

            Console.WriteLine(sql);

            sql = SqlBuilder<Example1>.SelectCount();

            Console.WriteLine(sql);

            sql = SqlBuilder<Example1>.SelectAll();

            Console.WriteLine(sql);
        }
    }
}
