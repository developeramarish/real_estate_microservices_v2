﻿using Dapper;
using DC.Business.Domain.Entities.Organization;
using DC.Business.Domain.Repositories.Organization;
using DC.Core.DataAccess.MySql;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DC.Business.DataAccess.MySql.Repositories.Organization
{
    public class ImageRepository : BusinessRepository, IImageRepository
    {
        public ImageRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task AddPropertyPhoto(long propertyId, string imageName, string imagePath)
        {
            // using(var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add(name: "PropertyId", value: propertyId, dbType: DbType.Int64, direction: ParameterDirection.Input);
                parameters.Add(name: "ImageName", value: imageName, dbType: DbType.String, direction: ParameterDirection.Input);
                parameters.Add(name: "ImagePath", value: imagePath, dbType: DbType.String, direction: ParameterDirection.Input);

                SQLBuilder insertStmt = new SQLBuilder(SQLStatements.Images.InsertPropertyPhoto);
                await connection.ExecuteAsync(insertStmt.ToStatement(), parameters).ConfigureAwait(false);
            }
        }

        public async Task AddPropertyTempPhoto(long propertyId, string imageName, string imagePath)
        {
            // using(var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add(name: "PropertyId", value: propertyId, dbType: DbType.Int64, direction: ParameterDirection.Input);
                parameters.Add(name: "ImageName", value: imageName, dbType: DbType.String, direction: ParameterDirection.Input);
                parameters.Add(name: "ImagePath", value: imagePath, dbType: DbType.String, direction: ParameterDirection.Input);

                SQLBuilder insertStmt = new SQLBuilder(SQLStatements.Images.InsertPropertyTempPhoto);
                await connection.ExecuteAsync(insertStmt.ToStatement(), parameters).ConfigureAwait(false);
            }
        }

        public async Task DeletePropertyPhoto(string imagePath, long propertyId)
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder deleteStmt = new SQLBuilder(SQLStatements.Images.DeletePropertyPhoto);
                await connection.ExecuteAsync(deleteStmt.ToStatement(), new { PropertyId = propertyId, ImagePath = imagePath }).ConfigureAwait(false);
            }
        }

        public async Task DeleteBulkPropertyTempPhoto(long propertyId)
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder deleteStmt = new SQLBuilder(SQLStatements.Images.DeleteBulkPropertyTempPhoto);
                await connection.ExecuteAsync(deleteStmt.ToStatement(), new { PropertyId = propertyId }).ConfigureAwait(false);
            }
        }

        public async Task<List<Image>> GetPropertyImagesById(long id)
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder getStmt = new SQLBuilder(SQLStatements.Images.GetPropertyImagesById);
                var result = await connection.QueryAsync<Image>(getStmt.ToStatement(), new { id = id }).ConfigureAwait(false);
                return result?.ToList();
            }
        }
    }
}
