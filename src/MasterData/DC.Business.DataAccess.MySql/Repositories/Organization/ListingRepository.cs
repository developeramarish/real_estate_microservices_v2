using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using DC.Business.Application.Contracts.Dtos.Organization.Listing;
using DC.Business.Application.Contracts.Dtos.Organization.Listing.Admin;
using DC.Business.Domain.Entities.Organization;
using DC.Business.Domain.Repositories.Organization;
using DC.Core.Contracts.Application.Pipeline.Dtos.Input;
using DC.Core.DataAccess.MySql;
using Microsoft.Extensions.Configuration;
using DC.Business.DataAccess.MySql.Extensions;

namespace DC.Business.DataAccess.MySql.Repositories.Organization
{
    public class ListingRepository : BusinessRepository, IListingRepository
    {
        public ListingRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<ulong> ListProperty(Property property)
        {
            using(IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                DynamicParameters propertyParameters = new DynamicParameters(property);

                SQLBuilder insertPropertyStmt = new SQLBuilder(SQLStatements.Listing.InsertProperty);
                ulong insertedId = (ulong)await connection.ExecuteScalarAsync(insertPropertyStmt.ToStatement(), propertyParameters).ConfigureAwait(false);
                return insertedId;
            }
        }

        public async Task<ulong> ListTempProperty(Property property)
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                DynamicParameters propertyParameters = new DynamicParameters(property);

