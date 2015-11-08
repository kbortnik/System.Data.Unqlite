using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.Unqlite.Tests
{
    [TestClass]
    public class Jx9Tests
    {
        private const string databaseName = ":mem:";

        [TestMethod]
        public void Test_Jx9_Compile()
        {
            var db = UnqliteDB.Create();
            var res = db.Open(databaseName, Unqlite_Open.CREATE);
            if (res)
            {
                var compileJx9 = db.CompileJx9("[{name: 'Test'}]");
                Assert.AreEqual(true, compileJx9);
                db.Close();
            }
        }

        [TestMethod]
        public void Test_Store()
        {
            var db = UnqliteDB.Create();
            var res = db.Open(databaseName, Unqlite_Open.CREATE);
            if (res)
            {
                var x = File.ReadAllText("../../jx9testfiles/store.jx9");

                var sb = new StringBuilder();
                var storeRes = db.ExecuteJx9(x, s =>
                {
                    sb.Append(s);
                });

                Assert.AreEqual(true, storeRes);
                Assert.AreEqual("Total number of stored records: 5", sb.ToString().TrimEnd());

                db.Close();
            }
        }

        [TestMethod]
        public void Test_Store_and_Fetch_All()
        {
            var db = UnqliteDB.Create();
            var res = db.Open(databaseName, Unqlite_Open.CREATE);
            if (res)
            {
                var x = File.ReadAllText("../../jx9testfiles/store_and_fetch_all.jx9");
                
                var storeAndFetchAllRes = db.ExecuteJx9(x, s =>
                {
                    Console.WriteLine(s);
                    Assert.AreEqual("[{\"name\":\"robert\",\"age\":35,\"mail\":\"rob@example.com\",\"__id\":1},{\"name\":\"monji\",\"age\":47,\"mail\":\"monji@example.com\",\"__id\":2},{\"name\":\"barzini\",\"age\":52,\"mail\":\"barz@mobster.com\",\"__id\":3}]", s);
                });
                Assert.AreEqual(true, storeAndFetchAllRes);

                db.Close();
            }
        }
    }
}
