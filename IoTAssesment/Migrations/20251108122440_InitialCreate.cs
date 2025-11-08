using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IoTAssesment.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IoTDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DeviceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SerialNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FirmwareVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BatteryLevel = table.Column<double>(type: "numeric(5,2)", nullable: true),
                    Temperature = table.Column<double>(type: "numeric(6,2)", nullable: true),
                    Humidity = table.Column<double>(type: "numeric(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IoTDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceLogs_IoTDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "IoTDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "IoTDevices",
                columns: new[] { "Id", "BatteryLevel", "CreatedAt", "Description", "DeviceType", "FirmwareVersion", "Humidity", "IsOnline", "LastSeen", "Location", "Name", "SerialNumber", "Temperature", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 85.5, new DateTime(2025, 10, 9, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7310), "Monitors temperature in warehouse storage area", "Temperature Sensor", "1.2.3", 45.200000000000003, true, new DateTime(2025, 11, 8, 12, 19, 39, 728, DateTimeKind.Utc).AddTicks(7302), "Warehouse A - Zone 1", "Temperature Sensor - Warehouse A", "TEMP-001-WHA", 22.5, new DateTime(2025, 11, 8, 12, 19, 39, 728, DateTimeKind.Utc).AddTicks(7312) },
                    { 2, 92.0, new DateTime(2025, 9, 24, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7317), "Electronic lock for main building entrance", "Access Control", "2.1.0", null, true, new DateTime(2025, 11, 8, 12, 22, 39, 728, DateTimeKind.Utc).AddTicks(7317), "Building Main Entrance", "Smart Lock - Main Entrance", "LOCK-002-ME", null, new DateTime(2025, 11, 8, 12, 22, 39, 728, DateTimeKind.Utc).AddTicks(7318) },
                    { 3, 15.199999999999999, new DateTime(2025, 9, 9, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7322), "Monitors humidity levels in the server room", "Environmental Sensor", "1.0.8", 35.799999999999997, false, new DateTime(2025, 11, 8, 10, 24, 39, 728, DateTimeKind.Utc).AddTicks(7321), "Server Room - Floor 2", "Humidity Monitor - Server Room", "HUM-003-SR", 18.699999999999999, new DateTime(2025, 11, 8, 10, 24, 39, 728, DateTimeKind.Utc).AddTicks(7322) },
                    { 4, 78.900000000000006, new DateTime(2025, 10, 24, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7326), "Detects motion in the parking lot area", "Security Sensor", "3.2.1", null, true, new DateTime(2025, 11, 8, 12, 23, 39, 728, DateTimeKind.Utc).AddTicks(7325), "Parking Lot - North Side", "Motion Detector - Parking Lot", "MOT-004-PL", null, new DateTime(2025, 11, 8, 12, 23, 39, 728, DateTimeKind.Utc).AddTicks(7326) },
                    { 5, 5.0999999999999996, new DateTime(2025, 10, 19, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7330), "Monitors air quality in the main office area", "Environmental Sensor", "1.5.2", 50.299999999999997, false, new DateTime(2025, 11, 7, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7329), "Main Office - Floor 1", "Air Quality Monitor - Office", "AIR-005-OF", 21.199999999999999, new DateTime(2025, 11, 7, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7330) }
                });

            migrationBuilder.InsertData(
                table: "DeviceLogs",
                columns: new[] { "Id", "Action", "Description", "DeviceId", "Status", "Timestamp", "UserAgent" },
                values: new object[,]
                {
                    { 1, "Created", "Device created and registered", 1, "Success", new DateTime(2025, 10, 9, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7402), null },
                    { 2, "StatusUpdate", "Device came online", 1, "Success", new DateTime(2025, 11, 8, 12, 19, 39, 728, DateTimeKind.Utc).AddTicks(7404), null },
                    { 3, "Created", "Device created and registered", 2, "Success", new DateTime(2025, 9, 24, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7406), null },
                    { 4, "FirmwareUpdate", "Firmware updated to version 2.1.0", 2, "Success", new DateTime(2025, 10, 29, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7407), null },
                    { 5, "Created", "Device created and registered", 3, "Success", new DateTime(2025, 9, 9, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7408), null },
                    { 6, "StatusUpdate", "Device went offline - low battery", 3, "Warning", new DateTime(2025, 11, 8, 10, 24, 39, 728, DateTimeKind.Utc).AddTicks(7411), null },
                    { 7, "Created", "Device created and registered", 4, "Success", new DateTime(2025, 10, 24, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7412), null },
                    { 8, "Created", "Device created and registered", 5, "Success", new DateTime(2025, 10, 19, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7413), null },
                    { 9, "StatusUpdate", "Device went offline - battery critically low", 5, "Critical", new DateTime(2025, 11, 7, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7415), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceLogs_Action",
                table: "DeviceLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceLogs_DeviceId",
                table: "DeviceLogs",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceLogs_Timestamp",
                table: "DeviceLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_IoTDevices_DeviceType",
                table: "IoTDevices",
                column: "DeviceType");

            migrationBuilder.CreateIndex(
                name: "IX_IoTDevices_IsOnline",
                table: "IoTDevices",
                column: "IsOnline");

            migrationBuilder.CreateIndex(
                name: "IX_IoTDevices_Name",
                table: "IoTDevices",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceLogs");

            migrationBuilder.DropTable(
                name: "IoTDevices");
        }
    }
}
