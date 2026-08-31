namespace XeoTechErp.Api.Domain.Enums;

public enum OrderStatus { Pending = 0, Processing = 1, Shipped = 2, Delivered = 3, Cancelled = 4 }
public enum QuoteStatus { Draft = 0, Sent = 1, Approved = 2, Converted = 3 }
public enum InvoiceStatus { Pending = 0, Paid = 1, Overdue = 2 }
public enum PoStatus { Pending = 0, Approved = 1, InTransit = 2, Received = 3 }
public enum PaymentMethod { Card = 0, BankTransfer = 1, Cash = 2, Other = 3 }
public enum AssetStatus { InService = 0, Disposed = 1 }
public enum EmployeeStatus { Active = 0, OnLeave = 1 }
