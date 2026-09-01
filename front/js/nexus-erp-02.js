};
function fillRoleCreds(role){
  const acc=Object.entries(DEMO_ACCOUNTS).find(([,v])=>v.role===role);
  if(!acc)return;
  $('#liEmail').value=acc[0];$('#liPass').value=acc[1].pass;$('#liRole').value=role;
  const err=$('#liError');if(err)err.style.display='none';
}
function showLogin(){audit('fa-right-from-bracket','Signed out','Auth',db.session&&db.session.email||'');db.session=null;save();$('#liPass').value='';const err=$('#liError');if(err)err.style.display='none';$('#login').classList.remove('hidden');}
function submitLogin(e){
  e.preventDefault();
  const email=$('#liEmail').value.trim().toLowerCase();
  const pass=$('#liPass').value;
  const acc=DEMO_ACCOUNTS[email];
  const err=$('#liError');
  if(!acc||acc.pass!==pass){err.style.display='block';$('#liPass').value='';$('#liPass').focus();return;}
  err.style.display='none';
  db.profile.role=acc.role;db.profile.name=acc.name;
  db.session={email,role:acc.role,time:new Date().toISOString()};
  audit('fa-right-to-bracket','Signed in','Auth',email,acc.role);
  save();renderUserChip();updateNotifDot();applyRoleUI();
  $('#login').classList.add('hidden');
  toast(`Welcome, ${acc.name.split(' ')[0]} — signed in as ${acc.role}`,'fa-hand');
  if(currentView&&VIEWS[currentView])navigate(currentView);
}

/* ================= STOCK MOVEMENTS / PAYMENTS / INVOICES ================= */
function logMovement(productId,delta,reason,ref){
  const p=db.products.find(x=>x.id===productId);
  db.movements.unshift({id:'m'+Date.now()+Math.floor(Math.random()*999),productId,productName:p?p.name:'Deleted product',delta,reason:reason||'Adjustment',ref:ref||'',by:db.profile.name,time:new Date().toISOString()});
  db.movements=db.movements.slice(0,500);
}
function orderPaid(o){return (o.payments||[]).reduce((s,p)=>s+p.amount,0);}
function orderRefunded(o){return (o.returns||[]).reduce((s,r)=>s+r.amount,0);}
function orderNet(o){return Math.max(0,o.total-orderRefunded(o));}
function payStatus(o){if(o.status==='Cancelled')return'Cancelled';const net=orderNet(o),paid=orderPaid(o);if(paid>=net-0.005)return'Paid';if(paid>0)return'Partially Paid';return'Unpaid';}

/* ================= PAYMENT TERMS / CREDIT ================= */
const TERMS_DAYS={'Due on Receipt':0,'Net 15':15,'Net 30':30,'Net 45':45,'Net 60':60};
const termsDays=t=>TERMS_DAYS[t]!==undefined?TERMS_DAYS[t]:30;
function dueFromIssued(issued,terms){return new Date(new Date(issued).getTime()+termsDays(terms)*864e5).toISOString();}
function customerOutstanding(c){
  const inv=(db.invoices||[]).filter(i=>i.customerId===c.id&&i.status!=='Paid').reduce((s,i)=>s+i.amount,0);
  const ord=(db.orders||[]).filter(o=>o.customerId===c.id&&o.status!=='Cancelled').reduce((s,o)=>s+Math.max(0,orderNet(o)-orderPaid(o)),0);
  return inv+ord;
}
function creditState(c){
  const limit=+(c.creditLimit)||0;
  const used=Math.round(customerOutstanding(c));
  return {limit,used,remain:limit>0?Math.max(0,limit-used):null,over:limit>0&&used>limit};
}
function creditBlock(c,extra=0,base){
  const st=creditState(c);
  const used=base!=null?base:st.used;
  if(c.hold)return `Account on hold — ${c.company}`;
  if(st.limit>0&&(used+extra)>st.limit)return `Credit limit exceeded for ${c.company} — ${money(used+extra)} of ${money(st.limit)} used`;
  return null;
}

/* ================= WEIGHTED-AVERAGE COSTING / STOCK AGE ================= */
function restock(p,qty,cost){
  if(!p||qty<=0)return;
  const c=cost!=null&&!isNaN(cost)?+cost:p.cost;
  p.cost=+((p.stock*p.cost+qty*c)/(p.stock+qty)).toFixed(2);
  p.stock+=qty;
}
function lastMovementTime(pid){
  const mv=db.movements.find(m=>m.productId===pid);
  return mv?new Date(mv.time).getTime():null;
}
function stockAgeDays(p){
  const t=lastMovementTime(p.id);
  if(!t)return null;
  return Math.max(0,Math.floor((Date.now()-t)/864e5));
}

