using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.Framework.Utils
{
    public class EmailTemplateKey
    {
        //booking related
        public static class Booking
        {
            public static string BookingEmail { get { return "BookingEmail"; } }
            public static string RugTrackerBookingEmail { get { return "RugOnlineBookingTemp"; } }
            public static string RugTrackerBookingEmailNotification { get { return "RugOnlineBookingNotificationTemp"; } }
        }
        public static class BookingSign
        {
            public static string CustomerSignedBookingConfirmationToCustomer { get { return "CustomerSignedBookingConfirmationToCustomer"; } }
            public static string CustomerSignedBookingConfirmationToSalesPerson { get { return "CustomerSignedBookingConfirmationToSalesPerson"; } }
            public static string CustomerDeclinedBookingToSalesPerson { get { return "CustomerDeclinedBookingToSalesPerson"; } }

        }
        public static class Contact
        {
            public static string ContactUs { get { return "ContactUs"; } }
            public static string ContactUsNotification { get { return "ContactUsNotification"; } }
             
        } 
        public static class PasswordReset
        {
            public static string ResetPassword { get { return "ResetPassword"; } }
            public static string PasswordHasBeenReset { get { return "PasswordHasBeenReset"; } }
        }
        public static class TaxInvoice
        {
            public static string SendTaxInvoice { get { return "SendTaxInvoice"; } }
        }
        public static class Registration
        {
            public static string ThanksForJoiningNoPurchase { get { return "ThanksForJoiningNoPurchase"; } }
            public static string ThanksForJoining { get { return "ThanksForJoining"; } }
            public static string VerifyEmailAddress { get { return "VerifyEmailAddress"; } }
 
        }

        public static class Companies
        {
            public static string ShareFileTemplate { get { return "ShareCompanyFiles"; } }
        }

        public static class ReminderEmail
        {
            public static string ReminderTemplate { get { return "ReminderTemplate"; } }
        }
        public static class SurveyEmail
        {
            public static string SurveyTemplate { get { return "SurveyTemplate"; } }
            public static string SurveyConfirmationTemplate { get { return "SurveyConfirmationTemplate"; } }
            public static string SurveyTemplateUser { get { return "SurveyTemplateUser"; } }
        }

        public static class EmailToSalesPersonFromLeads
        {       
            public static string EmailToSalesPersonFromLead { get { return "EmailToSalesPersonFromLead"; } }
        }

        public static class EmailWorkOrderComplete
        {
            public static string mailWorkOrderComplete { get { return "mailWorkOrderComplete"; } }
        }

        public static class EmailServiceOrderComplete
        {
            public static string mailServiceOrderComplete { get { return "mailServiceOrderComplete"; } }
        }

        public static class EmailToResponsiblePersonFromCustomerNote
        {
            public static string mailToResponsiblePersonFromCustomerNote { get { return "mailToResponsiblePersonFromCustomerNote"; } }
        }

        public static class EmailToLeadSignAgreement
        {
            public static string mailToLeadSignAgreement { get { return "mailToLeadSignAgreement"; } }
        }

        public static class CancellationSignAgreement
        {
            public static string CancellationAgreement { get { return "CancellationAgreementMail"; } }
        }

        public static class CustomerCancellationConfirm
        {
            public static string CancellationConfirm { get { return "CancellationConfirm"; } }
        }
        public static class WorkOrderInformationSendEMail
        {
            public static string WorkOrderInformationSendMail { get { return "WorkOrderInformationSendMail"; } }
        }
        public static class ServiceOrderInformationSendEMail
        {
            public static string ServiceOrderInformationSendMail { get { return "ServiceOrderInformationSendMail"; } }
        }

        public static class EmailToLeadFromCurrentLoggedinUser
        {
            public static string EmailToLeadFromCurrentLoggedInUser { get { return "EmailToLeadFromCurrentLoggedinUser"; } }
        }

        public static class RequestAdmin
        {
            public static string RequestToAdmin { get { return "RequestToAdmin"; } }
        }

        public static class EmailConvertLeadToCustomer
        {
            public static string mailConvertLeadToCustomer { get { return "mailConvertLeadToCustomer"; } }
        }

        public static class EmailToCreateRestaurant
        {
            public static string mailToCreateRestaurant { get { return "mailToCreateRestaurant"; } }
        }

        public static class EmailNotConvertLeadToCustomer
        {
            public static string mailNotConvertLeadToCustomer { get { return "mailNotConvertLeadToCustomer"; } }
        }

        public static class EmailAssignInventoryTechReceive
        {
            public static string mailAssignInventoryTechReceive { get { return "mailAssignInventoryTechReceive"; } }
        }

        public static class TicketNotificationEmail
        {
            public static string TicketNotificationmail { get { return "TicketNotificationmail"; } }
        }

        public static class EmailCreateWorkOrder
        {
            public static string mailCreateWorkOrder { get { return "mailCreateWorkOrder"; } }
        }
        public static class EmailCreateServiceOrder
        {
            public static string mailCreateServiceOrder { get { return "mailCreateServiceOrder"; } }
        }
        public static class EmailToSetUpCustomer
        {
            public static string mailCreateCustomerSetup { get { return "mailCreateCustomerSetup"; } }
        }
        public static class mailtoLeadsAggrement
        {
            public static string EmailtoLeadsAggrement { get { return "EmailtoLeadsAggrement"; } }
        }
        public static class mailToFileManagement
        {
            public static string FileManagementMail { get { return "FileManagementMail"; } }
            public static string FileManagementMailWithoutCustomerSignature { get { return "FileManagementMailWithoutCustomerSignature"; } }
            public static string FileManagementNotificationSendEmail { get { return "FileManagementNotificationSendEmail"; } }
        }
        public static class mailToFileManagementConfirmation
        {
            public static string FileManagementConfirmationMail { get { return "FileManagementConfirmationMail"; } }
        }
        public static class mailToLeadCreation
        {
            public static string SendMailLeadCreation { get { return "SendMailLeadCreation"; } }
        }
        public static class MailToAddendum
        {
            public static string EMailToAddendum { get { return "EmailtoAddendum"; } }
        }
        public static class TicketAssignMail
        {
            public static string TicketAssignEmail { get { return "TicketAssignEmail"; } }
        }
        public static class CredentialEmail
        {
            public static string CredentialMail { get { return "CredentialMail"; } }
        }
        public static class EmailToCustomerForTransaction
        {
            public static string mailtoCustomerforTransaction { get { return "mailtoCustomerforTransaction"; } }
            public static string mailtoSalesPersonforTransaction { get { return "mailtoSalesPersonforTransaction"; } }
        }
        public static class EmailToLeadtoCusforQA
        {
            public static string QAforLeadtoCusConvert { get { return "QAforLeadtoCusConvert"; } }
        }
        public static class EmailLeadSuccess
        {
            public static string mailleadSuccessSetup { get { return "mailleadSuccessSetup"; } }
        }
        public static class RUGTicketAgreementEmail
        {
            public static string RUGTicketAgreement { get { return "RUGTicketAgreement"; } }
        }

        public static class TicketPDFMail
        {
            public static string TicketPDFEmail { get { return "TicketPDFEmail"; } }
        }
        public static class EmailNotSetCustomerBilling
        {
            public static string mailNotSetCustomerBilling { get { return "mailNotSetCustomerBilling"; } }
        }

        public static class EmailToEmployeeFromCustomerNote
        {
            public static string EmailToEmployeeFromCustomerNotes { get { return "EmailToEmployeeFromCustomerNotes"; } }
        }

        public static class SendFundingEmail
        {
            public static string SendFundingmail { get { return "SendFundingEmail"; } }
        }

        public static class EmailToEmployeeFromFollowUpNote
        {
            public static string mailToEmployeeFromFollowUpNote { get { return "mailToEmployeeFromFollowUpNote"; } }
        }
        //[Shariful-16-9-19]
        public static class DeclineEmail
        {
            public static string declineEmail { get { return "DeclineMail"; } }
        }
        //[~Shariful-16-9-19]
        public static class InvoiceTemplate
        {
            public static string InvoiceEmail { get { return "InvoiceEmail"; } }
            public static string EstimateNotificationSendTemplate { get { return "EstimateNotificationSendTemplate"; } }
        }
        public static class Requisition
        {
            public static string RequisitionEmail { get { return "RequisitionEmail"; } }
        }
        public static class FileAttachment
        {
            public static string FilesAttachment { get { return "FileAttachmentForCustomerReview"; } }
        }

        public static class EmailToOrderPlace
        {
            public static string mailToOrderPlace { get { return "mailToOrderPlace"; } }
        }

        public static class EmailToOrderStatusTemp
        {
            public static string mailToOrderStatusTemp { get { return "OrderStatusTemp"; } }
        }

        public static class Estimate
        {
            public static string EstimateEmail { get { return "EstimateEmail"; } }
        }
        public static class Estimator
        {
            public static string EstimatorEmail { get { return "EstimatorEmail"; } }
        }
        public static class CustomerInfo
        {
            public static string CustomerInfoMail { get { return "CustomerInfoMail"; } }
            public static string CustomerInfoPredefineEmailTemplate { get { return "CustomerInfoPredefineEmailTemplate"; } }

        }
        public static class ActivityNotification
        {
            public static string ActivityNotifications { get { return "ActivityNotifications"; } }
        }

        public static class LateNotificationTicket
        {
            public static string LateNotificationsTicket { get { return "LateNotificationTicket"; } }
        }

        public static class EstimateSign
        {
            public static string CustomerSignedEstimateConfirmationToCustomer { get { return "CustomerSignedEstimateConfirmationToCustomer"; } }
            public static string CustomerSignedEstimateConfirmationToSalesPerson { get { return "CustomerSignedEstimateConfirmationToSalesPerson"; } }
            public static string CustomerDeclinedEstimateToSalesPerson { get { return "CustomerDeclinedEstimateToSalesPerson"; } }

            public static string CustomerSignedEstimateConfirmationToCreatedBy { get { return "CustomerSignedEstimateConfirmationToCreatedBy"; } }
            public static string CustomerDeclinedEstimateToCreatedBy { get { return "CustomerDeclinedEstimateToCreatedBy"; } }


        }
        public static class SubscriptionNotification
        {
            public static string SubscribedToAuthorizeCustomerNotification { get { return "SubscribedToAuthorizeCustomerNotification"; } }
        }
        public static class OTPEmail
        {
            public static string SendOTPEmail { get { return "OTPEmail"; } }
        }
        public static class FriendEmail
        {
            public static string SendFriendEmail { get { return "FriendEmail"; } }
        }
        public static class SendSurveyEmail
        {
            public static string SurveyEmail { get { return "SurveyForCustomer"; } }
        }
        public static class UpdateCustomerEmail
        {
            public static string SendUpdateCustomerEmail { get { return "UpdateCustomerEmail"; } }
        }
        public static class PredefinedTemplates
        {
          

            public static string EstimatePredefineEmailTemplate { get { return "EstimatePredefineEmailTemplate"; } }
            public static string InvoicePredefineEmailTemplate { get { return "InvoicePredefineEmailTemplate"; } }
            public static string POPredefineEmailTemplate { get { return "POPredefineEmailTemplate"; } }
        }
        public static class TicketEmailTemplates
        {
            public static string SendTicketCreatedNotification { get { return "SendTicketCreatedNotification"; } }
            public static string SendTicketUpdatedNotification { get { return "TicketUpdatedEmail"; } }
            public static string SendEqpReleasedNotification { get { return "EqpReleasedEmail"; } }
            //public static string SendTicketReplyNotification { get { return "SendTicketReplyNotification"; } }
        }
        public static class PurchaseOrderEmailTemplates
        {
            public static string POCreatedEmail { get { return "POCreatedEmail"; } }
        }
        public static class Agreement
        {
            public static string AgreementRMR { get { return "Agreement"; } }
        
        }
        public static class CoverLetter
        {
            public static string EstimatorPredefineEmailTemplate { get { return "EstimatorPredefineEmailTemplate"; } }
            public static string EstimatorEmailPredefineEmailTemplate { get { return "EstimatorEmailPredefineEmailTemplate"; } }
            

        }
        public static class SmartAgreement
        {
            public static string SmartAgreementRMR { get { return "SmartAgreement"; } }
        }

        public static class AgreementFirstPage
        {
            public static string SmartAgreementFirstPage { get { return "SmartAgreementFirstPage"; } }
        }
        public static class AgreementCommercial
        {
            public static string CommercialAgreement { get { return "CommercialAgreement"; } }
        }
        public static class CodeSafetyDocumentTemplate
        {
            public static string CodeSafetyDocument { get { return "CodeSafetyDocument"; } }
        }
        public static class CustomerCancellationAggrement
        {
            public static string CancellationAggrement { get { return "CancellationAggrement"; } }
        }
        public static class CostomerAddendumPdf
        {
            public static string CustomerAddendum { get { return "CustomerAddendum"; } }
        }
        public static class BlankAgreementTemplates
        {
            public static string BlankAgreement { get { return "BlankAgreement"; } }
        }

        public static class EmployeeOnboradEmail
        {
            public static string WaitingForProfileApproval { get { return "WaitingForProfileApproval"; } }
            public static string ApprovedConfirmation { get { return "ApprovedConfirmation"; } }
            public static string ProfileUpdateRequest { get { return "ProfileUpdateRequest"; } }

            public static string OfferLetterSend { get { return "OfferLetterSend"; } }
        }
        public static class LeaveApplication
        {
            public static string NotifyToLeaveApproval { get { return "NotifyToLeaveApproval"; } }
            public static string NotifyToManager { get { return "NotifyToManager"; } }
            public static string NotifyToHrManager { get { return "NotifyToHrManager"; } }
        }
        public static class TCustomer
        {
            public static string CustomerEmail { get { return "CustomerEmail"; } }
            public static string CustomerNotification { get { return "CustomerNotification"; } }
            public static string CustomerBill { get { return "CustomerBill"; } }
        }

    }
}
