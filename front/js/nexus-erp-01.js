'use strict';
/* ================= HELPERS ================= */
const $=(s,r=document)=>r.querySelector(s);
const $$=(s,r=document)=>[...r.querySelectorAll(s)];
const esc=v=>String(v??'').replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
const rnd=(a,b)=>a+Math.random()*(b-a);
const ri=(a,b)=>Math.floor(rnd(a,b+1));
const pick=a=>a[Math.floor(Math.random()*a.length)];
const money=n=>'$'+Math.round(Number(n||0)).toLocaleString('en-US');
const money2=n=>'$'+Number(n||0).toLocaleString('en-US',{minimumFractionDigits:2,maximumFractionDigits:2});
const fmtDate=d=>new Date(d).toLocaleDateString('en-US',{month:'short',day:'numeric',year:'numeric'});
const fmtDT=d=>fmtDate(d)+' · '+new Date(d).toLocaleTimeString('en-US',{hour:'numeric',minute:'2-digit'});
const daysAgoISO=d=>new Date(Date.now()-d*864e5).toISOString();
const hoursAgoISO=h=>new Date(Date.now()-h*36e5).toISOString();
const gridC=()=>getComputedStyle(document.documentElement).getPropertyValue('--chart-grid').trim();
function timeAgo(iso){const s=(Date.now()-new Date(iso))/1e3;if(s<60)return'just now';if(s<3600)return Math.floor(s/60)+'m ago';if(s<86400)return Math.floor(s/3600)+'h ago';if(s<604800)return Math.floor(s/86400)+'d ago';return fmtDate(iso);}
function avatar(name,size=32){const h=[...name].reduce((a,c)=>a+c.charCodeAt(0),0)%360;const ini=name.split(/\s+/).map(w=>w[0]).slice(0,2).join('').toUpperCase();return `<span class="avatar" style="width:${size}px;height:${size}px;font-size:${Math.round(size*.36)}px;background:hsl(${h} 70% 45% / .16);color:hsl(${h} 72% 45%)">${esc(ini)}</span>`;}
const STYLE={'Pending':'b-amber','Processing':'b-blue','Shipped':'b-violet','Delivered':'b-green','Cancelled':'b-red','Paid':'b-green','Partially Paid':'b-amber','Unpaid':'b-gray','Overdue':'b-red','Active':'b-green','On Leave':'b-amber','Approved':'b-blue','In Transit':'b-violet','Received':'b-green','Out of Stock':'b-red','Low Stock':'b-amber','In Stock':'b-green','VIP':'b-gold','Standard':'b-gray','New':'b-blue','Draft':'b-gray','Sent':'b-blue','Converted':'b-violet','In Service':'b-green','Disposed':'b-red','Sales':'b-violet','Return':'b-red','Order Edit':'b-blue','Order Deleted':'b-red','Engineering':'b-blue','Finance':'b-green','Human Resources':'b-gold','Support':'b-pink','Marketing':'b-pink','Operations':'b-gray'};
const badge=t=>`<span class="badge ${STYLE[t]||'b-gray'}">${esc(t)}</span>`;
const FLAGS={'United States':'🇺🇸','United Kingdom':'🇬🇧','Mexico':'🇲🇽','Singapore':'🇸🇬','Germany':'🇩🇪','Canada':'🇨🇦','Australia':'🇦🇺','South Korea':'🇰🇷','United Arab Emirates':'🇦🇪','Sweden':'🇸🇪','China':'🇨🇳'};
function pct(cur,prev){return prev>0?((cur-prev)/prev*100):null;}
function deltaHtml(p){if(p===null)return'<span class="vs">— vs last month</span>';const up=p>=0;return `<span class="kpi-delta ${up?'up':'down'}"><i class="fa-solid fa-arrow-trend-${up?'up':'down'}"></i>${Math.abs(p).toFixed(1)}%</span><span class="vs">vs last month</span>`;}
function spark(vals,color){if(!vals||vals.length<2)return'';const w=96,h=30,min=Math.min(...vals),max=Math.max(...vals),rg=max-min||1;const pts=vals.map((v,i)=>`${(i/(vals.length-1))*w},${(h-((v-min)/rg)*(h-6)-3).toFixed(1)}`).join(' ');return `<svg class="spark" viewBox="0 0 ${w} ${h}"><polyline points="${pts}" fill="none" stroke="${color}" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"/></svg>`;}
function stars(r){let s='';for(let i=1;i<=5;i++)s+=`<i class="fa-${i<=Math.round(r)?'solid':'regular'} fa-star"></i>`;return `<span class="stars">${s}<em>${Number(r).toFixed(1)}</em></span>`;}
function download(name,mime,content){const b=new Blob([content],{type:mime});const a=document.createElement('a');a.href=URL.createObjectURL(b);a.download=name;a.click();setTimeout(()=>URL.revokeObjectURL(a.href),500);}
function exportCSV(name,headers,rows){const csv=[headers,...rows].map(r=>r.map(c=>`"${String(c??'').replace(/"/g,'""')}"`).join(',')).join('\n');download(name,'text/csv','\ufeff'+csv);}

