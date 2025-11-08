using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoTAssesment.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "IoTDevices",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "IoTDevices",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConnectionStatus",
                table: "IoTDevices",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMqttConnection",
                table: "IoTDevices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MqttPasswordHash",
                table: "IoTDevices",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MqttUsername",
                table: "IoTDevices",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 1,
                column: "Timestamp",
                value: new DateTime(2025, 10, 9, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(379));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 2,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 13, 26, 58, 545, DateTimeKind.Utc).AddTicks(382));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 3,
                column: "Timestamp",
                value: new DateTime(2025, 9, 24, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(383));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 4,
                column: "Timestamp",
                value: new DateTime(2025, 10, 29, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(385));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 5,
                column: "Timestamp",
                value: new DateTime(2025, 9, 9, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(424));

            migrationBuilder.UpdateData(
                table: "DeviceLogs",
                keyColumn: "Id",
                keyValue: 6,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 11, 31, 58, 545, DateTimeKind.Utc).AddTicks(426));

            migrationBuilder.UpdateData(
                table: "IoTDevices",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApiKey", "ClientId", "ConnectionStatus", "CreatedAt", "LastMqttConnection", "LastSeen", "MqttPasswordHash", "MqttUsername", "UpdatedAt" },
                values: new object[] { "", "", "Never Connected", new DateTime(2025, 10, 9, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(352), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 8, 13, 26, 58, 545, DateTimeKind.Utc).AddTicks(347), null, null, new DateTime(2025, 11, 8, 13, 26, 58, 545, DateTimeKind.Utc).AddTicks(354) });

            migrationBuilder.UpdateData(
                table: "IoTDevices",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApiKey", "ClientId", "ConnectionStatus", "CreatedAt", "LastMqttConnection", "LastSeen", "MqttPasswordHash", "MqttUsername", "UpdatedAt" },
                values: new object[] { "", "", "Never Connected", new DateTime(2025, 9, 24, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(359), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 8, 13, 29, 58, 545, DateTimeKind.Utc).AddTicks(358), null, null, new DateTime(2025, 11, 8, 13, 29, 58, 545, DateTimeKind.Utc).AddTicks(359) });

            migrationBuilder.UpdateData(
                table: "IoTDevices",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ApiKey", "ClientId", "ConnectionStatus", "CreatedAt", "LastMqttConnection", "LastSeen", "MqttPasswordHash", "MqttUsername", "UpdatedAt" },
                values: new object[] { "", "", "Never Connected", new DateTime(2025, 9, 9, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(363), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 8, 11, 31, 58, 545, DateTimeKind.Utc).AddTicks(362), null, null, new DateTime(2025, 11, 8, 11, 31, 58, 545, DateTimeKind.Utc).AddTicks(363) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(267), new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(268) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(275), new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(275) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(277), new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(277) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(279), new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(279) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(281), new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(281) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(283), new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(283) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(284), new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(285) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(286), new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(286) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(287), new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(288) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(290), new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(290) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 11,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(291), new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(291) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 12,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(292), new DateTime(2025, 11, 8, 13, 31, 58, 545, DateTimeKind.Utc).AddTicks(292) });

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 31, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 31, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 36, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 5L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 36, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 7L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 41, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 8L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 41, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 10L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 46, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 11L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 46, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 13L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 51, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 14L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 51, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 16L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 56, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 17L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 56, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 19L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 13, 1, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 20L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 13, 1, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 22L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 13, 6, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 23L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 13, 6, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 25L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 13, 11, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 26L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 13, 11, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 28L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 13, 16, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 29L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 13, 16, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 50L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 31, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 51L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 41, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 52L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 51, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 53L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 13, 1, 58, 545, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 54L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 13, 11, 58, 545, DateTimeKind.Utc).AddTicks(441));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "IoTDevices");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "IoTDevices");

            migrationBuilder.DropColumn(
                name: "ConnectionStatus",
                table: "IoTDevices");

            migrationBuilder.DropColumn(
                name: "LastMqttConnection",
                table: "IoTDevices");

            migrationBuilder.DropColumn(
                name: "MqttPasswordHash",
                table: "IoTDevices");

            migrationBuilder.DropColumn(
                name: "MqttUsername",
                table: "IoTDevices");

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
                columns: new[] { "CreatedAt", "LastSeen", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 9, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1140), new DateTime(2025, 11, 8, 12, 43, 55, 152, DateTimeKind.Utc).AddTicks(1134), new DateTime(2025, 11, 8, 12, 43, 55, 152, DateTimeKind.Utc).AddTicks(1141) });

            migrationBuilder.UpdateData(
                table: "IoTDevices",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "LastSeen", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1146), new DateTime(2025, 11, 8, 12, 46, 55, 152, DateTimeKind.Utc).AddTicks(1146), new DateTime(2025, 11, 8, 12, 46, 55, 152, DateTimeKind.Utc).AddTicks(1147) });

            migrationBuilder.UpdateData(
                table: "IoTDevices",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "LastSeen", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 9, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1150), new DateTime(2025, 11, 8, 10, 48, 55, 152, DateTimeKind.Utc).AddTicks(1150), new DateTime(2025, 11, 8, 10, 48, 55, 152, DateTimeKind.Utc).AddTicks(1151) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1032), new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1033) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1041), new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1041) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1043), new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1044) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1045), new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1045) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1047), new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1047) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1049), new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1050) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1051), new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1051) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1052), new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1053) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1054), new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1054) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1056), new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1056) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 11,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1057), new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1058) });

            migrationBuilder.UpdateData(
                table: "KeyDictionaries",
                keyColumn: "KeyId",
                keyValue: 12,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1058), new DateTime(2025, 11, 8, 12, 48, 55, 152, DateTimeKind.Utc).AddTicks(1059) });

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 11, 48, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 11, 48, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 11, 53, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 5L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 11, 53, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 7L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 11, 58, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 8L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 11, 58, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 10L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 3, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 11L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 3, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 13L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 8, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 14L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 8, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 16L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 13, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 17L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 13, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 19L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 18, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 20L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 18, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 22L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 23, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 23L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 23, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 25L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 28, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 26L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 28, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 28L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 33, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 29L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 33, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 50L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 11, 48, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 51L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 11, 58, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 52L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 8, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 53L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 18, 55, 152, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Telemetries",
                keyColumn: "Id",
                keyValue: 54L,
                column: "Timestamp",
                value: new DateTime(2025, 11, 8, 12, 28, 55, 152, DateTimeKind.Utc).AddTicks(1192));
        }
    }
}
