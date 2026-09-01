  bindSearch('#ordersSearch',c);restoreFocus(c,'#ordersSearch');bindPager(c);
  $$('#view [data-ochip]').forEach(b=>b.onclick=()=>{c.status=b.dataset.ochip;c.page=1;rerender();});
  $$('#view [data-osort]').forEach(th=>th.onclick=()=>{const k=th.dataset.osort;if(c.sortKey===k)c.sortDir*=-1;else{c.sortKey=k;c.sortDir=(k==='date'||k==='total')?-1:1;}c.page=1;rerender();});
  $$('#view [data-oact]').forEach(b=>b.onclick=()=>{const o=db.orders.find(x=>x.id===b.dataset.id);if(!o)return;const a=b.dataset.oact;
    if(a==='view')openOrderView(o.id);
    if(a==='edit'){if(!guard('edit'))return;openOrderModal(o);}
    if(a==='dup'){if(!guard('create'))return;duplicateOrder(o.id);}
    if(a==='del'){if(!guard('delete'))return;confirmModal('Delete order',`This will permanently delete ${o.id} and restore its stock. This action cannot be undone.`,()=>{if(o.status!=='Cancelled')applyStock(o,1,'Order Deleted',o.id);db.orders.splice(db.orders.indexOf(o),1);addActivity('fa-trash','Order '+o.id+' deleted');audit('fa-trash','Order deleted','Orders',o.id);save();toast('Order deleted','fa-trash','warn');rerender();});}});
  const nn=$('#btnOrdersNew');if(nn)nn.onclick=()=>{if(guard('create'))openOrderModal(null);};
  const ne=$('#btnOrdersExport');if(ne)ne.onclick=()=>{if(!guard('export'))return;exportCSV('orders.csv',['Order ID','Customer','Date','Status','Subtotal','Tax','Shipping','Total'],db.orders.map(o=>[o.id,o.customerName,fmtDate(o.date),o.status,o.sub.toFixed(2),o.tax.toFixed(2),o.ship.toFixed(2),o.total.toFixed(2)]));audit('fa-download','Exported','Orders','orders.csv');save();toast('Orders exported to CSV');};
}

