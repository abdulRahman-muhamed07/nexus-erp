# Nexus ERP — Business Suite

A complete, client-side Enterprise Resource Planning (ERP) application built with vanilla HTML, CSS, and JavaScript. Runs entirely in the browser with data persisted to `localStorage` — no backend required.

## Features

- **Dashboard** — Revenue KPIs, sales-by-category charts, top products, and live activity feed
- **Sales Orders** — Create, edit, view, filter, sort, and delete orders with automatic stock allocation
- **Quotes** — Draft / Sent / Approved pipeline with one-click conversion into a live order
- **Role-Based Permissions** — Four roles with a strict capability ladder (create/edit/delete/export) **plus module-level visibility**: Viewer can't open Finance/HR/Settings, Analyst is read-only + exports (no HR), Manager has full operations but no deletions, Administrator is unrestricted. Restricted modules are hidden in the sidebar and direct navigation is blocked.
- **Audit Log** — Searchable trail of every action (who/what/when) across all modules with user/module filters and CSV export; visible to Manager & Administrator
- **Configurable Billing** — Tax rate, shipping fee, and free-shipping threshold set in Settings, applied to new orders and quotes, plus per-order discount %
- **Returns & Refunds** — Record a return on a Delivered order (restore stock, log movement, apply refund) with refund-aware payment status
- **Auto-PO** — One-click purchase order generation for every item below its reorder level
- **Inventory** — Stock levels, low-stock alerts, valuation, quick adjustments, and a full stock-movement history (sales, cancellations, PO receipts, manual adjustments)
- **Payments** — Order-level payment tracking with Paid / Partially Paid / Unpaid status
- **Invoices** — Auto-generated from Delivered orders, linked back to the order, with Mark Paid flow that syncs to the order
- **Procurement** — Purchase orders (approve → ship → receive) and supplier management
- **Customers** — CRM with tiers, lifetime value, and flags by country
- **Human Resources** — Employee directory, departments, and payroll overview
- **Finance** — Revenue vs expenses, profit margin, receivables, and invoice management
- **Reports** — Revenue vs target, top customers, and CSV/JSON exports
- **Settings** — Profile, light/dark theme, preferences, backup, and demo-data reset
- **Extras** — Global quick search (`/`), notifications, toast feedback, CSV exports

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

Use **Settings → Reset Demo Data** to regenerate the demo dataset at any time.
