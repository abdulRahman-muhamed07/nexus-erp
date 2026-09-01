}
function openCustomerModal(x){
  const editing=!!x;
  openModal(`<div class="modal-head"><div><h3>${editing?'Edit Customer':'Add Customer'}</h3></div><button class="icon-btn modal-close"><i class="fa-solid fa-xmark"></i></button></div>
  <form id="cForm" class="modal-body">
    <div class="frow2"><label>Company<input id="cc-co" required value="${editing?esc(x.company):''}"></label><label>Contact Person<input id="cc-name" required value="${editing?esc(x.name):''}"></label></div>
    <div class="frow2"><label>Email<input type="email" id="cc-email" required value="${editing?esc(x.email):''}"></label><label>Phone<input id="cc-phone" value="${editing?esc(x.phone):''}"></label></div>
    <div class="frow2"><label>Country<select id="cc-country">${Object.keys(FLAGS).map(c=>`<option ${editing&&x.country===c?'selected':''}>${c}</option>`).join('')}</select></label><label>Tier<select id="cc-tier">${['VIP','Standard','New'].map(t=>`<option ${editing&&x.tier===t?'selected':''}>${t}</option>`).join('')}</select></label></div>
    <div class="frow2"><label>Payment Terms<select id="cc-terms">${Object.keys(TERMS_DAYS).map(t=>`<option ${editing&&x.terms===t?'selected':''}>${t}</option>`).join('')}</select></label><label>Credit Limit ($)<input type="number" min="0" step="100" id="cc-limit" placeholder="0 = no limit" value="${editing?(x.creditLimit||''):''}"></label></div>
    <label class="check"><input type="checkbox" id="cc-hold" ${editing&&x.hold?'checked':''}> <span>Place account on hold (blocks new orders)</span></label>
  </form>
  <div class="modal-foot"><div class="spacer"></div><button class="btn btn-ghost modal-close">Cancel</button><button class="btn btn-primary" id="cc-save"><i class="fa-solid fa-check"></i> ${editing?'Save Changes':'Add Customer'}</button></div>`,560);
  $('#cForm').onsubmit=e=>{e.preventDefault();$('#cc-save').click();};
  $('#cc-save').onclick=()=>{
    const data={company:$('#cc-co').value.trim(),name:$('#cc-name').value.trim(),email:$('#cc-email').value.trim(),phone:$('#cc-phone').value.trim()||'—',country:$('#cc-country').value,tier:$('#cc-tier').value,terms:$('#cc-terms').value,creditLimit:+$('#cc-limit').value||0,hold:$('#cc-hold').checked};
    if(!data.company||!data.name)return;
    if(editing){Object.assign(x,data);audit('fa-pen-to-square','Customer updated','Customers',data.company);}else{db.customers.push({id:'CUS-'+(db.seq.cust++),since:new Date().toISOString(),...data});addActivity('fa-user-plus','New customer '+data.company+' added');audit('fa-user-plus','Customer created','Customers',data.company,data.tier);}
    save();closeModal();toast(editing?'Customer updated':'Customer added');rerender();
  };
}
function openEmployeeModal(e){
  const editing=!!e;
  const depts=['Sales','Engineering','Finance','Human Resources','Operations','Support','Marketing'];
  openModal(`<div class="modal-head"><div><h3>${editing?'Edit Employee':'Add Employee'}</h3></div><button class="icon-btn modal-close"><i class="fa-solid fa-xmark"></i></button></div>
  <form id="eForm" class="modal-body">
    <div class="frow2"><label>Full Name<input id="ee-name" required value="${editing?esc(e.name):''}"></label><label>Role / Title<input id="ee-role" required value="${editing?esc(e.role):''}"></label></div>
    <div class="frow2"><label>Department<select id="ee-dept">${depts.map(d=>`<option ${editing&&e.dept===d?'selected':''}>${d}</option>`).join('')}</select></label><label>Status<select id="ee-status">${['Active','On Leave'].map(s=>`<option ${editing&&e.status===s?'selected':''}>${s}</option>`).join('')}</select></label></div>
    <div class="frow2"><label>Annual Salary ($)<input type="number" min="0" id="ee-salary" required value="${editing?e.salary:''}"></label><label>Hire Date<input type="date" id="ee-hired" value="${(editing?e.hired:new Date().toISOString()).slice(0,10)}"></label></div>
  </form>
  <div class="modal-foot"><div class="spacer"></div><button class="btn btn-ghost modal-close">Cancel</button><button class="btn btn-primary" id="ee-save"><i class="fa-solid fa-check"></i> ${editing?'Save Changes':'Add Employee'}</button></div>`,560);
  $('#eForm').onsubmit=e=>{e.preventDefault();$('#ee-save').click();};
  $('#ee-save').onclick=()=>{
    const data={name:$('#ee-name').value.trim(),role:$('#ee-role').value.trim(),dept:$('#ee-dept').value,status:$('#ee-status').value,salary:+$('#ee-salary').value,hired:new Date($('#ee-hired').value+'T12:00:00').toISOString(),email:($('#ee-name').value.trim().toLowerCase().replace(/[^a-z]+/g,'.')||'staff')+'@nexuserp.io'};
    if(!data.name||!data.role||isNaN(data.salary))return;
    if(editing){Object.assign(e,data);audit('fa-pen-to-square','Employee updated','HR',data.name);}else{db.employees.push({id:'EMP-'+(db.seq.emp++),...data});audit('fa-user-plus','Employee created','HR',data.name,data.dept);}
    save();closeModal();toast(editing?'Employee updated':'Employee added');rerender();
  };
}
function openSupplierModal(s){
  const editing=!!s;
  openModal(`<div class="modal-head"><div><h3>${editing?'Edit Supplier':'Add Supplier'}</h3></div><button class="icon-btn modal-close"><i class="fa-solid fa-xmark"></i></button></div>
  <form id="sForm" class="modal-body">
    <div class="frow2"><label>Company<input id="ss-name" required value="${editing?esc(s.name):''}"></label><label>Contact Person<input id="ss-contact" required value="${editing?esc(s.contact):''}"></label></div>
    <div class="frow2"><label>Email<input type="email" id="ss-email" required value="${editing?esc(s.email):''}"></label><label>Country<select id="ss-country">${Object.keys(FLAGS).map(c=>`<option ${editing&&s.country===c?'selected':''}>${c}</option>`).join('')}</select></label></div>
    <label>Rating (1–5)<input type="number" min="1" max="5" step="0.1" id="ss-rating" required value="${editing?s.rating:4}"></label>
  </form>
  <div class="modal-foot"><div class="spacer"></div><button class="btn btn-ghost modal-close">Cancel</button><button class="btn btn-primary" id="ss-save"><i class="fa-solid fa-check"></i> ${editing?'Save Changes':'Add Supplier'}</button></div>`,560);
  $('#sForm').onsubmit=e=>{e.preventDefault();$('#ss-save').click();};
  $('#ss-save').onclick=()=>{
    const data={name:$('#ss-name').value.trim(),contact:$('#ss-contact').value.trim(),email:$('#ss-email').value.trim(),country:$('#ss-country').value,rating:Math.max(1,Math.min(5,+$('#ss-rating').value))};
    if(!data.name)return;
    if(editing){Object.assign(s,data);audit('fa-pen-to-square','Supplier updated','Procurement',data.name);}else{db.suppliers.push({id:'SUP-'+(db.seq.sup++),...data});audit('fa-building','Supplier created','Procurement',data.name);}
    save();closeModal();toast(editing?'Supplier updated':'Supplier added');rerender();
  };
}
function openPoModal(){
  openModal(`<div class="modal-head"><div><h3>New Purchase Order</h3><p>Restock inventory from a supplier</p></div><button class="icon-btn modal-close"><i class="fa-solid fa-xmark"></i></button></div>
  <form id="poForm" class="modal-body">
    <label>Supplier<select id="po-sup">${db.suppliers.map(s=>`<option value="${s.id}">${esc(s.name)}</option>`).join('')}</select></label>
    <label>Product<select id="po-prod">${db.products.map(p=>`<option value="${p.id}">${esc(p.name)} (stock: ${p.stock})</option>`).join('')}</select></label>
    <div class="frow2"><label>Quantity<input type="number" min="1" id="po-qty" value="50"></label><label>Unit Cost ($)<input type="number" step="0.01" min="0" id="po-cost" value=""></label></div>
    <label>Expected Arrival<input type="date" id="po-eta" value="${new Date(Date.now()+14*864e5).toISOString().slice(0,10)}"></label>
  </form>
  <div class="modal-foot"><div class="spacer"></div><button class="btn btn-ghost modal-close">Cancel</button><button class="btn btn-primary" id="po-save"><i class="fa-solid fa-check"></i> Create PO</button></div>`,520);
  const syncCost=()=>{const p=db.products.find(x=>x.id===$('#po-prod').value);if(p)$('#po-cost').value=p.cost;};
  syncCost();$('#po-prod').onchange=syncCost;
  $('#poForm').onsubmit=e=>{e.preventDefault();$('#po-save').click();};
  $('#po-save').onclick=()=>{
    const sup=db.suppliers.find(s=>s.id===$('#po-sup').value),p=db.products.find(x=>x.id===$('#po-prod').value);
    const qty=Math.max(1,+$('#po-qty').value||1),cost=+$('#po-cost').value||p.cost;
    const poId='PO-'+(db.seq.po++);
    db.pos.unshift({id:poId,supplierId:sup.id,supplierName:sup.name,productId:p.id,productName:p.name,qty,cost,status:'Pending',eta:new Date($('#po-eta').value+'T12:00:00').toISOString(),created:new Date().toISOString()});
    addActivity('fa-file-lines','Purchase order created for '+qty+' × '+p.name);
    audit('fa-file-lines','PO created','Procurement',poId,`${qty} × ${p.name} from ${sup.name}`);
    save();closeModal();toast('Purchase order created');rerender();
  };
}
function openInvoiceModal(){
  openModal(`<div class="modal-head"><div><h3>New Invoice</h3></div><button class="icon-btn modal-close"><i class="fa-solid fa-xmark"></i></button></div>
  <form id="iForm" class="modal-body">
    <label>Customer<select id="ii-cust">${db.customers.map(c=>`<option value="${c.id}">${esc(c.company)}</option>`).join('')}</select></label>
    <div class="frow2"><label>Amount ($)<input type="number" min="1" step="0.01" id="ii-amt" required></label><label>Due Date<input type="date" id="ii-due" value="${new Date(Date.now()+30*864e5).toISOString().slice(0,10)}"></label></div>
  <div class="muted small" id="ii-credit"></div>
  </form>
  <div class="modal-foot"><div class="spacer"></div><button class="btn btn-ghost modal-close">Cancel</button><button class="btn btn-primary" id="ii-save"><i class="fa-solid fa-check"></i> Issue Invoice</button></div>`,480);
  $('#iForm').onsubmit=e=>{e.preventDefault();$('#ii-save').click();};
  const hint=$('#ii-credit');const refresh=()=>{const c=db.customers.find(x=>x.id===$('#ii-cust').value);if(!c){hint.innerHTML='';return;}const st=creditState(c);hint.innerHTML=`<i class="fa-solid fa-circle-info"></i> ${esc(c.terms)} · Credit <b>${money(st.used)}</b> of <b>${money(st.limit)}</b> used`;};$('#ii-cust').onchange=refresh;refresh();
  $('#ii-save').onclick=()=>{
    const c=db.customers.find(x=>x.id===$('#ii-cust').value);const amt=+$('#ii-amt').value;
    if(!amt||amt<=0){toast('Enter a valid amount','fa-triangle-exclamation','warn');return;}
    db.invoices.unshift({id:'INV-'+(db.seq.inv++),customerId:c.id,customerName:c.company,amount:Math.round(amt),issued:new Date().toISOString(),due:new Date($('#ii-due').value+'T12:00:00').toISOString(),status:'Pending',paidOn:null});
    audit('fa-file-invoice-dollar','Invoice issued','Finance','INV-'+(db.seq.inv),`${money2(amt)} · ${c.company}`);
    save();closeModal();toast('Invoice issued');rerender();
  };
}