/* ----- Inventory ----- */
function viewInventory(){
  const tabs=`<div class="tabs"><button class="tab ${invTab==='products'?'on':''}" data-itab="products"><i class="fa-solid fa-box"></i> Products</button><button class="tab ${invTab==='movements'?'on':''}" data-itab="movements"><i class="fa-solid fa-arrows-rotate"></i> Stock Movements</button></div>`;
  if(invTab==='movements'){
    const c=ctl.movements;
    let list=[...db.movements];
    if(c.q){const q=c.q.toLowerCase();list=list.filter(m=>m.productName.toLowerCase().includes(q)||m.reason.toLowerCase().includes(q)||m.ref.toLowerCase().includes(q));}
    const {slice,pages,total}=paginate(list,c);
    const rows=slice.map(m=>`<tr><td class="muted">${fmtDT(m.time)}</td><td>${avatar(m.productName,26)} ${esc(m.productName)}</td><td class="strong ${m.delta>=0?'muted':''}" style="color:${m.delta>=0?'var(--green)':'var(--red)'}">${m.delta>=0?'+':''}${m.delta}</td><td>${badge(m.reason)}</td><td class="mono muted">${esc(m.ref)||'—'}</td><td>${esc(m.by)}</td></tr>`).join('');
    return tabs+`<div class="toolbar"><div class="search-box"><i class="fa-solid fa-magnifying-glass"></i><input id="mvSearch" placeholder="Search product, reason or reference…" value="${esc(c.q)}"></div><div class="spacer"></div><span class="muted small">${total} movement${total===1?'':'s'} recorded</span></div>
    <div class="card"><div class="tbl-wrap"><table class="tbl"><thead><tr><th>When</th><th>Product</th><th>Delta</th><th>Reason</th><th>Reference</th><th>By</th></tr></thead><tbody>${rows}</tbody></table>${slice.length?'':'<div class="empty"><i class="fa-regular fa-folder-open"></i>No movements recorded</div>'}</div>${pagerHtml(c,pages,total)}</div>`;
  }
  const c=ctl.inventory;
  const cats=[...new Set(db.products.map(p=>p.category))];
  let list=[...db.products];
  if(c.cat!=='all')list=list.filter(p=>p.category===c.cat);
  if(c.q){const q=c.q.toLowerCase();list=list.filter(p=>p.name.toLowerCase().includes(q)||p.sku.toLowerCase().includes(q)||p.supplier.toLowerCase().includes(q));}
  const {slice,pages,total}=paginate(list,c);
  const low=db.products.filter(p=>p.stock<=p.reorder);
  const ageBuckets=[['0–30 days',0,30],['31–60 days',31,60],['61–90 days',61,90],['90+ days',91,1e9]];
  const ageStrip=ageBuckets.map(b=>{const items=db.products.filter(p=>{const a=stockAgeDays(p);return a!=null&&a>=b[1]&&a<=b[2];});const val=items.reduce((s,p)=>s+p.stock*p.cost,0);return `<div class="card" style="flex:1;min-width:140px;padding:12px 14px;margin:0"><div class="muted small">Stock ${b[0]}</div><div class="strong" style="font-size:15px">${items.length} item${items.length===1?'':'s'}</div><div class="muted small">${money(Math.round(val))}</div></div>`;}).join('');
  const rows=slice.map(p=>{const st=p.stock===0?'Out of Stock':p.stock<=p.reorder?'Low Stock':'In Stock';const age=stockAgeDays(p);
    return `<tr><td class="mono muted">${p.sku}</td><td><div style="display:flex;align-items:center;gap:10px">${avatar(p.name,28)}<div><b style="font-size:13px">${esc(p.name)}</b><div class="muted small">${esc(p.supplier)}</div></div></div></td><td><span class="badge b-gray">${p.category}</span></td><td class="strong">${money2(p.price)}</td><td class="muted">${money2(p.cost)}</td><td><div class="stepper">${can('edit')?`<button class="icon-btn sm" data-adj="-1" data-id="${p.id}" ${p.stock<=0?'disabled':''}><i class="fa-solid fa-minus"></i></button>`:'<span></span>'}<span class="stock-n">${p.stock}</span>${can('edit')?`<button class="icon-btn sm" data-adj="1" data-id="${p.id}"><i class="fa-solid fa-plus"></i></button>`:'<span></span>'}</div></td><td>${badge(st)}</td><td class="muted">${age==null?'—':age+' d'}</td><td class="muted">${money(p.stock*p.cost)}</td><td><div class="row-actions">${can('edit')?`<button class="icon-btn sm" data-pact2="edit" data-id="${p.id}"><i class="fa-regular fa-pen-to-square"></i></button>`:''}${can('delete')?`<button class="icon-btn sm danger" data-pact2="del" data-id="${p.id}"><i class="fa-regular fa-trash-can"></i></button>`:''}</div></td></tr>`;}).join('');
  return tabs+`${low.length?`<div class="alert-strip"><i class="fa-solid fa-boxes-stacked"></i><span><b>${low.length}</b> item${low.length>1?'s':''} need reordering.</span>${can('create')?`<button class="btn btn-primary btn-xs" id="btnAutoPo" style="margin-left:auto"><i class="fa-solid fa-truck-fast"></i> Auto-PO</button>`:''}</div>`:''}
  <div style="display:flex;gap:10px;flex-wrap:wrap;margin:14px 0">${ageStrip}</div>
  <div class="toolbar"><div class="toolbar-right" style="margin-right:auto"><div class="search-box"><i class="fa-solid fa-magnifying-glass"></i><input id="invSearch" placeholder="Search name, SKU, supplier…" value="${esc(c.q)}"></div><select id="invCat" style="width:170px"><option value="all">All Categories</option>${cats.map(x=>`<option ${c.cat===x?'selected':''}>${x}</option>`).join('')}</select></div><div class="toolbar-right">${can('export')?`<button class="btn btn-ghost btn-sm" id="btnInvExport"><i class="fa-solid fa-download"></i> Export</button>`:''}${can('create')?`<button class="btn btn-primary btn-sm" id="btnInvNew"><i class="fa-solid fa-plus"></i> Add Product</button>`:''}</div></div>
  <div class="card"><div class="tbl-wrap"><table class="tbl"><thead><tr><th>SKU</th><th>Product</th><th>Category</th><th>Price</th><th>Avg Cost</th><th>Stock</th><th>Status</th><th>Age</th><th>Stock Value</th><th></th></tr></thead><tbody>${rows}</tbody></table>${slice.length?'':'<div class="empty"><i class="fa-regular fa-folder-open"></i>No products found</div>'}</div>${pagerHtml(c,pages,total)}</div>`;
}
function mountInventory(){
  const c=ctl.inventory;
  $$('#view [data-itab]').forEach(b=>b.onclick=()=>{invTab=b.dataset.itab;rerender();});
  if(invTab==='movements'){
    const mc=ctl.movements;
    bindSearch('#mvSearch',mc);restoreFocus(mc,'#mvSearch');bindPager(mc);
    return;
  }
  bindSearch('#invSearch',c);restoreFocus(c,'#invSearch');bindPager(c);
  $('#invCat').onchange=e=>{c.cat=e.target.value;c.page=1;rerender();};
  $$('#view [data-adj]').forEach(b=>b.onclick=()=>{if(!guard('edit'))return;const p=db.products.find(x=>x.id===b.dataset.id);const d=+b.dataset.adj;if(d>0)restock(p,d,p.cost);else p.stock=Math.max(0,p.stock+d);logMovement(p.id,d,'Manual Adjustment','');addActivity('fa-arrows-rotate',`${d>0?'Stock added':'Stock removed'}: ${Math.abs(d)} × ${p.name}`);audit('fa-arrows-rotate',`Stock ${d>0?'increased':'decreased'}`,'Inventory',p.name,`${d} units → ${p.stock} · avg cost ${money2(p.cost)}`);save();rerender();});
  $$('#view [data-pact2]').forEach(b=>b.onclick=()=>{const p=db.products.find(x=>x.id===b.dataset.id);
    if(b.dataset.pact2==='edit'){if(!guard('edit'))return;openProductModal(p);}
    else{if(!guard('delete'))return;confirmModal('Delete product',`Remove "${p.name}" from inventory? Existing orders keep their history.`,()=>{db.products.splice(db.products.indexOf(p),1);addActivity('fa-trash','Product '+p.name+' removed');audit('fa-trash','Product deleted','Inventory',p.name);save();toast('Product deleted','fa-trash','warn');rerender();});}});
  const ni=$('#btnInvNew');if(ni)ni.onclick=()=>{if(guard('create'))openProductModal(null);};
  const nx=$('#btnInvExport');if(nx)nx.onclick=()=>{if(!guard('export'))return;exportCSV('inventory.csv',['SKU','Name','Category','Supplier','Price','Cost','Stock','Reorder Level','Stock Value'],db.products.map(p=>[p.sku,p.name,p.category,p.supplier,p.price.toFixed(2),p.cost.toFixed(2),p.stock,p.reorder,(p.stock*p.cost).toFixed(2)]));audit('fa-download','Exported','Inventory','inventory.csv');save();toast('Inventory exported to CSV');};
  const ap=$('#btnAutoPo');if(ap)ap.onclick=()=>{
    if(!guard('create'))return;
    const n=createAutoPo();
    if(n){save();toast(`${n} PO${n>1?'s':''} created for low-stock items`);rerender();}
  };
}
function createAutoPo(){
  if(!guard('create'))return 0;
  const low=db.products.filter(p=>p.stock<=p.reorder);
  if(!low.length){toast('No items need reordering','fa-circle-check');return 0;}
  let n=0;
  low.forEach(p=>{const sup=db.suppliers.find(s=>s.name===p.supplier)||db.suppliers[0];if(!sup)return;
    const qty=Math.max(p.reorder*2-p.stock,1);
    db.pos.unshift({id:'PO-'+(db.seq.po++),supplierId:sup.id,supplierName:sup.name,productId:p.id,productName:p.name,qty,cost:p.cost,status:'Pending',eta:new Date(Date.now()+14*864e5).toISOString(),created:new Date().toISOString()});n++;});
  audit('fa-truck-fast','Auto-PO created','Procurement',n+' PO(s)',`Restock ${low.length} low-stock item(s)`);
  addActivity('fa-truck-fast',`Auto-PO: ${n} purchase order${n>1?'s':''} created for low-stock items`);
  pushNotif('fa-truck-fast','Auto-PO generated',`${n} PO(s) created from reorder list`);
  return n;
}

