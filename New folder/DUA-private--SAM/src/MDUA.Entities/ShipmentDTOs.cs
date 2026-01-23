using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.Entities
{

    public class ShipmentModalItemDto
    {
        public int DeliveryItemId { get; set; }
        public int Quantity { get; set; }

        public string ProductName { get; set; }
        public string VariantName { get; set; }
        public string Sku { get; set; }
    }
    public class CourierConnectionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class ShipmentModalDto
    {
        public int DeliveryId { get; set; }
        public int SalesOrderId { get; set; }

        public string OrderNumber { get; set; }          // ON00000001 / DO00000001
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientAddress { get; set; }

        public int ItemQuantity { get; set; }
        public decimal AmountToCollect { get; set; }     // COD (Due amount)
        public string SpecialInstruction { get; set; }

        public int? SelectedCompanyCarrierId { get; set; }
        public int? PackageWeightGrams { get; set; }

        public List<ShipmentModalItemDto> Items { get; set; } = new List<ShipmentModalItemDto>();
    }

    public class CompanyCarrierOptionDto
    {
        public int CompanyCarrierId { get; set; }
        public string CarrierName { get; set; }
        public string ApiEndpoint { get; set; }
        public bool RequiresApi { get; set; }
    }

    public class CompanyCarrierCredentialDto
    {
        public int CompanyCarrierId { get; set; }
        public int CompanyId { get; set; }

        public int CarrierId { get; set; }
        public string CarrierName { get; set; }

        public string ApiEndpoint { get; set; }
        public bool RequiresApi { get; set; }

        // Generic (Steadfast: ApiKey + ApiSecret)
        // Pathao: ApiKey=client_id, ApiSecret=client_secret
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }

        // Pathao-only extras
        public string ApiUsername { get; set; }
        public string ApiPassword { get; set; }
        public int? StoreId { get; set; }
    }
    public class CourierShipmentRequest
    {
        public string Invoice { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientAddress { get; set; }

        public decimal CodAmount { get; set; }
        public int TotalItem { get; set; }

        public string Note { get; set; }
        public string ItemDescription { get; set; }

        // Injected from DB
        public string ApiBaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }

        // Pathao-specific
        public string ApiUsername { get; set; }
        public string ApiPassword { get; set; }
        public int? StoreId { get; set; }

        // Optional shipment details
        public int? PackageWeightGrams { get; set; }
    }

    public class CourierShipmentResult
    {
        public bool Success { get; set; }

        public string TrackingNumber { get; set; }
        public string ConsignmentId { get; set; }

        public string RawResponse { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class CourierCredentialRowDto
    {
        public int CarrierId { get; set; }
        public string CarrierName { get; set; }
        public string ApiEndpoint { get; set; }
        public bool RequiresApi { get; set; }

        public int? CompanyCarrierId { get; set; }
        public bool IsActive { get; set; }

        public string ApiKeyMasked { get; set; }       // "********1234"
        public string ApiSecretMasked { get; set; }    // "********5678"
        public bool HasApiKey { get; set; }
        public bool HasApiSecret { get; set; }
        public DateTime? SecretUpdatedAt { get; set; }

        public string ApiUsernameMasked { get; set; }
        public string ApiPasswordMasked { get; set; }
        public bool HasApiUsername { get; set; }
        public bool HasApiPassword { get; set; }
        public int? StoreId { get; set; }
    }

    public class SaveCourierCredentialRequest
    {
        public int CarrierId { get; set; }
        public bool IsActive { get; set; }

        // Generic (Steadfast/RedX/etc)
        public string ApiKey { get; set; }      // Pathao: client_id
        public string ApiSecret { get; set; }   // Pathao: client_secret

        // Pathao-only extras
        public string ApiUsername { get; set; } // Pathao: username/email
        public string ApiPassword { get; set; } // Pathao: password
        public int? StoreId { get; set; }       // Pathao: store_id
    }

}
