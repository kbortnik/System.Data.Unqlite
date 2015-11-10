using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace System.Data.Unqlite.Tests
{
    [TestFixture]
    public class ConnectionTests
    {
        private const string databaseName = ":mem:";

        [TearDown]
        public void Test_DeleteDB()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var dbFilePath = Path.Combine(dir, databaseName);
            if (File.Exists(dbFilePath))
            {
                File.Delete(dbFilePath);
            }
        }

        [Test]
        public void Test_connection_Open_Create()
        {
            var db = UnqliteDB.Create();
            var res = db.Open(databaseName, Unqlite_Open.CREATE);
            if (res)
            {
                db.Close();
            }
            Assert.IsTrue(res);
        }

        [Test]
        public void Test_KeyValue_Store()
        {
            var db = UnqliteDB.Create();
            var res = db.Open(databaseName, Unqlite_Open.CREATE);
            if (res)
            {
                var res1 = db.SaveKeyValue("test", "hello world");
                var value = db.GetKeyValue("test");
                Assert.IsTrue(value == "hello world");
                db.Close();
            }
        }

        [Test]
        public void Test_KeyValue_Store_With_Callback()
        {
            var db = UnqliteDB.Create();
            var res = db.Open(databaseName, Unqlite_Open.CREATE);
            if (res)
            {
                var res1 = db.SaveKeyValue("test", "hello world");
                db.GetKeyValue("test", value => { Assert.IsTrue(value == "hello world"); });
                db.Close();
            }
        }

        [Test]
        public void Test_KeyValue_BinaryStore_With_Callback()
        {
            var db = UnqliteDB.Create();
            var res = db.Open(databaseName, Unqlite_Open.CREATE);
            if (res)
            {
                var res1 = db.SaveKeyValue("test", "hello world");
                db.GetKeyBinaryValue("test", value =>
                {
                    var strValue = Encoding.ASCII.GetString(value, 0, value.Length);
                    Assert.IsTrue(strValue == "hello world");
                });
                db.Close();
            }
        }

        [Test]
        public void Test_KeyValue_Cursor()
        {
            var db = UnqliteDB.Create();
            var res = db.Open(databaseName, Unqlite_Open.CREATE);
            for (var i = 0; i < 100; i++)
            {
                db.SaveKeyValue("test" + (i + 1), "hello world " + (i + 1));
            }
            using (var cursor = db.CreateKeyValueCursor())
            {
                while (cursor.Read())
                {
                    var key = cursor.GetKey();
                    var binaryKey = cursor.GetBinaryKey();
                    Console.Out.WriteLine("Key:" + key);
                    var value = cursor.GetValue();
                    var binaryValue = cursor.GetBinaryValue();
                    Console.Out.WriteLine("Value:" + value);
                    cursor.Next();
                }
            }
            db.Close();
        }

        [Test]
        public void Test_KeyValue_Cursor_with_callback()
        {
            var db = UnqliteDB.Create();
            var res = db.Open(databaseName, Unqlite_Open.CREATE);
            for (var i = 0; i < 20; i++)
            {
                db.SaveKeyValue("test" + (i + 1), "hello world " + (i + 1));
            }
            using (var cursor = db.CreateKeyValueCursor())
            {
                while (cursor.Read())
                {
                    cursor.GetStringKey(key => { Console.Out.WriteLine("Key:" + key); });
                    cursor.GetStringValue(value => { Console.Out.WriteLine("Value:" + value); });
                    cursor.Next();
                }
            }
            db.Close();
        }

        [Test]
        public void Test_KeyValue_Cursor_Seek()
        {
            var db = UnqliteDB.Create();
            var res = db.Open(databaseName, Unqlite_Open.CREATE);
            for (var i = 0; i < 20; i++)
            {
                db.SaveKeyValue("test" + (i + 1), "hello world " + (i + 1));
            }
            using (var cursor = db.CreateKeyValueCursor())
            {
                if (cursor.Read())
                {
                    cursor.Seek("test1");
                    var value = cursor.GetValue();
                    Assert.IsTrue(value == "hello world 1");
                }
            }
            db.Close();
        }

        [Test]
        public void Test_KeyValue_Cursor_SeekModeGE()
        {
            var db = UnqliteDB.Create();
            var res = db.Open(databaseName, Unqlite_Open.CREATE);
            for (var i = 0; i < 20; i++)
            {
                db.SaveKeyValue("test" + (i + 1), "hello world " + (i + 1));
            }
            using (var cursor = db.CreateKeyValueCursor())
            {
                if (cursor.Read())
                {
                    cursor.Seek("test1", Unqlite_Cursor_Seek.Match_GE);
                    var value = cursor.GetValue();
                    Assert.IsTrue(value == "hello world 1");
                }
            }
            db.Close();
        }

        [Test]
        public void Test_KeyValue_Cursor_Seek_Delete()
        {
            var db = UnqliteDB.Create();
            var res = db.Open(databaseName, Unqlite_Open.CREATE);
            for (var i = 0; i < 20; i++)
            {
                db.SaveKeyValue("test" + (i + 1), "hello world " + (i + 1));
            }
            using (var cursor = db.CreateKeyValueCursor())
            {
                if (cursor.Read())
                {
                    cursor.Seek("test4", Unqlite_Cursor_Seek.Match_GE);
                    var deleted = cursor.Delete();
                    Assert.IsTrue(deleted);
                }
            }
            db.Close();
        }
    }
}