/* ----- Customers ----- */
function viewCustomers(){
  const c=ctl.customers;
  let list=[...db.customers];
  if(c.tier!=='all')list=list.filter(x=>x.tier===c.tier);
  if(c.q){const q=c.q.toLowerCase();list=list.filter(x=>x.company.toLowerCase().includes(q)||x.name.toLowerCase().includes(q)||x.email.toLowerCase().includes(q));}
  const {slice,pages,total}=paginate(list,c);
  const rows=slice.map(x=>{const ords=db.orders.filter(o=>o.customerId===x.id&&o.status!=='Cancelled');const spent=ords.reduce((s,o)=>s+o.total,0);const cs=creditState(x);
    return `<tr><td><div style="display:flex;align-items:center;gap:10px">${avatar(x.company,32)}<div><b style="font-size:13px">${esc(x.company)}</b><div class="muted small">${esc(x.name)}</div></div></div></td><td class="muted">${esc(x.email)}</td><td class="muted">${FLAGS[x.country]||'🌐'} ${esc(x.country)}</td><td>${badge(x.tier)}</td><td>${x.hold?'<span class="badge" style="background:rgba(239,68,68,.12);color:var(--red)">Held</span>':'<span class="badge" style="background:rgba(99,102,241,.12);color:var(--primary)">'+esc(x.terms)+'</span>'}<div class="muted small" style="${cs.over?'color:var(--red);font-weight:600':''}">${cs.limit>0?money(cs.used)+' / '+money(cs.limit):'No limit'}</div></td><td class="strong">${ords.length}</td><td class="strong">${money(spent)}</td><td class="muted">${fmtDate(x.since)}</td><td><div class="row-actions">${can('edit')?`<button class="icon-btn sm" data-cact="edit" data-id="${x.id}"><i class="fa-regular fa-pen-to-square"></i></button>`:''}${can('delete')?`<button class="icon-btn sm danger" data-cact="del" data-id="${x.id}"><i class="fa-regular fa-trash-can"></i></button>`:''}</div></td></tr>`;}).join('');
  return `<div class="toolbar"><div class="toolbar-right" style="margin-right:auto"><div class="search-box"><i class="fa-solid fa-magnifying-glass"></i><input id="cusSearch" placeholder="Search company, contact, email…" value="${esc(c.q)}"></div><select id="cusTier" style="width:140px"><option value="all">All Tiers</option>${['VIP','Standard','New'].map(t=>`<option ${c.tier===t?'selected':''}>${t}</option>`).join('')}</select></div><div class="toolbar-right">${can('export')?`<button class="btn btn-ghost btn-sm" id="btnCusExport"><i class="fa-solid fa-download"></i> Export</button>`:''}${can('create')?`<button class="btn btn-primary btn-sm" id="btnCusNew"><i class="fa-solid fa-user-plus"></i> Add Customer</button>`:''}</div></div>
  <div class="card"><div class="tbl-wrap"><table class="tbl"><thead><tr><th>Company</th><th>Email</th><th>Country</th><th>Tier</th><th>Terms / Credit</th><th>Orders</th><th>Lifetime Value</th><th>Customer Since</th><th></th></tr></thead><tbody>${rows}</tbody></table>${slice.length?'':'<div class="empty"><i class="fa-regular fa-folder-open"></i>No customers found</div>'}</div>${pagerHtml(c,pages,total)}</div>`;
}
function mountCustomers(){
  const c=ctl.customers;
  bindSearch('#cusSearch',c);restoreFocus(c,'#cusSearch');bindPager(c);
  $('#cusTier').onchange=e=>{c.tier=e.target.value;c.page=1;rerender();};
  $$('#view [data-cact]').forEach(b=>b.onclick=()=>{const x=db.customers.find(v=>v.id===b.dataset.id);
    if(b.dataset.cact==='edit'){if(!guard('edit'))return;openCustomerModal(x);}
    else{if(!guard('delete'))return;confirmModal('Delete customer',`Remove ${x.company}? Order history will be preserved.`,()=>{db.customers.splice(db.customers.indexOf(x),1);audit('fa-trash','Customer deleted','Customers',x.company);save();toast('Customer deleted','fa-trash','warn');rerender();});}});
  const nc=$('#btnCusNew');if(nc)nc.onclick=()=>{if(guard('create'))openCustomerModal(null);};
  const nx=$('#btnCusExport');if(nx)nx.onclick=()=>{if(!guard('export'))return;exportCSV('customers.csv',['Company','Contact','Email','Phone','Country','Tier','Terms','Credit Limit','Hold'],db.customers.map(x=>[x.company,x.name,x.email,x.phone,x.country,x.tier,x.terms,x.creditLimit||'',x.hold?'Yes':'No']));audit('fa-download','Exported','Customers','customers.csv');save();toast('Customers exported to CSV');};
}

