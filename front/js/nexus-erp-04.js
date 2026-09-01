  </div>
  <div class="card"><div class="card-h"><div><h3>Invoices &amp; Receivables</h3><p>${db.invoices.length} invoices</p></div>${can('create')?`<button class="btn btn-primary btn-sm" id="btnInvNew"><i class="fa-solid fa-plus"></i> New Invoice</button>`:''}</div><div class="tbl-wrap"><table class="tbl"><thead><tr><th>Invoice</th><th>Customer</th><th>Source</th><th>Issued</th><th>Due</th><th>Amount</th><th>Status</th><th></th></tr></thead><tbody>${rows}</tbody></table></div></div>
  <div class="card"><div class="card-h"><div><h3>Accounts Receivable Aging</h3><p>${openInv.length} outstanding — aging from invoice due date</p></div></div><div style="display:flex;gap:10px;flex-wrap:wrap;padding:14px 16px 0">${agBuckets.map(b=>`<div style="flex:1;min-width:120px;padding:10px 12px;border:1px solid var(--border);border-radius:10px"><div class="muted small">${b[0]}</div><div class="strong">${money(b[3])}</div><div class="muted small">${agList.filter(r=>r.days>=b[1]&&r.days<=b[2]).length} invoice(s)</div></div>`).join('')}</div><div class="tbl-wrap"><table class="tbl"><thead><tr><th>Invoice</th><th>Customer</th><th>Due Date</th><th>Days Overdue</th><th>Bucket</th><th>Amount</th></tr></thead><tbody>${arows}</tbody></table>${openInv.length?'':'<div class="empty"><i class="fa-regular fa-circle-check"></i>No outstanding receivables</div>'}</div></div>`;
}
function mountFinance(){
  animateKpis();
  $$('#view [data-iact]').forEach(b=>b.onclick=()=>{const i=db.invoices.find(x=>x.id===b.dataset.id);
    if(b.dataset.iact==='pay'){if(!guard('edit'))return;i.status='Paid';i.paidOn=new Date().toISOString();const o=db.orders.find(x=>x.id===i.orderId);if(o){o.payments=o.payments||[];o.payments.push({id:'p'+Date.now(),amount:i.amount,method:'Bank Transfer',date:new Date().toISOString()});}addActivity('fa-file-invoice-dollar',`Invoice ${i.id} paid by ${i.customerName}`);audit('fa-file-invoice-dollar','Invoice paid','Finance',i.id,`${money2(i.amount)} · ${i.customerName}`);pushNotif('fa-file-invoice-dollar','Payment received',`${i.id} settled — ${money(i.amount)}`);save();toast(i.id+' marked as paid');rerender();}
    else{if(!guard('delete'))return;confirmModal('Delete invoice','Delete '+i.id+'?',()=>{db.invoices.splice(db.invoices.indexOf(i),1);audit('fa-trash','Invoice deleted','Finance',i.id);save();toast('Invoice deleted','fa-trash','warn');rerender();});}});
  const ni=$('#btnInvNew');if(ni)ni.onclick=()=>{if(guard('create'))openInvoiceModal();};
  const {rev}=monthAgg();const exp=expSeries(rev);
  charts.push(new Chart($('#chFin'),{type:'bar',data:{labels:last12Labels(),datasets:[{label:'Revenue',data:rev.map(v=>Math.round(v)),backgroundColor:'rgba(99,102,241,.75)',hoverBackgroundColor:'#6366f1',borderRadius:6,maxBarThickness:18},{label:'Expenses',data:exp,backgroundColor:'rgba(148,163,184,.5)',hoverBackgroundColor:'#94a3b8',borderRadius:6,maxBarThickness:18}]},options:{maintainAspectRatio:false,plugins:{legend:{labels:{usePointStyle:true,boxWidth:7}},tooltip:tipMoney},scales:{y:{grid:{color:gridC()},ticks:{callback:v=>'$'+(v>=1000?(v/1000)+'k':v)}},x:{grid:{display:false}}}}}));
  const totExp=exp.reduce((a,b)=>a+b,0);
  charts.push(new Chart($('#chExp'),{type:'doughnut',data:{labels:EXP_RATIOS.map(p=>p[0]),datasets:[{data:EXP_RATIOS.map(p=>Math.round(totExp*p[1])),backgroundColor:PALETTE,borderWidth:0,hoverOffset:8}]},options:{maintainAspectRatio:false,cutout:'68%',plugins:{legend:{position:'bottom',labels:{usePointStyle:true,boxWidth:7,padding:14}},tooltip:{callbacks:{label:c=>` ${c.label}: ${money(c.parsed)}`}}}}}));
  const bdgB=EXP_RATIOS.map(x=>catBudget(x[0])),bdgA=EXP_RATIOS.map(x=>Math.round(catActual(x[0])));
  charts.push(new Chart($('#chBdg'),{type:'bar',data:{labels:EXP_RATIOS.map(x=>x[0]),datasets:[{label:'Budget',data:bdgB,backgroundColor:'rgba(148,163,184,.55)',hoverBackgroundColor:'#94a3b8',borderRadius:6,maxBarThickness:22},{label:'Actual',data:bdgA,backgroundColor:'rgba(99,102,241,.8)',hoverBackgroundColor:'#6366f1',borderRadius:6,maxBarThickness:22}]},options:{maintainAspectRatio:false,plugins:{legend:{labels:{usePointStyle:true,boxWidth:7}},tooltip:tipMoney},scales:{y:{grid:{color:gridC()},ticks:{callback:v=>'$'+v}},x:{grid:{display:false}}}}}));
  const cfs=cashFlowSeries();
  charts.push(new Chart($('#chCash'),{type:'line',data:{labels:cfs.labels,datasets:[{label:'Net cash flow',data:cfs.net,backgroundColor:'rgba(16,185,129,.12)',borderColor:'#10b981',fill:true,tension:.4,pointRadius:3,borderWidth:2}]},options:{maintainAspectRatio:false,plugins:{legend:{display:false},tooltip:tipMoney},scales:{y:{grid:{color:gridC()},ticks:{callback:v=>'$'+v}},x:{grid:{display:false}}}}}));
}

/* ----- Fixed Assets ----- */
function viewAssets(){
  const c=ctl.assets;
  const cats=[...new Set(db.assets.map(a=>a.category))];
  let list=[...db.assets];
  if(c.cat!=='all')list=list.filter(a=>a.category===c.cat);
  if(c.q){const q=c.q.toLowerCase();list=list.filter(a=>a.name.toLowerCase().includes(q)||a.category.toLowerCase().includes(q));}
  const {slice,pages,total}=paginate(list,c);
  const totalCost=db.assets.reduce((s,a)=>s+a.cost,0),accDep=db.assets.reduce((s,a)=>s+assetAccumDep(a),0);
  const nbv=totalCost-accDep,monthlyDep=db.assets.reduce((s,a)=>s+assetMonthlyDep(a),0);
  const rows=slice.map(a=>{const md=assetMonthlyDep(a),acc=assetAccumDep(a),nb=assetNbv(a);
    return `<tr><td class="mono strong">${a.id}</td><td><div style="display:flex;align-items:center;gap:10px">${avatar(a.name,30)}<div><b style="font-size:13px">${esc(a.name)}</b><div class="muted small">${a.category}</div></div></div></td><td class="muted">${fmtDate(a.purchaseDate)}</td><td class="strong">${money(a.cost)}</td><td class="muted">${a.usefulLifeYears} yr</td><td class="muted">${money2(md)}</td><td class="muted">${money2(acc)}</td><td class="strong">${money(nb)}</td><td>${badge(a.status)}</td><td><div class="row-actions">${a.status!=='Disposed'&&can('edit')?`<button class="btn btn-ghost btn-xs" data-aact="dispose" data-id="${a.id}"><i class="fa-solid fa-box-archive"></i> Dispose</button>`:''}${can('edit')?`<button class="icon-btn sm" data-aact="edit" data-id="${a.id}"><i class="fa-regular fa-pen-to-square"></i></button>`:''}${can('delete')?`<button class="icon-btn sm danger" data-aact="del" data-id="${a.id}"><i class="fa-regular fa-trash-can"></i></button>`:''}</div></td></tr>`;}).join('');
  return `<div class="grid g-4 mb">
    <div class="card kpi"><div class="kpi-top"><div class="kpi-ico c1"><i class="fa-solid fa-coins"></i></div></div><div class="kpi-val" data-num="${totalCost}" data-fmt="money">${money(totalCost)}</div><div class="kpi-label">Total Asset Cost</div><div class="kpi-sub"><span class="vs">${db.assets.length} assets registered</span></div></div>
    <div class="card kpi"><div class="kpi-top"><div class="kpi-ico c3"><i class="fa-solid fa-arrow-down"></i></div></div><div class="kpi-val" data-num="${accDep}" data-fmt="money">${money(accDep)}</div><div class="kpi-label">Accumulated Depreciation</div><div class="kpi-sub"><span class="vs">${money2(monthlyDep)}/month running</span></div></div>
    <div class="card kpi"><div class="kpi-top"><div class="kpi-ico c4"><i class="fa-solid fa-building-columns"></i></div></div><div class="kpi-val" data-num="${nbv}" data-fmt="money">${money(nbv)}</div><div class="kpi-label">Net Book Value</div><div class="kpi-sub">${totalCost?Math.round(nbv/totalCost*100)+'% of original cost':''}</div></div>
    <div class="card kpi"><div class="kpi-top"><div class="kpi-ico c2"><i class="fa-solid fa-calendar-days"></i></div></div><div class="kpi-val">${money2(monthlyDep)}</div><div class="kpi-label">Monthly Depreciation</div><div class="kpi-sub"><span class="vs">Straight-line method</span></div></div>
  </div>
  <div class="toolbar"><div class="toolbar-right" style="margin-right:auto"><div class="search-box"><i class="fa-solid fa-magnifying-glass"></i><input id="astSearch" placeholder="Search asset or category…" value="${esc(c.q)}"></div><select id="astCat" style="width:170px"><option value="all">All Categories</option>${cats.map(x=>`<option ${c.cat===x?'selected':''}>${x}</option>`).join('')}</select></div><div class="toolbar-right">${can('export')?`<button class="btn btn-ghost btn-sm" id="btnAstExport"><i class="fa-solid fa-download"></i> Export</button>`:''}${can('create')?`<button class="btn btn-primary btn-sm" id="btnAstNew"><i class="fa-solid fa-plus"></i> Add Asset</button>`:''}</div></div>
  <div class="card"><div class="tbl-wrap"><table class="tbl"><thead><tr><th>Asset</th><th>Name</th><th>Purchased</th><th>Cost</th><th>Life</th><th>Monthly Dep</th><th>Accum. Dep</th><th>NBV</th><th>Status</th><th></th></tr></thead><tbody>${rows}</tbody></table>${slice.length?'':'<div class="empty"><i class="fa-regular fa-folder-open"></i>No assets found</div>'}</div>${pagerHtml(c,pages,total)}</div>`;
}
function mountAssets(){
  const c=ctl.assets;
  bindSearch('#astSearch',c);restoreFocus(c,'#astSearch');bindPager(c);animateKpis();
  $('#astCat').onchange=e=>{c.cat=e.target.value;c.page=1;rerender();};
  $$('#view [data-aact]').forEach(b=>b.onclick=()=>{const a=db.assets.find(x=>x.id===b.dataset.id);
    if(b.dataset.aact==='edit'){if(!guard('edit'))return;openAssetModal(a);return;}
    if(b.dataset.aact==='dispose'){if(!guard('edit'))return;confirmModal('Dispose asset','Mark '+a.name+' as disposed? Depreciation stops and NBV is written off.',()=>{a.status='Disposed';a.disposedOn=new Date().toISOString();audit('fa-box-archive','Asset disposed','Assets',a.id,`${a.name} · NBV ${money2(assetNbv(a))}`);save();toast(a.name+' disposed','fa-box-archive','warn');rerender();},'Dispose');return;}
    if(!guard('delete'))return;confirmModal('Delete asset','Remove '+a.name+' from the register?',()=>{db.assets.splice(db.assets.indexOf(a),1);audit('fa-trash','Asset deleted','Assets',a.id,a.name);save();toast('Asset deleted','fa-trash','warn');rerender();});});
  const na=$('#btnAstNew');if(na)na.onclick=()=>{if(guard('create'))openAssetModal(null);};
  const nx=$('#btnAstExport');if(nx)nx.onclick=()=>{if(!guard('export'))return;exportCSV('assets.csv',['ID','Name','Category','Purchase Date','Cost','Salvage','Useful Life (yrs)','Status','Accum. Depreciation','NBV'],db.assets.map(a=>[a.id,a.name,a.category,a.purchaseDate.slice(0,10),a.cost,a.salvage,a.usefulLifeYears,a.status,assetAccumDep(a).toFixed(2),assetNbv(a).toFixed(2)]));audit('fa-download','Exported','Assets','assets.csv');save();toast('Assets exported to CSV');};
}
function openAssetModal(a){
  const editing=!!a;
  const cats=['IT Equipment','Vehicles','Furniture','Office Equipment','Machinery','Software','Leasehold Improvements'];
  openModal(`<div class="modal-head"><div><h3>${editing?'Edit Asset':'Add Asset'}</h3></div><button class="icon-btn modal-close"><i class="fa-solid fa-xmark"></i></button></div>
  <form id="aForm" class="modal-body">
    <label>Asset Name<input id="aa-name" required value="${editing?esc(a.name):''}" placeholder="e.g. Dell PowerEdge R740 Server"></label>
    <div class="frow2"><label>Category<select id="aa-cat">${cats.map(c=>`<option ${editing&&a.category===c?'selected':''}>${c}</option>`).join('')}</select></label><label>Purchase Date<input type="date" id="aa-date" required value="${(editing?a.purchaseDate:new Date().toISOString()).slice(0,10)}"></label></div>
    <div class="frow2"><label>Purchase Cost ($)<input type="number" step="0.01" min="0" id="aa-cost" required value="${editing?a.cost:''}"></label><label>Salvage Value ($)<input type="number" step="0.01" min="0" id="aa-salvage" value="${editing?a.salvage:0}"></label></div>
    <label>Useful Life (years)<input type="number" min="1" max="50" id="aa-life" required value="${editing?a.usefulLifeYears:5}"></label>
  </form>
  <div class="modal-foot"><div class="spacer"></div><button class="btn btn-ghost modal-close">Cancel</button><button class="btn btn-primary" id="aa-save"><i class="fa-solid fa-check"></i> ${editing?'Save Changes':'Add Asset'}</button></div>`,540);
  $('#aForm').onsubmit=e=>{e.preventDefault();$('#aa-save').click();};
  $('#aa-save').onclick=()=>{
    const data={name:$('#aa-name').value.trim(),category:$('#aa-cat').value,purchaseDate:new Date($('#aa-date').value+'T12:00:00').toISOString(),cost:+$('#aa-cost').value,salvage:+$('#aa-salvage').value||0,usefulLifeYears:Math.max(1,+$('#aa-life').value)};
    if(!data.name||isNaN(data.cost))return;
    if(editing){Object.assign(a,data);audit('fa-pen-to-square','Asset updated','Assets',a.id,data.name);}else{db.assets.push({id:'AST-'+String(db.seq.asset++).padStart(3,'0'),status:'In Service',disposedOn:null,...data});audit('fa-plus','Asset created','Assets',db.assets[0].id,`${data.name} · ${money2(data.cost)}`);addActivity('fa-building-columns','New asset added: '+data.name);}
    save();closeModal();toast(editing?'Asset updated':'Asset added');rerender();
  };
}

/* ----- Reports ----- */
function viewReports(){
  return `<div class="grid g-21 mb">
    <div class="card"><div class="card-h"><div><h3>Revenue vs Target</h3><p>Monthly performance against $9.5k target</p></div></div><div class="card-b"><div class="chart-box"><canvas id="chTarget"></canvas></div></div></div>
    <div class="card"><div class="card-h"><div><h3>Top Customers</h3><p>By lifetime revenue</p></div></div><div class="card-b"><div class="chart-box"><canvas id="chCust"></canvas></div></div></div>
  </div>
  <div class="card"><div class="card-h"><div><h3>Export Center</h3><p>Download datasets for external analysis</p></div></div><div class="card-b" style="display:flex;gap:10px;flex-wrap:wrap">
    <button class="btn btn-ghost btn-sm" data-exp="orders"><i class="fa-solid fa-cart-shopping"></i> Orders CSV</button>
    <button class="btn btn-ghost btn-sm" data-exp="products"><i class="fa-solid fa-box"></i> Inventory CSV</button>
    <button class="btn btn-ghost btn-sm" data-exp="customers"><i class="fa-solid fa-users"></i> Customers CSV</button>
    <button class="btn btn-ghost btn-sm" data-exp="employees"><i class="fa-solid fa-user-tie"></i> Employees CSV</button>
    <button class="btn btn-primary btn-sm" data-exp="json"><i class="fa-solid fa-database"></i> Full Backup (JSON)</button>
  </div></div>`;
}
function mountReports(){
  const {rev}=monthAgg();
  charts.push(new Chart($('#chTarget'),{type:'line',data:{labels:last12Labels(),datasets:[{label:'Actual',data:rev.map(v=>Math.round(v)),borderColor:'#6366f1',backgroundColor:c=>grad(c,'99,102,241'),fill:true,tension:.4,borderWidth:2.5,pointRadius:3},{label:'Target',data:rev.map(()=>9500),borderColor:'#f59e0b',borderDash:[6,6],borderWidth:2,pointRadius:0,fill:false}]},options:{maintainAspectRatio:false,interaction:{mode:'index',intersect:false},plugins:{legend:{labels:{usePointStyle:true,boxWidth:7}},tooltip:tipMoney},scales:{y:{grid:{color:gridC()},ticks:{callback:v=>'$'+(v>=1000?(v/1000)+'k':v)}},x:{grid:{display:false}}}}}));
  const tc=Object.entries(db.orders.filter(o=>o.status!=='Cancelled').reduce((m,o)=>{m[o.customerName]=(m[o.customerName]||0)+o.total;return m;},{})).sort((a,b)=>b[1]-a[1]).slice(0,7);
  charts.push(new Chart($('#chCust'),{type:'bar',data:{labels:tc.map(t=>t[0]),datasets:[{label:'Revenue',data:tc.map(t=>Math.round(t[1])),backgroundColor:'rgba(139,92,246,.7)',hoverBackgroundColor:'#8b5cf6',borderRadius:6}]},options:{indexAxis:'y',maintainAspectRatio:false,plugins:{legend:{display:false},tooltip:tipMoney},scales:{x:{grid:{color:gridC()},ticks:{callback:v=>'$'+(v>=1000?(v/1000)+'k':v)}},y:{grid:{display:false}}}}}));
  $$('#view [data-exp]').forEach(b=>b.onclick=()=>{if(!guard('export'))return;const k=b.dataset.exp;
    if(k==='orders')exportCSV('orders-report.csv',['Order ID','Customer','Date','Status','Total'],db.orders.map(o=>[o.id,o.customerName,fmtDate(o.date),o.status,o.total.toFixed(2)]));
    if(k==='products')exportCSV('inventory-report.csv',['SKU','Name','Category','Stock','Price'],db.products.map(p=>[p.sku,p.name,p.category,p.stock,p.price.toFixed(2)]));
    if(k==='customers')exportCSV('customers-report.csv',['Company','Contact','Country','Tier'],db.customers.map(x=>[x.company,x.name,x.country,x.tier]));
    if(k==='employees')exportCSV('employees-report.csv',['Name','Role','Department','Salary','Status'],db.employees.map(e=>[e.name,e.role,e.dept,e.salary,e.status]));
    if(k==='json')download('nexus-erp-backup.json','application/json',JSON.stringify(db,null,2));
    audit('fa-download','Report exported','Reports',k==='json'?'nexus-erp-backup.json':k+'-report.csv');
    save();toast('Export ready — download started');});
}

/* ----- Audit Log ----- */
function viewAudit(){
  const c=ctl.audit;
  let list=[...db.audit];
  if(c.module!=='all')list=list.filter(a=>a.module===c.module);
  if(c.user!=='all')list=list.filter(a=>a.user===c.user);
  if(c.q){const q=c.q.toLowerCase();list=list.filter(a=>a.action.toLowerCase().includes(q)||a.target.toLowerCase().includes(q)||a.detail.toLowerCase().includes(q)||a.module.toLowerCase().includes(q));}
  const {slice,pages,total}=paginate(list,c);
  const mods=[...new Set(db.audit.map(a=>a.module))];
  const users=[...new Set(db.audit.map(a=>a.user))];
  const rows=slice.map(a=>`<tr><td class="muted" style="white-space:nowrap">${fmtDT(a.time)}</td><td><div style="display:flex;align-items:center;gap:9px"><span class="act-ico" style="width:26px;height:26px;font-size:11px"><i class="fa-solid ${a.icon}"></i></span><b style="font-size:12.5px">${esc(a.action)}</b></div></td><td>${badge(a.module)}</td><td class="mono muted">${esc(a.target)}</td><td class="muted">${esc(a.detail)}</td><td>${avatar(a.user,24)} ${esc(a.user)}</td><td><span class="badge b-gray">${a.role}</span></td></tr>`).join('');
  return `<div class="toolbar"><div class="toolbar-right" style="margin-right:auto"><div class="search-box"><i class="fa-solid fa-magnifying-glass"></i><input id="auditSearch" placeholder="Search action, module, target…" value="${esc(c.q)}"></div><select id="auditMod"><option value="all">All Modules</option>${mods.map(m=>`<option ${c.module===m?'selected':''}>${m}</option>`).join('')}</select><select id="auditUser"><option value="all">All Users</option>${users.map(u=>`<option ${c.user===u?'selected':''}>${u}</option>`).join('')}</select></div><div class="toolbar-right">${can('export')?`<button class="btn btn-ghost btn-sm" id="btnAuditExport"><i class="fa-solid fa-download"></i> Export CSV</button>`:''}</div></div>
  <div class="card"><div class="tbl-wrap"><table class="tbl"><thead><tr><th>When</th><th>Action</th><th>Module</th><th>Target</th><th>Detail</th><th>User</th><th>Role</th></tr></thead><tbody>${rows}</tbody></table>${slice.length?'':'<div class="empty"><i class="fa-regular fa-folder-open"></i>No audit entries match your filters</div>'}</div>${pagerHtml(c,pages,total)}</div>`;
}
function mountAudit(){
  const c=ctl.audit;
  bindSearch('#auditSearch',c);restoreFocus(c,'#auditSearch');bindPager(c);
  const m=$('#auditMod');if(m)m.onchange=e=>{c.module=e.target.value;c.page=1;rerender();};
  const u=$('#auditUser');if(u)u.onchange=e=>{c.user=e.target.value;c.page=1;rerender();};
  const x=$('#btnAuditExport');if(x)x.onclick=()=>{if(!guard('export'))return;exportCSV('audit-log.csv',['Time','User','Role','Module','Action','Target','Detail'],db.audit.map(a=>[fmtDT(a.time),a.user,a.role,a.module,a.action,a.target,a.detail]));audit('fa-download','Exported','Audit','audit-log.csv','Export of audit trail');save();toast('Audit log exported to CSV');};
}

/* ----- Settings ----- */
function viewSettings(){
  const p=db.profile;
  return `<div class="grid g-2">
    <div class="card"><div class="card-h"><h3>Profile</h3></div><div class="card-b"><form id="profForm">
      <div class="frow2"><label>Full Name<input id="pfName" required value="${esc(p.name)}"></label><label>Role<select id="pfRole">${['Administrator','Manager','Analyst','Viewer'].map(r=>`<option ${r===p.role?'selected':''}>${r}</option>`).join('')}</select></label></div>
      <label>Email Address<input id="pfEmail" type="email" required value="${esc(p.email)}"></label>
      <button class="btn btn-primary" type="submit"><i class="fa-solid fa-floppy-disk"></i> Save Profile</button>
    </form></div></div>
    <div class="card"><div class="card-h"><h3>Appearance &amp; Preferences</h3></div><div class="card-b">
      <div class="grid" style="grid-template-columns:1fr 1fr;gap:12px">
        <div class="theme-opt ${db.theme==='light'?'on':''}" data-themechoice="light"><span class="theme-swatch" style="background:#f2f4f9"></span><div><b style="font-size:13px">Light</b><p class="muted small">Clean &amp; bright</p></div>${db.theme==='light'?'<i class="fa-solid fa-circle-check" style="color:var(--primary);margin-left:auto"></i>':''}</div>
        <div class="theme-opt ${db.theme==='dark'?'on':''}" data-themechoice="dark"><span class="theme-swatch" style="background:#0b1020"></span><div><b style="font-size:13px">Dark</b><p class="muted small">Easy on the eyes</p></div>${db.theme==='dark'?'<i class="fa-solid fa-circle-check" style="color:var(--primary);margin-left:auto"></i>':''}</div>
      </div>
      <div style="margin-top:18px">
        <div class="pref-row"><div style="flex:1"><b>Email notifications</b><p>Order and payment alerts to your inbox</p></div><label class="switch" style="margin:0"><input type="checkbox" data-pref="emailNotif" ${db.prefs.emailNotif?'checked':''}><i></i></label></div>
        <div class="pref-row"><div style="flex:1"><b>Weekly digest</b><p>Summary of KPIs every Monday</p></div><label class="switch" style="margin:0"><input type="checkbox" data-pref="digest" ${db.prefs.digest?'checked':''}><i></i></label></div>
        <div class="pref-row"><div style="flex:1"><b>Low stock alerts</b><p>Notify when items hit reorder level</p></div><label class="switch" style="margin:0"><input type="checkbox" data-pref="lowStock" ${db.prefs.lowStock?'checked':''}><i></i></label></div>
      </div>
    </div></div>
    <div class="card" style="grid-column:1/-1"><div class="card-h"><h3>Billing &amp; Pricing</h3><p>Defaults applied to new orders and quotes</p></div><div class="card-b"><form id="cfgForm">
      <div class="frow2"><label>Tax Rate (%)<input type="number" min="0" max="40" step="0.5" id="cf-tax" required value="${db.config.taxRate}"></label><label>Shipping Fee ($)<input type="number" min="0" step="0.01" id="cf-ship" required value="${db.config.shippingFee}"></label></div>
      <label>Free Shipping Over ($)<input type="number" min="0" step="0.01" id="cf-free" required value="${db.config.freeShipOver}"></label>
      <button class="btn btn-primary" type="submit"><i class="fa-solid fa-floppy-disk"></i> Save Pricing Defaults</button>
    </form></div></div>
    <div class="card" style="grid-column:1/-1"><div class="card-h"><h3>Budget &amp; Planning</h3><p>Monthly operating budget per expense category</p></div><div class="card-b"><form id="bdgForm">
      <div class="frow2">${EXP_RATIOS.map(([c])=>`<label>${esc(c)} ($)<input type="number" min="0" step="10" data-bdg="${c}" required value="${catBudget(c)}"></label>`).join('')}</div>
      <button class="btn btn-primary" type="submit"><i class="fa-solid fa-floppy-disk"></i> Save Budget Plan</button>
    </form></div></div>
    <div class="card" style="grid-column:1/-1"><div class="card-h"><h3>Data Management</h3></div><div class="card-b" style="display:flex;gap:10px;flex-wrap:wrap;align-items:center">
      <button class="btn btn-ghost btn-sm" id="btnBackup"><i class="fa-solid fa-download"></i> Download Backup (JSON)</button>
      <span class="muted small">Stored locally in your browser · ${Math.round(JSON.stringify(db).length/1024)} KB</span>
      <div class="spacer"></div>
      <button class="btn btn-danger btn-sm" id="btnReset"><i class="fa-solid fa-rotate-left"></i> Reset Demo Data</button>
    </div></div>
  </div>`;
}
function mountSettings(){
  $('#profForm').onsubmit=e=>{e.preventDefault();if(!guard('edit'))return;db.profile.name=$('#pfName').value.trim();db.profile.email=$('#pfEmail').value.trim();db.profile.role=$('#pfRole').value;audit('fa-user','Profile updated','Settings',db.profile.email);save();renderUserChip();toast('Profile updated');};
  $('#cfgForm').onsubmit=e=>{e.preventDefault();if(!guard('edit'))return;const tr=+$('#cf-tax').value,sh=+$('#cf-ship').value,fr=+$('#cf-free').value;if(isNaN(tr)||isNaN(sh)||isNaN(fr))return;db.config.taxRate=Math.max(0,Math.min(40,tr));db.config.shippingFee=Math.max(0,sh);db.config.freeShipOver=Math.max(0,fr);audit('fa-sliders','Pricing defaults updated','Settings','',`Tax ${db.config.taxRate}% · Ship ${money2(db.config.shippingFee)} · Free over ${money2(db.config.freeShipOver)}`);save();toast('Pricing defaults saved');};
  $('#bdgForm').onsubmit=e=>{e.preventDefault();if(!guard('edit'))return;if(!db.budget)db.budget={cats:{}};$$('[data-bdg]').forEach(inp=>db.budget.cats[inp.dataset.bdg]=Math.max(0,+inp.value||0));audit('fa-sliders','Budget plan updated','Settings','',EXP_RATIOS.map(([c])=>c+': '+money(db.budget.cats[c])).join(' · '));save();toast('Budget plan saved');};
  $$('#view [data-themechoice]').forEach(b=>b.onclick=()=>applyTheme(b.dataset.themechoice));
  $$('#view [data-pref]').forEach(s=>s.onchange=()=>{if(!guard('edit')){s.checked=!s.checked;return;}db.prefs[s.dataset.pref]=s.checked;audit('fa-sliders','Preference updated','Settings',s.dataset.pref,String(s.checked));save();toast('Preference saved','fa-sliders','info');});
  $('#btnBackup').onclick=()=>{if(!guard('export'))return;download('nexus-erp-backup.json','application/json',JSON.stringify(db,null,2));audit('fa-download','Backup downloaded','Settings','nexus-erp-backup.json');save();toast('Backup downloaded');};
  $('#btnReset').onclick=()=>{if(!guard('delete'))return;confirmModal('Reset demo data','All changes will be discarded and fresh demo data will be generated. This cannot be undone.',()=>{audit('fa-rotate-left','Demo data reset','Settings');localStorage.removeItem(LS_KEY);db=seed();save();applyTheme(db.theme,false);renderUserChip();updateNotifDot();navigate('dashboard');toast('Demo data has been reset','fa-rotate-left','info');},'Reset');};
}

/* ================= MODAL FORMS ================= */
let draft={items:[],editingId:null,mode:'order'};
function openOrderModal(order,mode='order'){
  const editing=!!order;
  draft={mode,items:editing?order.items.map(i=>({...i})):[{productId:'',qty:1}],editingId:editing?order.id:null,discPct:editing?(order.discPct||0):0};
  const isQ=mode==='quote';
  const custOpts=db.customers.map(c=>`<option value="${c.id}" ${editing&&order.customerId===c.id?'selected':''}>${esc(c.company)}</option>`).join('');
  const stOpts=isQ?['Draft','Sent','Approved'].map(s=>`<option ${editing&&order.status===s?'selected':''}>${s}</option>`).join(''):['Pending','Processing','Shipped','Delivered','Cancelled'].map(s=>`<option ${editing&&order.status===s?'selected':''}>${s}</option>`).join('');
  openModal(`<div class="modal-head"><div><h3>${editing?(isQ?'Edit ':'Edit ')+order.id:(isQ?'New Sales Quote':'New Sales Order')}</h3><p>${isQ?'Prepare a quote — no stock is allocated until converted':'Create a new order — stock is allocated automatically'}</p></div><button class="icon-btn modal-close"><i class="fa-solid fa-xmark"></i></button></div>
  <form id="orderForm" class="modal-body" style="overflow-y:auto">
    <div class="frow2"><label>Customer<select id="of-cust" required>${custOpts}</select></label><label>${isQ?'Quote Date':'Order Date'}<input type="date" id="of-date" required value="${(editing?order.date:new Date().toISOString()).slice(0,10)}"></label></div>
    ${isQ?'':'<div class="muted small" id="of-credit"></div>'}
    <label>Status<select id="of-status">${stOpts}</select></label>
    <div class="items-label"><span>Line Items</span><button type="button" class="btn btn-ghost btn-xs" id="of-additem"><i class="fa-solid fa-plus"></i> Add Item</button></div>
    <div id="of-items"></div>
    <div class="frow2" style="margin-top:12px"><label>Discount %<input type="number" min="0" max="100" step="0.5" id="of-disc" value="${draft.discPct}"></label></div>
    <div class="of-totals" id="of-totals"></div>
  </form>
  <div class="modal-foot"><div class="spacer"></div><button class="btn btn-ghost modal-close">Cancel</button><button class="btn btn-primary" id="of-save"><i class="fa-solid fa-check"></i> ${editing?'Save Changes':(isQ?'Create Quote':'Create Order')}</button></div>`,660);
  renderDraft();
  $('#orderForm').onsubmit=e=>{e.preventDefault();saveOrder();};
  $('#of-additem').onclick=()=>{draft.items.push({productId:'',qty:1});renderDraft();};
  $('#of-save').onclick=saveOrder;
  $('#of-disc').oninput=()=>{draft.discPct=+$('#of-disc').value||0;renderDraft();};
  if(!isQ){const hint=$('#of-credit');const refresh=()=>{const c=db.customers.find(x=>x.id===$('#of-cust').value);if(!c){hint.innerHTML='';return;}const st=creditState(c);hint.innerHTML=st.limit>0?`<i class="fa-solid fa-circle-info"></i> ${esc(c.terms)} · Credit <b>${money(st.used)}</b> of <b>${money(st.limit)}</b> used${st.over?' <span style="color:var(--red);font-weight:600">— over limit</span>':''}`:`<i class="fa-solid fa-circle-info"></i> ${esc(c.terms)} · No credit limit set`;};$('#of-cust').onchange=refresh;refresh();}