/* ================= SEED ================= */
const LS_KEY='nexus_erp_v1';
function seed(){
  const P=[['Aurora Wireless Headphones','Electronics',129.99,72.5,86,20,'TechSource Ltd'],['Nimbus Mechanical Keyboard','Electronics',89.5,48,54,15,'TechSource Ltd'],['Vertex 4K Monitor 27"','Electronics',349,238,32,10,'Shenzhen Nova'],['Pulse Smart Speaker','Electronics',59.99,31,8,15,'Shenzhen Nova'],['Glide Wireless Mouse','Accessories',24.99,11,210,40,'Shenzhen Nova'],['Hyper USB-C Hub 8-in-1','Accessories',49.99,26,14,25,'Pacific Trade Co'],['Titan Laptop Stand','Accessories',39.99,19,96,20,'Pacific Trade Co'],['Flux 65W GaN Charger','Electronics',45,22,0,30,'Shenzhen Nova'],['Echo Webcam Pro 1080p','Electronics',79,44,61,15,'TechSource Ltd'],['Atlas Ergonomic Chair','Furniture',289,168,27,8,'EuroSupply GmbH'],['Summit Standing Desk','Furniture',549,342,11,12,'EuroSupply GmbH'],['Halo LED Desk Lamp','Office',34.99,15,142,30,'Nordic Parts AB'],['Orbit Notebook Set (x3)','Office',18.5,7.4,315,60,'Nordic Parts AB'],['Stride Backpack 25L','Accessories',68,36,73,18,'Pacific Trade Co']];
  const pre={Electronics:'EL',Accessories:'AC',Furniture:'FU',Office:'OF'};
  const products=P.map((d,i)=>({id:'PRD-'+(1001+i),sku:pre[d[1]]+'-'+(1001+i),name:d[0],category:d[1],price:d[2],cost:d[3],stock:d[4],reorder:d[5],supplier:d[6]}));
  const C=[['Acme Corporation','Sarah Mitchell','United States','VIP',430],['Globex Industries','James Carter','United Kingdom','VIP',380],['Initech Solutions','Maria Gonzalez','Mexico','Standard',300],['Umbrella Retail Group','Chen Wei','Singapore','Standard',260],['Stark Trading GmbH','Emma Wilson','Germany','VIP',230],['Wayne Enterprises','Liam Brown','Canada','Standard',190],['Cyberdyne Systems','Olivia Davis','Australia','New',90],['Hooli Labs','Noah Martinez','United States','Standard',150],['Vandelay Imports','Sophia Lee','South Korea','New',45],['Pinnacle Group','Ethan Taylor','United Arab Emirates','New',12]];
  const customers=C.map((d,i)=>{const terms=['Net 30','Net 15','Net 60','Due on Receipt','Net 45'][i%5];return {id:'CUS-'+(501+i),company:d[0],name:d[1],email:d[1].toLowerCase().replace(/[^a-z]+/g,'.')+'@'+d[0].toLowerCase().replace(/[^a-z]+/g,'')+'.com',phone:'+1 ('+ri(200,989)+') '+ri(200,989)+'-'+ri(1000,9999),country:d[2],tier:d[3],since:daysAgoISO(d[4]),terms,creditLimit:d[3]==='VIP'?100000:(d[3]==='Standard'?50000:25000),hold:false};});
  const E=[['Daniel Reyes','Sales Director','Sales',92000,'Active',700],['Priya Sharma','Lead Engineer','Engineering',118000,'Active',540],['Tom Becker','Senior Accountant','Finance',68000,'Active',820],['Lucia Fernandez','HR Manager','Human Resources',74000,'On Leave',610],["Kevin O'Neill",'Support Lead','Support',56000,'Active',450],['Aisha Bello','Marketing Specialist','Marketing',61000,'Active',300],['Marcus Chen','DevOps Engineer','Engineering',105000,'Active',260],['Elena Petrova','Sales Executive','Sales',52000,'Active',180],['Ryan Foster','Warehouse Supervisor','Operations',48000,'Active',390],['Maya Kaplan','Product Manager','Engineering',99000,'Active',150]];
  const employees=E.map((d,i)=>({id:'EMP-'+(201+i),name:d[0],role:d[1],dept:d[2],email:d[0].toLowerCase().replace(/[^a-z]+/g,'.')+'@nexuserp.io',salary:d[3],status:d[4],hired:daysAgoISO(d[5])}));
  const S=[['TechSource Ltd','David Kim','South Korea',4.6],['Shenzhen Nova','Lin Zhang','China',4.2],['EuroSupply GmbH','Anna Weber','Germany',4.8],['Pacific Trade Co','Ryan Tan','Singapore',4.0],['Nordic Parts AB','Freja Larsen','Sweden',4.4],['Atlas Components','Omar Haddad','United Arab Emirates',3.9]];
  const suppliers=S.map((d,i)=>({id:'SUP-'+(301+i),name:d[0],contact:d[1],country:d[2],rating:d[3],email:'sales@'+d[0].toLowerCase().replace(/[^a-z]+/g,'')+'.com',phone:'+82 10-'+ri(1000,9999)+'-'+ri(1000,9999)}));
  const orders=[];
  for(let i=0;i<64;i++){
    const age=ri(0,355);
    const created=new Date(Date.now()-age*864e5-ri(0,20)*36e5);
    const c=pick(customers);const used=new Set();const items=[];
    const n=ri(1,3);
    for(let k=0;k<n;k++){let p=pick(products);while(used.has(p.id))p=pick(products);used.add(p.id);items.push({productId:p.id,name:p.name,qty:ri(1,8),price:p.price});}
    const sub=items.reduce((s,it)=>s+it.qty*it.price,0);const tax=sub*.08;const ship=sub>1000?0:25;
    let status;
    if(age>30)status=Math.random()<.82?'Delivered':(Math.random()<.4?'Cancelled':'Shipped');
    else if(age>12)status=pick(['Delivered','Shipped','Shipped','Processing']);
    else if(age>4)status=pick(['Shipped','Processing','Pending']);
    else status=pick(['Pending','Processing','Pending']);
    orders.push({customerId:c.id,customerName:c.company,items,sub,tax,ship,total:sub+tax+ship,status,date:created.toISOString(),payments:[]});
  }
  orders.sort((a,b)=>new Date(a.date)-new Date(b.date));
  orders.forEach((o,i)=>o.id='ORD-'+(1001+i));
  orders.reverse();
  orders.filter(o=>o.status==='Delivered').slice(0,10).forEach((o,i)=>{
    if(Math.random()<.6){o.payments=[{id:'sp'+i,amount:o.total,method:pick(['Card','Bank Transfer','Cash']),date:daysAgoISO(ri(1,20))}];}
    else if(Math.random()<.5){o.payments=[{id:'sp'+i,amount:Math.round(o.total/2),method:'Card',date:daysAgoISO(ri(1,20))}];}
  });
  const quotes=[];
  for(let i=0;i<6;i++){
    const age=ri(1,45);
    const c=pick(customers);const used=new Set();const items=[];
    const n=ri(1,3);
    for(let k=0;k<n;k++){let p=pick(products);while(used.has(p.id))p=pick(products);used.add(p.id);items.push({productId:p.id,name:p.name,qty:ri(1,8),price:p.price});}
    const sub=items.reduce((s,it)=>s+it.qty*it.price,0);const tax=sub*.08;const ship=sub>1000?0:25;
    quotes.push({id:'QT-'+(2401+i),customerId:c.id,customerName:c.company,items,sub,tax,ship,total:sub+tax+ship,status:pick(['Draft','Sent','Approved']),date:new Date(Date.now()-age*864e5).toISOString()});
  }
  const invoices=[];
  const src=orders.filter(o=>o.status!=='Cancelled').slice(0,10);
  src.forEach((o,i)=>{const issued=new Date(o.date);const cust=customers.find(c=>c.id===o.customerId);const due=new Date(issued.getTime()+termsDays(cust?cust.terms:'Net 30')*864e5);const paid=Math.random()<.62;
    invoices.push({id:'INV-'+(2401+i),orderId:o.id,customerId:o.customerId,customerName:o.customerName,amount:Math.round(o.total),issued:issued.toISOString(),due:due.toISOString(),status:paid?'Paid':(due<new Date()?'Overdue':'Pending'),paidOn:paid?new Date(issued.getTime()+ri(3,25)*864e5).toISOString():null});o.invoiceId='INV-'+(2401+i);
    if(paid&&!o.payments.length)o.payments.push({id:'ip'+i,amount:o.total,method:'Bank Transfer',date:issued.toISOString()});});
  const PD=[[0,2,40,'Approved',8],[1,7,60,'In Transit',4],[2,9,15,'Pending',18],[3,4,120,'Received',-6],[4,11,200,'Pending',25],[5,13,80,'Approved',12]];
  const pos=PD.map((d,i)=>{const s=suppliers[d[0]],p=products[d[1]];return {id:'PO-'+(9001+i),supplierId:s.id,supplierName:s.name,productId:p.id,productName:p.name,qty:d[2],cost:p.cost,status:d[3],eta:daysAgoISO(-d[4]),created:daysAgoISO(ri(10,40))};});
  const lowP=products.filter(p=>p.stock<=p.reorder);
  const notifications=[];
  lowP.slice(0,3).forEach((p,i)=>notifications.push({id:'ntf'+i,icon:'fa-triangle-exclamation',title:p.stock===0?'Out of stock':'Low stock alert',desc:`${p.name} — ${p.stock} left (reorder at ${p.reorder})`,time:hoursAgoISO(1+i*3),read:false}));
  notifications.push({id:'ntf3',icon:'fa-cart-shopping',title:'New order received',desc:`${orders[0].id} · ${orders[0].customerName} · ${money(orders[0].total)}`,time:hoursAgoISO(4),read:false});
  const od=invoices.filter(i=>i.status==='Overdue').length;
  if(od)notifications.push({id:'ntf4',icon:'fa-clock',title:od+' invoice(s) overdue',desc:'Review outstanding receivables in Finance',time:hoursAgoISO(7),read:true});
  notifications.push({id:'ntf5',icon:'fa-file-invoice-dollar',title:'Payment received',desc:'INV-2401 settled by '+invoices[0].customerName,time:hoursAgoISO(2),read:true});
  const activities=[{icon:'fa-file-invoice-dollar',text:'Invoice INV-2401 paid by '+invoices[0].customerName,time:hoursAgoISO(2)},{icon:'fa-cart-shopping',text:`New order ${orders[0].id} from ${orders[0].customerName}`,time:hoursAgoISO(4)},{icon:'fa-truck',text:'PO-9002 shipped by Shenzhen Nova',time:hoursAgoISO(9)},{icon:'fa-user-plus',text:'New customer Pinnacle Group added',time:hoursAgoISO(26)},{icon:'fa-box',text:'Stock received: 120 × Glide Wireless Mouse',time:hoursAgoISO(31)},{icon:'fa-user-tie',text:'Elena Petrova completed onboarding',time:hoursAgoISO(50)},{icon:'fa-chart-line',text:'Quarterly sales report generated',time:hoursAgoISO(74)},{icon:'fa-tags',text:'Price updated on Vertex 4K Monitor 27"',time:hoursAgoISO(96)}].map((a,i)=>({id:'act'+i,...a}));
  const movements=[];
  const mv=[['PRD-1014',40,'PO Received','PO-9003'],['PRD-1006',-12,'Sale','ORD-1028'],['PRD-1003',-5,'Sale','ORD-1031'],['PRD-1001',60,'PO Received','PO-9001'],['PRD-1008',-3,'Manual Adjustment','']];
  mv.forEach((m,i)=>movements.push({id:'ms'+i,productId:m[0],productName:(products.find(p=>p.id===m[0])||{}).name||'Unknown',delta:m[1],reason:m[2],ref:m[3],by:'Alex Morgan',time:hoursAgoISO((i+1)*7)}));
  products.forEach((p,i)=>movements.push({id:'op'+i,productId:p.id,productName:p.name,delta:p.stock,reason:'Opening Stock',ref:'',by:'System',time:daysAgoISO(ri(60,330))}));
  movements.sort((a,b)=>new Date(b.time)-new Date(a.time));
  const audit=[];
  const au=[['fa-right-to-bracket','Signed in','Auth','admin@nexuserp.io'],['fa-cart-shopping','New order','Orders',orders[0].id+' · '+orders[0].customerName],['fa-file-invoice-dollar','Invoice paid','Finance','INV-2401'],['fa-box','Stock received','Inventory','120 × Glide Wireless Mouse'],['fa-file-lines','Quote created','Quotes','QT-2402'],['fa-gear','Settings updated','Settings','Tax rate set to 8%']];
  au.forEach((a,i)=>audit.push({id:'sd'+i,time:hoursAgoISO(2+i*9),user:'Alex Morgan',role:'Administrator',icon:a[0],action:a[1],module:a[2],target:a[3],detail:''}));
  const assets=[{id:'AST-001',name:'Dell PowerEdge Server',category:'IT Equipment',purchaseDate:daysAgoISO(760),cost:12500,salvage:1200,usefulLifeYears:5,status:'In Service',disposedOn:null},{id:'AST-002',name:'Ford Transit Delivery Van',category:'Vehicles',purchaseDate:daysAgoISO(620),cost:28500,salvage:5000,usefulLifeYears:6,status:'In Service',disposedOn:null},{id:'AST-003',name:'MacBook Pro 16" (Marketing)',category:'IT Equipment',purchaseDate:daysAgoISO(180),cost:3200,salvage:400,usefulLifeYears:4,status:'In Service',disposedOn:null},{id:'AST-004',name:'Office Furniture — HQ Floor 2',category:'Furniture',purchaseDate:daysAgoISO(1100),cost:9600,salvage:900,usefulLifeYears:8,status:'In Service',disposedOn:null}];
  return {version:2,profile:{name:'Alex Morgan',email:'alex.morgan@nexuserp.io',role:'Administrator'},config:{taxRate:8,shippingFee:25,freeShipOver:1000},budget:{cats:budgetCatsFromTotal(expSeries(rawRev(orders))[11])},prefs:{emailNotif:true,digest:false,lowStock:true},ui:{collapsed:false},theme:'light',seq:{order:1065,product:1015,cust:511,emp:211,sup:307,po:9007,inv:2411,quote:2407,asset:5},products,customers,employees,suppliers,orders,invoices,quotes,pos,notifications,activities,movements,audit,assets};
}
let db;
function save(){localStorage.setItem(LS_KEY,JSON.stringify(db));}
function load(){try{const raw=localStorage.getItem(LS_KEY);if(raw){const d=JSON.parse(raw);if(d&&Array.isArray(d.orders)&&(d.version===1||d.version===2)){if(!d.quotes)d.quotes=[];if(!d.movements)d.movements=[];if(!d.seq)d.seq={};if(!d.seq.quote)d.seq.quote=2407;if(!d.seq.asset)d.seq.asset=1;if(!d.audit)d.audit=[];if(!d.config)d.config={taxRate:8,shippingFee:25,freeShipOver:1000};if(!d.budget||!d.budget.cats)d.budget={cats:budgetCatsFromTotal(expSeries(rawRev(d.orders))[11])};if(!Array.isArray(d.orders[0].payments))d.orders.forEach(o=>o.payments=o.payments||[]);d.orders.forEach(o=>{if(!Array.isArray(o.returns))o.returns=[];if(o.discPct==null)o.discPct=0;});if(!Array.isArray(d.quotes[0]&&d.quotes[0].returns))d.quotes.forEach(q=>{if(q.discPct==null)q.discPct=0;});(d.customers||[]).forEach(c=>{if(c.terms==null)c.terms='Net 30';if(c.creditLimit==null)c.creditLimit=(c.tier==='VIP'?100000:(c.tier==='Standard'?50000:25000));if(c.hold==null)c.hold=false;});if(!d.assets||!Array.isArray(d.assets))d.assets=[];d.assets.forEach(a=>{if(a.salvage==null)a.salvage=0;if(a.usefulLifeYears==null)a.usefulLifeYears=5;if(a.status==null)a.status='In Service';if(a.disposedOn==null)a.disposedOn=null;});d.version=2;save();db=d;return;}}}catch(e){}db=seed();save();}

