System.Data.Unqlite
===================

[![Build status](https://ci.appveyor.com/api/projects/status/5ooif300du228558?svg=true)](https://ci.appveyor.com/project/kbortnik/system-data-unqlite) [![Coverage Status](https://coveralls.io/repos/kbortnik/System.Data.Unqlite/badge.svg?branch=master&service=github)](https://coveralls.io/github/kbortnik/System.Data.Unqlite?branch=master)

A wrapper to use Unqlite library in C# (or any CLR compatible language).

### About UnQLite

UnQLite is a in-process software library which implements a self-contained, serverless, zero-configuration, transactional NoSQL database engine. UnQLite is a document store database similar to MongoDB, Redis, CouchDB etc. as well a standard Key/Value store similar to BerkeleyDB, LevelDB, etc.

UnQLite is an embedded NoSQL (Key/Value store and Document-store) database engine. Unlike most other NoSQL databases, UnQLite does not have a separate server process. UnQLite reads and writes directly to ordinary disk files. A complete database with multiple collections, is contained in a single disk file. The database file format is cross-platform, you can freely copy a database between 32-bit and 64-bit systems or between big-endian and little-endian architectures.

For more info about Unqlite check the official page: http://www.unqlite.org/