/* ================= FIXED ASSETS / DEPRECIATION ================= */
function assetMonthlyDep(a){const life=Math.max(1,a.usefulLifeYears||0)*12;return Math.max(0,(a.cost-a.salvage)/life);}
function assetAccumDep(a){
  const months=Math.max(0,Math.floor((Date.now()-new Date(a.purchaseDate).getTime())/864e5/30.44));
  const cap=Math.max(0,a.cost-a.salvage);
  return Math.min(cap,assetMonthlyDep(a)*months);
}
function assetNbv(a){return Math.max(0,a.cost-assetAccumDep(a));}

function createInvoiceForOrder(o){
  const ex=db.invoices.find(i=>i.orderId===o.id);if(ex)return ex;
  const cust=db.customers.find(c=>c.id===o.customerId);
  const inv={id:'INV-'+(db.seq.inv++),orderId:o.id,customerId:o.customerId,customerName:o.customerName,amount:Math.round(o.total),issued:new Date().toISOString(),due:dueFromIssued(new Date().toISOString(),cust?cust.terms:'Net 30'),status:'Pending',paidOn:null};
  db.invoices.unshift(inv);o.invoiceId=inv.id;
  audit('fa-file-invoice-dollar','Invoice generated','Finance',inv.id,'From order '+o.id+' · '+money2(inv.amount)+' · '+(cust?cust.terms:'Net 30'));
  return inv;
}
function convertQuote(id){
  const q=db.quotes.find(x=>x.id===id);if(!q||q.status==='Converted')return;
  const cust=db.customers.find(c=>c.id===q.customerId);
  if(cust){const block=creditBlock(cust,q.total);if(block){toast(block,'fa-ban','error');audit('fa-ban','Conversion blocked','Quotes',q.id,block);return;}}
  const order={id:'ORD-'+(db.seq.order++),customerId:q.customerId,customerName:q.customerName,items:q.items.map(i=>({...i})),sub:q.sub,tax:q.tax,ship:q.ship,total:q.total,status:'Pending',date:new Date().toISOString(),payments:[],quoteId:q.id};
  db.orders.unshift(order);applyStock(order,-1,'Sale',order.id);q.status='Converted';
  addActivity('fa-arrow-right',`Quote ${q.id} converted to ${order.id}`);
  audit('fa-arrow-right','Quote converted','Quotes',q.id,`→ ${order.id} · ${money2(order.total)}`);
  pushNotif('fa-file-invoice-dollar','Quote converted',`${q.id} → order ${order.id} for ${q.customerName}`);
  toast(`Quote converted — ${order.id} created`,'fa-arrow-right');
}

/* ================= CHART HELPERS ================= */
Chart.defaults.font.family="'Inter Variable','Inter',system-ui,sans-serif";
Chart.defaults.font.size=11.5;
function grad(ctx,rgb){const {chart}=ctx;const {ctx:c,chartArea:a}=chart;if(!a)return `rgba(${rgb},.2)`;const g=c.createLinearGradient(0,a.top,0,a.bottom);g.addColorStop(0,`rgba(${rgb},.32)`);g.addColorStop(1,`rgba(${rgb},0)`);return g;}
const tipMoney={callbacks:{label:c=>` ${c.dataset.label}: ${money2(c.parsed.y??c.parsed.x??c.parsed)}`}};