/* ----- HR ----- */
function viewHR(){
  const c=ctl.hr;
  const depts=[...new Set(db.employees.map(e=>e.dept))];
  let list=[...db.employees];
  if(c.dept!=='all')list=list.filter(e=>e.dept===c.dept);
  if(c.q){const q=c.q.toLowerCase();list=list.filter(e=>e.name.toLowerCase().includes(q)||e.role.toLowerCase().includes(q));}
  const {slice,pages,total}=paginate(list,c);
  const payroll=db.employees.filter(e=>e.status!=='On Leave').reduce((s,e)=>s+e.salary,0);
  const chips=depts.map(d=>`<button class="chip ${c.dept===d?'on':''}" data-hchip="${d}">${d}<span>${db.employees.filter(e=>e.dept===d).length}</span></button>`).join('');
  const rows=slice.map(e=>`<tr><td><div style="display:flex;align-items:center;gap:10px">${avatar(e.name,32)}<div><b style="font-size:13px">${esc(e.name)}</b><div class="muted small">${esc(e.email)}</div></div></div></td><td>${esc(e.role)}</td><td>${badge(e.dept)}</td><td class="strong">${money(e.salary)}</td><td class="muted">${fmtDate(e.hired)}</td><td>${badge(e.status)}</td><td><div class="row-actions">${can('edit')?`<button class="icon-btn sm" data-hact="edit" data-id="${e.id}"><i class="fa-regular fa-pen-to-square"></i></button>`:''}${can('delete')?`<button class="icon-btn sm danger" data-hact="del" data-id="${e.id}"><i class="fa-regular fa-trash-can"></i></button>`:''}</div></td></tr>`).join('');
  return `<div class="alert-strip" style="border-color:rgba(99,102,241,.3);background:rgba(99,102,241,.08)"><i class="fa-solid fa-hand-holding-dollar" style="color:var(--primary)"></i><span>Monthly payroll (active staff): <b>${money(Math.round(payroll/12))}</b> · ${db.employees.length} employees across ${depts.length} departments</span></div>
  <div class="toolbar"><div class="chips"><button class="chip ${c.dept==='all'?'on':''}" data-hchip="all">All<span>${db.employees.length}</span></button>${chips}</div><div class="toolbar-right"><div class="search-box"><i class="fa-solid fa-magnifying-glass"></i><input id="hrSearch" placeholder="Search name or role…" value="${esc(c.q)}"></div>${can('create')?`<button class="btn btn-primary btn-sm" id="btnHrNew"><i class="fa-solid fa-user-plus"></i> Add Employee</button>`:''}</div></div>
  <div class="card"><div class="tbl-wrap"><table class="tbl"><thead><tr><th>Employee</th><th>Role</th><th>Department</th><th>Salary</th><th>Hired</th><th>Status</th><th></th></tr></thead><tbody>${rows}</tbody></table>${slice.length?'':'<div class="empty"><i class="fa-regular fa-folder-open"></i>No employees found</div>'}</div>${pagerHtml(c,pages,total)}</div>`;
}
function mountHR(){
  const c=ctl.hr;
  bindSearch('#hrSearch',c);restoreFocus(c,'#hrSearch');bindPager(c);
  $$('#view [data-hchip]').forEach(b=>b.onclick=()=>{c.dept=b.dataset.hchip;c.page=1;rerender();});
  $$('#view [data-hact]').forEach(b=>b.onclick=()=>{const e=db.employees.find(v=>v.id===b.dataset.id);
    if(b.dataset.hact==='edit'){if(!guard('edit'))return;openEmployeeModal(e);}
    else{if(!guard('delete'))return;confirmModal('Delete employee',`Remove ${e.name} from the directory?`,()=>{db.employees.splice(db.employees.indexOf(e),1);audit('fa-trash','Employee deleted','HR',e.name);save();toast('Employee removed','fa-trash','warn');rerender();});}});
  const ne=$('#btnHrNew');if(ne)ne.onclick=()=>{if(guard('create'))openEmployeeModal(null);};
}

