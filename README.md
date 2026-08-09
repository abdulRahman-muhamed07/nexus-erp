# Nexus ERP — Business Suite

A complete, client-side Enterprise Resource Planning (ERP) application built with vanilla HTML, CSS, and JavaScript. Runs entirely in the browser with data persisted to `localStorage` — no backend required.

## Features

- **Dashboard** — Revenue KPIs, sales-by-category charts, top products, and live activity feed
- **Sales Orders** — Create, edit, view, filter, sort, and delete orders with automatic stock allocation
- **Inventory** — Stock levels, low-stock alerts, valuation, and quick adjustments
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

Then open http://localhost:8080/nexus-erp.html — any credentials work (demo environment).

## Tech

- Vanilla JS (ES6+)
- Chart.js 4 for visualizations
- Font Awesome 6 icons
- LocalStorage persistence

## Reset

Use **Settings → Reset Demo Data** to regenerate the demo dataset at any time.
