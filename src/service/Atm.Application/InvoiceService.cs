using Atm.Application.Results;
using Atm.Domain;
using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using System.Data;

namespace Atm.Application;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IOperationRepository _operationRepository;
    private readonly IPersistenceTransactionProvider _transactionProvider;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IAccountRepository accountRepository,
        ISessionRepository sessionRepository,
        IOperationRepository operationRepository,
        IPersistenceTransactionProvider transactionProvider)
    {
        _invoiceRepository = invoiceRepository;
        _accountRepository = accountRepository;
        _sessionRepository = sessionRepository;
        _operationRepository = operationRepository;
        _transactionProvider = transactionProvider;
    }

    public async Task<CreateInvoiceResult> CreateInvoiceAsync(Guid sessionKey, string payerAccountNumber, decimal amount)
    {
        Session? session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.User || session.AccountNumber == null)
            return new CreateInvoiceResult(CreateInvoiceStatus.Unauthorized, null);

        if (!await _accountRepository.ExistsAsync(payerAccountNumber))
            return new CreateInvoiceResult(CreateInvoiceStatus.PayerNotFound, null);

        var invoice = new Invoice(payerAccountNumber, session.AccountNumber, amount);
        await _invoiceRepository.SaveAsync(invoice);

        return new CreateInvoiceResult(CreateInvoiceStatus.Success, invoice.Id);
    }

    public async Task<PayInvoiceResult> PayInvoiceAsync(Guid sessionKey, Guid invoiceId)
    {
        Session? session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.User || session.AccountNumber == null)
            return PayInvoiceResult.Unauthorized;

        Invoice? invoice = await _invoiceRepository.GetByIdAsync(invoiceId);

        if (invoice == null)
            return PayInvoiceResult.NotFound;

        if (invoice.PayerAccountNumber != session.AccountNumber)
            return PayInvoiceResult.Unauthorized;

        Account? payer = await _accountRepository.GetByNumberAsync(invoice.PayerAccountNumber);
        Account? payee = await _accountRepository.GetByNumberAsync(invoice.PayeeAccountNumber);

        if (payer == null || payee == null)
            return PayInvoiceResult.NotFound;

        if (!invoice.Pay())
            return PayInvoiceResult.AlreadyProcessed;

        if (!payer.Withdraw(invoice.Amount))
            return PayInvoiceResult.InsufficientFunds;

        payee.Deposit(invoice.Amount);

        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(IsolationLevel.ReadCommitted, CancellationToken.None);

        await _accountRepository.SaveAsync(payer);
        await _accountRepository.SaveAsync(payee);
        await _invoiceRepository.SaveAsync(invoice);

        await _operationRepository.SaveAsync(new Operation(payer.Number, OperationType.Withdraw, invoice.Amount, invoice.Id));
        await _operationRepository.SaveAsync(new Operation(payee.Number, OperationType.Deposit, invoice.Amount, invoice.Id));

        await transaction.CommitAsync(CancellationToken.None);

        return PayInvoiceResult.Success;
    }

    public async Task<CancelInvoiceResult> CancelInvoiceAsync(Guid sessionKey, Guid invoiceId)
    {
        Session? session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.User || session.AccountNumber == null)
            return CancelInvoiceResult.Unauthorized;

        Invoice? invoice = await _invoiceRepository.GetByIdAsync(invoiceId);

        if (invoice == null)
            return CancelInvoiceResult.NotFound;

        if (invoice.PayeeAccountNumber != session.AccountNumber)
            return CancelInvoiceResult.Unauthorized;

        if (!invoice.Cancel())
            return CancelInvoiceResult.AlreadyProcessed;

        await _invoiceRepository.SaveAsync(invoice);

        return CancelInvoiceResult.Success;
    }

    public async Task<GetOutgoingInvoicesResult> GetOutgoingInvoicesAsync(Guid sessionKey, string? payerAccountNumber, InvoiceStatus? status, Guid? cursor, int pageSize)
    {
        Session? session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.User || session.AccountNumber == null)
            return new GetOutgoingInvoicesResult(GetInvoicesStatus.Unauthorized, null, null);

        var invoices = new List<Invoice>();

        await foreach (Invoice invoice in _invoiceRepository.GetOutgoingAsync(session.AccountNumber, payerAccountNumber, status, cursor, pageSize + 1))
            invoices.Add(invoice);

        Guid? nextCursor = null;

        if (invoices.Count > pageSize)
        {
            nextCursor = invoices[pageSize - 1].Id;
            invoices.RemoveRange(pageSize, invoices.Count - pageSize);
        }

        return new GetOutgoingInvoicesResult(GetInvoicesStatus.Success, invoices, nextCursor);
    }

    public async Task<GetIncomingInvoicesResult> GetIncomingInvoicesAsync(Guid sessionKey, string? payeeAccountNumber, InvoiceStatus? status, Guid? cursor, int pageSize)
    {
        Session? session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.User || session.AccountNumber == null)
            return new GetIncomingInvoicesResult(GetInvoicesStatus.Unauthorized, null, null);

        var invoices = new List<Invoice>();

        await foreach (Invoice invoice in _invoiceRepository.GetIncomingAsync(session.AccountNumber, payeeAccountNumber, status, cursor, pageSize + 1))
            invoices.Add(invoice);

        Guid? nextCursor = null;

        if (invoices.Count > pageSize)
        {
            nextCursor = invoices[pageSize - 1].Id;
            invoices.RemoveRange(pageSize, invoices.Count - pageSize);
        }

        return new GetIncomingInvoicesResult(GetInvoicesStatus.Success, invoices, nextCursor);
    }
}
