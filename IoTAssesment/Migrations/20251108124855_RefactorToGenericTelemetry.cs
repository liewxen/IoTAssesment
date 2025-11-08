using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IoTAssesment.Migrations
{
    /// <inheritdoc />
    public partial class RefactorToGenericTelemetry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "IoTDevices",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "IoTDevices",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "BatteryLevel",
                table: "IoTDevices");

            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "IoTDevices");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "IoTDevices");

            migrationBuilder.AddColumn<string>(
                name: "ConfigurationJson",
                table: "IoTDevices",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "IoTDevices",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerName",
                table: "IoTDevices",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModelNumber",
                table: "IoTDevices",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "KeyDictionaries",
                columns: table => new
                {
                    KeyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KeyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DataType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    MinValue = table.Column<double>(type: "numeric(18,4)", nullable: true),
                    MaxValue = table.Column<double>(type: "numeric(18,4)", nullable: true),
                    DefaultValue = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ValidationRules = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyDictionaries", x => x.KeyId);
                });

            migrationBuilder.CreateTable(
                name: "Telemetries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    KeyId = table.Column<int>(type: "integer", nullable: false),
                    DblValue = table.Column<double>(type: "numeric(18,4)", nullable: true),
                    LongValue = table.Column<long>(type: "bigint", nullable: true),
                    StrValue = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    JsonValue = table.Column<string>(type: "jsonb", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Quality = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Context = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    partition_date = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telemetries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Telemetries_IoTDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "IoTDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Telemetries_KeyDictionaries_KeyId",
                        column: x => x.KeyId,
                        principalTable: "KeyDictionaries",
                        principalColumn: "KeyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 1,
                column: "Timestamp",
                value: new DateTime(2025, 10, 9, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1169));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 2,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 43, 55, 152, DateTimeKind.Utc).AddTicks(1171));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 3,
                column: "Timestamp",
                value: new DateTime(2025, 9, 24, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1173));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 4,
                column: "Timestamp",
                value: new DateTime(2025, 10, 29, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1176));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 5,
                column: "Timestamp",
                value: new DateTime(2025, 9, 9, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1177));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 6,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 10, 48, 55, 152, DateTimeKind.Utc).AddTicks(1179));

            migrationBuilder.UpdateData(
                table: "IoTDevices",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConfigurationJson", "CreatedAt", "IsActive", "LastSeen", "ManufacturerName", "ModelNumber", "UpdatedAt" },
                values: new object[] { null, new DateTime(2025, 10, 9, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1140), true, new DateTime(2025, 11, 8, 12, 43, 55, 152, DateTimeKind.Utc).AddTicks(1134), "TechSensors Inc", "TS-100", new DateTime(2025, 11, 8, 12, 43, 55, 152, DateTimeKind.Utc).AddTicks(1141) });

            migrationBuilder.UpdateData(
                table: "IoTDevices",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConfigurationJson", "CreatedAt", "IsActive", "LastSeen", "ManufacturerName", "ModelNumber", "UpdatedAt" },
                values: new object[] { null, new DateTime(2025, 9, 24, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1146), true, new DateTime(2025, 11, 8, 12, 46, 55, 152, DateTimeKind.Utc).AddTicks(1146), "SecureTech", "SL-200", new DateTime(2025, 11, 8, 12, 46, 55, 152, DateTimeKind.Utc).AddTicks(1147) });

            migrationBuilder.UpdateData(
                table: "IoTDevices",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConfigurationJson", "CreatedAt", "IsActive", "LastSeen", "ManufacturerName", "ModelNumber", "UpdatedAt" },
                values: new object[] { null, new DateTime(2025, 9, 9, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1150), true, new DateTime(2025, 11, 8, 10, 48, 55, 152, DateTimeKind.Utc).AddTicks(1150), "EnviroSense", "ES-300", new DateTime(2025, 11, 8, 10, 48, 55, 152, DateTimeKind.Utc).AddTicks(1151) });

            migrationBuilder.InsertData(
                table: "KeyDictionaries",
                columns: new[] { "KeyId", "Category", "CreatedAt", "DataType", "DefaultValue", "Description", "IsActive", "IsRequired", "KeyName", "MaxValue", "MinValue", "Unit", "UpdatedAt", "ValidationRules" },
                values: new object[,]
                {
                    { 1, "sensor", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1032), "double", null, "Device battery level percentage", true, false, "battery_level", 100.0, 0.0, "%", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1033), null },
                    { 2, "sensor", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1041), "double", null, "Temperature reading", true, false, "temperature", 100.0, -50.0, "°C", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1041), null },
                    { 3, "sensor", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1043), "double", null, "Humidity percentage", true, false, "humidity", 100.0, 0.0, "%", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1044), null },
                    { 4, "status", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1045), "long", null, "Signal strength indicator", true, false, "signal_strength", 0.0, -120.0, "dBm", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1045), null },
                    { 5, "metadata", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1047), "string", null, "Current firmware version", true, false, "firmware_version", null, null, null, new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1047), null },
                    { 6, "status", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1049), "string", null, "Current device operational status", true, false, "device_status", null, null, null, new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1050), null },
                    { 7, "status", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1051), "long", null, "Device uptime in seconds", true, false, "uptime", null, 0.0, "seconds", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1051), null },
                    { 8, "performance", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1052), "double", null, "Memory usage percentage", true, false, "memory_usage", 100.0, 0.0, "%", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1053), null },
                    { 9, "performance", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1054), "double", null, "CPU usage percentage", true, false, "cpu_usage", 100.0, 0.0, "%", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1054), null },
                    { 10, "status", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1056), "long", null, "Number of errors since last reset", true, false, "error_count", null, 0.0, null, new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1056), null },
                    { 11, "location", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1057), "json", null, "GPS coordinates", true, false, "location_coordinates", null, null, null, new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1058), null },
                    { 12, "config", new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1058), "json", null, "Device configuration settings", true, false, "configuration", null, null, null, new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1059), null }
                });

            migrationBuilder.InsertData(
                table: "Telemetries",
                columns: new[] { "Id", "Context", "DblValue", "DeviceId", "JsonValue", "KeyId", "LongValue", "partition_date", "Quality", "StrValue", "Timestamp" },
                values: new object[,]
                {
                    { 1L, null, 22.5, 1, null, 2, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 11, 48, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 2L, null, 85.5, 1, null, 1, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 11, 48, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 4L, null, 23.0, 1, null, 2, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 11, 53, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 5L, null, 85.400000000000006, 1, null, 1, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 11, 53, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 7L, null, 23.5, 1, null, 2, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 11, 58, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 8L, null, 85.299999999999997, 1, null, 1, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 11, 58, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 10L, null, 24.0, 1, null, 2, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 12, 3, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 11L, null, 85.200000000000003, 1, null, 1, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 12, 3, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 13L, null, 24.5, 1, null, 2, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 12, 8, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 14L, null, 85.099999999999994, 1, null, 1, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 12, 8, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 16L, null, 25.0, 1, null, 2, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 12, 13, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 17L, null, 85.0, 1, null, 1, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 12, 13, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 19L, null, 25.5, 1, null, 2, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 12, 18, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 20L, null, 84.900000000000006, 1, null, 1, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 12, 18, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 22L, null, 26.0, 1, null, 2, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 12, 23, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 23L, null, 84.799999999999997, 1, null, 1, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 12, 23, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 25L, null, 26.5, 1, null, 2, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 12, 28, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 26L, null, 84.700000000000003, 1, null, 1, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 12, 28, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 28L, null, 27.0, 1, null, 2, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 12, 33, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 29L, null, 84.599999999999994, 1, null, 1, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, new DateTime(2025, 11, 8, 12, 33, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 50L, null, null, 2, null, 6, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", "Operational", new DateTime(2025, 11, 8, 11, 48, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 51L, null, null, 2, null, 6, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", "Operational", new DateTime(2025, 11, 8, 11, 58, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 52L, null, null, 2, null, 6, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", "Operational", new DateTime(2025, 11, 8, 12, 8, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 53L, null, null, 2, null, 6, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", "Operational", new DateTime(2025, 11, 8, 12, 18, 55, 152, DateTimeKind.Utc).AddTicks(1192) },
                    { 54L, null, null, 2, null, 6, null, new DateTime(2025, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Good", "Operational", new DateTime(2025, 11, 8, 12, 28, 55, 152, DateTimeKind.Utc).AddTicks(1192) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IoTDevices_IsActive",
                table: "IoTDevices",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_IoTDevices_Location",
                table: "IoTDevices",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_KeyDictionaries_Category",
                table: "KeyDictionaries",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_KeyDictionaries_DataType",
                table: "KeyDictionaries",
                column: "DataType");

            migrationBuilder.CreateIndex(
                name: "IX_KeyDictionaries_IsActive",
                table: "KeyDictionaries",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_KeyDictionaries_KeyName",
                table: "KeyDictionaries",
                column: "KeyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Telemetries_DeviceId_KeyId_Timestamp",
                table: "Telemetries",
                columns: new[] { "DeviceId", "KeyId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Telemetries_KeyId",
                table: "Telemetries",
                column: "KeyId");

            migrationBuilder.CreateIndex(
                name: "IX_Telemetries_partition_date_DeviceId",
                table: "Telemetries",
                columns: new[] { "partition_date", "DeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_Telemetries_Quality",
                table: "Telemetries",
                column: "Quality");

            migrationBuilder.CreateIndex(
                name: "IX_Telemetries_Timestamp",
                table: "Telemetries",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Telemetries");

            migrationBuilder.DropTable(
                name: "KeyDictionaries");

            migrationBuilder.DropIndex(
                name: "IX_IoTDevices_IsActive",
                table: "IoTDevices");

            migrationBuilder.DropIndex(
                name: "IX_IoTDevices_Location",
                table: "IoTDevices");

            migrationBuilder.DropColumn(
                name: "ConfigurationJson",
                table: "IoTDevices");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "IoTDevices");

            migrationBuilder.DropColumn(
                name: "ManufacturerName",
                table: "IoTDevices");

            migrationBuilder.DropColumn(
                name: "ModelNumber",
                table: "IoTDevices");

            migrationBuilder.AddColumn<double>(
                name: "BatteryLevel",
                table: "IoTDevices",
                type: "numeric(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Humidity",
                table: "IoTDevices",
                type: "numeric(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Temperature",
                table: "IoTDevices",
                type: "numeric(6,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 1,
                column: "Timestamp",
                value: new DateTime(2025, 10, 9, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7402));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 2,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 19, 39, 728, DateTimeKind.Utc).AddTicks(7404));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 3,
                column: "Timestamp",
                value: new DateTime(2025, 9, 24, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7406));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 4,
                column: "Timestamp",
                value: new DateTime(2025, 10, 29, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7407));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 5,
                column: "Timestamp",
                value: new DateTime(2025, 9, 9, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7408));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 6,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 10, 24, 39, 728, DateTimeKind.Utc).AddTicks(7411));

            migrationBuilder.UpdateData(
                table: "IoTDevices",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BatteryLevel", "CreatedAt", "Humidity", "LastSeen", "Temperature", "UpdatedAt" },
                values: new object[] { 85.5, new DateTime(2025, 10, 9, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7310), 45.200000000000003, new DateTime(2025, 11, 8, 12, 19, 39, 728, DateTimeKind.Utc).AddTicks(7302), 22.5, new DateTime(2025, 11, 8, 12, 19, 39, 728, DateTimeKind.Utc).AddTicks(7312) });

            migrationBuilder.UpdateData(
                table: "IoTDevices",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BatteryLevel", "CreatedAt", "Humidity", "LastSeen", "Temperature", "UpdatedAt" },
                values: new object[] { 92.0, new DateTime(2025, 9, 24, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7317), null, new DateTime(2025, 11, 8, 12, 22, 39, 728, DateTimeKind.Utc).AddTicks(7317), null, new DateTime(2025, 11, 8, 12, 22, 39, 728, DateTimeKind.Utc).AddTicks(7318) });

            migrationBuilder.UpdateData(
                table: "IoTDevices",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BatteryLevel", "CreatedAt", "Humidity", "LastSeen", "Temperature", "UpdatedAt" },
                values: new object[] { 15.199999999999999, new DateTime(2025, 9, 9, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7322), 35.799999999999997, new DateTime(2025, 11, 8, 10, 24, 39, 728, DateTimeKind.Utc).AddTicks(7321), 18.699999999999999, new DateTime(2025, 11, 8, 10, 24, 39, 728, DateTimeKind.Utc).AddTicks(7322) });

            migrationBuilder.InsertData(
                table: "IoTDevices",
                columns: new[] { "Id", "BatteryLevel", "CreatedAt", "Description", "DeviceType", "FirmwareVersion", "Humidity", "IsOnline", "LastSeen", "Location", "Name", "SerialNumber", "Temperature", "UpdatedAt" },
                values: new object[,]
                {
                    { 4, 78.900000000000006, new DateTime(2025, 10, 24, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7326), "Detects motion in the parking lot area", "Security Sensor", "3.2.1", null, true, new DateTime(2025, 11, 8, 12, 23, 39, 728, DateTimeKind.Utc).AddTicks(7325), "Parking Lot - North Side", "Motion Detector - Parking Lot", "MOT-004-PL", null, new DateTime(2025, 11, 8, 12, 23, 39, 728, DateTimeKind.Utc).AddTicks(7326) },
                    { 5, 5.0999999999999996, new DateTime(2025, 10, 19, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7330), "Monitors air quality in the main office area", "Environmental Sensor", "1.5.2", 50.299999999999997, false, new DateTime(2025, 11, 7, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7329), "Main Office - Floor 1", "Air Quality Monitor - Office", "AIR-005-OF", 21.199999999999999, new DateTime(2025, 11, 7, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7330) }
                });

            migrationBuilder.InsertData(
                table: "DeviceLogs",
                columns: new[] { "Id", "Action", "Description", "DeviceId", "Status", "Timestamp", "UserAgent" },
                values: new object[,]
                {
                    { 7, "Created", "Device created and registered", 4, "Success", new DateTime(2025, 10, 24, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7412), null },
                    { 8, "Created", "Device created and registered", 5, "Success", new DateTime(2025, 10, 19, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7413), null },
                    { 9, "StatusUpdate", "Device went offline - battery critically low", 5, "Critical", new DateTime(2025, 11, 7, 12, 24, 39, 728, DateTimeKind.Utc).AddTicks(7415), null }
                });
        }
    }
}