/* ================= STATE ================= */
let currentView='';
let procTab='po';
let ordersTab='orders';
let invTab='products';
let charts=[];
const destroyCharts=()=>{charts.forEach(c=>c.destroy());charts=[];};
const ctl={
  orders:{q:'',status:'all',sortKey:'date',sortDir:-1,page:1,per:8},
  quotes:{q:'',page:1,per:8},
  inventory:{q:'',cat:'all',page:1,per:10},
  movements:{q:'',page:1,per:10},
  customers:{q:'',tier:'all',page:1,per:8},
  hr:{q:'',dept:'all',page:1,per:8},
  assets:{q:'',cat:'all',page:1,per:10},
  audit:{q:'',module:'all',user:'all',page:1,per:12}
};
const rerender=()=>navigate(currentView);

/* ================= AGGREGATIONS ================= */
function monthAgg(){const rev=Array(12).fill(0),ord=Array(12).fill(0),units=Array(12).fill(0);const now=new Date();
  db.orders.forEach(o=>{if(o.status==='Cancelled')return;const d=new Date(o.date);const diff=(now.getFullYear()-d.getFullYear())*12+(now.getMonth()-d.getMonth());if(diff<0||diff>11)return;rev[11-diff]+=o.total;ord[11-diff]++;o.items.forEach(i=>units[11-diff]+=i.qty);});
  return {rev,ord,units};}
