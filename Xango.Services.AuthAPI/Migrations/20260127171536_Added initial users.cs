using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Xango.Services.AuthAPI.Migrations
{
	/// <inheritdoc />
	public partial class Addedinitialusers : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.InsertData(
				table: "AspNetRoles",
				columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
				values: new object[,]
				{
					{  "025bcf83-2572-47a1-a9a8-37c271701214","CUSTOMER","CUSTOMER",null },
					{  "4bd2f013-1469-4779-9091-6ec6b0a1bfd1","ADMIN","ADMIN",null }
				}

				);

			migrationBuilder.InsertData(
				table: "AspNetUsers",
				columns: new[] {
				  "Id",
				  "UserName",
				  "NormalizedUserName",
				  "Email",
				  "NormalizedEmail",
				  "EmailConfirmed",
				  "PasswordHash",
				  "SecurityStamp",
				  "ConcurrencyStamp",
				  "PhoneNumber",
				  "PhoneNumberConfirmed",
				  "TwoFactorEnabled",
				  "LockoutEnd",
				  "LockoutEnabled",
				  "AccessFailedCount",
				  "Name"
				},
				values: new object[,]
				{
					{
						"1b3abdb1-60f7-4690-a659-fcbda02d3217",
						"user1@none.com",
						"USER1@NONE.COM",
						"user1@none.com",
						"USER1@NONE.COM",
						true,
						"AQAAAAIAAYagAAAAEMhsY6EJRQZFWo/cDDrOwJI9TpLbYbdO/6x0nD01YbKwlur0psaGV4IK+OfOtzV0aQ==",
						"U7MP64EWVM5JVQRS5RAZYBTQDFKRCQ7Y",
						"1691e4ec-35d4-4703-8cc9-d1f112c9c315",
						"783",
						true,
						false,
						null,
						false,
						0,
						"User1" 
					},
					{
						"516c8dd1-d330-430c-8842-f3c00f7ae327",
						"admin@none.com",
						"ADMIN@NONE.COM",
						"admin@none.com",
						"ADMIN@NONE.COM",
						true,
						"AQAAAAIAAYagAAAAEAGmUMcEuHs/DNoaMeDk6ScYOp5HV2UBk77f5K3ZhKT+kTljAaZeuJN8Gx6P0bakkg==",
						"CHJBDDJLETTSUHD2D2OGKVCVVFRMAVIP",
						"45c3210d-842c-46f7-ab47-553ace0c28b3",
						"783",
						true,
						false,
						null,
						false,
						0,
						"Admin" 
					}
		}
			);

			migrationBuilder.InsertData(
				table: "AspNetUserRoles",
				columns: new[] { "UserId", "RoleId" },
				values: new object[,]
				{
					{ "1b3abdb1-60f7-4690-a659-fcbda02d3217", "025bcf83-2572-47a1-a9a8-37c271701214" },
					{ "516c8dd1-d330-430c-8842-f3c00f7ae327", "4bd2f013-1469-4779-9091-6ec6b0a1bfd1" }
				}
			);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{

		}
	}
}
