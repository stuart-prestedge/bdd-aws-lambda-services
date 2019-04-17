using System;
using System.Collections.Generic;
using BDDReferenceService.Contracts;
using BDDReferenceService.Model;

namespace BDDReferenceService
{

    /**
     * Interface for the data stores.
     */
    public interface IDataStores
    {
        INoSQLDataStore GetNoSQLDataStore();
        ISQLDataStore GetSQLDataStore();
    }

    /**
     * Interface for the NoSQL data store.
     */
    public interface INoSQLDataStore
    {
        object GetDBClient();
    }

    /**
     * Interface for the SQL data store.
     */
    public interface ISQLDataStore
    {
        
    }

    public static class DatabaseHelper
    {

    }   // DatabaseHelper
        
}   // BDDReferenceService