function last12Labels(){const a=[];for(let i=11;i>=0;i--){const d=new Date();d.setDate(1);d.setMonth(d.getMonth()-i);a.push(d.toLocaleDateString('en-US',{month:'short'}));}return a;}
function newCustMonths(){const a=Array(12).fill(0);const now=new Date();db.customers.forEach(c=>{const d=new Date(c.since);const diff=(now.getFullYear()-d.getFullYear())*12+(now.getMonth()-d.getMonth());if(diff>=0&&diff<12)a[11-diff]++;});return a;}
function catSales(){const m={};db.orders.filter(o=>o.status!=='Cancelled').forEach(o=>o.items.forEach(it=>{const p=db.products.find(x=>x.id===it.productId);const c=p?p.category:'Other';m[c]=(m[c]||0)+it.qty*it.price;}));return m;}
function topProducts(){const m={};db.orders.filter(o=>o.status!=='Cancelled').forEach(o=>o.items.forEach(it=>{m[it.name]=m[it.name]||{units:0,rev:0};m[it.name].units+=it.qty;m[it.name].rev+=it.qty*it.price;}));return Object.entries(m).sort((a,b)=>b[1].rev-a[1].rev);}
function expSeries(rev){return rev.map((v,i)=>Math.round(v*.58+420+(i%3)*260));}
const PALETTE=['#6366f1','#8b5cf6','#ec4899','#f59e0b','#10b981','#06b6d4','#f43f5e'];