/* ================= VIEWS REGISTRY ================= */
const VIEWS={
  dashboard:{title:'Dashboard',sub:'Business overview & key performance',render:viewDashboard,mount:mountDashboard},
  orders:{title:'Sales Orders',sub:'Create, track and manage customer orders',render:viewOrders,mount:mountOrders},
  inventory:{title:'Inventory',sub:'Stock levels, valuation and adjustments',render:viewInventory,mount:mountInventory},
  customers:{title:'Customers',sub:'CRM — accounts and lifetime value',render:viewCustomers,mount:mountCustomers},
  hr:{title:'Human Resources',sub:'Team, departments and payroll',render:viewHR,mount:mountHR},
  procurement:{title:'Procurement',sub:'Purchase orders and suppliers',render:viewProcurement,mount:mountProcurement},
  finance:{title:'Finance',sub:'Revenue, expenses and receivables',render:viewFinance,mount:mountFinance},
  assets:{title:'Fixed Assets',sub:'Asset register and depreciation schedules',render:viewAssets,mount:mountAssets},
  reports:{title:'Reports',sub:'Analytics and data exports',render:viewReports,mount:mountReports},
  audit:{title:'Audit Log',sub:'Searchable trail of every system action',render:viewAudit,mount:mountAudit},
  settings:{title:'Settings',sub:'Profile, preferences and data',render:viewSettings,mount:mountSettings}
};
function navigate(v){
  if(!VIEWS[v])return;
  if(!canViewModule(v)){toast(`Access denied — the ${db.profile.role} role cannot open ${VIEWS[v].title}`,'fa-lock','warn');v='dashboard';if(!canViewModule(v))return;}
  currentView=v;
  $$('.nav-item').forEach(n=>n.classList.toggle('active',n.dataset.view===v));
  destroyCharts();
  $('#viewTitle').textContent=VIEWS[v].title;
  $('#viewSub').textContent=VIEWS[v].sub;
  $('#view').innerHTML=VIEWS[v].render();
  VIEWS[v].mount();
  document.body.classList.remove('m-open');
  window.scrollTo(0,0);
}