/* ----- Procurement ----- */
function viewProcurement(){
  const tabs=`<div class="tabs"><button class="tab ${procTab==='po'?'on':''}" data-ptab="po"><i class="fa-solid fa-file-lines"></i> Purchase Orders</button><button class="tab ${procTab==='sup'?'on':''}" data-ptab="sup"><i class="fa-solid fa-building"></i> Suppliers</button></div>`;
  if(procTab==='po'){
    const rows=db.pos.map(po=>{let acts='';
      if(po.status==='Pending'&&can('edit'))acts+=`<button class="btn btn-ghost btn-xs" data-poact="approve" data-id="${po.id}">Approve</button>`;
      if(po.status==='Approved'&&can('edit'))acts+=`<button class="btn btn-ghost btn-xs" data-poact="ship" data-id="${po.id}">Mark Shipped</button>`;
      if((po.status==='Approved'||po.status==='In Transit')&&can('edit'))acts+=`<button class="btn btn-primary btn-xs" data-poact="receive" data-id="${po.id}">Receive</button>`;
      if(po.status==='Received')acts+=`<span class="muted small"><i class="fa-solid fa-circle-check" style="color:var(--green)"></i> Completed</span>`;
      if(can('delete'))acts+=`<button class="icon-btn sm danger" data-poact="del" data-id="${po.id}"><i class="fa-regular fa-trash-can"></i></button>`;
      return `<tr><td class="mono strong">${po.id}</td><td>${esc(po.supplierName)}</td><td>${esc(po.productName)}</td><td class="strong">${po.qty}</td><td class="strong">${money(po.qty*po.cost)}</td><td class="muted">${fmtDate(po.eta)}</td><td>${badge(po.status)}</td><td><div class="row-actions">${acts}</div></td></tr>`;}).join('');
    return tabs+`<div class="toolbar"><span class="muted small">${db.pos.length} purchase orders · ${db.pos.filter(p=>p.status!=='Received').length} open</span><div class="spacer"></div>${can('create')?`<button class="btn btn-primary btn-sm" id="btnPoNew"><i class="fa-solid fa-plus"></i> New Purchase Order</button>`:''}</div>
    <div class="card"><div class="tbl-wrap"><table class="tbl"><thead><tr><th>PO</th><th>Supplier</th><th>Product</th><th>Qty</th><th>Cost</th><th>ETA</th><th>Status</th><th></th></tr></thead><tbody>${rows}</tbody></table></div></div>`;
  }
  const rows=db.suppliers.map(s=>`<tr><td><div style="display:flex;align-items:center;gap:10px">${avatar(s.name,30)}<b style="font-size:13px">${esc(s.name)}</b></div></td><td>${esc(s.contact)}</td><td class="muted">${FLAGS[s.country]||'🌐'} ${esc(s.country)}</td><td class="muted">${esc(s.email)}</td><td>${stars(s.rating)}</td><td class="strong">${db.products.filter(p=>p.supplier===s.name).length}</td><td><div class="row-actions">${can('edit')?`<button class="icon-btn sm" data-sact="edit" data-id="${s.id}"><i class="fa-regular fa-pen-to-square"></i></button>`:''}${can('delete')?`<button class="icon-btn sm danger" data-sact="del" data-id="${s.id}"><i class="fa-regular fa-trash-can"></i></button>`:''}</div></td></tr>`).join('');
  return tabs+`<div class="toolbar"><span class="muted small">${db.suppliers.length} active suppliers</span><div class="spacer"></div>${can('create')?`<button class="btn btn-primary btn-sm" id="btnSupNew"><i class="fa-solid fa-plus"></i> Add Supplier</button>`:''}</div>
  <div class="card"><div class="tbl-wrap"><table class="tbl"><thead><tr><th>Supplier</th><th>Contact</th><th>Country</th><th>Email</th><th>Rating</th><th>Products</th><th></th></tr></thead><tbody>${rows}</tbody></table></div></div>`;
}
function mountProcurement(){
  $$('#view [data-ptab]').forEach(b=>b.onclick=()=>{procTab=b.dataset.ptab;rerender();});
  $$('#view [data-poact]').forEach(b=>b.onclick=()=>{const po=db.pos.find(x=>x.id===b.dataset.id);const a=b.dataset.poact;
    if(a!=='del'&&!guard('edit'))return;
    if(a==='approve'){po.status='Approved';audit('fa-check','PO approved','Procurement',po.id);toast(po.id+' approved');}
    if(a==='ship'){po.status='In Transit';audit('fa-truck','PO shipped','Procurement',po.id);toast(po.id+' marked in transit','fa-truck','info');}
    if(a==='receive'){po.status='Received';const p=db.products.find(x=>x.id===po.productId);if(p){const prev=p.cost;restock(p,po.qty,po.cost);logMovement(p.id,po.qty,'PO Received',po.id);addActivity('fa-box',`Stock received: ${po.qty} × ${po.productName}`);audit('fa-box','PO received','Procurement',po.id,`${po.qty} × ${po.productName} · avg cost ${money2(prev)} → ${money2(p.cost)}`);pushNotif('fa-box','Stock received',`${po.qty} × ${po.productName} from ${po.supplierName}`);}else{addActivity('fa-box',`Stock received: ${po.qty} × ${po.productName}`);audit('fa-box','PO received','Procurement',po.id,`${po.qty} × ${po.productName}`);}toast(po.id+' received — stock updated');}
    if(a==='del'){if(!guard('delete'))return;confirmModal('Delete PO','Delete '+po.id+'? This does not affect stock.',()=>{db.pos.splice(db.pos.indexOf(po),1);audit('fa-trash','PO deleted','Procurement',po.id);save();toast('PO deleted','fa-trash','warn');rerender();});return;}
    save();rerender();});
  $$('#view [data-sact]').forEach(b=>b.onclick=()=>{const s=db.suppliers.find(x=>x.id===b.dataset.id);
    if(b.dataset.sact==='edit'){if(!guard('edit'))return;openSupplierModal(s);}
    else{if(!guard('delete'))return;confirmModal('Delete supplier',`Remove ${s.name}?`,()=>{db.suppliers.splice(db.suppliers.indexOf(s),1);audit('fa-trash','Supplier deleted','Procurement',s.name);save();toast('Supplier deleted','fa-trash','warn');rerender();});}});
  const np=$('#btnPoNew');if(np)np.onclick=()=>{if(guard('create'))openPoModal();};
  const ns=$('#btnSupNew');if(ns)ns.onclick=()=>{if(guard('create'))openSupplierModal(null);};
}

