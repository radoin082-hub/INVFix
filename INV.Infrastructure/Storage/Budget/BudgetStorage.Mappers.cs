using INV.Domain.Entities.Budget;
using Microsoft.Data.SqlClient;

namespace INV.Infrastructure.Storage.Budget
{
public partial class BudgetStorage
{
    private static Article ArticleDataReader(SqlDataReader reader)
    {
        return new Article
        {
            CodeArticle = (int)reader["CodeArticle"],
            Name = reader["Name"].ToString(),
            CodeChapter = (int)reader["CodeChapter"]
        };
    }

    public static Chapter ChapterDataReader(SqlDataReader reader)
    {
        return new Chapter
        {
            CodeChapter = (int)reader["CodeChapter"],
            Name = (string)reader["Name"]
        };
    }
}
}