/* ================= THEME / USER ================= */
function applyTheme(t,persist=true){
  document.documentElement.dataset.theme=t;
  db.theme=t;
  const btn=$('#btnTheme');if(btn)btn.innerHTML=t==='dark'?'<i class="fa-solid fa-sun"></i>':'<i class="fa-solid fa-moon"></i>';
  Chart.defaults.color=getComputedStyle(document.documentElement).getPropertyValue('--muted').trim();
  if(persist)save();
  if(currentView&&VIEWS[currentView])navigate(currentView);
}
function renderUserChip(){
  $('#tbName').textContent=db.profile.name;$('#tbRole').textContent=db.profile.role;
  $('#tbAvatar').textContent=db.profile.name.split(' ').map(w=>w[0]).join('').slice(0,2).toUpperCase();
  $('#upName').textContent=db.profile.name;$('#upEmail').textContent=db.profile.email;
}

/* ================= TOPBAR ================= */
function renderNotifList(){
  const list=db.notifications.map(n=>`<div class="notif-item ${n.read?'':'unread'}" data-nid="${n.id}"><span class="n-ico" style="background:rgba(99,102,241,.12);color:var(--primary)"><i class="fa-solid ${n.icon}"></i></span><div style="flex:1"><b>${esc(n.title)}</b><p>${esc(n.desc)}</p><time>${timeAgo(n.time)}</time></div>${n.read?'':'<span class="dot" style="position:static;margin-top:4px"></span>'}</div>`).join('');
  $('#notifPop').innerHTML=`<div class="pop-head"><b>Notifications</b><button id="markAll">Mark all read</button></div><div class="notif-list">${list||'<div class="empty">No notifications</div>'}</div>`;
  $$('#notifPop [data-nid]').forEach(el=>el.onclick=()=>{const n=db.notifications.find(x=>x.id===el.dataset.nid);n.read=true;save();updateNotifDot();renderNotifList();});
  $('#markAll').onclick=()=>{db.notifications.forEach(n=>n.read=true);save();updateNotifDot();renderNotifList();};
}
function bindTopbar(){
  $('#btnSidebar').onclick=()=>{if(window.innerWidth<=1080)document.body.classList.toggle('m-open');else{document.body.classList.toggle('collapsed');db.ui.collapsed=document.body.classList.contains('collapsed');save();}};
  $('#sbOverlay').onclick=()=>document.body.classList.remove('m-open');
  $$('.nav-item').forEach(n=>n.onclick=()=>navigate(n.dataset.view));
  $('#btnTheme').onclick=()=>applyTheme(db.theme==='dark'?'light':'dark');
  $('#btnNewOrder').onclick=()=>openOrderModal(null);
  $('#btnNotif').onclick=e=>{e.stopPropagation();$('#userPop').classList.remove('show');$('#notifPop').classList.toggle('show');renderNotifList();};
  $('#btnUser').onclick=e=>{e.stopPropagation();$('#notifPop').classList.remove('show');$('#userPop').classList.toggle('show');};
  $('#upSettings').onclick=()=>{$('#userPop').classList.remove('show');navigate('settings');};
  $('#upBackup').onclick=()=>{$('#userPop').classList.remove('show');download('nexus-erp-backup.json','application/json',JSON.stringify(db,null,2));toast('Backup downloaded');};
  $('#btnSignout').onclick=()=>{$('#userPop').classList.remove('show');showLogin();};
  $('#loginForm').onsubmit=submitLogin;
  $('#liRole').onchange=e=>fillRoleCreds(e.target.value);
  $$('#login [data-quick]').forEach(b=>b.onclick=()=>fillRoleCreds(b.dataset.quick));
  document.addEventListener('click',e=>{
    if(!e.target.closest('#notifPop')&&!e.target.closest('#btnNotif'))$('#notifPop').classList.remove('show');
    if(!e.target.closest('#userPop')&&!e.target.closest('#btnUser'))$('#userPop').classList.remove('show');
    if(!e.target.closest('.tb-search'))$('#searchPop').classList.remove('show');
  });
  document.addEventListener('keydown',e=>{
    if(e.key==='Escape'){closeModal();$('#searchPop').classList.remove('show');$('#notifPop').classList.remove('show');$('#userPop').classList.remove('show');}
    if(e.key==='/'&&!/INPUT|TEXTAREA|SELECT/.test(document.activeElement.tagName)){e.preventDefault();$('#globalSearch').focus();}
    if(e.ctrlKey&&e.key==='n'&&!/INPUT|TEXTAREA|SELECT/.test(document.activeElement.tagName)){e.preventDefault();if(guard('create'))openOrderModal(null);}
  });
  /* Global quick search */
  const gs=$('#globalSearch'),pop=$('#searchPop');
  gs.addEventListener('input',()=>{
    const q=gs.value.trim().toLowerCase();
    if(q.length<2){pop.classList.remove('show');return;}
    const o=db.orders.filter(x=>x.id.toLowerCase().includes(q)||x.customerName.toLowerCase().includes(q)).slice(0,4);
    const p=db.products.filter(x=>x.name.toLowerCase().includes(q)||x.sku.toLowerCase().includes(q)).slice(0,4);
    const c=db.customers.filter(x=>x.company.toLowerCase().includes(q)||x.name.toLowerCase().includes(q)).slice(0,4);
    const e=db.employees.filter(x=>x.name.toLowerCase().includes(q)).slice(0,3);
    if(!o.length&&!p.length&&!c.length&&!e.length){pop.innerHTML='<div class="sp-empty">No matches for “'+esc(gs.value)+'”</div>';pop.classList.add('show');return;}
    let html='';
    if(o.length)html+='<div class="sp-group">Orders</div>'+o.map(x=>`<button class="sp-item" data-go="orders" data-q="${esc(x.id)}"><i class="fa-solid fa-cart-shopping"></i><span>${esc(x.id)} · ${esc(x.customerName)}</span><em>${money(x.total)}</em></button>`).join('');
    if(p.length)html+='<div class="sp-group">Products</div>'+p.map(x=>`<button class="sp-item" data-go="inventory" data-q="${esc(x.name)}"><i class="fa-solid fa-box"></i><span>${esc(x.name)}</span><em>${x.stock} in stock</em></button>`).join('');
    if(c.length)html+='<div class="sp-group">Customers</div>'+c.map(x=>`<button class="sp-item" data-go="customers" data-q="${esc(x.company)}"><i class="fa-solid fa-users"></i><span>${esc(x.company)}</span><em>${esc(x.tier)}</em></button>`).join('');
    if(e.length)html+='<div class="sp-group">Employees</div>'+e.map(x=>`<button class="sp-item" data-go="hr" data-q="${esc(x.name)}"><i class="fa-solid fa-user-tie"></i><span>${esc(x.name)}</span><em>${esc(x.dept)}</em></button>`).join('');
    pop.innerHTML=html;pop.classList.add('show');
    $$('#searchPop [data-go]').forEach(b=>b.onclick=()=>{const v=b.dataset.go;ctl[v].q=b.dataset.q;ctl[v].page=1;if(v==='orders')ctl[v].status='all';gs.value='';pop.classList.remove('show');navigate(v);});
  });
}

/* ================= INIT ================= */
load();
if(db.ui&&db.ui.collapsed&&window.innerWidth>1080)document.body.classList.add('collapsed');
renderUserChip();
updateNotifDot();
applyRoleUI();
bindTopbar();
applyTheme(db.theme||'light',false);
navigate('dashboard');
if(!(db.session&&db.session.email)){fillRoleCreds('Administrator');$('#login').classList.remove('hidden');}else{$('#liEmail').value=db.session.email;}