/* ----- Finance ----- */
function viewFinance(){
  const {rev}=monthAgg();const exp=expSeries(rev);
  const outstanding=db.invoices.filter(i=>i.status!=='Paid').reduce((s,i)=>s+i.amount,0);
  const profit=rev[11]-exp[11];const margin=rev[11]?Math.round(profit/rev[11]*100):0;
  const rows=db.invoices.map(i=>`<tr><td class="mono strong">${i.id}</td><td>${avatar(i.customerName,26)} ${esc(i.customerName)}</td><td class="mono muted">${i.orderId?esc(i.orderId):'—'}</td><td class="muted">${fmtDate(i.issued)}</td><td class="muted">${fmtDate(i.due)}</td><td class="strong">${money(i.amount)}</td><td>${badge(i.status)}</td><td><div class="row-actions">${i.status!=='Paid'&&can('edit')?`<button class="btn btn-ghost btn-xs" data-iact="pay" data-id="${i.id}"><i class="fa-solid fa-check"></i> Mark Paid</button>`:(i.status==='Paid'?`<span class="muted small">Settled ${i.paidOn?fmtDate(i.paidOn):''}</span>`:'')}${can('delete')?`<button class="icon-btn sm danger" data-iact="del" data-id="${i.id}"><i class="fa-regular fa-trash-can"></i></button>`:''}</div></td></tr>`).join('');
  const openInv=db.invoices.filter(i=>i.status!=='Paid');
  const agBuckets=[['Current',0,1e9,0],['1–30 days',1,30,0],['31–60 days',31,60,0],['61–90 days',61,90,0],['90+ days',91,1e9,0]];
  const agList=openInv.map(i=>({...i,days:Math.max(0,Math.floor((Date.now()-new Date(i.due))/864e5))})).sort((a,b)=>b.days-a.days);
  agBuckets.forEach(b=>{b[3]=agList.filter(r=>r.days>=b[1]&&r.days<=b[2]).reduce((s,r)=>s+r.amount,0);});
  const arows=agList.map(i=>`<tr><td class="mono strong">${i.id}</td><td>${avatar(i.customerName,26)} ${esc(i.customerName)}</td><td class="muted">${fmtDate(i.due)}</td><td class="strong" style="${i.days>0?'color:var(--red)':''}">${i.days>0?i.days+' d':'Due now'}</td><td>${badge(i.days===0?'Current':(i.days<=30?'1–30':(i.days<=60?'31–60':(i.days<=90?'61–90':'90+'))))}</td><td class="strong">${money(i.amount)}</td></tr>`).join('');
  const bdgRows=EXP_RATIOS.map(([cat])=>{const b=catBudget(cat),a=catActual(cat),vr=a-b,ov=vr>0,pb=b?Math.round(vr/b*100):0,w=b?Math.min(100,Math.round(a/b*100)):0;
    return `<tr><td>${esc(cat)}</td><td class="strong">${money(b)}</td><td class="strong">${money(a)}</td><td><div style="max-width:110px"><div style="height:6px;background:var(--border);border-radius:99px;overflow:hidden"><div style="height:100%;width:${w}%;background:${ov?'var(--red)':'var(--green)'};border-radius:99px"></div></div></div></td><td class="strong" style="color:${ov?'var(--red)':'var(--green)'}">${vr>=0?'+':''}${money(vr)}<div class="muted small">${pb>=0?'+':''}${pb}%</div></td><td>${ov?'<span class="badge b-red">Over</span>':'<span class="badge b-green">On Track</span>'}</td></tr>`;}).join('');
  const bdgTot=EXP_RATIOS.reduce((s,[c])=>s+catBudget(c),0);
  const now2=new Date(),y2=now2.getFullYear(),m2=now2.getMonth();
  const cfOpIn=incomeMonth(y2,m2),cfOpOut=exp[11],cfInv=assetBuyMonth(y2,m2),cfNet=cfOpIn-cfOpOut-cfInv;
  const cfRow=(ico,label,val,color)=>`<div style="display:flex;justify-content:space-between;align-items:center;padding:9px 0;border-bottom:1px solid var(--border)"><div class="muted"><i class="fa-solid ${ico}" style="color:${color};margin-right:8px;width:14px"></i>${label}</div><b style="${color?`color:${color};`:''}">${val}</b></div>`;
  const nowLbl=new Date().toLocaleDateString('en-US',{month:'long',year:'numeric'});
  return `<div class="grid g-4 mb">
    <div class="card kpi"><div class="kpi-top"><div class="kpi-ico c4"><i class="fa-solid fa-arrow-trend-up"></i></div>${spark(rev.slice(-8),'#10b981')}</div><div class="kpi-val" data-num="${rev[11]}" data-fmt="money">${money(rev[11])}</div><div class="kpi-label">Revenue (this month)</div><div class="kpi-sub">${deltaHtml(pct(rev[11],rev[10]))}</div></div>
    <div class="card kpi"><div class="kpi-top"><div class="kpi-ico c3"><i class="fa-solid fa-arrow-trend-down"></i></div>${spark(exp.slice(-8),'#f59e0b')}</div><div class="kpi-val" data-num="${exp[11]}" data-fmt="money">${money(exp[11])}</div><div class="kpi-label">Expenses (this month)</div><div class="kpi-sub">${deltaHtml(pct(exp[11],exp[10]))}</div></div>
    <div class="card kpi"><div class="kpi-top"><div class="kpi-ico c1"><i class="fa-solid fa-scale-balanced"></i></div>${spark(rev.map((v,i)=>v-exp[i]).slice(-8),'#6366f1')}</div><div class="kpi-val" data-num="${profit}" data-fmt="money">${money(profit)}</div><div class="kpi-label">Net Profit · ${margin}% margin</div><div class="kpi-sub">${deltaHtml(pct(profit,rev[10]-exp[10]))}</div></div>
    <div class="card kpi"><div class="kpi-top"><div class="kpi-ico c2"><i class="fa-solid fa-hourglass-half"></i></div></div><div class="kpi-val" data-num="${outstanding}" data-fmt="money">${money(outstanding)}</div><div class="kpi-label">Outstanding Receivables</div><div class="kpi-sub"><span class="vs">${db.invoices.filter(i=>i.status!=='Paid').length} open invoice(s)</span></div></div>
  </div>
  <div class="grid g-21 mb">
    <div class="card"><div class="card-h"><div><h3>Revenue vs Expenses</h3><p>Trailing 12 months</p></div></div><div class="card-b"><div class="chart-box"><canvas id="chFin"></canvas></div></div></div>
    <div class="card"><div class="card-h"><div><h3>Expense Breakdown</h3><p>YTD allocation</p></div></div><div class="card-b"><div class="chart-box"><canvas id="chExp"></canvas></div></div></div>
  </div>
  <div class="grid g-21 mb">
    <div class="card"><div class="card-h"><div><h3>Budget vs Actual</h3><p>This month · ${money(bdgTot)} total budget</p></div></div><div class="tbl-wrap"><table class="tbl"><thead><tr><th>Category</th><th>Budget</th><th>Actual</th><th>Spend</th><th>Variance</th><th>Status</th></tr></thead><tbody>${bdgRows}</tbody></table></div><div class="chart-box"><canvas id="chBdg"></canvas></div></div>
    <div class="card"><div class="card-h"><div><h3>Cash Flow Statement</h3><p>${nowLbl}</p></div></div><div class="card-b" style="padding:4px 16px 0">
      ${cfRow('fa-arrow-down-long','Cash from operations (collections)',money(cfOpIn),'var(--green)')}
      ${cfRow('fa-arrow-up-long','Operating expenses','−'+money(cfOpOut),'var(--red)')}
      ${cfRow('fa-box','Capital expenditure (asset purchases)',cfInv?'−'+money(cfInv):money(0),cfInv?'var(--red)':'')}
      ${cfRow('fa-coins','Financing (loans / equity)',money(0),'')}
      <div style="display:flex;justify-content:space-between;align-items:center;padding:12px 0"><b style="font-size:13px">Net change in cash</b><b style="font-size:16px;color:${cfNet>=0?'var(--green)':'var(--red)'}">${cfNet>=0?'+':''}${money(cfNet)}</b></div>
    </div><div class="chart-box"><canvas id="chCash"></canvas></div></div>
