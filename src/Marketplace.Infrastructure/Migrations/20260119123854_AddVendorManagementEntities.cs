using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorManagementEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessName = table.Column<string>(type: "text", nullable: false),
                    BusinessDescription = table.Column<string>(type: "text", nullable: true),
                    BusinessEmail = table.Column<string>(type: "text", nullable: false),
                    BusinessPhone = table.Column<string>(type: "text", nullable: false),
                    Website = table.Column<string>(type: "text", nullable: true),
                    Street = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    ZipCode = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    TaxId = table.Column<string>(type: "text", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: true),
                    Logo = table.Column<string>(type: "text", nullable: true),
                    CoverImage = table.Column<string>(type: "text", nullable: true),
                    VerificationStatus = table.Column<int>(type: "integer", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerificationNotes = table.Column<string>(type: "text", nullable: true),
                    Rating = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalReviews = table.Column<int>(type: "integer", nullable: false),
                    TotalSales = table.Column<int>(type: "integer", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalProducts = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AcceptOrders = table.Column<bool>(type: "boolean", nullable: false),
                    AutoApproveReviews = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendors_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivityType = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: true),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    PerformedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorActivityLogs_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorApiKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    KeyName = table.Column<string>(type: "text", nullable: false),
                    ApiKey = table.Column<string>(type: "text", nullable: false),
                    ApiSecret = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastUsedIp = table.Column<string>(type: "text", nullable: true),
                    AllowedIps = table.Column<string[]>(type: "text[]", nullable: true),
                    Permissions = table.Column<string[]>(type: "text[]", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorApiKeys_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorBankAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    BankName = table.Column<string>(type: "text", nullable: false),
                    AccountHolderName = table.Column<string>(type: "text", nullable: false),
                    AccountNumber = table.Column<string>(type: "text", nullable: false),
                    RoutingNumber = table.Column<string>(type: "text", nullable: false),
                    SwiftCode = table.Column<string>(type: "text", nullable: true),
                    Iban = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerificationNotes = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorBankAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorBankAccounts_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorBusinessHours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    OpenTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    CloseTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    Is24Hours = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorBusinessHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorBusinessHours_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorCommissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CommissionPercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    FixedFee = table.Column<decimal>(type: "numeric", nullable: true),
                    MinimumCommission = table.Column<decimal>(type: "numeric", nullable: true),
                    MaximumCommission = table.Column<decimal>(type: "numeric", nullable: true),
                    CommissionType = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorCommissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorCommissions_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    ZipCode = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsPickupLocation = table.Column<bool>(type: "boolean", nullable: false),
                    IsWarehouse = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorLocations_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorNotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailOrderNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    EmailPaymentNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    EmailReviewNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    EmailProductNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    EmailMarketingEmails = table.Column<bool>(type: "boolean", nullable: false),
                    SmsOrderNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    SmsPaymentNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    SmsUrgentAlerts = table.Column<bool>(type: "boolean", nullable: false),
                    PushOrderNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    PushPaymentNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    PushReviewNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    InAppOrderNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    InAppPaymentNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    InAppReviewNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    InAppSystemNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorNotificationPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorNotificationPreferences_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorPerformanceMetrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalOrders = table.Column<int>(type: "integer", nullable: false),
                    FulfilledOrders = table.Column<int>(type: "integer", nullable: false),
                    CancelledOrders = table.Column<int>(type: "integer", nullable: false),
                    ReturnedOrders = table.Column<int>(type: "integer", nullable: false),
                    OrderFulfillmentRate = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageResponseTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    TotalMessages = table.Column<int>(type: "integer", nullable: false),
                    RespondedMessages = table.Column<int>(type: "integer", nullable: false),
                    ResponseRate = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageShippingTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    OnTimeDeliveries = table.Column<int>(type: "integer", nullable: false),
                    LateDeliveries = table.Column<int>(type: "integer", nullable: false),
                    OnTimeDeliveryRate = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageRating = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalReviews = table.Column<int>(type: "integer", nullable: false),
                    PositiveReviews = table.Column<int>(type: "integer", nullable: false),
                    NegativeReviews = table.Column<int>(type: "integer", nullable: false),
                    ActiveProducts = table.Column<int>(type: "integer", nullable: false),
                    TotalProducts = table.Column<int>(type: "integer", nullable: false),
                    LowStockProducts = table.Column<int>(type: "integer", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "numeric", nullable: false),
                    PendingPayouts = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPayouts = table.Column<decimal>(type: "numeric", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorPerformanceMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorPerformanceMetrics_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorShippingSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    OffersFreeShipping = table.Column<bool>(type: "boolean", nullable: false),
                    FreeShippingThreshold = table.Column<decimal>(type: "numeric", nullable: true),
                    DefaultShippingCost = table.Column<decimal>(type: "numeric", nullable: true),
                    DefaultShippingMethod = table.Column<int>(type: "integer", nullable: false),
                    AllowLocalPickup = table.Column<bool>(type: "boolean", nullable: false),
                    LocalPickupFee = table.Column<decimal>(type: "numeric", nullable: true),
                    EstimatedDeliveryDays = table.Column<int>(type: "integer", nullable: true),
                    ShippingPolicy = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorShippingSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorShippingSettings_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Plan = table.Column<int>(type: "integer", nullable: false),
                    MonthlyFee = table.Column<decimal>(type: "numeric", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethodId = table.Column<string>(type: "text", nullable: true),
                    LastPaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextBillingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AutoRenew = table.Column<bool>(type: "boolean", nullable: false),
                    CancellationReason = table.Column<string>(type: "text", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorSubscriptions_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorTaxInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaxId = table.Column<string>(type: "text", nullable: false),
                    TaxIdType = table.Column<int>(type: "integer", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: true),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerificationNotes = table.Column<string>(type: "text", nullable: true),
                    TaxRate = table.Column<decimal>(type: "numeric", nullable: true),
                    CollectsTax = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorTaxInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorTaxInfos_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorVerifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentType = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorVerifications_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorActivityLogs_VendorId",
                table: "VendorActivityLogs",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorActivityLogs_VendorId_CreatedAt",
                table: "VendorActivityLogs",
                columns: new[] { "VendorId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorApiKeys_ApiKey",
                table: "VendorApiKeys",
                column: "ApiKey");

            migrationBuilder.CreateIndex(
                name: "IX_VendorApiKeys_VendorId",
                table: "VendorApiKeys",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorBankAccounts_VendorId",
                table: "VendorBankAccounts",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorBusinessHours_VendorId",
                table: "VendorBusinessHours",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCommissions_VendorId",
                table: "VendorCommissions",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorLocations_Latitude_Longitude",
                table: "VendorLocations",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorLocations_VendorId",
                table: "VendorLocations",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorNotificationPreferences_VendorId",
                table: "VendorNotificationPreferences",
                column: "VendorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorPerformanceMetrics_VendorId",
                table: "VendorPerformanceMetrics",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPerformanceMetrics_VendorId_Period_PeriodStart",
                table: "VendorPerformanceMetrics",
                columns: new[] { "VendorId", "Period", "PeriodStart" });

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_BusinessEmail",
                table: "Vendors",
                column: "BusinessEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_UserId",
                table: "Vendors",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorShippingSettings_VendorId",
                table: "VendorShippingSettings",
                column: "VendorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorSubscriptions_VendorId",
                table: "VendorSubscriptions",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorTaxInfos_TaxId",
                table: "VendorTaxInfos",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorTaxInfos_VendorId",
                table: "VendorTaxInfos",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorVerifications_VendorId",
                table: "VendorVerifications",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorActivityLogs");

            migrationBuilder.DropTable(
                name: "VendorApiKeys");

            migrationBuilder.DropTable(
                name: "VendorBankAccounts");

            migrationBuilder.DropTable(
                name: "VendorBusinessHours");

            migrationBuilder.DropTable(
                name: "VendorCommissions");

            migrationBuilder.DropTable(
                name: "VendorLocations");

            migrationBuilder.DropTable(
                name: "VendorNotificationPreferences");

            migrationBuilder.DropTable(
                name: "VendorPerformanceMetrics");

            migrationBuilder.DropTable(
                name: "VendorShippingSettings");

            migrationBuilder.DropTable(
                name: "VendorSubscriptions");

            migrationBuilder.DropTable(
                name: "VendorTaxInfos");

            migrationBuilder.DropTable(
                name: "VendorVerifications");

            migrationBuilder.DropTable(
                name: "Vendors");
        }
    }
}
