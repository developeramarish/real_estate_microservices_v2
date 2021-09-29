﻿using DC.Core.Infrastructure.Resources;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DC.Business.DataAccess.MySql.SQLStatements
{
    internal sealed class Images
    {
        private const string ResourceNamespace = "DC.Business.DataAccess.MySql.SQLStatements.Organization.Images";
        private static readonly Assembly ResourceAssembly = typeof(Users).Assembly;
        
        internal static string InsertPropertyPhoto => new ResourceManager(ResourceNamespace, ResourceAssembly).GetString("InsertPropertyPhoto.sql");
        internal static string InsertPropertyTempPhoto => new ResourceManager(ResourceNamespace, ResourceAssembly).GetString("InsertPropertyTempPhoto.sql");
        
        internal static string DeletePropertyPhoto => new ResourceManager(ResourceNamespace, ResourceAssembly).GetString("DeletePropertyPhoto.sql");
        internal static string DeleteBulkPropertyTempPhoto => new ResourceManager(ResourceNamespace, ResourceAssembly).GetString("DeleteBulkPropertyTempPhoto.sql");
        
        internal static string GetPropertyImagesById => new ResourceManager(ResourceNamespace, ResourceAssembly).GetString("GetPropertyImagesById.sql");
        
    }
}
