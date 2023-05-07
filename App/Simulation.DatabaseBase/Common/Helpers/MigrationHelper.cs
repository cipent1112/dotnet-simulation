using Microsoft.EntityFrameworkCore.Migrations;

namespace Simulation.DatabaseBase.Common.Helpers;

public static class MigrationHelper
{
    private static string SetIndexName(string table, object column, bool unique)
    {
        var indexPrefix = unique ? "Unique" : "Index";

        if (!string.IsNullOrEmpty(table) && table.Length > 1)
            table = $"{table[0].ToString().ToUpper() + table[1..].ToLower()}";
        else table = $"{table[0].ToString().ToUpper()}";


        var columnString = string.Empty;
        if (column is string[] columns)
            columnString = columns.Aggregate(columnString, (current, col) => current + col);
        else columnString = column.ToString();

        return $"{indexPrefix}_{table}__{columnString}";
    }

    public static void SetIndex(
        this MigrationBuilder builder,
        string table,
        object column,
        string? schema = null,
        bool unique = false,
        string? filter = null,
        bool[]? descending = null
    )
    {
        switch (column)
        {
            case string[] columns:
            {
                builder.CreateIndex(
                    name: SetIndexName(table, columns, unique),
                    table: table,
                    columns: columns,
                    schema: schema,
                    unique: unique,
                    filter: filter,
                    descending: descending
                );
                break;
            }
            case string col:
                builder.CreateIndex(
                    name: SetIndexName(table, col, unique),
                    table: table,
                    column: col,
                    schema: schema,
                    unique: unique,
                    filter: filter,
                    descending: descending
                );
                break;
            default:
                throw new Exception("Invalid column data type.");
        }
    }

    public static void SetForeignKey(
        this MigrationBuilder builder,
        string table,
        object column,
        string principalTable,
        string? schema = null,
        string? principalSchema = null,
        object? principalColumn = null,
        ReferentialAction onUpdate = ReferentialAction.Cascade,
        ReferentialAction onDelete = ReferentialAction.NoAction
    )
    {
        switch (column)
        {
            case string[] columns when principalColumn is string[] principalColumns:
                builder.AddForeignKey(
                    name: "",
                    table: table,
                    columns: columns,
                    principalTable: principalTable,
                    schema: schema,
                    principalSchema: principalSchema,
                    principalColumns: principalColumns,
                    onUpdate: onUpdate,
                    onDelete: onDelete
                );
                break;
            case string columnString when principalColumn is string principalColumnString:
                builder.AddForeignKey(
                    name: "",
                    table: table,
                    column: columnString,
                    principalTable: principalTable,
                    schema: schema,
                    principalSchema: principalSchema,
                    principalColumn: principalColumnString,
                    onUpdate: onUpdate,
                    onDelete: onDelete
                );
                break;
            default:
                throw new Exception("Invalid foreign key format.");
        }
    }
}