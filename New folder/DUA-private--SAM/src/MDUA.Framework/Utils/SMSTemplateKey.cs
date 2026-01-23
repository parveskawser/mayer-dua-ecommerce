using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.Framework.Utils
{
    public class SMSTemplateKey
    {
        public static class AgreementSMS
        {
            public static string SendAgrementSms { get { return "AgrementSms"; } }
        }
        public static class FileSMS
        {
            public static string SendFileSms { get { return "FileSms"; } }
            public static string SendFileSmsWithoutCustomerSign { get { return "FileSmsWithoutCustomerSign"; } }
        }
        public static class CancellationAgreementSMS
        {
            public static string AgreementSMS { get { return "CancellationAgrementSms"; } }
        }
        public static class AddendumSMS
        {
            public static string SMSAddendum { get { return "AddendumSMS"; } }
        }
        public static class InvoiceSMS
        {
            public static string SendInvoiceSms { get { return "InvSms"; } }
        }
    
        public static class EstimatorSMS
        {
            public static string SendEstimatorSms { get { return "EstSms"; } }
        }

        public static class ReminderSMS
        {
            public static string SendReminderSms { get { return "ReminderSms"; } }
        }
        public static class NoteSMS
        {
            public static string SendNoteSms { get { return "NoteSms"; } }
        }
        public static class RequisitionSMS
        {
            public static string SendRequisitionSms { get { return "ReqSms"; } }
        }
        public static class CustomerInfoSMS
        {
            public static string SendCustomerInfoSms { get { return "CustomerInfoSMS"; } }
        }
        public static class ActivityNotificationSMS
        {
            public static string ActivitySMS { get { return "ActivitySMS"; } }
        }
        public static class PurchaseOrderSMS
        {
            public static string SendPurchaseOrderSMS { get { return "PerchaseOrderSms"; } }
        }
        public static class SalesSMS
        {
            public static string SendSalesSms { get { return "SalesPersonSms"; } }
        }
        public static class LeadAgreementSMS
        {
            public static string SendAgreementSms { get { return "LeadsEstimateAgreeSms"; } }
        }

        public static class ConvertLeadtoCustomerSMS
        {
            public static string SendConvertLeadtoCustomerSMS { get { return "ConvertLeadtoCustomerSMS"; } }
        }
        public static class EstimateSignedSMS
        {
            public static string SendEstimateSignedSMS { get { return "EstimateSignedSMS"; } }
        }

    }
}