/* ================= BUDGET VS ACTUAL / CASH FLOW ================= */
const EXP_RATIOS=[['Cost of Goods',.58],['Payroll',.22],['Marketing',.09],['Operations',.07],['Other',.04]];
function rawRev(orders){const rev=Array(12).fill(0);const now=new Date();orders.forEach(o=>{if(o.status==='Cancelled')return;const d=new Date(o.date);const diff=(now.getFullYear()-d.getFullYear())*12+(now.getMonth()-d.getMonth());if(diff<0||diff>11)return;rev[11-diff]+=o.total;});return rev;}
function budgetCatsDefault(){return budgetCatsFromTotal(expSeries(rawRev(db.orders))[11]);}
function budgetCatsFromTotal(tot){const o={};EXP_RATIOS.forEach(([k,r])=>o[k]=Math.max(0,Math.round(tot*r/10)*10));return o;}
function catBudget(cat){return (db.budget&&db.budget.cats&&db.budget.cats[cat])||0;}
function catActualSeries(cat){const r=(EXP_RATIOS.find(x=>x[0]===cat)||[])[1]||0;return expSeries(monthAgg().rev).map(v=>v*r);}
function catActual(cat){return catActualSeries(cat)[11];}
function incomeMonth(y,m){return db.orders.reduce((s,o)=>(o.payments||[]).reduce((s2,p)=>{const d=new Date(p.date);return d&&!isNaN(d)&&d.getFullYear()===y&&d.getMonth()===m?s2+p.amount:s2;},s),0);}
function assetBuyMonth(y,m){return db.assets.filter(a=>{const d=new Date(a.purchaseDate);return d.getFullYear()===y&&d.getMonth()===m;}).reduce((s,a)=>s+a.cost,0);}
function cashFlowSeries(){const rev=monthAgg().rev;const labels=[],net=[];const now=new Date();for(let i=11;i>=0;i--){const d=new Date(now.getFullYear(),now.getMonth()-(11-i),1);labels.push(d.toLocaleDateString('en-US',{month:'short'}));net.push(incomeMonth(d.getFullYear(),d.getMonth())-expSeries(rev)[i]-assetBuyMonth(d.getFullYear(),d.getMonth()));}return {labels,net};}

