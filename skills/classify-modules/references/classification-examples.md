# Classification Examples

## Example 1 — E-commerce Order Processing

### Input files
- `OrderService.cs`
- `OrderValidator.cs`
- `OrderRepository.cs`
- `DiscountCalculator.cs`

### Dependency analysis

```
External: OrderController → OrderService (incoming)
OrderService → OrderValidator (outgoing)
OrderService → OrderRepository (outgoing)
OrderService → DiscountCalculator (outgoing)
OrderValidator → (no outgoing within set)
OrderRepository → IDbConnection (external - Dapper)
DiscountCalculator → (no outgoing within set)
```

After scanning the codebase, `DiscountCalculator` is also used by `InvoiceService` (outside the batch).

### Classification

| Class | Classification | Belongs To | Rationale |
|-------|---------------|------------|-----------|
| OrderService | Standalone module | — | External callers (OrderController) depend on it; hides validation, persistence, and discount logic |
| OrderValidator | Subdivision | OrderService | Only called by OrderService; validates order rules internally |
| OrderRepository | Subdivision | OrderService | Only called by OrderService; handles persistence |
| DiscountCalculator | Shared infrastructure | — | Called by both OrderService AND InvoiceService (discovered via codebase scan) |
| IDbConnection | External dependency | — | NuGet package (System.Data) |

### Key insight
`DiscountCalculator` appeared to be a subdivision of `OrderService` from the batch alone. Only the codebase scan revealed its shared nature — it serves multiple standalone modules.

---

## Example 2 — Classitis Pattern

### Input files
- `UserNameValidator.cs`
- `UserEmailValidator.cs`
- `UserAgeValidator.cs`
- `UserValidationOrchestrator.cs`
- `UserService.cs`

### Classification

| Class | Classification | Belongs To | Rationale |
|-------|---------------|------------|-----------|
| UserService | Standalone module | — | External entry point |
| UserValidationOrchestrator | Subdivision | UserService | Only called by UserService |
| UserNameValidator | Subdivision | UserService (via orchestrator) | Only called by orchestrator |
| UserEmailValidator | Subdivision | UserService (via orchestrator) | Only called by orchestrator |
| UserAgeValidator | Subdivision | UserService (via orchestrator) | Only called by orchestrator |

### Flag: Classitis
4 subdivision classes for simple validation logic that could live inside `UserService` or a single `UserValidator`. Each tiny class adds interface cost without hiding meaningful complexity.

---

## Example 3 — Circular Dependency

### Input files
- `NotificationService.cs`
- `UserService.cs`

### Dependency analysis

```
NotificationService → UserService (to get user email)
UserService → NotificationService (to send welcome email on registration)
```

### Flag: Circular dependency
`NotificationService ↔ UserService`. This typically indicates a missing abstraction — extract an `INotificationSender` interface or use domain events to break the cycle.

---

## Example 4 — Transitive Discovery

### Input files provided
- `ReportGenerator.cs`

### Step-by-step discovery

1. **Read ReportGenerator.cs** → depends on `IDataSource`, `PdfFormatter`, `ReportCache`
2. **Scan for PdfFormatter** → found at `Formatters/PdfFormatter.cs` → depends on `ITemplateEngine`
3. **Scan for ReportCache** → found at `Caching/ReportCache.cs` → depends on `IDistributedCache` (NuGet — stop)
4. **Scan for ITemplateEngine** → found at `Templates/RazorTemplateEngine.cs` → depends on `Microsoft.AspNetCore.Razor` (NuGet — stop)
5. **Scan for IDataSource** → interface at `Data/IDataSource.cs`, implementations: `SqlDataSource.cs`, `ApiDataSource.cs`
6. **Scan incoming calls for ReportGenerator** → called by `ReportController`, `ScheduledReportJob`

### Final classification

| Class | File | Classification | Belongs To |
|-------|------|---------------|------------|
| ReportGenerator | ReportGenerator.cs | Standalone module | — |
| PdfFormatter | Formatters/PdfFormatter.cs | Subdivision | ReportGenerator |
| ReportCache | Caching/ReportCache.cs | Subdivision | ReportGenerator |
| RazorTemplateEngine | Templates/RazorTemplateEngine.cs | Subdivision | ReportGenerator (via PdfFormatter) |
| SqlDataSource | Data/SqlDataSource.cs | Subdivision | ReportGenerator |
| ApiDataSource | Data/ApiDataSource.cs | Subdivision | ReportGenerator |
| IDistributedCache | — | External dependency | — |
| Microsoft.AspNetCore.Razor | — | External dependency | — |
