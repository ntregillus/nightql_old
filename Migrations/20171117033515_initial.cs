using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace NightQL.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DbSchema",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbSchema", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DbUser",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Description = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Secret = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbUser", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DbChange",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Backward = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Forward = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemaID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbChange", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DbChange_DbSchema_SchemaID",
                        column: x => x.SchemaID,
                        principalTable: "DbSchema",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DbEntity",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SchemaID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbEntity", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DbEntity_DbSchema_SchemaID",
                        column: x => x.SchemaID,
                        principalTable: "DbSchema",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DbRelationship",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChildAlias = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ChildEntity = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ChildRequiresParent = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ParentAlias = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ParentEntity = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SchemaID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbRelationship", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DbRelationship_DbSchema_SchemaID",
                        column: x => x.SchemaID,
                        principalTable: "DbSchema",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateAccess = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReadAccess = table.Column<bool>(type: "bit", nullable: false),
                    SchemaName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    WriteAccess = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Permissions_DbUser_UserID",
                        column: x => x.UserID,
                        principalTable: "DbUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DbLink",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChildID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ParentID = table.Column<long>(type: "bigint", nullable: false),
                    RelationshipID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbLink", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DbLink_DbEntity_ChildID",
                        column: x => x.ChildID,
                        principalTable: "DbEntity",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbLink_DbEntity_ParentID",
                        column: x => x.ParentID,
                        principalTable: "DbEntity",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbLink_DbRelationship_RelationshipID",
                        column: x => x.RelationshipID,
                        principalTable: "DbRelationship",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DbChange_SchemaID",
                table: "DbChange",
                column: "SchemaID");

            migrationBuilder.CreateIndex(
                name: "IX_DbEntity_SchemaID",
                table: "DbEntity",
                column: "SchemaID");

            migrationBuilder.CreateIndex(
                name: "IX_DbLink_ChildID",
                table: "DbLink",
                column: "ChildID");

            migrationBuilder.CreateIndex(
                name: "IX_DbLink_ParentID",
                table: "DbLink",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_DbLink_RelationshipID",
                table: "DbLink",
                column: "RelationshipID");

            migrationBuilder.CreateIndex(
                name: "IX_DbRelationship_SchemaID",
                table: "DbRelationship",
                column: "SchemaID");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_UserID",
                table: "Permissions",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbChange");

            migrationBuilder.DropTable(
                name: "DbLink");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "DbEntity");

            migrationBuilder.DropTable(
                name: "DbRelationship");

            migrationBuilder.DropTable(
                name: "DbUser");

            migrationBuilder.DropTable(
                name: "DbSchema");
        }
    }
}