/* ================= VIEWS ================= */
function viewDashboard(){
  const active=db.orders.filter(o=>o.status!=='Cancelled');
  const revenue=active.reduce((s,o)=>s+o.total,0);
  const invValue=db.products.reduce((s,p)=>s+p.stock*p.cost,0);
  const {rev,ord,units}=monthAgg();const cust=newCustMonths();
  const low=db.products.filter(p=>p.stock<=p.reorder);
  const top=topProducts().slice(0,5);const maxRev=top[0]?top[0][1].rev:1;
  const alert=low.length?`<div class="alert-strip"><i class="fa-solid fa-triangle-exclamation"></i><span><b>${low.length} product${low.length>1?'s':''}</b> at or below reorder level — ${low.slice(0,3).map(p=>esc(p.name)).join(', ')}${low.length>3?'…':''}</span><div class="spacer"></div><button class="btn btn-ghost btn-sm" data-goto="inventory">Review Inventory</button></div>`:'';
  const kpi=(icon,cls,label,val,fmt,delta,sp)=>`<div class="card kpi"><div class="kpi-top"><div class="kpi-ico ${cls}"><i class="fa-solid ${icon}"></i></div>${sp}</div><div class="kpi-val" data-num="${val}" data-fmt="${fmt}">${fmt==='money'?money(val):val}</div><div class="kpi-label">${label}</div><div class="kpi-sub">${delta}</div></div>`;
  const recent=db.orders.slice(0,8).map(o=>`<tr style="cursor:pointer" data-openorder="${o.id}"><td class="mono strong">${o.id}</td><td>${avatar(o.customerName,26)} ${esc(o.customerName)}</td><td class="muted">${fmtDate(o.date)}</td><td class="strong">${money2(o.total)}</td><td>${badge(o.status)}</td></tr>`).join('');
  return `${alert}
  <div class="grid g-4 mb">
    ${kpi('fa-sack-dollar','c1','Total Revenue',Math.round(revenue),'money',deltaHtml(pct(rev[11],rev[10])),spark(rev.slice(-8),'#6366f1'))}
    ${kpi('fa-cart-shopping','c2','Total Orders',active.length,'int',deltaHtml(pct(ord[11],ord[10])),spark(ord.slice(-8),'#0ea5e9'))}
    ${kpi('fa-users','c3','Active Customers',db.customers.length,'int',deltaHtml(pct(cust[11],cust[10])),spark(cust.slice(-8),'#f59e0b'))}
    ${kpi('fa-warehouse','c4','Inventory Value',Math.round(invValue),'money',deltaHtml(pct(units[11],units[10])),spark(units.slice(-8),'#10b981'))}
  </div>
  <div class="grid g-21 mb">
    <div class="card"><div class="card-h"><div><h3>Revenue Performance</h3><p>Monthly revenue vs order volume — last 12 months</p></div><span class="badge b-violet">Live</span></div><div class="card-b"><div class="chart-box"><canvas id="chRev"></canvas></div></div></div>
    <div class="card"><div class="card-h"><div><h3>Sales by Category</h3><p>Revenue distribution</p></div></div><div class="card-b"><div class="chart-box"><canvas id="chCat"></canvas></div></div></div>
  </div>
  <div class="grid g-21">
    <div class="card"><div class="card-h"><div><h3>Recent Orders</h3><p>Latest transactions across all channels</p></div><button class="btn btn-ghost btn-sm" data-goto="orders">View All <i class="fa-solid fa-arrow-right"></i></button></div><div class="tbl-wrap"><table class="tbl"><thead><tr><th>Order</th><th>Customer</th><th>Date</th><th>Total</th><th>Status</th></tr></thead><tbody>${recent}</tbody></table></div></div>
    <div class="stack">
      <div class="card"><div class="card-h"><h3>Top Products</h3></div><div class="card-b">${top.map((t,i)=>`<div class="tp-row"><span class="tp-rank">${i+1}</span><div style="flex:1;min-width:0"><div style="display:flex;justify-content:space-between;font-size:12.5px;font-weight:600"><span style="overflow:hidden;text-overflow:ellipsis;white-space:nowrap">${esc(t[0])}</span><span>${money(t[1].rev)}</span></div><div class="bar" style="margin-top:6px"><i style="width:${Math.round(t[1].rev/maxRev*100)}%"></i></div></div><span class="muted small" style="flex:none">${t[1].units} sold</span></div>`).join('')}</div></div>
      <div class="card"><div class="card-h"><h3>Activity Feed</h3></div><div class="card-b">${db.activities.slice(0,5).map(a=>`<div class="act-item"><span class="act-ico"><i class="fa-solid ${a.icon}"></i></span><div><p>${esc(a.text)}</p><time>${timeAgo(a.time)}</time></div></div>`).join('')}</div></div>
    </div>
  </div>`;
}
function mountDashboard(){
  animateKpis();
  $$('#view [data-goto]').forEach(b=>b.onclick=()=>navigate(b.dataset.goto));
  $$('#view [data-openorder]').forEach(tr=>tr.onclick=()=>openOrderView(tr.dataset.openorder));
  const {rev,ord}=monthAgg();const cat=catSales();
  charts.push(new Chart($('#chRev'),{type:'bar',data:{labels:last12Labels(),datasets:[
    {type:'line',label:'Revenue',data:rev.map(v=>Math.round(v)),borderColor:'#6366f1',backgroundColor:c=>grad(c,'99,102,241'),fill:true,tension:.4,borderWidth:2.5,pointRadius:3,pointBackgroundColor:'#6366f1',yAxisID:'y'},
    {type:'bar',label:'Orders',data:ord,backgroundColor:'rgba(139,92,246,.28)',hoverBackgroundColor:'rgba(139,92,246,.55)',borderRadius:6,yAxisID:'y1',maxBarThickness:22}]},
    options:{maintainAspectRatio:false,interaction:{mode:'index',intersect:false},plugins:{legend:{labels:{usePointStyle:true,boxWidth:7}},tooltip:{callbacks:{label:c=>` ${c.dataset.label}: ${c.dataset.type==='line'?money2(c.parsed.y):c.parsed.y}`}}},scales:{y:{grid:{color:gridC()},ticks:{callback:v=>'$'+(v>=1000?(v/1000)+'k':v)}},y1:{position:'right',grid:{drawOnChartArea:false},ticks:{precision:0}},x:{grid:{display:false}}}}}));
  charts.push(new Chart($('#chCat'),{type:'doughnut',data:{labels:Object.keys(cat),datasets:[{data:Object.values(cat).map(v=>Math.round(v)),backgroundColor:PALETTE,borderWidth:0,hoverOffset:8}]},options:{maintainAspectRatio:false,cutout:'68%',plugins:{legend:{position:'bottom',labels:{usePointStyle:true,boxWidth:7,padding:14}},tooltip:{callbacks:{label:c=>` ${c.label}: ${money2(c.parsed)}`}}}}}));
}

