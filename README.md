# XeoTech ERP - Business Suite

A complete, client-side Enterprise Resource Planning (ERP) application built with vanilla HTML, CSS, and JavaScript. Runs entirely in the browser with data persisted to `localStorage` - no backend required.

## Features

- **Dashboard** - Revenue KPIs, sales-by-category charts, top products, and live activity feed
- **Sales Orders** - Create, edit, view, filter, sort, and delete orders with automatic stock allocation
- **Quotes** - Draft / Sent / Approved pipeline with one-click conversion into a live order
- **Role-Based Permissions** - Four roles with a strict capability ladder (create/edit/delete/export) plus module-level visibility: Viewer can't open Finance/HR/Settings, Analyst is read-only + exports (no HR), Manager has full operations but no deletions, Administrator is unrestricted. Restricted modules are hidden in the sidebar and direct navigation is blocked.
- **Payment Terms & Credit Limits** - Per-customer terms (Due on Receipt / Net 15 / Net 30 / Net 45 / Net 60) that drive invoice due dates, configurable credit limits with a live used/limit indicator, over-limit and account-hold blocking on new orders
- **Accounts Receivable Aging** - Unpaid invoices bucketed into Current / 1-30 / 31-60 / 61-90 / 90+ days with bucket totals in Finance
- **Budget vs Actual** - Set a monthly operating budget per expense category (Settings - Budget & Planning) and compare actual spend with variance and on-track/over status in Finance
- **Cash Flow Statement** - Operating / investing / financing breakdown for the current month from real collections, expenses, and asset purchases, plus a trailing-12-month net-cash trend chart
- **Inventory Costing & Stock Age** - Weighted-average costing on PO receipts and stock-in adjustments, plus a stock-aging panel (0-30 / 31-60 / 61-90 / 90+ days) and per-item age column
- **Fixed Assets & Depreciation** - Asset register with straight-line depreciation schedules, accumulated depreciation, net book value, disposal flow, and monthly depreciation reporting
- **Audit Log** - Searchable trail of every action (who/what/when) across all modules with user/module filters and CSV export; visible to Manager & Administrator
- **Configurable Billing** - Tax rate, shipping fee, and free-shipping threshold set in Settings, applied to new orders and quotes, plus per-order discount %
- **Returns & Refunds** - Record a return on a Delivered order (restore stock, log movement, apply refund) with refund-aware payment status
- **Auto-PO** - One-click purchase order generation for every item below its reorder level
- **Inventory** - Stock levels, low-stock alerts, valuation, quick adjustments, and a full stock-movement history (sales, cancellations, PO receipts, manual adjustments)
- **Payments** - Order-level payment tracking with Paid / Partially Paid / Unpaid status
- **Invoices** - Auto-generated from Delivered orders, linked back to the order, with Mark Paid flow that syncs to the order
- **Procurement** - Purchase orders (approve - ship - receive) and supplier management
- **Customers** - CRM with tiers, lifetime value, and flags by country
- **Human Resources** - Employee directory, departments, and payroll overview
- **Finance** - Revenue vs expenses, profit margin, receivables, AR aging, and invoice management
- **Reports** - Revenue vs target, top customers, and CSV/JSON exports
- **Settings** - Profile, light/dark theme, preferences, backup, and demo-data reset
- **Extras** - Global quick search (`/`), notifications, toast feedback, CSV exports

## Run locally

```bash
python -m http.server 8080
```

Then open http://localhost:8080/nexus-erp.html and sign in with a demo account:

| Role | Email | Password |
| --- | --- | --- |
| Administrator | `admin@nexuserp.io` | `admin123` |
| Manager | `manager@nexuserp.io` | `manager123` |
| Analyst | `analyst@nexuserp.io` | `analyst123` |
| Viewer | `viewer@nexuserp.io` | `viewer123` |

## Tech

- Vanilla JS (ES6+)
- Chart.js 4 for visualizations
- Font Awesome 6 icons
- LocalStorage persistence

## Reset

Use **Settings - Reset Demo Data** to regenerate the demo dataset at any time.
