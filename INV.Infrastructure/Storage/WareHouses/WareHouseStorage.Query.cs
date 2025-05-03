namespace INV.Infrastructure.Storage.WareHouses
{
    public partial class WareHouseStorage
    {
        private const string SelectAllWareHousesQuery = @"
        SELECT Id,Path, Name, Description, Type 
        FROM [INV].[dbo].[WareHouse]";

        private const string GetMaxRootIdQuery = @"
        SELECT MAX(Path) 
        FROM WareHouse 
        WHERE Path.GetAncestor(1) = hierarchyid::GetRoot()";

        private const string GetMaxChildIdQuery = @"
        SELECT MAX(Path) 
        FROM WareHouse 
        WHERE Path.GetAncestor(1) = @aParent";

        private const string InsertWareHouseQuery = @"
        INSERT INTO WareHouse (Id,Path ,Name, Description, Type) 
        VALUES (@aId,@aPath, @aName, @aDesc, @aType)";

        private const string GetMaxTargetIdQuery = @"
        SELECT MAX(Path) 
        FROM WareHouse 
        WHERE Path.GetAncestor(1) = @targetId";

        private const string UpdateNodeParentQuery = @"
        UPDATE WareHouse 
        SET Id = @anewId 
        WHERE Id = @acurrentId";

        private const string GetWarhousesByWarehouseType = @"
        SElECT * FROM WareHouse
        WHERE Type = @aType";

        private const string UpdateWareHouseQuery = @"
    UPDATE WareHouse 
    SET Name = @aName,
        Description = @aDesc,
        Type = @aType
    WHERE Path = @aPath";

        private const string DeleteWareHouseQuery = @"
    DELETE FROM WareHouse
    WHERE Path = @aPath";

        private const string CheckChildrenQuery = @"
    SELECT COUNT(*)
    FROM WareHouse
    WHERE Path.GetAncestor(1) = @aPath";
    }
}