﻿using System.Data.Unqlite.Interop;
using System.Text;

namespace System.Data.Unqlite
{
    public class KeyValueCursor : IDisposable
    {
        private IntPtr cursor;
        private readonly UnqliteDBProxy dbProxy;

        internal KeyValueCursor(UnqliteDBProxy dbProxy, bool forwardCursor)
        {
            this.dbProxy = dbProxy;
            var success = dbProxy.InitKVCursor(out cursor);
            if (success)
            {
                if (forwardCursor)
                {
                    success = dbProxy.KVMoveToFirstEntry(cursor);
                }
                else
                {
                    success = dbProxy.KVMoveToLastEntry(cursor);
                }
                Open = true;
            }
            else
            {
                Open = false;
            }
        }

        public bool Open { get; set; }

        public void Dispose()
        {
            dbProxy.ReleaseCursor(cursor);
            cursor = IntPtr.Zero;
            Open = false;
        }

        public bool Read()
        {
            return dbProxy.KV_ValidEntry(cursor);
        }

        public void Prev()
        {
            dbProxy.KV_PrevEntry(cursor);
        }

        public void Next()
        {
            dbProxy.KV_NextEntry(cursor);
        }

        public string GetKey()
        {
            var keyData = dbProxy.KV_GetCurrentKey(cursor);
            return Encoding.ASCII.GetString(keyData);
        }

        public byte[] GetBinaryKey()
        {
            return dbProxy.KV_GetCurrentKey(cursor);
        }

        public string GetValue()
        {
            var valueData = dbProxy.KV_GetCurrentValue(cursor);
            return Encoding.ASCII.GetString(valueData);
        }

        public byte[] GetBinaryValue()
        {
            return dbProxy.KV_GetCurrentValue(cursor);
        }

        public void GetStringKey(Action<string> action)
        {
            dbProxy.GetCursorKeyValue(cursor, action);
        }

        public void GetBinaryKey(Action<byte[]> action)
        {
            dbProxy.GetCursorKeyValue(cursor, action);
        }

        public void GetStringValue(Action<string> action)
        {
            dbProxy.GetCursorValue(cursor, action);
        }

        public void GetBinaryValue(Action<byte[]> action)
        {
            dbProxy.GetCursorValue(cursor, action);
        }


        public void Seek(string key)
        {
            dbProxy.SeekKey(cursor, key, Unqlite_Cursor_Seek.Match_Exact);
        }


        public void Seek(string key, Unqlite_Cursor_Seek seekMode)
        {
            dbProxy.SeekKey(cursor, key, seekMode);
        }

        public bool Delete()
        {
            return dbProxy.DeleteEntry(cursor);
        }
    }
}