using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.Unqlite.Interop
{
    internal class UnqliteDBProxy : IDisposable
    {
        private bool disposed;
        public IntPtr DBHandle { get; private set; }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnqliteDBProxy()
        {
            Dispose(false);
        }

        internal bool Open(string fileName, Unqlite_Open iMode)
        {
            var handler = IntPtr.Zero;
            var res = Libunqlite.unqlite_open(out handler, fileName, (int) iMode);
            DBHandle = handler;
            return res == 0;
        }

        internal bool SaveKeyValue(string Key, string Value)
        {
            var keyData = Encoding.UTF8.GetBytes(Key);
            var data = Encoding.UTF8.GetBytes(Value);

            var res = Libunqlite.unqlite_kv_store(DBHandle, keyData, keyData.Length, data, (ulong) data.Length);
            return res == 0;
        }

        internal bool SaveKeyValue(string Key, byte[] data)
        {
            var keyData = Encoding.UTF8.GetBytes(Key);
            var res = Libunqlite.unqlite_kv_store(DBHandle, keyData, keyData.Length, data, (ulong) data.Length);
            return res == 0;
        }

        internal bool AppendKeyValue(string Key, string Value)
        {
            var keyData = Encoding.UTF8.GetBytes(Key);
            var data = Encoding.UTF8.GetBytes(Value);
            var res = Libunqlite.unqlite_kv_append(DBHandle, keyData, keyData.Length, data, (ulong) data.Length);
            return res == 0;
        }

        internal bool AppendKeyValue(string Key, byte[] data)
        {
            var keyData = Encoding.UTF8.GetBytes(Key);
            var res = Libunqlite.unqlite_kv_append(DBHandle, keyData, keyData.Length, data, (ulong) data.Length);
            return res == 0;
        }

        internal string GetKeyValue(string Key)
        {
            byte[] value = null;
            ulong valueLength = 0;
            var keyData = Encoding.UTF8.GetBytes(Key);
            var res = Libunqlite.unqlite_kv_fetch(DBHandle, keyData, keyData.Length, null, out valueLength);
            if (res == 0)
            {
                value = new byte[valueLength];
                Libunqlite.unqlite_kv_fetch(DBHandle, keyData, keyData.Length, value, out valueLength);
            }
            if (value != null)
            {
                return Encoding.UTF8.GetString(value);
            }
            return null;
        }

        internal byte[] GetKeyBinaryValue(string Key)
        {
            byte[] value = null;
            ulong valueLength = 0;
            var keyData = Encoding.UTF8.GetBytes(Key);
            var res = Libunqlite.unqlite_kv_fetch(DBHandle, keyData, keyData.Length, null, out valueLength);
            if (res == 0)
            {
                value = new byte[valueLength];
                Libunqlite.unqlite_kv_fetch(DBHandle, keyData, keyData.Length, value, out valueLength);
            }
            if (value != null)
            {
                return value;
            }
            return null;
        }

        internal bool Compile(string jx9)
        {
            var handler = IntPtr.Zero;
            var jx9bytes = Encoding.UTF8.GetBytes(jx9);
            var res = Libunqlite.unqlite_compile(DBHandle, jx9bytes, jx9bytes.Length, out handler);

            return res == 0;
        }

        internal bool ExecuteJx9(string jx9, Action<string> action)
        {
            int res;

            var handler = IntPtr.Zero;
            var jx9bytes = Encoding.UTF8.GetBytes(jx9);
            res = Libunqlite.unqlite_compile(DBHandle, jx9bytes, jx9bytes.Length, out handler);
            if (res != 0) return false;

            res = Libunqlite.unqlite_vm_config(handler, (int) UNQLITE_VM_CONFIG.UNQLITE_VM_CONFIG_OUTPUT,
                (pointer, len, data) =>
                {
                    var value = Marshal.PtrToStringAnsi(pointer, (int)len);
                    action(value);
                    return 0;
                }, null);

            if (res != 0) return false;

            res = Libunqlite.unqlite_vm_exec(handler);

            if (res != 0) return false;

            Libunqlite.unqlite_vm_release(handler);

            return res == 0;
        }

        internal void Close()
        {
            Libunqlite.unqlite_close(DBHandle);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                Terminate();
            }

            disposed = true;
        }

        private void Terminate()
        {
            if (DBHandle == IntPtr.Zero)
            {
            }
        }

        internal void GetKeyValue(string Key, Action<string> action)
        {
            var keyData = Encoding.UTF8.GetBytes(Key);
            Libunqlite.unqlite_kv_fetch_callback(DBHandle, keyData, keyData.Length,
                (dataPointer, dataLen, pUserData) =>
                {
                    var value = Marshal.PtrToStringAnsi(dataPointer, (int) dataLen);
                    action(value);
                    return 0;
                }, null);
        }

        internal void GetKeyBinaryValue(string Key, Action<byte[]> action)
        {
            var keyData = Encoding.UTF8.GetBytes(Key);
            Libunqlite.unqlite_kv_fetch_callback(DBHandle, keyData, keyData.Length,
                (dataPointer, dataLen, pUserData) =>
                {
                    var buffer = new byte[dataLen];
                    Marshal.Copy(dataPointer, buffer, 0, (int) dataLen);
                    action(buffer);
                    return 0;
                }, null);
        }

        internal bool InitKVCursor(out IntPtr cursor)
        {
            var res = Libunqlite.unqlite_kv_cursor_init(DBHandle, out cursor);
            return res == 0;
        }

        internal bool KVMoveToFirstEntry(IntPtr cursor)
        {
            var res = Libunqlite.unqlite_kv_cursor_first_entry(cursor);
            return res == 0;
        }

        internal bool KVMoveToLastEntry(IntPtr cursor)
        {
            var res = Libunqlite.unqlite_kv_cursor_last_entry(cursor);
            return res == 0;
        }

        internal bool KV_ValidEntry(IntPtr cursor)
        {
            var res = Libunqlite.unqlite_kv_cursor_valid_entry(cursor);
            return res == 1;
        }

        internal void KV_PrevEntry(IntPtr cursor)
        {
            Libunqlite.unqlite_kv_cursor_prev_entry(cursor);
        }

        internal void KV_NextEntry(IntPtr cursor)
        {
            Libunqlite.unqlite_kv_cursor_next_entry(cursor);
        }

        internal byte[] KV_GetCurrentKey(IntPtr cursor)
        {
            byte[] value = null;
            var keyLength = 0;
            var res = Libunqlite.unqlite_kv_cursor_key(cursor, null, out keyLength);
            if (res == 0)
            {
                value = new byte[keyLength];
                Libunqlite.unqlite_kv_cursor_key(cursor, value, out keyLength);
            }
            return value;
        }

        internal byte[] KV_GetCurrentValue(IntPtr cursor)
        {
            byte[] value = null;
            ulong valueLength = 0;
            var res = Libunqlite.unqlite_kv_cursor_data(cursor, null, out valueLength);
            if (res == 0)
            {
                value = new byte[valueLength];
                Libunqlite.unqlite_kv_cursor_data(cursor, value, out valueLength);
            }
            return value;
        }

        internal void GetCursorKeyValue(IntPtr cursor, Action<string> action)
        {
            // cursor
            Libunqlite.unqlite_kv_cursor_key_callback(cursor,
                (dataPointer, dataLen, pUserData) =>
                {
                    var value = Marshal.PtrToStringAnsi(dataPointer, (int)dataLen);
                    action(value);
                    return 0;
                }, null);
        }

        internal void GetCursorKeyValue(IntPtr cursor, Action<byte[]> action)
        {
            Libunqlite.unqlite_kv_cursor_key_callback(cursor,
                (dataPointer, dataLen, pUserData) =>
                {
                    var buffer = new byte[dataLen];
                    Marshal.Copy(dataPointer, buffer, 0, (int) dataLen);
                    action(buffer);
                    return 0;
                }, null);
        }

        internal void GetCursorValue(IntPtr cursor, Action<string> action)
        {
            Libunqlite.unqlite_kv_cursor_data_callback(cursor,
                (dataPointer, dataLen, pUserData) =>
                {
                    var value = Marshal.PtrToStringAnsi(dataPointer, (int) dataLen);
                    action(value);
                    return 0;
                }, null);
        }

        internal void GetCursorValue(IntPtr cursor, Action<byte[]> action)
        {
            Libunqlite.unqlite_kv_cursor_data_callback(cursor,
                (dataPointer, dataLen, pUserData) =>
                {
                    var buffer = new byte[dataLen];
                    Marshal.Copy(dataPointer, buffer, 0, (int) dataLen);
                    action(buffer);
                    return 0;
                }, null);
        }

        internal void ReleaseCursor(IntPtr cursor)
        {
            Libunqlite.unqlite_kv_cursor_release(DBHandle, cursor);
        }

        internal void SeekKey(IntPtr cursor, string key, Unqlite_Cursor_Seek seekMode)
        {
            var keyBytes = Encoding.ASCII.GetBytes(key);
            Libunqlite.unqlite_kv_cursor_seek(cursor, keyBytes, keyBytes.Length, (int) seekMode);
        }

        internal bool DeleteEntry(IntPtr cursor)
        {
            var res = Libunqlite.unqlite_kv_cursor_delete_entry(cursor);
            return res == 0;
        }

        public bool Commit()
        {
            var res = Libunqlite.unqlite_commit(DBHandle);
            return res == 0;
        }
    }
}