/* ----- Orders ----- */
function viewOrders(){
  const tabs=`<div class="tabs"><button class="tab ${ordersTab==='orders'?'on':''}" data-otab="orders"><i class="fa-solid fa-cart-shopping"></i> Orders</button><button class="tab ${ordersTab==='quotes'?'on':''}" data-otab="quotes"><i class="fa-solid fa-file-lines"></i> Quotes</button></div>`;
  return tabs+(ordersTab==='quotes'?viewQuotes():viewOrdersList());
}
function viewOrdersList(){
  const c=ctl.orders;
  const counts={};['Pending','Processing','Shipped','Delivered','Cancelled'].forEach(s=>counts[s]=db.orders.filter(o=>o.status===s).length);
  const chips=['all',...Object.keys(counts)].map(s=>`<button class="chip ${c.status===s?'on':''}" data-ochip="${s}">${s==='all'?'All':s}<span>${s==='all'?db.orders.length:counts[s]}</span></button>`).join('');
  let list=[...db.orders];
  if(c.status!=='all')list=list.filter(o=>o.status===c.status);
  if(c.q){const q=c.q.toLowerCase();list=list.filter(o=>o.id.toLowerCase().includes(q)||o.customerName.toLowerCase().includes(q)||o.items.some(i=>i.name.toLowerCase().includes(q)));}
  list.sort((a,b)=>{let x=a[c.sortKey],y=b[c.sortKey];if(c.sortKey==='total'){x=+x;y=+y;}else{x=String(x).toLowerCase();y=String(y).toLowerCase();}return (x<y?-1:x>y?1:0)*c.sortDir;});
  const {slice,pages,total}=paginate(list,c);
  const ico=k=>c.sortKey===k?`<i class="fa-solid fa-caret-${c.sortDir===1?'up':'down'}"></i>`:'';
  const rows=slice.map(o=>`<tr><td class="mono strong">${o.id}</td><td>${avatar(o.customerName,26)} ${esc(o.customerName)}</td><td class="muted">${fmtDate(o.date)}</td><td class="muted">${o.items.reduce((s,i)=>s+i.qty,0)} items</td><td class="strong">${money2(o.total)}</td><td>${badge(o.status)}</td><td>${badge(payStatus(o))}${orderPaid(o)>0?`<span class="muted small" style="margin-left:6px" title="Paid ${money2(orderPaid(o))} of ${money2(orderNet(o))}">${money2(orderPaid(o))}</span>`:''}</td><td><div class="row-actions"><button class="icon-btn sm" data-oact="view" data-id="${o.id}" title="View"><i class="fa-regular fa-eye"></i></button>${can('edit')?`<button class="icon-btn sm" data-oact="edit" data-id="${o.id}" title="Edit"><i class="fa-regular fa-pen-to-square"></i></button>`:''}${can('create')?`<button class="icon-btn sm" data-oact="dup" data-id="${o.id}" title="Duplicate"><i class="fa-regular fa-copy"></i></button>`:''}${can('delete')?`<button class="icon-btn sm danger" data-oact="del" data-id="${o.id}" title="Delete"><i class="fa-regular fa-trash-can"></i></button>`:''}</div></td></tr>`).join('');
  return `<div class="toolbar"><div class="chips">${chips}</div><div class="toolbar-right">${can('export')?`<button class="btn btn-ghost btn-sm" id="btnOrdersExport"><i class="fa-solid fa-download"></i> Export CSV</button>`:''}${can('create')?`<button class="btn btn-primary btn-sm" id="btnOrdersNew"><i class="fa-solid fa-plus"></i> New Order</button>`:''}</div></div>
  <div class="card"><div class="toolbar inner"><div class="search-box"><i class="fa-solid fa-magnifying-glass"></i><input id="ordersSearch" placeholder="Search by ID, customer or product…" value="${esc(c.q)}"></div><div class="spacer"></div><span class="muted small">${total} order${total===1?'':'s'}</span></div>
  <div class="tbl-wrap"><table class="tbl"><thead><tr><th data-osort="id">Order ${ico('id')}</th><th data-osort="customerName">Customer ${ico('customerName')}</th><th data-osort="date">Date ${ico('date')}</th><th>Items</th><th data-osort="total">Total ${ico('total')}</th><th data-osort="status">Status ${ico('status')}</th><th>Payment</th><th></th></tr></thead><tbody>${rows}</tbody></table>${slice.length?'':'<div class="empty"><i class="fa-regular fa-folder-open"></i>No orders match your filters</div>'}</div>${pagerHtml(c,pages,total)}</div>`;
}
function viewQuotes(){
  const c=ctl.quotes;
  let list=[...db.quotes];
  if(c.q){const q=c.q.toLowerCase();list=list.filter(x=>x.id.toLowerCase().includes(q)||x.customerName.toLowerCase().includes(q)||x.items.some(i=>i.name.toLowerCase().includes(q)));}
  list.sort((a,b)=>new Date(b.date)-new Date(a.date));
  const {slice,pages,total}=paginate(list,c);
  const rows=slice.map(q=>`<tr><td class="mono strong">${q.id}</td><td>${avatar(q.customerName,26)} ${esc(q.customerName)}</td><td class="muted">${fmtDate(q.date)}</td><td class="muted">${q.items.reduce((s,i)=>s+i.qty,0)} items</td><td class="strong">${money2(q.total)}</td><td>${badge(q.status)}</td><td><div class="row-actions"><button class="icon-btn sm" data-qact="view" data-id="${q.id}" title="View"><i class="fa-regular fa-eye"></i></button>${q.status!=='Converted'&&can('create')?`<button class="btn btn-primary btn-xs" data-qact="convert" data-id="${q.id}"><i class="fa-solid fa-arrow-right"></i> Convert</button>`:''}${can('delete')?`<button class="icon-btn sm danger" data-qact="del" data-id="${q.id}"><i class="fa-regular fa-trash-can"></i></button>`:''}</div></td></tr>`).join('');
  return `<div class="toolbar"><div class="search-box"><i class="fa-solid fa-magnifying-glass"></i><input id="quoteSearch" placeholder="Search quotes by ID, customer or product…" value="${esc(c.q)}"></div><div class="spacer"></div><span class="muted small">${total} quote${total===1?'':'s'}</span>${can('create')?`<button class="btn btn-primary btn-sm" id="btnQuoteNew"><i class="fa-solid fa-plus"></i> New Quote</button>`:''}</div>
  <div class="card"><div class="tbl-wrap"><table class="tbl"><thead><tr><th>Quote</th><th>Customer</th><th>Date</th><th>Items</th><th>Total</th><th>Status</th><th></th></tr></thead><tbody>${rows}</tbody></table>${slice.length?'':'<div class="empty"><i class="fa-regular fa-folder-open"></i>No quotes found</div>'}</div>${pagerHtml(c,pages,total)}</div>`;
}
function mountOrders(){
  const c=ctl.orders;
  $$('#view [data-otab]').forEach(b=>b.onclick=()=>{ordersTab=b.dataset.otab;rerender();});
  if(ordersTab==='quotes'){
    const qc=ctl.quotes;
    bindSearch('#quoteSearch',qc);restoreFocus(qc,'#quoteSearch');bindPager(qc);
    $$('#view [data-qact]').forEach(b=>b.onclick=()=>{const q=db.quotes.find(x=>x.id===b.dataset.id);const a=b.dataset.qact;
      if(a==='view')openQuoteView(q.id);
      if(a==='convert'){if(!guard('create'))return;convertQuote(q.id);save();rerender();}
      if(a==='del')confirmModal('Delete quote',`Delete ${q.id}?`,()=>{db.quotes.splice(db.quotes.indexOf(q),1);audit('fa-trash','Quote deleted','Quotes',q.id);save();toast('Quote deleted','fa-trash','warn');rerender();});});
    const nq=$('#btnQuoteNew');if(nq)nq.onclick=()=>openOrderModal(null,'quote');
    return;
  }
