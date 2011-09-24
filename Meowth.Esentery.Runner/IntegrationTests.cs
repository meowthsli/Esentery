using System;
using System.Text;
using Meowth.Esentery.Core;
using Meowth.Esentery.Querying;
using Meowth.Esentery.Extensions;

namespace Meowth.Esentery.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var engine = new Engine("instance", new EngineOptions()))
            {
                using (var session = engine.OpenSession())
                using (var db = session.CreateDatabase("edbtest.db"))
                {
                    Create(db);
                }

                using (var session = engine.OpenSession())
                using (var db = session.OpenDatabase("edbtest.db"))
                {
                    Test(db);
                }
            }
        }

        private static void AssertRC(int expected, ICursor cursor, string message = "")
        {
            var actual = 0;
            while (cursor.MoveNext())
                actual++;

            var ok = (expected == actual);
            Console.WriteLine(ok ? "SUCCES {2} expected {0}, actual {1}" : "FAILED {2} expected {0}, actual {1}",
                              expected, actual, message);

            Total++;
            if (ok)
                Succeeded++;
        }

        private static int Total;
        private static int Succeeded;

        private static void Create(Database db)
        {
            const string TABLE = "table";
            const string COLUMN = "column";
            const string COLUMN2 = "column2";
            const string COLUMN3 = "column3";
            const string COLUMN4 = "int_column";

            using (var tx = db.Session.BeginTransaction())
            using (var table = db.CreateTable(TABLE))
            {
                var col1 = table.AddColumn<string>(COLUMN, 
                    new ColumnOptions { Encoding = Encoding.Unicode, Length = 200 });

                var col2 = table.AddColumn<string>(COLUMN2,
                    new ColumnOptions { Encoding = Encoding.Unicode, Length = 200 });

                table.AddColumn<string>(COLUMN3, 
                    new ColumnOptions { Encoding = Encoding.Unicode, Length = 200 });

                var col4 = table.AddColumn<int>(COLUMN4, new ColumnOptions { ColumnType = typeof(int), IsNullable = false});
                table.AddSearchIndex(COLUMN, col1);
                table.AddSearchIndex(COLUMN2, col2);
                table.AddSearchIndex(COLUMN4, col4);

                tx.Commit();
            }
        }

        static void Test(Database db)
        {
            const string TABLE = "table";
            const string COLUMN = "column";
            const string COLUMN2 = "column2";
            const string COLUMN3 = "column3";
            const string COLUMN4 = "int_column";

            const string message2 = "выаыаваыва";

            using (var tx = db.Session.BeginTransaction())
            using (var table = db.OpenTable(TABLE))
            using (var cursor = table.OpenPrimaryCursor())
            {
                const string message = "Hello world";
                const string msg3 = "msg3";

                var column1 = table.GetColumn<string>(COLUMN);
                var column2 = table.GetColumn<string>(COLUMN2);
                var column3 = table.GetColumn<string>(COLUMN3);
                var column4 = table.GetColumn<int>(COLUMN4);

                using (var ins = cursor.AddRow())
                {
                    ins.SetValue(column1, message);
                    ins.SetValue(column2, message2);
                    ins.SetValue(column3, msg3);
                    ins.SetValue(column4, 1);
                    ins.Save();
                }

                using (var ins = cursor.AddRow())
                {
                    ins.SetValue(column1, msg3);
                    ins.SetValue(column2, message2);
                    ins.SetValue(column3, msg3);
                    ins.SetValue(column4, 4);
                    ins.Save();
                }

                tx.Commit();
            }

            Console.WriteLine(COLUMN + " : " + COLUMN2 + " : " + COLUMN3);

            using (var table = db.OpenTable(TABLE))
            {
                var pf = new PredicateFactory(table);

                using (var cursor = table.OpenPrimaryCursor())
                    AssertRC(2, cursor, "Simply enum rows");

                using (var cursor = table.OpenCursor(new Eq<string>(table.GetIndex<string>(COLUMN), "msg3")))
                    AssertRC(1, cursor, "Find where column1 = msg3");

                using (var cursor = table.OpenCursor(pf.Eq(COLUMN, "msg3")))
                    AssertRC(1, cursor, "Find where column1 = msg3");
                
                using (var cursor = table.OpenCursor(pf.Eq(COLUMN2, message2)))
                    AssertRC(2, cursor, "Find where column2 = message2");
                
                using (var cursor = table.OpenCursor(new StartsWith(table.GetIndex<string>(COLUMN), "Hell")))
                    AssertRC(1, cursor, "Find where column1 LIKE Hell");

                var predicate = new And(
                    pf.StartsWith(COLUMN, "ms"),
                    pf.Eq(COLUMN2, message2)
                    );

                using (var cursor = table.OpenCursor(predicate))
                    AssertRC(1, cursor, "Find where column1 = ms AND column2 = message2");

                using (var cursor = table.OpenCursor(pf.Between(COLUMN, "zzzs", "msg/", false, true)))
                    AssertRC(1, cursor, "Find column1 between msg/ AND zzzs");

                using (var cursor = table.OpenCursor(new Eq<int>(table.GetIndex<int>(COLUMN4), 4)))
                    AssertRC(1, cursor, "Find column4 = 4");

                using (var cursor = table.OpenCursor(new Eq<int>(table.GetIndex<int>(COLUMN4), 6)))
                    AssertRC(0, cursor, "Find column4 = 6");

                using (var cursor = table.OpenCursor(pf.Le(COLUMN4, 3)))
                    AssertRC(1, cursor, "Find column4 < 3");

                using (var cursor = table.OpenCursor(pf.Le(COLUMN4, 3)))
                {
                    cursor.MoveNext();

                    using (var upd = cursor.EditRow())
                    {
                        upd.SetValue(table.GetColumn<int>(COLUMN4), 6);
                        upd.Save();
                    }

                }
                using (var c2 = table.OpenCursor(pf.Le(COLUMN4, 3)))
                    AssertRC(0, c2, "Find column4 < 3");
            }

            Console.WriteLine("---");
            Console.WriteLine("Succeeded {0} out of {1}", Succeeded, Total);
        }
    }
}