                SQLBuilder insertPropertyStmt = new SQLBuilder(SQLStatements.Listing.InsertTempProperty);
                ulong insertedId = (ulong)await connection.ExecuteScalarAsync(insertPropertyStmt.ToStatement(), propertyParameters).ConfigureAwait(false);
                return insertedId;
            }
        }

        public async Task AddTempCharacteristics(Characteristics characterisitcs)
        {
            // using(var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                DynamicParameters parameters = new DynamicParameters(characterisitcs);

                SQLBuilder insertStmt = new SQLBuilder(SQLStatements.Listing.InsertTempCharacteristics);
                await connection.ExecuteAsync(insertStmt.ToStatement(), parameters).ConfigureAwait(false);
            }
        }

        public async Task AddCharacteristics(Characteristics characterisitcs)
        {
            // using(var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                DynamicParameters parameters = new DynamicParameters(characterisitcs);
     
                SQLBuilder insertStmt = new SQLBuilder(SQLStatements.Listing.InsertCharacteristics);
                await connection.ExecuteAsync(insertStmt.ToStatement(), parameters).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Property>> GetPropertiesByUserBasicService(long userId)
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder getStmt = new SQLBuilder(SQLStatements.Listing.GetPropertiesByUserBasic);
                var result = await connection.QueryAsync<Property>(getStmt.ToStatement(), new { userId = userId }).ConfigureAwait(false);
                return result?.ToList();
            }
        }

        public async Task<Property> GetPropertyById(long propertyId)
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder getStmt = new SQLBuilder(SQLStatements.Listing.GetPropertyById);
                var result = await connection.QueryAsync<Property>(getStmt.ToStatement(), new { id = propertyId }).ConfigureAwait(false);
                return result?.SingleOrDefault();
            }
        }

        public async Task<Property> GetTempPropertyById(long propertyId)
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder getStmt = new SQLBuilder(SQLStatements.Listing.GetTempPropertyById);
                var result = await connection.QueryAsync<Property>(getStmt.ToStatement(), new { id = propertyId }).ConfigureAwait(false);
                return result?.SingleOrDefault();
            }
        }

        public async Task<Property> GetPropertyByTempId(Guid tempId)
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder getStmt = new SQLBuilder(SQLStatements.Listing.GetPropertyByTempId);
                var lookup = new Dictionary<int, Property>();
                var result = connection.Query<Property, Characteristics, Image, Property>(getStmt.ToStatement(),
                   (p, c, i) =>
                   {
                       Property property;

                       if (!lookup.TryGetValue(c.Id, out property))
                           lookup.Add(c.Id, property = p);

                       if (property.Characteristics == null)
                           property.Characteristics = new List<Characteristics>();
                       if(c != null)
                           property.Characteristics.Add(c);

                       if (property.Images == null)
                           property.Images = new List<Image>();
                       if (i != null)
                           property.Images.Add(i);

                       return property;
                   }, new { id = tempId }, splitOn: "Id").Distinct();
                var resultList = lookup.Values;

                Property parentResult = null; 
                if(resultList != null && resultList?.Count > 0)
                {
                    parentResult = resultList.First();

                    var characteristics = new List<Characteristics>();
                    var images = new List<Image>();
                    foreach (var chrts in resultList)
                    {
                        if(chrts.Characteristics.Count > 0)
                        {
                            characteristics.Add(chrts.Characteristics.First());
                        }

                        if (chrts.Images.Count > 0)
                        {
                            images.Add(chrts.Images.First());
                        }
                    }

                    if(characteristics.Count > 0)
                    {
                        var characteristicsResult = characteristics.GroupBy(y => y.Id).Select(x => x.First()).ToList();
                        parentResult.Characteristics = characteristicsResult;
                    }

                    if(images.Count > 0)
                    {
                        var imagesResult = images.GroupBy(y => y.Id).Select(x => x.First()).ToList();
                        parentResult.Images = imagesResult;
                    }
                }
                
                return parentResult != null ? parentResult : new Property();
            }
        }

        public async Task<Property> GetPropertyByEmail(string email)
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder getStmt = new SQLBuilder(SQLStatements.Listing.GetPropertyByEmail);
                var result = await connection.QueryAsync<Property>(getStmt.ToStatement(), new { email = email }).ConfigureAwait(false);
                return result?.SingleOrDefault();
            }
        }

        public async Task<Property> GetPropertyByUser(PropertyByIdUserIdDto input)
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder getStmt = new SQLBuilder(SQLStatements.Listing.GetPropertyByUserId);
                var result = await connection.QueryAsync<Property>(getStmt.ToStatement(), new { id = input.id, userId = input.userId }).ConfigureAwait(false);
                return result?.SingleOrDefault();
            }
        }

       public async Task DeletePropertyByUser(PropertyByIdUserIdDto input)
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder deleteStmt = new SQLBuilder(SQLStatements.Listing.DeletePropertyByUserId);
                await connection.ExecuteAsync(deleteStmt.ToStatement(), new { id = input.id, userId = input.userId }).ConfigureAwait(false);
            }
        }

        public async Task DeleteTempPropertyById(long id)
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder deleteStmt = new SQLBuilder(SQLStatements.Listing.DeleteTempPropertyById);
                await connection.ExecuteAsync(deleteStmt.ToStatement(), new { id = id }).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<OperationType>> GetOperationTypes()
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder getOperationTypesStmt = new SQLBuilder(SQLStatements.Listing.GetOperationTypes);
                var result = await connection.QueryAsync<OperationType>(getOperationTypesStmt.ToStatement()).ConfigureAwait(false);
                return result?.ToList();
            }
        }

        public async Task<IEnumerable<PropertyType>> GetPropertyTypes()
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder getPropertyTypesStmt = new SQLBuilder(SQLStatements.Listing.GetPropertyTypes);
                var result = await connection.QueryAsync<PropertyType>(getPropertyTypesStmt.ToStatement()).ConfigureAwait(false);
                return result?.ToList();
            }
        }

        #region Admin
        public async Task<IEnumerable<Property>> SearchPropertiesForAdmin(SearchPaginationRequestDto<SearchPropertyForAdminRequestDto> inputDto)
        {
            SQLBuilder query = new SQLBuilder(SQLStatements.Listing.SearchPropertiesForAdmin);

            if (string.IsNullOrEmpty(inputDto.OrderBy))
                query.AddOrderBy(nameof(Property.Id), inputDto.OrderDescending);
            else
                query.AddOrderBy(inputDto.OrderBy, inputDto.OrderDescending);

            IEnumerable<string> filterParameters = inputDto.RestrictionCriteria?.GetFilterParameters();
            query.KeepParameters(filterParameters);

            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                // var lookup = new Dictionary<long, User>();
                var limit = (inputDto.PageNumber - 1) * inputDto.RowsPerPage;

                var result = await connection.QueryAsync<Property>(query.ToPaginated(), new
                {
                    Type = inputDto.RestrictionCriteria.Type,
                    LimitPagination = limit,
                    RowsPerPage = inputDto.RowsPerPage,
                }).ConfigureAwait(false);
                return result.ToList();
            }
        }

        public async Task<int> CountPropertiesForAdmin(SearchPaginationRequestDto<SearchPropertyForAdminRequestDto> inputDto)
        {
            SQLBuilder query = new SQLBuilder(SQLStatements.Listing.CountPropertiesForAdmin);

            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                var result = await connection.QueryFirstAsync<int>(query.ToStatement(), new
                {
                    Type = inputDto.RestrictionCriteria.Type
                }).ConfigureAwait(false);
                return result;
            }
        }

        public async Task ApprovePropertyForAdminService(int mySqlId)
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder statement = new SQLBuilder(SQLStatements.Listing.ApprovePropertyForAdminService);
                await connection.ExecuteAsync(statement.ToStatement(), new { Id = mySqlId }).ConfigureAwait(false);
            }
        }

        public async Task BlockPropertyByAdminService(int mySqlId)
        {
            using (IDbConnection connection = BusinessDatabase.OpenConnection())
            {
                SQLBuilder statement = new SQLBuilder(SQLStatements.Listing.BlockPropertyByAdminService);
                await connection.ExecuteAsync(statement.ToStatement(), new { Id = mySqlId }).ConfigureAwait(false);
            }
        }
        #endregion

    }
}
