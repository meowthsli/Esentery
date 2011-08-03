﻿using System;
using System.Linq;
using System.Text;
using Meowth.Esentery.Core;
using Meowth.Esentery.Querying;
using Microsoft.Isam.Esent.Interop;
using Meowth.Esentery.Additions;
using Session = Meowth.Esentery.Core.Session;

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

        private static void Create(Database db)
        {
            const string TABLE = "table";
            const string COLUMN = "column";
            const string COLUMN2 = "column2";
            const string COLUMN3 = "column3";
            using (var tx = db.Session.BeginTransaction())
            using (var table = db.CreateTable(TABLE))
            {
                var col1 = table.AddColumn<string>(COLUMN, 
                    new ColumnOptions { Encoding = Encoding.Unicode, Length = 200 });

                var col2 = table.AddColumn<string>(COLUMN2,
                    new ColumnOptions { Encoding = Encoding.Unicode, Length = 200 });

                table.AddColumn<string>(COLUMN3, 
                    new ColumnOptions { Encoding = Encoding.Unicode, Length = 200 });

                table.AddColumn<int>("int_columns", new ColumnOptions { ColumnType = typeof(int), IsNullable = false});
                table.AddSearchIndex(COLUMN, col1);
                table.AddSearchIndex(COLUMN2, col2);

                tx.Commit();
            }
        }

        static void Assert<T>(T left, T right)
            where T : IEquatable<T>
        {
            if (!left.Equals(right))
                throw new ArgumentException();
        }

        static void Test(Database db)
        {
            const string TABLE = "table";
            const string COLUMN = "column";
            const string COLUMN2 = "column2";
            const string COLUMN3 = "column3";

            const string message2 = "выаыаваыва";

            using (var tx = db.Session.BeginTransaction())
            using (var table = db.OpenTable(TABLE))
            using (var cursor = table.OpenPrimaryCursor())
            {
                const string message = "Hello world";
                const string msg3 = "msg3";

                var column1 = (Column<string>)table.GetColumns().First(x => x.ColumnName == COLUMN);
                var column2 = (Column<string>)table.GetColumns().First(x => x.ColumnName == COLUMN2);
                var column3 = (Column<string>)table.GetColumns().First(x => x.ColumnName == COLUMN3);

                using (var ins = cursor.AddRow())
                {
                    ins.SetField(column1, message);
                    ins.SetField(column2, message2);
                    ins.SetField(column3, msg3);
                    ins.Save();
                }

                using (var ins = cursor.AddRow())
                {
                    ins.SetField(column1, msg3);
                    ins.SetField(column2, message2);
                    ins.SetField(column3, msg3);
                    ins.Save();
                }

                tx.Commit();
            }

            Console.WriteLine(COLUMN + " : " + COLUMN2 + " : " + COLUMN3);

            using (var table = db.OpenTable(TABLE))
            using (var cursor = table.OpenPrimaryCursor())
                while (cursor.MoveNext())
                    Console.WriteLine(cursor.GetString(COLUMN) + " | " + cursor.GetString(COLUMN2) + " | " + cursor.GetString(COLUMN3));

            Console.WriteLine("2.........\n\n");

            using (var table = db.OpenTable(TABLE))
            using (var cursor = table.OpenCursor(new Eq<string>(table.GetIndex<string>(COLUMN), "msg3")))
            {
                while (cursor.MoveNext())
                    Console.WriteLine(cursor.GetString(COLUMN) + " | " + cursor.GetString(COLUMN2) + " | " +
                                        cursor.GetString(COLUMN3));
            }

            Console.WriteLine("1.........\n\n");

            using (var table = db.OpenTable(TABLE))
            using (var cursor = table.OpenCursor(new Eq<string>(table.GetIndex<string>(COLUMN2), message2)))
            {
                while (cursor.MoveNext())
                    Console.WriteLine(cursor.GetString(COLUMN) + " | " + cursor.GetString(COLUMN2) + " | " +
                                        cursor.GetString(COLUMN3));
            }

            Console.WriteLine("2.........\n\n");

            using (var table = db.OpenTable(TABLE))
            using (var cursor = table.OpenCursor(new StartsWith(table.GetIndex<string>(COLUMN), "Hell")))
                while (cursor.MoveNext())

                    Console.WriteLine(cursor.GetString(COLUMN) + " | " + cursor.GetString(COLUMN2) + " | " +
                                        cursor.GetString(COLUMN3));

            Console.WriteLine("1.........\n\n");

            using (var table = db.OpenTable(TABLE))
            using (var cursor = table.OpenCursor(
                new And(
                    new StartsWith(table.GetIndex<string>(COLUMN), "ms"),
                    new Eq<string>(table.GetIndex<string>(COLUMN2), message2)
                    )))
                while (cursor.MoveNext())
                    Console.WriteLine(cursor.GetString(COLUMN) + " | " + cursor.GetString(COLUMN2) + " | " +
                                        cursor.GetString(COLUMN3));

            Console.WriteLine("1.........\n\n");

            using (var table = db.OpenTable(TABLE))
            using (var cursor = table.OpenCursor(
                    new Between<string>(table.GetIndex<string>(COLUMN),
                        "zzzs", false,
                        "msg/", true)))
                while (cursor.MoveNext())
                    Console.WriteLine(cursor.GetString(COLUMN) + " | " + cursor.GetString(COLUMN2) + " | " +
                                        cursor.GetString(COLUMN3));

            Console.WriteLine("1.........\n\n");
        }
    }
}