/* ================= SHARED UI ================= */
function toast(msg,icon='fa-circle-check',type='success'){const el=document.createElement('div');el.className='toast '+type;el.innerHTML=`<i class="fa-solid ${icon}"></i><span>${esc(msg)}</span>`;$('#toasts').appendChild(el);setTimeout(()=>{el.classList.add('out');setTimeout(()=>el.remove(),300);},3200);}
function openModal(html,width=560){const root=$('#modalRoot');root.innerHTML=`<div class="modal-backdrop"></div><div class="modal" style="max-width:${width}px">${html}</div>`;root.classList.add('show');document.body.style.overflow='hidden';$('.modal-backdrop',root).onclick=closeModal;$$('.modal-close',root).forEach(b=>b.onclick=closeModal);}
function closeModal(){const root=$('#modalRoot');root.classList.remove('show');document.body.style.overflow='';setTimeout(()=>{if(!root.classList.contains('show'))root.innerHTML='';},200);}
function confirmModal(title,msg,onOk,okLabel='Delete'){openModal(`<div class="modal-head"><div class="confirm-ico"><i class="fa-solid fa-triangle-exclamation"></i></div><button class="icon-btn modal-close"><i class="fa-solid fa-xmark"></i></button></div><div class="modal-body"><h3 style="margin-bottom:6px">${esc(title)}</h3><p class="muted">${esc(msg)}</p></div><div class="modal-foot"><div class="spacer"></div><button class="btn btn-ghost modal-close">Cancel</button><button class="btn btn-danger" id="cfmOk">${esc(okLabel)}</button></div>`,440);$('#cfmOk').onclick=()=>{closeModal();onOk();};}
function addActivity(icon,text){db.activities.unshift({id:'a'+Date.now(),icon,text,time:new Date().toISOString()});db.activities=db.activities.slice(0,30);}
function audit(icon,action,module,target,detail){
  db.audit.unshift({id:'ad'+Date.now()+Math.floor(Math.random()*999),time:new Date().toISOString(),user:db.profile.name,role:db.profile.role,icon,action,module,target:target||'',detail:detail||''});
  db.audit=db.audit.slice(0,800);
}
function pushNotif(icon,title,desc){db.notifications.unshift({id:'n'+Date.now(),icon,title,desc,time:new Date().toISOString(),read:false});db.notifications=db.notifications.slice(0,20);updateNotifDot();}
function updateNotifDot(){const n=db.notifications.filter(x=>!x.read).length;const d=$('#notifDot');d.style.display=n?'flex':'none';d.textContent=n>9?'9+':n;}
function paginate(list,c){const pages=Math.max(1,Math.ceil(list.length/c.per));if(c.page>pages)c.page=pages;const start=(c.page-1)*c.per;return {slice:list.slice(start,start+c.per),pages,total:list.length};}
function pageNumbers(cur,tot){if(tot<=7)return Array.from({length:tot},(_,i)=>i+1);const s=[...new Set([1,2,tot-1,tot,cur-1,cur,cur+1])].filter(n=>n>=1&&n<=tot).sort((a,b)=>a-b);const out=[];let prev=0;s.forEach(n=>{if(n-prev>1)out.push('…');out.push(n);prev=n;});return out;}
function pagerHtml(c,pages,total){if(pages<=1)return `<div class="pager"><span class="muted small">${total} record${total===1?'':'s'}</span><span></span></div>`;
  return `<div class="pager"><span class="muted small">Showing ${(c.page-1)*c.per+1}–${Math.min(c.page*c.per,total)} of ${total}</span><div class="pager-btns"><button class="icon-btn sm" data-pg="-1" ${c.page<=1?'disabled':''}><i class="fa-solid fa-chevron-left"></i></button>${pageNumbers(c.page,pages).map(p=>p==='…'?'<span class="dots">…</span>':`<button class="page-n ${p===c.page?'on':''}" data-pgoto="${p}">${p}</button>`).join('')}<button class="icon-btn sm" data-pg="1" ${c.page>=pages?'disabled':''}><i class="fa-solid fa-chevron-right"></i></button></div></div>`;}
function bindPager(c){$$('#view [data-pg]').forEach(b=>b.onclick=()=>{c.page+=+b.dataset.pg;rerender();});$$('#view [data-pgoto]').forEach(b=>b.onclick=()=>{c.page=+b.dataset.pgoto;rerender();});}
function bindSearch(sel,c){const el=$(sel);if(!el)return;let t;el.addEventListener('input',()=>{c.q=el.value;clearTimeout(t);t=setTimeout(()=>{c._focus=true;c.page=1;rerender();},350);});el.addEventListener('keydown',e=>{if(e.key==='Enter'){e.preventDefault();clearTimeout(t);c.q=el.value;c._focus=true;c.page=1;rerender();}});}
function restoreFocus(c,sel){if(c._focus){const el=$(sel);if(el){el.focus();const v=el.value;el.value='';el.value=v;}c._focus=false;}}
function animateKpis(){$$('#view .kpi-val[data-num]').forEach(el=>{const target=parseFloat(el.dataset.num),fmt=el.dataset.fmt,t0=performance.now(),dur=750;function step(t){const p=Math.min(1,(t-t0)/dur),e=1-Math.pow(1-p,3),v=target*e;el.textContent=fmt==='money'?money(v):Math.round(v).toLocaleString('en-US');if(p<1)requestAnimationFrame(step);}requestAnimationFrame(step);});}
function applyStock(o,dir,reason,ref){o.items.forEach(it=>{const p=db.products.find(x=>x.id===it.productId);if(p){p.stock=Math.max(0,p.stock+dir*it.qty);logMovement(p.id,dir*it.qty,reason||'Adjustment',ref||o.id);}});}
function changeStatus(o,ns){if(o.status===ns)return;if(ns==='Cancelled'&&o.status!=='Cancelled')applyStock(o,1,'Cancellation',o.id);if(ns!=='Cancelled'&&o.status==='Cancelled')applyStock(o,-1,'Sale',o.id);o.status=ns;}

/* ================= PERMISSIONS ================= */
const ROLE_RANK={Viewer:1,Analyst:2,Manager:3,Administrator:4};
const ROLE_DESC={
  Viewer:'Read-only access across core modules',
  Analyst:'Read-only + exports across all modules',
  Manager:'Full business operations, no deletions',
  Administrator:'Complete access, including delete & reset'
};
const ROLE_MODULES={
  Viewer:['dashboard','orders','inventory','customers','procurement','reports'],
  Analyst:['dashboard','orders','inventory','customers','procurement','finance','assets','reports','settings'],
  Manager:['dashboard','orders','inventory','customers','procurement','finance','assets','hr','reports','settings','audit'],
  Administrator:['dashboard','orders','inventory','customers','procurement','finance','assets','hr','reports','settings','audit']
};
function can(p){return (ROLE_RANK[db.profile.role]||1)>=(p==='view'?1:p==='export'?2:p==='create'?3:p==='edit'?3:4);}
function guard(p,msg){if(!can(p)){toast(msg||'Insufficient permissions for this action','fa-lock','warn');return false;}return true;}
function canViewModule(v){return (ROLE_MODULES[db.profile.role]||[]).includes(v);}
function applyRoleUI(){
  $$('.nav-item').forEach(n=>n.style.display=canViewModule(n.dataset.view)?'':'none');
  const nb=$('#btnNewOrder');if(nb)nb.style.display=can('create')?'':'none';
  const sr=$('#sysRole');if(sr)sr.textContent='Signed in as '+db.profile.role;
  const sd=$('#sysRoleDesc');if(sd)sd.textContent=ROLE_DESC[db.profile.role]||'';
}

/* ================= AUTH ================= */
const DEMO_ACCOUNTS={
  'admin@nexuserp.io':   {pass:'admin123',   role:'Administrator', name:'Alex Morgan'},
  'manager@nexuserp.io': {pass:'manager123', role:'Manager',       name:'Priya Sharma'},
  'analyst@nexuserp.io': {pass:'analyst123', role:'Analyst',       name:'Tom Becker'},
  'viewer@nexuserp.io':  {pass:'viewer123',  role:'Viewer',        name:'Lucia Fernandez'}
